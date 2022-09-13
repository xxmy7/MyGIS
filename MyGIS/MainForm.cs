using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.DisplayUI;

using MyGIS.Forms;
using MyGIS.Classes;

namespace MyGIS
{

    //地图编辑方式类型枚举
    public enum editOperationType
    {
        //无操作
        None,
        //添加对象
        Create,
        //移动对象
        Move,
        //删除对象
        Delete,
    }

    public partial class MainForm : Form
    {
        //标记当前操作方式
        private string mTool;
        //查询方式
        private int mQueryMode;
        //图层索引
        private int mLayerIndex;

        //放大、缩小，漫游的basetool
        private ZoomIn mZoomIn;
        private ZoomOut mZoomOut;
        private Pan mPan;

        //当前查询的图层
        public static int layerIndex = 0;


        #region 网络分析成员变量
        //几何网络
        private IGeometricNetwork mGeometricNetwork;
        //给定点的集合
        private IPointCollection mPointCollection;
        //获取给定点最近的Network元素
        private IPointToEID mPointToEID;

        //返回结果变量
        private IEnumNetEID mEnumNetEID_Junctions;
        private IEnumNetEID mEnumNetEID_Edges;
        private double mdblPathCost;
        #endregion

        //地图编辑
        private Edit mEdit;
        editOperationType mEditOperationType = editOperationType.None; //当前的地图编辑方式

        //三维分析
        private bool is3DPointQuery = false;

        /*****************==================================================******************/
        public MainForm()
        {
            InitializeComponent();
        }

        /****** 框架初始化加载事件 ******/
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.axTOCControl1.SetBuddyControl(axMapControl1.Object);
            this.axToolbarControl1.SetBuddyControl(axMapControl1.Object);

            //将地图编辑的选择操作类型、保存编辑结果、结束编辑菜单设置为不可选
            this.menuSelectEditingOperation.Enabled = false;
            this.menuSaveEditingResult.Enabled = false;
            this.menuFinishEditing.Enabled = false;
            //将工具栏上的选择编辑图层设置为不可选
            this.selectLayerToolStripComboBox.Enabled = false;

            //将三维点查询状态处于关闭状态
            is3DPointQuery = false;

        }

