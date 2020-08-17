using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Linq;
using IWT_OCR.Common;
using System.Windows.Forms;

namespace IWT_OCR.Common
{
    public class ClsCsvOutput
    {
        ///---------------------------------------------------
        /// <summary>
        ///     仕入データ作成クラス </summary>
        ///---------------------------------------------------
        public class ClsShiireCsv
        {
            SQLiteConnection cn2 = null;
            Table<Common.ClsDeviveryNote> tblDeliv = null;
            string CsvOutPath = string.Empty;
            string TifPath = string.Empty;

            ///-------------------------------------------------------
            /// <summary>
            ///     仕入データ作成コンストラクタ </summary>
            /// <param name="_cn2">
            ///     SQLiteConnection </param>
            /// <param name="_tblDeliv">
            ///     Table<Common.ClsDeviveryNote></param>
            /// <param name="_csvOutPath">
            ///     仕入データ作成先パス</param>
            /// <param name="_tifPath">
            ///     画像保存先パス</param>
            ///-------------------------------------------------------
            public ClsShiireCsv(SQLiteConnection _cn2, Table<Common.ClsDeviveryNote> _tblDeliv, string _csvOutPath, string _tifPath)
            {
                tblDeliv = _tblDeliv;
                CsvOutPath = _csvOutPath;
                cn2 = _cn2;
                TifPath = _tifPath;     // 画像保存先パス
            }

            ///-------------------------------------------------------
            /// <summary>
            ///     売上原価Pro仕入データ作成 </summary>
            ///-------------------------------------------------------
            public void CreateShiireCsv()
            {
                DateTime dt;
                string[] ProArray = null;
                int iX = 0;

                try
                {
                    var s = tblDeliv.Where(a => a.DenKbn == Utility.StrtoInt(global.DEN_NOUHINKARI) ||
                                           a.DenKbn == Utility.StrtoInt(global.DEN_GENPIN));

                    foreach (var t in s)
                    {
                        if (!DateTime.TryParse(t.NounyuDate, out dt))
                        {
                            continue;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            string BCode = "";
                            string BSuu = "";

                            if (i == 0)
                            {
                                BCode = t.BuhinCode_1;
                                BSuu = t.Suu_1;
                            }
                            else if (i == 1)
                            {
                                BCode = t.BuhinCode_2;
                                BSuu = t.Suu_2;
                            }
                            else if (i == 2)
                            {
                                BCode = t.BuhinCode_3;
                                BSuu = t.Suu_3;
                            }
                            else if (i == 3)
                            {
                                BCode = t.BuhinCode_4;
                                BSuu = t.Suu_4;
                            }
                            else if (i == 4)
                            {
                                BCode = t.BuhinCode_5;
                                BSuu = t.Suu_5;
                            }

                            if (BCode == string.Empty)
                            {
                                continue;
                            }

                            StringBuilder sb = new StringBuilder();

                            sb.Append(dt.Year + dt.Month.ToString("D2") + dt.Day.ToString("D2")).Append(",");   // 仕入日
                            sb.Append(t.KakeGenkin.ToString()).Append(",");             // 仕入区分
                            sb.Append(t.NonyuCode.ToString()).Append(",");              // 仕入先コード
                            sb.Append(string.Empty).Append(",");                        // 受注番号：値なし 2020/07/24
                            //sb.Append(t.BumonCode.ToString()).Append(",");            // 部門コード  // 2020/08/17 コメント化
                            sb.Append(string.Empty).Append(",");                        // 部門コード : 値なし 2020/08/17
                            sb.Append(BCode.Replace(",", " ")).Append(",");             // 部品コード

                            // 商品マスターより名称・規格を取得
                            ClsCsvData.ClsCsvSyohin cls = Utility.GetSyohinsFromDataTable(BCode, global.dtSyohin);
                            sb.Append(cls.SyohinName.Replace(",", " ")).Append(",");    // 品名
                            sb.Append(cls.Kikaku.Replace(",", " ")).Append(",");        // 規格

                            sb.Append(BSuu).Append(",");                                // 数量
                            sb.Append(string.Empty).Append(",");                        // 単位
                            sb.Append(string.Empty).Append(",");                        // 仕入単価
                            sb.Append(string.Empty).Append(",");                        // 備考１
                            sb.Append(string.Empty);                                    // 備考２

                            Array.Resize(ref ProArray, iX + 1);
                            ProArray[iX] = sb.ToString();

                            iX++;
                        }
                    }

                    // 売上原価Proデータ CSVファイル出力
                    if (ProArray != null)
                    {
                        txtFileWrite(CsvOutPath, ProArray, "仕入");
                    }

                    // データ削除
                    DeleteShiireData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {

                }
            }

            ///------------------------------------------------------
            /// <summary>
            ///     仕入データ、画像削除 </summary>
            ///------------------------------------------------------
            private void DeleteShiireData()
            {
                int DenNouhinkari = Utility.StrtoInt(global.DEN_NOUHINKARI);
                int DenGenpin = Utility.StrtoInt(global.DEN_GENPIN);
                string DirName = "";

                try
                {
                    var s = tblDeliv.Where(a => a.DenKbn == DenNouhinkari || a.DenKbn == DenGenpin);

                    // 画像保存
                    foreach (var t in s)
                    {
                        // 仕入先名取得
                        ClsCsvData.ClsCsvShiiresaki clsCsv = Utility.GetShiireFromDataTable(t.NonyuCode.ToString(), global.dtShiire);
                        if (clsCsv == null)
                        {
                            DirName = "不明";
                        }
                        else
                        {
                            DirName = clsCsv.ShiireCode + "_" + Utility.GetWithoutCorp(clsCsv.ShiireRyakusyo);
                        }

                        // 画像保存
                        MoveImage(t.ImageFileName, DirName, TifPath);
                    }

                    // 納品伝票データ削除
                    string sql = "DELETE FROM 納品伝票 WHERE 伝票区分 = " + DenNouhinkari + " OR 伝票区分 = " + DenGenpin;

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

                }
            }
        }

