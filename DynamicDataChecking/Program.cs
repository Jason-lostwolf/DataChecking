using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DynamicDataChecking
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmScanBarcode("V1.3"));
        }
    }
}
// V1.2 变更扫描样式，增加查询和单个PO防重复功能
// V1.3 保存的时候刷新列表,Barcode右边增加列DeviceId