        #region mapcontrol的鼠标事件
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            this.axMapControl1.Map.ClearSelection();
            //获取当前视图
            IActiveView pActiveView = this.axMapControl1.ActiveView;
            //获取鼠标点
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            switch (mTool)
            {
                case "ZoomIn":
                    this.mZoomIn.OnMouseDown(e.button, e.shift, e.x, e.y);
                    break;
                case "ZoomOut":
                    this.mZoomOut.OnMouseDown(e.button, e.shift, e.x, e.y);
                    break;
                case "Pan":
                    //设置鼠标形状
                    this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPanning;
                    this.mPan.OnMouseDown(e.button, e.shift, e.x, e.y);
                    break;
                case "SpaceQuery":
                    IGeometry pGeometry = null;
                    if (this.mQueryMode == 0)//矩形查询
                    {
                        pGeometry = this.axMapControl1.TrackRectangle();
                    }
                    else if (this.mQueryMode == 1)//线查询
                    {
                        pGeometry = this.axMapControl1.TrackLine();
                    }
                    else if (this.mQueryMode == 2)//点查询
                    {
                        ITopologicalOperator pTopo;
                        IGeometry pBuffer;
                        pGeometry = pPoint;
                        pTopo = pGeometry as ITopologicalOperator;
                        //根据点位创建缓冲区，缓冲半径为0.1，可修改
                        pBuffer = pTopo.Buffer(0.1);
                        pGeometry = pBuffer.Envelope;
                    }
                    else if (this.mQueryMode == 3)//圆查询
                    {
                        pGeometry = this.axMapControl1.TrackCircle();
                    }
                    else if(this.mQueryMode == 4)//多边形查询
                    {
                        pGeometry = axMapControl1.TrackPolygon();
                    }else if(this.mQueryMode == 5) //折线查询
                    {
                        pGeometry = axMapControl1.TrackLine();
                    }
                    //设置查询图层
                    IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(this.mLayerIndex) as IFeatureLayer;

                    DataTable pDataTable = this.LoadQueryResult(this.axMapControl1, pFeatureLayer, pGeometry);
                    //弹窗显示多少个查询结果
                    double index = axMapControl1.Map.SelectionCount;
                    index = Math.Round(index, 0);//小数点后指定为０位数字
                    MessageBox.Show("当前共有" + index.ToString() + "个查询结果");

                    this.dataGridView1.DataSource = pDataTable.DefaultView;
                    this.dataGridView1.Refresh();
                    break;
                case "Network":
                    //记录鼠标点击的点
                    IPoint pNewPoint = new PointClass();
                    pNewPoint.PutCoords(e.mapX, e.mapY);

                    if (mPointCollection == null)
                        mPointCollection = new MultipointClass();
                    //添加点，before和after标记添加点的索引，这里不定义
                    object before = Type.Missing;
                    object after = Type.Missing;
                    mPointCollection.AddPoint(pNewPoint, ref before, ref after);
                    break;
                case "Edit":
                    //判断是否鼠标左键
                    if (e.button != 1)
                        return;
                    if (mEdit == null)
                        return;
                    //判断是否处于编辑状态
                    if (mEdit.IsEditing())
                    {
                        switch (mEditOperationType)
                        {
                            case editOperationType.Create:
                                mEdit.CreateMouseDown(e.mapX, e.mapY);
                                break;
                            case editOperationType.Move:
                                mEdit.PanMouseDown(e.mapX, e.mapY);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            switch (mTool)
            {
                case "ZoomIn":
                    this.mZoomIn.OnMouseMove(e.button, e.shift, e.x, e.y);
                    break;
                case "ZoomOut":
                    this.mZoomOut.OnMouseMove(e.button, e.shift, e.x, e.y);
                    break;
                case "Pan":
                    this.mPan.OnMouseMove(e.button, e.shift, e.x, e.y);
                    break;
                case "Edit":
                    if (mEdit == null)
                        return;
                    //判断是否处于编辑状态
                    if (mEdit.IsEditing())
                    {
                        switch (mEditOperationType)
                        {
                            case editOperationType.Create:
                            case editOperationType.Move:
                                mEdit.MouseMove(e.mapX, e.mapY);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            // 显示当前比例尺
            this.statusScale.Text = " 比例尺 1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标
            this.statusCoordinate.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits;
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            switch (mTool)
            {
                case "ZoomIn":
                    this.mZoomIn.OnMouseUp(e.button, e.shift, e.x, e.y);
                    break;
                case "ZoomOut":
                    this.mZoomOut.OnMouseUp(e.button, e.shift, e.x, e.y);
                    break;
                case "Pan":
                    this.mPan.OnMouseUp(e.button, e.shift, e.x, e.y);
                    //设置鼠标形状
                    this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;
                    break;
                case "Edit":
                    if (mEdit == null)
                        return;
                    //判断是否鼠标左键
                    if (e.button != 1)
                        return;
                    //判断是否处于编辑状态
                    if (mEdit.IsEditing())
                    {
                        switch (mEditOperationType)
                        {
                            case editOperationType.Create:
                                break;
                            case editOperationType.Move:
                                mEdit.PanMouseUp(e.mapX, e.mapY);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if(string.Equals(mTool, "Edit"))
            {
                //判断是否鼠标左键
                if (e.button != 1)
                    return;
                if(mEdit == null)
                {
                    return;
                }
                //判断是否处于编辑状态
                if (mEdit.IsEditing())
                {
                    switch (mEditOperationType)
                    {
                        case editOperationType.Create:
                            mEdit.CreateDoubleClick(e.mapX, e.mapY);
                            break;
                        case editOperationType.Move:
                            break;
                    }
                }
            }
            else if(string.Equals(mTool, "Network"))
            {
                try
                {
                    //路径计算
                    //注意权重名称与设置保持一致
                    SolvePath("LENGTH");
                    //路径转换为几何要素
                    IPolyline pPolyLineResult = PathToPolyLine();
                    //获取屏幕显示
                    IActiveView pActiveView = this.axMapControl1.ActiveView;
                    IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
                    //设置显示符号
                    ILineSymbol pLineSymbol = new CartographicLineSymbolClass();
                    IRgbColor pColor = new RgbColorClass();
                    pColor.Red = 255;
                    pColor.Green = 0;
                    pColor.Blue = 0;
                    //设置线宽
                    pLineSymbol.Width = 4;
                    //设置颜色
                    pLineSymbol.Color = pColor as IColor;
                    //绘制线型符号
                    pScreenDisplay.StartDrawing(0, 0);
                    pScreenDisplay.SetSymbol((ISymbol)pLineSymbol);
                    pScreenDisplay.DrawPolyline(pPolyLineResult);
                    pScreenDisplay.FinishDrawing();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("路径分析出现错误:" + "\r\n" + ex.Message);
                }
                //点集设为空
                mPointCollection = null;
            }
            else
            {

            }
        }

        #endregion

        #region 菜单 -> 文件
        /// <summary>
        /// 点击菜单文件->打开mxd、打开mxd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpenMxd_Click(object sender, EventArgs e)
        {
            //文件路径名称,包含文件名称和路径名称
            string strName = null;

            //定义OpenFileDialog，获取并打开地图文档
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开MXD";
            openFileDialog.Filter = "MXD文件（*.mxd）|*.mxd";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                strName = openFileDialog.FileName;
                if (strName != "")
                {
                    this.axMapControl1.LoadMxFile(strName);
                    //IMapDocument pm = new MapDocument();
                    //pm.Open(strName, "");
                    //axMapControl1.Map = pm.ActiveView.FocusMap;
                    //axMapControl1.ActiveView.Refresh();
                }
            }
            //地图文档全图显示
            this.axMapControl1.Extent = this.axMapControl1.FullExtent;
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAddData_Click(object sender, EventArgs e)
        {
            ICommand cmd = new ControlsAddDataCommandClass();
            cmd.OnCreate(this.axMapControl1.Object);
            cmd.OnClick();
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        /// <summary>
        /// 清楚mapcontrol的图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClearLayer_Click(object sender, EventArgs e)
        {
            //将当前所有操作清除，鼠标形状恢复成箭头
            axMapControl1.CurrentTool = null;
            mTool = "None";
            axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrow;
            
            this.dataGridView1.DataSource = null;
            this.dataGridView1.Refresh();
            axMapControl1.Map.ClearLayers();
            axMapControl2.Map.ClearLayers();
            axMapControl1.Refresh();
            axMapControl2.Refresh();
            axTOCControl1.Update();
        }


        #endregion

        #region 菜单 -> 视图
        /// <summary>
        /// 菜单居中放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFixedZoomIn_Click(object sender, EventArgs e)
        {
            FixedZoomIn fixedZoomIn = new FixedZoomIn();
            fixedZoomIn.OnCreate(this.axMapControl1.Object);
            fixedZoomIn.OnClick();
        }

        /// <summary>
        /// 菜单居中缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFixedZoomOut_Click(object sender, EventArgs e)
        {
            FixedZoomOut fixedZoomOut = new FixedZoomOut();
            fixedZoomOut.OnCreate(this.axMapControl1.Object);
            fixedZoomOut.OnClick();
        }

        /// <summary>
        /// 菜单全图显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFullExtent_Click(object sender, EventArgs e)
        {
            //初始化FullExtent对象
            FullExtent fullExtent = new FullExtent();
            //FullExtent对象与MapControl关联
            fullExtent.OnCreate(this.axMapControl1.Object);
            fullExtent.OnClick();
        }

        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            mZoomIn = new ZoomIn();
            mZoomIn.OnCreate(this.axMapControl1.Object);
            //设置鼠标形状
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomIn;
            //标记操作为“ZoomIn”
            this.mTool = "ZoomIn";
        }


        private void menuZoomOut_Click(object sender, EventArgs e)
        {
            mZoomOut = new ZoomOut();
            mZoomOut.OnCreate(this.axMapControl1.Object);
            //设置鼠标形状
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomOut;
            //标记操作为“ZoomOut”
            this.mTool = "ZoomOut";
        }

        private void menuPan_Click(object sender, EventArgs e)
        {
            //初始化Pan对象
            mPan = new Pan();
            //Pan对象与MapControl关联
            mPan.OnCreate(this.axMapControl1.Object);
            //设置鼠标形状
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;
            //标记操作为“Pan”
            this.mTool = "Pan";
        }

        /// <summary>
        /// 重置mTool以及鼠标形状
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuResetArrow_Click(object sender, EventArgs e)
        {
            //将当前所有操作清除，鼠标形状恢复成箭头
            axMapControl1.CurrentTool = null;
            mTool = "None";
            axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrow;
        }
        #endregion

        #region 菜单 -> 查询
        /// <summary>
        /// 获取空间查询得到的要素的属性
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        /// <param name="featureLayer">图层</param>
        /// <param name="geometry">查询空间几何形状</param>
        /// <returns>属性表</returns>
        private DataTable LoadQueryResult(AxMapControl mapControl, IFeatureLayer featureLayer, IGeometry geometry)
        {
            IFeatureClass pFeatureClass = featureLayer.FeatureClass;

            //根据图层属性字段初始化DataTable
            IFields pFields = pFeatureClass.Fields;
            DataTable pDataTable = new DataTable();
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                string strFldName;
                strFldName = pFields.get_Field(i).AliasName;
                pDataTable.Columns.Add(strFldName);
            }


            //空间过滤器
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = geometry;

            //根据图层类型选择缓冲方式
            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }
            //定义空间过滤器的空间字段
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;

            IQueryFilter pQueryFilter;
            IFeatureCursor pFeatureCursor;
            IFeature pFeature;
            //利用要素过滤器查询要素
            pQueryFilter = pSpatialFilter as IQueryFilter;
            pFeatureCursor = featureLayer.Search(pQueryFilter, true);
            pFeature = pFeatureCursor.NextFeature();

            while (pFeature != null)
            {
                string strFldValue = null;
                DataRow dr = pDataTable.NewRow();
                //遍历图层属性表字段值，并加入pDataTable
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string strFldName = pFields.get_Field(i).Name;
                    if (strFldName == "Shape")
                    {
                        strFldValue = Convert.ToString(pFeature.Shape.GeometryType);
                    }
                    else
                        strFldValue = Convert.ToString(pFeature.get_Value(i));
                    dr[i] = strFldValue;
                }
                pDataTable.Rows.Add(dr);
                //高亮选择要素
                mapControl.Map.SelectFeature((ILayer)featureLayer, pFeature);
                mapControl.ActiveView.Refresh();
                pFeature = pFeatureCursor.NextFeature();
            }
            return pDataTable;
        }
        
        /// <summary>
        /// 删除查询结果的高亮显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClearQuery_Click(object sender, EventArgs e)
        {
            //获得axMapControl1的ActiveView
            //map和activeview实际上都是代表同一个东西，就是地图，activeview针对地图的显示，例如地图显示范围属性extent就在activeview
            IActiveView pActiveView = axMapControl1.ActiveView;
            //绑定图层
            IFeatureLayer pFeaturelayer = axMapControl1.get_Layer(layerIndex) as IFeatureLayer;
            if (pActiveView == null || pFeaturelayer == null) { return; }

            //将原图层转换成IFeatureSelection，包含当前显示的所有查询结果
            IFeatureSelection featureSelection = pFeaturelayer as IFeatureSelection;
            //重绘地图中的部分内容，esriViewGeoSelection是函数参数，意思为刷新图层中的选中要素
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            //清除所有选择
            featureSelection.Clear();
            //dataGrid清空
            this.dataGridView1.DataSource = null;
            this.dataGridView1.Refresh();

            axMapControl1.Map.ClearSelection();
            axMapControl1.Refresh();
        }

        /// <summary>
        /// 属性查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAttributeQuery_Click(object sender, EventArgs e)
        {
            //初始化属性查询窗体
            //AttributeQueryForm attributeQueryForm = new AttributeQueryForm(this.axMapControl1);
            SelectByAttrFrm attributeQueryForm = new SelectByAttrFrm(this.axMapControl1,this.dataGridView1);
            attributeQueryForm.Show();
        }

        /// <summary>
        /// 空间查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSpatialQuery_Click(object sender, EventArgs e)
        {
            //初始化空间查询窗体
            SpatialQueryForm spatialQueryForm = new SpatialQueryForm(this.axMapControl1);
            if (spatialQueryForm.ShowDialog() == DialogResult.OK)
            {
                //标记为“空间查询”
                this.mTool = "SpaceQuery";
                //获取查询方式和图层
                this.mQueryMode = spatialQueryForm.mQueryMode;
                this.mLayerIndex = spatialQueryForm.mLayerIndex;
                MainForm.layerIndex = this.mLayerIndex;
                //定义鼠标形状
                this.axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerCrosshair;
            }
        }

        #endregion

        #region 鹰眼相关
        /// <summary>
        /// 首先在axMapControl1中视图范围改变时鹰眼窗体要做出对应的响应，
        /// 即绘制线框并显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //创建鹰眼中线框
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            //获取矩形坐标
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;

            //设置线框的边线对象，包括颜色和线宽
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;

            // 产生一个矩形线框对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;

            // 设置颜色属性 
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;

            // 设置线框填充符号的属性 
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;

            //构建矩形元素
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;

            // 得到鹰眼视图中的图形元素容器
            IGraphicsContainer pGra = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素 
            pGra.DeleteAllElements();
            
            // 鹰眼视图中添加线框
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新鹰眼
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 当鼠标点击鹰眼窗体时，主窗体Extent随之改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框 
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框 
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        /// <summary>
        /// 当鼠标左键按下且在鹰眼窗体移动时，主窗体Extent随之改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 如果不是左键按下就直接返回 
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);

            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }


        /// <summary>
        /// 获取主视图范围最大的图层作为鹰眼视图
        /// </summary>
        /// <param name="map">地图</param>
        /// <returns></returns>
        private ILayer GetOverviewLayer(IMap map)
        {
            //获取主视图的第一个图层
            ILayer pLayer = map.get_Layer(0);
            //遍历其他图层，并比较视图范围的宽度，返回宽度最大的图层
            ILayer pTempLayer = null;
            for (int i = 1; i < map.LayerCount; i++)
            {
                pTempLayer = map.get_Layer(i);
                if (pLayer.AreaOfInterest.Width < pTempLayer.AreaOfInterest.Width)
                    pLayer = pTempLayer;
            }
            return pLayer;
        }

        /// <summary>
        /// 对axMapControl1添加地图文档（mxd文件）的响应,同步更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            ////获取鹰眼图层
            //this.axMapControl2.AddLayer(this.GetOverviewLayer(this.axMapControl1.Map));
            //// 设置 MapControl 显示范围至数据的全局范围
            //this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            //// 刷新鹰眼控件地图
            //this.axMapControl2.Refresh();
            axMapControl2.Map = new MapClass();
            if (axMapControl1.LayerCount > 0)
            {
                for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; ++i)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.FullExtent;
            }
        }

        /// <summary>
        /// 对axMapControl1添加单个图层的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnFullExtentUpdated(object sender, IMapControlEvents2_OnFullExtentUpdatedEvent e)
        {
            //获取鹰眼图层
            if (axMapControl1.LayerCount == 0) return;
            this.axMapControl2.AddLayer(this.GetOverviewLayer(this.axMapControl1.Map));
            // 设置 MapControl 显示范围至数据的全局范围
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh();

            // 以下也可以用
            //axMapControl2.Map = new MapClass();
            //if (axMapControl1.LayerCount > 0)
            //{
            //    for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; ++i)
            //    {
            //        axMapControl2.AddLayer(axMapControl1.get_Layer(i));
            //    }
            //    axMapControl2.Extent = axMapControl1.FullExtent;
            //}
        }


        #endregion

        #region 菜单 -> 空间分析
        /// <summary>
        /// 缓冲区分析menu响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuBuffer_Click(object sender, EventArgs e)
        {
            this.mTool = "Buffer";
            axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrow;
            BufferForm bufferForm = new BufferForm(this.axMapControl1.Object);
            if (bufferForm.ShowDialog() == DialogResult.OK)
            {
                //获取输出文件路径
                string strBufferPath = bufferForm.strOutputPath;
                //缓冲区图层载入到MapControl
                int index = strBufferPath.LastIndexOf("\\");
                this.axMapControl1.AddShapeFile(strBufferPath.Substring(0, index), strBufferPath.Substring(index));
            }
        }
        
        /// <summary>
        /// 叠置分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOverlay_Click(object sender, EventArgs e)
        {
            this.mTool = "Overlay";
            axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrow;
            OverlayForm overlayForm = new OverlayForm();
            if (overlayForm.ShowDialog() == DialogResult.OK)
            {
                string strOverlayPath = overlayForm.strOutputPath;
                //int index = strOverlayPath.LastIndexOf("\\");
                //this.axMapControl1.AddShapeFile(strOverlayPath.Substring(0, index), strOverlayPath.Substring(index));
                string path = System.IO.Path.GetDirectoryName(strOverlayPath);//路径
                string fileName = System.IO.Path.GetFileName(strOverlayPath);//文件名
                this.axMapControl1.AddShapeFile(path, fileName);
            }
        }

        #region 网络分析自定义函数
        /// <summary>
        /// 路径计算
        /// </summary>
        /// <param name="weightName">权重名称</param>
        private void SolvePath(string weightName)
        {
            //创建ITraceFlowSolverGEN
            ITraceFlowSolverGEN pTraceFlowSolverGEN = new TraceFlowSolverClass();
            INetSolver pNetSolver = pTraceFlowSolverGEN as INetSolver;
            //初始化用于路径计算的Network
            INetwork pNetWork = mGeometricNetwork.Network;
            pNetSolver.SourceNetwork = pNetWork;

            //获取分析经过的点的个数
            int intCount = mPointCollection.PointCount;
            if (intCount < 1)
                return;


            INetFlag pNetFlag;
            //用于存储路径计算得到的边
            IEdgeFlag[] pEdgeFlags = new IEdgeFlag[intCount];


            IPoint pEdgePoint = new PointClass();
            int intEdgeEID;
            IPoint pFoundEdgePoint;
            double dblEdgePercent;

            //用于获取几何网络元素的UserID, UserClassID,UserSubID
            INetElements pNetElements = pNetWork as INetElements;
            int intEdgeUserClassID;
            int intEdgeUserID;
            int intEdgeUserSubID;
            for (int i = 0; i < intCount; i++)
            {
                pNetFlag = new EdgeFlagClass();
                //获取用户点击点
                pEdgePoint = mPointCollection.get_Point(i);
                //获取距离用户点击点最近的边
                mPointToEID.GetNearestEdge(pEdgePoint, out intEdgeEID, out pFoundEdgePoint, out dblEdgePercent);
                if (intEdgeEID <= 0)
                    continue;
                //根据得到的边查询对应的几何网络中的元素UserID, UserClassID,UserSubID
                pNetElements.QueryIDs(intEdgeEID, esriElementType.esriETEdge,
                    out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
                if (intEdgeUserClassID <= 0 || intEdgeUserID <= 0)
                    continue;

                pNetFlag.UserClassID = intEdgeUserClassID;
                pNetFlag.UserID = intEdgeUserID;
                pNetFlag.UserSubID = intEdgeUserSubID;
                pEdgeFlags[i] = pNetFlag as IEdgeFlag;
            }
            //设置路径求解的边
            pTraceFlowSolverGEN.PutEdgeOrigins(ref pEdgeFlags);

            //路径计算权重
            INetSchema pNetSchema = pNetWork as INetSchema;
            INetWeight pNetWeight = pNetSchema.get_WeightByName(weightName);
            if (pNetWeight == null)
                return;

            //设置权重，这里双向的权重设为一致
            INetSolverWeights pNetSolverWeights = pTraceFlowSolverGEN as INetSolverWeights;
            pNetSolverWeights.ToFromEdgeWeight = pNetWeight;
            pNetSolverWeights.FromToEdgeWeight = pNetWeight;

            object[] arrResults = new object[intCount - 1];
            //执行路径计算
            pTraceFlowSolverGEN.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum,
                out mEnumNetEID_Junctions, out mEnumNetEID_Edges, intCount - 1, ref arrResults);

            //获取路径计算总代价（cost）
            mdblPathCost = 0;
            for (int i = 0; i < intCount - 1; i++)
                mdblPathCost += (double)arrResults[i];
        }

        /// <summary>
        /// 路径转换为几何要素
        /// </summary>
        /// <returns></returns>
        private IPolyline PathToPolyLine()
        {
            IPolyline pPolyLine = new PolylineClass();
            IGeometryCollection pNewGeometryCollection = pPolyLine as IGeometryCollection;
            if (mEnumNetEID_Edges == null)
                return null;

            IEIDHelper pEIDHelper = new EIDHelperClass();
            //获取几何网络
            pEIDHelper.GeometricNetwork = mGeometricNetwork;
            //获取地图空间参考
            ISpatialReference pSpatialReference = this.axMapControl1.Map.SpatialReference;
            pEIDHelper.OutputSpatialReference = pSpatialReference;
            pEIDHelper.ReturnGeometries = true;
            //根据边的ID获取边的信息
            IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(mEnumNetEID_Edges);
            int intCount = pEnumEIDInfo.Count;
            pEnumEIDInfo.Reset();

            IEIDInfo pEIDInfo;
            IGeometry pGeometry;
            for (int i = 0; i < intCount; i++)
            {
                pEIDInfo = pEnumEIDInfo.Next();
                //获取边的几何要素
                pGeometry = pEIDInfo.Geometry;
                pNewGeometryCollection.AddGeometryCollection((IGeometryCollection)pGeometry);
            }
            return pPolyLine;
        }
        #endregion
        
        /// <summary>
        /// 网络分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNetwork_Click(object sender, EventArgs e)
        {
            //修改当前工具
            this.mTool = "Network";
            //获取几何网络文件路径
            //注意修改此路径为当前存储路径
            string strPath = @"F:\GIS综合实习\例子数据\Network\USA_Highway_Network_GDB.mdb";
            //打开工作空间
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactory();
            IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(strPath, 0) as IFeatureWorkspace;
            //获取要素数据集
            //注意名称的设置要与上面创建保持一致
            IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset("high");

            //获取network集合
            INetworkCollection pNetWorkCollection = pFeatureDataset as INetworkCollection;
            //获取network的数量,为零时返回
            int intNetworkCount = pNetWorkCollection.GeometricNetworkCount;
            if (intNetworkCount < 1)
                return;
            //FeatureDataset可能包含多个network，我们获取指定的network
            //注意network的名称的设置要与上面创建保持一致
            mGeometricNetwork = pNetWorkCollection.get_GeometricNetworkByName("high_net");

            //将Network中的每个要素类作为一个图层加入地图控件
            IFeatureClassContainer pFeatClsContainer = mGeometricNetwork as IFeatureClassContainer;
            //获取要素类数量，为零时返回
            int intFeatClsCount = pFeatClsContainer.ClassCount;
            if (intFeatClsCount < 1)
                return;
            IFeatureClass pFeatureClass;
            IFeatureLayer pFeatureLayer;
            for (int i = 0; i < intFeatClsCount; i++)
            {
                //获取要素类
                pFeatureClass = pFeatClsContainer.get_Class(i);
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pFeatureClass.AliasName;
                //加入地图控件
                this.axMapControl1.AddLayer((ILayer)pFeatureLayer, 0);
            }

            //计算snap tolerance为图层最大宽度的1/100
            //获取图层数量
            int intLayerCount = this.axMapControl1.LayerCount;
            IGeoDataset pGeoDataset;
            IEnvelope pMaxEnvelope = new EnvelopeClass();
            for (int i = 0; i < intLayerCount; i++)
            {
                //获取图层
                pFeatureLayer = this.axMapControl1.get_Layer(i) as IFeatureLayer;
                pGeoDataset = pFeatureLayer as IGeoDataset;
                //通过Union获得较大图层范围
                pMaxEnvelope.Union(pGeoDataset.Extent);
            }
            double dblWidth = pMaxEnvelope.Width;
            double dblHeight = pMaxEnvelope.Height;
            double dblSnapTol;
            if (dblHeight < dblWidth)
                dblSnapTol = dblWidth * 0.01;
            else
                dblSnapTol = dblHeight * 0.01;

            //设置源地图，几何网络以及捕捉容差
            mPointToEID = new PointToEIDClass();
            mPointToEID.SourceMap = this.axMapControl1.Map;
            mPointToEID.GeometricNetwork = mGeometricNetwork;
            mPointToEID.SnapTolerance = dblSnapTol;
        }

        #endregion

        #region 菜单 -> 地图编辑
        /// <summary>
        /// 地图编辑菜单下的开始编辑响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStartEditing_Click(object sender, EventArgs e)
        {
            //判断是否存在可编辑图层
            //没有添加图层时返回
            if (this.axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("MapControl中未添加图层！", "提示");
                return;
            }
            //编辑图层默认为操作图层下拉框（queryIndexComboBox1）所选择的图层，图层序号为：layerIndex

            //将地图功能切换成地图编辑
            this.mTool = "Edit";

            //鼠标形状变成箭头
            axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrow;
            
            //更新工具栏上的选择Layer的checkbox，加载图层
            selectLayerToolStripComboBox.Enabled = true;
            //清空原有选项
            selectLayerToolStripComboBox.Items.Clear();


            for (int i = 0; i < this.axMapControl1.Map.LayerCount; i++)
            {
                ILayer pLayer = this.axMapControl1.get_Layer(i);
                this.selectLayerToolStripComboBox.Items.Add(pLayer.Name);
            }
            this.selectLayerToolStripComboBox.SelectedIndex = 0;
            this.axMapControl1.Refresh();

            //开始编辑设为不可用，将其他编辑菜单项设为可用
            this.menuStartEditing.Enabled = false;
            this.menuSelectEditingOperation.Enabled = true;
            this.menuSaveEditingResult.Enabled = true;
            this.menuFinishEditing.Enabled = true;
        }

        private void menuSaveEditingResult_Click(object sender, EventArgs e)
        {
            //判断编辑是否初始化
            if (mEdit == null)
                return;
            //处于编辑状态且已编辑则保存
            if (mEdit.IsEditing() && mEdit.HasEdited())
            {
                mEdit.SaveEditing(true);

                this.menuStartEditing.Enabled = true;
                this.menuSelectEditingOperation.Enabled = false;
                this.menuSaveEditingResult.Enabled = false;
                this.menuFinishEditing.Enabled = false;
                //将工具栏上的选择编辑图层设置
                this.selectLayerToolStripComboBox.Items.Clear();
                this.selectLayerToolStripComboBox.Enabled = false;

                this.axMapControl1.Refresh();
            }
        }

        private void menuFinishEditing_Click(object sender, EventArgs e)
        {
            if (mEdit == null)
            {
                this.menuStartEditing.Enabled = true;
                this.menuSelectEditingOperation.Enabled = false;
                this.menuSaveEditingResult.Enabled = false;
                this.menuFinishEditing.Enabled = false;
                //将工具栏上的选择编辑图层设置
                this.selectLayerToolStripComboBox.Items.Clear();
                this.selectLayerToolStripComboBox.Enabled = false;
                this.mTool = "None";

                return;
            }
            if (mEdit.HasEdited())
            {
                DialogResult dr = MessageBox.Show("图层已编辑，是否保存？", "提示", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                    mEdit.StopEditing(true);
                else
                    mEdit.StopEditing(false);

                this.menuStartEditing.Enabled = true;
                this.menuSelectEditingOperation.Enabled = false;
                this.menuSaveEditingResult.Enabled = false;
                this.menuFinishEditing.Enabled = false;
                //将工具栏上的选择编辑图层设置
                this.selectLayerToolStripComboBox.Items.Clear();
                this.selectLayerToolStripComboBox.Enabled = false;

                this.axMapControl1.Refresh();
            }
            this.mTool = "None";
            this.mEdit = null;
        }

        private void menuCreateFeatures_Click(object sender, EventArgs e)
        {
            mEditOperationType = editOperationType.Create;
            this.mLayerIndex = this.selectLayerToolStripComboBox.SelectedIndex;

            //获取地图和编辑图层
            IMap pMap = this.axMapControl1.Map;
            IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(mLayerIndex) as IFeatureLayer;

            //初始化编辑
            if (mEdit == null)
            {
                mEdit = new Edit(pFeatureLayer, pMap);
            }

            //开始编辑
            mEdit.StartEditing();
        }

        private void menuMoveFeatures_Click(object sender, EventArgs e)
        {
            mEditOperationType = editOperationType.Move;
            this.mLayerIndex = this.selectLayerToolStripComboBox.SelectedIndex;

            //获取地图和编辑图层
            IMap pMap = this.axMapControl1.Map;
            IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(mLayerIndex) as IFeatureLayer;

            //初始化编辑
            if (mEdit == null)
            {
                mEdit = new Edit(pFeatureLayer, pMap);
            }

            //开始编辑
            mEdit.StartEditing();
        }

        private void selectLayerToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.mLayerIndex = this.selectLayerToolStripComboBox.SelectedIndex;
            if (mEdit != null)
            {
                if (mEdit.HasEdited())
                {
                    DialogResult dr = MessageBox.Show("图层已编辑，是否保存？", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                        mEdit.StopEditing(true);
                    else
                        mEdit.StopEditing(false);
                }

                this.axMapControl1.Refresh();
                //获取地图和编辑图层
                IMap pMap = this.axMapControl1.Map;
                IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(mLayerIndex) as IFeatureLayer;
                mEdit = new Edit(pFeatureLayer, pMap);

                //开始编辑
                mEdit.StartEditing();
            }
        }

        #endregion

        /// <summary>
        /// 地图二三维选项卡切换时图层空间绑定更改的响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) { 
                this.axTOCControl1.SetBuddyControl(axMapControl1.Object);
                this.axToolbarControl1.SetBuddyControl(axMapControl1.Object);
            }
            else if (tabControl1.SelectedIndex == 1) { 
                this.axTOCControl1.SetBuddyControl(axSceneControl1.Object);
                this.axToolbarControl1.SetBuddyControl(axSceneControl1.Object);
            }
        }