        ///------------------------------------------------------
        /// <summary>
        ///     売上データ作成クラス </summary>
        ///------------------------------------------------------
        public class ClsUriageCsv
        {
            SQLiteConnection cn2 = null;
            Table<Common.ClsDeviveryNote> tblDeliv = null;
            string CsvOutPath = string.Empty;
            string TifPath = string.Empty;

            ///---------------------------------------------------------------
            /// <summary>
            ///     売上原価Pro売上データ作成 </summary>
            /// <param name="_cn2">
            ///     SQLiteConnection　</param>
            /// <param name="_tblDeliv">
            ///     Table<></param>
            /// <param name="_csvOutPath">
            ///     CSVデータ出力パス</param>
            ///---------------------------------------------------------------
            public ClsUriageCsv(SQLiteConnection _cn2, Table<Common.ClsDeviveryNote> _tblDeliv, string _csvOutPath, string _tifPath)
            {
                tblDeliv = _tblDeliv;
                CsvOutPath = _csvOutPath;
                cn2 = _cn2;
                TifPath = _tifPath;     // 画像保存先パス
            }

            ///-------------------------------------------------------
            /// <summary>
            ///     売上原価売上データ作成 </summary>
            ///-------------------------------------------------------
            public void CreateUriageCsv()
            {
                DateTime dt;
                string[] ProArray = null;
                int iX = 0;

                try
                {
                    var s = tblDeliv.Where(a => a.DenKbn == Utility.StrtoInt(global.DEN_NOUHIN));

                    foreach (var t in s)
                    {
                        if (!DateTime.TryParse(t.NounyuDate, out dt))
                        {
                            continue;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            string BCode = "";
                            string BSuu = "";

                            if (i == 0)
                            {
                                BCode = t.BuhinCode_1;
                                BSuu = t.Suu_1;
                            }
                            else if (i == 1)
                            {
                                BCode = t.BuhinCode_2;
                                BSuu = t.Suu_2;
                            }
                            else if (i == 2)
                            {
                                BCode = t.BuhinCode_3;
                                BSuu = t.Suu_3;
                            }
                            else if (i == 3)
                            {
                                BCode = t.BuhinCode_4;
                                BSuu = t.Suu_4;
                            }
                            else if (i == 4)
                            {
                                BCode = t.BuhinCode_5;
                                BSuu = t.Suu_5;
                            }

                            if (BCode == string.Empty)
                            {
                                continue;
                            }

                            StringBuilder sb = new StringBuilder();

                            sb.Append(dt.Year + dt.Month.ToString("D2") + dt.Day.ToString("D2")).Append(",");   // 売上日
                            sb.Append(t.KakeGenkin.ToString()).Append(",");             // 売上区分
                            sb.Append(t.NonyuCode.ToString()).Append(",");              // 受注先コード
                            sb.Append(t.NonyuCode.ToString()).Append(",");              // 請求先コード
                            sb.Append(string.Empty).Append(",");                        // 受注番号：値なし　2020/07/24
                            //sb.Append(t.BumonCode.ToString()).Append(",");            // 部門コード  // 2020/08/17 コメント化
                            sb.Append(string.Empty).Append(",");                        // 部門コード : 値なし 2020/07/17
                            sb.Append(BCode.Replace(",", " ")).Append(",");             // 部品コード

                            // 商品マスターより名称・規格を取得
                            ClsCsvData.ClsCsvSyohin cls = Utility.GetSyohinsFromDataTable(BCode, global.dtSyohin);
                            sb.Append(cls.SyohinName.Replace(",", " ")).Append(",");    // 品名
                            sb.Append(cls.Kikaku.Replace(",", " ")).Append(",");        // 規格

                            sb.Append(BSuu).Append(",");              // 数量
                            sb.Append(string.Empty).Append(",");      // 単位
                            sb.Append(string.Empty).Append(",");      // 単価
                            sb.Append(string.Empty).Append(",");      // 備考１
                            sb.Append(string.Empty);                  // 備考２

                            Array.Resize(ref ProArray, iX + 1);
                            ProArray[iX] = sb.ToString();

                            iX++;
                        }
                    }

                    // 売上原価Proデータ CSVファイル出力
                    if (ProArray != null)
                    {
                        txtFileWrite(CsvOutPath, ProArray, "売上");
                    }

                    // データ削除
                    DeleteUriageData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {

                }
            }

            ///------------------------------------------------------
            /// <summary>
            ///     売上データ、画像削除 </summary>
            ///------------------------------------------------------
            private void DeleteUriageData()
            {
                int DenNouhin = Utility.StrtoInt(global.DEN_NOUHIN);
                string DirName = "";

                try
                {
                    var s = tblDeliv.Where(a => a.DenKbn == DenNouhin);

                    // 画像保存
                    foreach (var t in s)
                    {
                        // 取引先名取得
                        ClsCsvData.ClsCsvTorihikisaki clsCsv = Utility.GetTorihikisakiFromDataTable(t.NonyuCode.ToString(), global.dtTorihiki);
                        if (clsCsv == null)
                        {
                            DirName = "不明";
                        }
                        else
                        {
                            DirName = clsCsv.TorihikisakiCode + "_" + Utility.GetWithoutCorp(clsCsv.TorihikisakiRyakusyo);
                        }

                        // 画像保存
                        MoveImage(t.ImageFileName, DirName, TifPath);
                    }

                    // 売上データ削除
                    string sql = "DELETE FROM 納品伝票 WHERE 伝票区分 = " + DenNouhin;

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

                }
            }
        }


