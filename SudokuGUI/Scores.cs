using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuGUI
{
    public partial class Scores : Form
    {
        private Form1 _parent;
        public Scores(Form1 par)
        {
            InitializeComponent();
            _parent = par;
            LoadFileToList();
        }
        private void LoadFileToList()
        {
            List<string> lines = File.ReadAllLines(_parent.SaveFile).ToList();
            var sorted = lines.OrderBy(x => x.Substring(4,2)).Reverse();            

            foreach (string line in sorted)                 
            {
                string[] elements = line.Split(' ');
                ListViewItem item = new ListViewItem(elements[0].ToUpper());
                item.SubItems.Add(elements[1]);
                string time = $"{Int32.Parse(elements[2]) / 60}:{(Int32.Parse(elements[2]) % 60)}";
                item.SubItems.Add(time);
                item.SubItems.Add(elements[3]);

                this.listView1.Items.Add(item);
            }
        }
        private void Scores_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.Show();
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string text = listView1.SelectedItems[0].SubItems[3].Text;
            _parent.LoadGame(text);
            _parent.Show();
            this.Close();
        }
    }
}
