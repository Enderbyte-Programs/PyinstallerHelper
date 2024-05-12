using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    public partial class VerboseCommandExecutorForm : Form
    {
        public string EFile;
        public string EArgs;
        private bool kill = false;
        public string Title;
        public int ExpectedLines;
        public VerboseCommandExecutorForm()
        {
            InitializeComponent();
        }
        private void OnInputRecv(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                richTextBox1.Text += "\n" + e.Data;
                if (progressBar1.Value == progressBar1.Maximum - 1)
                {
                    //Has exceeded expected lines. Switch to marquee
                    progressBar1.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    progressBar1.PerformStep();
                }
            });
        }

        private void RunExecute()
        {
            Process p = new Process();
            p.StartInfo.FileName = EFile;
            p.StartInfo.Arguments = EArgs;
            //MessageBox.Show(EFile + "\n" + EArgs);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += OnInputRecv;
            p.ErrorDataReceived += OnInputRecv;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            while (!p.HasExited)
            {
                if (kill)
                {
                    p.Kill();
                    MessageBox.Show("Command aborted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            var ec = p.ExitCode;
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = progressBar1.Maximum;
                if (ec != 0)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            });
        }

        private void VerboseCommandExecutorForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            progressBar1.Maximum = ExpectedLines;
            label1.Text = Title;
            richTextBox1.Text = $"{EFile} {EArgs}";
            new Thread(new ThreadStart(RunExecute)).Start();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.kill = true;
        }
    }
}
