using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWT_OCR.Common;

namespace IWT_OCR.OCR
{
    public partial class frmHoldRec : Form
    {
        public frmHoldRec()
        {
            InitializeComponent();
        }

        clsNG[] ngf_H;
        clsNG[] ngf_R;

        Image OcrImg = null;
        const float B_WIDTH = 0.66f;
        const float B_HEIGHT = 0.66f;

        SQLiteConnection cn2 = null;
        DataContext context2 = null;

        string Local_DB = Properties.Settings.Default.Local_DB;

        // 納品伝票
        Table<Common.ClsDeviveryNote> tblDeliv = null;
        ClsDeviveryNote ClsDeviveryNote = null;

        private class clsNG
        {
            public string ngFileName { get; set; }
            public string ngRecDate { get; set; }
        }

        private void frmHoldRec_Load(object sender, EventArgs e)
        {
            // ローカルDB接続
            cn2 = new SQLiteConnection("DataSource=" + Local_DB);
            context2 = new DataContext(cn2);
            tblDeliv = context2.GetTable<Common.ClsDeviveryNote>();     // 納品伝票

            // 処理中伝票件数取得
            lblDen.Text = tblDeliv.Count().ToString();

            // 保留画像
            ShowThumbnail(listView1, imageList1, Properties.Settings.Default.HoldTifPath, ref ngf_H);
            if (listView1.Items.Count == 0)
            {
                linkLblOn.Enabled = false;
                linkLblOff.Enabled = false;
            }
            else
            {
                linkLblOn.Enabled = true;
                linkLblOff.Enabled = true;
            }

            // リカバリ画像
            ShowThumbnail(listView2, imageList2, Properties.Settings.Default.recPath, ref ngf_R);
            if (listView2.Items.Count == 0)
            {
                linkLabel2.Enabled = false;
                linkLabel1.Enabled = false;
            }
            else
            {
                linkLabel2.Enabled = true;
                linkLabel1.Enabled = true;
            }
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     サムネイル画像表示 </summary>
        ///----------------------------------------------------------
        private void ShowThumbnail(ListView listView, ImageList imageList, string inPath, ref clsNG [] ngf)
        {
            string imageDir = inPath; // 画像ディレクトリ

            string[] jpgFiles = System.IO.Directory.GetFiles(imageDir, "*.tif");

            int width = 160;
            int height = 100;

            listView.Items.Clear();
            imageList.Images.Clear();

            imageList.ImageSize = new Size(width, height);
            listView.LargeImageList = imageList;

            if (jpgFiles.Length == 0)
            {
                return;
            }

            ngf = new clsNG[jpgFiles.Length];

            int Cnt = 0;

            for (int i = 0; i < jpgFiles.Length; i++)
            {
                Image original = Bitmap.FromFile(jpgFiles[i]);
                Image thumbnail = Utility.CreateThumbnail(original, width, height);

                imageList.Images.Add(thumbnail);
                //listView1.Items.Add(jpgFiles[i], i);
                listView.Items.Add(System.IO.Path.GetFileName(jpgFiles[i]), i);

                original.Dispose();
                thumbnail.Dispose();

                ngf[Cnt] = new clsNG();
                ngf[Cnt].ngFileName = jpgFiles[i];

                string fn = System.IO.Path.GetFileName(jpgFiles[i]);

                ngf[Cnt].ngRecDate = fn.Substring(0, 4) + "年" + fn.Substring(4, 2) + "月" + fn.Substring(6, 2) + "日" +
                                     fn.Substring(8, 2) + "時" + fn.Substring(10, 2) + "分" + fn.Substring(12, 2) + "秒";
                Cnt++;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            
            if (lv.SelectedItems.Count < 1)
            {
                //button2.Enabled = false;
                return;
            }

            if (lv.SelectedItems == null)
            {
                //button2.Enabled = false;
                return;
            }
            else
            {
                //button2.Enabled = true;
            }

            if (!System.IO.File.Exists(ngf_H[lv.SelectedItems[0].Index].ngFileName))
            {
                //button2.Enabled = false;
                return;
            }

            //画像イメージ表示
            OcrImg = Utility.CreateImage(ngf_H[lv.SelectedItems[0].Index].ngFileName);
            imgShow(OcrImg, B_WIDTH, B_HEIGHT);
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
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = canvas;
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLblOn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listAllCheck(listView1, true);
        }

        private void linkLblOff_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listAllCheck(listView1, false);
        }

        private void linkLblOn_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listAllCheck(listView2, true);
        }

        private void linkLblOff_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listAllCheck(listView2, false);
        }

        private void listAllCheck(ListView lv, bool bl)
        {
            for (int i = 0; i < lv.Items.Count; i++)
            {
                lv.Items[i].Checked = bl;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ho = Utility.StrtoInt(lblHoldDen.Text);
            int reco = Utility.StrtoInt(lblRecDen.Text);

            if ((ho + reco) == global.flgOff)
            {
                Close();
            }
            else
            {
                string msg = "以下の伝票を読み込みます。よろしいですか？" + Environment.NewLine + Environment.NewLine;

                if (ho > 0)
                {
                    // 保留伝票
                    msg += "保留伝票：" + ho + "件" + Environment.NewLine;
                }

                if (reco > 0)
                {
                    // リカバリ伝票
                    msg += "リカバリ伝票：" + reco + "件" + Environment.NewLine;
                }

                if (MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    // 保留、リカバリ伝票読み込み
                    LoadHoldRecData(ho, reco);
                    Close();
                }
            }
        }

        private void LoadHoldRecData(int ho, int reco)
        {
            if (ho > 0)
            {
                // 保留伝票
                setHoldData();
            }

            if (reco > 0)
            {
                // リカバリ伝票
                GetRecData();
            }
        }



        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            lblHoldDen.Text = listView1.CheckedItems.Count.ToString();
        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            lblRecDen.Text = listView2.CheckedItems.Count.ToString();
        }

