namespace MyGIS.Forms
{
    partial class OverlayForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboOverLay = new System.Windows.Forms.ComboBox();
            this.btnOutputLayer = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOverLay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInputFeat = new System.Windows.Forms.TextBox();
            this.txtOverlayFeat = new System.Windows.Forms.TextBox();
            this.btnInputFeat = new System.Windows.Forms.Button();
            this.btnOverlayFeat = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "输入要素:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "叠置要素:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "叠置方式:";
            // 
            // cboOverLay
            // 
            this.cboOverLay.FormattingEnabled = true;
            this.cboOverLay.Location = new System.Drawing.Point(83, 72);
            this.cboOverLay.Name = "cboOverLay";
            this.cboOverLay.Size = new System.Drawing.Size(201, 20);
            this.cboOverLay.TabIndex = 19;
            // 
            // btnOutputLayer
            // 
            this.btnOutputLayer.Location = new System.Drawing.Point(291, 102);
            this.btnOutputLayer.Name = "btnOutputLayer";
            this.btnOutputLayer.Size = new System.Drawing.Size(40, 23);
            this.btnOutputLayer.TabIndex = 22;
            this.btnOutputLayer.Text = "...";
            this.btnOutputLayer.UseVisualStyleBackColor = true;
            this.btnOutputLayer.Click += new System.EventHandler(this.btnOutputLayer_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(83, 104);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.ReadOnly = true;
            this.txtOutputPath.Size = new System.Drawing.Size(201, 21);
            this.txtOutputPath.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "输出图层:";
            // 
            // btnOverLay
            // 
            this.btnOverLay.Location = new System.Drawing.Point(71, 131);
            this.btnOverLay.Name = "btnOverLay";
            this.btnOverLay.Size = new System.Drawing.Size(75, 23);
            this.btnOverLay.TabIndex = 23;
            this.btnOverLay.Text = "分析";
            this.btnOverLay.UseVisualStyleBackColor = true;
            this.btnOverLay.Click += new System.EventHandler(this.btnOverLay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(211, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInputFeat
            // 
            this.txtInputFeat.Location = new System.Drawing.Point(83, 12);
            this.txtInputFeat.Name = "txtInputFeat";
            this.txtInputFeat.ReadOnly = true;
            this.txtInputFeat.Size = new System.Drawing.Size(200, 21);
            this.txtInputFeat.TabIndex = 25;
            // 
            // txtOverlayFeat
            // 
            this.txtOverlayFeat.Location = new System.Drawing.Point(84, 42);
            this.txtOverlayFeat.Name = "txtOverlayFeat";
            this.txtOverlayFeat.ReadOnly = true;
            this.txtOverlayFeat.Size = new System.Drawing.Size(200, 21);
            this.txtOverlayFeat.TabIndex = 26;
            // 
            // btnInputFeat
            // 
            this.btnInputFeat.Location = new System.Drawing.Point(291, 10);
            this.btnInputFeat.Name = "btnInputFeat";
            this.btnInputFeat.Size = new System.Drawing.Size(40, 23);
            this.btnInputFeat.TabIndex = 27;
            this.btnInputFeat.Text = "...";
            this.btnInputFeat.UseVisualStyleBackColor = true;
            this.btnInputFeat.Click += new System.EventHandler(this.btnInputFeat_Click);
            // 
            // btnOverlayFeat
            // 
            this.btnOverlayFeat.Location = new System.Drawing.Point(291, 40);
            this.btnOverlayFeat.Name = "btnOverlayFeat";
            this.btnOverlayFeat.Size = new System.Drawing.Size(40, 23);
            this.btnOverlayFeat.TabIndex = 28;
            this.btnOverlayFeat.Text = "...";
            this.btnOverlayFeat.UseVisualStyleBackColor = true;
            this.btnOverlayFeat.Click += new System.EventHandler(this.btnOverlayFeat_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMessage);
            this.groupBox1.Location = new System.Drawing.Point(14, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 113);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "处理过程消息";
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(3, 17);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(311, 93);
            this.txtMessage.TabIndex = 0;
            // 
            // OverlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 290);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOverlayFeat);
            this.Controls.Add(this.btnInputFeat);
            this.Controls.Add(this.txtOverlayFeat);
            this.Controls.Add(this.txtInputFeat);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOverLay);
            this.Controls.Add(this.btnOutputLayer);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboOverLay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "OverlayForm";
            this.Text = "叠置分析";
            this.Load += new System.EventHandler(this.OverlayForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboOverLay;
        private System.Windows.Forms.Button btnOutputLayer;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOverLay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInputFeat;
        private System.Windows.Forms.TextBox txtOverlayFeat;
        private System.Windows.Forms.Button btnInputFeat;
        private System.Windows.Forms.Button btnOverlayFeat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMessage;
    }
}