namespace DynamicDataChecking
{
    partial class FrmScanReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tsslStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.sspStatusBar = new System.Windows.Forms.StatusStrip();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.sspStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsslStatusMsg
            // 
            this.tsslStatusMsg.AutoSize = false;
            this.tsslStatusMsg.Name = "tsslStatusMsg";
            this.tsslStatusMsg.Size = new System.Drawing.Size(500, 17);
            this.tsslStatusMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sspStatusBar
            // 
            this.sspStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatusMsg});
            this.sspStatusBar.Location = new System.Drawing.Point(0, 428);
            this.sspStatusBar.Name = "sspStatusBar";
            this.sspStatusBar.Size = new System.Drawing.Size(800, 22);
            this.sspStatusBar.TabIndex = 9;
            this.sspStatusBar.Text = "statusStrip1";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(800, 428);
            this.tabControlMain.TabIndex = 10;
            // 
            // FrmScanReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.sspStatusBar);
            this.Name = "FrmScanReport";
            this.Text = "报表查询";
            this.TopMost = true;
            this.sspStatusBar.ResumeLayout(false);
            this.sspStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel tsslStatusMsg;
        private System.Windows.Forms.StatusStrip sspStatusBar;
        private System.Windows.Forms.TabControl tabControlMain;
    }
}