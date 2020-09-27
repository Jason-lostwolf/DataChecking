using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynamicDataChecking
{
    public partial class FrmScanReport : Form
    {
        public FrmScanReport()
        {
            InitializeComponent();
            this.Load += FrmScanReport_Load;
        }

        private void FrmScanReport_Load(object sender, EventArgs e)
        {
            bool needTabControl = true;
            TabPage page = new TabPage("报表查询");
            page.Controls.Add(new ScanReport(tabControlMain));
            if (needTabControl == true)
            {
                tabControlMain.TabPages.Add(page);
                tabControlMain.SelectedTab = page;
            }
        }
    }
}
