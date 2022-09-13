
namespace MyGIS.Forms
{
    partial class ConstructTINForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConstructTIN = new System.Windows.Forms.Button();
            this.cboField = new System.Windows.Forms.ComboBox();
            this.lblField = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.lblFind = new System.Windows.Forms.Label();
            this.cboTINType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(144, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConstructTIN
            // 
            this.btnConstructTIN.Location = new System.Drawing.Point(37, 146);
            this.btnConstructTIN.Name = "btnConstructTIN";
            this.btnConstructTIN.Size = new System.Drawing.Size(75, 23);
            this.btnConstructTIN.TabIndex = 10;
            this.btnConstructTIN.Text = "构建TIN";
            this.btnConstructTIN.UseVisualStyleBackColor = true;
            this.btnConstructTIN.Click += new System.EventHandler(this.btnConstructTIN_Click);
            // 
            // cboField
            // 
            this.cboField.FormattingEnabled = true;
            this.cboField.Location = new System.Drawing.Point(83, 60);
            this.cboField.Name = "cboField";
            this.cboField.Size = new System.Drawing.Size(162, 20);
            this.cboField.TabIndex = 9;
            // 
            // lblField
            // 
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(12, 63);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(65, 12);
            this.lblField.TabIndex = 8;
            this.lblField.Text = "字段名称：";
            // 
            // cboLayer
            // 
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(83, 18);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(162, 20);
            this.cboLayer.TabIndex = 7;
            this.cboLayer.SelectedIndexChanged += new System.EventHandler(this.cboLayer_SelectedIndexChanged);
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(12, 21);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(65, 12);
            this.lblLayer.TabIndex = 6;
            this.lblLayer.Text = "选择图层：";
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(12, 110);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(59, 12);
            this.lblFind.TabIndex = 12;
            this.lblFind.Text = "TIN类型：";
            // 
            // cboTINType
            // 
            this.cboTINType.FormattingEnabled = true;
            this.cboTINType.Items.AddRange(new object[] {
            "点",
            "直线",
            "光滑线"});
            this.cboTINType.Location = new System.Drawing.Point(83, 107);
            this.cboTINType.Name = "cboTINType";
            this.cboTINType.Size = new System.Drawing.Size(162, 20);
            this.cboTINType.TabIndex = 13;
            // 
            // ConstructTINForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 187);
            this.Controls.Add(this.cboTINType);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConstructTIN);
            this.Controls.Add(this.cboField);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.cboLayer);
            this.Controls.Add(this.lblLayer);
            this.Name = "ConstructTINForm";
            this.Text = "属性查询";
            this.Load += new System.EventHandler(this.ConstructTINForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConstructTIN;
        private System.Windows.Forms.ComboBox cboField;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.ComboBox cboTINType;
    }
}