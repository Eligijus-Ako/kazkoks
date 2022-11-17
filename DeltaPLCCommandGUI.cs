using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using Base;
using System.Runtime.CompilerServices;

namespace DeltaPlugin
{
    #region COMMAND GUI
    public partial class DeltaCommandGUI : ICommandGUI
    {
        private static DeltaCommandGUI gui = null;
        DeltaCommandSettings parameters;

        public DeltaCommandGUI()
        {
            InitializeComponent();
            tabs1.tab1controls.Add(setterFlow);
            tabs1.tab2controls.Add(getterFlow);
            tabs1.SelectLastSelectedTab();
        }

        private void DeltaCommandGUI_VisibleChanged(object sender, EventArgs e)
        {
            tabs1.SelectLastSelectedTab();
        }


        public static ICommandGUI Get(DeltaCommandSettings parameters)
        {
            if (gui == null) gui = new DeltaCommandGUI();
            gui.parameters = parameters;
            gui.Set(gui.parameters);
            gui.tabs1.SelectLastSelectedTab();
            return gui;
        }

        void Set(DeltaCommandSettings parameters)
        {
            Set(Controls.GetEnumerator(), parameters.parameters, toolTip);
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            setBlock.Enabled = setEnabled.Value;
            getBlock.Enabled = getEnabled.Value;
            setCommand.Enabled = setCommandSelector.GetSelectedItemIndex() == 0;
            getCommand.Enabled = getCommandSelector.GetSelectedItemIndex() == 0;
            if (setCommandSelector.GetSelectedItemIndex() > 0)
            {
                boolValue.Visible = parameters.setCommandType.list[setCommandSelector.GetSelectedItemIndex() - 1].ToLower() == "bool";
                value.Visible = parameters.setCommandType.list[setCommandSelector.GetSelectedItemIndex() - 1].ToLower() != "bool";
                if (boolValue.Visible) value.SetValue(parameters.boolValue.value ? "1" : "0");
            }
            else
            {
                boolValue.Visible = false;
                value.Visible = true;
            }
        }

        private void SetGetEnabled_ParameterFieldValueChanged(object sender, IParameter param)
        {
            UpdateVisibility();
        }

        private void CommandSelector_StringParameterFieldChanged(StringParameterField sender, string value)
        {
            if (sender == setCommandSelector)
                setCommand.Enabled = sender.GetSelectedItemIndex() == 0;
            else if (sender == getCommandSelector)
                getCommand.Enabled = sender.GetSelectedItemIndex() == 0;

            if (sender.GetSelectedItemIndex() != 0)
            {
                if (sender == setCommandSelector)
                    setCommand.SetValue(parameters.setCommandID.list[sender.GetSelectedItemIndex()]);
                else if (sender == getCommandSelector)
                    getCommand.SetValue(parameters.getCommandID.list[sender.GetSelectedItemIndex()]);
            }

            if (sender == setCommandSelector) UpdateVisibility();
        }

        private void BoolValue_ParameterFieldValueChanged(object sender, IParameter param)
        {
            value.SetValue(parameters.boolValue.value ? "1" : "0");
        }
    }
    #endregion

    #region COMMAND
    public class DeltaCommand : ICommand
    {

        public delegate bool GetFunc(string id, out double value);

        public DeltaCommandSettings parameters = new DeltaCommandSettings();
        public static readonly string UN = "deltaplc_cmd";

        public static DeltaCommand Create() { return new DeltaCommand(); }
        public override Bitmap GetIcon() { return Properties.Resources.audio_console_48; }

        bool updatedAfterConnect = false;

        Func<string, string, string, bool> SetValue = null;
        GetFunc GetValue = null;
        Delta delta = null;

        public DeltaCommand() : base(UN, "Delta PLC", "Delta PLC Command")
        {
            Add(parameters);
            ImportPLCData();

            foreach (var device in SystemDevices.devices)
            {
                if (device is DeltaPlugin)
                {
                    DeltaPlugin dp = device as DeltaPlugin;
                    SetValue = dp.SetValue;
                    GetValue = dp.GetValue;
                    delta = dp.Delta;
                    break;
                }
            }
        }

