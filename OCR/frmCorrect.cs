using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Data.Linq;
using IWT_OCR.Common;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace IWT_OCR.OCR
{
    public partial class frmCorrect : Form
    {
        public frmCorrect()
        {
            InitializeComponent();
        }

        string dID = string.Empty;              // 表示する発注データのID
        string _img = string.Empty;             // 画像名

        bool _eMode = true;

        // 仕入先名チェンジステータス
        bool ShiireNameChangeStatus = true;
        bool ShiireCodeChangeStatus = true;
        // 商品コードチェンジステータス
        bool HinCodeChangeStatus = true;

        bool gridViewCellEnterStatus = true;
        bool DataShowStatus = false;

        // 編集ログ書き込み状態
        bool editLogStatus = false;

        // カレントデータRowsインデックス
        string[] cID = null;
        int cI = 0;

        // グローバルクラス
        global gl = new global();

        Image OcrImg = null;

        // 表示中の伝票の伝票区分
        string DEN_KBN = "";

        // 画像サイズ
        float B_WIDTH = 0.30f;
        float B_HEIGHT = 0.30f;
        float n_width = 0f;
        float n_height = 0f;

        // splitContainer上部から境界線までの距離
        int SpDistance_NouhinKari = 446;
        int SpDistance_Genpin = 486;
        int SpDistance_Nouhin = 530;

        // カラム定義
        private readonly string colHinCode = "c0";
        private readonly string colHinName = "c1";
        private readonly string colKikaku = "c2";
        private readonly string colSuu = "c3";

        // データベース：Sqlite3
        SQLiteConnection cn = null;
        DataContext context = null;

        SQLiteConnection cn2 = null;
        DataContext context2 = null;

        string db_file = Properties.Settings.Default.DB_File;
        string Local_DB = Properties.Settings.Default.Local_DB;

        // 環境設定
        Table<Common.ClsSystemConfig> tblCfg = null;
        //ClsSystemConfig ClsSystemConfig = null;

        // 納品伝票
        Table<Common.ClsDeviveryNote> tblDeliv = null;
        ClsDeviveryNote ClsDeviveryNote = null;

        //// 保留データ
        //Table<Common.ClsHoldData> tblHold = null;
        //ClsHoldData clsHold = null;

        // セル値
        private string cellName = string.Empty;         // セル名
        private string cellBeforeValue = string.Empty;  // 編集前
        private string cellAfterValue = string.Empty;   // 編集後

        #region 編集ログ・項目名
        private const string LOG_YEAR = "年";
        private const string LOG_MONTH = "月";
        private const string LOG_DAY = "日";
        private const string LOG_SHIIRENAME = "仕入先名";
        private const string LOG_SHIIRECODE = "仕入先コード";
        private const string LOG_SYOHINCD = "商品コード";
        private const string LOG_SUU = "納入数";
        private const string LOG_BIKOU = "備考";
        private const string LOG_MEMO = "メモ欄";
        private const string LOG_DELETE = "伝票削除";
        private const string LOG_BUMON = "部門コード";
        private const string LOG_KAKEGENKIN = "掛現金区分";
        private const string LOG_SEIKYUNAME = "請求先名";
        private const string LOG_SEIKYUCODE = "請求先コード";
        private const string LOG_JYUCHUNAME = "受注先名";
        private const string LOG_JYUCHUCODE = "受注先コード";
        #endregion 編集ログ・項目名

        #region 終了ステータス定数
        const string END_BUTTON = "btn";
        const string END_MAKEDATA = "data";
        const string END_CONTOROL = "close";
        const string END_NODATA = "non Data";
        #endregion

        // 売上原価Proデータ出力先パス
        string CsvOutPath = string.Empty;
        // 画像保存先パス
        string TifOutPath = string.Empty;
        // 画像保存月数
        int ImageSpan = 0;
        // ログ保存月数
        int LogSpan = 0;

        private void frmCorrect_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            //Utility.WindowsMaxSize(this, this.Width, this.Height);
            //Utility.WindowsMinSize(this, this.Width, this.Height);

            // 共有DB接続
            cn = new SQLiteConnection("DataSource=" + db_file);
            context = new DataContext(cn);
            tblCfg = context.GetTable<Common.ClsSystemConfig>();        // 環境設定

            // ローカルDB接続
            cn2 = new SQLiteConnection("DataSource=" + Local_DB);
            context2 = new DataContext(cn2);
            tblDeliv = context2.GetTable<Common.ClsDeviveryNote>();     // 納品伝票

            // CSVデータをローカルマスターへ読み込みます
            GetCsvDataToSQLite();

            // DBオープン
            cn2.Open();

            // 環境設定テーブルを取得
            var cf = tblCfg.Single(a => a.ID == global.configKEY);
            CsvOutPath = cf.DataPath;   // CSV作成先パス
            TifOutPath = cf.ImgPath;    // 画像保存先パス
            ImageSpan = cf.DataSpan;    // 画像保存月数
            LogSpan = cf.LogSpan;       // ログ保存月数

            // データテーブル件数カウント
            if (tblDeliv.Count() == 0)
            {
                MessageBox.Show("納品書データがありません", "発注書登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //終了処理
                Environment.Exit(0);
            }

            // キー配列作成
            keyArrayCreate();

            // キャプション
            this.Text = "売上原価Proデータ作成";

            GridviewSet(dg1);

            // 画像サムネイル表示
            ShowThumbnail();

            // 最初のレコードを表示
            cI = 0;
            //showOcrData(cI);

            // リストビューセレクト
            ListViewSelect(cID[cI]);

            // tagを初期化
            this.Tag = string.Empty;

            // 現在の表示倍率を初期化
            gl.miMdlZoomRate = 0f;

            //// 部門コンボボックス
            ////Utility.ComboBumon.Load(cmbBumon, global.dtBumon);
            //cmbBumon.DataSource = global.dtBumon;
            //cmbBumon.DisplayMember = "部門名";
            //cmbBumon.ValueMember = "部門コード";
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     キー配列作成 </summary>
        ///-------------------------------------------------------------
        private void keyArrayCreate()
        {
            //MessageBox.Show(tblFax.Count().ToString());

            int iX = 0;
            foreach (var t in tblDeliv.OrderBy(a => a.ID))
            {
                Array.Resize(ref cID, iX + 1);
                cID[iX] = t.ID.ToString();
                iX++;
            }
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     納品伝票データグリッドビュー定義 </summary>
        ///------------------------------------------------------------------------
        private void GridviewSet(DataGridViewEx tempDGV)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する

                tempDGV.EnableHeadersVisualStyles = false;
                tempDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.Lavender;
                tempDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;

                tempDGV.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 列ヘッダーフォント指定
                tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("ＭＳ ゴシック", (float)(11), FontStyle.Regular);

                // データフォント指定
                tempDGV.DefaultCellStyle.Font = new Font("ＭＳ ゴシック", 11, FontStyle.Regular);

                // 行の高さ
                tempDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                //tempDGV.ColumnHeadersHeight = 20;
                tempDGV.RowTemplate.Height = 20;

                // 全体の高さ
                tempDGV.Height = 122;

                // 奇数行の色
                tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add(colHinCode, "部品コード");
                tempDGV.Columns.Add(colHinName, "品名");
                tempDGV.Columns.Add(colKikaku, "規格・型番");
                tempDGV.Columns.Add(colSuu, "納入数");

                tempDGV.Columns[colHinCode].Width = 200;
                //tempDGV.Columns[colHinName].Width = 70;
                tempDGV.Columns[colKikaku].Width = 350;
                tempDGV.Columns[colSuu].Width = 100;

                tempDGV.Columns[colHinName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colHinCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colSuu].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                //tempDGV.Columns[colSuu].DefaultCellStyle.Format = "#,#";

                // 編集可否
                tempDGV.ReadOnly = false;

                // 列ごとの設定
                foreach (DataGridViewColumn c in tempDGV.Columns)
                {
                    // 編集可否
                    if (c.Name == colHinName || c.Name == colKikaku)
                    {
                        c.ReadOnly = true;
                    }
                    else
                    {
                        c.ReadOnly = false;
                    }

                    //// フォントサイズ
                    //if (c.Name == colMaker)
                    //{
                    //    c.DefaultCellStyle.Font = new Font("ＭＳ ゴシック", (float)(9.5), FontStyle.Regular);
                    //}
                    //else if (c.Name == colDay1 || c.Name == colDay2 || c.Name == colDay3 || c.Name == colDay4 ||
                    //         c.Name == colDay5 || c.Name == colDay6 || c.Name == colDay7)
                    //{
                    //    c.DefaultCellStyle.Font = new Font("ＭＳ ゴシック", 11, FontStyle.Regular);
                    //}
                    //else if (c.Name == colSyubai)
                    //{
                    //    c.DefaultCellStyle.Font = new Font("ＭＳ ゴシック", 9, FontStyle.Regular);
                    //}
                }

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
                tempDGV.MultiSelect = false;

                // 編集モード
                tempDGV.EditMode = DataGridViewEditMode.EditOnEnter;

                // 追加行表示しない
                tempDGV.AllowUserToAddRows = false;

                // データグリッドビューから行削除を禁止する
                tempDGV.AllowUserToDeleteRows = false;

                // 手動による列移動の禁止
                tempDGV.AllowUserToOrderColumns = false;

                // 列サイズ変更禁止
                tempDGV.AllowUserToResizeColumns = true;

                // 行サイズ変更禁止
                tempDGV.AllowUserToResizeRows = false;

                //TAB動作
                tempDGV.StandardTab = false;

                // Enter次行移動先カラム
                global.NEXT_COLUMN = colHinCode;

                // 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                tempDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // ソート不可
                foreach (DataGridViewColumn c in tempDGV.Columns)
                {
                    c.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // 罫線
                //tempDGV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                //tempDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;

                // コンテキストメニュー
                //tempDGV.ContextMenuStrip = this.contextMenuStrip1;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     CSVデータをMDBへインサートする</summary>
        /// <param name="DataPath">
        ///     データフォルダパス</param>
        ///----------------------------------------------------------------------------
        private void GetCsvDataToSQLite()
        {
            // CSVファイル数をカウント
            string[] inCsv = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath, "*.csv");

            // CSVファイルがなければ終了
            if (inCsv.Length == 0)
            {
                return;
            }

            // オーナーフォームを無効にする
            this.Enabled = false;

            //プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = this;
            frmP.Show();

            // OCRのCSVデータをSQLiteへ取り込む
            CsvToSQLite(Properties.Settings.Default.dataPath, frmP);

            // いったんオーナーをアクティブにする
            this.Activate();

            // 進行状況ダイアログを閉じる
            frmP.Close();

            // オーナーのフォームを有効に戻す
            this.Enabled = true;
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     納品仮伝票CSVデータをSQLiteに登録する </summary>
        /// <param name="_inPath">
        ///     CSVデータパス</param>
        /// <param name="frmP">
        ///     プログレスバーフォームオブジェクト</param>
        ///-----------------------------------------------------------------------
        private void CsvToSQLite(string _inPath, frmPrg frmP)
        {
            //ClsDeviveryNote clsDevivery = null;

            try
            {
                // 対象CSVファイル数を取得
                int cLen = System.IO.Directory.GetFiles(_inPath, "*.csv").Count();

                //CSVデータをSQLiteへ取込
                int cCnt = 0;
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    //件数カウント
                    cCnt++;

                    //プログレスバー表示
                    frmP.Text = "ＯＣＲ認識ＣＳＶデータロード中　" + cCnt.ToString() + "/" + cLen.ToString();
                    frmP.progressValue = cCnt * 100 / cLen;
                    frmP.ProgressStep();

                    // CSVファイルインポート
                    foreach (var stBuffer in System.IO.File.ReadAllLines(files, Encoding.Default))
                    {
                        // カンマ区切りで分割して配列に格納する
                        string[] stCSV = stBuffer.Split(',');

                        switch (stCSV[2])
                        {
                            // 現品票
                            case global.DEN_GENPIN:
                                SetDeviveryData_Genpin(stCSV);
                                break;

                            // 納品仮伝票
                            case global.DEN_NOUHINKARI:
                                SetDeviveryData_NouhinKari(stCSV);
                                break;

                            // 納品書
                            case global.DEN_NOUHIN:
                                SetDeviveryData_Nouhin(stCSV);
                                break;

                            default:
                                break;
                        }
                    }
                }

                // ローカルのデータベースを更新
                context2.SubmitChanges();

                //CSVファイルを削除する
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    System.IO.File.Delete(files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ＯＣＲ認識ＣＳＶデータインポート処理", MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     現品票CSVデータSQLiteへ取り込み </summary>
        /// <param name="s">
        ///     CSVデータ配列</param>
        ///------------------------------------------------------------------
        private void SetDeviveryData_Genpin(string [] s)
        {
            if (s.Length < 7)
            {
                return;
            }

            ClsDeviveryNote clsDevivery = null;

            clsDevivery = new ClsDeviveryNote()
            {
                ID = Utility.GetStringSubMax(s[1].Trim(), 17),
                DenKbn = Utility.StrtoInt(global.DEN_GENPIN),
                ImageFileName = Utility.GetStringSubMax(s[1].Trim(), 21),
                NounyuDate = s[4].Trim().Replace("年", "/").Replace("月", "/").Replace("日", ""),
                BuhinCode_1 = s[5].Trim(),
                Suu_1 = s[6].Trim(),
                BuhinCode_2 = string.Empty,
                Suu_2 = string.Empty,
                BuhinCode_3 = string.Empty,
                Suu_3 = string.Empty,
                BuhinCode_4 = string.Empty,
                Suu_4 = string.Empty,
                BuhinCode_5 = string.Empty,
                Suu_5 = string.Empty,
                NonyuName = s[3].Trim(),
                NonyuCode = global.flgOff,
                Bikou = s[7].Trim(),
                memo = string.Empty,
                Check = global.flgOff,
                YyMmDd = DateTime.Now.ToString(),
                KakeGenkin = global.flgOn,
                JyuchuCode = global.flgOff,
                SeikyuuCode = global.flgOff,
                BumonCode = global.flgOff
            };

            // 納品伝票データを追加登録する
            tblDeliv.InsertOnSubmit(clsDevivery);
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     納品仮伝票CSVデータSQLiteへ取り込み </summary>
        /// <param name="s">
        ///     CSVデータ配列</param>
        ///------------------------------------------------------------------
        private void SetDeviveryData_NouhinKari(string[] s)
        {
            if (s.Length < 8)
            {
                return;
            }

            ClsDeviveryNote clsDevivery = null;

            string[] BuhinArray = new string[5];
            string[] SuuArray = new string[5];

            for (int i = 0; i < 5; i++)
            {
                BuhinArray[i] = "";
                SuuArray[i] = "";
            }

            int iX = 0;

            // 部品コード,数量を区切り文字で分割して配列に入れる
            foreach (var item in s[5].Trim().Split('/'))
            {
                BuhinArray[iX] = item;
                iX++;
            }

            iX = 0;
            foreach (var item in s[6].Trim().Split('/'))
            {
                SuuArray[iX] = item;
                iX++;
            }

            clsDevivery = new ClsDeviveryNote()
            {
                ID = Utility.GetStringSubMax(s[1].Trim(), 17),
                DenKbn = Utility.StrtoInt(global.DEN_NOUHINKARI),
                ImageFileName = Utility.GetStringSubMax(s[1].Trim(), 21),
                NounyuDate = s[4].Trim().Replace("年", "/").Replace("月", "/").Replace("日", "").Replace(" ", ""),
                BuhinCode_1 = BuhinArray[0].Trim(),
                Suu_1 = SuuArray[0].Trim(),
                BuhinCode_2 = BuhinArray[1].Trim(),
                Suu_2 = SuuArray[1].Trim(),
                BuhinCode_3 = BuhinArray[2].Trim(),
                Suu_3 = SuuArray[2].Trim(),
                BuhinCode_4 = BuhinArray[3].Trim(),
                Suu_4 = SuuArray[3].Trim(),
                BuhinCode_5 = BuhinArray[4].Trim(),
                Suu_5 = SuuArray[4].Trim(),
                NonyuName = s[3].Trim(),
                NonyuCode = global.flgOff,
                Bikou = s[7].Trim(),
                memo = string.Empty,
                Check = global.flgOff,
                YyMmDd = DateTime.Now.ToString(),
                KakeGenkin = global.flgOn,
                JyuchuCode = global.flgOff,
                SeikyuuCode = global.flgOff,
                BumonCode = global.flgOff
            };

            // 納品伝票データを追加登録する
            tblDeliv.InsertOnSubmit(clsDevivery);
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     納品書CSVデータSQLiteへ取り込み </summary>
        /// <param name="s">
        ///     CSVデータ配列</param>
        ///------------------------------------------------------------------
        private void SetDeviveryData_Nouhin(string[] s)
        {
            if (s.Length < 8)
            {
                return;
            }

            ClsDeviveryNote clsDevivery = null;

            clsDevivery = new ClsDeviveryNote()
            {
                ID = Utility.GetStringSubMax(s[1].Trim(), 17),
                DenKbn = Utility.StrtoInt(global.DEN_NOUHIN),
                ImageFileName = Utility.GetStringSubMax(s[1].Trim(), 21),
                NounyuDate = s[4].Trim().Replace("年", "/").Replace("月", "/").Replace("日", ""),
                BuhinCode_1 = s[5].Trim(),
                Suu_1 = s[6].Trim(),
                BuhinCode_2 = s[7].Trim(),
                Suu_2 = s[8].Trim(),
                BuhinCode_3 = s[9].Trim(),
                Suu_3 = s[10].Trim(),
                BuhinCode_4 = s[11].Trim(),
                Suu_4 = s[12].Trim(),
                BuhinCode_5 = s[13].Trim(),
                Suu_5 = s[14].Trim(),
                NonyuName = s[3].Trim(),
                NonyuCode = global.flgOff,
                Bikou = s[15].Trim(),
                memo = string.Empty,
                Check = global.flgOff,
                YyMmDd = DateTime.Now.ToString(),
                KakeGenkin = global.flgOn,
                JyuchuCode = global.flgOff,
                SeikyuuCode = global.flgOff,
                BumonCode = global.flgOff
            };

            // 納品伝票データを追加登録する
            tblDeliv.InsertOnSubmit(clsDevivery);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 非ログ書き込み状態とする
            editLogStatus = false;

            // フォームを閉じる
            this.Tag = END_BUTTON;
            Close();
        }

        private void frmCorrect_FormClosing(object sender, FormClosingEventArgs e)
        {
            //「受入データ作成終了」「勤務票データなし」以外での終了のとき
            if (this.Tag.ToString() != END_MAKEDATA && this.Tag.ToString() != END_NODATA)
            {
                //if (MessageBox.Show("終了します。よろしいですか", "終了確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //{
                //    e.Cancel = true;
                //    return;
                //}

                // カレントデータ更新
                if (dID == string.Empty)
                {
                    CurDataUpDate(cI);
                }
            }

            // 編集ログデータアップロード
            EditDataUpload();

            //データベース接続解除
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }

            if (cn2.State == ConnectionState.Open)
            {
                cn2.Close();
            }

            // 解放する
            this.Dispose();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("表示中の伝票を保留にします。よろしいですか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            //カレントデータの更新
            CurDataUpDate(cI);

            // 保留処理
            setHoldData(cID[cI]);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cI);

            //レコードの移動
            cI = 0;
            //showOcrData(cI);

            // リストビューセレクト
            DataShowStatus = false;
            ListViewSelect(cID[cI]);
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     リストビューのアイテムを選択状態にする </summary>
        /// <param name="val">
        ///     ListViewの値</param>
        ///--------------------------------------------------------------------
        private void ListViewSelect(string val)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Text.Replace(".tif", "") == val)
                {
                    listView1.Items[i].Selected = true;
                    listView1.EnsureVisible(i);
                    break;
                }
            }
        }

        private void btnBefore_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cI);

            //レコードの移動
            if (cI > 0)
            {
                cI--;
                //showOcrData(cI);
            }

            // リストビューセレクト
            DataShowStatus = false;
            ListViewSelect(cID[cI]);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cI);

            //レコードの移動
            if (cI + 1 < cID.Length)
            {
                cI++;
                //showOcrData(cI);
            }

            // リストビューセレクト
            DataShowStatus = false;
            ListViewSelect(cID[cI]);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cI);

            //レコードの移動
            cI = cID.Length - 1;
            //showOcrData(cI);

            // リストビューセレクト
            DataShowStatus = false;
            ListViewSelect(cID[cI]);
        }

        ///-----------------------------------------------------------------------------------
        /// <summary>
        ///     カレントデータを更新する</summary>
        /// <param name="iX">
        ///     カレントレコードのインデックス</param>
        ///-----------------------------------------------------------------------------------
        private void CurDataUpDate(int iX)
        {
            // エラーメッセージ
            string errMsg = "納品伝票データ更新";

            //cn2.Open();

            try
            {
                string Sql = "UPDATE 納品伝票 set ";
                Sql += "納入年月日 = '" + Utility.NulltoStr(txtYear.Text) + "/" + Utility.NulltoStr(txtMonth.Text) + "/" + Utility.NulltoStr(txtDay.Text) + "',";
                Sql += "納入先名 = '" + Utility.NulltoStr(txtShiireName.Text).Replace(",", "").Replace("'", "''") + "',";
                Sql += "納入先コード = " + Utility.StrtoInt(Utility.NulltoStr(txtShiireCode.Text)) + ",";
                Sql += "備考 = '" + Utility.NulltoStr(txtBikou.Text).Replace(",", "").Replace("'", "''") + "',";
                Sql += "メモ = '" + Utility.NulltoStr(txtMemo.Text).Replace(",", "").Replace("'", "''") + "',";
                Sql += "部品コード1 = '" + Utility.NulltoStr(dg1[colHinCode, 0].Value).Replace(",", "").Replace("'", "''") + "',";
                Sql += "数量1 = '" + Utility.NulltoStr(dg1[colSuu, 0].Value).Replace(",", "") + "',";
                Sql += "部品コード2 = '" + Utility.NulltoStr(dg1[colHinCode, 1].Value).Replace(",", "").Replace("'", "''") + "',";
                Sql += "数量2 = '" + Utility.NulltoStr(dg1[colSuu, 1].Value).Replace(",", "") + "',";
                Sql += "部品コード3 = '" + Utility.NulltoStr(dg1[colHinCode, 2].Value).Replace(",", "").Replace("'", "''") + "',";
                Sql += "数量3 = '" + Utility.NulltoStr(dg1[colSuu, 2].Value).Replace(",", "") + "',";
                Sql += "部品コード4 = '" + Utility.NulltoStr(dg1[colHinCode, 3].Value).Replace(",", "").Replace("'", "''") + "',";
                Sql += "数量4 = '" + Utility.NulltoStr(dg1[colSuu, 3].Value).Replace(",", "") + "',";
                Sql += "部品コード5 = '" + Utility.NulltoStr(dg1[colHinCode, 4].Value).Replace(",", "").Replace("'", "''") + "',";
                Sql += "数量5 = '" + Utility.NulltoStr(dg1[colSuu, 4].Value).Replace(",", "") + "',";
                Sql += "確認 = " + Convert.ToInt32(checkBox1.Checked) + ",";
                Sql += "更新年月日 = '" + DateTime.Now.ToString() + "',";
                Sql += "掛現金区分 = " + (comboBox1.SelectedIndex + 1) + ",";
                //Sql += "部門コード = " + Utility.StrtoInt(Utility.NulltoStr(cmbBumon.SelectedValue).ToString()) + ",";  // 2020/08/17 コメント化
                Sql += "部門コード = " + global.flgOff + ",";    // 2020/08/17
                Sql += "受注先コード = " + global.flgOff + ",";
                Sql += "請求先コード = " + global.flgOff + " ";
                Sql += "WHERE ID = '" + cID[iX] + "'";

                using (SQLiteCommand com = new SQLiteCommand(Sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                // 納品伝票テーブル読み込む
                context2 = new DataContext(cn2);
                tblDeliv = context2.GetTable<Common.ClsDeviveryNote>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, errMsg, MessageBoxButtons.OK);
            }
            finally
            {

            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cI);

            //レコードの移動
            cI = hScrollBar1.Value;
            //showOcrData(cI);

            // リストビューセレクト
            ListViewSelect(cID[cI]);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            n_width = B_WIDTH + (float)trackBar1.Value * 0.05f;
            n_height = B_HEIGHT + (float)trackBar1.Value * 0.05f;

            imgShow(OcrImg, n_width, n_height);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            ImageRotate(pictureBox1.Image);
        }

        ///-------------------------------------------------------
        /// <summary>
        ///     画像回転 </summary>
        /// <param name="img">
        ///     Image</param>
        ///-------------------------------------------------------
        private void ImageRotate(Image img)
        {
            Bitmap bmp = (Bitmap)img;

            // 反転せず時計回りに90度回転する
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

            //表示
            pictureBox1.Image = img;
        }

        private void dg1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            // 商品コード
            if (e.ColumnIndex == 0)
            {
                if (!HinCodeChangeStatus)
                {
                    return;
                }

                // 大文字に変換
                string syCD = Utility.NulltoStr(dg1[colHinCode, e.RowIndex].Value).ToString().ToUpper();

                // 商品マスター取得
                ClsCsvData.ClsCsvSyohin cls = Utility.GetSyohinsFromDataTable(syCD, global.dtSyohin);

                dg1[colHinName, e.RowIndex].Value = cls.SyohinName;
                if (cls.SyohinCode != "")
                {
                    HinCodeChangeStatus = false;
                    dg1[colHinCode, e.RowIndex].Value = cls.SyohinCode;
                }

                HinCodeChangeStatus = true;
            }
        }

        private void btnErrCheck_Click(object sender, EventArgs e)
        {
            // 非ログ書き込み状態とする：2015/09/25
            editLogStatus = false;

            // OCRDataクラス生成
            OCRData ocr = new OCRData();

            // エラーチェックを実行
            if (getErrData(cI, ocr))
            {
                System.Diagnostics.Debug.WriteLine("エラーチェック...エラーなし：" + cID[cI].ToString());

                MessageBox.Show("エラーはありませんでした", "エラーチェック", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // データ表示：エラーを消すため表示中データを再表示
                showOcrData(cI);

                //// リストビューセレクト
                //ListViewSelect(cID[cI]);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("エラーチェック...エラーあり：" + cID[ocr._errHeaderIndex].ToString());

                if (cI != ocr._errHeaderIndex)
                {
                    // カレントインデックスをエラーありインデックスで更新
                    cI = ocr._errHeaderIndex;

                    //// データ表示
                    //showOcrData(cI);

                    // リストビューセレクト
                    DataShowStatus = false;
                    ListViewSelect(cID[cI]);
                }
                else
                {
                    // 表示中画面でエラー有りのとき
                    // データ表示：エラーを消すため表示中データを再表示
                    showOcrData(cI);
                }

                // エラー表示
                ErrShow(ocr);
            }
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     エラーチェックを実行する</summary>
        /// <param name="cIdx">
        ///     現在表示中の勤務票ヘッダデータインデックス</param>
        /// <param name="ocr">
        ///     OCRDATAクラスインスタンス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        /// -----------------------------------------------------------------------------------
        private bool getErrData(int cIdx, OCRData ocr)
        {
            // カレントレコード更新
            CurDataUpDate(cI);

            // エラー番号初期化
            ocr._errNumber = ocr.eNothing;

            // エラーメッセージクリーン
            ocr._errMsg = string.Empty;

            // エラーチェック実行①:カレントレコードから最終レコードまで
            if (!ocr.errCheckMain(cIdx, cID.Length - 1, this, tblDeliv, cID))
            {
                return false;
            }

            // エラーチェック実行②:最初のレコードからカレントレコードの前のレコードまで
            if (cIdx > 0)
            {
                if (!ocr.errCheckMain(0, (cIdx - 1), this, tblDeliv, cID))
                {
                    return false;
                }
            }

            // エラーなし
            lblErrMsg.Text = string.Empty;

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     エラー表示 </summary>
        /// <param name="ocr">
        ///     OCRDATAクラス</param>
        ///------------------------------------------------------------------------------------
        private void ErrShow(OCRData ocr)
        {
            if (ocr._errNumber != ocr.eNothing)
            {
                // グリッドビューCellEnterイベント処理は実行しない
                gridViewCellEnterStatus = false;

                lblErrMsg.Visible = true;
                lblErrMsg.Text = ocr._errMsg;

                // 確認
                if (ocr._errNumber == ocr.eDataCheck)
                {
                    checkBox1.BackColor = Color.Yellow;
                    checkBox1.Focus();
                }

                // 年月
                if (ocr._errNumber == ocr.eYearMonth)
                {
                    txtYear.BackColor = Color.Yellow;
                    txtYear.Focus();
                }

                if (ocr._errNumber == ocr.eMonth)
                {
                    txtMonth.BackColor = Color.Yellow;
                    txtMonth.Focus();
                }

                // 仕入先コード
                if (ocr._errNumber == ocr.eShiireNo)
                {
                    txtShiireCode.BackColor = Color.Yellow;
                    txtShiireCode.Focus();
                }

                // 商品コード
                if (ocr._errNumber == ocr.eHinCode)
                {
                    dg1[colHinCode, ocr._errRow].Style.BackColor = Color.Yellow;
                    dg1.Focus();
                    dg1.CurrentCell = dg1[colHinCode, ocr._errRow];
                }

                // 納入数
                if (ocr._errNumber == ocr.eSuu)
                {
                    dg1[colSuu, ocr._errRow].Style.BackColor = Color.Yellow;
                    dg1.Focus();
                    dg1.CurrentCell = dg1[colSuu, ocr._errRow];
                }

                // 受注先コード
                if (ocr._errNumber == ocr.eJyuchuuCode)
                {
                    txtShiireCode.BackColor = Color.Yellow;
                    txtShiireCode.Focus();
                }

                //// 請求先コード
                //if (ocr._errNumber == ocr.eSeikyuuCode)
                //{
                //    txtSeikyuuCode.BackColor = Color.Yellow;
                //    txtSeikyuuCode.Focus();
                //}

                // 部門コード
                if (ocr._errNumber == ocr.eBumom)
                {
                    cmbBumon.BackColor = Color.Yellow;
                    cmbBumon.Focus();
                }

                // グリッドビューCellEnterイベントステータスを戻す
                gridViewCellEnterStatus = true;
            }
        }

        private void txtShiireName_TextChanged(object sender, EventArgs e)
        {
            //// 仕入先コード逆引き
            ////if (Utility.StrtoInt(Utility.NulltoStr(txtShiireCode.Text.Trim())) == global.flgOff)
            ////{

            //if (!ShiireNameChangeStatus)
            //{
            //    return;
            //}

            //string sName = GetWithoutCorp(txtShiireName.Text);

            //ClsCsvData.ClsCsvShiiresaki[] clsCsv = Utility.GetShiireCodeFromDataTable(sName, global.dtShiire);

            //if (clsCsv == null)
            //{
            //    txtShiireCode.Text = "該当仕入先なし";
            //}
            //else if (clsCsv.Length == 1)
            //{
            //    ShiireNameChangeStatus = false;
            //    ShiireCodeChangeStatus = false;

            //    // 該当件数が1件のとき表示
            //    txtShiireCode.Text = clsCsv[0].ShiireCode.ToString();
            //    //txtShiireName.Text = clsCsv[0].ShiireName.ToString();
            //}
            //else if (clsCsv.Length > 1)
            //{
            //    txtShiireCode.Text = "複数の仕入先が該当しました";
            //}

            //ShiireNameChangeStatus = true;
            //ShiireCodeChangeStatus = true;
            ////}
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + "1...listView1_ItemSelectionChanged");

            if (DataShowStatus)
            {
                System.Diagnostics.Debug.WriteLine("2...データ更新開始：" + cID[cI].ToString());

                //カレントデータの更新
                CurDataUpDate(cI);

                System.Diagnostics.Debug.WriteLine("3...データ更新終了：" + cID[cI].ToString());
            }

            if (listView1.SelectedItems.Count > 0)
            {
                bool s = false;

                for (int i = 0; i < cID.Length; i++)
                {
                    if (cID[i] == listView1.SelectedItems[0].Text.Replace(".tif", ""))
                    {
                        cI = i;
                        s = true;
                        break;
                    }
                }

                trackBar1.Value = 0;

                // データ表示
                if (s)
                {
                    System.Diagnostics.Debug.WriteLine("4...表示する画像 " + cID[cI] + ":" + cI);
                    showOcrData(cI);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("画像を印刷します。よろしいですか？", "印刷確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            // 印刷実行
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            printDocument1.DefaultPageSettings.Landscape = true;
            printDocument1.PrinterSettings.PrinterName = pd.PrinterSettings.PrinterName;       // デフォルトプリンタを設定
            printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image img;

            img = Image.FromFile(_img);

            // 元画像のピクセル調整を行わないことによる縮小調整
            if (DEN_KBN == global.DEN_GENPIN || DEN_KBN == global.DEN_NOUHINKARI)
            {
                e.Graphics.DrawImage(img, 0, 0, img.Width * 33 / 100, img.Height * 33 / 100);
            }
            else if (DEN_KBN == global.DEN_NOUHIN)
            {
                e.Graphics.DrawImage(img, 0, 0, img.Width * 24 / 100, img.Height * 24 / 100);
            }

            e.HasMorePages = false;

            MessageBox.Show("印刷が終了しました");
            img.Dispose();
        }


        ///------------------------------------------------------------------
        /// <summary>
        ///     FAX発注書削除  </summary>
        ///------------------------------------------------------------------
        private void DenpyoDelete()
        {
            if (MessageBox.Show("表示中の伝票を削除します。よろしいですか", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            //// 非ログ書き込み状態とする
            //editLogStatus = false;

            // レコードと画像ファイルを削除する
            DataDelete(cI);

            // ログ書き込み
            LogDataUpdate(0, 0, global.flgOff, LOG_DELETE);

            // 件数カウント
            if (tblDeliv.Count() > 0)
            {
                // 配列キー再構築
                keyArrayCreate();

                // カレントレコードインデックスを再設定
                if (cID.Length - 1 < cI)
                {
                    cI = cID.Length - 1;
                }

                //// データ画面表示　2020/07/06 コメント化
                //showOcrData(cI);

                // 画像サムネイル表示
                ShowThumbnail();

                // リストビュー選択
                DataShowStatus = false;
                ListViewSelect(cID[cI]);
            }
            else
            {
                // ゼロならばプログラム終了
                MessageBox.Show("全ての伝票データが削除されました。処理を終了します。", "発注書削除", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //終了処理
                this.Tag = END_NODATA;
                this.Close();
            }
        }

        ///-------------------------------------------------------------------------------
        /// <summary>
        ///     １．指定した伝票データを削除する　
        ///     ２．該当する画像データを削除する</summary>
        ///-------------------------------------------------------------------------------
        private void DataDelete(int iX)
        {
            string errMsg = string.Empty;

            try
            {
                // 画像ファイル名を取得します
                string sImgNm = System.IO.Path.GetFileName(_img);

                errMsg = "伝票データ削除";

                // 発注書データを削除します
                string sql = "Delete from 納品伝票 ";
                sql += "WHERE ID = '" + cID[iX] + "'";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                // 画像ファイルを削除します
                errMsg = "伝票画像";
                if (sImgNm != string.Empty)
                {
                    if (System.IO.File.Exists(Properties.Settings.Default.dataPath + sImgNm))
                    {
                        System.IO.File.Delete(Properties.Settings.Default.dataPath + sImgNm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(errMsg + "の削除に失敗しました" + Environment.NewLine + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
            }
        }

        /// ----------------------------------------------------------------------
        /// <summary>
        ///     編集ログデータ書き込み </summary>
        /// <param name="rIndex">
        ///     データグリッドビュー行インデックス</param>
        /// <param name="iX">
        ///     列番号</param>
        /// <param name="dType">
        ///     データタイプ　0:ヘッダーデータ, 1:発注明細データ</param>
        /// <param name="colName">
        ///     カラム名</param>
        /// ----------------------------------------------------------------------
        private void LogDataUpdate(int rIndex, int iX, int dType, string colName)
        {
            //cn.Open();

            try
            {
                DateTime NowDate = DateTime.Now;

                // データ追加
                string sql = "insert into 編集ログ ";
                sql += "(年月日時刻, 項目名, 変更前値, 変更後値, 画像名, 編集アカウントID, コンピューター名, 更新年月日) ";
                sql += "values ('";
                sql += NowDate.Year + "/" + NowDate.Month.ToString("D2") + "/" + NowDate.Day.ToString("D2") + " " +
                       NowDate.Hour.ToString("D2") + ":" + NowDate.Minute.ToString("D2") + ":" + NowDate.Second.ToString("D2") + "','";    // 年月日時刻
                sql += colName + "','";                         // 項目名
                sql += cellBeforeValue + "','";                 // 変更前値
                sql += cellAfterValue + "','";                  // 変更後値
                sql += ClsDeviveryNote.ImageFileName + "','";   // 画像名
                sql += "','";                                   // 編集アカウントID
                sql += System.Net.Dns.GetHostName() + "','";    // コンピュータ名
                sql += DateTime.Now.ToString() + "');";         // 更新年月日
                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
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
                //cn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 伝票削除
            DenpyoDelete();
        }
        private void txtYear_Enter(object sender, EventArgs e)
        {
            if (editLogStatus)
            {
                // 年
                if (sender == txtYear)
                {
                    cellName = LOG_YEAR;
                }

                // 月
                if (sender == txtMonth)
                {
                    cellName = LOG_MONTH;
                }

                // 日
                if (sender == txtDay)
                {
                    cellName = LOG_DAY;
                }

                // 仕入先名
                if (sender == txtShiireName)
                {
                    if (DEN_KBN == global.DEN_NOUHINKARI || DEN_KBN == global.DEN_GENPIN)
                    {
                        // 仕入先名編集ログ
                        cellName = LOG_SHIIRENAME;
                    }
                    else if (DEN_KBN == global.DEN_NOUHIN)
                    {
                        // 受注先名編集ログ
                        cellName = LOG_JYUCHUNAME;
                    }
                }

                // 仕入先コード
                if (sender == txtShiireCode)
                {
                    if (DEN_KBN == global.DEN_NOUHINKARI || DEN_KBN == global.DEN_GENPIN)
                    {
                        // 仕入先コード編集ログ
                        cellName = LOG_SHIIRECODE;
                    }
                    else if (DEN_KBN == global.DEN_NOUHIN)
                    {
                        // 受注先コード編集ログ
                        cellName = LOG_JYUCHUCODE;
                    }
                }

                //// 請求先コード
                //if (sender == txtSeikyuuCode)
                //{
                //    cellName = LOG_SEIKYUCODE;
                //}

                // 備考
                if (sender == txtBikou)
                {
                    cellName = LOG_BIKOU;
                }

                // メモ
                if (sender == txtMemo)
                {
                    cellName = LOG_MEMO;
                }

                //// 部門コード
                //if (sender == txtBumon)
                //{
                //    cellName = LOG_BUMON;
                //}

                TextBox tb = (TextBox)sender;

                // 値を保持
                cellBeforeValue = Utility.NulltoStr(tb.Text).Replace("'", "''");
            }
        }

        private void txtYear_Leave(object sender, EventArgs e)
        {
            if (editLogStatus)
            {
                TextBox tb = (TextBox)sender;
                cellAfterValue = Utility.NulltoStr(tb.Text).Replace("'", "''");

                // 変更のとき編集ログデータを書き込み
                if (cellBeforeValue != cellAfterValue)
                {
                    LogDataUpdate(0, 0, global.flgOff, cellName);
                }
            }
        }

        private void dg1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // ログ書き込み状態のとき、値を保持する
            if (editLogStatus)
            {
                // 部品コード
                if (e.ColumnIndex == 0)
                {
                    cellName = LOG_SYOHINCD;
                }

                // 納入数
                if (e.ColumnIndex == 3)
                {
                    cellName = LOG_SUU;
                }

                cellBeforeValue = Utility.NulltoStr(dg1[e.ColumnIndex, e.RowIndex].Value);
            }
        }

        private void dg1_Leave(object sender, EventArgs e)
        {
            dg1.CurrentCell = null;
        }

        private void dg1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (editLogStatus)
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 3)
                {
                    dg1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    cellAfterValue = Utility.NulltoStr(dg1[e.ColumnIndex, e.RowIndex].Value);

                    // 変更のとき編集ログデータを書き込み
                    if (cellBeforeValue != cellAfterValue)
                    {
                        LogDataUpdate(e.RowIndex, e.ColumnIndex, global.flgOn, cellName);
                    }
                }
            }
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     保留処理 </summary>
        /// <param name="iX">
        ///     データインデックス</param>
        ///----------------------------------------------------------
        private void setHoldData(string iX)
        {
            //cn2.Open();

            try
            {
                // IWT_SHARE.db3をAttachする
                //string sql = "ATTACH [";
                //sql += Properties.Settings.Default.DB_File.Replace(@"\\\", @"\\") + "] AS db;";

                //{
                //using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                //    com.ExecuteNonQuery();
                //}

                string sql = "";

                // 保留テーブルに発注書データを移動する
                sql = "INSERT INTO 保留伝票 ";
                sql += "SELECT * FROM 納品伝票 ";
                sql += "WHERE ID = '" + ClsDeviveryNote.ID + "'";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                // 発注書データを削除します
                sql = "Delete from 納品伝票 ";
                sql += "WHERE ID= '" + ClsDeviveryNote.ID + "'";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                // 画像ファイル名を取得します
                string sImgNm = System.IO.Path.GetFileName(_img);

                // 画像ファイルが存在するとき
                if (System.IO.File.Exists(_img))
                {
                    // 移動先に同じ名前のファイルが存在する場合、既にあるファイルを削除する
                    string tifName = Properties.Settings.Default.HoldTifPath + sImgNm;

                    if (System.IO.File.Exists(tifName))
                    {
                        System.IO.File.Delete(tifName);
                    }

                    // 画像ファイルを保留フォルダに移動する
                    System.IO.File.Move(_img, tifName);
                }

                // 終了メッセージ
                MessageBox.Show("伝票が保留されました", "伝票保留", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (cn2.State == ConnectionState.Open)
                {
                    // いったん閉じて又開く
                    cn2.Close();
                    cn2.Open();
                }

                // 件数カウント
                if (tblDeliv.Count() > 0)
                {
                    // 配列キー再構築
                    keyArrayCreate();

                    // カレントレコードインデックスを再設定
                    if (cID.Length - 1 < cI)
                    {
                        cI = cID.Length - 1;
                    }

                    //// データ画面表示    2020/07/07 コメント化
                    //showOcrData(cI);

                    // 画像サムネイル表示
                    ShowThumbnail();

                    // リストビューセレクト
                    DataShowStatus = false;
                    ListViewSelect(cID[cI]);
                }
                else
                {
                    // ゼロならばプログラム終了
                    MessageBox.Show("全ての発注書データが保留されました。処理を終了します。", "発注書保留", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    //if (cn2.State == ConnectionState.Open)
                    //{
                    //    // いったん閉じて又開く
                    //    cn2.Close();
                    //    cn2.Open();
                    //}

                    //終了処理
                    this.Tag = END_NODATA;
                    this.Close();
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

        ///------------------------------------------------------------------------
        /// <summary>
        ///     編集ログデータアップロード </summary>
        ///------------------------------------------------------------------------

        private void EditDataUpload()
        {
            string errMsg = "";

            //cn2.Open();

            try
            {
                Cursor = Cursors.WaitCursor;

                // STSH_OCR.db3をAttachする
                string sql = "ATTACH [";
                sql += Properties.Settings.Default.DB_File.Replace(@"\\\", @"\\") + "] AS db;";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                sql = "INSERT INTO db.編集ログ (";
                sql += "年月日時刻, 項目名, 変更前値, 変更後値, 画像名, 編集アカウントID, コンピューター名, 更新年月日) ";
                sql += "SELECT 年月日時刻, 項目名, 変更前値, 変更後値, 画像名, 編集アカウントID, コンピューター名, 更新年月日 ";
                sql += "FROM main.編集ログ ";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }

                // ローカルの編集ログデータを削除します
                errMsg = "ローカル編集ログデータ削除";
                sql = "delete from 編集ログ ";

                using (SQLiteCommand com = new SQLiteCommand(sql, cn2))
                {
                    com.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, errMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                //if (cn2.State == ConnectionState.Open)
                //{
                //    cn2.Close();
                //}
            }
        }

        private void lblNoImage_Click(object sender, EventArgs e)
        {

        }

        private void btnData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("売上原価Proデータを作成します。よろしいですか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            // 非ログ書き込み状態とする：2015/09/25
            editLogStatus = false;

            // OCRDataクラス生成
            OCRData ocr = new OCRData();

            // エラーチェックを実行
            if (getErrData(cI, ocr))
            {
                // エラーがなかったとき
                Cursor = Cursors.WaitCursor;

                // 仕入データ作成
                ClsCsvOutput.ClsShiireCsv clsShiire = new ClsCsvOutput.ClsShiireCsv(cn2, tblDeliv, CsvOutPath, TifOutPath);
                clsShiire.CreateShiireCsv();

                // 売上データ作成
                ClsCsvOutput.ClsUriageCsv clsUriage = new ClsCsvOutput.ClsUriageCsv(cn2, tblDeliv, CsvOutPath, TifOutPath);
                clsUriage.CreateUriageCsv();

                Cursor = Cursors.Default;

                MessageBox.Show("売上原価Proデータが作成されました", "処理終了", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 閉じる
                Close();
            }
            else
            {
                if (cI != ocr._errHeaderIndex)
                {
                    // カレントインデックスをエラーありインデックスで更新
                    cI = ocr._errHeaderIndex;

                    // リストビューセレクト
                    ListViewSelect(cID[cI]);
                }
                else
                {
                    // 表示中画面でエラー有りのとき
                    // データ表示：エラーを消すため表示中データを再表示
                    showOcrData(cI);
                }

                // エラー表示
                ErrShow(ocr);
            }
        }

        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '\t')
                e.Handled = true;
        }

        private void dg1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                // 納入数：数字のみ入力可能とする
                if (dgv.CurrentCell.ColumnIndex == 3)
                {
                    //イベントハンドラが複数回追加されてしまうので最初に削除する
                    e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);

                    //イベントハンドラを追加する
                    e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
                }
                else
                {
                    //イベントハンドラを削除する
                    e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
                }
            }
        }

        private void txtShiireCode_TextChanged(object sender, EventArgs e)
        {
            //if (!ShiireCodeChangeStatus)
            //{
            //    return;
            //}

            //ClsCsvData.ClsCsvShiiresaki clsCsv = Utility.GetShiireFromDataTable(txtShiireCode.Text, global.dtShiire);

            //if (clsCsv != null)
            //{
            //    txtShiireName.Text = clsCsv.ShiireName;
            //}
        }

        private void txtShiireName_DoubleClick(object sender, EventArgs e)
        {
            string sName = Utility.GetWithoutCorp(txtShiireName.Text);

            frmShiireCode frm = new frmShiireCode(sName, DEN_KBN);
            frm.ShowDialog();

            if (frm.MyPropertyCode != string.Empty)
            {
                if (DEN_KBN == global.DEN_NOUHINKARI || DEN_KBN == global.DEN_GENPIN)
                {
                    // 仕入先コード編集ログ
                    cellName = LOG_SHIIRECODE;
                }
                else if (DEN_KBN == global.DEN_NOUHIN)
                {
                    // 受注先コード編集ログ
                    cellName = LOG_JYUCHUCODE;
                }

                // 変更前値を保持
                cellBeforeValue = Utility.NulltoStr(txtShiireCode.Text).Replace("'", "''");

                ShiireCodeChangeStatus = false;
                txtShiireCode.Text = frm.MyPropertyCode;
                txtShiireName.Text = frm.MyPropertyName;

                // 編集ログ
                if (editLogStatus)
                {
                    cellAfterValue = Utility.NulltoStr(txtShiireCode.Text).Replace("'", "''");

                    // 変更のとき編集ログデータを書き込み
                    if (cellBeforeValue != cellAfterValue)
                    {
                        LogDataUpdate(0, 0, global.flgOff, cellName);
                    }
                }
            }

            ShiireCodeChangeStatus = true;

            frm.Dispose();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (editLogStatus)
            {
                cellName = LOG_KAKEGENKIN;

                // 値を保持
                cellBeforeValue = comboBox1.SelectedItem.ToString().Replace("'", "''");
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            // 編集ログ
            if (editLogStatus)
            {
                cellAfterValue = comboBox1.SelectedItem.ToString().Replace("'", "''");

                // 変更のとき編集ログデータを書き込み
                if (cellBeforeValue != cellAfterValue)
                {
                    LogDataUpdate(0, 0, global.flgOff, cellName);
                }
            }
        }

        private void cmbBumon_SelectedValueChanged(object sender, EventArgs e)
        {
            // 編集ログ
            if (editLogStatus)
            {
                cellAfterValue = cmbBumon.Text.Replace("'", "''");

                // 変更のとき編集ログデータを書き込み
                if (cellBeforeValue != cellAfterValue)
                {
                    LogDataUpdate(0, 0, global.flgOff, cellName);
                }
            }
        }

        private void cmbBumon_Click(object sender, EventArgs e)
        {
            if (editLogStatus)
            {
                cellName = LOG_BUMON;

                // 値を保持
                cellBeforeValue = Utility.NulltoStr(cmbBumon.Text).ToString().Replace("'", "''");
            }
        }
    }
}
