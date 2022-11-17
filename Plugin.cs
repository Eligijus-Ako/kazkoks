using Base;
using Core;
using DMC;
using Easel.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using DeltaTCPClient;
using System.Data;

namespace DeltaPlugin
{
    public class DeltaPlugin : IDevice
    {
        #region VARIABLES AND PARAMETERS
        string _UniqueName_ = "DeltaPlugin";

        public Interfaces.IEaselService service = null;
        Delta delta = null;
        internal Delta Delta => delta;

        DeltaPluginSettings settings = new DeltaPluginSettings();

        ToolGUI toolGui;
        StatusGUI statusGUI;
        RibbonGUI ribbonGUI;

        List<DeltaIO> deltaIOs = new List<DeltaIO>();
        #endregion

        public DeltaPlugin()
        {
            var cmd = DMC.Helpers.AddTool(ToolLocation.HomeTab, ICommand.AddCreator(typeof(DeltaCommand), DeltaCommand.UN, "Devices"), "Delta PLC");
            cmd.SetImage(Properties.Resources.audio_console_48, false);

            ManageIO();
        }

        #region IDEVICE Implementation
        public bool ApplySettings()
        {
            ManageIO();
            return true;
        }
        public bool Connect()
        {
            if (!settings.Enabled) return true;

            //if (settings.serviceAddress.ValueEmpty)
            //    return Functions.ErrorF("Delta PLC service address must be provided.");

            //EndpointAddress endpoint = new EndpointAddress(settings.ServiceAddress);
            //Binding binding = null;

            //if (settings.ServiceAddress.StartsWith("net.tcp"))
            //{
            //    binding = new NetTcpBinding(SecurityMode.None)
            //    {
            //        Namespace = "http://easel.lt/easelservice/2019/01"
            //    };
            //}
            //else // "net.pipe
            //{
            //    binding = new NetNamedPipeBinding()
            //    {
            //        Namespace = "http://easel.lt/easelservice/2019/01"
            //    };
            //}

            //if (binding != null)
            //{
            //    try
            //    {
            //        var channelFactory = new ChannelFactory<Interfaces.IEaselService>(binding, endpoint);
            //        service = channelFactory.CreateChannel(endpoint);
            //    }
            //    catch (Exception e)
            //    {
            //        return Functions.ErrorF("Failed to connect to Delta service. ", e);
            //    }
            //}

            //if (service != null)
            //{
            //    string desc;
            //    try
            //    {
            //        desc = service.Descriptor();
            //    }
            //    catch (Exception e)
            //    {
            //        return Functions.ErrorF("Failed to start Delta PLC service. ", e);
            //    }

            //    delta = new Delta(settings.logFilename.Value, settings.logMethod.value, settings, settings.logInterval.value);
            //    try
            //    {
            //        Newtonsoft.Json.JsonConvert.PopulateObject(desc, delta);
            //        delta.ExportCommandList();
            //        service.Start();
            //        Functions.Action(this, $"Delta service started: {service.Started()}");
            //        Functions.Action(this, $"Delta service status: {service.Status()}");
            //        delta.StartLogging();
            //    }
            //    catch
            //    {
            //        Disconnect();
            //    }

            //}
            //else
            //{
            //    try
            //    {
            //        Disconnect();
            //    }
            //    catch
            //    {

            //    }
            //    return Functions.ErrorF("Failed to connect to Delta service. Reason: No channel created.");
            //}



            delta = new Delta(settings.logFilename.Value, settings.logMethod.value, settings, settings.logInterval.value);
            try
            {
                string desc = "";
                if (System.IO.File.Exists("DeltaPlugin.json"))
                {
                    desc = System.IO.File.ReadAllText(@"DeltaPlugin.json");
                }
                else
                {
                    if (System.IO.File.Exists(@"c:\Easel\Services\Delta\Json\Generated.json"))
                    {
                        System.IO.File.Copy(@"c:\Easel\Services\Delta\Json\Generated.json", "DeltaPlugin.json");
                        desc = System.IO.File.ReadAllText(@"c:\Easel\Services\Delta\Json\Generated.json");
                    }
                    else
                    {
                        Functions.ErrorF("Cannot find DeltaPlugin settings (JSON) file.");
                    }
                }
                
                Newtonsoft.Json.JsonConvert.PopulateObject(desc, delta);
                delta.ExportCommandList();
                DeltaPLCModbus.Instance.Connect(settings.PollTime);

                Functions.Action(this, $"Delta started: {DeltaPLCModbus.Instance.Connected()}");
                Functions.Action(this, $"Delta status: {DeltaPLCModbus.Instance.GetStatus()}");
                delta.StartLogging();
            }
            catch
            {
                Disconnect();
            }

            AddDeltaTools();
            ManageIO();
            //Poll();
            Task.Factory.StartNew(Poll);

            return true;
        }

