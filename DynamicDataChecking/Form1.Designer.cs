namespace DynamicDataChecking
{
    partial class Form1
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
            this.label9 = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblMAC = new System.Windows.Forms.Label();
            this.txtQRCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDeviceId = new System.Windows.Forms.TextBox();
            this.lblScanResult = new System.Windows.Forms.Label();
            this.lblPrintQty = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPrintMACQty = new System.Windows.Forms.Label();
            this.lblSystemTime1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(54, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(155, 35);
            this.label9.TabIndex = 18;
            this.label9.Text = "一维条码";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBarcode.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBarcode.Location = new System.Drawing.Point(49, 60);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(818, 47);
            this.txtBarcode.TabIndex = 17;
            // 
            // lblMAC
            // 
            this.lblMAC.AutoSize = true;
            this.lblMAC.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMAC.ForeColor = System.Drawing.Color.White;
            this.lblMAC.Location = new System.Drawing.Point(54, 129);
            this.lblMAC.Name = "lblMAC";
            this.lblMAC.Size = new System.Drawing.Size(155, 35);
            this.lblMAC.TabIndex = 20;
            this.lblMAC.Text = "二维条码";
            // 
            // txtQRCode
            // 
            this.txtQRCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQRCode.Enabled = false;
            this.txtQRCode.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtQRCode.Location = new System.Drawing.Point(49, 175);
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.Size = new System.Drawing.Size(818, 47);
            this.txtQRCode.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(54, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 35);
            this.label1.TabIndex = 22;
            this.label1.Text = "DeviceId";
            // 
            // txtDeviceId
            // 
            this.txtDeviceId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeviceId.Enabled = false;
            this.txtDeviceId.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDeviceId.Location = new System.Drawing.Point(49, 276);
            this.txtDeviceId.Name = "txtDeviceId";
            this.txtDeviceId.Size = new System.Drawing.Size(818, 47);
            this.txtDeviceId.TabIndex = 21;
            // 
            // lblScanResult
            // 
            this.lblScanResult.BackColor = System.Drawing.Color.Lime;
            this.lblScanResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblScanResult.Font = new System.Drawing.Font("SimSun", 45F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblScanResult.ForeColor = System.Drawing.Color.Black;
            this.lblScanResult.Location = new System.Drawing.Point(0, 480);
            this.lblScanResult.Name = "lblScanResult";
            this.lblScanResult.Size = new System.Drawing.Size(921, 66);
            this.lblScanResult.TabIndex = 23;
            this.lblScanResult.Text = "Success";
            // 
            // lblPrintQty
            // 
            this.lblPrintQty.Font = new System.Drawing.Font("SimHei", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPrintQty.ForeColor = System.Drawing.Color.Lime;
            this.lblPrintQty.Location = new System.Drawing.Point(261, 411);
            this.lblPrintQty.Name = "lblPrintQty";
            this.lblPrintQty.Size = new System.Drawing.Size(110, 31);
            this.lblPrintQty.TabIndex = 26;
            this.lblPrintQty.Text = "3000";
            this.lblPrintQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(68, 412);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 29);
            this.label2.TabIndex = 24;
            this.label2.Text = "未扫描条码";
            // 
            // lblPrintMACQty
            // 
            this.lblPrintMACQty.Font = new System.Drawing.Font("SimHei", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPrintMACQty.ForeColor = System.Drawing.Color.Lime;
            this.lblPrintMACQty.Location = new System.Drawing.Point(261, 359);
            this.lblPrintMACQty.Name = "lblPrintMACQty";
            this.lblPrintMACQty.Size = new System.Drawing.Size(110, 31);
            this.lblPrintMACQty.TabIndex = 27;
            this.lblPrintMACQty.Text = "100";
            this.lblPrintMACQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSystemTime1
            // 
            this.lblSystemTime1.AutoSize = true;
            this.lblSystemTime1.Font = new System.Drawing.Font("SimSun", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSystemTime1.ForeColor = System.Drawing.Color.White;
            this.lblSystemTime1.Location = new System.Drawing.Point(68, 359);
            this.lblSystemTime1.Name = "lblSystemTime1";
            this.lblSystemTime1.Size = new System.Drawing.Size(158, 29);
            this.lblSystemTime1.TabIndex = 25;
            this.lblSystemTime1.Text = "已扫描条码";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(921, 546);
            this.Controls.Add(this.lblPrintQty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblPrintMACQty);
            this.Controls.Add(this.lblSystemTime1);
            this.Controls.Add(this.lblScanResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDeviceId);
            this.Controls.Add(this.lblMAC);
            this.Controls.Add(this.txtQRCode);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtBarcode);
            this.Name = "Form1";
            this.Text = "条码扫描";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblMAC;
        private System.Windows.Forms.TextBox txtQRCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDeviceId;
        private System.Windows.Forms.Label lblScanResult;
        private System.Windows.Forms.Label lblPrintQty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPrintMACQty;
        private System.Windows.Forms.Label lblSystemTime1;
    }
}