        private void listView1_Leave(object sender, EventArgs e)
        {
            foreach (var item in listView1.SelectedIndices)
            {
                listView1.Items[Utility.StrtoInt(item.ToString())].Selected = false;
            }
        }

        private void listView2_Leave(object sender, EventArgs e)
        {
            foreach (var item in listView2.SelectedIndices)
            {
                listView2.Items[Utility.StrtoInt(item.ToString())].Selected = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     リカバリ伝票読み込み </summary>
        ///---------------------------------------------------------
        private void GetRecData()
        {
            string _OutPath = Properties.Settings.Default.dataPath;
            int i = 0;

            foreach (var item in listView2.CheckedIndices)
            {
                i = Utility.StrtoInt(item.ToString());

                //MessageBox.Show(ngf[i].ngFileName);

                // 対象ファイル名取得
                string fileNm = System.IO.Path.GetFileNameWithoutExtension(ngf_R[i].ngFileName);
                string FromImg = Properties.Settings.Default.recPath + fileNm + ".tif";
                string FromCsv = Properties.Settings.Default.recPath + fileNm + ".csv";

                // ファイルコピー
                File.Copy(FromImg, _OutPath + fileNm + ".tif");    // 画像
                File.Copy(FromCsv, _OutPath + fileNm + ".csv");    // CSV

                // ファイル削除
                File.Delete(FromImg);   // 画像
                File.Delete(FromCsv);   // csv
            }
        }

        private void frmHoldRec_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 後片付け
            Dispose();
        }

        private void lblHoldDen_TextChanged(object sender, EventArgs e)
        {
            //if ((Utility.StrtoInt(lblHoldDen.Text) + Utility.StrtoInt(lblRecDen.Text)) > 0)
            //{
            //    button2.Enabled = true;
            //}
            //else
            //{
            //    button2.Enabled = false;
            //}
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     保留処理 </summary>
        /// <param name="iX">
        ///     データインデックス</param>
        ///----------------------------------------------------------
        private void setHoldData()
        {
            string _OutPath = Properties.Settings.Default.dataPath;
            int i = 0;

            SQLiteConnection cn2 = null;

            try
            {
                // ローカルDB接続
                string Local_DB = Properties.Settings.Default.Local_DB;
                cn2 = new SQLiteConnection("DataSource=" + Local_DB);
                cn2.Open();

                foreach (var item in listView1.CheckedIndices)
                {
                    i = Utility.StrtoInt(item.ToString());

                    // 対象ファイル名取得
                    string fileNm = System.IO.Path.GetFileNameWithoutExtension(ngf_H[i].ngFileName);
                    string FromImg = Properties.Settings.Default.HoldTifPath + fileNm + ".tif";

                    string sql = "";

                    // 納品伝票テーブルに保留データを移動する
                    sql = "INSERT INTO 納品伝票 ";
                    sql += "SELECT * FROM 保留伝票 ";
                    sql += "WHERE ID = '" + fileNm + "'";

                    using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                    {
                        com.ExecuteNonQuery();
                    }

                    // 保留データを削除します
                    sql = "Delete from 保留伝票 ";
                    sql += "WHERE ID= '" + fileNm + "'";

                    using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                    {
                        com.ExecuteNonQuery();
                    }

                    // 画像ファイルが存在するとき
                    if (System.IO.File.Exists(FromImg))
                    {
                        // 移動先に同じ名前のファイルが存在する場合、既にあるファイルを削除する
                        string tifName = Properties.Settings.Default.dataPath + fileNm + ".tif";

                        if (System.IO.File.Exists(tifName))
                        {
                            System.IO.File.Delete(tifName);
                        }

                        // 画像ファイルコピー
                        File.Copy(FromImg, tifName);    // 画像

                        // 画像ファイル削除
                        File.Delete(FromImg);   // 画像
                    }
                }


                //// 終了メッセージ
                //MessageBox.Show("伝票が保留されました", "伝票保留", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (cn2.State == ConnectionState.Open)
                //{
                //    // いったん閉じて又開く
                //    cn2.Close();
                //    cn2.Open();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (cn2.State == ConnectionState.Open)
                {
                    cn2.Close();
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;

            if (lv.SelectedItems.Count < 1)
            {
                //button2.Enabled = false;
                return;
            }

            if (lv.SelectedItems == null)
            {
                //button2.Enabled = false;
                return;
            }
            else
            {
                //button2.Enabled = true;
            }

            if (!System.IO.File.Exists(ngf_R[lv.SelectedItems[0].Index].ngFileName))
            {
                //button2.Enabled = false;
                return;
            }

            //画像イメージ表示
            OcrImg = Utility.CreateImage(ngf_R[lv.SelectedItems[0].Index].ngFileName);
            imgShow(OcrImg, B_WIDTH, B_HEIGHT);
        }
    }
}
