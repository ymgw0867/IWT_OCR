using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Data.Linq;
using IWT_OCR.Common;
using IWT_OCR.OCR;
using IWT_OCR.Config;

namespace IWT_OCR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // データベース：Sqlite3
        SQLiteConnection cn = null;
        DataContext context = null;

        string db_file = Properties.Settings.Default.DB_File;

        // 環境設定
        Table<Common.ClsSystemConfig> tblCfg = null;

        // 売上原価Proデータ出力先パス
        string CsvOutPath = string.Empty;
        // 画像保存先パス
        string TifOutPath = string.Empty;
        // 画像保存月数
        int ImageSpan = 0;
        // ログ保存月数
        int LogSpan = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            int HoldCnt = System.IO.Directory.GetFiles(Properties.Settings.Default.HoldTifPath, "*.tif").Count();
            int RecCnt = System.IO.Directory.GetFiles(Properties.Settings.Default.recPath, "*.tif").Count();

            // 保留、リカバリ伝票の選択画面
            if ((HoldCnt + RecCnt) > 0)
            {
                this.Hide();
                frmHoldRec frms = new frmHoldRec();
                frms.ShowDialog();
                this.Show();
            }

            // 修正画面
            this.Hide();
            frmCorrect frm = new frmCorrect();
            frm.ShowDialog();
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // キャプションにバージョンを追加
            this.Text += "   ver " + Application.ProductVersion;

            Utility.WindowsMaxSize(this, this.Width, this.Height);
            //Utility.WindowsMinSize(this, this.Width, this.Height);

            // 共有DB接続
            cn = new SQLiteConnection("DataSource=" + db_file);
            context = new DataContext(cn);
            tblCfg = context.GetTable<Common.ClsSystemConfig>();    // 環境設定

            // 環境設定テーブルを取得
            tblCfg = context.GetTable<Common.ClsSystemConfig>();
            var cf = tblCfg.Single(a => a.ID == global.configKEY);
            TifOutPath = cf.ImgPath;    // 画像保存先パス
            ImageSpan = cf.DataSpan;    // 画像保存月数
            LogSpan = cf.LogSpan;       // ログ保存月数

            // CSVデータをDataSetに読み込む
            global.dtSyohin = readCSV(cf.HinMstPath);           // 商品マスターCSV
            global.dtShiire = readCSV(cf.ShiireMstPath);        // 仕入先マスターCSV
            //global.dtBumon = readCSV(cf.BumonMstPath);          // 部門マスターCSV  2020/08/17 コメント化
            global.dtTorihiki = readCSV(cf.TorihikiMstPath);    // 取引先マスターCSV
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmConfig frm = new frmConfig();
            frm.ShowDialog();
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 保存月数経過画像・編集ログデータ削除
            EndLogic();

            // 閉じる
            Close();
        }

        private void EndLogic()
        {
            // 保存月数経過画像削除
            if (ImageSpan > global.flgOff)
            {
                DeletePastImage(TifOutPath, ImageSpan);
            }

            // 保存月数経過編集ログデータ削除
            if (LogSpan > global.flgOff)
            {
                DeleteLogData(LogSpan);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 後片付け
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int sCnt = System.IO.Directory.GetFiles(Properties.Settings.Default.scanPath, "*.tif").Count();

            // WinReaderHandsのerrorパスの画像の存在を確認する
            if (sCnt > 0)
            {
                if (MessageBox.Show(sCnt + "件の画像のＯＣＲ認識処理を行います。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // ＯＣＲ認識処理
                doOCr(Properties.Settings.Default.dataPath);

                // ＮＧ画像件数取得
                int ng = System.IO.Directory.GetFiles(Properties.Settings.Default.wrNgPath, "*.tif").Count();
                
                // アンマッチ画像をNGフォルダへ移動する
                getNgImage(Properties.Settings.Default.wrNgPath, Properties.Settings.Default.ngPath);

                // 終了メッセージ
                //MessageBox.Show("ＯＣＲ認識処理が終了しました");

                // 認識結果
                frmOCRResult frm = new frmOCRResult(sCnt, sCnt - ng, ng);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("認識する画像がありません", "対象データ不在", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        ///------------------------------------------------------------
        /// <summary>
        ///     ＯＣＲ認識処理 </summary>
        /// <param name="jobname">
        ///     WinReaderJob名</param>
        /// <param name="outPath">
        ///     出力先パス(DATAフォルダ)</param>
        ///------------------------------------------------------------
        private void doOCr(string outPath)
        {
            // SCANパスの画像の存在を確認してOCR認識を行う : 2017/10/22
            if (System.IO.Directory.GetFiles(Properties.Settings.Default.scanPath, "*.tif").Count() > 0)
            {
                // マルチTiff画像をシングルtifに分解する(SCANフォルダ → TRAYフォルダ)
                if (MultiTif_New(Properties.Settings.Default.scanPath, Properties.Settings.Default.trayPath))
                {
                    int dNum = 0;   // ファイル名末尾連番

                    // 納品仮伝票のOCR処理を実施する
                    WinReaderOCR(Properties.Settings.Default.jobName_Nouhinkariden);

                    // 納品書
                    if (System.IO.Directory.GetFiles(Properties.Settings.Default.wrNgPath, "*.tif").Count() > 0)
                    {
                        // WinreaderエラーフォルダからTRAYフォルダへファイルをコピー
                        foreach (var file in System.IO.Directory.GetFiles(Properties.Settings.Default.wrNgPath, "*.tif"))
                        {
                            System.IO.File.Copy(file, Properties.Settings.Default.trayPath + System.IO.Path.GetFileName(file));
                        }

                        // 納品書のOCR処理を実施する
                        WinReaderOCR(Properties.Settings.Default.jobName_Nouhinsho);
                    }

                    // 現品票
                    if (System.IO.Directory.GetFiles(Properties.Settings.Default.wrNgPath, "*.tif").Count() > 0)
                    {
                        // WinreaderエラーフォルダからTRAYフォルダへファイルをコピー
                        foreach (var file in System.IO.Directory.GetFiles(Properties.Settings.Default.wrNgPath, "*.tif"))
                        {
                            System.IO.File.Copy(file, Properties.Settings.Default.trayPath + System.IO.Path.GetFileName(file));
                        }

                        // 現品票のOCR処理を実施する
                        WinReaderOCR(Properties.Settings.Default.jobName_Genpinhyo);
                    }

                    // ファイル名（日付時間部分）
                    string fName = string.Format("{0:0000}", DateTime.Today.Year) +
                            string.Format("{0:00}", DateTime.Today.Month) +
                            string.Format("{0:00}", DateTime.Today.Day) +
                            string.Format("{0:00}", DateTime.Now.Hour) +
                            string.Format("{0:00}", DateTime.Now.Minute) +
                            string.Format("{0:00}", DateTime.Now.Second);

                    /* OCR認識結果ＣＳＶデータを出勤簿ごとに分割して
                     * 画像ファイルと共にDATAフォルダへ移動する */

                    // 納品仮伝票OCRデータ
                    LoadCsvDivide(fName, ref dNum, outPath, Properties.Settings.Default.納品仮伝票パス);

                    // 納品書OCRデータ
                    LoadCsvDivide(fName, ref dNum, outPath, Properties.Settings.Default.納品書パス);
                    
                    // 現品票OCRデータ
                    LoadCsvDivide(fName, ref dNum, outPath, Properties.Settings.Default.現品票パス);
                }
            }
            else
            {
                MessageBox.Show("認識する納品書等の画像がありません", "対象データ不在", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        ///------------------------------------------------------------------------------
        /// <summary>
        ///     マルチフレームの画像ファイルを頁ごとに分割する：OpenCVバージョン</summary>
        /// <param name="InPath">
        ///     画像ファイル入力パス</param>
        /// <param name="outPath">
        ///     分割後出力パス</param>
        /// <returns>
        ///     true:分割を実施, false:分割ファイルなし</returns>
        ///------------------------------------------------------------------------------
        private bool MultiTif_New(string InPath, string outPath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // 出力先フォルダがなければ作成する
                if (System.IO.Directory.Exists(outPath) == false)
                {
                    System.IO.Directory.CreateDirectory(outPath);
                }

                // 出力先フォルダ内の全てのファイルを削除する（通常ファイルは存在しないが例外処理などで残ってしまった場合に備えて念のため）
                foreach (string files in System.IO.Directory.GetFiles(outPath, "*"))
                {
                    System.IO.File.Delete(files);
                }

                int _pageCount = 0;
                string fnm = string.Empty;

                // マルチTIFを分解して画像ファイルをTRAYフォルダへ保存する
                foreach (string files in System.IO.Directory.GetFiles(InPath, "*.tif"))
                {
                    //TIFFのImageCodecInfoを取得する
                    ImageCodecInfo ici = GetEncoderInfo("image/tiff");

                    if (ici == null)
                    {
                        return false;
                    }

                    using (System.IO.FileStream tifFS = new System.IO.FileStream(files, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        Image gim = Image.FromStream(tifFS);

                        FrameDimension gfd = new FrameDimension(gim.FrameDimensionsList[0]);

                        //全体のページ数を得る
                        int pageCount = gim.GetFrameCount(gfd);

                        for (int i = 0; i < pageCount; i++)
                        {
                            gim.SelectActiveFrame(gfd, i);

                            //// 画像サイズ変更（Ａ４縦サイズ）：2019/10/18 コメント化：2020/06/28
                            //Bitmap jj = new Bitmap(gim, 1637, 2322);

                            //// 画像解像度変更：2019/10/18 コメント化：2020/06/28
                            //jj.SetResolution(200F, 200F);

                            // ファイル名（日付時間部分）
                            string fName = string.Format("{0:0000}", DateTime.Today.Year) + string.Format("{0:00}", DateTime.Today.Month) +
                                    string.Format("{0:00}", DateTime.Today.Day) + string.Format("{0:00}", DateTime.Now.Hour) +
                                    string.Format("{0:00}", DateTime.Now.Minute) + string.Format("{0:00}", DateTime.Now.Second);

                            _pageCount++;

                            // ファイル名設定
                            fnm = outPath + fName + string.Format("{0:000}", _pageCount) + ".tif";

                            EncoderParameters ep = null;

                            // 圧縮方法を指定する
                            ep = new EncoderParameters(1);
                            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionCCITT4);

                            // 画像保存
                            gim.Save(fnm, ici, ep);

                            //// 画像保存 2019/10/18,  コメント化：2020/06/28
                            //jj.Save(fnm, ici, ep);

                            ep.Dispose();
                        }
                    }
                }

                // InPathフォルダの全てのtifファイルを削除する
                foreach (var files in System.IO.Directory.GetFiles(InPath, "*.tif"))
                {
                    System.IO.File.Delete(files);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///----------------------------------------------------------------------------------
        /// <summary>
        ///     WinReader P.Form を起動してOCR処理を実施する </summary>
        ///----------------------------------------------------------------------------------
        private void WinReaderOCR(string wrJobName)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // WinReaderJOB起動文字列
                string JobName = @"""" + wrJobName + @"""" + " /H2";
                string winReader_exe = Properties.Settings.Default.wrHands_Path + Properties.Settings.Default.wrHands_Prg;

                // ProcessStartInfo の新しいインスタンスを生成する
                System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();

                // 起動するアプリケーションを設定する
                p.FileName = winReader_exe;

                // コマンドライン引数を設定する（WinReaderのJOB起動パラメーター）
                p.Arguments = JobName;

                // WinReaderを起動します
                System.Diagnostics.Process hProcess = System.Diagnostics.Process.Start(p);

                // taskが終了するまで待機する
                hProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     winReaderHandsのerrorフォルダ内の画像をNGフォルダへ移動する </summary>
        /// <param name="wrNgPath">
        ///     winReaderHandsのerrorフォルダパス</param>
        /// <param name="outNgPath">
        ///     NGフォルダパス</param>
        ///----------------------------------------------------------------------------
        private void getNgImage(string wrNgPath, string outNgPath)
        {
            // WinReaderHandsのerrorパスの画像の存在を確認する
            if (System.IO.Directory.GetFiles(wrNgPath, "*.tif").Count() > 0)
            {
                // ファイル名（日付時間部分）
                string fName = string.Format("{0:0000}", DateTime.Today.Year) +
                        string.Format("{0:00}", DateTime.Today.Month) +
                        string.Format("{0:00}", DateTime.Today.Day) +
                        string.Format("{0:00}", DateTime.Now.Hour) +
                        string.Format("{0:00}", DateTime.Now.Minute) +
                        string.Format("{0:00}", DateTime.Now.Second);

                int _Pcnt = 0;

                foreach (var file in System.IO.Directory.GetFiles(wrNgPath, "*.tif"))
                {
                    _Pcnt++;

                    // ファイル名設定
                    string fnm = outNgPath + fName + string.Format("{0:000}", _Pcnt) + ".tif";

                    // 画像ファイルをリネームして移動
                    System.IO.File.Move(file, fnm);
                }
            }

            // WinReadHandsのerrorフォルダ内を空にする
            foreach (var f in System.IO.Directory.GetFiles(wrNgPath, "*.*"))
            {
                System.IO.File.Delete(f);
            }
        }

        ///-------------------------------------------------------------------------
        /// <summary>
        ///     MimeTypeで指定されたImageCodecInfoを探して返す </summary>
        /// <param name="mineType">
        ///     </param>
        /// <returns>
        ///     </returns>
        ///-------------------------------------------------------------------------
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string mineType)
        {
            //GDI+ に組み込まれたイメージ エンコーダに関する情報をすべて取得
            System.Drawing.Imaging.ImageCodecInfo[] encs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            //指定されたMimeTypeを探して見つかれば返す
            foreach (System.Drawing.Imaging.ImageCodecInfo enc in encs)
            {
                if (enc.MimeType == mineType)
                {
                    return enc;
                }
            }
            return null;
        }


        ///-----------------------------------------------------------------
        /// <summary>
        ///     伝票ＣＳＶデータを一枚ごとに分割する </summary>
        ///-----------------------------------------------------------------
        private void LoadCsvDivide_Nouhinsho(string fnm, ref int dNum, string outPath, string filePath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                string imgName = string.Empty;      // 画像ファイル名
                string firstFlg = global.FLGON;
                string[] stArrayData;               // CSVファイルを１行単位で格納する配列
                string newFnm = string.Empty;
                int dCnt = 0;   // 処理件数

                // 対象ファイルの存在を確認します
                if (!System.IO.File.Exists(filePath + Properties.Settings.Default.wrReaderOutFile))
                {
                    return;
                }

                // StreamReader の新しいインスタンスを生成する
                //入力ファイル
                System.IO.StreamReader inFile = new System.IO.StreamReader(filePath + Properties.Settings.Default.wrReaderOutFile, Encoding.Default);

                // 読み込んだ結果をすべて格納するための変数を宣言する
                string stResult = string.Empty;
                string stBuffer;

                // 行番号
                int sRow = 0;

                // 読み込みできる文字がなくなるまで繰り返す
                while (inFile.Peek() >= 0)
                {
                    // ファイルを 1 行ずつ読み込む
                    stBuffer = inFile.ReadLine();

                    // カンマ区切りで分割して配列に格納する
                    stArrayData = stBuffer.Split(',');

                    //先頭に「*」があったら新たな伝票なのでCSVファイル作成
                    if ((stArrayData[0] == "*"))
                    {
                        //最初の伝票以外のとき
                        if (firstFlg != global.FLGON)
                        {
                            //ファイル書き出し
                            outFileWrite(stResult, filePath + imgName, outPath + newFnm);
                        }

                        firstFlg = global.FLGOFF;

                        // 伝票連番
                        dNum++;

                        // 処理件数
                        dCnt++;

                        // ファイル名
                        newFnm = fnm + dNum.ToString().PadLeft(3, '0');

                        //画像ファイル名を取得
                        imgName = stArrayData[1];

                        //文字列バッファをクリア
                        stResult = string.Empty;

                        // 文字列再校正（画像ファイル名を変更する）
                        stBuffer = string.Empty;
                        for (int i = 0; i < stArrayData.Length; i++)
                        {
                            if (stBuffer != string.Empty)
                            {
                                stBuffer += ",";
                            }

                            // 画像ファイル名を変更する
                            if (i == 1)
                            {
                                stArrayData[i] = newFnm + ".tif"; // 画像ファイル名を変更
                            }

                            // フィールド結合
                            stBuffer += stArrayData[i];
                        }

                        sRow = 0;
                    }
                    else
                    {
                        sRow++;
                    }

                    // 読み込んだものを追加で格納する
                    stResult += (stBuffer + Environment.NewLine);
                }

                // 後処理
                if (dNum > 0)
                {
                    //ファイル書き出し
                    outFileWrite(stResult, filePath + imgName, outPath + newFnm);

                    // 入力ファイルを閉じる
                    inFile.Close();

                    //入力ファイル削除 : "txtout.csv"
                    Utility.FileDelete(filePath, Properties.Settings.Default.wrReaderOutFile);

                    //画像ファイル削除 : "WRH***.tif"
                    Utility.FileDelete(filePath, "WRH*.tif");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }


        ///-----------------------------------------------------------------
        /// <summary>
        ///     伝票ＣＳＶデータを一枚ごとに分割する </summary>
        ///-----------------------------------------------------------------
        private void LoadCsvDivide(string fnm, ref int dNum, string outPath, string filePath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                string imgName = string.Empty;      // 画像ファイル名
                string firstFlg = global.FLGON;
                string[] stArrayData;               // CSVファイルを１行単位で格納する配列
                string newFnm = string.Empty;
                int dCnt = 0;   // 処理件数

                // 対象ファイルの存在を確認します
                if (!System.IO.File.Exists(filePath + Properties.Settings.Default.wrReaderOutFile))
                {
                    return;
                }

                // StreamReader の新しいインスタンスを生成する
                //入力ファイル
                System.IO.StreamReader inFile = new System.IO.StreamReader(filePath + Properties.Settings.Default.wrReaderOutFile, Encoding.Default);

                // 読み込んだ結果をすべて格納するための変数を宣言する
                string stResult = string.Empty;
                string stBuffer;

                // 行番号
                int sRow = 0;

                // 読み込みできる文字がなくなるまで繰り返す
                while (inFile.Peek() >= 0)
                {
                    // ファイルを 1 行ずつ読み込む
                    stBuffer = inFile.ReadLine();

                    // カンマ区切りで分割して配列に格納する
                    stArrayData = stBuffer.Split(',');

                    //先頭に「*」があったら新たな伝票なのでCSVファイル作成
                    if ((stArrayData[0] == "*"))
                    {
                        //最初の伝票以外のとき
                        if (firstFlg != global.FLGON)
                        {
                            //ファイル書き出し
                            outFileWrite(stResult, filePath + imgName, outPath + newFnm);
                        }

                        firstFlg = global.FLGOFF;

                        // 伝票連番
                        dNum++;

                        // 処理件数
                        dCnt++;

                        // ファイル名
                        newFnm = fnm + dNum.ToString().PadLeft(3, '0');

                        //画像ファイル名を取得
                        imgName = stArrayData[1];

                        //文字列バッファをクリア
                        stResult = string.Empty;

                        // 文字列再校正（画像ファイル名を変更する）
                        stBuffer = string.Empty;
                        for (int i = 0; i < stArrayData.Length; i++)
                        {
                            if (stBuffer != string.Empty)
                            {
                                stBuffer += ",";
                            }

                            // 画像ファイル名を変更する
                            if (i == 1)
                            {
                                stArrayData[i] = newFnm + ".tif"; // 画像ファイル名を変更
                            }

                            // フィールド結合
                            stBuffer += stArrayData[i];
                        }

                        sRow = 0;
                    }
                    else
                    {
                        sRow++;
                    }

                    // 読み込んだものを追加で格納する
                    stResult += (stBuffer + Environment.NewLine);
                }

                // 後処理
                if (dNum > 0)
                {
                    //ファイル書き出し
                    outFileWrite(stResult, filePath + imgName, outPath + newFnm);

                    // 入力ファイルを閉じる
                    inFile.Close();

                    //入力ファイル削除 : "txtout.csv"
                    Utility.FileDelete(filePath, Properties.Settings.Default.wrReaderOutFile);

                    //画像ファイル削除 : "WRH***.tif"
                    Utility.FileDelete(filePath, "WRH*.tif");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     分割ファイルを書き出す </summary>
        /// <param name="tempResult">
        ///     書き出す文字列</param>
        /// <param name="tempImgName">
        ///     元画像ファイルパス</param>
        /// <param name="outFileName">
        ///     新ファイル名</param>
        ///----------------------------------------------------------------------------
        private void outFileWrite(string tempResult, string tempImgName, string outFileName)
        {
            //出力ファイル
            //System.IO.StreamWriter outFile = new System.IO.StreamWriter(Properties.Settings.Default.dataPath + outFileName + ".csv",
            //                                        false, System.Text.Encoding.GetEncoding(932));

            // 2017/11/20
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(outFileName + ".csv", false, System.Text.Encoding.GetEncoding(932));

            // ファイル書き出し
            outFile.Write(tempResult);

            //ファイルクローズ
            outFile.Close();

            //画像ファイルをコピー
            //System.IO.File.Copy(tempImgName, Properties.Settings.Default.dataPath + outFileName + ".tif");

            // 2017/11/20
            System.IO.File.Copy(tempImgName, outFileName + ".tif");
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     ＣＳＶデータからデータテーブルを生成する </summary>
        /// <param name="sPath">
        ///     CSVデータファイルパス</param>
        /// <returns>
        ///     データテーブル</returns>
        ///---------------------------------------------------------
        private DataTable readCSV(string sPath)
        {
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());

            ////パスの設定
            //string path = "CSVファイルのパス";

            //StreamReaderクラスのインスタンスの作成
            System.IO.StreamReader sr = new System.IO.StreamReader(sPath, Encoding.Default);

            //DataTableクラスのインスタンスの作成
            DataTable dt = new DataTable();

            //1行目を区切り文字(カンマ)で分割し列名を取得
            string[] items = sr.ReadLine().Split(',');

            //列の作成
            foreach (string item in items)
            {
                dt.Columns.Add(item.Replace("\"", ""), typeof(string));
            }

            //各行を読込み、テーブルを作成
            while (sr.Peek() != -1)
            {
                string[] values = sr.ReadLine().Split(',');

                DataRow dr = dt.NewRow();

                for (int ii = 0; ii < items.Length; ii++)
                {
                    dr[items[ii].Replace("\"", "")] = values[ii].Replace("\"", "");
                    //dr[items[ii]] = values[ii];
                }

                dt.Rows.Add(dr);
            }

            //StreamReaderクラスのインスタンスの破棄
            sr.Close();

            //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());

            return dt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.GetFiles(Properties.Settings.Default.ngPath, "*.tif").Count() == global.flgOff)
            {
                MessageBox.Show("ＮＧ画像はありません","",MessageBoxButtons.OK);
                return;
            }

            // ＮＧ画像確認画面
            this.Hide();
            frmNgRecovery frm = new frmNgRecovery();
            frm.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmEditLogRep frm = new frmEditLogRep();
            frm.ShowDialog();
            this.Show();
        }


        ///---------------------------------------------------------------
        /// <summary>
        ///     保存期間経過編集ログ削除 </summary>
        /// <param name="dM2">
        ///     編集ログ保存月数</param>
        ///---------------------------------------------------------------
        private void DeleteLogData(int dM2)
        {
            cn.Open();

            try
            {
                DateTime sdt = DateTime.Now.AddMonths(-1 * dM2);
                //DateTime sdt = DateTime.Now.AddDays(-1 * dM2); // デバッグ用

                string _sdt = sdt.Year + "/" + sdt.Month.ToString("D2") + "/" + sdt.Day.ToString("D2") + " " +
                              sdt.Hour.ToString("D2") + ":" + sdt.Minute.ToString("D2") + ":" + sdt.Second.ToString("D2");

                // 編集ログ
                string sql = "delete from 編集ログ ";
                sql += "where 年月日時刻 < '" + _sdt + "'";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn))
                {
                    com.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        ///-------------------------------------------------------------------
        /// <summary>
        ///     保存画像を削除する </summary>
        /// <param name="rootPath">
        ///     画像保存先ルートパス</param>
        /// <param name="mc">
        ///     保存月数</param>
        ///-------------------------------------------------------------------
        public void DeletePastImage(string rootPath, int mc)
        {
            if (mc == global.flgOff)
            {
                // 保存月数ゼロは削除しない
                return;
            }

            // 基準日取得（これ以前の日付の画像は削除する）
            DateTime dt = DateTime.Today.AddMonths(-1 * mc);
            int val_0 = dt.Year * 10000 + dt.Month * 100 + dt.Day;

            string[] subFolders = System.IO.Directory.GetDirectories(rootPath);

            foreach (var dir in subFolders)
            {
                foreach (var file in System.IO.Directory.GetFiles(dir, "*.tif"))
                {
                    string f = System.IO.Path.GetFileNameWithoutExtension(file);
                    int val_1 = Utility.StrtoInt(f.Substring(0, 8));

                    if (val_1 < val_0)
                    {
                        // 基準日以前なら削除
                        System.IO.File.Delete(file);
                    }
                }
            }
        }
    }
}
