using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            button2.Enabled = false;
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            running = true;
            this.Text = "Compiling...";
            //TODO Compile code
            ToBuild = richTextBox1.Text;//Load any changes the user has made
            var ts = new ThreadStart(() => DoTheDeed(tempdir));
            ts += UIMarkEndInstall;
            Thread t = new Thread(ts);

            t.Start();
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
        private void OnInputRecv(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                richTextBox1.Text += "\n"+e.Data;
                progressBar1.PerformStep();
            });
        }
        private void DoTheDeed(string tempdir)
        {
            Process p = new Process();
            var slp = CLISplit.SplitCommandLine(ToBuild).ToList();
            p.StartInfo.FileName = "pyinstaller";
            p.StartInfo.Arguments = String.Join(" ",slp.GetRange(1,slp.Count-1));

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += OnInputRecv;
            p.ErrorDataReceived += OnInputRecv;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            
            while (!p.HasExited)
            {
                if (abort)
                {
                    p.Kill();
                    MessageBox.Show("Compilation aborted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar1.Value = progressBar1.Maximum;
            });
            
            var ec = p.ExitCode;
            if (ec != 0)
            {
                MessageBox.Show("Error compiling: Please check log", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                MessageBox.Show("Completed successfully!","Report",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
            
        }
        private void UIMarkEndInstall()
        {
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                File.WriteAllText(tempdir + "\\compile.log", richTextBox1.Text);
                Text = "Finished";
                progressBar1.Style = ProgressBarStyle.Continuous;
                running = false;
                button2.Enabled = true;
            });
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
