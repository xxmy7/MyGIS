using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

namespace MyGIS.Forms
{
    public partial class ConstructTINForm : Form
    {
        //地图数据
        private AxSceneControl mSceneControl;

        public ConstructTINForm(AxSceneControl sceneControl)
        {
            InitializeComponent();
            this.mSceneControl = sceneControl;
            this.cboTINType.SelectedIndex = 0;
        }

        private void ConstructTINForm_Load(object sender, EventArgs e)
        {
            this.mSceneControl.BringToFront();

            cboLayer.Items.Clear();
            //得到当前场景中所有图层
            int nCount = mSceneControl.Scene.LayerCount;
            if (nCount <= 0)//没有图层的情况
            {
                MessageBox.Show("场景中没有图层，请加入图层");
                return;
            }
            int i;
            ILayer pLayer = null;
            //将所有的图层的名称显示到复选框中
            for (i = 0; i < nCount; i++)
            {
                pLayer = mSceneControl.Scene.get_Layer(i);
                cboLayer.Items.Add(pLayer.Name);
            }
            //将复选框设置为选中第一项
            cboLayer.SelectedIndex = 0;
            addFieldNameToCombox(cboLayer.Items[cboLayer.SelectedIndex].ToString());
        }
        
        /// <summary>
        /// 更加图层的名字将该图层的字段加入到combox中
        /// </summary>
        /// <param name="layerName"></param>
        private void addFieldNameToCombox(string layerName)
        {
            cboField.Items.Clear();
            int i;
            IFeatureLayer pFeatureLayer = null;
            IFields pField = null;
            int nCount = mSceneControl.Scene.LayerCount;
            ILayer pLayer = null;
            //寻找名称为layerName的FeatureLayer;
            for (i = 0; i < nCount; i++)
            {
                pLayer = mSceneControl.Scene.get_Layer(i) as IFeatureLayer;
                if (pLayer.Name == layerName)//找到了layerName的Featurelayer
                {
                    pFeatureLayer = pLayer as IFeatureLayer;
                    break;
                }
            }
            if (pFeatureLayer != null)//判断是否找到
            {
                pField = pFeatureLayer.FeatureClass.Fields;
                nCount = pField.FieldCount;
                //将该图层中所用的字段写入到mFeildCombox中去
                for (i = 0; i < nCount; i++)
                {
                    cboField.Items.Add(pField.get_Field(i).Name);
                }
            }
            cboField.SelectedIndex = 0;
        }

        private void btnConstructTIN_Click(object sender, EventArgs e)
        {
            if (cboLayer.Text == "" || cboField.Text == "")//判断输入合法性
            {
                MessageBox.Show("没有相应的图层");
                return;
            }
            ITinEdit pTin = new TinClass();
            //寻找Featurelayer
            IFeatureLayer pFeatureLayer =
                mSceneControl.Scene.get_Layer(cboLayer.SelectedIndex) as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                IField pField = null;
                //找字段
                pField = pFeatureClass.Fields.get_Field(pFeatureClass.Fields.FindField(cboField.Text));
                if (pField.Type == esriFieldType.esriFieldTypeInteger ||
                     pField.Type == esriFieldType.esriFieldTypeDouble ||
                     pField.Type == esriFieldType.esriFieldTypeSingle)//判断类型
                {
                    IGeoDataset pGeoDataset = pFeatureLayer as IGeoDataset;
                    pEnvelope = pGeoDataset.Extent;
                    //设置空间参考系
                    ISpatialReference pSpatialReference;
                    pSpatialReference = pGeoDataset.SpatialReference;
                    //选择生成TIN的输入类型
                    esriTinSurfaceType pSurfaceTypeCount = esriTinSurfaceType.esriTinMassPoint;
                    switch (cboTINType.Text)
                    {
                        case "点":
                            pSurfaceTypeCount = esriTinSurfaceType.esriTinMassPoint;
                            break;
                        case "直线":
                            pSurfaceTypeCount = esriTinSurfaceType.esriTinSoftLine;
                            break;
                        case "光滑线":
                            pSurfaceTypeCount = esriTinSurfaceType.esriTinHardLine;
                            break;
                    }
                    //创建TIN
                    pTin.InitNew(pEnvelope);
                    object missing = Type.Missing;
                    //生成TIN
                    pTin.AddFromFeatureClass(pFeatureClass, pQueryFilter, pField, pField, pSurfaceTypeCount, ref missing);
                    pTin.SetSpatialReference(pGeoDataset.SpatialReference);
                    //创建Tin图层并将Tin图层加入到场景中去
                    ITinLayer pTinLayer = new TinLayerClass();
                    pTinLayer.Dataset = pTin as ITin;
                    mSceneControl.Scene.AddLayer(pTinLayer, true);
                }
                else
                {
                    MessageBox.Show("该字段的类型不符合构建TIN的条件");
                }
            }
        }
    

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cboLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            addFieldNameToCombox(cboLayer.Items[cboLayer.SelectedIndex].ToString());
        }
    }
}