        ///----------------------------------------------------------------------------
        /// <summary>
        ///     CSVファイルを出力する</summary>
        ///　<param name="sPath">
        ///　    出力するフォルダ </param>
        /// <param name="arrayData">
        ///     書き込む配列データ</param>
        /// <param name="hd">
        ///     仕入または売上</param>
        ///----------------------------------------------------------------------------
        private static void txtFileWrite(string sPath, string[] arrayData, string hd)
        {
            // 付加文字列（タイムスタンプ）
            DateTime dtt = DateTime.Now;
            string newFileName = dtt.Year.ToString() + dtt.Month.ToString().PadLeft(2, '0') +
                                 dtt.Day.ToString().PadLeft(2, '0') + dtt.Hour.ToString().PadLeft(2, '0') +
                                 dtt.Minute.ToString().PadLeft(2, '0') + dtt.Second.ToString().PadLeft(2, '0');

            // ファイル名
            string outFileName = sPath + hd + newFileName + ".csv";

            // テキストファイル出力
            System.IO.File.WriteAllLines(outFileName, arrayData, System.Text.Encoding.GetEncoding(932));
        }


        ///-----------------------------------------------------------------
        /// <summary>
        ///     画像保存 </summary>
        /// <param name="Img">
        ///     画像ファイル名</param>
        /// <param name="Dir">
        ///     画像保存フォルダ名（コード_仕入先名）</param>
        /// <param name="tifPath">
        ///     画像保存ルートパス名</param>
        ///-----------------------------------------------------------------
        private static void MoveImage(string Img, string Dir, string tifPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(tifPath + Dir))
                {
                    // 保存先フォルダ未作成の場合は作成する
                    System.IO.Directory.CreateDirectory(tifPath + Dir);
                }

                // 画像名
                string imgFile = Properties.Settings.Default.dataPath + Img;

                // 保存先画像名パス
                string NewFile = tifPath + Dir + @"\" + Img;

                // 移動先に同名ファイルが登録済みのとき削除する
                if (System.IO.File.Exists(NewFile))
                {
                    System.IO.File.Delete(NewFile);
                }

                // 画像コピー＆削除
                System.IO.File.Copy(imgFile, NewFile);
                System.IO.File.Delete(imgFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }

    }
}
