using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;

namespace MyGIS.Forms
{
    public partial class BufferForm : Form
    {
        //接收MapControl中的数据
        private IHookHelper mHookHelper = new HookHelperClass();
        //缓冲区文件输出路径
        public string strOutputPath;

        //重写构造函数，添加参数hook，用于传入MapControl中的数据
        public BufferForm(object hook)
        {
            InitializeComponent();
            this.mHookHelper.Hook = hook;
        }

        private void BufferForm_Load(object sender, EventArgs e)
        {
            //传入数据为空时返回
            if (null == mHookHelper || null == mHookHelper.Hook || 0 == mHookHelper.FocusMap.LayerCount)
                return;

            //获取图层名称并加入cboLayers
            for (int i = 0; i < this.mHookHelper.FocusMap.LayerCount; i++)
            {
                ILayer pLayer = this.mHookHelper.FocusMap.get_Layer(i);
                cboLayers.Items.Add(pLayer.Name);
            }

            //cboLayers控件中默认显示第一个选项
            if (cboLayers.Items.Count > 0)
                cboLayers.SelectedIndex = 0;

            //设置生成文件的默认输出路径和名称
            string tempDir = @"..\temp\bufferResults\";
            txtOutputPath.Text = System.IO.Path.Combine(tempDir, ((string)cboLayers.SelectedItem + "_buffer.shp"));

            //设置默认地图单位
            lblUnits.Text = Convert.ToString(mHookHelper.FocusMap.MapUnits);
        }

        /// <summary>
        /// 根据图层名称获取要素图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            IFeatureLayer pFeatureLayer = null;
            //遍历图层，获取与名称匹配的图层
            for (int i = 0; i < this.mHookHelper.FocusMap.LayerCount; i++)
            {
                ILayer pLayer = this.mHookHelper.FocusMap.get_Layer(i);
                if (pLayer.Name == layerName)
                {
                    pFeatureLayer = pLayer as IFeatureLayer;
                }
            }

            if (pFeatureLayer != null)
                return pFeatureLayer;
            else
                return null;
        }

        private void btnOutputLayer_Click(object sender, EventArgs e)
        {
            //定义输出文件路径
            SaveFileDialog saveDlg = new SaveFileDialog();
            //检查路径是否存在
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "Shapefile (*.shp)|*.shp";
            //保存时覆盖同名文件
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "输出路径";
            //对话框关闭前还原当前目录
            saveDlg.RestoreDirectory = true;
            saveDlg.FileName = (string)cboLayers.SelectedItem + "_buffer.shp";

            //读取文件输出路径到txtOutputPath
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                txtOutputPath.Text = saveDlg.FileName;
        }

        private void btnBuffer_Click(object sender, EventArgs e)
        {
            //缓冲距离
            double bufferDistance;
            //输入的缓冲距离转换为double
            double.TryParse(txtBufferDistance.Text.ToString(), out bufferDistance);


            //判断输出路径是否合法
            if (".shp" != System.IO.Path.GetExtension(txtOutputPath.Text))
            {
                MessageBox.Show("输出文件格式错误!");
                return;
            }

            //如果文件夹不存在，先创建
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtOutputPath.Text)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txtOutputPath.Text));
            }

            //判断图层个数
            if (mHookHelper.FocusMap.LayerCount == 0)
                return;
            //获取图层
            IFeatureLayer pFeatureLayer = GetFeatureLayer((string)cboLayers.SelectedItem);
            if (null == pFeatureLayer)
            {
                MessageBox.Show("图层 " + (string)cboLayers.SelectedItem + "不存在!\r\n");
                return;
            }

            //获取一个geoprocessor的实例
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            //OverwriteOutput为真时，输出图层会覆盖当前文件夹下的同名图层
            gp.OverwriteOutput = true;
            //缓冲区保存路径
            strOutputPath = txtOutputPath.Text;
            //创建一个Buffer工具的实例
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFeatureLayer, strOutputPath, bufferDistance.ToString());
            //执行缓冲区分析
            IGeoProcessorResult results = null;
            results = (IGeoProcessorResult)gp.Execute(buffer, null);
            //判断缓冲区是否成功生成
            if (results.Status != esriJobStatus.esriJobSucceeded)
                MessageBox.Show("图层" + pFeatureLayer.Name + "缓冲区生成失败！");
            else
            {
                this.DialogResult = DialogResult.OK;
                MessageBox.Show("缓冲区生成成功！");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}