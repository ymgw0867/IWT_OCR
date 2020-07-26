using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWT_OCR.Common
{
    class ClsCsvData
    {
        ///------------------------------------------------
        /// <summary>
        ///     商品マスター </summary>
        ///------------------------------------------------
        public class ClsCsvSyohin
        {
            public string SyohinCode { get; set; }

            public string SyohinName { get; set; }

            public string Kikaku { get; set; }
        }

        ///------------------------------------------------
        /// <summary>
        ///     仕入先マスター </summary>
        ///------------------------------------------------
        public class ClsCsvShiiresaki
        {
            public int ShiireCode { get; set; }

            public string ShiireName { get; set; }

            public string ShiireRyakusyo { get; set; }
        }

        ///------------------------------------------------
        /// <summary>
        ///     取引先マスター </summary>
        ///------------------------------------------------
        public class ClsCsvTorihikisaki
        {
            public int TorihikisakiCode { get; set; }

            public string TorihikisakiName { get; set; }

            public string TorihikisakiRyakusyo { get; set; }
        }

        ///------------------------------------------------
        /// <summary>
        ///     部門マスター </summary>
        ///------------------------------------------------
        public class ClsCsvBumon
        {
            public int BumonCode { get; set; }

            public string BumonName { get; set; }

            public string BumonRyakusyo { get; set; }
        }

        public class ClsProShiire
        {
            /// <summary>
            ///     仕入日 </summary>
            public string ShiireDate { get; set; }

            /// <summary>
            ///     仕入区分 </summary>
            public string ShiireKubun { get; set; }

            /// <summary>
            ///     仕入先コード </summary>
            public string ShiireCode { get; set; }

            /// <summary>
            ///     受注番号 </summary>
            public string OrderNum { get; set; }

            /// <summary>
            ///     部品コード </summary>
            public string BuhinCode { get; set; }

            /// <summary>
            ///     品名 </summary>
            public string BuhinName { get; set; }

            /// <summary>
            ///     規格・型番 </summary>
            public string Kikaku { get; set; }

            /// <summary>
            ///     数量 </summary>
            public string Suu { get; set; }

            /// <summary>
            ///     単位 </summary>
            public string Tanni { get; set; }

            /// <summary>
            ///     仕入単価 </summary>
            public string Tanka { get; set; }

            /// <summary>
            ///     備考１ </summary>
            public string Bikou1 { get; set; }

            /// <summary>
            ///     備考２ </summary>
            public string Bikou2 { get; set; }
        }
    }
}
