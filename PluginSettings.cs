using Base;
using Easel.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace DeltaPlugin
{
    public class DeltaPluginSettings : IDeviceSettings
    {
        #region STATIC FIELDS
        static readonly string _UniqueName_ = "DeltaPluginSettings";
        static readonly string _FriendlyName_ = "Delta";
        static readonly string _Description_ = "Delta PLC";
        #endregion

        #region VARIABLES AND PARAMETERS
        public StringListParameter serviceAddress = new StringListParameter("serviceAddress", "Delta PLC Service Address", "", null, true);
        public string ServiceAddress => serviceAddress.Value;

        public IntParameter pollTime = new IntParameter("pollTime", "Query Interval (ms)", 100);
        public int PollTime => pollTime.value;

        public StringParameter logFilename = new StringParameter("logFilename", "Log Filename", Settings.PathTEMP + "deltaPLC_log.txt");
        public BoolParameter logMethod = new BoolParameter("logMethod", "Log Method", "Log Method", false, "At Interval", "On Change");
        public IntParameter logInterval = new IntParameter("logInterval", "Log Interval (ms)", 500);

        public StringParameter loggedIds = new StringParameter("loggedIds", "LoggedIDs", "");
        public StringParameter ribbonItems = new StringParameter("ribbonItems", "Ribbon Items", "");

        #endregion

        public DeltaPluginSettings() : base(_UniqueName_, _FriendlyName_, _Description_)
        {
            Add(serviceAddress);
            Add(pollTime);

            Add(logFilename);
            Add(logMethod);
            Add(logInterval);
            Add(loggedIds);
            Add(ribbonItems);
        }

        public override System.Windows.Forms.UserControl GetGUI()
        {
            DeltaPluginSettingsGUI gui = new DeltaPluginSettingsGUI();
            gui.Set(this);
            return gui;
        }

        public void Discover()
        {
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindCriteria findCriteria = new FindCriteria(typeof(Interfaces.IEaselService))
            {
                Duration = TimeSpan.FromMilliseconds(1000)
            };

            FindResponse findResponse = discoveryClient.Find(findCriteria);

            string[] uris = null;
            if (findResponse.Endpoints.Count > 0)
            {
                string machineName = Environment.MachineName.ToUpper();
                uris = findResponse.Endpoints.Where(x =>
                (x.Address.ToString().ToUpper().Contains(machineName) &&
                !x.Address.ToString().ToUpper().Contains(machineName + "/HEDYLOGOS") ||
                x.Address.Uri.Scheme == "net.tcp")).Select(x => x.Address.Uri.ToString()).ToArray();
            }

            if (uris != null)
            {
                serviceAddress.list = uris;
                if (serviceAddress.ValueEmpty || !serviceAddress.ValueInList)
                    serviceAddress.InitWithFirstListValue();
            }
        }
    }
}