        #region 菜单 -> 三维分析
        /// <summary>
        /// 打开sxd文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpenSxdFile_Click(object sender, EventArgs e)
        {
            //打开文件资源管理器选择加载的文件
            OpenFileDialog sxdOpenDialog = new OpenFileDialog();
            sxdOpenDialog.Filter = "sxd文件|*.sxd";
            //打开文件对话框打开事件
            if (sxdOpenDialog.ShowDialog() == DialogResult.OK)
            {
                //从打开对话框中得到打开文件的全路径,并将该路径传入到axSceneControl1中
                axSceneControl1.LoadSxFile(sxdOpenDialog.FileName);
            }
            //将控件置于顶层
            axSceneControl1.BringToFront();
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }

        /// <summary>
        /// 打开raster文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpenRasterFile_Click(object sender, EventArgs e)
        {
            string sFileName = null;
            //新建栅格图层
            IRasterLayer pRasterLayer = new RasterLayerClass();

            //打开文件资源管理器选择加载的文件
            OpenFileDialog rasterLayerOpenDialog = new OpenFileDialog();
            rasterLayerOpenDialog.Filter = "所有文件|*.*";

            //打开文件对话框打开事件
            if (rasterLayerOpenDialog.ShowDialog() == DialogResult.OK)
            {
                //从打开对话框中得到打开文件的全路径
                sFileName = rasterLayerOpenDialog.FileName;
                //创建栅格图层
                pRasterLayer.CreateFromFilePath(sFileName);
                //将图层加入到控件中
                axSceneControl1.Scene.AddLayer(pRasterLayer, true);


                //将当前视点跳转到栅格图层
                ICamera pCamera = axSceneControl1.Scene.SceneGraph.ActiveViewer.Camera;
                //得到范围
                IEnvelope pEenvelop = pRasterLayer.VisibleExtent;
                //添加z轴上的范围
                pEenvelop.ZMin = axSceneControl1.Scene.Extent.ZMin;
                pEenvelop.ZMax = axSceneControl1.Scene.Extent.ZMax;
                //设置相机
                pCamera.SetDefaultsMBB(pEenvelop);
                axSceneControl1.Refresh();
            }

            //将控件置于顶层
            axSceneControl1.BringToFront();
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }

