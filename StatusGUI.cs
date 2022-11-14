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
using Base.GUI;

namespace DeltaPlugin
{
    public partial class StatusGUI : UserControl
    {
        List<InfoItem> infoItems = new List<InfoItem>();
        Delta deltaPLC = null;
        bool initialisation = false;

        public event EventHandler InitRibbon;
        public event EventHandler AddRemoveRibbonItem;
        public class AddRemoveRibbonItemEventArgs : EventArgs
        {
            public bool Add { get; protected set; }
            public string PLCItem { get; protected set; }
            public AddRemoveRibbonItemEventArgs(string plcItem, bool add)
            {
                PLCItem = plcItem;
                Add = add;
            }
        }

        StringParameter ribbonItems;

        public StatusGUI(Delta deltaPLC, StringParameter ribbonItems)
        {
            InitializeComponent();

            this.deltaPLC = deltaPLC;
            this.ribbonItems = ribbonItems;

            if (deltaPLC != null)
                AddStatuses();

            statusPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            statusPanel.AutoSize = true;

            Disposed += StatusGUI_Disposed;
            Functions.FixForm(this);
        }

        private void StatusGUI_Disposed(object sender, EventArgs e)
        {
            foreach (var item in infoItems)
                item.Dispose();
            infoItems.Clear();
        }

        #region INITIALISATION
        private void AddStatuses()
        {
            initialisation = true;
            foreach (var stateVariable in deltaPLC.StateVariables)
            {
                if (stateVariable.Hidden) continue;

                InfoItem infoItem = new InfoItem(deltaPLC.IsLogged(stateVariable.Id), ribbonItems.Value.Contains(stateVariable.Id), stateVariable.Id) {
                Text = stateVariable.DisplayName,
                Tag = stateVariable.Id,
                Margin = new Padding(0, 3, 0, 0) };
                
                infoItem.LoggingChanged += PLC_LoggingChanged;
                infoItem.RibbonChanged += PLC_RibbonChanged;
                infoItems.Add(infoItem);
                statusPanel.Controls.Add(infoItem);

                double value;
                if (double.TryParse(stateVariable.Value, out value))
                {
                    if (stateVariable.Id == "PLC_STATUS")
                        infoItem.BoolValue = value == 0;
                    else
                    {
                        if (stateVariable.IsBoolVariable)
                            infoItem.BoolValue = value > 0;
                        else
                            infoItem.FloatValue = value;
                    }
                }
                else
                {
                    infoItem.FloatValue = -1;
                }
            }
            initialisation = false;
        }

        private void PLC_LoggingChanged(object sender, EventArgs e)
        {
            if (e is InfoItem.LoggingEventArgs && !initialisation)
            {
                InfoItem.LoggingEventArgs logging = e as InfoItem.LoggingEventArgs;
                if (sender is InfoItem)
                {
                    InfoItem i = sender as InfoItem;
                    deltaPLC.Log((string)i.Tag, logging.Log);
                }
            }
        }

        private void PLC_RibbonChanged(object sender, EventArgs e)
        {
            if (e is InfoItem.RibbonEventArgs && !initialisation)
            {
                InfoItem.RibbonEventArgs ribbon = e as InfoItem.RibbonEventArgs;
                if (sender is InfoItem)
                {
                    InfoItem i = sender as InfoItem;
                    AddRemoveRibbonItem?.Invoke(this, new AddRemoveRibbonItemEventArgs((string)i.Tag, ribbon.AddToRibbon));
                    InitRibbon?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion

        #region EVENTS
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (Visible && deltaPLC != null)
            {
                foreach (var stateVariable in deltaPLC.StateVariables)
                {
                    double value;
                    if (double.TryParse(stateVariable.Value, out value))
                    {
                        InfoItem infoItem = GetInfoItemById(stateVariable.Id);

                        if (stateVariable.Id == "PLC_STATUS")
                            infoItem.BoolValue = value == 0;
                        else
                        {
                            if (stateVariable.IsBoolVariable)
                                infoItem.BoolValue = value > 0;
                            else
                                infoItem.FloatValue = value;
                        }
                    }
                }
            }
        }
        #endregion

        #region HELPERS
        InfoItem GetInfoItemById(string id)
        {
            try
            {
                var ii = infoItems.Where(x => (string)x.Tag == id);
                if (ii != null && ii.Count() > 0)
                    return ii.First();
                else
                    return new InfoItem(false, false, id);
            }
            catch
            {
                return new InfoItem(false, false, id);
            }
        }

        Label CreateLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Update();
            return label;
        }

        NoSelectButton CreateButton(string text, Delta.Command command)
        {
            NoSelectButton button = new NoSelectButton();
            button.Text = text;
            button.Margin = new Padding(0);
            button.FlatStyle = FlatStyle.Flat;
            button.Dock = DockStyle.Right;
            button.Tag = command.Id;
            return button;
        }

        private static FlowLayoutPanel CreateFlowLayoutPanel()
        {
            FlowLayoutPanel commandControls = new FlowLayoutPanel();
            commandControls.AutoSize = true;
            commandControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            commandControls.Margin = new Padding(0, 3, 0, 0);
            return commandControls;
        }

        private static DoubleParameterField CreateDoubleParameterField(Delta.Command command)
        {
            var control = new DoubleParameterField();
            control._title = command.DisplayName;
            control.Margin = new Padding(0, 3, 0, 0);
            control.Minimum = 0;
            control.Maximum = int.MaxValue;
            control.Tag = command.Id;
            return control;
        }

        private static BoolParameterField CreateBoolParameterField(Delta.Command command)
        {
            var control = new BoolParameterField();
            control._value_title1 = MultiLang.Translate("On");
            control._value_title2 = MultiLang.Translate("Off");
            control._title = command.DisplayName;
            control.Margin = new Padding(0, 3, 0, 0);
            control.IsCheckedSecond = true;
            control.Tag = command.Id;
            return control;
        }
        #endregion
    }
}
