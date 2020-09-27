using DataAccessLayer.Model;
using DynamicDataChecking.CommonClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DynamicDataChecking.Common;
using DynamicDataChecking.Common.CommonClass;

namespace DynamicDataChecking
{
    public partial class FrmScanBarcode : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(IntPtr hWnd);
        OpaqueCommand oc = new OpaqueCommand(125, true); //加载层

        public FrmScanBarcode(string version)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            InitializeComponent();
            this.Load += FrmScanBarcode_Load;
            this.Text = this.Text + version;
        }
        private static List<BusinessDataSetting> DataSettingList { get; set; }
        private List<Mas_ValidationRule> RuleList { get; set; }

        void FrmScanBarcode_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadDataSetting();
                this.LoadRulesSetting();
                this.InitDataGrid();
                this.BacktoPO();
                this.LoadPO();
                //this.RefreshDataGrid();
                //this.RefreshUI();
                this.ShowText("", false);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitScan(bool initScan = true)
        {
            this.txtPO.Enabled = !initScan;
            this.txtDeviceId.Enabled = initScan;
            if(initScan==false)
            {
                this.txtPO.FocusAndSelectAll();
            }
            else
            {
                this.txtDeviceId.FocusAndSelectAll();
            }
        }

        private void BacktoPO()
        {
            InitScan(false);
        }

        private void LoadRulesSetting()
        {
            RuleList = new List<Mas_ValidationRule>();
            RuleList.Add(new Mas_ValidationRule() { Operator = "Truncate", StartNumber = "1", EndNumber = "2", RuleTarget="DeviceId" });
        }

        private void LoadDataSetting()
        {
            DataSettingList = new List<BusinessDataSetting>();
            DataSettingList.Add(new BusinessDataSetting() { FileType = "Data", FileName = "Data", FileExtension = "csv", FileSpilit = ",", TitleColumn = "DeviceId,ModelNumber,HardwareVersion,QRCodeFormat,EncryptedQRCode,ScanTime,Barcode", DataIndex =1});
            DataSettingList.Add(new BusinessDataSetting() { FileType = "Import", FileName = "Data", FileExtension = "csv", FileSpilit = ",", TitleColumn = "DeviceId,ModelNumber,HardwareVersion,QRCodeFormat,EncryptedQRCode", DataIndex = 1 });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool IsPOLoaded { get; set; }

        private List<ScanModel> POMACList = new List<ScanModel>();
        private List<ScanModel> ViewPOMACList
        {
            get
            {
                return this.POMACList.Where(c => string.IsNullOrEmpty(c.Barcode) == false).ToList();
            }
        }
         
        //private BindingList<ScanModel> ViewPOMACList = new BindingList<ScanModel>();
        private ScanModel CurrentScanModel;
        private void LoadPO(string poNumber = "")
        {
            if (string.IsNullOrEmpty(poNumber) == false)
            {
                BusinessDataSetting setting = DataSettingList.FirstOrDefault(c => c.FileType == "Data");
                string po = poNumber + "." + setting.FileExtension;
                string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"ScanLogs";
                POMACList.Clear();
                if (!Directory.Exists(strPath))
                {
                    return;
                }
                strPath = strPath + "\\" + po;

                if (File.Exists(strPath) == false)
                {
                    return;
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
                            POMACList.Add(model);
                    }
                }
            }
            //else
            //{
            //    
            //    POMACList.Clear();
            //}
            //POMACList.AddRange(barcodeArray);

            this.RefreshDataGrid();
            this.RefreshUI();
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
                if(titleColumnList.Contains("ScanTime"))
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
        
        private void ScanBarcode(string barcode, string checkProperty)
        {
            try
            {
                if (this.IsPOLoaded == false)
                {
                    this.LoadPO();
                    this.IsPOLoaded = true;
                }
                
                string messageresult = "";
                SoundType soundType;
                bool checkResult = false;
                if(checkProperty == "DeviceId")
                {
                    checkResult = CheckDeviceId(barcode, out messageresult, out soundType);
                    if(checkResult==false)
                    {
                        ShowText(messageresult);
                        this.Invoke(new Action(delegate
                        {
                            ScanSoundHelper.PlaySound(soundType);
                            this.txtDeviceId.FocusAndSelectAll();
                        }));
                        return;
                    }
                    else
                    {
                        ShowText("", false);
                        this.txtDeviceId.DisableScan();
                        this.txtQRCode.FocusAndSelectAll();
                        return;
                    }
                }
                else
                {
                    checkResult = CheckQRCode(barcode, out messageresult, out soundType);
                    if (checkResult == false)
                    {
                        ShowText(messageresult);
                        this.Invoke(new Action(delegate
                        {
                            ScanSoundHelper.PlaySound(soundType);
                            this.txtQRCode.FocusAndSelectAll();
                        }));
                        return;
                    }
                    else
                    {
                        oc.ShowOpaqueLayer(this);
                        new Thread(delegate()
                        {
                            try
                            {
                                CurrentScanModel.ScanTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                SaveData(this.txtPO.Text, POMACList);

                                //增加条码，数量+1

                                this.Invoke(new Action(delegate
                                {
                                    this.txtShowData.Text = CurrentScanModel.DeviceId;
                                    RefreshUI();
                                    lblScanResult.Text = "成功 " + CurrentScanModel.ScanTime;// +" time:" + (endDate - startDate).TotalMilliseconds;
                                    lblScanResult.BackColor = Color.Lime;
                                    ScanSoundHelper.PlaySound(SoundType.Succeed);
                                    this.txtDeviceId.Text = "";
                                    this.txtQRCode.Text = "";
                                    this.txtDeviceId.FocusAndSelectAll();
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
                                oc.HideOpaqueLayer();
                            }

                        }).Start();
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //oc.HideOpaqueLayer();
            }
        }

        private void ShowText(string message, bool isError = true)
        {
            if (this.lblScanResult.InvokeRequired)
            {
                this.Invoke(new Action(delegate
                {
                    this.lblScanResult.Text = message;
                    this.lblScanResult.BackColor = isError ? Color.Red : Color.Lime;
                    //this.txtDeviceId.FocusAndSelectAll();
                }));
            }
            else
            {
                this.lblScanResult.Text = message;
                this.lblScanResult.BackColor = isError ? Color.Red : Color.Lime;
                //this.txtDeviceId.FocusAndSelectAll();
            }
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
            foreach(ScanModel entity in dataList)
            {
                sb.AppendLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", setting.FileSpilit, entity.DeviceId, entity.ModelNumber, entity.HardwareVersion, entity.QRCodeFormat, entity.EncryptedQRCode, entity.ScanTime,entity.Barcode));
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

        private void ChangeWindowState()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;

            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }


        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(this.txtPO.Text.Trim()))
                {
                    MessageBox.Show(this, "请输入PO", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = SystemConst.CSV_Filter;
                if (open.ShowDialog() == DialogResult.OK)
                {
                    oc.ShowOpaqueLayer(this.Parent);
                    new Thread(delegate()
                    {
                        try
                        {
                            ImportFile(open.FileName);
                            SaveData(this.txtPO.Text.Trim(), POMACList);
                                                        

                            this.Invoke(new Action(delegate
                            {
                                LoadPO(this.txtPO.Text.Trim());
                                this.txtDeviceId.FocusAndSelectAll();
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
                            oc.HideOpaqueLayer();
                        }

                    }).Start();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportFile(string fileName)
        {
            BusinessDataSetting setting = DataSettingList.FirstOrDefault(c => c.FileType == "Import");
            string strPath = fileName;

            if (File.Exists(strPath) == false)
            {
                return;
            }
            string content = "";
            using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read))
            {
                StreamReader thisStreamReader = new StreamReader(fs, Encoding.UTF8);
                content = thisStreamReader.ReadToEnd();
                thisStreamReader.Close();
            }

            List<string> dataList = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            POMACList.Clear();
            //format string to Model
            foreach (string entityContent in dataList)
            {
                if (dataList.IndexOf(entityContent) >= setting.DataIndex)
                {
                    try
                    {
                        ScanModel model;
                        if (FormatModel(entityContent, setting, out model))
                            POMACList.Add(model);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(string.Format("Row {0}:{1}", dataList.IndexOf(entityContent), entityContent), ex.InnerException);
                    }
                }
            }
            //POMACList.AddRange(barcodeArray);
        }

        private void RefreshUI()
        {
            this.lblScanQty.Text = POMACList.Count(c => string.IsNullOrEmpty(c.ScanTime) == false).ToString();
            this.lblNotScanQty.Text = POMACList.Count(c => string.IsNullOrEmpty(c.ScanTime)).ToString();
            this.lblTotalQty.Text = POMACList.Count.ToString();
        }

        #region ScanEvent
        private void txtDeviceId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string barcode = this.txtDeviceId.Text.Trim();
                    this.ScanBarcode(barcode, "DeviceId");
                }
                else if (e.KeyCode == Keys.F8)
                {
                    ChangeWindowState();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.F5)
                {
                    this.ResetTotalUI();
                    BacktoPO();
                    this.POMACList.Clear();

                    this.RefreshDataGrid();
                    this.RefreshUI();

                    this.ShowText("", false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtQRCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barcode = this.txtQRCode.Text.Trim();
                this.ScanBarcode(barcode,"QRCode");
            }
            else if (e.KeyCode == Keys.F8)
            {
                ChangeWindowState();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.ResetTotalUI();
                BacktoPO();
                this.POMACList.Clear();

                this.RefreshDataGrid();
                this.RefreshUI();

                this.ShowText("", false);
            }
        }

        private bool CheckDeviceId(string barcode, out string message, out SoundType soundType)
        {
            #region 是否匹配规则
            string ruleType = "";

            string originalBarcode = barcode;

            //截取条码
            string CheckData = barcode.Replace("-", "");
            if(CheckData.Length>=2)
            {
                CheckData = CheckData.Substring(2);
            }

            if (string.IsNullOrEmpty(CheckData) == true)
            {
                message = "条码不能为空";
                soundType = SoundType.NoData;
                return false;
            }
            #endregion

            #region whether dumplicated
            //先判断逻辑
            //1.条码不能是已扫
            ScanModel product = POMACList.FirstOrDefault(c => c.DeviceId == CheckData);
            if (product != null && string.IsNullOrEmpty(product.ScanTime)==false)
            {
                message = "条码已扫描, 时间:" + product.ScanTime;
                soundType = SoundType.SameData;
                return false;
            }
            else if (product==null)
            {
                message = "找不到条码";
                soundType = SoundType.BarcodeNotSuccess;
                return false;
            }
            #endregion

            product.Barcode = originalBarcode;
            CurrentScanModel = product;
            message = "";
            soundType = SoundType.Succeed;
            return true;
        }

        private bool CheckQRCode(string barcode, out string message, out SoundType soundType)
        {
            if (string.IsNullOrEmpty(barcode) == true)
            {
                message = "条码不能为空";
                soundType = SoundType.NoData;
                return false;
            }

            if(CurrentScanModel==null)
            {
                message = "请扫描DeviceId";
                soundType = SoundType.Error;
                return false;
            }

            if (CurrentScanModel.EncryptedQRCode!= barcode)
            {
                message = "EncryptedQRCode扫描错误";
                soundType = SoundType.Error;
                return false;
            }
            

            message = "";
            soundType = SoundType.Succeed;
            return true;
        }
        #endregion

        private void FrmScanBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                ChangeWindowState();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void tsbExport_Click(object sender, EventArgs e)
        {
            List<ScanModel> list = this.POMACList;
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
                oc.ShowOpaqueLayer(this.Parent);
                //使用线程加载数据
                new Thread(delegate()
                {
                    try
                    {

                        SaveData(this.txtPO.Text.Trim(), POMACList, save.FileName);

                        this.Invoke(new Action(delegate
                        {
                            this.txtDeviceId.FocusAndSelectAll();
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

        List<string> DisplayColumnNameList = new List<string>() { "Barcode" };
        private void InitDataGrid()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = ViewPOMACList;
            dataGridView.DataSource = bs;

            string[] displayColumns = DisplayColumnNameList.ToArray();
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                item.Visible = displayColumns.Contains(item.Name);
                item.Width = 300;
            }

        }
        private void RefreshDataGrid(ScanModel scanEntity = null)
        {
            BindingSource bs = this.dataGridView.DataSource as BindingSource;
            bs.DataSource = null;
            bs.DataSource = ViewPOMACList;
            bs.ResetBindings(true);
            string[] displayColumns = DisplayColumnNameList.ToArray();
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                item.Visible = displayColumns.Contains(item.Name);
                item.Width = 300;
            }
            //dataGridView.Columns["SN"].Width = 300;
            //dataGridView.Columns["MAC"].Width = 300;
            this.dataGridView.ClearSelection();

            if (scanEntity != null)
            {
                int pIndex = ViewPOMACList.IndexOf(scanEntity);
                if (pIndex >= 0)
                {
                    this.dataGridView.Rows[pIndex].Selected = true;
                }
            }
            else if(this.ViewPOMACList.Count>0)
            {
                this.dataGridView.Rows[this.ViewPOMACList.Count - 1].Selected = true;
            }
        }

        private void txtPO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string poNumber = this.txtPO.Text.Trim();
                //加载PO的内容同时显示
                LoadPO(poNumber);
                
                InitScan();
            }
            else if (e.KeyCode == Keys.F8)
            {
                ChangeWindowState();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.ResetTotalUI();
                BacktoPO();
                this.POMACList.Clear();

                this.RefreshDataGrid();
                this.RefreshUI();

                this.ShowText("", false);
            }
        }

        private void ResetTotalUI()
        {
            this.txtPO.Text = "";
            this.txtDeviceId.Text = "";
            this.txtQRCode.Text = "";
            this.txtShowData.Text = "";
        }

        private void btnSearchReport_Click(object sender, EventArgs e)
        {
            try
            {
                //bool txtPOFocus = this.txtPO.Focused;
                //bool txtDeviceIdFocus = this.txtDeviceId.Focused;
                //bool txtQRCodeFocus = this.txtQRCode.Focused;

                FrmScanReport frm = new FrmScanReport();
                frm.ShowDialog();

                this.txtPO.FocusAndSelectAll();
                //if (txtDeviceIdFocus) this.txtDeviceId.FocusAndSelectAll();
                //if (txtQRCodeFocus) this.txtQRCode.FocusAndSelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
