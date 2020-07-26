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
    public partial class frmShiireCode : Form
    {
        public frmShiireCode(string sName, string den_Kbn)
        {
            InitializeComponent();

            _sName = sName;
            _den_Kbn = den_Kbn;
        }

        string _sName = "";
        string _den_Kbn = "";

        // カラム定義
        private readonly string colShiireCode = "c0";
        private readonly string colShiireName = "c1";
        private readonly string colRyakusyo = "c2";

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dg.Rows.Clear();

            // 仕入データ
            if (_den_Kbn == global.DEN_NOUHINKARI || _den_Kbn == global.DEN_GENPIN)
            {
                // 仕入先コード逆引き
                ClsCsvData.ClsCsvShiiresaki[] csvShiiresakis = Utility.GetShiireCodeFromDataTable(Utility.NulltoStr(txtShiireName.Text).ToUpper().Trim(), global.dtShiire);

                if (csvShiiresakis == null)
                {
                    return;
                }

                if (csvShiiresakis.Length > 0)
                {
                    foreach (var item in csvShiiresakis)
                    {
                        dg.Rows.Add();
                        dg[colShiireCode, dg.Rows.Count - 1].Value = item.ShiireCode;
                        dg[colShiireName, dg.Rows.Count - 1].Value = item.ShiireName;
                        dg[colRyakusyo, dg.Rows.Count - 1].Value = item.ShiireRyakusyo;
                    }

                    dg.CurrentCell = null;
                }
            }

            // 売上データ
            if (_den_Kbn == global.DEN_NOUHIN)
            {
                // 取引先コード逆引き
                ClsCsvData.ClsCsvTorihikisaki[] clsCsvTorihikisakis = Utility.GetTorihikisakiCodeFromDataTable(Utility.NulltoStr(txtShiireName.Text).ToUpper().Trim(), global.dtTorihiki);

                if (clsCsvTorihikisakis == null)
                {
                    return;
                }

                if (clsCsvTorihikisakis.Length > 0)
                {
                    foreach (var item in clsCsvTorihikisakis)
                    {
                        dg.Rows.Add();
                        dg[colShiireCode, dg.Rows.Count - 1].Value = item.TorihikisakiCode;
                        dg[colShiireName, dg.Rows.Count - 1].Value = item.TorihikisakiName;
                        dg[colRyakusyo, dg.Rows.Count - 1].Value = item.TorihikisakiRyakusyo;
                    }

                    dg.CurrentCell = null;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmShiireCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            //// 後片付け
            //Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string msg = dg[colShiireName, dg.SelectedRows[0].Index].Value.ToString() + "が選択されました。よろしいですか？";

            //if (MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    MyPropertyCode = dg[colShiireCode, dg.SelectedRows[0].Index].Value.ToString();
            //    MyPropertyName = dg[colShiireName, dg.SelectedRows[0].Index].Value.ToString();

            //    // 閉じる
            //    Close();
            //}
            //else
            //{
            //    dg.CurrentCell = null;
            //}

            MyPropertyCode = dg[colShiireCode, dg.SelectedRows[0].Index].Value.ToString();
            MyPropertyName = dg[colShiireName, dg.SelectedRows[0].Index].Value.ToString();

            // 閉じる
            Close();
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     納品伝票データグリッドビュー定義 </summary>
        ///------------------------------------------------------------------------
        private void GridviewSet(DataGridView tempDGV)
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
                //tempDGV.Height = 122;

                // 奇数行の色
                tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add(colShiireCode, "仕入先コード");
                tempDGV.Columns.Add(colShiireName, "仕入先名");
                tempDGV.Columns.Add(colRyakusyo, "略称");

                tempDGV.Columns[colShiireCode].Width = 120;
                tempDGV.Columns[colShiireName].Width = 200;
                tempDGV.Columns[colRyakusyo].Width = 200;

                tempDGV.Columns[colShiireName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                //tempDGV.Columns[colHinCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //tempDGV.Columns[colSuu].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // 編集可否
                tempDGV.ReadOnly = true;

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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

                //// Enter次行移動先カラム
                //global.NEXT_COLUMN = colHinCode;

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

        private void frmShiireCode_Load(object sender, EventArgs e)
        {
            GridviewSet(dg);

            txtShiireName.Text = _sName;
            button1.Enabled = false;

            MyPropertyCode = string.Empty;
            MyPropertyName = string.Empty;
        }

        private void dg_SelectionChanged(object sender, EventArgs e)
        {
            if (dg.SelectedRows.Count > 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        public string MyPropertyCode { get; set; }
        public string MyPropertyName { get; set; }
    }
}
