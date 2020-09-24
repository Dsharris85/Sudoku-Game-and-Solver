using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuGUI
{
    public partial class HighScorePrompts : Form
    {
        Form1 _parent;

        public HighScorePrompts(Form1 par)
        {
            InitializeComponent();
            _parent = par;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string initials = this.textBox1.Text;
            _parent.Initials =initials;
            _parent.AddScoreToFile();
            _parent.Show();
            this.Close();
        }
    }
}
