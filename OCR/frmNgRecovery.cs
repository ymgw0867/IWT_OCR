//using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using IWT_OCR.Common;

namespace IWT_OCR.OCR
{
    public partial class frmNgRecovery : Form
    {
        public frmNgRecovery()
        {
            InitializeComponent();
        }

        string _InPath = Properties.Settings.Default.ngPath;
        string _OutPath = Properties.Settings.Default.recPath;

        clsNG[] ngf;

        string _img = string.Empty;
        global gl = new global();

        // openCvSharp 関連　2019/08/19
        const float B_WIDTH = 0.66f;
        const float B_HEIGHT = 0.66f;

        const float A_WIDTH = 0.05f;
        const float A_HEIGHT = 0.05f;

        float n_width = 0f;
        float n_height = 0f;

        Image FaxImg = null;

        Image OcrImg = null;

        private void frmNgRecovery_Load(object sender, EventArgs e)
        {
            //MaximumSize = new System.Drawing.Size(Width, Height);
            //MinimumSize = new System.Drawing.Size(Width, Height);

            // NGリスト
            //GetNgList();
            ShowThumbnail();

            // ボタン
            trackBar1.Enabled = false;
            btnLeft.Enabled = false;

            button2.Enabled = false;
            btnPrn.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void frmNgRecovery_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     ＮＧ画像リストを表示する </summary>
        ///----------------------------------------------------------
        private void GetNgList()
        {
            listView1.Items.Clear();
            string[] f = System.IO.Directory.GetFiles(_InPath, "*.tif");

            if (f.Length == 0)
            {
                //label2.Text = "NG画像はありませんでした";

                linkLblOn.Enabled = false;
                linkLblOff.Enabled = false;

                button2.Enabled = false;
                btnPrn.Enabled = false;
                btnDelete.Enabled = false;

                return;
            }

            ngf = new clsNG[f.Length];

            int Cnt = 0;

            foreach (string files in System.IO.Directory.GetFiles(_InPath, "*.tif"))
            {
                ngf[Cnt] = new clsNG();
                ngf[Cnt].ngFileName = files;
                string fn = System.IO.Path.GetFileName(files);
                ngf[Cnt].ngRecDate = fn.Substring(0, 4) + "年" + fn.Substring(4, 2) + "月" + fn.Substring(6, 2) + "日" +
                                     fn.Substring(8, 2) + "時" + fn.Substring(10, 2) + "分" + fn.Substring(12, 2) + "秒";

                listView1.Items.Add(System.IO.Path.GetFileName(ngf[Cnt].ngRecDate));
                Cnt++;
            }

            //label2.Text = f.Length.ToString() + "件のＮＧ画像があります";

            linkLblOn.Enabled = true;
            linkLblOff.Enabled = true;

            button2.Enabled = true;
            btnPrn.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                trackBar1.Enabled = false;
                btnLeft.Enabled = false;

                button2.Enabled = false;
                btnPrn.Enabled = false;
                btnDelete.Enabled = false;

                return;
            }

            if (listView1.SelectedItems == null)
            {
                trackBar1.Enabled = false;
                btnLeft.Enabled = false;

                button2.Enabled = false;
                btnPrn.Enabled = false;
                btnDelete.Enabled = false;

                return;
            }
            else
            {
                trackBar1.Enabled = true;
                trackBar1.Value = 0;
                btnLeft.Enabled = true;

                button2.Enabled = true;
                btnPrn.Enabled = true;
                btnDelete.Enabled = true;
            }

            if (!System.IO.File.Exists(ngf[listView1.SelectedItems[0].Index].ngFileName))
            {
                trackBar1.Enabled = false;
                btnLeft.Enabled = false;

                button2.Enabled = false;
                btnPrn.Enabled = false;
                btnDelete.Enabled = false;

                return;
            }

            //画像イメージ表示 : 2020/04/14
            //imgShow(ngf[listView1.SelectedItems[0].Index].ngFileName);

            // System.Drawing.Imageを作成する

            OcrImg = Utility.CreateImage(ngf[listView1.SelectedItems[0].Index].ngFileName);

            imgShow(OcrImg, B_WIDTH, B_HEIGHT);
        }

        private class clsNG
        {
            public string ngFileName { get; set; }
            public string ngRecDate { get; set; }
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     画像表示メイン : 2020/04/14 </summary>
        /// <param name="mImg">
        ///     Mat形式イメージ</param>
        /// <param name="w">
        ///     width</param>
        /// <param name="h">
        ///     height</param>
        ///---------------------------------------------------------
        private void imgShow(Image mImg, float w, float h)
        {
            int cWidth = 0;
            int cHeight = 0;

            try
            {
                Bitmap bt = new Bitmap(mImg);

                // Bitmapサイズ
                if (panel1.Width < (bt.Width * w) || panel1.Height < (bt.Height * h))
                {
                    cWidth = (int)(bt.Width * w);
                    cHeight = (int)(bt.Height * h);
                }
                else
                {
                    cWidth = panel1.Width;
                    cHeight = panel1.Height;
                }

                // Bitmap を生成
                Bitmap canvas = new Bitmap(cWidth, cHeight);

                // ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(canvas);

                // 画像をcanvasの座標(0, 0)の位置に指定のサイズで描画する
                g.DrawImage(bt, 0, 0, bt.Width * w, bt.Height * h);

                //メモリクリア
                bt.Dispose();
                g.Dispose();

                // PictureBox1に表示する
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = canvas;

                // ボタン表示
                trackBar1.Enabled = true;
                btnLeft.Enabled = true;

                button2.Enabled = true;
                btnPrn.Enabled = true;
                btnDelete.Enabled = true;
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                MessageBox.Show(ex.Message);
            }
        }


