using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;

namespace MyGIS.Classes
{
    /// <summary>
    /// Summary description for ZoomIn.
    /// </summary>
    [Guid("fb9ebd10-bfcd-4989-9d9f-0baef45a5322")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MyGIS.Classes.ZoomIn")]
    public sealed class ZoomIn : BaseTool
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
        //��¼���λ��
        private IPoint m_point;
        //���MouseDown�Ƿ���
        private Boolean m_isMouseDown;
        //׷������ƶ������µ�Envelope
        private INewEnvelopeFeedback m_feedBack;

        public ZoomIn()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "�Ŵ�";  //localizable text 
            base.m_message = "�Ŵ�";  //localizable text
            base.m_toolTip = "�Ŵ�";  //localizable text
            base.m_name = "ZoomIn";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
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

            // TODO:  Add ZoomIn.OnCreate implementation
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add ZoomIn.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ZoomIn.OnMouseDown implementation
            //��ǰ��ͼ��ͼΪ��ʱ����
            if (m_hookHelper.ActiveView == null)
                return;
            //��ȡ�����λ��
            m_point = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            m_isMouseDown = true;

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ZoomIn.OnMouseMove implementation
            //MouseDownΪ����ʱ����
            if (!m_isMouseDown)
                return;

            IActiveView pActiveView = m_hookHelper.ActiveView;
            //m_feedBack׷������ƶ�
            if (m_feedBack == null)
            {
                m_feedBack = new NewEnvelopeFeedbackClass();
                m_feedBack.Display = pActiveView.ScreenDisplay;
                //��ʼ׷��
                m_feedBack.Start(m_point);
            }
            //׷������ƶ�λ��
            m_feedBack.MoveTo(pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y));

        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ZoomIn.OnMouseUp implementation
            //MouseDownΪ����ʱ����
            if (!m_isMouseDown) return;

            IActiveView pActiveView = m_hookHelper.ActiveView;

            //��ȡMouseUp����ʱ�ķ�Χ���Ŵ�
            IEnvelope pEnvelope;
            if (m_feedBack == null)//���δ����ʱ���й̶������߷Ŵ�
            {
                pEnvelope = pActiveView.Extent;
                pEnvelope.Expand(0.5, 0.5, true);
                pEnvelope.CenterAt(m_point);
            }
            else
            {
                //ֹͣ׷��
                pEnvelope = m_feedBack.Stop();

                //�ж��µķ�Χ�ĸ߶ȺͿ���Ƿ�Ϊ��
                if (pEnvelope.Width == 0 || pEnvelope.Height == 0)
                {
                    m_feedBack = null;
                    m_isMouseDown = false;
                }
            }
            //��ȡ�µķ�Χ
            pActiveView.Extent = pEnvelope;
            //ˢ����ͼ
            pActiveView.Refresh();
            m_feedBack = null;
            m_isMouseDown = false;
        }
        #endregion
    }
}
