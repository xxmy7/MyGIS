
namespace MyGIS.Forms
{
    partial class PointSearchResultForm
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
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // mTreeView
            // 
            this.mTreeView.Location = new System.Drawing.Point(10, 13);
            this.mTreeView.Name = "mTreeView";
            this.mTreeView.Size = new System.Drawing.Size(274, 242);
            this.mTreeView.TabIndex = 0;
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 267);
            this.Controls.Add(this.mTreeView);
            this.Name = "ResultForm";
            this.Text = "查询结果";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView mTreeView;
    }
}