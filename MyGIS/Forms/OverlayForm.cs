using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessing;

namespace MyGIS.Forms
{
    public partial class OverlayForm : Form
    {
        //����ļ�·��
        public string strOutputPath;

        public OverlayForm()
        {
            InitializeComponent();
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            //���ص��÷�ʽ
            this.cboOverLay.Items.Add("��(Intersect)");
            this.cboOverLay.Items.Add("��(Union)");
            this.cboOverLay.Items.Add("��ʶ(Identity)");
            this.cboOverLay.SelectedIndex = 0;
            //����Ĭ�����·��
            string tempDir = @"..\temp\overlayResults\";
            txtOutputPath.Text = tempDir;
        }

        private void btnInputFeat_Click(object sender, EventArgs e)
        {
            //����OpenfileDialog
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Shapefile (*.shp)|*.shp";
            openDlg.Title = "ѡ���һ��Ҫ��";
            //�����ļ���·���Ƿ����
            openDlg.CheckFileExists=true;
            openDlg.CheckPathExists=true;
            //���Ի����Դ�·��
            openDlg.InitialDirectory = @"..\temp\overlayResults\";
            //��ȡ�ļ�·����txtFeature1��
            if (openDlg.ShowDialog()==DialogResult.OK)
            {
                this.txtInputFeat.Text=openDlg.FileName;
            }
        }

        private void btnOverlayFeat_Click(object sender, EventArgs e)
        {
            //����OpenfileDialog
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Shapefile (*.shp)|*.shp";
            openDlg.Title = "ѡ��ڶ���Ҫ��";
            //�����ļ���·���Ƿ����
            openDlg.CheckFileExists = true;
            openDlg.CheckPathExists = true;
            //���Ի����Դ�·��
            openDlg.InitialDirectory = @"..\temp\overlayResults\";
            //��ȡ�ļ�·����txtFeature2��
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                this.txtOverlayFeat.Text = openDlg.FileName;
            }
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
            saveDlg.FileName = (string)cboOverLay.SelectedItem + ".shp";

            //��ȡ�ļ����·����txtOutputPath
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                txtOutputPath.Text = saveDlg.FileName;
        }

        private void btnOverLay_Click(object sender, EventArgs e)
        {
            //�ж��Ƿ�ѡ��Ҫ��
            if (this.txtInputFeat.Text==""||this.txtInputFeat.Text==null||
                this.txtOverlayFeat.Text==""||this.txtOverlayFeat.Text==null)
            {
                txtMessage.Text="�����õ���Ҫ�أ�";
                return;
            }
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            //OverwriteOutputΪ��ʱ�����ͼ��Ḳ�ǵ�ǰ�ļ����µ�ͬ��ͼ��
            gp.OverwriteOutput = true;


            //���ò�����÷����Ķ������
            object inputFeat = this.txtInputFeat.Text;
            object overlayFeat = this.txtOverlayFeat.Text;            
            IGpValueTableObject pObject = new GpValueTableObjectClass();
            pObject.SetColumns(2);
            pObject.AddRow(ref inputFeat);
            pObject.AddRow(ref overlayFeat);

            //��ȡҪ������
            string str = System.IO.Path.GetFileName(this.txtInputFeat.Text);
            int index = str.LastIndexOf(".");
            string strName = str.Remove(index);

            //����ļ��в����ڣ��ȴ���
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtOutputPath.Text)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txtOutputPath.Text));
            }

            //�������·��
            strOutputPath = txtOutputPath.Text;

            //���÷������
            IGeoProcessorResult result = null;

            //�������÷���ʵ����ִ�е��÷���
            string strOverlay = cboOverLay.SelectedItem.ToString();
            try
            {
                //��Ӵ��������Ϣ
                txtMessage.Text = "��ʼ���÷�������"+"\r\n";
                switch (strOverlay)
                {
                    case "��(Intersect)":
                        Intersect intersectTool = new Intersect();
                        //��������Ҫ��
                        intersectTool.in_features = pObject;
                        //�������·��
                        strOutputPath +=  strName +  "_intersect.shp";
                        intersectTool.out_feature_class = strOutputPath;
                        //ִ��������
                        result = gp.Execute(intersectTool, null) as IGeoProcessorResult;
                        break;
                    case "��(Union)":
                        Union unionTool = new Union();
                        //��������Ҫ��
                        unionTool.in_features = pObject;
                        //�������·��
                        strOutputPath += strName + "_" + "_union.shp";
                        unionTool.out_feature_class = strOutputPath;
                        //ִ��������
                        result = gp.Execute(unionTool, null) as IGeoProcessorResult;
                        break;
                    case "��ʶ(Identity)":
                        Identity identityTool = new Identity();
                        //��������Ҫ��
                        identityTool.in_features = inputFeat;
                        identityTool.identity_features = overlayFeat;
                        //�������·��
                        strOutputPath += strName + "_"  + "_identity.shp";
                        identityTool.out_feature_class = strOutputPath;
                        //ִ�б�ʶ����
                        result = gp.Execute(identityTool, null) as IGeoProcessorResult;
                        break;

                }
            }
            catch (System.Exception ex)
            {
                //��Ӵ��������Ϣ
                txtMessage.Text += "���÷������̳��ִ���" + ex.Message+"\r\n";
            }

            
            //�жϵ��÷����Ƿ�ɹ�
            if (result.Status != ESRI.ArcGIS.esriSystem.esriJobStatus.esriJobSucceeded)
                txtMessage.Text += "����ʧ��!";
            else
            {
                this.DialogResult = DialogResult.OK;
                txtMessage.Text += "���óɹ�!";                
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
   }
}