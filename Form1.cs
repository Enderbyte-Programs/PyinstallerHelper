using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Hide();
            textBox2.Hide();
            label8.Enabled = checkBox2.Checked;
            label9.Enabled = checkBox2.Checked;
            label10.Enabled = checkBox2.Checked;
            label11.Enabled = checkBox2.Checked;
            label12.Enabled = checkBox2.Checked;
            label13.Enabled = checkBox2.Checked;
            label14.Enabled = checkBox2.Checked;
            label15.Enabled = checkBox2.Checked;
            textBox4.Enabled = checkBox2.Checked;
            textBox5.Enabled = checkBox2.Checked;
            textBox6.Enabled = checkBox2.Checked;
            textBox7.Enabled = checkBox2.Checked;
            textBox8.Enabled = checkBox2.Checked;
            textBox9.Enabled = checkBox2.Checked;
            textBox10.Enabled = checkBox2.Checked;
            textBox11.Enabled = checkBox2.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.textBox3.Text = Constants.DistPath;
            
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompileForm cf = new CompileForm();
            string td = Routines.GenerateNewTempdir();
            try
            {
                
                cf.ToBuild = BuildCompileCommand(td);
                cf.tempdir = td;
            } catch
            {
                return;
            }
            cf.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Python Files|*.py|Windows Python Files|*.pyw|All Files|*.*";
            ofd.Title = "Choose a source Python file";
            
            ofd.ShowDialog();
            string nf = ofd.FileName;
            textBox1.Text = nf;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.Description = "Choose an output directory";

            ofd.ShowDialog();
            string nf = ofd.SelectedPath;
            textBox3.Text = nf;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Icon Files|*.ico|All Files|*.*";
            ofd.Title = "Choose an icon for the EXE";

            ofd.ShowDialog();
            string nf = ofd.FileName;
            textBox2.Text = nf;
        }
        public string BuildCompileCommand(string tempdir)
        {
            string command = $"pyinstaller --log-level=DEBUG --noconfirm --workpath={tempdir} --distpath=\"{textBox3.Text}\"";
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("You must specify a Python file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
            
            if (radioButton2.Checked)
            {
                command += " -w";
            }
            if (radioButton3.Checked)
            {
                command += " --icon=NONE";
            }
            if (radioButton5.Checked)
            {
                if (File.Exists(textBox2.Text))
                {
                    command += $" --icon=\"{textBox2.Text}\"";
                }
                else
                {
                    MessageBox.Show("Icon file not found.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    throw new Exception();
                }
            }
            if (checkBox1.Checked)
            {
                command += " --onefile";
            }
            if (!Directory.Exists(textBox3.Text))
            {
                MessageBox.Show("Output folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
            if (checkBox2.Checked)
            {
                var pxvfl = tempdir + "\\vf.txt";
                var ffn = Constants.VersionFileTemplate;
                ffn = ffn.Replace("#VERSION",Routines.PadVersionRight( textBox6.Text))
                    .Replace("#VERPSION",Routines.PadVersionRight(textBox11.Text))
                    .Replace("#VERSSION",Routines.PadVersionRight(textBox6.Text).Replace(".", ","))
                    .Replace("#VERSPSION",Routines.PadVersionRight(textBox11.Text).Replace(".", ","))
                    .Replace("#COMPANYNAME",textBox4.Text)
                    .Replace("#DESCR",textBox5.Text)
                    .Replace("#INAME",textBox7.Text)
                    .Replace("#CO",textBox8.Text)
                    .Replace("#OGF",textBox9.Text)
                    .Replace("#PNAME",textBox10.Text);
                File.WriteAllText(pxvfl, ffn);
                command += $" --version-file=\"{pxvfl}\"";
            }

            command += $" \"{textBox1.Text}\"";
            return command;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                button2.Show();
                textBox2.Show();
            } else
            {
                button2.Hide();
                textBox2.Hide();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = new Process();//Start self again
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            p.Start();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label8.Enabled = checkBox2.Checked;
            label9.Enabled = checkBox2.Checked;
            label10.Enabled = checkBox2.Checked;
            label11.Enabled = checkBox2.Checked;
            label12.Enabled = checkBox2.Checked;
            label13.Enabled = checkBox2.Checked;
            label14.Enabled = checkBox2.Checked;
            label15.Enabled = checkBox2.Checked;
            textBox4.Enabled = checkBox2.Checked;
            textBox5.Enabled = checkBox2.Checked;
            textBox6.Enabled = checkBox2.Checked;
            textBox7.Enabled = checkBox2.Checked;
            textBox8.Enabled = checkBox2.Checked;
            textBox9.Enabled = checkBox2.Checked;
            textBox10.Enabled = checkBox2.Checked;
            textBox11.Enabled = checkBox2.Checked;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox11.Text.Equals("") || textBox6.Text.Contains(textBox11.Text))
            {
                textBox11.Text = textBox6.Text;
            }
        }
    }
}
