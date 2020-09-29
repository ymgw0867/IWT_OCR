using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace IWT_OCR.Common
{
    class OCRData
    {
        public OCRData()
        {

        }

        #region エラー項目番号プロパティ
        //---------------------------------------------------
        //          エラー情報
        //---------------------------------------------------

        enum errCode
        {
            eNothing, eYearMonth, eMonth, eDay, eKinmuTaikeiCode
        }

        /// <summary>
        ///     エラーヘッダ行RowIndex</summary>
        public int _errHeaderIndex { get; set; }

        /// <summary>
        ///     エラー項目番号</summary>
        public int _errNumber { get; set; }

        /// <summary>
        ///     エラー明細行RowIndex </summary>
        public int _errRow { get; set; }

        /// <summary> 
        ///     エラーメッセージ </summary>
        public string _errMsg { get; set; }

        /// <summary> 
        ///     エラーなし </summary>
        public int eNothing = 0;

        /// <summary>
        ///     エラー項目 = 確認チェック </summary>
        public int eDataCheck = 35;

        /// <summary> 
        ///     エラー項目 = 対象年月日 </summary>
        public int eYearMonth = 1;

        /// <summary> 
        ///     エラー項目 = 月 </summary>
        public int eMonth = 2;

        /// <summary> 
        ///     エラー項目 = 日 </summary>
        public int eDay = 3;

        /// <summary> 
        ///     エラー項目 = パターンID </summary>
        public int ePattern = 4;

        /// <summary> 
        ///     エラー項目 = 仕入先番号 </summary>
        public int eShiireNo = 5;

        /// <summary> 
        ///     エラー項目 = 商品コード </summary>
        public int eHinCode = 6;

        /// <summary> 
        ///     エラー項目 = 発注数 </summary>
        public int eSuu = 7;

        /// <summary> 
        ///     エラー項目 = 請求先コード </summary>
        public int eSeikyuuCode = 9;

        /// <summary>
        ///     エラー項目 = 部門コード </summary>
        public int eBumom = 10;

        /// <summary>
        ///     エラー項目 = 受注先コード </summary>
        public int eJyuchuuCode = 11;

        /// <summary>
        ///     伝票種別 </summary>
        public int eDenpyo = 36;

        #endregion

        #region 警告項目
        ///     <!--警告項目配列 -->
        public int[] warArray = new int[6];

        /// <summary>
        ///     警告項目番号</summary>
        public int _warNumber { get; set; }

        /// <summary>
        ///     警告明細行RowIndex </summary>
        public int _warRow { get; set; }

        /// <summary> 
        ///     警告項目 = 勤怠記号1&2 </summary>
        public int wKintaiKigou = 0;

        /// <summary> 
        ///     警告項目 = 開始終了時分 </summary>
        public int wSEHM = 1;

        /// <summary> 
        ///     警告項目 = 時間外時分 </summary>
        public int wZHM = 2;

        /// <summary> 
        ///     警告項目 = 深夜勤務時分 </summary>
        public int wSIHM = 3;

        /// <summary> 
        ///     警告項目 = 休日出勤時分 </summary>
        public int wKSHM = 4;

        /// <summary> 
        ///     警告項目 = 出勤形態 </summary>
        public int wShukeitai = 5;

        #endregion

        // 発注書パターン
        //ClsOrderPattern OrderPattern = null;

        ///----------------------------------------------------------------------------------------
        /// <summary>
        ///     値1がemptyで値2がNot string.Empty のとき "0"を返す。そうではないとき値1をそのまま返す</summary>
        /// <param name="str1">
        ///     値1：文字列</param>
        /// <param name="str2">
        ///     値2：文字列</param>
        /// <returns>
        ///     文字列</returns>
        ///----------------------------------------------------------------------------------------
        private string hmStrToZero(string str1, string str2)
        {
            string rVal = str1;
            if (str1 == string.Empty && str2 != string.Empty)
            {
                rVal = "0";
            }

            return rVal;
        }


        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        ///     エラーチェックメイン処理。
        ///     エラーのときOCRDataクラスのヘッダ行インデックス、フィールド番号、明細行インデックス、
        ///     エラーメッセージが記録される </summary>
        /// <param name="sIx">
        ///     開始ヘッダ行インデックス</param>
        /// <param name="eIx">
        ///     終了ヘッダ行インデックス</param>
        /// <param name="frm">
        ///     親フォーム</param>
        /// <param name="tblFax">
        ///     ClsFaxOrderクラス</param>
        /// <param name="tblPtn">
        ///     ClsOrderPatternクラス</param>
        /// <param name="cID">
        ///     FAX注文書@ID配列</param>
        /// <returns>
        ///     True:エラーなし、false:エラーあり</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckMain(int sIx, int eIx, Form frm, Table<ClsDeviveryNote> tblDeliv, string[] cID)
        {
            // 2020/04/08 コメント化
            int sDate = DateTime.Today.Year * 10000 + DateTime.Today.Month * 100 + DateTime.Today.Day;

            int rCnt = 0;

            // オーナーフォームを無効にする
            frm.Enabled = false;

            // プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = frm;
            frmP.Show();

            // レコード件数取得
            int cTotal = cID.Length;

            Boolean eCheck = true;

            // 発注書データ読み出し
            for (int i = 0; i < cTotal; i++)
            {
                // データ件数加算
                rCnt++;

                // プログレスバー表示
                frmP.Text = "エラーチェック実行中　" + rCnt.ToString() + "/" + cTotal.ToString();
                frmP.progressValue = rCnt * 100 / cTotal;
                frmP.ProgressStep();

                // 指定範囲ならエラーチェックを実施する
                if (i >= sIx && i <= eIx)
                {
                    // 納品伝票データのコレクションを取得します
                    ClsDeviveryNote r = tblDeliv.Single(a => a.ID == cID[i]);

                    // エラーチェック実施
                    eCheck = errCheckData(r);

                    if (!eCheck)　//エラーがあったとき
                    {
                        _errHeaderIndex = i;     // エラーとなったヘッダRowIndex
                        break;
                    }
                }
            }

            // いったんオーナーをアクティブにする
            frm.Activate();

            // 進行状況ダイアログを閉じる
            frmP.Close();

            // オーナーのフォームを有効に戻す
            frm.Enabled = true;

            return eCheck;
        }


        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        ///     エラーチェックメイン処理。
        ///     エラーのときOCRDataクラスのヘッダ行インデックス、フィールド番号、明細行インデックス、
        ///     エラーメッセージが記録される </summary>
        /// <param name="sIx">
        ///     開始ヘッダ行インデックス</param>
        /// <param name="eIx">
        ///     終了ヘッダ行インデックス</param>
        /// <param name="frm">
        ///     親フォーム</param>
        /// <param name="dtsC">
        ///     NHBR_CLIDataSet </param>
        /// <param name="dts">
        ///     NHBRDataSet </param>
        /// <param name="cID">
        ///     FAX注文書@ID配列</param>
        /// <returns>
        ///     True:エラーなし、false:エラーあり</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckMain(string sID, Table<ClsDeviveryNote> tblDev)
        {
            int sDate = DateTime.Today.Year * 10000 + DateTime.Today.Month * 100 + DateTime.Today.Day;

            // 納品伝票データ読み出し
            Boolean eCheck = true;

            // 納品伝票データのコレクションを取得します
            ClsDeviveryNote r = tblDev.Single(a => a.ID == sID);

            // エラーチェック実施
            eCheck = errCheckData(r);

            return eCheck;
        }


        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     エラー情報を取得します </summary>
        /// <param name="eID">
        ///     エラーデータのID</param>
        /// <param name="eNo">
        ///     エラー項目番号</param>
        /// <param name="eRow">
        ///     エラー明細行</param>
        /// <param name="eMsg">
        ///     表示メッセージ</param>
        ///---------------------------------------------------------------------------------
        private void setErrStatus(int eNo, int eRow, string eMsg)
        {
            //errHeaderIndex = eHRow;
            _errNumber = eNo;
            _errRow = eRow;
            _errMsg = eMsg;
        }


        ///-----------------------------------------------------------------------------------------------
        /// <summary>
        ///     項目別エラーチェック。
        ///     エラーのときヘッダ行インデックス、フィールド番号、明細行インデックス、エラーメッセージが記録される </summary>
        /// <param name="r">
        ///     納品伝票行コレクション</param>
        /// <returns>
        ///     エラーなし：true, エラー有り：false</returns>
        ///-----------------------------------------------------------------------------------------------
        /// 
        public Boolean errCheckData(ClsDeviveryNote r)
        {
            int eNum = 0;

            // 売上仕入区分：2020/09/25
            if (r.UriShiire.ToString() != global.DEN_URIAGE && r.UriShiire.ToString() != global.DEN_SHIIRE)
            {
                setErrStatus(eDenpyo, 0, "売上・仕入が不明の伝票です。削除後、再度ＯＣＲ認識を行ってください。");
                return false;
            }

            // 確認チェック
            if (r.Check == global.flgOff)
            {
                setErrStatus(eDataCheck, 0, "未確認の伝票です");
                return false;
            }

            // 納入日
            DateTime dt;
            if (!DateTime.TryParse(r.NounyuDate, out dt))
            {
                setErrStatus(eYearMonth, 0, "納入日が正しくありません");
                return false;
            }

            // 2020/09/29 コメント化
            //// 納品仮伝票・現品票：仕入
            //if (r.DenKbn.ToString() == global.DEN_NOUHINKARI || r.DenKbn.ToString() == global.DEN_GENPIN)
            //{
            //    // 仕入先コード
            //    if (!getShiireStatus(r.NonyuCode.ToString()))
            //    {
            //        setErrStatus(eShiireNo, 0, "不明な仕入先コードです");
            //        return false;
            //    }
            //}

            //// 納品書：売上
            //if (r.DenKbn == Utility.StrtoInt(global.DEN_NOUHIN))
            //{
            //    // 取引先コード
            //    if (!getTorihikisakiStatus(r.NonyuCode.ToString()))
            //    {
            //        setErrStatus(eShiireNo, 0, "不明な取引先コードです");
            //        return false;
            //    }
            //}

            // 仕入先コード：売上仕入区分で売上仕入を判断　2020/09/29
            if (r.UriShiire.ToString() == global.DEN_SHIIRE)
            {
                // 仕入先コード
                if (!getShiireStatus(r.NonyuCode.ToString()))
                {
                    setErrStatus(eShiireNo, 0, "不明な仕入先コードです");
                    return false;
                }
            }

            // 取引先コード：売上仕入区分で売上仕入を判断　2020/09/29
            if (r.UriShiire.ToString() == global.DEN_URIAGE)
            {
                // 取引先コード
                if (!getTorihikisakiStatus(r.NonyuCode.ToString()))
                {
                    setErrStatus(eShiireNo, 0, "不明な取引先コードです");
                    return false;
                }
            }

            // 商品コード（部品コード）・数量
            if (!CheckSyohin(r.BuhinCode_1, r.Suu_1, eHinCode, eSuu, 0))
            {
                return false;
            }

            if (!CheckSyohin(r.BuhinCode_2, r.Suu_2, eHinCode, eSuu, 1))
            {
                return false;
            }

            if (!CheckSyohin(r.BuhinCode_3, r.Suu_3, eHinCode, eSuu, 2))
            {
                return false;
            }

            if (!CheckSyohin(r.BuhinCode_4, r.Suu_4, eHinCode, eSuu, 3))
            {
                return false;
            }

            if (!CheckSyohin(r.BuhinCode_5, r.Suu_5, eHinCode, eSuu, 4))
            {
                return false;
            }

            // 2020/08/17 コメント化
            //// 部門コード
            //if (Utility.GetBumonFromDataTable(r.BumonCode, global.dtBumon).BumonCode == global.flgOff)
            //{
            //    setErrStatus(eBumom, 0, "部門が未選択です");
            //    return false;
            //}

            return true;
        }

        private bool CheckSyohin(string HinCode, string Suu, int errHin, int errSuu, int errRow)
        {
            // 商品コード（部品コード）・数量5
            if (!getSyohinStatus(HinCode.Trim().ToUpper()))
            {
                setErrStatus(errHin, errRow, "不明な部品コードです");
                return false;
            }

            if (HinCode.Trim() != "" && Suu.Trim() == "")
            {
                setErrStatus(errSuu, errRow, "納入数が未登録です");
                return false;
            }

            if (HinCode.Trim() == "" && Suu.Trim() != "")
            {
                setErrStatus(errHin, errRow, "部品コードが未登録です");
                return false;
            }

            return true;
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     仕入先番号が登録済みか調べる </summary>
        /// <param name="tCode">
        ///     仕入先番号</param>
        /// <returns>
        ///     true:登録済み、false:未登録</returns>
        ///--------------------------------------------------------------
        private bool getShiireStatus(string tCode)
        {
            bool rtn = false;

            if (Utility.GetShiireFromDataTable(tCode.ToUpper(), global.dtShiire).ShiireCode != global.flgOff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     取引先番号が登録済みか調べる </summary>
        /// <param name="tCode">
        ///     取引先番号</param>
        /// <returns>
        ///     true:登録済み、false:未登録</returns>
        ///--------------------------------------------------------------
        private bool getTorihikisakiStatus(string tCode)
        {
            if (Utility.GetTorihikisakiFromDataTable(tCode.ToUpper(), global.dtTorihiki).TorihikisakiCode != global.flgOff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     商品コードが登録済みか調べる </summary>
        /// <param name="tCode">
        ///     商品コード</param>
        /// <returns>
        ///     true:登録済み、false:未登録</returns>
        ///--------------------------------------------------------------
        private bool getSyohinStatus(string tCode)
        {
            bool rtn = false;

            if (tCode == string.Empty)
            {
                return true;
            }

            if (Utility.GetSyohinsFromDataTable(tCode, global.dtSyohin).SyohinCode == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /////--------------------------------------------------------------
        ///// <summary>
        /////     部門コードが登録済みか調べる </summary>
        ///// <param name="tCode">
        /////     部門コード</param>
        ///// <returns>
        /////     true:登録済み、false:未登録</returns>
        /////--------------------------------------------------------------
        //private bool getBumonStatus(int bCode)
        //{
        //    bool rtn = false;

        //    if (Utility.GetBumonFromDataTable(bCode, global.dtBumon).BumonCode  != global.flgOff)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
