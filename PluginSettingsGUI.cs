using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base;

namespace DeltaPlugin
{
    public partial class DeltaPluginSettingsGUI : IDeviceSettingsGUI
    {
        DeltaPluginSettings settings;

        public DeltaPluginSettingsGUI()
        {
            InitializeComponent();
        }

        internal void Set(DeltaPluginSettings settings)
        {
            this.settings = settings;
            Set(settings.Parameters, this.Controls.GetEnumerator(), toolTip1);
            settingsFlow.Enabled = enabled.Value;
            logInterval.Visible = settings.logMethod.value;
        }

        private void DiscoverButton_Click(object sender, EventArgs e)
        {
            settings.Discover();
            Set(settings.Parameters, this.Controls.GetEnumerator(), toolTip1);
        }

        private void Enabled_ParameterFieldValueChanged(object sender, IParameter param)
        {
            settingsFlow.Enabled = enabled.Value;
        }

        private void LogMethod_ParameterFieldValueChanged(object sender, IParameter param)
        {
            logInterval.Visible = settings.logMethod.value;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var sd = new SaveFileDialog();
            sd.Filter = "Text files(*.txt)|*.txt|All Files (*.*)|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                logFilename.SetValue(sd.FileName);
            }
        }
    }
}
