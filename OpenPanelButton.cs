using System;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;

namespace FZFZ
{
    public class OpenPanelButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public OpenPanelButton()
        {
        }

        protected override void OnClick()
        {
            try
            {
                IDockableWindowManager dwm = ArcMap.Application as IDockableWindowManager;
                if (dwm == null)
                {
                    System.Windows.Forms.MessageBox.Show("无法获取窗口管理器");
                    return;
                }

                UID uid = new UIDClass();
                uid.Value = "FZFZ_AssignPanel";

                IDockableWindow dockWindow = dwm.GetDockableWindow(uid);
                if (dockWindow == null)
                {
                    System.Windows.Forms.MessageBox.Show("找不到窗口，ID: FZFZ_AssignPanel");
                    return;
                }

                dockWindow.Show(!dockWindow.IsVisible());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("打开面板失败：" + ex.Message);
            }
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}