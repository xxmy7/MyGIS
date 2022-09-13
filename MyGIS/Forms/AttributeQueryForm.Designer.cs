namespace MyGIS.Forms
{
    partial class AttributeQueryForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.cboField = new System.Windows.Forms.ComboBox();
            this.lblField = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.lblFind = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(192, 182);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 29);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(49, 182);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 29);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "查找";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cboField
            // 
            this.cboField.FormattingEnabled = true;
            this.cboField.Location = new System.Drawing.Point(111, 75);
            this.cboField.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboField.Name = "cboField";
            this.cboField.Size = new System.Drawing.Size(215, 23);
            this.cboField.TabIndex = 9;
            // 
            // lblField
            // 
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(16, 79);
            this.lblField.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(82, 15);
            this.lblField.TabIndex = 8;
            this.lblField.Text = "字段名称：";
            // 
            // cboLayer
            // 
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(111, 22);
            this.cboLayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(215, 23);
            this.cboLayer.TabIndex = 7;
            this.cboLayer.SelectedIndexChanged += new System.EventHandler(this.cboLayer_SelectedIndexChanged);
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(16, 26);
            this.lblLayer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(82, 15);
            this.lblLayer.TabIndex = 6;
            this.lblLayer.Text = "选择图层：";
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(16, 138);
            this.lblFind.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(82, 15);
            this.lblFind.TabIndex = 12;
            this.lblFind.Text = "查找内容：";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(111, 134);
            this.txtValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(215, 25);
            this.txtValue.TabIndex = 13;
            // 
            // AttributeQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 234);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboField);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.cboLayer);
            this.Controls.Add(this.lblLayer);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AttributeQueryForm";
            this.Text = "属性查询";
            this.Load += new System.EventHandler(this.AttributeQueryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox cboField;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.TextBox txtValue;
    }
}