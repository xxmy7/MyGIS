using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace MyGIS.Classes
{
    /// <summary>
    /// Summary description for Pan.
    /// </summary>
    [Guid("b7808bdd-ee56-457f-a964-344edc57ccd5")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MyGIS.Classes.Pan")]
    public sealed class Pan : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;
        //��ȡ��ͼ��Χ
        private IScreenDisplay m_focusScreenDisplay=null;
        //��ǲ�������
        private bool m_PanOperation;

        public Pan()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "����";  //localizable text 
            base.m_message = "����";  //localizable text
            base.m_toolTip = "����";  //localizable text
            base.m_name = "Pan";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add Pan.OnCreate implementation
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add Pan.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Pan.OnMouseDown implementation
            //�ж��Ƿ�������
            if (Button != 1) return;

            //��ȡ��ͼ��Χ����ʼ����
            IActiveView pActiveView = m_hookHelper.ActiveView;
            m_focusScreenDisplay = pActiveView.ScreenDisplay;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X,Y);
            m_focusScreenDisplay.PanStart(pPoint);
            //������β���Ϊ��
            m_PanOperation = true;

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Pan.OnMouseMove implementation
            //�ж��Ƿ�������
            if (Button != 1) return;
            //�Ƿ�����״̬
            if (!m_PanOperation) return;
            //׷�����
            IPoint pPoint = m_focusScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            m_focusScreenDisplay.PanMoveTo(pPoint);
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Pan.OnMouseUp implementation
            //�ж��Ƿ�������
            if (Button != 1) return;
            //�Ƿ�����״̬
            if (!m_PanOperation) return;

            IEnvelope pExtent = m_focusScreenDisplay.PanStop();

            //�ж��ƶ������Ƿ�Ϊ��
            if (pExtent != null)
            {
                m_focusScreenDisplay.DisplayTransformation.VisibleBounds = pExtent;
                m_focusScreenDisplay.Invalidate(null, true, (short)esriScreenCache.esriAllScreenCaches);
            }
            //�ر�����״̬
            m_PanOperation = false;
        }
        #endregion
    }
}
