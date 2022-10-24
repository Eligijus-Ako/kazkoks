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
    public partial class ToolGUI : UserControl
    {
        List<UserControl> controlList = new List<UserControl>();
        Delta delta = null;
        Func<string, string, string, bool> ExecuteCommand = null;
        bool updateToPLC = true;

        public ToolGUI(Delta deltaPLC, Func<string, string, string, bool> executeCommandAction)
        {
            InitializeComponent();
            this.delta = deltaPLC;
            this.ExecuteCommand = executeCommandAction;
            
            if (deltaPLC != null)
                AddControls();

            controlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlPanel.AutoSize = true;

            Disposed += ToolGUI_Disposed;
            Functions.FixForm(this);
        }

        private void ToolGUI_Disposed(object sender, EventArgs e)
        {
            foreach (var item in controlList)
                item.Dispose();
            controlList.Clear();
        }

        #region INITIALISATION
        private void AddControls()
        {
            foreach (var commandGroup in delta.CommandCategories)
            {
                foreach (var command in commandGroup.Commands)
                {
                    if (command.Id == "PLC_WRITE_ANY_DO") continue;
                    foreach (var parameter in command.Parameters)
                    {
                        FlowLayoutPanel commandControls = CreateFlowLayoutPanel();
                        if (command.Parameters[0].IsBoolVariable)
                        {
                            BoolParameterField control = CreateBoolParameterField(command, parameter);
                            control.ParameterFieldValueChanged += (s, e) =>
                            {
                                if (!updateToPLC) return;

                                timer1.Enabled = false;
                                string value = control.IsCheckedFirst ? "1" : "0";
                                SendValueToPLC(control, value);
                                timer1.Interval = 3000;
                                timer1.Enabled = true;
                            };
                            commandControls.Controls.Add(control);
                            controlList.Add(control);
                        }
                        else
                        {
                            NumWithSlider control = CreateNumWithSlider(command, parameter);
                            control.ValueChanged += (s) =>
                            {
                                if (!updateToPLC) return;

                                timer1.Enabled = false;
                                string value = control.Value.ToString("F3");
                                SendValueToPLC(control, value);
                                timer1.Interval = 3000;
                                timer1.Enabled = true;
                            };
                            commandControls.Controls.Add(control);
                            controlList.Add(control);
                        }
                        controlPanel.Controls.Add(commandControls);
                    }
                }
            }
        }

        private void SendValueToPLC(UserControl control, string value)
        {
            string ids = control.Tag.ToString();
            string[] _ids = ids.Split('|');
            if (_ids.Length != 2) { Functions.ErrorF("Can't parse command / parameter ids."); return; }
            string commandId = _ids[0];
            string parameterId = _ids[1];

            if (ExecuteCommand != null && !string.IsNullOrWhiteSpace(commandId) && !string.IsNullOrWhiteSpace(parameterId) && !string.IsNullOrWhiteSpace(value))
            {
                if (!ExecuteCommand(commandId, parameterId, value))
                    Functions.ShowLastError();
            }
            else
            {
                Functions.ErrorF($"No Execute Function is assigned for {commandId}.{parameterId}");
                Functions.ShowLastError();
            }
        }
        #endregion

        #region EVENTS
       

        private void ToolGUI_VisibleChanged(object sender, EventArgs e)
        {
        }
        #endregion

        #region HELPERS

        private static FlowLayoutPanel CreateFlowLayoutPanel()
        {
            FlowLayoutPanel commandControls = new FlowLayoutPanel();
            commandControls.AutoSize = true;
            commandControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            commandControls.Margin = new Padding(0, 3, 0, 0);
            return commandControls;
        }

        private static NumWithSlider CreateNumWithSlider(Delta.Command command, Delta.Parameters parameter)
        {
            var control = new NumWithSlider();
            control._title = $"{command.DisplayName}";
            control.Margin = new Padding(0, 0, 3, 0);
            control.Minimum = 0;
            control.Maximum = 100;
            control.DecimalPlaces = 3;
            control.Value = 0;
            control.Tag = command.Id + "|" + parameter.Id;

            return control;
        }

        private static BoolParameterField CreateBoolParameterField(Delta.Command command, Delta.Parameters parameter)
        {
            var control = new BoolParameterField();
            control._value_title1 = MultiLang.Translate("On");
            control._value_title2 = MultiLang.Translate("Off");
            control._title = $"{command.DisplayName}";
            control.Margin = new Padding(0, 3, 0, 0);
            control.IsCheckedSecond = true;
            control.Tag = command.Id + "|" + parameter.Id;
            return control;
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Interval == 3000)
                timer1.Interval = 250;

            if (delta != null)
            {
                foreach (var stateVariable in delta.StateVariables)
                {
                    double value;
                    if (double.TryParse(stateVariable.Value, out value))
                    {
                        var ctrls = controlList.Where(c => (c is BoolParameterField bpf && bpf._title.Contains(stateVariable.DisplayName)) || (c is NumWithSlider nws && nws._title.Contains(stateVariable.DisplayName)));
                        if (ctrls.Any())
                        {
                            updateToPLC = false;
                            UserControl ctrl = ctrls.First();
                            if (!(ctrl?.Focused ?? true))
                            {
                                switch (ctrl)
                                {
                                    case BoolParameterField boolParam:
                                        boolParam.IsCheckedFirst = value > 0;
                                        boolParam.IsCheckedSecond = value < 1;
                                        break;
                                    case NumWithSlider numWithSlider:
                                        numWithSlider.Value = (decimal)value;
                                        break;
                                }
                            }
                            updateToPLC = true;
                        }
                    }
                }
            }
        }
    }
}
