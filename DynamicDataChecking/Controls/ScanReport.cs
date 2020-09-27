using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows;
using System.Reflection;
using System.Globalization;
using DynamicDataChecking.CommonClass;
using DynamicDataChecking.Common;
using System.IO;
using DynamicDataChecking.Common.CommonClass;

namespace DynamicDataChecking
{
    public partial class ScanReport : UserControl
    {
        bool IsdgvFormat = false;
        DataTable dt;
        DataTable ExcludeOrderTable;
        DataTable HoldOrderRuleTable;
        private bool result;
        TabControl tabControlMain;
        OpaqueCommand oc = new OpaqueCommand(125, true); //加载层
        StatusStrip sspStatusBar;
        private static List<BusinessDataSetting> DataSettingList { get; set; }
        private List<Mas_ValidationRule> RuleList { get; set; }
        private List<ScanModel> POMACList = new List<ScanModel>();

        public ScanReport(TabControl tabControlMain)
        {
            InitializeComponent();
            this.tabControlMain = tabControlMain;
            this.Dock = DockStyle.Fill;
            Control[] ctrl = tabControlMain.Parent.Controls.Find("sspStatusBar", true);
            if (ctrl != null && ctrl[0].GetType().Name == "StatusStrip")
                sspStatusBar = (StatusStrip)ctrl[0];

            this.Load += POForm_Load;
        }
        //public ScanReport()
        //{
        //    InitializeComponent();
        //    //this.tabControlMain = null;
        //    //this.Dock = DockStyle.Fill;
        //    //Control[] ctrl = tabControlMain.Parent.Controls.Find("sspStatusBar", true);
        //    //if (ctrl != null && ctrl[0].GetType().Name == "StatusStrip")
        //    //    sspStatusBar = (StatusStrip)ctrl[0];

        //    //this.Load += POForm_Load;
        //}

        void POForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadDataSetting();
                this.LoadRulesSetting();
                this.txtPONumber.FocusAndSelectAll();
                //Bind();
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                MessageBox.Show(this,ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void LoadRulesSetting()
        {
            RuleList = new List<Mas_ValidationRule>();
            RuleList.Add(new Mas_ValidationRule() { Operator = "Truncate", StartNumber = "1", EndNumber = "2", RuleTarget = "DeviceId" });
        }

        private void LoadDataSetting()
        {
            DataSettingList = new List<BusinessDataSetting>();
            DataSettingList.Add(new BusinessDataSetting() { FileType = "Data", FileName = "Data", FileExtension = "csv", FileSpilit = ",", TitleColumn = "DeviceId,ModelNumber,HardwareVersion,QRCodeFormat,EncryptedQRCode,ScanTime,Barcode", DataIndex = 1 });
            DataSettingList.Add(new BusinessDataSetting() { FileType = "Import", FileName = "Data", FileExtension = "csv", FileSpilit = ",", TitleColumn = "DeviceId,ModelNumber,HardwareVersion,QRCodeFormat,EncryptedQRCode", DataIndex = 1 });
        }

        void Bind()
        {
            try
            {
                string poNumber = this.txtPONumber.Text.Trim();
                if (string.IsNullOrEmpty(poNumber) == true)
                {
                    MessageBox.Show(this, "请输入PO号", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //string strMAC = this.txtMAC.Text.Trim();
                //string strProductCode = this.txtProduct.Text.Trim();
                string strSN = this.txtSN.Text.Trim();
                //oc.ShowOpaqueLayer(this.Parent);
                new Thread(delegate()
                {
                    try
                    {

                        List<ScanModel> list = SearchPO(poNumber);

                        this.Invoke(new Action(delegate
                        {
                            this.dataGridView.DataSource = list;
                            //List<string> exportColumnNameList = new List<string>() { "PONumber","SN", "ScanTime" };
                            //for (int j = this.dataGridView.Columns.Count - 1; j >= 0; j--)
                            //{
                            //    if (exportColumnNameList.Contains(this.dataGridView.Columns[j].HeaderText) == false)
                            //    {
                            //        this.dataGridView.Columns[j].Visible = false;
                            //    }
                            //}
                            //this.dataGridView.Columns["ScanTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

                            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
                            sspStatusBar.Items[0].Text = string.Format("总共 {0} 条记录", dataGridView.Rows.Count);
                        }));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex.ToString());
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }));
                    }
                    finally
                    {
                        //oc.HideOpaqueLayer();
                    }

                }).Start();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<ScanModel> SearchPO(string poNumber)
        {
            BusinessDataSetting setting = DataSettingList.FirstOrDefault(c => c.FileType == "Data");
            string po = poNumber + "." + setting.FileExtension;
            string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"ScanLogs";
            List<ScanModel> modelList = new List<ScanModel>();
            if (!Directory.Exists(strPath))
            {
                return modelList;
            }
            strPath = strPath + "\\" + po;

            if (File.Exists(strPath) == false)
            {
                return modelList;
            }
            string content = "";
            using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read))
            {
                StreamReader thisStreamReader = new StreamReader(fs, Encoding.UTF8);
                content = thisStreamReader.ReadToEnd();
                thisStreamReader.Close();
            }

