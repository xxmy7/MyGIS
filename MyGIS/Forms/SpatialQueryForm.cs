using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;

namespace MyGIS.Forms
{
    public partial class SpatialQueryForm : Form
    {
        //��ȡ�������MapControl����
        private AxMapControl mMapControl;
        //��ѯ��ʽ
        public int mQueryMode;
        //ͼ������
        public int mLayerIndex;

        //Ϊ���캯����Ӳ���MapControl
        public SpatialQueryForm(AxMapControl mapControl)
        {
            InitializeComponent();
            this.mMapControl = mapControl;
        }

        private void SpatialQueryForm_Load(object sender, EventArgs e)
        {
            //MapControl��û��ͼ��ʱ����
            if (this.mMapControl.LayerCount <= 0)
                return;

            //��ȡMapControl�е�ȫ��ͼ�����ƣ�������ComboBox
            //ͼ��
            ILayer pLayer;
            //ͼ������
            string strLayerName;
            for (int i = 0; i < this.mMapControl.LayerCount; i++)
            {
                pLayer = this.mMapControl.get_Layer(i);
                strLayerName = pLayer.Name;
                //ͼ�����Ƽ���ComboBox
                this.cboLayer.Items.Add(strLayerName);
            }

            //���ز�ѯ��ʽ
            this.cboMode.Items.Add("���β�ѯ");
            this.cboMode.Items.Add("�߲�ѯ");
            this.cboMode.Items.Add("���ѯ");
            this.cboMode.Items.Add("Բ��ѯ");
            this.cboMode.Items.Add("����β�ѯ");
            this.cboMode.Items.Add("���߲�ѯ");

            //��ʼ��ComboBoxĬ��ֵ
            this.cboLayer.SelectedIndex = 0;
            this.cboMode.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //���������ʱ����Ľ��
            this.DialogResult = DialogResult.OK;
            //�ж��Ƿ����ͼ��
            if (this.cboLayer.Items.Count <= 0)
            {
                MessageBox.Show("��ǰMapControlû�����ͼ�㣡","��ʾ");
                return;
            }
            MainForm.layerIndex = this.cboLayer.SelectedIndex;
            //��ȡѡ�еĲ�ѯ��ʽ��ͼ������
            this.mLayerIndex = this.cboLayer.SelectedIndex;
            this.mQueryMode = this.cboMode.SelectedIndex;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


    }
}