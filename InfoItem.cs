using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeltaPlugin
{
    public partial class InfoItem : UserControl
    {
        public override string Text // weee pakeitimas
        {
            get => textLabel.Text;
            set => textLabel.Text = value;
        }

        private string infoItemID;
        bool boolValue = false;
        public bool BoolValue
        {
            get => boolValue;
            set
            {
                boolValue = value;
                valueLabel.BackColor = boolValue ? BGColorTrue : BGColorFalse;
                var PositionAddress = new[] { "Y77", "Y61" };
                if(PositionAddress.Any(infoItemID.Contains ))
                    valueLabel.Text = boolValue ? Base.MultiLang.Translate("Home") : Base.MultiLang.Translate("Out");
                
                else
                    valueLabel.Text = boolValue ? Base.MultiLang.Translate("On") : Base.MultiLang.Translate("Off");
                valueLabel.ForeColor = boolValue ? Color.DarkSlateGray : Color.WhiteSmoke;
                valueLabel.Font = new Font(valueLabel.Font, FontStyle.Bold);
            }
        }

        double floatValue = 0;
        public double FloatValue
        {
            get => floatValue;
            set
            {
                floatValue = value;
                valueLabel.BackColor = Color.Transparent;
                valueLabel.Text = floatValue.ToString("F3");
                valueLabel.Font = new Font(valueLabel.Font, FontStyle.Regular);
            }
        }

        public event EventHandler LoggingChanged;
        public event EventHandler RibbonChanged;
        public class LoggingEventArgs : EventArgs
        {
            public bool Log { get; protected set; }
            public LoggingEventArgs(bool log)
            {
                Log = log;
            }
        }
        public class RibbonEventArgs : EventArgs
        {
            public bool AddToRibbon { get; protected set; }
            public RibbonEventArgs(bool addToRibbon)
            {
                AddToRibbon = addToRibbon;
            }
        }

        public Color BGColorTrue { get; set; } = Color.LimeGreen;
        public Color BGColorFalse { get; set; } = Color.Red;


        public InfoItem(bool logging, bool inRibbon, string inInfoItemID)
        {
            InitializeComponent();
            log.Checked = logging;
            ribbon.Checked = inRibbon;
            infoItemID = inInfoItemID;
        }

        private void Log_CheckedChanged(object sender, EventArgs e)
        {
            LoggingChanged?.Invoke(this, new LoggingEventArgs(log.Checked));
        }

        private void Ribbon_CheckedChanged(object sender, EventArgs e)
        {
            RibbonChanged?.Invoke(this, new RibbonEventArgs(ribbon.Checked));
        }
    }
}
