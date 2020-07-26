using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IWT_OCR.OCR
{
    public partial class frmNgRecoverySub : Form
    {
        public frmNgRecoverySub(int cnt)
        {
            InitializeComponent();

            _cnt = cnt;
        }

        int _cnt = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            MyProperty = 0;

            Close();
        }

        private void frmNgRecoverySub_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string denName = "";

            if (radioButton1.Checked)
            {
                denName = "納品仮伝票";
                MyProperty = 1;
            }
            else if (radioButton2.Checked)
            {
                denName = "現品票";
                MyProperty = 3;
            }
            else if (radioButton3.Checked)
            {
                denName = "納品書";
                MyProperty = 2;
            }

            if (MessageBox.Show(_cnt + "件の画像を" + denName + "データとしてリカバリします。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                denName = "";
                MyProperty = 0;
            }

            Close();
        }

        public int MyProperty { get; set; }
    }
}
