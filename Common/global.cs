using System;
using System.Data;

namespace IWT_OCR.Common
{
    class global
    {
        #region 画像表示倍率（%）・座標
        public float miMdlZoomRate = 0f;     // 現在の表示倍率
        public float ZOOM_RATE = 0.36f;      // 標準倍率
        public float ZOOM_MAX = 2.00f;       // 最大倍率
        public float ZOOM_MIN = 0.05f;       // 最小倍率
        public float ZOOM_STEP = 0.05f;      // ステップ倍率
        public float ZOOM_NOW = 0.0f;        // 現在の倍率

        public int RECTD_NOW = 0;            // 現在の座標
        public int RECTS_NOW = 0;            // 現在の座標
        public int RECT_STEP = 20;           // ステップ座標
        #endregion

        // 表示色
        public static System.Drawing.Color defaultColor = System.Drawing.Color.DarkBlue;

        // ChangeValueStatus
        public static bool ChangeValueStatus = true;

        #region ローカルMDB関連定数
        public const string MDBFILE = "NHBR_CLI.mdb";           // MDBファイル名
        public const string MDBTEMP = "NHBR_CLI_Temp.mdb";      // 最適化一時ファイル名
        public const string MDBBACK = "Backmdb.mdb";        // 最適化後バックアップファイル名
        #endregion

        #region フラグオン・オフ定数
        public const int flgOn = 1;            //フラグ有り(1)
        public const int flgOff = 0;           //フラグなし(0)
        public const string FLGON = "1";
        public const string FLGOFF = "0";
        #endregion

        public static int pblDenNum = 0;            // データ数

        public const int configKEY = 1;         // 環境設定データキー
        public const int mailKey = 1;           // メール設定データキー

        //ＯＣＲ処理ＣＳＶデータの検証要素
        public const int CSVLENGTH = 197;          //データフィールド数 2011/06/11
        public const int CSVFILENAMELENGTH = 21;   //ファイル名の文字数 2011/06/11  

        #region 環境設定項目
        public int cnfYear = 0;                  // 対象年
        public int cnfMonth = 0;                 // 対象月
        public string cnfPath = string.Empty;    // 受け渡しデータ作成パス
        public string cnfImgPath = string.Empty; // 画像保存先パス
        public string cnfLogPath = string.Empty; // ログデータ作成パス
        public int cnfArchived = 0;              // データ保管期間（月数）
        #endregion

        // ＯＣＲモード
        public static string OCR_SCAN = "1";
        public static string OCR_IMAGE = "2";

        // フォーム登録モード
        public const int FORM_ADDMODE = 0;
        public const int FORM_EDITMODE = 1;

        // 年月日未設定値
        public static DateTime NODATE = DateTime.Parse("1900/01/01");

        // ＣＳＶファイル名
        public static string CSV_LOG = "logdata";   // ログデータ

        public static string LOCK_FILEHEAD = "LOCK-";    //LOCKFILENAMEの前に付加する文字列

        // グリッドビューで次の行の移動先カラム名
        public static string NEXT_COLUMN = string.Empty;

        // CSVデータ
        public static string DTKBN = "1";

        // データセット：2020/04/09
        public static DataSet DataSet;

        // データテーブル：2020/04/09
        public static DataTable dtSyohin;       // 商品データテーブル
        public static DataTable dtShiire;       // 仕入先データテーブル
        public static DataTable dtTorihiki;     // 取引先テーブル
        //public static DataTable dtBumon;        // 部門テーブル     2020/08/17 コメント化

        // 伝票区分
        public const string DEN_NOUHINKARI = "1";   // 納品仮伝票
        public const string DEN_NOUHIN = "2";       // 納品書
        public const string DEN_GENPIN = "3";       // 現品票

        // 伝票種別 : 2020/09/24
        public const string DEN_URIAGE = "1";       // 売上
        public const string DEN_SHIIRE = "2";       // 仕入
        public static string[] DEN_ARRAY = { "不明", "売上", "仕入" };
    }
}
