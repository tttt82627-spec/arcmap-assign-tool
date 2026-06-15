using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;
using WinFormsButton = System.Windows.Forms.Button;

namespace FZFZ
{
    public partial class AssignPanel : UserControl
    {
        private IApplication m_application;
        private ComboBox _layerCombo;
        private ComboBox _fieldCombo;
        private Panel _btnPanel;
        private WinFormsButton _btnCustom;
        private Dictionary<string, IFeatureLayer> _layerMap = new Dictionary<string, IFeatureLayer>();

        public AssignPanel(object hook)
        {
            m_application = hook as IApplication;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Size = new Size(280, 500);
            this.BackColor = SystemColors.Control;

            // 图层标签
            Label lblLayer = new Label
            {
                Text = "图层：",
                Location = new Point(5, 5),
                Size = new Size(50, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblLayer);

            // 图层下拉框
            _layerCombo = new ComboBox
            {
                Location = new Point(60, 2),
                Size = new Size(215, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _layerCombo.SelectedIndexChanged += (s, e) =>
            {
                string layerName = _layerCombo.SelectedItem as string;
                if (!string.IsNullOrEmpty(layerName) && _layerMap.TryGetValue(layerName, out IFeatureLayer layer))
                    RefreshFields(layer);
            };
            this.Controls.Add(_layerCombo);

            // 字段标签
            Label lblField = new Label
            {
                Text = "字段：",
                Location = new Point(5, 32),
                Size = new Size(50, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblField);

            // 字段下拉框
            _fieldCombo = new ComboBox
            {
                Location = new Point(60, 29),
                Size = new Size(215, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(_fieldCombo);

            // 按钮面板（可滚动）
            _btnPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 135),
                AutoScroll = true,
                BackColor = Color.White
            };
            this.Controls.Add(_btnPanel);

            // 刷新按钮
            WinFormsButton btnRefresh = new WinFormsButton
            {
                Text = "刷新图层和按钮",
                Location = new Point(10, this.ClientSize.Height - 70),
                Size = new Size(260, 30),
                BackColor = Color.LightGreen
            };
            btnRefresh.Click += (s, e) =>
            {
                try
                {
                    RefreshLayers();
                    LoadButtons();
                    MessageBox.Show($"刷新完成！\n图层数量: {_layerCombo.Items.Count}\n按钮数量: {_btnPanel.Controls.Count}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"刷新失败：{ex.Message}\n{ex.StackTrace}");
                }
            };
            this.Controls.Add(btnRefresh);

            // 自定义按钮
            _btnCustom = new WinFormsButton
            {
                Text = "自定义（编辑配置）",
                Location = new Point(10, this.ClientSize.Height - 35),
                Size = new Size(260, 30),
                BackColor = Color.LightSteelBlue
            };
            _btnCustom.Click += (s, e) =>
            {
                IniHelper.OpenConfigFile();
                LoadButtons();
            };
            this.Controls.Add(_btnCustom);

            this.Load += (s, e) =>
            {
                // 延迟刷新，确保 ArcMap 已准备好
                this.BeginInvoke(new Action(() =>
                {
                    RefreshLayers();
                    LoadButtons();
                }));
            };
        }

        public void RefreshLayers()
        {
            _layerCombo.Items.Clear();
            _layerMap.Clear();
            if (m_application == null)
            {
                System.Diagnostics.Debug.WriteLine("m_application is null");
                return;
            }
            IMxDocument mxDoc = m_application.Document as IMxDocument;
            if (mxDoc == null)
            {
                System.Diagnostics.Debug.WriteLine("mxDoc is null");
                return;
            }
            IMap map = mxDoc.FocusMap;
            System.Diagnostics.Debug.WriteLine($"Map has {map.LayerCount} layers");

            for (int i = 0; i < map.LayerCount; i++)
            {
                if (map.Layer[i] is IFeatureLayer featLayer && featLayer.FeatureClass != null)
                {
                    string layerName = featLayer.Name;
                    _layerCombo.Items.Add(layerName);
                    _layerMap[layerName] = featLayer;
                    System.Diagnostics.Debug.WriteLine($"Added layer: {layerName}");
                }
            }
            if (_layerCombo.Items.Count > 0)
                _layerCombo.SelectedIndex = 0;
            else
                System.Diagnostics.Debug.WriteLine("No feature layers found!");
        }

        private void RefreshFields(IFeatureLayer layer)
        {
            _fieldCombo.Items.Clear();
            if (layer?.FeatureClass == null) return;
            IFields fields = layer.FeatureClass.Fields;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField field = fields.Field[i];
                if (field.Type != esriFieldType.esriFieldTypeOID &&
                    field.Type != esriFieldType.esriFieldTypeGeometry)
                    _fieldCombo.Items.Add(field.Name);
            }
            if (_fieldCombo.Items.Count > 0)
                _fieldCombo.SelectedIndex = 0;
        }

        public void LoadButtons()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadButtons() called");
                _btnPanel.Controls.Clear();
                var items = IniHelper.GetAllAssignItems();
                System.Diagnostics.Debug.WriteLine($"Found {items.Count} config items");

                int y = 5;
                int btnWidth = Math.Max(100, _btnPanel.ClientSize.Width - 20);
                System.Diagnostics.Debug.WriteLine($"btnPanel ClientSize: {_btnPanel.ClientSize.Width}x{_btnPanel.ClientSize.Height}, btnWidth: {btnWidth}");

                foreach (var item in items)
                {
                    WinFormsButton btn = new WinFormsButton
                    {
                        Text = item.Key,
                        Tag = item.Value,
                        Width = btnWidth,
                        Height = 38,
                        Top = y,
                        Left = 5,
                        BackColor = Color.White,
                        Visible = true
                    };
                    btn.Click += (s, e) =>
                    {
                        if (s is WinFormsButton b) AssignValue(b.Tag.ToString());
                    };
                    _btnPanel.Controls.Add(btn);
                    y += 45;
                    System.Diagnostics.Debug.WriteLine($"Added button: {item.Key} at y={y}");
                }
                System.Diagnostics.Debug.WriteLine($"Total buttons in panel: {_btnPanel.Controls.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadButtons error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"加载按钮失败：{ex.Message}\n\n详细信息：{ex.StackTrace}");
            }
        }

        private void AssignValue(string value)
        {
            if (_layerCombo.SelectedItem == null)
            {
                MessageBox.Show("请先选择图层！");
                return;
            }
            if (string.IsNullOrEmpty(_fieldCombo.Text))
            {
                MessageBox.Show("请先选择字段！");
                return;
            }

            string layerName = _layerCombo.SelectedItem as string;
            if (!_layerMap.TryGetValue(layerName, out IFeatureLayer layer))
            {
                MessageBox.Show("未找到图层！");
                return;
            }

            IFeatureSelection sel = layer as IFeatureSelection;
            if (sel?.SelectionSet.Count == 0)
            {
                MessageBox.Show("请先选中要修改的图斑！");
                return;
            }

            IEditor editor = ArcMap.Editor;
            if (editor.EditState != esriEditState.esriStateEditing)
            {
                MessageBox.Show("请先开始编辑！");
                return;
            }

            try
            {
                int fieldIdx = layer.FeatureClass.Fields.FindField(_fieldCombo.Text);
                if (fieldIdx == -1)
                {
                    MessageBox.Show("未找到指定字段！");
                    return;
                }

                editor.StartOperation();
                ICursor cursor;
                sel.SelectionSet.Search(null, false, out cursor);
                IFeatureCursor fc = cursor as IFeatureCursor;
                int count = 0;
                IFeature f = fc.NextFeature();
                while (f != null)
                {
                    f.set_Value(fieldIdx, value);
                    f.Store();
                    count++;
                    f = fc.NextFeature();
                }
                editor.StopOperation("批量赋值");
                MessageBox.Show($"完成！修改了 {count} 个图斑");
                (m_application.Document as IMxDocument)?.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                editor.AbortOperation();
                MessageBox.Show("错误：" + ex.Message);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_btnPanel != null)
            {
                foreach (Control ctrl in _btnPanel.Controls)
                    if (ctrl is WinFormsButton btn)
                        btn.Width = _btnPanel.ClientSize.Width - 20;
            }
        }
    }
}