        void ManageIO()
        {
            foreach (var io in deltaIOs)
                IODevices.Remove(io);

            deltaIOs.Clear();

            if (!settings.Enabled) return;

            string[] plcData = Delta.ImportDeltaCommands();
            if (plcData != null)
            {
                foreach (var line in plcData)
                {
                    if (line.Contains("PLC_WRITE_ANY_DO")) continue;
                    string[] data = line.Split('\t');
                    if (data.Length < 3) continue;

                    bool isDigital = data.Length > 3 && data[3].Trim().ToLower() == "bool";
                    string name = data[2].Trim();

                    bool isOutput = false;
                    string cmdId = "";
                    string paramId = "";

                    if (data[0] == "c")
                    {
                        string[] ids = data[1].Trim().Split('|');
                        switch (ids.Length)
                        {
                            case 1: cmdId = ids[0]; paramId = ""; break;
                            case 2: cmdId = ids[0]; paramId = ids[1]; break;
                        }
                        isOutput = true;

                    }
                    else
                    {
                        cmdId = data[1].Trim();
                        paramId = cmdId;
                        isOutput = false;
                    }
                    deltaIOs.Add(new DeltaIO(name, cmdId, paramId, isDigital, isOutput, this));
                }
            }



            foreach (var io in deltaIOs)
                IODevices.Add(io);
        }

        List<IFormTool> plcTools = new List<IFormTool>();
        void AddDeltaTools()
        {
            ClearDeltaTools();

            toolGui = new ToolGUI(delta, SetValue);
            IFormTool menuTool = Helpers.AddPopupTool(ToolLocation.HomeTab, "Delta PLC", "PLC Control", toolGui, true, Properties.Resources.audio_console_48);
            plcTools.Add(menuTool);
            statusGUI = new StatusGUI(delta, settings.ribbonItems);
            //statusGUI.InitRibbon += StatusGUI_InitRibbon;
            statusGUI.AddRemoveRibbonItem += StatusGUI_AddRemoveRibbonItem;
            IFormTool statusTool = Helpers.AddPopupTool(ToolLocation.HomeTab, "Delta PLC", "PLC Status", statusGUI, true, Properties.Resources.audio_console_48);
            plcTools.Add(statusTool);
            AddRibbonGUI();
        }

        private void StatusGUI_AddRemoveRibbonItem(object sender, EventArgs e)
        {
            if (e is StatusGUI.AddRemoveRibbonItemEventArgs)
            {
                StatusGUI.AddRemoveRibbonItemEventArgs ea = e as StatusGUI.AddRemoveRibbonItemEventArgs;
                if (ea.Add)
                {
                    settings.ribbonItems.Value += (settings.ribbonItems.ValueEmpty ? "" : "|") + ea.PLCItem;
                    if (ribbonGUI != null)
                        ribbonGUI.AddItem(ea.PLCItem);
                }
                else
                {
                    settings.ribbonItems.Value = settings.ribbonItems.Value.Replace(ea.PLCItem, "");
                    string[] data = settings.ribbonItems.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    settings.ribbonItems.Value = string.Join("|", data);
                    if (ribbonGUI != null)
                        ribbonGUI.RemoveItem(ea.PLCItem);
                }
                Base.Settings.SaveSettings();
            }
        }

        //private void StatusGUI_InitRibbon(object sender, EventArgs e)
        //{
        //    AddRibbonGUI();
        //}

