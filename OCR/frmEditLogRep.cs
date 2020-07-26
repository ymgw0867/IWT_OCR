using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLibrary;
using IWT_OCR.Common;

namespace IWT_OCR.OCR
{
    public partial class frmEditLogRep : Form
    {
        public frmEditLogRep()
        {
            InitializeComponent();
        }

        private void frmEditLogRep_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            Utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            //Utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;

            // 共有DB接続
            cn = new SQLiteConnection("DataSource=" + db_file);
            context = new DataContext(cn);
            tblEditLog = context.GetTable<Common.ClsDataEditLog>(); // 編集ログテーブル

            // PC名コンボボックスアイテム追加
            foreach (var t in tblEditLog.Select(a => a.ComputerName).Distinct())
            {
                comboBox1.Items.Add(t);
            }

            GridViewSetting(dataGridView1);

            button1.Enabled = false;
        }

        string colPcName = "c2";
        string colEditDate = "c4";
        string colField = "c5";
        string colBefore = "c6";
        string colAfter = "c7";
        string ColSyohinNM = "c8";
        string colTokuisakiNM = "c10";
        string colPatternID = "c11";
        string colYear = "c12";
        string colID = "c15";

        // データベース：Sqlite3
        SQLiteConnection cn = null;
        DataContext context = null;

        string db_file = Properties.Settings.Default.DB_File;
        
        // 編集ログデータ
        Table<Common.ClsDataEditLog> tblEditLog = null;
        ClsDataEditLog ClsDataEditLog = null;

        public void GridViewSetting(DataGridView tempDGV)
        {
            //フォームサイズ定義

            // 列スタイルを変更する

            tempDGV.EnableHeadersVisualStyles = false;
            tempDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            tempDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tempDGV.EnableHeadersVisualStyles = false;

            // 列ヘッダー表示位置指定
            tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 列ヘッダーフォント指定
            tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("ＭＳ ゴシック", 9, FontStyle.Regular);

            // データフォント指定
            tempDGV.DefaultCellStyle.Font = new Font("ＭＳ ゴシック", (float)10.5, FontStyle.Regular);

            // 行の高さ
            tempDGV.ColumnHeadersHeight = 20;
            tempDGV.RowTemplate.Height = 20;
                       
            // 全体の高さ
            tempDGV.Height = 718;            

            // 奇数行の色
            tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

            // 各列幅指定
            tempDGV.Columns.Add(colEditDate, "編集日時");
            tempDGV.Columns.Add(colPcName, "ＰＣ名");
            tempDGV.Columns.Add(colField, "編集項目");
            tempDGV.Columns.Add(colBefore, "編集前");
            tempDGV.Columns.Add(colAfter, "編集後");

            tempDGV.Columns[colEditDate].Width = 200;
            tempDGV.Columns[colPcName].Width = 200;
            //tempDGV.Columns[colField].Width = 140;
            //tempDGV.Columns[colBefore].Width = 200;
            //tempDGV.Columns[colAfter].Width = 200;

            tempDGV.Columns[colField].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tempDGV.Columns[colBefore].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tempDGV.Columns[colAfter].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            tempDGV.Columns[colEditDate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tempDGV.Columns[colBefore].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tempDGV.Columns[colAfter].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 

            // 編集可否
            tempDGV.ReadOnly = true;

            // 行ヘッダを表示しない
            tempDGV.RowHeadersVisible = false;

            // 選択モード
            tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tempDGV.MultiSelect = false;

            // 追加行表示しない
            tempDGV.AllowUserToAddRows = false;

            // データグリッドビューから行削除を禁止する
            tempDGV.AllowUserToDeleteRows = false;

            // 手動による列移動の禁止
            tempDGV.AllowUserToOrderColumns = false;

            // 列サイズ変更禁止
            //tempDGV.AllowUserToResizeColumns = false;

            // 行サイズ変更禁止
            tempDGV.AllowUserToResizeRows = false;

            // 罫線
            tempDGV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            tempDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;
        }

        /// ----------------------------------------------------------------------
        /// <summary>
        ///     グリッドビュー表示 </summary>
        /// <param name="tempDGV">
        ///     DataGridViewオブジェクト名</param>
        /// <param name="sCode">
        ///     指定所属コード</param>
        /// ----------------------------------------------------------------------
        private void GridViewShowData(DataGridView g)
        {
            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            // データグリッド行クリア
            g.Rows.Clear();

            try
            {
                // 開始日付
                DateTime sdt = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                string _sdt = sdt.Year + "/" + sdt.Month.ToString("D2") + "/" + sdt.Day.ToString("D2") + " " +
                              sdt.Hour.ToString("D2") + ":" + sdt.Minute.ToString("D2") + ":" + sdt.Second.ToString("D2");

                // 終了日付
                DateTime edt = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, 23, 59, 59);
                string _edt = edt.Year + "/" + edt.Month.ToString("D2") + "/" + edt.Day.ToString("D2") + " " +
                              edt.Hour.ToString("D2") + ":" + edt.Minute.ToString("D2") + ":" + edt.Second.ToString("D2");

                var s = tblEditLog.Where(a => a.Date_Time.CompareTo(_sdt) >= 0 && a.Date_Time.CompareTo(_edt) <= 0).OrderByDescending(a => a.Date_Time);

                // PC指定
                if (comboBox1.SelectedIndex != -1)
                {
                    s = s.Where(a => a.ComputerName.Contains(comboBox1.Text)).OrderByDescending(a => a.Date_Time);
                }

                foreach (var t in s)
                {
                    g.Rows.Add();

                    g[colEditDate, g.Rows.Count - 1].Value = t.Date_Time;
                    g[colPcName, g.Rows.Count - 1].Value = t.ComputerName;
                    g[colField, g.Rows.Count - 1].Value = t.FieldName;
                    g[colBefore, g.Rows.Count - 1].Value = t.BeforeValue;
                    g[colAfter, g.Rows.Count - 1].Value = t.AfterValue;
                }

                g.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
            finally
            {
                // カーソルを戻す
                this.Cursor = Cursors.Default;
            }

            // 該当するデータがないとき
            if (g.RowCount == 0)
            {
                MessageBox.Show("該当するデータはありませんでした", "発注データ編集ログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void btnS_Click(object sender, EventArgs e)
        {
            GridViewShowData(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 閉じる
            Close();
        }

        private void frmEditLogRep_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 後片付け
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("表示中の編集ログをCSV形式で出力します。よろしいですか。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            MyLibrary.CsvOut.GridView(dataGridView1, "編集ログ");
        }
    }
}