        private void menuSaveImage_Click(object sender, EventArgs e)
        {
            string sFileName = "";
            //打开文件资源管理器选择要保存成的文件
            SaveFileDialog imageSaveDialog = new SaveFileDialog();
            //保存对话框的标题
            imageSaveDialog.Title = "保存图片";
            //保存对话框过滤器
            imageSaveDialog.Filter = "BMP图片|*.bmp|JPG图片|*.jpg";
            //图片的高度和宽度
            int Width = axSceneControl1.Width;
            int Height = axSceneControl1.Height;
            if (imageSaveDialog.ShowDialog() == DialogResult.OK)
            {
                sFileName = imageSaveDialog.FileName;
                if (imageSaveDialog.FilterIndex == 1)//保存成BMP格式的文件
                {
                    axSceneControl1.SceneViewer.GetSnapshot(Width, Height,
                        esri3DOutputImageType.BMP, sFileName);
                }
                else//保存成JPG格式的文件
                {
                    axSceneControl1.SceneViewer.GetSnapshot(Width, Height,
                        esri3DOutputImageType.JPEG, sFileName);
                }
                MessageBox.Show("保存图片成功！");
                axSceneControl1.Refresh();
            }
        }
        private void menuPointSearch_Click(object sender, EventArgs e)
        {
            if (string.Equals("关闭点查询", this.menuPointSearch.Text))
            {
                this.menuPointSearch.Text = "开启点查询";
            }
            else
            {
                this.menuPointSearch.Text = "关闭点查询";
            }

            this.is3DPointQuery = !is3DPointQuery;
        }