        private void AddRibbonGUI()
        {
            if (ribbonGUI == null)
            {
                ribbonGUI = new RibbonGUI(settings.ribbonItems, delta);
                IFormTool ribbonTool = Helpers.AddTool(DMC.ToolLocation.HomeTab, "Delta PLC", ribbonGUI);
                plcTools.Add(ribbonTool);
            }
        }

        void ClearDeltaTools()
        {
            for (int i = 0; i < plcTools.Count; i++)
            {
                var tool = plcTools[i];
                tool.SetVisible(false);
                FormTool t = (FormTool)tool;
                DMC.Helpers.RemoveTool(t);
                DMC.Helpers.RemoveGroup(ToolLocation.HomeTab, "Delta PLC");
                tool = null;
            }
            plcTools.Clear();
            if (toolGui != null)
            {
                toolGui.Dispose();
                toolGui = null;
            }
            if (statusGUI != null)
            {
                //statusGUI.InitRibbon -= StatusGUI_InitRibbon;
                statusGUI.Dispose();
                statusGUI = null;
            }
            if (ribbonGUI != null)
            {
                ribbonGUI.Dispose();
                ribbonGUI = null;
            }
        }

        private void Poll()
        {
            while (DeltaPLCModbus.Instance.Connected())
            {
                try
                {
                    //Delta newDelta = new Delta();
                    //string q = service.Query();
                    //"{\"StateVariables\":[\r\n  {\r\n    \"Id\": \"PLC_STATUS\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"PLC Status\",\r\n    \"ValueType\": \"\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"PLC_STATUS\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M2999\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Key\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M2999\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y20\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Shutter\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y20\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3004\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Power\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3004\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y33\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Power\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y33\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3013\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"L Syst. OK\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3013\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3020\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Enable\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3020\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y42\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Interlock\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y42\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D308\",\r\n    \"Value\": \"0.0\",\r\n    \"DisplayName\": \"External Level\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D308\",\r\n        \"Formula\": \"VALUE *2.5\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3010\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Armed\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3010\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y41\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Red Pointer 1\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y41\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3012\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"L Thermistor OK\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3012\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y0\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Red Pointer 2\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y0\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3011\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"L SHG Tem. OK\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3011\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D107\",\r\n    \"Value\": \"0.0\",\r\n    \"DisplayName\": \"L External Level\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D107\",\r\n        \"Formula\": \"VALUE*2.5\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y26\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Blowing\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y26\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y35\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Exhaust\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y35\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y27\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Vacuum\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y27\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3023\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Lock Doors\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3023\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X7\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Adjust mode\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X7\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_Y32\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Stage Z Reset\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"Y32\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X6\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Auto mode\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X6\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X33\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Body L\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X33\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X34\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Body R\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X34\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X35\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Body RR\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X35\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X36\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Door Closed\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X36\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X37\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Door Locked\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X37\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_X25\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Exhaust Sensor\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"X25\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3006\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Shutter Sensor\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3006\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3000\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"System Key\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3000\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D306\",\r\n    \"Value\": \"0.0\",\r\n    \"DisplayName\": \"Lighting Level\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D306\",\r\n        \"Formula\": \"VALUE *2.5\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D206\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Scanner T C\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D206\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D207\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Laser Body T C\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D207\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_D208\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Body T C\",\r\n    \"ValueType\": \"float\",\r\n    \"Hidden\": 0,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"D208\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3026\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Safety by PwM\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3026\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  },\r\n  {\r\n    \"Id\": \"PLC_VAR_M3030\",\r\n    \"Value\": \"0\",\r\n    \"DisplayName\": \"Safety by Doors\",\r\n    \"ValueType\": \"bool\",\r\n    \"Hidden\": 1,\r\n    \"Internals\": [\r\n      {\r\n        \"Register\": \"M3030\",\r\n        \"Formula\": \"VALUE\",\r\n        \"Command\": \"READ\",\r\n        \"Default\": null\r\n      }\r\n    ]\r\n  }\r\n]}"
                    //Newtonsoft.Json.JsonConvert.PopulateObject(q, newDelta);

                    foreach (var v in this.delta.StateVariables)
                    {
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(v.Internals[0]);
                        Delta.Internal intern = Newtonsoft.Json.JsonConvert.DeserializeObject<Delta.Internal>(output);
                        var formula = intern.Formula;
                        var register = intern.Register;

                        //PLC_VAR_Y35
                        string plcVar = v.Id != "PLC_STATUS" ? register.ToUpper() : "PLC_STATUS";
                        string val = "";
                        switch (plcVar.Substring(0, 1))
                        {
                            case "D":   //Data register
                                var d = int.Parse(plcVar.Substring(1));
                                val = DeltaPLCModbus.Instance.GetRegister(d).ToString();
                                v.Value = ComputeToDec(formula, val).ToString().Replace(',', '.');// TAISYTA, prideta Replace(',','.')
                                break;
                            case "X":   //Input
                                var x = int.Parse(plcVar.Substring(1));
                                val = DeltaPLCModbus.Instance.GetInput(x).ToString();
                                v.Value = ComputeToBool(formula, val) ? "1" : "0";

                                break;
                            case "Y":   //Output
                                var y = int.Parse(plcVar.Substring(1));
                                val = DeltaPLCModbus.Instance.GetOutput(y).ToString();
                                v.Value = ComputeToBool(formula, val) ? "1" : "0";
                                break;
                            case "P": // PLC_STATUS
                                //val = DeltaPLCModbus.Instance.GetStatus().ToString();
                                var status = DeltaPLCModbus.Instance.GetStatus();
                                v.Value = status == 0 ? "0" : "1";
                                break;
                        }

                        //Console.WriteLine(v.DisplayName + " " + v.Value);
                    }

                    //Task.Run(() => this.delta.Update(newDelta));

                    System.Threading.Thread.Sleep(settings.PollTime);
                }
                catch (Exception e)
                {
                    Functions.Error($"Exception while polling Delta PLC.\r\n {e.ToString()}", true);
                }
            }
        }

