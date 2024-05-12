using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    public partial class PyinstallerSetting : Form
    {
        public PyinstallerSetting()
        {
            InitializeComponent();
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose pyinstaller executable";
            ofd.Filter = "Executable|*.exe";
            ofd.Multiselect = false;
            ofd.ShowDialog();
            if (ofd.FileName == null || ofd.FileName.Length == 0)
            {
                return;
            }
            else
            {
                textBox1.Text += ofd.FileName;
            }
        }

        private void PyinstallerSetting_Load(object sender, EventArgs e)
        {
            textBox1.Text = Routines.GetFullPath("pyinstaller.exe");
            if (!Routines.ExistsOnPath("pyinstaller.exe"))
            {
                label3.Text = "Pyinstaller not found";
                label3.ForeColor = Color.Red;
            }
            else
            {
                Process p = new Process()
                {
                    StartInfo =
                    {
                        FileName = "pyinstaller.exe",
                        Arguments = "--version",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    },
                    
                };
                p.Start();
                p.WaitForExit();
                label3.Text = $"Pyinstaller Version: {p.StandardOutput.ReadToEnd()}{p.StandardError.ReadToEnd()}";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Routines.ExistsOnPath("py.exe"))
            {
                MessageBox.Show("Python could not be found! Please install Python or go to Settings -> Python to fix this!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                if (Routines.RunGUICommand("py.exe -m pip install pyinstaller --upgrade --break --no-color --no-input --yes", 300, "Upgrading Pyinstaller", this) == DialogResult.OK)
                {
                    MessageBox.Show("Successfully upgraded Pyinstaller");
                } else
                {
                    MessageBox.Show("Failed! Please try manually");
                }
            }
            PyinstallerSetting_Load(sender, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
