using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    public partial class CompileForm : Form
    {
        public string ToBuild;
        public string tempdir;
        public bool abort = false;
        private bool running = false;
        public Stream ns = Stream.Null;
        private StreamReader nsr;
        private StreamWriter nsw;
        private string ToPut = "";
        public CompileForm()
        {
            nsr = new StreamReader(ns);
            nsw = new StreamWriter(ns);
            InitializeComponent();
        }

        private void CompileForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = ToBuild;
            this.MaximizeBox = false;//Messes up UI
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (running)
            {
                abort = true;
            }
            else
            {
                this.Close();
            }
        }
        private void DoTheDeed(string tempdir)
        {
            Process p = new Process();
            var slp = CLISplit.SplitCommandLine(ToBuild).ToList();
            p.StartInfo.FileName = "powershell.exe";
            p.StartInfo.Arguments = ToBuild+$" | tee {tempdir}\\compile.log";
            p.StartInfo.UseShellExecute = false;
            if (!checkBox1.Checked)
            {
                p.StartInfo.CreateNoWindow = true;
            } else
            {
                p.StartInfo.CreateNoWindow = false;
            }
            p.Start();
            p.WaitForExit();
            var ec = p.ExitCode;
            if (ec != 0)
            {
                MessageBox.Show("Error compiling: Please check log", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                MessageBox.Show("Completed successfully!");
            }
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            running = true;
            this.Text = "Compiling...";
            //TODO Compile code
            ToBuild = richTextBox1.Text;//Load any changes the user has made
            var ts = new ThreadStart(() => DoTheDeed(tempdir));
            ts += UIMarkEndInstall;
            Thread t = new Thread(ts) ;
            
            t.Start();

            
        }
        private void UIMarkEndInstall()
        {
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                Text = "Finished";
                progressBar1.Style = ProgressBarStyle.Continuous;
                running = false;
                button2.Enabled = true;
            });
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