        void ImportPLCData(bool initValues = true)
        {
            string[] plcData = Delta.ImportDeltaCommands();
            List<string> cmdIds = new List<string>();
            List<string> varIds = new List<string>();
            List<string> cmdNames = new List<string>();
            List<string> varNames = new List<string>();
            List<string> cmdTypes = new List<string>();
            List<string> varTypes = new List<string>();

            cmdIds.Add("");
            cmdNames.Add(MultiLang.Translate("- custom -"));
            varIds.Add("");
            varNames.Add(MultiLang.Translate("- custom -"));
            if (plcData != null)
            {
                foreach (var line in plcData)
                {
                    if (line.Contains("PLC_WRITE_ANY_DO")) continue;
                    string[] data = line.Split('\t');
                    if (data.Length < 3) continue;
                    if (data[0] == "c")
                    {
                        cmdIds.Add(data[1].Trim());
                        cmdNames.Add(data[2].Trim());
                        cmdTypes.Add(data.Length > 3 ? data[3].Trim() : "float");
                    }
                    else
                    {
                        varIds.Add(data[1].Trim());
                        varNames.Add(data[2].Trim());
                        varTypes.Add(data.Length > 3 ? data[3].Trim() : "float");
                    }
                }
            }

            parameters.setCommandSelector.list = cmdNames.ToArray();
            parameters.getCommandSelector.list = varNames.ToArray();
            parameters.setCommandID.list = cmdIds.ToArray();
            parameters.getCommandID.list = varIds.ToArray();
            parameters.setCommandType.list = cmdTypes.ToArray();

            if (initValues)
            {
                if (parameters.setCommandSelector.ValueEmpty || !parameters.setCommandSelector.ValueInList)
                    parameters.setCommandSelector.InitWithFirstListValue();
                if (parameters.getCommandSelector.ValueEmpty || !parameters.getCommandSelector.ValueInList)
                    parameters.getCommandSelector.InitWithFirstListValue();
            }
        }

        public override ICommandGUI GetGUI()
        {
            if (State.is_connected_to_hardware && !updatedAfterConnect)
            {
                ImportPLCData();
                updatedAfterConnect = true;
            }
            if (!State.is_connected_to_hardware) updatedAfterConnect = false;

            return DeltaCommandGUI.Get(parameters);
        }

        public override bool Compile()
        {
            if (!ParseAll()) return false;
            InitDelta();

            string fn = "Delta PLC";

            if (parameters.setEnabled.value)
            {
                if (parameters.setCommand.ValueEmpty) return Functions.ErrorF("PLC Command ID or Register must be set."); //kazka

                if (State.is_connected_to_hardware)
                {
                    if (delta != null)
                    {
                        string[] ids = parameters.setCommand.Value.Split('|');
                        if (ids.Length != 2) return Functions.ErrorF("Failed to parse Command / Parameter IDs.");
                        string commandID = ids[0];
                        string parameterID = ids[1];

                        Delta.Command cmd = delta.GetCommandById(commandID);
                        if (cmd == null)
                            return Functions.ErrorF("PLC Command with ID '{0}' not found.", commandID);
                        else
                        {
                            if (cmd.GetParameterById(parameterID) == null)
                                return Functions.ErrorF("PLC Command Parameter with ID '{0} not found.", parameterID);
                        }
                    }
                }
                if (parameters.value.number < 0) return Functions.ErrorF("PLC Command value must be greater or equal 0.");

                int i = Array.IndexOf(parameters.setCommandSelector.list, parameters.setCommandSelector.Value);
                if (i > 0 && parameters.setCommandType.list[i - 1].ToLower() == "bool")
                    fn += ": " + parameters.setCommandSelector.Value + "=" + (parameters.boolValue.value ? "On" : "Off");
                else
                    fn += ": " + parameters.setCommandSelector.Value + "=" + parameters.value.number;
            }

            if (parameters.getEnabled.value)
            {
                if (parameters.getCommand.ValueEmpty) return Functions.ErrorF("PLC Variable ID or Register must be set.");
                if (State.is_connected_to_hardware)
                {
                    if (delta != null)
                    {
                        Delta.StateVariable sv = delta.GetStateVariableById(parameters.getCommand.Value);
                        if (sv == null)
                            return Functions.ErrorF("PLC Variable with ID '{0}' not found.", parameters.getCommand.Value);
                    }
                }
                if (parameters.plcVariable.ValueEmpty) return Functions.ErrorF("Variable name must be set.");
                Core.Recipes.ActiveRecipe.variables.Add(new Variable(parameters.plcVariable.Value, 0));
                fn += parameters.setEnabled.value ? ", " : ": ";
                fn += parameters.plcVariable.Value + "=" + parameters.getCommandSelector.Value;
            }

            this.friendly_name = fn;

            return true;
        }