        public void Disconnect()
        {
            DeltaPLCModbus.Instance.Disconnect();

            ClearDeltaTools();
            if (delta != null)
            {
                try
                {
                    delta.Disconnect();
                }
                finally
                {
                    delta = null;
                }
            }
        }
        public string GetErrorMessage() { return Functions.GetLastErrorMessage(); }
        public string GetName() { return _UniqueName_; }
        public IDeviceSettings GetSettings() { return settings; }
        public bool IsConnected() { return true; }
        public bool IsEnabled() { return settings.Enabled; }
        public void OnRecipeFinish() { }
        public bool OnRecipeStart() { return true; }
        public void Stop() { }
        #endregion

        public bool SetValue(string commandId, string parameterId, string value)
        {
            if (!settings.Enabled) return Functions.ErrorF("Delta PLC is disabled in settings.");

            var command = delta.GetCommandById(commandId);
            if (command == null)
                return Functions.ErrorF("PLC Command with ID '{0}' not found.", commandId);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(command.Internals[0]);
            Delta.Internal intern = Newtonsoft.Json.JsonConvert.DeserializeObject<Delta.Internal>(output);
            var formula = intern.Formula;
            var register = intern.Register;

            string plcCmd = register.ToUpper();
            switch (plcCmd.Substring(0, 1))
            {
                case "D":   //Data register
                    var d = int.Parse(plcCmd.Substring(1));
                    var i = ComputeToInt(formula, value);
                    DeltaPLCModbus.Instance.SetRegister(d, i);
                    break;
                case "Y":   //Output
                    var y = int.Parse(plcCmd.Substring(1));

                    var b = ComputeToBool(formula, value);
                    DeltaPLCModbus.Instance.SetOutput(y, b);
                    break;
            }

            return true;
        }

        public bool GetValue(string id, out bool value)
        {
            value = false;
            if (!settings.Enabled) return Functions.ErrorF("Delta PLC is disabled in settings.");

            Delta.StateVariable sv = delta.GetStateVariableById(id);
            if (sv != null)
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(sv.Internals[0]);
                Delta.Internal intern = Newtonsoft.Json.JsonConvert.DeserializeObject<Delta.Internal>(output);
                var formula = intern.Formula;
                var register = intern.Register;

                var x = int.Parse(register.Substring(1));
                var v = DeltaPLCModbus.Instance.GetInput(x).ToString();
               
                value = ComputeToBool(formula, v);
                return true;
            }
            value = false;
            return false;



