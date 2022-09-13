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
    /// Summary description for ZoomOut.
    /// </summary>
    [Guid("81364fe7-b1aa-46ea-a561-f850509e8c08")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MyGIS.Classes.ZoomOut")]
    public sealed class ZoomOut : BaseTool
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

        public ZoomOut()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "��С";  //localizable text 
            base.m_message = "��С";  //localizable text
            base.m_toolTip = "��С";  //localizable text
            base.m_name = "ZoomOut";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
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

            // TODO:  Add ZoomOut.OnCreate implementation
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add ZoomOut.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ZoomOut.OnMouseDown implementation
            //��ǰ��ͼ��ͼΪ��ʱ����
            if (m_hookHelper.ActiveView == null)
                return;
            //��ȡ�����λ��
            m_point = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            //
            
            m_isMouseDown = true;

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ZoomOut.OnMouseMove implementation
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
            // TODO:  Add ZoomOut.OnMouseUp implementation

            //MouseDownΪ����ʱ����
            if (!m_isMouseDown) return;

            //�µķ�Χ�Ŀ�Ⱥ͸߶�
            double newWidth, newHeight;

            IActiveView pActiveView = m_hookHelper.ActiveView;

            //��ȡMouseUp����ʱ�ķ�Χ����С
            IEnvelope pEnvelope;
            //׷�ٻ�ȡ�ķ�Χ
            IEnvelope pFeedEnvelope;
            if (m_feedBack == null)//���δ����ʱ���й̶���������С
            {
                pEnvelope = pActiveView.Extent;
                pEnvelope.Expand(2, 2, true);
                pEnvelope.CenterAt(m_point);
            }
            else
            {
                //ֹͣ׷��
                pFeedEnvelope = m_feedBack.Stop();

                //�ж��µķ�Χ�ĸ߶ȺͿ���Ƿ�Ϊ��
                if (pFeedEnvelope.Width == 0 || pFeedEnvelope.Height == 0)
                {
                    m_feedBack = null;
                    m_isMouseDown = false;
                }

                newWidth = pActiveView.Extent.Width * (pActiveView.Extent.Width / pFeedEnvelope.Width);
                newHeight = pActiveView.Extent.Height * (pActiveView.Extent.Height / pFeedEnvelope.Height);

                //�µķ�Χ
                pEnvelope = new EnvelopeClass();
                pEnvelope.PutCoords(pActiveView.Extent.XMin - ((pFeedEnvelope.XMin - pActiveView.Extent.XMin) * (pActiveView.Extent.Width / pFeedEnvelope.Width)),
                    pActiveView.Extent.YMin - ((pFeedEnvelope.YMin - pActiveView.Extent.YMin) * (pActiveView.Extent.Height / pFeedEnvelope.Height)),
                    (pActiveView.Extent.XMin - ((pFeedEnvelope.XMin - pActiveView.Extent.XMin) * (pActiveView.Extent.Width / pFeedEnvelope.Width))) + newWidth,
                    (pActiveView.Extent.YMin - ((pFeedEnvelope.YMin - pActiveView.Extent.YMin) * (pActiveView.Extent.Height / pFeedEnvelope.Height))) + newHeight);
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
