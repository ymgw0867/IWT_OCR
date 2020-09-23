using System;
using System.Windows.Forms;

namespace IWT_OCR
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Mutex の新しいインスタンスを生成する (Mutex の名前にアセンブリ名を付ける)
            System.Threading.Mutex hMutex = new System.Threading.Mutex(false, Application.ProductName);

            // Mutex のシグナルを受信できるかどうか判断する
            if (hMutex.WaitOne(0, false))
            {
                SetProcessDPIAware();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());

                // 2020/09/23 会社選択画面
                Common.frmCorpSelect frm = new Common.frmCorpSelect();
                Application.Run(frm);
                string Cpath = Common.Utility.NulltoStr(frm.MyProperty);
                frm.Dispose();

                // 2020/09/23 会社を指定するとメインメニューを表示
                if (Cpath != "")
                {
                    Application.Run(new Form1(Cpath));
                }
            }
            else
            {
                // グローバル・ミューテックスによる多重起動禁止
                MessageBox.Show("このアプリケーションはすでに起動しています。2つ同時には起動できません。", "多重起動禁止");
                return;
            }

            // GC.KeepAlive メソッドが呼び出されるまで、ガベージ コレクション対象から除外される
            GC.KeepAlive(hMutex);

            // Mutex を閉じる (正しくは オブジェクトの破棄を保証する を参照)
            hMutex.Close();
        }

        // 高DPI対応 : 2020/06/24
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
