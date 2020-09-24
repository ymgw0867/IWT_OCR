using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWT_OCR.Common;

namespace IWT_OCR.OCR
{
    public partial class frmOCR_UriageShiire : Form
    {
        public frmOCR_UriageShiire(int _Scnt)
        {
            InitializeComponent();

            // 画像件数
            Scnt = _Scnt;
        }

        int Scnt = 0;

        private void frmOCR_UriageShiire_Load(object sender, EventArgs e)
        {
            label2.Text = Scnt.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("伝票種別を選択してください", "伝票種別未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MyProperty = comboBox1.SelectedIndex + 1;

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyProperty = global.flgOff;
            Close();
        }

        // 伝票種別
        public int MyProperty { get; set; }
    }
}
