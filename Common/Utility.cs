using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data;

//using DocumentFormat.OpenXml.Drawing.Charts;
//using DocumentFormat.OpenXml.Drawing;
//using Excel = Microsoft.Office.Interop.Excel;

namespace IWT_OCR.Common
{
    class Utility
    {
        ///----------------------------------------------------------------------
        /// <summary>
        ///     ウィンドウ最小サイズの設定 </summary>
        /// <param name="tempFrm">
        ///     対象とするウィンドウオブジェクト</param>
        /// <param name="wSize">
        ///     width</param>
        /// <param name="hSize">
        ///     Height</param>
        ///----------------------------------------------------------------------
        public static void WindowsMinSize(Form tempFrm, int wSize, int hSize)
        {
            tempFrm.MinimumSize = new System.Drawing.Size(wSize, hSize);
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     ウィンドウ最小サイズの設定 </summary>
        /// <param name="tempFrm">
        ///     対象とするウィンドウオブジェクト</param>
        /// <param name="wSize">
        ///     width</param>
        /// <param name="hSize">
        ///     height</param>
        ///----------------------------------------------------------------------
        public static void WindowsMaxSize(Form tempFrm, int wSize, int hSize)
        {
            tempFrm.MaximumSize = new System.Drawing.Size(wSize, hSize);
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     文字列の値が数字かチェックする </summary>
        /// <param name="tempStr">
        ///     検証する文字列</param>
        /// <returns>
        ///     数字:true,数字でない:false</returns>
        ///------------------------------------------------------------------------
        public static bool NumericCheck(string tempStr)
        {
            double d;

            if (tempStr == null) return false;

            if (double.TryParse(tempStr, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out d) == false)
                return false;

            return true;
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     emptyを"0"に置き換える </summary>
        /// <param name="tempStr">
        ///     stringオブジェクト</param>
        /// <returns>
        ///     nullのときstring.Empty、not nullのときそのまま値を返す</returns>
        ///------------------------------------------------------------------------
        public static string EmptytoZero(string tempStr)
        {
            if (tempStr == string.Empty)
            {
                return "0";
            }
            else
            {
                return tempStr;
            }
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     Nullをstring.Empty("")に置き換える </summary>
        /// <param name="tempStr">
        ///     stringオブジェクト</param>
        /// <returns>
        ///     nullのときstring.Empty、not nullのとき文字型値を返す</returns>
        ///------------------------------------------------------------------------
        public static string NulltoStr(string tempStr)
        {
            if (tempStr == null)
            {
                return string.Empty;
            }
            else
            {
                return tempStr;
            }
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     Nullをstring.Empty("")に置き換える </summary>
        /// <param name="tempStr">
        ///     stringオブジェクト</param>
        /// <returns>
        ///     nullのときstring.Empty、not nullのときそのまま値を返す</returns>
        ///------------------------------------------------------------------------
        public static string NulltoStr(object tempStr)
        {
            if (tempStr == null)
            {
                return string.Empty;
            }
            else
            {
                if (tempStr == DBNull.Value)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)tempStr.ToString();
                }
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     文字型をIntへ変換して返す（数値でないときは０を返す） </summary>
        /// <param name="tempStr">
        ///     文字型の値</param>
        /// <returns>
        ///     Int型の値</returns>
        ///----------------------------------------------------------------------
        public static int StrtoInt(string tempStr)
        {
            if (NumericCheck(tempStr)) return int.Parse(tempStr);
            else return 0;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     文字型をDoubleへ変換して返す（数値でないときは０を返す）</summary>
        /// <param name="tempStr">
        ///     文字型の値</param>
        /// <returns>
        ///     double型の値</returns>
        ///----------------------------------------------------------------------
        public static double StrtoDouble(string tempStr)
        {
            if (NumericCheck(tempStr)) return double.Parse(tempStr);
            else return 0;
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     経過時間を返す </summary>
        /// <param name="s">
        ///     開始時間</param>
        /// <param name="e">
        ///     終了時間</param>
        /// <returns>
        ///     経過時間</returns>
        ///-----------------------------------------------------------------------
        public static TimeSpan GetTimeSpan(DateTime s, DateTime e)
        {
            TimeSpan ts;
            if (s > e)
            {
                TimeSpan j = new TimeSpan(24, 0, 0);
                ts = e + j - s;
            }
            else
            {
                ts = e - s;
            }

            return ts;
        }

        /// ------------------------------------------------------------------------
        /// <summary>
        ///     指定した精度の数値に切り捨てします。</summary>
        /// <param name="dValue">
        ///     丸め対象の倍精度浮動小数点数。</param>
        /// <param name="iDigits">
        ///     戻り値の有効桁数の精度。</param>
        /// <returns>
        ///     iDigits に等しい精度の数値に切り捨てられた数値。</returns>
        /// ------------------------------------------------------------------------
        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor(dValue * dCoef) / dCoef :
                                System.Math.Ceiling(dValue * dCoef) / dCoef;
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     ファイル選択ダイアログボックスの表示 </summary>
        /// <param name="sTitle">
        ///     タイトル文字列</param>
        /// <param name="sFilter">
        ///     ファイルのフィルター</param>
        /// <returns>
        ///     選択したファイル名</returns>
        ///------------------------------------------------------------------
        public static string userFileSelect(string sTitle, string sFilter)
        {
            DialogResult ret;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //ダイアログボックスの初期設定
            openFileDialog1.Title = sTitle;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = sFilter;
            //openFileDialog1.Filter = "CSVファイル(*.CSV)|*.csv|全てのファイル(*.*)|*.*";

            //ダイアログボックスの表示
            ret = openFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return string.Empty;
            }

            if (MessageBox.Show(openFileDialog1.FileName + Environment.NewLine + " が選択されました。よろしいですか?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return string.Empty;
            }

            return openFileDialog1.FileName;
        }

        public class frmMode
        {
            public int ID { get; set; }

            public int Mode { get; set; }

            public int rowIndex { get; set; }
        }

        public class xlsShain
        {
            public int sCode { get; set; }
            public string sName { get; set; }
            public int bCode { get; set; }
            public string bName { get; set; }
        }


        ///---------------------------------------------------------------------
        /// <summary>
        ///     任意のディレクトリのファイルを削除する </summary>
        /// <param name="sPath">
        ///     指定するディレクトリ</param>
        /// <param name="sFileType">
        ///     ファイル名及び形式</param>
        /// --------------------------------------------------------------------
        public static void FileDelete(string sPath, string sFileType)
        {
            //sFileTypeワイルドカード"*"は、すべてのファイルを意味する
            foreach (string files in System.IO.Directory.GetFiles(sPath, sFileType))
            {
                // ファイルを削除する
                System.IO.File.Delete(files);
            }
        }



        ///---------------------------------------------------------------------
        /// <summary>
        ///     文字列を指定文字数をＭＡＸとして返します</summary>
        /// <param name="s">
        ///     文字列</param>
        /// <param name="n">
        ///     文字数</param>
        /// <returns>
        ///     文字数範囲内の文字列</returns>
        /// --------------------------------------------------------------------
        public static string GetStringSubMax(string s, int n)
        {
            string val = string.Empty;

            // 文字間のスペースを除去 2015/03/10
            s = s.Replace(" ", "");

            if (s.Length > n) val = s.Substring(0, n);
            else val = s;

            return val;
        }


        ///-------------------------------------------------------------------------
        /// <summary>
        ///     自らのロックファイルが存在したら削除する </summary>
        /// <param name="fPath">
        ///     パス</param>
        /// <param name="PcK">
        ///     自分のロックファイル文字列</param>
        ///-------------------------------------------------------------------------
        public static void deleteLockFile(string fPath, string PcK)
        {
            string FileName = fPath + global.LOCK_FILEHEAD + PcK + ".loc";

            if (System.IO.File.Exists(FileName))
            {
                System.IO.File.Delete(FileName);
            }
        }

        ///-------------------------------------------------------------------------
        /// <summary>
        ///     データフォルダにロックファイルが存在するか調べる </summary>
        /// <param name="fPath">
        ///     データフォルダパス</param>
        /// <returns>
        ///     true:ロックファイルあり、false:ロックファイルなし</returns>
        ///-------------------------------------------------------------------------
        public static Boolean existsLockFile(string fPath)
        {
            int s = System.IO.Directory.GetFiles(fPath, global.LOCK_FILEHEAD + "*.*", System.IO.SearchOption.TopDirectoryOnly).Count();

            if (s == 0)
            {
                return false; //LOCKファイルが存在しない
            }
            else
            {
                return true;   //存在する
            }
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     ロックファイルを登録する </summary>
        /// <param name="fPath">
        ///     書き込み先フォルダパス</param>
        /// <param name="LocName">
        ///     ファイル名</param>
        ///----------------------------------------------------------------
        public static void makeLockFile(string fPath, string LocName)
        {
            string FileName = fPath + global.LOCK_FILEHEAD + LocName + ".loc";

            //存在する場合は、処理なし
            if (System.IO.File.Exists(FileName))
            {
                return;
            }

            // ロックファイルを登録する
            try
            {
                System.IO.StreamWriter outFile = new System.IO.StreamWriter(FileName, false, System.Text.Encoding.GetEncoding(932));
                outFile.Close();
            }
            catch
            {
            }

            return;
        }

        ///---------------------------------------------------------------------
        /// <summary>
        ///     楽商商品コードを8桁頭ゼロ埋め文字列に変換する </summary>
        /// <param name="s">
        ///     商品コード</param>
        /// <returns>
        ///     変換後文字列</returns>
        ///---------------------------------------------------------------------
        public static string ptnShohinStr(int s)
        {
            string val = string.Empty;

            if (s == global.flgOff)
            {
                val = string.Empty;
            }
            else
            {
                val = s.ToString().PadLeft(8, '0');
            }

            return val;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     得意先情報をDataTableからClsCsvData.ClsCsvTokuisakiクラスに取得 : 
        ///     2020/04/09</summary>
        /// <param name="tID">
        ///     得意先コード</param>
        /// <returns>
        ///     ClsCsvData.ClsCsvTokuisakiクラス</returns>
        ///-----------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvShiiresaki GetShiireFromDataTable(string tID, System.Data.DataTable data)
        {
            // 返り値クラス初期化
            ClsCsvData.ClsCsvShiiresaki cls = new ClsCsvData.ClsCsvShiiresaki
            {
                ShiireCode = 0,
                ShiireName = "",
                ShiireRyakusyo = ""
            };

            DataRow[] rows = data.AsEnumerable().Where(a => a["仕入先コード"].ToString() == tID).ToArray();

            foreach (var t in rows)
            {
                cls.ShiireCode = Utility.StrtoInt(t["仕入先コード"].ToString());      // 仕入先コード
                cls.ShiireName = t["仕入先名1"].ToString();                        // 仕入先名
                cls.ShiireRyakusyo = t["略称"].ToString();                // 略称

                break;
            }

            return cls;
        }


        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     取引先情報をDataTableからClsCsvData.ClsCsvTorihikisakiクラスに取得 </summary>
        /// <param name="tID">
        ///     取引先コード</param>
        /// <returns>
        ///     ClsCsvData.ClsCsvTorihikisakiクラス</returns>
        ///-----------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvTorihikisaki GetTorihikisakiFromDataTable(string tID, System.Data.DataTable data)
        {
            // 返り値クラス初期化
            ClsCsvData.ClsCsvTorihikisaki cls = new ClsCsvData.ClsCsvTorihikisaki
            {
                TorihikisakiCode = 0,
                TorihikisakiName = "",
                TorihikisakiRyakusyo = ""
            };

            DataRow[] rows = data.AsEnumerable().Where(a => a["取引先コード"].ToString() == tID).ToArray();

            foreach (var t in rows)
            {
                cls.TorihikisakiCode = Utility.StrtoInt(t["取引先コード"].ToString());      // 取引先コード
                cls.TorihikisakiName = t["取引先名1"].ToString();                          // 取引先名
                cls.TorihikisakiRyakusyo = t["略称"].ToString();                          // 略称

                break;
            }

            return cls;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     取引先情報をDataTableからClsCsvData.ClsCsvTorihikisakiクラスに取得</summary>
        /// <param name="tID">
        ///     取引先コード</param>
        /// <returns>
        ///     ClsCsvData.ClsCsvTorihikisakiクラス</returns>
        ///-----------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvTorihikisaki[] GetTorihikisakiCodeFromDataTable(string tName, System.Data.DataTable data)
        {
            ClsCsvData.ClsCsvTorihikisaki[] cls = null;

            string sName = tName.Replace("（株）", "");
            sName = sName.Replace("株式会社", "");
            sName = sName.Replace("(株)", "");
            sName = sName.Replace("㈱", "");
            sName = sName.Replace("有限会社", "");
            sName = sName.Replace("（有）", "");
            sName = sName.Replace("(有)", "");
            sName = sName.Replace("㈲", "");

            DataRow[] rows = data.AsEnumerable().Where(a => a["取引先名1"].ToString().Contains(sName)).ToArray();

            if (rows.Length > 0)
            {
                cls = new ClsCsvData.ClsCsvTorihikisaki[rows.Length];

                int iX = 0;

                foreach (var t in rows)
                {
                    cls[iX] = new ClsCsvData.ClsCsvTorihikisaki();
                    cls[iX].TorihikisakiCode = Utility.StrtoInt(t["取引先コード"].ToString());      // 取引先コード
                    cls[iX].TorihikisakiName = t["取引先名1"].ToString();                        // 取引先名
                    cls[iX].TorihikisakiRyakusyo = t["略称"].ToString();                // 略称

                    iX++;
                }
            }

            return cls;
        }


        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     仕入先情報をDataTableからClsCsvData.ClsCsvTokuisakiクラスに取得</summary>
        /// <param name="tID">
        ///     仕入先コード</param>
        /// <returns>
        ///     ClsCsvData.ClsCsvShiiresakiクラス</returns>
        ///-----------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvShiiresaki [] GetShiireCodeFromDataTable(string tName, System.Data.DataTable data)
        {
            ClsCsvData.ClsCsvShiiresaki[] cls = null;

            string sName = tName.Replace("（株）", "");
            sName = sName.Replace("株式会社", "");
            sName = sName.Replace("(株)", "");
            sName = sName.Replace("㈱", "");
            sName = sName.Replace("有限会社", "");
            sName = sName.Replace("（有）", "");
            sName = sName.Replace("(有)", "");
            sName = sName.Replace("㈲", "");

            DataRow[] rows = data.AsEnumerable().Where(a => a["仕入先名1"].ToString().Contains(sName)).ToArray();

            if (rows.Length > 0)
            {
                cls = new ClsCsvData.ClsCsvShiiresaki[rows.Length];

                int iX = 0;

                foreach (var t in rows)
                {
                    cls[iX] = new ClsCsvData.ClsCsvShiiresaki();
                    cls[iX].ShiireCode = Utility.StrtoInt(t["仕入先コード"].ToString());      // 仕入先コード
                    cls[iX].ShiireName = t["仕入先名1"].ToString();                        // 仕入先名
                    cls[iX].ShiireRyakusyo = t["略称"].ToString();                // 略称

                    iX++;
                }
            }

            return cls;
        }


        ///------------------------------------------------------------------------
        /// <summary>
        ///     商品マスターデータテーブルから情報を取得する : 2020/04/09 </summary>
        /// <param name="syohins">
        ///     ClsCsvData.ClsCsvSyohin_Newクラス配列</param>
        /// <param name="sSyohinCD">
        ///     商品コード  </param>
        /// <returns>
        ///     商品マスタークラス</returns>
        ///------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvSyohin GetSyohinsFromDataTable(string sSyohinCD, System.Data.DataTable data)
        {
            ClsCsvData.ClsCsvSyohin cls = new ClsCsvData.ClsCsvSyohin
            {
                SyohinCode = "",
                SyohinName = "",
                Kikaku = ""
            };

            DataRow[] row = data.AsEnumerable().Where(a => a["商品コード"].ToString() == sSyohinCD).ToArray();

            foreach (var t in row)
            {
                cls.SyohinCode = t["商品コード"].ToString();
                cls.SyohinName = t["商品名"].ToString();
                //cls.Kikaku = t["規格"].ToString();
                break;
            }

            return cls;
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     部門マスターデータテーブルから情報を取得する </summary>
        /// <param name="sBmnCD">
        ///     部門コード  </param>
        /// <param name="data">
        ///     部門データテーブル</param>
        /// <returns>
        ///     部門マスタークラス</returns>
        ///------------------------------------------------------------------------
        public static ClsCsvData.ClsCsvBumon GetBumonFromDataTable(int sBmnCD, System.Data.DataTable data)
        {
            ClsCsvData.ClsCsvBumon cls= new ClsCsvData.ClsCsvBumon
            {
                BumonCode = global.flgOff,
                BumonName = "",
                BumonRyakusyo = ""
            };

            DataRow[] row = data.AsEnumerable().Where(a => a["部門コード"].ToString() == sBmnCD.ToString()).ToArray();

            foreach (var t in row)
            {
                cls.BumonCode = Utility.StrtoInt(t["部門コード"].ToString());
                cls.BumonName = t["部門名"].ToString();
                break;
            }

            return cls;
        }



        ///-----------------------------------------------------------------------------------
        /// <summary>
        ///     得意先別画像保存フォルダパス取得 </summary>
        /// <param name="ImgPath">
        ///     画像保存先フォルダパス</param>
        /// <param name="TokuisakiCD">
        ///     得意先コード</param>
        /// <returns>
        ///    フォルダ名</returns>
        ///-----------------------------------------------------------------------------------
        public static string GetImageFilePath(string ImgPath, string TokuisakiCD)
        {
            string DirNM = string.Empty;

            // フォルダ名に得意先コードが含まれるフォルダ
            foreach (var dir in System.IO.Directory.GetDirectories(ImgPath, TokuisakiCD + "*"))
            {
                DirNM = dir;
                break;
            }

            return DirNM;
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        ///     商品明細クラス作成 </summary>
        /// <param name="r">
        ///     ClsOrder </param>
        /// <param name="tenDates">
        ///     店着日配列</param>
        /// <param name="tblHistories">
        ///     商品発注履歴テーブル</param>
        /// <returns>
        ///     商品明細クラス配列</returns>
        ///---------------------------------------------------------------------------
        //public static ClsGoods [] SetGoodsTabla(ClsOrder r, ClsTenDate [] tenDates, Table<ClsOrderHistory> tblHistories)
        //{
        //    ClsGoods[] goods = new ClsGoods[15];
        //    for (int i = 0; i < global.MAX_GYO; i++)
        //    {
        //        goods[i] = new ClsGoods();
        //        goods[i].Suu = new string[7];
        //        goods[i].Target = new bool[7];

        //        switch (i)
        //        {
        //            case 0:
        //                goods[i].Code = r.G_Code1;
        //                goods[i].Suu[0] = r.Goods1_1;
        //                goods[i].Suu[1] = r.Goods1_2;
        //                goods[i].Suu[2] = r.Goods1_3;
        //                goods[i].Suu[3] = r.Goods1_4;
        //                goods[i].Suu[4] = r.Goods1_5;
        //                goods[i].Suu[5] = r.Goods1_6;
        //                goods[i].Suu[6] = r.Goods1_7;
        //                goods[i].Nouka = r.G_Nouka1;
        //                goods[i].Baika = r.G_Baika1;
        //                goods[i].Syubai = r.G_Syubai1;
        //                break;

        //            case 1:
        //                goods[i].Code = r.G_Code2;
        //                goods[i].Suu[0] = r.Goods2_1;
        //                goods[i].Suu[1] = r.Goods2_2;
        //                goods[i].Suu[2] = r.Goods2_3;
        //                goods[i].Suu[3] = r.Goods2_4;
        //                goods[i].Suu[4] = r.Goods2_5;
        //                goods[i].Suu[5] = r.Goods2_6;
        //                goods[i].Suu[6] = r.Goods2_7;
        //                goods[i].Nouka = r.G_Nouka2;
        //                goods[i].Baika = r.G_Baika2;
        //                goods[i].Syubai = r.G_Syubai2;
        //                break;

        //            case 2:
        //                goods[i].Code = r.G_Code3;
        //                goods[i].Suu[0] = r.Goods3_1;
        //                goods[i].Suu[1] = r.Goods3_2;
        //                goods[i].Suu[2] = r.Goods3_3;
        //                goods[i].Suu[3] = r.Goods3_4;
        //                goods[i].Suu[4] = r.Goods3_5;
        //                goods[i].Suu[5] = r.Goods3_6;
        //                goods[i].Suu[6] = r.Goods3_7;
        //                goods[i].Nouka = r.G_Nouka3;
        //                goods[i].Baika = r.G_Baika3;
        //                goods[i].Syubai = r.G_Syubai3;
        //                break;

        //            case 3:
        //                goods[i].Code = r.G_Code4;
        //                goods[i].Suu[0] = r.Goods4_1;
        //                goods[i].Suu[1] = r.Goods4_2;
        //                goods[i].Suu[2] = r.Goods4_3;
        //                goods[i].Suu[3] = r.Goods4_4;
        //                goods[i].Suu[4] = r.Goods4_5;
        //                goods[i].Suu[5] = r.Goods4_6;
        //                goods[i].Suu[6] = r.Goods4_7;
        //                goods[i].Nouka = r.G_Nouka4;
        //                goods[i].Baika = r.G_Baika4;
        //                goods[i].Syubai = r.G_Syubai4;
        //                break;

        //            case 4:
        //                goods[i].Code = r.G_Code5;
        //                goods[i].Suu[0] = r.Goods5_1;
        //                goods[i].Suu[1] = r.Goods5_2;
        //                goods[i].Suu[2] = r.Goods5_3;
        //                goods[i].Suu[3] = r.Goods5_4;
        //                goods[i].Suu[4] = r.Goods5_5;
        //                goods[i].Suu[5] = r.Goods5_6;
        //                goods[i].Suu[6] = r.Goods5_7;
        //                goods[i].Nouka = r.G_Nouka5;
        //                goods[i].Baika = r.G_Baika5;
        //                goods[i].Syubai = r.G_Syubai5;
        //                break;

        //            case 5:
        //                goods[i].Code = r.G_Code6;
        //                goods[i].Suu[0] = r.Goods6_1;
        //                goods[i].Suu[1] = r.Goods6_2;
        //                goods[i].Suu[2] = r.Goods6_3;
        //                goods[i].Suu[3] = r.Goods6_4;
        //                goods[i].Suu[4] = r.Goods6_5;
        //                goods[i].Suu[5] = r.Goods6_6;
        //                goods[i].Suu[6] = r.Goods6_7;
        //                goods[i].Nouka = r.G_Nouka6;
        //                goods[i].Baika = r.G_Baika6;
        //                goods[i].Syubai = r.G_Syubai6;
        //                break;

        //            case 6:
        //                goods[i].Code = r.G_Code7;
        //                goods[i].Suu[0] = r.Goods7_1;
        //                goods[i].Suu[1] = r.Goods7_2;
        //                goods[i].Suu[2] = r.Goods7_3;
        //                goods[i].Suu[3] = r.Goods7_4;
        //                goods[i].Suu[4] = r.Goods7_5;
        //                goods[i].Suu[5] = r.Goods7_6;
        //                goods[i].Suu[6] = r.Goods7_7;
        //                goods[i].Nouka = r.G_Nouka7;
        //                goods[i].Baika = r.G_Baika7;
        //                goods[i].Syubai = r.G_Syubai7;
        //                break;

        //            case 7:
        //                goods[i].Code = r.G_Code8;
        //                goods[i].Suu[0] = r.Goods8_1;
        //                goods[i].Suu[1] = r.Goods8_2;
        //                goods[i].Suu[2] = r.Goods8_3;
        //                goods[i].Suu[3] = r.Goods8_4;
        //                goods[i].Suu[4] = r.Goods8_5;
        //                goods[i].Suu[5] = r.Goods8_6;
        //                goods[i].Suu[6] = r.Goods8_7;
        //                goods[i].Nouka = r.G_Nouka8;
        //                goods[i].Baika = r.G_Baika8;
        //                goods[i].Syubai = r.G_Syubai8;
        //                break;

        //            case 8:
        //                goods[i].Code = r.G_Code9;
        //                goods[i].Suu[0] = r.Goods9_1;
        //                goods[i].Suu[1] = r.Goods9_2;
        //                goods[i].Suu[2] = r.Goods9_3;
        //                goods[i].Suu[3] = r.Goods9_4;
        //                goods[i].Suu[4] = r.Goods9_5;
        //                goods[i].Suu[5] = r.Goods9_6;
        //                goods[i].Suu[6] = r.Goods9_7;
        //                goods[i].Nouka = r.G_Nouka9;
        //                goods[i].Baika = r.G_Baika9;
        //                goods[i].Syubai = r.G_Syubai9;
        //                break;

        //            case 9:
        //                goods[i].Code = r.G_Code10;
        //                goods[i].Suu[0] = r.Goods10_1;
        //                goods[i].Suu[1] = r.Goods10_2;
        //                goods[i].Suu[2] = r.Goods10_3;
        //                goods[i].Suu[3] = r.Goods10_4;
        //                goods[i].Suu[4] = r.Goods10_5;
        //                goods[i].Suu[5] = r.Goods10_6;
        //                goods[i].Suu[6] = r.Goods10_7;
        //                goods[i].Nouka = r.G_Nouka10;
        //                goods[i].Baika = r.G_Baika10;
        //                goods[i].Syubai = r.G_Syubai10;
        //                break;

        //            case 10:
        //                goods[i].Code = r.G_Code11;
        //                goods[i].Suu[0] = r.Goods11_1;
        //                goods[i].Suu[1] = r.Goods11_2;
        //                goods[i].Suu[2] = r.Goods11_3;
        //                goods[i].Suu[3] = r.Goods11_4;
        //                goods[i].Suu[4] = r.Goods11_5;
        //                goods[i].Suu[5] = r.Goods11_6;
        //                goods[i].Suu[6] = r.Goods11_7;
        //                goods[i].Nouka = r.G_Nouka11;
        //                goods[i].Baika = r.G_Baika11;
        //                goods[i].Syubai = r.G_Syubai11;
        //                break;

        //            case 11:
        //                goods[i].Code = r.G_Code12;
        //                goods[i].Suu[0] = r.Goods12_1;
        //                goods[i].Suu[1] = r.Goods12_2;
        //                goods[i].Suu[2] = r.Goods12_3;
        //                goods[i].Suu[3] = r.Goods12_4;
        //                goods[i].Suu[4] = r.Goods12_5;
        //                goods[i].Suu[5] = r.Goods12_6;
        //                goods[i].Suu[6] = r.Goods12_7;
        //                goods[i].Nouka = r.G_Nouka12;
        //                goods[i].Baika = r.G_Baika12;
        //                goods[i].Syubai = r.G_Syubai12;
        //                break;

        //            case 12:
        //                goods[i].Code = r.G_Code13;
        //                goods[i].Suu[0] = r.Goods13_1;
        //                goods[i].Suu[1] = r.Goods13_2;
        //                goods[i].Suu[2] = r.Goods13_3;
        //                goods[i].Suu[3] = r.Goods13_4;
        //                goods[i].Suu[4] = r.Goods13_5;
        //                goods[i].Suu[5] = r.Goods13_6;
        //                goods[i].Suu[6] = r.Goods13_7;
        //                goods[i].Nouka = r.G_Nouka13;
        //                goods[i].Baika = r.G_Baika13;
        //                goods[i].Syubai = r.G_Syubai13;
        //                break;

        //            case 13:
        //                goods[i].Code = r.G_Code14;
        //                goods[i].Suu[0] = r.Goods14_1;
        //                goods[i].Suu[1] = r.Goods14_2;
        //                goods[i].Suu[2] = r.Goods14_3;
        //                goods[i].Suu[3] = r.Goods14_4;
        //                goods[i].Suu[4] = r.Goods14_5;
        //                goods[i].Suu[5] = r.Goods14_6;
        //                goods[i].Suu[6] = r.Goods14_7;
        //                goods[i].Nouka = r.G_Nouka14;
        //                goods[i].Baika = r.G_Baika14;
        //                goods[i].Syubai = r.G_Syubai14;
        //                break;

        //            case 14:
        //                goods[i].Code = r.G_Code15;
        //                goods[i].Suu[0] = r.Goods15_1;
        //                goods[i].Suu[1] = r.Goods15_2;
        //                goods[i].Suu[2] = r.Goods15_3;
        //                goods[i].Suu[3] = r.Goods15_4;
        //                goods[i].Suu[4] = r.Goods15_5;
        //                goods[i].Suu[5] = r.Goods15_6;
        //                goods[i].Suu[6] = r.Goods15_7;
        //                goods[i].Nouka = r.G_Nouka15;
        //                goods[i].Baika = r.G_Baika15;
        //                goods[i].Syubai = r.G_Syubai15;
        //                break;

        //            default:
        //                break;
        //        }
        //    }

        //    // 発注対象ステータスを設定：2020/04/13
        //    for (int i = 0; i < goods.Length; i++)
        //    {
        //        for (int iX = 0; iX < tenDates.Length; iX++)
        //        {
        //            // 発注対象ステータス初期値：2020/04/20
        //            goods[i].Target[iX] = false;

        //            // 終売取消のときネグる
        //            if (goods[i].Syubai == global.SYUBAI_TORIKESHI)
        //            {
        //                continue;
        //            }

        //            // 空白の店着日はネグる
        //            if (tenDates[iX].Day == string.Empty)
        //            {
        //                continue;
        //            }

        //            // 昨日以前の店着日はネグる
        //            DateTime dt;
        //            if (DateTime.TryParse(tenDates[iX].Year + "/" + tenDates[iX].Month + "/" + tenDates[iX].Day, out dt))
        //            {
        //                if (dt < DateTime.Today)
        //                {
        //                    continue;
        //                }
        //            }

        //            int ss = Utility.StrtoInt(goods[i].Suu[iX]);

        //            // 発注あり：2020/04/13
        //            if (ss > 0)
        //            {
        //                // 注文済み（得意先、発注日、商品コード、数量同一）商品はネグる：2020/04/13
        //                string dd = tenDates[iX].Year + tenDates[iX].Month.PadLeft(2, '0') + tenDates[iX].Day.PadLeft(2, '0');
        //                string syCD = goods[i].Code;
        //                if (tblHistories.Any(a => a.TokuisakiCD == r.TokuisakiCode && a.SyohinCD == goods[i].Code && a.OrderDate == dd && a.Suu == ss))
        //                {
        //                    continue;
        //                }
        //            }

        //            // 発注対象ステータス：2020/04/20
        //            goods[i].Target[iX] = true;
        //        }
        //    }

        //    return goods;
        //}

        ///------------------------------------------------------------------
        /// <summary>
        ///     注文済み商品ありメッセージのコントロール </summary>
        ///------------------------------------------------------------------
        public static string ShowPastOrderMessage(DataGridView dg1)
        {
            bool msgStatus = false;

            // 注文済み商品ありメッセージのコントロール
            string label1 = "";
            for (int i = 6; i <= 12; i++)
            {
                for (int r = 0; r < dg1.RowCount; r++)
                {
                    if (dg1.Rows[r].Cells[i].Style.BackColor == Color.MistyRose)
                    {
                        label1 = "注文済商品 ①発注数同じ：グレー、ロック済・発注書データ対象外、②発注数違い：赤、編集可・発注書データ作成";
                        msgStatus = true;
                        break;
                    }
                }

                if (msgStatus)
                {
                    break;
                }
            }

            return label1;
        }

        ///-------------------------------------------------------------------------------
        /// <summary>
        ///     指定したファイルをロックせずに、System.Drawing.Imageを作成する。</summary>
        /// <param name="filename">
        ///     作成元のファイルのパス</param>
        /// <returns>
        ///     作成したSystem.Drawing.Image。</returns>
        ///-------------------------------------------------------------------------------
        public static System.Drawing.Image CreateImage(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                filename,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);

            fs.Close();
            return img;

        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     終売判断：2020/04/15 </summary>
        /// <param name="YMD">
        ///     日付</param>
        /// <returns>
        ///     true:終売, false:非終売</returns>
        ///--------------------------------------------------------------
        public static bool IsShubai(string YMD)
        {
            // 終売判断：2020/04/15
            bool SHUBAI = false;
            string L_YMD = "";

            if (YMD.Length > 7)
            {
                L_YMD = YMD.Substring(0, 4) + "/" + YMD.Substring(4, 2) + "/" + YMD.Substring(6, 2);

                // 終売判断：2020/04/15
                DateTime dt;
                if (DateTime.TryParse(L_YMD, out dt))
                {
                    if (dt < DateTime.Today)
                    {
                        SHUBAI = true;
                    }
                }
            }

            return SHUBAI;
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     幅w、高さhのImageオブジェクトを作成 </summary>
        /// <param name="image">
        ///     イメージ</param>
        /// <param name="w">
        ///     画像サイズ・幅</param>
        /// <param name="h">
        ///     画像サイズ・高さ</param>
        ///----------------------------------------------------------
        public static Image CreateThumbnail(Image image, int w, int h)
        {
            Bitmap canvas = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(canvas);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);

            float fw = (float)w / (float)image.Width;
            float fh = (float)h / (float)image.Height;

            float scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }

        public static string GetWithoutCorp(string s)
        {
            string sName = s.Trim().Replace("（株）", "");
            sName = sName.Replace("株式会社", "");
            sName = sName.Replace("(株)", "");
            sName = sName.Replace("㈱", "");
            sName = sName.Replace("有限会社", "");
            sName = sName.Replace("（有）", "");
            sName = sName.Replace("(有)", "");
            sName = sName.Replace("㈲", "");
            sName = sName.Replace(@"\", "");    // ファイル名禁止文字
            sName = sName.Replace("/", "");     // ファイル名禁止文字
            sName = sName.Replace(":", "");     // ファイル名禁止文字
            sName = sName.Replace("*", "");     // ファイル名禁止文字
            sName = sName.Replace("?", "");     // ファイル名禁止文字
            sName = sName.Replace("\"", "");    // ファイル名禁止文字
            sName = sName.Replace("<", "");     // ファイル名禁止文字
            sName = sName.Replace(">", "");     // ファイル名禁止文字
            sName = sName.Replace("|", "");     // ファイル名禁止文字

            return sName;
        }


        //部門コンボボックスクラス
        public class ComboBumon
        {
            public string ID { get; set; }
            public string DisplayName { get; set; }
            public string Name { get; set; }
            public string NameShow { get; set; }
            public string code { get; set; }

            ///-------------------------------------------------------------------------
            /// <summary>
            ///     部門マスターロード </summary>
            /// <param name="tempObj">
            ///     コンボボックスオブジェクト</param>
            /// <param name="bumon">
            ///     部門 DataTable</param>
            ///-------------------------------------------------------------------------
            public static void Load(ComboBox tempObj, System.Data.DataTable bumon)
            {
                try
                {
                    ComboBumon cmb1;

                    tempObj.Items.Clear();
                    tempObj.DisplayMember = "Name";
                    tempObj.ValueMember = "ID";

                    DataRow[] rows = bumon.AsEnumerable().OrderBy(a => Utility.StrtoInt(a["部門コード"].ToString())).ToArray();

                    foreach (var t in rows)
                    {
                        cmb1 = new ComboBumon();
                        cmb1.ID = t["部門コード"].ToString();       // 部門コード
                        cmb1.Name = t["部門名"].ToString();        // 部門名
                        tempObj.Items.Add(cmb1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "部門コンボボックスロード");
                }
            }

            //部門コンボ表示
            public static void selectedIndex(ComboBox tempObj, int id)
            {
                //ComboBumon cmbS = new ComboBumon();
                //Boolean Sh;

                //Sh = false;

                //for (int iX = 0; iX <= tempObj.Items.Count - 1; iX++)
                //{
                //    tempObj.SelectedIndex = iX;
                //    cmbS = (ComboBumon)tempObj.SelectedItem;

                //    if (cmbS.ID == id.ToString())
                //    {
                //        Sh = true;
                //        break;
                //    }
                //}

                //if (Sh == false)
                //{
                //    tempObj.SelectedIndex = -1;
                //}
            }
        }

    }
}
