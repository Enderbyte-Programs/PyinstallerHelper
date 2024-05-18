using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    public partial class TextBoxDisplay : Form
    {
        public string data;
        public TextBoxDisplay()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        private void TextBoxDisplay_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.ShowIcon = false;
            richTextBox1.Text = data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
