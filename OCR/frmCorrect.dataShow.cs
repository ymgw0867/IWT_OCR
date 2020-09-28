using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data.Linq;
using System.Data.SQLite;
using IWT_OCR.Common;

namespace IWT_OCR.OCR
{
    partial class frmCorrect
    {
        #region 単位時間フィールド
        /// <summary> 
        ///     ３０分単位 </summary>
        private int tanMin30 = 30;

        /// <summary> 
        ///     １５分単位 </summary> 
        private int tanMin15 = 15;

        /// <summary> 
        ///     １０分単位 </summary> 
        private int tanMin10 = 10;

        /// <summary> 
        ///     １分単位 </summary>
        private int tanMin1 = 1;
        #endregion
        
        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     データを画面に表示します </summary>
        /// <param name="iX">
        ///     ヘッダデータインデックス</param>
        ///------------------------------------------------------------------------------------
        private void showOcrData(int iX)
        {
            System.Diagnostics.Debug.WriteLine("5...伝票表示開始：" + cID[iX]);

            Cursor = Cursors.WaitCursor;
            //showStatus = false;

            // 非ログ書き込み状態とする
            editLogStatus = false;

            // フォーム初期化
            formInitialize(dID, iX);

            // 発注データを取得
            ClsDeviveryNote = tblDeliv.Single(a => a.ID == cID[iX]);

            global.ChangeValueStatus = true;   // これ以下ChangeValueイベントを発生させない

            string Sql = "select * from 納品伝票 WHERE ID = '" + cID[iX] + "'";

            using (SQLiteCommand com = new SQLiteCommand(Sql, cn2))
            {
                SQLiteDataReader dataReader = com.ExecuteReader();

                while (dataReader.Read())
                {
                    // 売上・仕入による制御：2020/09/25
                    switch (dataReader["売上仕入区分"].ToString())
                    {
                        case global.DEN_URIAGE: // 売上
                            URISHIIRE_KBN = global.DEN_URIAGE;  // 2020/09/28
                            label7.Text = "売上区分：";
                            label5.Text = "社名";
                            break;

                        case global.DEN_SHIIRE: // 仕入
                            URISHIIRE_KBN = global.DEN_SHIIRE;  // 2020/09/28
                            label7.Text = "仕入区分：";
                            label5.Text = "納入者";
                            break;

                        default:
                            URISHIIRE_KBN = "";     // 2020/09/28
                            label7.Text = "区分：";
                            label5.Text = "社名";
                            break;
                    }

                    switch (dataReader["伝票区分"].ToString())
                    {
                        case global.DEN_NOUHINKARI:

                            DEN_KBN = global.DEN_NOUHINKARI;
                            //label5.Text = "納入者";  2020/09/25 コメント化
                            txtShiireCode.Text = dataReader["納入先コード"].ToString();
                            dg1.Columns[0].HeaderCell.Value = "部品コード";
                            dg1.Columns[1].HeaderCell.Value = "部品名称";
                            dg1.Columns[2].HeaderCell.Value = "規格・型番";

                            cmbBumon.SelectedValue = dataReader["部門コード"].ToString();
                            string [] ss = dataReader["備考"].ToString().Split('/');

                            if (ss.Length > 0)
                            {
                                string bk = "";

                                for (int i = 0; i < ss.Length; i++)
                                {
                                    bk += ss[i].Trim() + Environment.NewLine;
                                }

                                txtBikou.Text = bk;
                            }

                            // splitContainerの上部から境界線までの距離
                            splitContainer2.SplitterDistance = SpDistance_NouhinKari;

                            // 2020/09/25 コメント化
                            // 仕入区分
                            //label7.Text = "仕入区分：";
                            //comboBox1.Items.Clear();
                            //comboBox1.Items.Add("掛");
                            //comboBox1.Items.Add("現金");

                            // 伝票ごとの画像サイズ初期値を調整
                            B_WIDTH = 0.32f;
                            B_HEIGHT = 0.32f;

                            break;

                        case global.DEN_NOUHIN: 

                            DEN_KBN = global.DEN_NOUHIN;
                            //label5.Text = "社名";   2020/09/25 コメント化
                            txtShiireCode.Text = dataReader["納入先コード"].ToString();
                            //txtSeikyuuCode.Text = dataReader["請求先コード"].ToString();
                            cmbBumon.SelectedValue = dataReader["部門コード"].ToString();
                            dg1.Columns[0].HeaderCell.Value = "品目コード";
                            dg1.Columns[1].HeaderCell.Value = "品目名";
                            dg1.Columns[2].HeaderCell.Value = "図番・仕様";
                            txtBikou.Text = dataReader["備考"].ToString();

                            // splitContainerの上部から境界線までの距離
                            splitContainer2.SplitterDistance = SpDistance_Nouhin;

                            // 2020/09/25 コメント化
                            // 売上区分
                            //label7.Text = "売上区分：";
                            //comboBox1.Items.Clear();
                            //comboBox1.Items.Add("掛");
                            //comboBox1.Items.Add("現金");

                            // 伝票ごとの画像サイズ初期値を調整
                            B_WIDTH = 0.225f;
                            B_HEIGHT = 0.225f;
                            break;

                        case global.DEN_GENPIN:

                            DEN_KBN = global.DEN_GENPIN;
                            //label5.Text = "納入者";  2020/09/25 コメント化
                            txtShiireCode.Text = dataReader["納入先コード"].ToString();
                            dg1.Columns[0].HeaderCell.Value = "品目番号";
                            dg1.Columns[1].HeaderCell.Value = "品名";
                            dg1.Columns[2].HeaderCell.Value = "規格";
                            cmbBumon.SelectedValue = dataReader["部門コード"].ToString();
                            txtBikou.Text = dataReader["備考"].ToString();

                            // splitContainerの上部から境界線までの距離
                            splitContainer2.SplitterDistance = SpDistance_Genpin;

                            // 2020/09/25 コメント化
                            //// 仕入区分
                            //label7.Text = "仕入区分：";
                            //comboBox1.Items.Clear();
                            //comboBox1.Items.Add("掛");
                            //comboBox1.Items.Add("現金");

                            // 伝票ごとの画像サイズ初期値を調整
                            B_WIDTH = 0.32f;
                            B_HEIGHT = 0.32f;

                            break;

                        default:
                            break;
                    }

                    // 納入年月日
                    string[] nYYMMDD = dataReader["納入年月日"].ToString().Split('/');

                    if (nYYMMDD.Length > 2)
                    {
                        txtYear.Text = nYYMMDD[0];
                        txtMonth.Text = nYYMMDD[1];
                        txtDay.Text = nYYMMDD[2];
                    }
                    else if (nYYMMDD.Length > 1)
                    {
                        txtYear.Text = nYYMMDD[0];
                        txtMonth.Text = nYYMMDD[1];
                        txtDay.Text = "";
                    }
                    else if (nYYMMDD.Length > 0)
                    {
                        txtYear.Text = nYYMMDD[0];
                        txtMonth.Text = "";
                        txtDay.Text = "";
                    }

                    txtShiireName.Text = dataReader["納入先名"].ToString();

                    checkBox1.Checked = Convert.ToBoolean(Utility.StrtoInt(Utility.NulltoStr(dataReader["確認"])));
                    txtMemo.Text = dataReader["メモ"].ToString();

                    // 納品伝票データ表示
                    dg1[colHinCode, 0].Value = dataReader["部品コード1"].ToString();

                    if (dataReader["数量1"].ToString() != string.Empty)
                    {
                        dg1[colSuu, 0].Value = string.Format("{0:N0}", Utility.StrtoInt(dataReader["数量1"].ToString().Replace(",", "")));
                    }
                    else
                    {
                        dg1[colSuu, 0].Value = dataReader["数量1"].ToString();
                    }

                    dg1[colHinCode, 1].Value = dataReader["部品コード2"].ToString();

                    if (dataReader["数量2"].ToString() != string.Empty)
                    {
                        dg1[colSuu, 1].Value = string.Format("{0:N0}", Utility.StrtoInt(dataReader["数量2"].ToString().Replace(",", "")));
                    }
                    else
                    {
                        dg1[colSuu, 1].Value = dataReader["数量2"].ToString();
                    }

                    dg1[colHinCode, 2].Value = dataReader["部品コード3"].ToString();

                    if (dataReader["数量3"].ToString() != string.Empty)
                    {
                        dg1[colSuu, 2].Value = string.Format("{0:N0}", Utility.StrtoInt(dataReader["数量3"].ToString().Replace(",", "")));
                    }
                    else
                    {
                        dg1[colSuu, 2].Value = dataReader["数量3"].ToString();
                    }

                    dg1[colHinCode, 3].Value = dataReader["部品コード4"].ToString();

                    if (dataReader["数量4"].ToString() != string.Empty)
                    {
                        dg1[colSuu, 3].Value = string.Format("{0:N0}", Utility.StrtoInt(dataReader["数量4"].ToString().Replace(",", "")));
                    }
                    else
                    {
                        dg1[colSuu, 3].Value = dataReader["数量4"].ToString();
                    }

                    dg1[colHinCode, 4].Value = dataReader["部品コード5"].ToString();

                    if (dataReader["数量5"].ToString() != string.Empty)
                    {
                        dg1[colSuu, 4].Value = string.Format("{0:N0}", Utility.StrtoInt(dataReader["数量5"].ToString().Replace(",", "")));
                    }
                    else
                    {
                        dg1[colSuu, 4].Value = dataReader["数量5"].ToString();
                    }

                    dg1.CurrentCell = null;

                    //txtBikou.Text = dataReader["備考"].ToString();
                    txtMemo.Text = dataReader["メモ"].ToString();

                    // 掛現金区分
                    comboBox1.SelectedIndex = Utility.StrtoInt(dataReader["掛現金区分"].ToString()) - 1;

                    // 画像表示
                    _img = Properties.Settings.Default.dataPath + dataReader["画像名"].ToString();

                    if (System.IO.File.Exists(_img))
                    {
                        // System.Drawing.Imageを作成する
                        OcrImg = Utility.CreateImage(_img);

                        //imgShow(_img);

                        imgShow(OcrImg, B_WIDTH, B_HEIGHT);
                        trackBar1.Enabled = true;
                        btnLeft.Enabled = true;
                    }
                    else
                    {
                        pictureBox1.Image = null;
                        trackBar1.Enabled = false;
                        btnLeft.Enabled = false;
                    }

                    // エラー情報表示初期化
                    lblErrMsg.Visible = false;
                    lblErrMsg.Text = string.Empty;

                    label6.Text = "[" + dataReader["ID"].ToString() + "]";

                    // 売上仕入区分：2020/09/25
                    lblUriShiire.Text = global.DEN_ARRAY[Utility.StrtoInt(dataReader["売上仕入区分"].ToString())];
                }

                dataReader.Close();
            }

            // ログ書き込み状態とする
            editLogStatus = true;

            Cursor = Cursors.Default;

            //comboBox1.SelectedIndex = 0;

            DataShowStatus = true;

            System.Diagnostics.Debug.WriteLine("6...伝票表示終了：" + cID[iX]);
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     画像表示メイン : 2020/04/14 </summary>
        /// <param name="filePath">
        ///     画像ファイルパス</param>
        ///---------------------------------------------------------
        private void imgShow(string filePath)
        {
            try
            {
                // System.Drawing.Imageを作成する
                OcrImg = Utility.CreateImage(filePath);

                // PictureBoxの大きさにあわせて画像を拡大または縮小して表示する
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                // 画像を表示する
                pictureBox1.Image = OcrImg;
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                MessageBox.Show(ex.Message);
            }
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
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                MessageBox.Show(ex.Message);
            }
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     画像を表示する </summary>
        /// <param name="pic">
        ///     pictureBoxオブジェクト</param>
        /// <param name="imgName">
        ///     イメージファイルパス</param>
        /// <param name="fX">
        ///     X方向のスケールファクター</param>
        /// <param name="fY">
        ///     Y方向のスケールファクター</param>
        ///------------------------------------------------------------------------------------
        private void ImageGraphicsPaint(PictureBox pic, string imgName, float fX, float fY, int RectDest, int RectSrc)
        {
            Image _img = Image.FromFile(imgName);
            Graphics g = Graphics.FromImage(pic.Image);

            // 各変換設定値のリセット
            g.ResetTransform();

            // X軸とY軸の拡大率の設定
            g.ScaleTransform(fX, fY);

            // 画像を表示する
            g.DrawImage(_img, RectDest, RectSrc);

            // 現在の倍率,座標を保持する
            gl.ZOOM_NOW = fX;
            gl.RECTD_NOW = RectDest;
            gl.RECTS_NOW = RectSrc;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     フォーム表示初期化 </summary>
        /// <param name="sID">
        ///     過去データ表示時のヘッダID</param>
        /// <param name="cIx">
        ///     勤務票ヘッダカレントレコードインデックス</param>
        ///------------------------------------------------------------------------------------
        private void formInitialize(string sID, int cIx)
        {
            global.ChangeValueStatus = false;   // これ以下ChangeValueイベントを発生させない
            ShiireNameChangeStatus = false;
            ShiireCodeChangeStatus = false;

            // テキストボックス表示色設定
            txtYear.BackColor = Color.White;
            txtMonth.BackColor = Color.White;
            txtShiireName.BackColor = Color.White;
            txtShiireCode.BackColor = Color.White;
            //txtSeikyuuCode.BackColor = Color.White;
            //txtSeikyuuName.BackColor = Color.White;
            cmbBumon.BackColor = Color.White;
            txtBikou.BackColor = Color.White;
            txtMemo.BackColor = Color.White;
            checkBox1.BackColor = SystemColors.Control;

            lblErrMsg.Text = string.Empty;

            txtYear.ForeColor = global.defaultColor;
            txtMonth.ForeColor = global.defaultColor;
            txtShiireName.ForeColor = global.defaultColor;
            txtShiireCode.ForeColor = global.defaultColor;
            //txtSeikyuuCode.ForeColor = global.defaultColor;
            //txtSeikyuuName.ForeColor = global.defaultColor;
            cmbBumon.ForeColor = global.defaultColor;
            txtBikou.ForeColor = global.defaultColor;
            txtMemo.ForeColor = global.defaultColor;

            txtYear.Text = string.Empty;
            txtMonth.Text = string.Empty;
            txtShiireName.Text = string.Empty;
            txtShiireCode.Text = string.Empty;
            //txtSeikyuuCode.Text = string.Empty;
            //txtSeikyuuName.Text = string.Empty;
            txtBikou.Text = string.Empty;
            txtMemo.Text = string.Empty;
            lblNoImage.Visible = false;

            dg1.Rows.Clear();   // 行数をクリア
            dg1.Rows.Add(5);   // 行数を設定

            // 確認チェック欄
            checkBox1.BackColor = SystemColors.Control;
            checkBox1.Checked = false;

            // データ編集のとき
            if (sID == string.Empty)
            {
                // ヘッダ情報
                txtYear.ReadOnly = false;
                txtMonth.ReadOnly = false;

                // スクロールバー設定
                hScrollBar1.Enabled = true;
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = cID.Length - 1;
                hScrollBar1.Value = cIx;
                hScrollBar1.LargeChange = 1;
                hScrollBar1.SmallChange = 1;

                //移動ボタン制御
                btnFirst.Enabled = true;
                btnNext.Enabled = true;
                btnBefore.Enabled = true;
                btnEnd.Enabled = true;

                //最初のレコード
                if (cIx == 0)
                {
                    btnBefore.Enabled = false;
                    btnFirst.Enabled = false;
                }

                //最終レコード
                if ((cIx + 1) == cID.Length)
                {
                    btnNext.Enabled = false;
                    btnEnd.Enabled = false;
                }

                // その他のボタンを有効とする
                btnErrCheck.Visible = true;
                btnHold.Visible = true;
                btnDelete.Visible = true;
                btnPrint.Visible = true;
                btnData.Visible = true;

                //データ数表示
                lblPage.Text =  (cIx + 1).ToString() + "/" + cID.Length;
            }
            else
            {
                // ヘッダ情報
                txtYear.ReadOnly = true;
                txtMonth.ReadOnly = true;

                // スクロールバー設定
                hScrollBar1.Enabled = true;
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = 0;
                hScrollBar1.Value = 0;
                hScrollBar1.LargeChange = 1;
                hScrollBar1.SmallChange = 1;

                //移動ボタン制御
                btnFirst.Enabled = false;
                btnNext.Enabled = false;
                btnBefore.Enabled = false;
                btnEnd.Enabled = false;

                // その他のボタンを無効とする
                btnErrCheck.Visible = true;
                btnHold.Visible = true;
                btnDelete.Visible = true;
                btnPrint.Visible = true;
                btnData.Visible = true;

                //データ数表示
                lblPage.Text = string.Empty;
            }

            ShiireNameChangeStatus = true;
            ShiireCodeChangeStatus = true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     エラー表示 </summary>
        /// <param name="ocr">
        ///     OCRDATAクラス</param>
        ///------------------------------------------------------------------------------------
        //private void ErrShow(OCRData ocr)
        //{
        //    if (ocr._errNumber != ocr.eNothing)
        //    {
        //        // グリッドビューCellEnterイベント処理は実行しない
        //        gridViewCellEnterStatus = false;

        //        lblErrMsg.Visible = true;
        //        lblErrMsg.Text = ocr._errMsg;

        //        // 確認
        //        if (ocr._errNumber == ocr.eDataCheck)
        //        {
        //            checkBox1.BackColor = Color.Yellow;
        //            checkBox1.Focus();
        //        }

        //        // 年月
        //        if (ocr._errNumber == ocr.eYearMonth)
        //        {
        //            txtYear.BackColor = Color.Yellow;
        //            txtYear.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eMonth)
        //        {
        //            txtMonth.BackColor = Color.Yellow;
        //            txtMonth.Focus();
        //        }

        //        // 得意先コード
        //        if (ocr._errNumber == ocr.eTdkNo)
        //        {
        //            txtTokuisakiCD.BackColor = Color.Yellow;
        //            txtTokuisakiCD.Focus();

        //            // エラー有りフラグ
        //            txtErrStatus.Text = global.FLGON;
        //        }

        //        // パターンID
        //        if (ocr._errNumber == ocr.ePattern)
        //        {
        //            txtPID.BackColor = Color.Yellow;
        //            txtPID.Focus();
        //        }

        //        // 店着日付
        //        if (ocr._errNumber == ocr.eTenDate1)
        //        {
        //            txtTenDay1.BackColor = Color.Yellow;
        //            txtTenDay1.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate2)
        //        {
        //            txtTenDay2.BackColor = Color.Yellow;
        //            txtTenDay2.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate3)
        //        {
        //            txtTenDay3.BackColor = Color.Yellow;
        //            txtTenDay3.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate4)
        //        {
        //            txtTenDay4.BackColor = Color.Yellow;
        //            txtTenDay4.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate5)
        //        {
        //            txtTenDay5.BackColor = Color.Yellow;
        //            txtTenDay5.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate6)
        //        {
        //            txtTenDay6.BackColor = Color.Yellow;
        //            txtTenDay6.Focus();
        //        }

        //        if (ocr._errNumber == ocr.eTenDate7)
        //        {
        //            txtTenDay7.BackColor = Color.Yellow;
        //            txtTenDay7.Focus();
        //        }

        //        // 商品コード
        //        if (ocr._errNumber == ocr.eHinCode)
        //        {
        //            dg1[colHinCode,  ocr._errRow - 1].Style.BackColor = Color.Yellow;
        //            dg1[colHinCode,  ocr._errRow].Style.BackColor = Color.Yellow;
        //            dg1.Focus();
        //            dg1.CurrentCell = dg1[colHinCode, ocr._errRow];
        //        }

        //        // 発注数
        //        string col = "";
        //        for (int i = 0; i < 7; i++)
        //        {
        //            if (ocr._errNumber == ocr.eSuu[i])
        //            {
        //                switch (i)
        //                {
        //                    case 0:
        //                        col = colDay1;
        //                        break;
        //                    case 1:
        //                        col = colDay2;
        //                        break;
        //                    case 2:
        //                        col = colDay3;
        //                        break;
        //                    case 3:
        //                        col = colDay4;
        //                        break;
        //                    case 4:
        //                        col = colDay5;
        //                        break;
        //                    case 5:
        //                        col = colDay6;
        //                        break;
        //                    case 6:
        //                        col = colDay7;
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                dg1[col, ocr._errRow - 1].Style.BackColor = Color.Yellow;
        //                dg1[col, ocr._errRow].Style.BackColor = Color.Yellow;
        //                dg1.Focus();
        //                dg1.CurrentCell = dg1[col, ocr._errRow];

        //                break;
        //            }
        //        }

        //        // 終売コンボボックス
        //        if (ocr._errNumber == ocr.eShubai)
        //        {
        //            dg1[colSyubai, ocr._errRow - 1].Style.BackColor = Color.Yellow;
        //            dg1.Focus();
        //            dg1.CurrentCell = dg1[colSyubai, ocr._errRow];
        //        }


        //        // グリッドビューCellEnterイベントステータスを戻す
        //        gridViewCellEnterStatus = true;
        //    }
        //}

        /////----------------------------------------------------------
        ///// <summary>
        /////     幅w、高さhのImageオブジェクトを作成 </summary>
        ///// <param name="image">
        /////     イメージ</param>
        ///// <param name="w">
        /////     画像サイズ・幅</param>
        ///// <param name="h">
        /////     画像サイズ・高さ</param>
        /////----------------------------------------------------------
        //Image CreateThumbnail(Image image, int w, int h)
        //{
        //    Bitmap canvas = new Bitmap(w, h);

        //    Graphics g = Graphics.FromImage(canvas);
        //    g.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);

        //    float fw = (float)w / (float)image.Width;
        //    float fh = (float)h / (float)image.Height;

        //    float scale = Math.Min(fw, fh);
        //    fw = image.Width * scale;
        //    fh = image.Height * scale;

        //    g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
        //    g.Dispose();

        //    return canvas;
        //}

        ///----------------------------------------------------------
        /// <summary>
        ///     サムネイル画像表示 </summary>
        ///----------------------------------------------------------
        private void ShowThumbnail()
        {
            string imageDir = Properties.Settings.Default.dataPath; // 画像ディレクトリ

            string[] jpgFiles = System.IO.Directory.GetFiles(imageDir, "*.tif");

            int width = 90;
            int height = 72;

            listView1.Items.Clear();
            imageList1.Images.Clear();

            imageList1.ImageSize = new Size(width, height);
            //listView1.LargeImageList = imageList1;

            for (int i = 0; i < jpgFiles.Length; i++)
            {
                Image original = Bitmap.FromFile(jpgFiles[i]);
                Image thumbnail = Utility.CreateThumbnail(original, width, height);

                imageList1.Images.Add(thumbnail);
                //listView1.Items.Add(jpgFiles[i], i);
                listView1.Items.Add(System.IO.Path.GetFileName(jpgFiles[i]), i);

                original.Dispose();
                thumbnail.Dispose();
            }
        }
    }
}
