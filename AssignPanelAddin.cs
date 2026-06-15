using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Desktop.AddIns;

namespace FZFZ
{
    public class AssignPanelAddin : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
    {
        private AssignPanel _panel;
        private bool _initialized = false;

        protected override IntPtr OnCreateChild()
        {
            _panel = new AssignPanel(this.Hook);
            _panel.Dock = DockStyle.Fill;

            // 延迟初始化，确保 ArcMap 已准备好
            _panel.HandleCreated += (s, e) =>
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _panel.BeginInvoke(new Action(() =>
                    {
                        _panel.RefreshLayers();
                        _panel.LoadButtons();
                    }));
                }
            };

            return _panel.Handle;
        }
    }
}