        private void axSceneControl1_OnMouseDown(object sender, ISceneControlEvents_OnMouseDownEvent e)
        {
            if (is3DPointQuery)//check按钮处于打勾状态
            {
                //点查询接口
                IHit3DSet mHit3DSet;
                PointSearchResultForm md3QueryResultForm = new PointSearchResultForm();
                //查询
                axSceneControl1.SceneGraph.LocateMultiple(axSceneControl1.SceneGraph.ActiveViewer, e.x, e.y, esriScenePickMode.esriScenePickAll, false, out mHit3DSet);
                mHit3DSet.OnePerLayer();
                if (mHit3DSet == null)//没有选中对象
                {
                    MessageBox.Show("没有选中对象");
                }
                else
                {
                    //显示在ResultForm控件中。
                    md3QueryResultForm.Show();
                    md3QueryResultForm.refeshView(mHit3DSet);
                }
                axSceneControl1.Refresh();
            }
        }

        private void menuConstructTIN_Click(object sender, EventArgs e)
        {
            //初始化构建TIN窗体
            ConstructTINForm constructTINForm = new ConstructTINForm(this.axSceneControl1);
            constructTINForm.Show();
        }


        #endregion

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.mLayerIndex = layerIndex;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            int fid;
            int.TryParse(this.dataGridView1.Rows[e.RowIndex].Cells["FID"].Value.ToString(), out fid);
            pQueryFilter.WhereClause = string.Format("FID = {0}", fid);//通过字段设置查询该要素的条件