            //Delta query = new Delta();
            //try
            //{
            //    Newtonsoft.Json.JsonConvert.PopulateObject(service.Query(), query);
            //}
            //catch (Exception e)
            //{
            //    Functions.Error($"Failed to get value from Delta PLC. {e.Message}", true);
            //    return Functions.ErrorF("Failed to get value from DeltaPLC. ", e);
            //}


            //Delta.StateVariable sv = query.GetStateVariableById(id);
            //if (sv == null)
            //    return Functions.ErrorF("ID or Register '{0}' not found in PLC.", id);

            //double dval;
            //if (bool.TryParse(sv.Value, out value))
            //    return true;
            //else if (double.TryParse(sv.Value, out dval))
            //{
            //    value = dval > 0;
            //    return true;
            //}
            //else
            //    return Functions.ErrorF("Failed to parse returned value.");
        }

        public bool GetValue(string id, out double value)
        {
            value = 0;
            if (!settings.Enabled) return Functions.ErrorF("Delta PLC is disabled in settings.");
            Delta.StateVariable sv = delta.GetStateVariableById(id);
            if (sv != null)
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(sv.Internals[0]);
                Delta.Internal intern = Newtonsoft.Json.JsonConvert.DeserializeObject<Delta.Internal>(output);
                var formula = intern.Formula;
                var register = intern.Register;

                var x = int.Parse(register.Substring(1));
                //var v = DeltaPLCModbus.Instance.GetInput(x).ToString();
                var v = DeltaPLCModbus.Instance.GetRegister(x).ToString();
                value = Convert.ToDouble(ComputeToDec(formula, v));
                return true;
            }
            value = 0;
            return false;



            //value = 0;
            //if (!settings.Enabled) return Functions.ErrorF("Delta PLC is disabled in settings.");

            //Delta query = new Delta();
            //try
            //{
            //    Newtonsoft.Json.JsonConvert.PopulateObject(service.Query(), query);
            //}
            //catch (Exception e)
            //{
            //    Functions.Error($"Failed to get value from Delta PLC. {e.Message}", true);
            //    return Functions.ErrorF("Failed to get value from Delta. ", e);
            //}


            //Delta.StateVariable sv = query.GetStateVariableById(id);
            //if (sv == null)
            //    return Functions.ErrorF("ID or Register '{0}' not found in PLC.", id);

            //if (double.TryParse(sv.Value, out value))
            //    return true;
            //else
            //    return Functions.ErrorF("Failed to parse returned value.");
        }

        private string Compute(string formula, string value)
        {
            string ret = "";
            try
            {
                var dataTable = new System.Data.DataTable();
                char nds = Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                var v = value.Replace('.', nds).Replace(',', nds);
                var f = formula.Replace("VALUE", v);
                object obj = dataTable.Compute(f, "");
                ret = obj.ToString().Replace('.', nds).Replace(',', nds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + formula + "\r\n" + value );
            }

            return ret;
        }

        private int ComputeToInt(string formula, string decimalVal)
        {
            decimal d100 = decimal.Parse(decimalVal) * 100;
            var res =  Compute(formula, d100.ToString());
            var pos = res.IndexOf(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (pos > 0)
            {
                res = res.Substring(0, pos);
            }
            var r = 0;
            return int.TryParse(res, out r) ? r : 0;
        }

        private decimal ComputeToDec(string formula, string intVal)
        {
            var d100 = Compute(formula, intVal.ToString());
            decimal d = 0;
            return decimal.TryParse(d100, out d) ? d/100 : 0;
        }

        private bool ComputeToBool(string formula, string intVal)
        {
            var i = 0;
            var res = "0";
            if (int.TryParse(intVal, out i)){
                res = Compute(formula, i.ToString());
                return res.Equals("1");
            }
            return false;
        }
    }
}
