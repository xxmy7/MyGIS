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
        //����MapControl�е�����
        private IHookHelper mHookHelper = new HookHelperClass();
        //�������ļ����·��
        public string strOutputPath;

        //��д���캯������Ӳ���hook�����ڴ���MapControl�е�����
        public BufferForm(object hook)
        {
            InitializeComponent();
            this.mHookHelper.Hook = hook;
        }

        private void BufferForm_Load(object sender, EventArgs e)
        {
            //��������Ϊ��ʱ����
            if (null == mHookHelper || null == mHookHelper.Hook || 0 == mHookHelper.FocusMap.LayerCount)
                return;

            //��ȡͼ�����Ʋ�����cboLayers
            for (int i = 0; i < this.mHookHelper.FocusMap.LayerCount; i++)
            {
                ILayer pLayer = this.mHookHelper.FocusMap.get_Layer(i);
                cboLayers.Items.Add(pLayer.Name);
            }

            //cboLayers�ؼ���Ĭ����ʾ��һ��ѡ��
            if (cboLayers.Items.Count > 0)
                cboLayers.SelectedIndex = 0;

            //���������ļ���Ĭ�����·��������
            string tempDir = @"..\temp\bufferResults\";
            txtOutputPath.Text = System.IO.Path.Combine(tempDir, ((string)cboLayers.SelectedItem + "_buffer.shp"));

            //����Ĭ�ϵ�ͼ��λ
            lblUnits.Text = Convert.ToString(mHookHelper.FocusMap.MapUnits);
        }

        /// <summary>
        /// ����ͼ�����ƻ�ȡҪ��ͼ��
        /// </summary>
        /// <param name="layerName">ͼ������</param>
        /// <returns></returns>
        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            IFeatureLayer pFeatureLayer = null;
            //����ͼ�㣬��ȡ������ƥ���ͼ��
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
            //��������ļ�·��
            SaveFileDialog saveDlg = new SaveFileDialog();
            //���·���Ƿ����
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "Shapefile (*.shp)|*.shp";
            //����ʱ����ͬ���ļ�
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "���·��";
            //�Ի���ر�ǰ��ԭ��ǰĿ¼
            saveDlg.RestoreDirectory = true;
            saveDlg.FileName = (string)cboLayers.SelectedItem + "_buffer.shp";

            //��ȡ�ļ����·����txtOutputPath
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                txtOutputPath.Text = saveDlg.FileName;
        }

        private void btnBuffer_Click(object sender, EventArgs e)
        {
            //�������
            double bufferDistance;
            //����Ļ������ת��Ϊdouble
            double.TryParse(txtBufferDistance.Text.ToString(), out bufferDistance);


            //�ж����·���Ƿ�Ϸ�
            if (".shp" != System.IO.Path.GetExtension(txtOutputPath.Text))
            {
                MessageBox.Show("����ļ���ʽ����!");
                return;
            }

            //����ļ��в����ڣ��ȴ���
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtOutputPath.Text)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txtOutputPath.Text));
            }

            //�ж�ͼ�����
            if (mHookHelper.FocusMap.LayerCount == 0)
                return;
            //��ȡͼ��
            IFeatureLayer pFeatureLayer = GetFeatureLayer((string)cboLayers.SelectedItem);
            if (null == pFeatureLayer)
            {
                MessageBox.Show("ͼ�� " + (string)cboLayers.SelectedItem + "������!\r\n");
                return;
            }

            //��ȡһ��geoprocessor��ʵ��
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            //OverwriteOutputΪ��ʱ�����ͼ��Ḳ�ǵ�ǰ�ļ����µ�ͬ��ͼ��
            gp.OverwriteOutput = true;
            //����������·��
            strOutputPath = txtOutputPath.Text;
            //����һ��Buffer���ߵ�ʵ��
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFeatureLayer, strOutputPath, bufferDistance.ToString());
            //ִ�л���������
            IGeoProcessorResult results = null;
            results = (IGeoProcessorResult)gp.Execute(buffer, null);
            //�жϻ������Ƿ�ɹ�����
            if (results.Status != esriJobStatus.esriJobSucceeded)
                MessageBox.Show("ͼ��" + pFeatureLayer.Name + "����������ʧ�ܣ�");
            else
            {
                this.DialogResult = DialogResult.OK;
                MessageBox.Show("���������ɳɹ���");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}