            //设置查询图层
            IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(this.mLayerIndex) as IFeatureLayer;
            //执行查询
            IFeatureCursor pFeatureCursor = pFeatureLayer.FeatureClass.Search(pQueryFilter, true);

            //查询结果输出
            IFeature pFeature = pFeatureCursor.NextFeature();

            while (pFeature != null)
            {
                //闪烁3次，间隔300ms
                axMapControl1.FlashShape(pFeature.Shape, 3, 300, null);
                //将查询结果加到数据集中
                axMapControl1.Map.SelectFeature(pFeatureLayer, pFeature);
                pFeature = pFeatureCursor.NextFeature();
            }
        }

        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem toccItem = esriTOCControlItem.esriTOCControlItemNone;
            ILayer iLayer = null;
            IBasicMap iBasicMap = null;
            object unk = null;
            object data = null;
            if (e.button == 1)
            {
                axTOCControl1.HitTest(e.x, e.y, ref toccItem, ref iBasicMap, ref iLayer, ref unk, ref data);
                System.Drawing.Point pos = new System.Drawing.Point(e.x, e.y);
                if (toccItem == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    ESRI.ArcGIS.Carto.ILegendClass pLC = new LegendClassClass();
                    ESRI.ArcGIS.Carto.ILegendGroup pLG = new LegendGroupClass();
                    if (unk is ILegendGroup)
                    {
                        pLG = (ILegendGroup)unk;
                    }
                    pLC = pLG.get_Class((int)data);
                    ISymbol pSym;
                    pSym = pLC.Symbol;
                    ESRI.ArcGIS.DisplayUI.ISymbolSelector pSS = new SymbolSelectorClass();
                    bool bOK = false;
                    pSS.AddSymbol(pSym);
                    bOK = pSS.SelectSymbol(0);
                    if (bOK)
                    {
                        pLC.Symbol = pSS.GetSymbolAt(0);
                    }
                    this.axMapControl1.ActiveView.Refresh();
                    this.axTOCControl1.Refresh();

                }

            }

        }
    }
}

