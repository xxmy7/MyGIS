using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace MyGIS.Forms
{
    public partial class AttributeQueryForm : Form
    {
        //��ͼ����
        private AxMapControl mMapControl;
        //ѡ��ͼ��
        private IFeatureLayer mFeatureLayer;

        //�޸Ĺ��캯������ȡMapControl
        public AttributeQueryForm(AxMapControl mapControl)
        {
            InitializeComponent();
            this.mMapControl = mapControl;
        }

        private void AttributeQueryForm_Load(object sender, EventArgs e)
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
                //ͼ�����Ƽ���cboLayer
                this.cboLayer.Items.Add(strLayerName);
            }
            //Ĭ����ʾ��һ��ѡ��
            this.cboLayer.SelectedIndex = 0;
        }

        private void cboLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //��ȡcboLayer��ѡ�е�ͼ��
            mFeatureLayer = mMapControl.get_Layer(cboLayer.SelectedIndex) as IFeatureLayer;
            IFeatureClass pFeatureClass = mFeatureLayer.FeatureClass;
            //�ֶ�����
            string strFldName;
            for (int i = 0; i < pFeatureClass.Fields.FieldCount;i++ )
            {
                strFldName = pFeatureClass.Fields.get_Field(i).Name;
                //ͼ�����Ƽ���cboField
                this.cboField.Items.Add(strFldName);
            }
            //Ĭ����ʾ��һ��ѡ��
            this.cboField.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //��������Ϊ��ʱ����
            if (txtValue.Text == null)
                return;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            IFeatureCursor pFeatureCursor;
            IFeature pFeature;
            pQueryFilter.WhereClause = cboField.Text + "='" + txtValue.Text + "'";
            pFeatureCursor = mFeatureLayer.Search(pQueryFilter, true);
            pFeature = pFeatureCursor.NextFeature();

            if (pFeature == null)
            {
                //û�еõ�pFeature����ʾ
                MessageBox.Show("û���ҵ�" + txtValue.Text + "��", "��ʾ");
            }

            //�ж��Ƿ��ȡ��Ҫ��
            while (pFeature != null)
            {
                mMapControl.Refresh();
                //��̬��������ѡ�е�ͼ��index
                MainForm.layerIndex = this.cboLayer.SelectedIndex;

                //��˸3�Σ����300ms
                mMapControl.FlashShape(pFeature.Shape, 3, 300, null);

                //����ѯ����ӵ����ݼ���
                mMapControl.Map.SelectFeature(mFeatureLayer, pFeature);

                pFeature = pFeatureCursor.NextFeature();

                ////�Ŵ�Ҫ��
                //mMapControl.Extent = pFeature.Shape.Envelope;
                
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}