        public override bool Run()
        {
            if (!Compile()) return false;
            if (parameters.setEnabled.value)
            {
                if (SetValue != null)
                {
                    string[] ids = parameters.setCommand.Value.Split('|');
                    if (ids.Length != 2) return Functions.ErrorF("Failed to parse Command / Parameter IDs.");
                    string commandID = ids[0];
                    string parameterID = ids[1];
                    if (!SetValue(commandID, parameterID, parameters.value.number.ToString()))
                        return false;
                }
                else
                {
                    return Functions.ErrorF("No SetValue function is assigned for Delta recipe command.");
                }
            }
            if (parameters.getEnabled.value)
            {
                if (GetValue != null)
                {
                    double value;
                    if (GetValue(parameters.getCommand.Value, out value))
                        Core.Recipes.ActiveRecipe.variables.Set(parameters.plcVariable.Value, value);
                    else
                        return false;
                }
                else
                {
                    return Functions.ErrorF("No SetValue function is assigned for Delta recipe command.");
                }
            }
            return true;
        }

        private void InitDelta()
        {
            if (State.is_connected_to_hardware && delta == null)
                foreach (var device in SystemDevices.devices)
                {
                    if (device is DeltaPlugin)
                    {
                        DeltaPlugin dp = device as DeltaPlugin;
                        delta = dp.Delta;
                        break;
                    }
                }
        }
    }
    #endregion

    #region COMMAND SETTINGS
    public class DeltaCommandSettings : MultiParameter
    {
        public BoolParameter setEnabled = new BoolParameter("setEnabled", "Set Value To PLC", false);
        public BoolParameter getEnabled = new BoolParameter("getEnabled", "Get Value From PLC", false);

        public StringListParameter setCommandSelector = new StringListParameter("setCommandSelector", "PLC Command", "", null, true);
        public StringListParameter getCommandSelector = new StringListParameter("getCommandSelector", "PLC Variable", "", null, true);

        public StringListParameter setCommandID = new StringListParameter("setCommandID", "PLC Command ID", "", null, true);
        public StringListParameter getCommandID = new StringListParameter("getCommandID", "PLC Variable ID", "", null, true);

        public StringListParameter setCommandType = new StringListParameter("setCommandType", "PLC Command Type", "", null, true);

        public StringParameter setCommand = new StringParameter("setCommand", "PLC Command ID", "");
        public StringParameter getCommand = new StringParameter("getCommand", "PLC Variable ID", "");

        public ParamSD value = new ParamSD("value", "Value", 0);
        public BoolParameter boolValue = new BoolParameter("boolValue", "Value", "Value", false, "On", "Off");
        public StringParameter plcVariable = new StringParameter("plcVariable", "Variable Name", "name");

        public DeltaCommandSettings() : base("DPLC_settings", "Delta PLC Command Settings", "Delta PLC Command Settings")
        {
            Add(setEnabled);
            Add(getEnabled);
            Add(setCommandSelector);
            Add(getCommandSelector);
            Add(setCommandID);
            Add(getCommandID);
            Add(setCommand);
            Add(getCommand);
            Add(value);
            Add(plcVariable);

            Add(boolValue);
            Add(setCommandType);
        }

        protected override MultiParameter CloneObj()
        {
            DeltaCommandSettings p = new DeltaCommandSettings();
            p.AssignValuesFrom(parameters);
            return p;
        }
    }
    #endregion
}
