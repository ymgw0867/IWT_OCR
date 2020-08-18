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
    public partial class frmOCRResult : Form
    {
        public frmOCRResult(int img, int ok, int ng)
        {
            InitializeComponent();

            lblImg.Text = img.ToString();
            lblOK.Text = ok.ToString();
            lblNG.Text = ng.ToString();
        }

        //int _img = 0;
        //int _ok = 0;
        //int _ng = 0;

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void frmOCRResult_Load(object sender, EventArgs e)
        {
            if (Utility.StrtoInt(lblNG.Text) != global.flgOff)
            {
                label2.Text = "書式が一致しない画像がありました。" + Environment.NewLine + "ＮＧ画像確認画面で確認してください。";
            }
            else
            {
                label2.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmOCRResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 後片付け
            Dispose();
        }
    }
}