        ///--------------------------------------------------------------
        /// <summary>
        ///     ＮＧファイルリカバリ </summary>
        ///--------------------------------------------------------------
        private void NgRecovery(int denKbn)
        {
            DateTime dt = DateTime.Now;
            string _ID = string.Format("{0:0000}", dt.Year) + string.Format("{0:00}", dt.Month) +
                         string.Format("{0:00}", dt.Day) + string.Format("{0:00}", dt.Hour) +
                         string.Format("{0:00}", dt.Minute) + string.Format("{0:00}", dt.Second);

            // ＮＧファイルリカバリ処理
            int fCnt = 1;

            foreach (var item in listView1.SelectedIndices)
            {
                int i = Utility.StrtoInt(item.ToString());
                NgToData(_ID, fCnt, i, denKbn);
                fCnt++;
            }

            // 終了メッセージ
            MessageBox.Show(ngFileCount().ToString() + "件の画像を発注書データとしてリカバリし受信フォルダへ移動しました", "リカバリー処理完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // ＮＧ画像リスト再表示
            //GetNgList();
            ShowThumbnail();

            // イメージ表示初期化
            pictureBox1.Image = null;
            trackBar1.Enabled = false;
            btnLeft.Enabled = false;
        }

        ///--------------------------------------------------------------------------
        /// <summary>
        ///     ＣＳＶデータファイル作成・ＮＧ画像→データ画像へ </summary>
        /// <param name="fCnt">
        ///     リカバリファイル番号</param>
        /// <param name="ind">
        ///     リストボックスインデックス</param>
        ///--------------------------------------------------------------------------
        private void NgToData(string _ID, int fCnt, int ind, int DenKbn)
        {
            // IDを取得します
            _ID += fCnt.ToString().PadLeft(3, '0');

            // 出力ファイルインスタンス作成
            StreamWriter outFile = new StreamWriter(_OutPath + _ID + ".csv", false, System.Text.Encoding.GetEncoding(932));

            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Clear();

                // *,20200711111357001.tif,1,,,,
                sb.Append("*").Append(",");
                sb.Append(_ID + ".tif").Append(",");    // 画像ファイル名
                sb.Append(DenKbn).Append(",");          // 伝票区分
                sb.Append(",,,,");                      
                //sb.Append(Environment.NewLine);

                // ＣＳＶファイル作成
                outFile.WriteLine(sb.ToString());

                // 画像ファイル移動
                File.Copy(ngf[ind].ngFileName, _OutPath + _ID + ".tif");
                File.Delete(ngf[ind].ngFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ＮＧ画像リカバリ処理", MessageBoxButtons.OK);
            }
            finally
            {
                outFile.Close();
            }
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     チェックボックス選択数取得 </summary>
        /// <returns>
        ///     選択アイテム数</returns>
        ///-------------------------------------------------------------
        private int ngFileCount()
        {
            return listView1.SelectedItems.Count;
        }

        private void button7_Click(object sender, EventArgs e)
        {
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     ＮＧ画像削除処理 </summary>
        ///-------------------------------------------------------------
        private void NgFileDelete()
        {
            // ＮＧファイル削除
            foreach (var item in listView1.SelectedIndices)
            {
                int i = Utility.StrtoInt(item.ToString());
                imgDelete(ngf[i].ngFileName);
            }

            // ＮＧ画像リスト再表示
            //GetNgList();
            ShowThumbnail();

            // イメージ表示初期化
            pictureBox1.Image = null;
            trackBar1.Enabled = false;
            btnLeft.Enabled = false;
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     ファイル削除 </summary>
        /// <param name="imgPath">
        ///     画像ファイルパス</param>
        ///-------------------------------------------------------------
        private void imgDelete(string imgPath)
        {
            // ファイルを削除する
            if (System.IO.File.Exists(imgPath))
            {
                System.IO.File.Delete(imgPath);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        ///---------------------------------------------------
        /// <summary>
        ///     ＮＧ画像印刷 </summary>
        ///---------------------------------------------------
        private void NgImagePrint()
        {
            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();

            // ＮＧ画像印刷
            foreach (var item in listView1.SelectedIndices)
            {
                int i = Utility.StrtoInt(item.ToString());

                _img = ngf[i].ngFileName;

                // デフォルトプリンタ設定
                printDocument1.PrinterSettings.PrinterName = pd.PrinterSettings.PrinterName;

                // 印刷実行
                printDocument1.Print();
            }

            // 後片付け：2017/11/18
            printDocument1.Dispose();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image img;

            img = Image.FromFile(_img);
            //e.Graphics.DrawImage(img, 0, 0);

            // 2017/12/12 縮小
            //e.Graphics.DrawImage(img, 0, 0, img.Width * 49 / 100, img.Height * 49 / 100);

            // 2018/06/21 元画像のピクセル調整を行わないことによる縮小調整
            e.Graphics.DrawImage(img, 0, 0, img.Width * 47 / 100, img.Height * 47 / 100);
            e.HasMorePages = false;

            // 後片付け 2017/11/18
            img.Dispose();
        }

        private void btnPrn_Click(object sender, EventArgs e)
        {
            if (ngFileCount() == 0)
            {
                MessageBox.Show("画像が選択されていません", "画像未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show(ngFileCount().ToString() + "件の画像を印刷します。よろしいですか？", "印刷確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            // ＮＧ画像印刷
            NgImagePrint();

            // 印刷が終了しました
            MessageBox.Show("終了しました", "NG画像印刷");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ngFileCount() == 0)
            {
                MessageBox.Show("画像が選択されていません", "画像未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show(ngFileCount().ToString() + "件の画像を削除します。よろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            // ＮＧ画像削除処理
            NgFileDelete();

            // 削除が完了しました
            MessageBox.Show("削除が完了しました");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (ngFileCount() == 0)
            {
                MessageBox.Show("画像が選択されていません", "画像未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                frmNgRecoverySub frm = new frmNgRecoverySub(ngFileCount());
                frm.ShowDialog();

                int denKbn = frm.MyProperty;
                frm.Dispose();

                if (denKbn > 0)
                {
                    // ＮＧファイルリカバリ
                    NgRecovery(denKbn);
                }
            }
        }

        private void linkLblOn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("全てのNG画像を選択します。よろしいですか。", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Selected = true;
            }
        }

        private void linkLblOff_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("全てのNG画像の選択を解除します。よろしいですか。", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Selected = false;
            }

            pictureBox1.Image = null;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (ngFileCount() == 0)
            {
                MessageBox.Show("画像が選択されていません", "画像未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show(ngFileCount().ToString() + "件の画像を印刷した後、削除します。よろしいですか？", "一括印刷・削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            // ＮＧ画像印刷
            NgImagePrint();

            // ＮＧ画像削除処理
            NgFileDelete();

            // 処理が完了しました
            MessageBox.Show("印刷・削除が完了しました");
        }

        private void printDocument1_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            MessageBox.Show(printDocument1.DocumentName +
               " has finished printing.");
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            n_width = B_WIDTH + (float)trackBar1.Value * 0.05f;
            n_height = B_HEIGHT + (float)trackBar1.Value * 0.05f;

            imgShow(OcrImg, n_width, n_height);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            ImageRotate(pictureBox1.Image);
        }

        private void ImageRotate(Image img)
        {
            Bitmap bmp = (Bitmap)img;

            // 反転せず時計回りに90度回転する
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

            //表示
            pictureBox1.Image = img;
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     サムネイル画像表示 </summary>
        ///----------------------------------------------------------
        private void ShowThumbnail()
        {
            string imageDir = Properties.Settings.Default.ngPath; // 画像ディレクトリ

            string[] jpgFiles = System.IO.Directory.GetFiles(imageDir, "*.tif");

            int width = 220;
            int height = 100;

            listView1.Items.Clear();
            imageList1.Images.Clear();

            imageList1.ImageSize = new Size(width, height);
            listView1.LargeImageList = imageList1;

            //string[] f = System.IO.Directory.GetFiles(_InPath, "*.tif");

            if (jpgFiles.Length == 0)
            {
                //label2.Text = "NG画像はありませんでした";

                linkLblOn.Enabled = false;
                linkLblOff.Enabled = false;

                button2.Enabled = false;
                btnPrn.Enabled = false;
                btnDelete.Enabled = false;

                return;
            }

            ngf = new clsNG[jpgFiles.Length];

            int Cnt = 0;

            for (int i = 0; i < jpgFiles.Length; i++)
            {
                Image original = Bitmap.FromFile(jpgFiles[i]);
                Image thumbnail = Utility.CreateThumbnail(original, width, height);

                imageList1.Images.Add(thumbnail);
                //listView1.Items.Add(jpgFiles[i], i);
                listView1.Items.Add(System.IO.Path.GetFileName(jpgFiles[i]), i);

                original.Dispose();
                thumbnail.Dispose();

                ngf[Cnt] = new clsNG();
                ngf[Cnt].ngFileName = jpgFiles[i];

                string fn = System.IO.Path.GetFileName(jpgFiles[i]);

                ngf[Cnt].ngRecDate = fn.Substring(0, 4) + "年" + fn.Substring(4, 2) + "月" + fn.Substring(6, 2) + "日" +
                                     fn.Substring(8, 2) + "時" + fn.Substring(10, 2) + "分" + fn.Substring(12, 2) + "秒";
                Cnt++;
            }

            //label2.Text = f.Length.ToString() + "件のＮＧ画像があります";

            linkLblOn.Enabled = true;
            linkLblOff.Enabled = true;

            button2.Enabled = true;
            btnPrn.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
