using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IWT_OCR.Common
{
    public partial class frmCorpSelect : Form
    {
        public frmCorpSelect()
        {
            InitializeComponent();
        }

        string[] cArray = null;

        private void button2_Click(object sender, EventArgs e)
        {
            MyProperty = "";
            Close();
        }

        public string MyProperty { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0)
            {
                return;
            }

            MyProperty = cArray[listBox1.SelectedIndex];

            Close();
        }

        private void frmCorpSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void ShowDirectory(ListBox view)
        {
            string[] subFolders = System.IO.Directory.GetDirectories(Properties.Settings.Default.mstPath);

            foreach (var dir in subFolders)
            {
                view.Items.Add(System.IO.Path.GetFileName(dir));

                Array.Resize(ref cArray, view.Items.Count);
                cArray[view.Items.Count - 1] = dir;
            }
        }

        private void frmCorpSelect_Load(object sender, EventArgs e)
        {
            ShowDirectory(listBox1);
        }
    }
}
