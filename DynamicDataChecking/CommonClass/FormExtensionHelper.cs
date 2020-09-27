using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynamicDataChecking
{
    public static class FormExtensionHelper
    {
        public static void FocusAndSelectAll(this TextBoxBase s)
        {
            s.Enabled = true;
            s.Focus();
            s.SelectAll();
        }

        public static void DisableScan(this TextBoxBase s)
        {
            s.Enabled = false;
        }
    }
}
