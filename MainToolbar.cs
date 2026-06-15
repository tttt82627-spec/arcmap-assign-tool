using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace FZFZ
{
    [Guid("b4a34a58-e1f0-453f-82a8-b328c004c487")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("FZFZ.MainToolbar")]
    public sealed class MainToolbar : BaseToolbar
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            ArcGISCategoryUnregistration(registerType);
        }

        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Register(regKey);
        }

        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }
        #endregion

        public MainToolbar()
        {
            // 添加打开面板的按钮到工具条
            AddItem(typeof(OpenPanelButton).GUID.ToString("B"));
        }

        public override string Caption
        {
            get { return "地类赋值工具"; }
        }

        public override string Name
        {
            get { return "FZFZ_MainToolbar"; }
        }
    }
}