            List<string> dataList = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //format string to Model
            foreach (string entityContent in dataList)
            {
                if (dataList.IndexOf(entityContent) >= setting.DataIndex)
                {
                    ScanModel model;
                    if (FormatModel(entityContent, setting, out model))
                        modelList.Add(model);
                }
            }

            return modelList;
        }
        private bool FormatModel(string entityContent, BusinessDataSetting setting, out ScanModel model)
        {
            model = new ScanModel();
            List<string> dataList = entityContent.Split(new string[] { setting.FileSpilit }, StringSplitOptions.None).ToList();
            List<string> titleColumnList = setting.TitleColumn.Split(new string[] { setting.FileSpilit }, StringSplitOptions.None).ToList();
            if (string.IsNullOrEmpty(string.Join("", dataList).Trim()) == false)
            {
                if (dataList.Count < titleColumnList.Count) throw new Exception("导入文件格式出错");
                model.DeviceId = dataList[0];
                model.ModelNumber = dataList[1];
                model.HardwareVersion = dataList[2];
                model.QRCodeFormat = dataList[3];
                model.EncryptedQRCode = dataList[4];
                if (titleColumnList.Contains("ScanTime"))
                {
                    model.ScanTime = dataList[5];
                    model.Barcode = dataList[6];
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void tsbSearch_Click(object sender, EventArgs e)
        {
            Bind();
        }

        private void tsbExport_Click(object sender, EventArgs e)
        {
            string poNumber = this.txtPONumber.Text.Trim();
            if (string.IsNullOrEmpty(poNumber) == true)
            {
                MessageBox.Show(this, "请输入PO号", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            List<ScanModel> list = SearchPO(poNumber);
            if (list == null || list.Count == 0)
            {
                MessageBox.Show(this, "No data.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //有选中则导出选中没有则全部
            DataTable dtCopy;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = SystemConst.Export_CSV_Filter;
            if (save.ShowDialog() == DialogResult.OK)
            {
                string fileName = save.FileName;
                oc.ShowOpaqueLayer(this.Parent);
                //使用线程加载数据
                new Thread(delegate()
                {
                    try
                    {

                        SaveData(poNumber, list, fileName);

                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show(this, "Succeed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex.ToString());
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    finally
                    {
                        this.Invoke(new Action(delegate
                        {
                            oc.HideOpaqueLayer();
                        }));
                    }
                }).Start();

            }

        }

        private void ExportExcelFile(string fileName, DataTable dtCopy)
        {
        }

        private DataTable FormateExportDataTable(DataTable dtCopy)
        {
            List<string> listVisibleColumns = new List<string>();
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                if (!item.Visible && item.Name != "IsNew")
                    listVisibleColumns.Add(item.Name);
            }
            foreach (string item in listVisibleColumns)
                dtCopy.Columns.Remove(item);

            return dtCopy;
        }

        #region set txt Enter Keys
        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                Bind();
            }
        }
        #endregion
        
        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            
        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
           
        }

        public static void SaveData(string ponumber, List<ScanModel> dataList, string exportFileName = "")
        {

            BusinessDataSetting setting = DataSettingList.FirstOrDefault(c => c.FileType == "Data");
            string fileName = ponumber + "." + setting.FileExtension;
            string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"ScanLogs";



            strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"ScanLogs";

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            strPath = strPath + "\\" + fileName;

            if (string.IsNullOrEmpty(exportFileName) == false)
            {
                strPath = exportFileName;
            }

            //format datalist to string
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(setting.TitleColumn);
            foreach (ScanModel entity in dataList)
            {
                sb.AppendLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", setting.FileSpilit, entity.DeviceId, entity.ModelNumber, entity.HardwareVersion, entity.QRCodeFormat, entity.EncryptedQRCode, entity.ScanTime, entity.Barcode));
            }

            using (FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamWriter thisStreamWriter = new StreamWriter(fs, Encoding.UTF8);
                //thisStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                thisStreamWriter.Write(sb.ToString());
                thisStreamWriter.Flush();
                thisStreamWriter.Close();
            }
        }

    }
}
