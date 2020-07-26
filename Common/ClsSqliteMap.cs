using System.Data.Linq.Mapping;

namespace IWT_OCR.Common
{
    public class ClsSqliteMap
    {

    }

    // 環境設定
    [Table(Name = "環境設定")]
    public class ClsSystemConfig
    {
        [Column(Name = "ID", IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column(Name = "CSVデータ作成パス")]
        public string DataPath { get; set; }

        [Column(Name = "画像保存先パス")]
        public string ImgPath { get; set; }

        [Column(Name = "データ保存月数")]
        public int DataSpan { get; set; }

        [Column(Name = "ログ保存月数")]
        public int LogSpan { get; set; }

        [Column(Name = "仕入先マスターパス")]
        public string ShiireMstPath { get; set; }

        [Column(Name = "商品マスターパス")]
        public string HinMstPath { get; set; }

        [Column(Name = "取引先マスターパス")]
        public string TorihikiMstPath { get; set; }

        [Column(Name = "部門マスターパス")]
        public string BumonMstPath { get; set; }

        [Column(Name = "更新年月日")]
        public string YyMmDd { get; set; }
    }

    // 編集ログ
    [Table(Name = "編集ログ")]
    public class ClsDataEditLog
    {
        [Column(Name = "ID", IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column(Name = "年月日時刻")]
        public string Date_Time { get; set; }

        [Column(Name = "項目名")]
        public string FieldName { get; set; }

        [Column(Name = "変更前値")]
        public string BeforeValue { get; set; }

        [Column(Name = "変更後値")]
        public string AfterValue { get; set; }

        [Column(Name = "画像名")]
        public string ImageFileName { get; set; }

        [Column(Name = "編集アカウントID")]
        public string Edit_AccountID { get; set; }

        [Column(Name = "コンピューター名")]
        public string ComputerName { get; set; }

        [Column(Name = "更新年月日")]
        public string YyMmDd { get; set; }
    }

    // 納品伝票
    [Table(Name = "納品伝票")]
    public class ClsDeviveryNote
    {
        [Column(Name = "ID", IsPrimaryKey = true)]
        public string ID { get; set; }

        [Column(Name = "伝票区分")]
        public int DenKbn { get; set; }

        [Column(Name = "画像名")]
        public string ImageFileName { get; set; }

        [Column(Name = "納入年月日")]
        public string NounyuDate { get; set; }

        [Column(Name = "部品コード1")]
        public string BuhinCode_1 { get; set; }

        [Column(Name = "数量1")]
        public string Suu_1 { get; set; }

        [Column(Name = "部品コード2")]
        public string BuhinCode_2 { get; set; }

        [Column(Name = "数量2")]
        public string Suu_2 { get; set; }

        [Column(Name = "部品コード3")]
        public string BuhinCode_3 { get; set; }

        [Column(Name = "数量3")]
        public string Suu_3 { get; set; }

        [Column(Name = "部品コード4")]
        public string BuhinCode_4 { get; set; }

        [Column(Name = "数量4")]
        public string Suu_4 { get; set; }

        [Column(Name = "部品コード5")]
        public string BuhinCode_5 { get; set; }

        [Column(Name = "数量5")]
        public string Suu_5 { get; set; }

        [Column(Name = "納入先名")]
        public string NonyuName { get; set; }

        [Column(Name = "納入先コード")]
        public int NonyuCode { get; set; }

        [Column(Name = "備考")]
        public string Bikou { get; set; }

        [Column(Name = "メモ")]
        public string memo { get; set; }

        [Column(Name = "確認")]
        public int Check { get; set; }

        [Column(Name = "更新年月日")]
        public string YyMmDd { get; set; }

        [Column(Name = "掛現金区分")]
        public int KakeGenkin { get; set; }

        [Column(Name = "受注先コード")]
        public int JyuchuCode { get; set; }

        [Column(Name = "請求先コード")]
        public int SeikyuuCode { get; set; }

        [Column(Name = "部門コード")]
        public int BumonCode { get; set; }
    }

    // 保留伝票
    [Table(Name = "保留伝票")]
    public class ClsHoldData
    {
        [Column(Name = "ID", IsPrimaryKey = true)]
        public string ID { get; set; }

        [Column(Name = "伝票区分")]
        public int DenKbn { get; set; }

        [Column(Name = "画像名")]
        public string ImageFileName { get; set; }

        [Column(Name = "納入年月日")]
        public string NounyuDate { get; set; }

        [Column(Name = "部品コード1")]
        public string BuhinCode_1 { get; set; }

        [Column(Name = "数量1")]
        public string Suu_1 { get; set; }

        [Column(Name = "部品コード2")]
        public string BuhinCode_2 { get; set; }

        [Column(Name = "数量2")]
        public string Suu_2 { get; set; }

        [Column(Name = "部品コード3")]
        public string BuhinCode_3 { get; set; }

        [Column(Name = "数量3")]
        public string Suu_3 { get; set; }

        [Column(Name = "部品コード4")]
        public string BuhinCode_4 { get; set; }

        [Column(Name = "数量4")]
        public string Suu_4 { get; set; }

        [Column(Name = "部品コード5")]
        public string BuhinCode_5 { get; set; }

        [Column(Name = "数量5")]
        public string Suu_5 { get; set; }

        [Column(Name = "納入先名")]
        public string NonyuName { get; set; }

        [Column(Name = "納入先コード")]
        public int NonyuCode { get; set; }

        [Column(Name = "備考")]
        public string Bikou { get; set; }

        [Column(Name = "メモ")]
        public string memo { get; set; }

        [Column(Name = "確認")]
        public int Check { get; set; }

        [Column(Name = "更新年月日")]
        public string YyMmDd { get; set; }

        [Column(Name = "掛現金区分")]
        public int KakeGenkin { get; set; }

        [Column(Name = "受注先コード")]
        public int JyuchuCode { get; set; }

        [Column(Name = "請求先コード")]
        public int SeikyuuCode { get; set; }

        [Column(Name = "部門コード")]
        public int BumonCode { get; set; }
    }
}
