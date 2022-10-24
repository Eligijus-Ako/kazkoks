using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaPlugin
{
    public class Delta
    {
        public CommandCategory[] CommandCategories { get; set; }
        public StateVariable[] StateVariables { get; set; }

        public class CommandCategory
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public Command[] Commands { get; set; }
        }

        public class Command
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public Parameters[] Parameters { get; set; }
            public object[] Internals { get; set; }

            internal Parameters GetParameterById(string id)
            {
                foreach (var parameter in Parameters)
                    if (parameter.Id == id)
                        return parameter;
                return null;
            }
        }

        public class Parameters
        {
            public string Category { get; set; }
            public string Description { get; set; }
            public string DisplayName { get; set; }
            public string Id { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            internal bool IsBoolVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "bool";

            internal bool IsIntVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "int";

            internal bool IsFloatVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "float";
        }

        public class StateVariable
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public bool Hidden { get; set; }
            public object[] Internals { get; set; }
            internal object ParsedValue
            {
                get
                {
                    if (Internals.Length > 0)
                        if (IsBoolVariable) return Value != "0";
                        else if (IsIntVariable)
                        {
                            int value;
                            if (int.TryParse(Value, out value))
                                return value;
                        }
                        else if (IsFloatVariable)
                        {
                            double value;
                            if (double.TryParse(Value, out value))
                                return value;
                        }
                    return Value;
                }
            }

            internal bool IsBoolVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "bool";

            internal bool IsIntVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "int";

            internal bool IsFloatVariable => string.IsNullOrWhiteSpace(ValueType) ? false : ValueType.ToLower() == "float";
        }

        public class Internal
        {
            public string Command { get; set; }
            public string Register { get; set; }
            public string Formula { get; set; }
        }

        public class Setter
        {
            public string Id { get; set; }
            public Parameters[] Parameters { get; set; }

            internal static Setter Create(string commandId, string parameterId, string value)
            {
                return new Setter() { Id = commandId, Parameters = new Parameters[] { new Parameters() { Id = parameterId, Value = value } } };
            }
        }

        private string logFilename;
        private bool intervalLogging;
        private int logInterval;
        private bool stopLogging;
        private DeltaPluginSettings settings;

        public Delta()
        {
            stopLogging = true;
        }

        public Delta(string logFilename, bool intervalLogging, DeltaPluginSettings settings, int logInterval = 500)
        {
            this.logFilename = logFilename;
            this.intervalLogging = intervalLogging;
            this.logInterval = logInterval;
            this.stopLogging = false;
            this.settings = settings;
        }

        internal void StartLogging()
        {
            if (!string.IsNullOrWhiteSpace(settings.loggedIds.Value))
            {
                string[] ids = settings.loggedIds.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids)
                    if (!loggedIds.Contains(id) && GetStateVariableById(id) != null)
                        loggedIds.Add(id);

                string logids = string.Join(",", loggedIds);
                settings.loggedIds.Value = logids;
                Base.Settings.SaveSettings();
            }

            if (intervalLogging) (new Thread(() => LogThread())).Start();
        }

        internal void Disconnect()
        {
            stopLogging = true;
        }

        object _flock_ = new object();
        private void LogThread()
        {
            while (!stopLogging)
            {
                StringBuilder sb = new StringBuilder();
                string timestamp = GetTimeStamp();
                sb.Append(timestamp);
                string line = "";
                foreach (var logId in loggedIds)
                {
                    string logLine = GetVariableLogLine(logId);
                    if (!string.IsNullOrWhiteSpace(logLine))
                    {
                        line += logLine;
                    }
                }
                sb.AppendLine(line);

                lock (_flock_)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            File.AppendAllText(logFilename, sb.ToString());
                    }
                    catch
                    { }
                }
                Thread.Sleep(logInterval);
            }
        }

        private string GetVariableLogLine(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var variable = GetStateVariableById(id);
            if (variable == null) return null;

            string logLine = $"\t{variable.DisplayName}\t{variable.Value}";
            return logLine;
        }

        private string GetTimeStamp()
        {
            var datetime = DateTime.Now;
            return $"{datetime.Year}-{datetime.Month}-{datetime.Day} {datetime.Hour.ToString("D2")}:{datetime.Minute.ToString("D2")}:{datetime.Second.ToString("D2")}";
        }

        private List<string> loggedIds = new List<string>();

        internal void Log(string id, bool log)
        {
            switch(log)
            {
                case true:
                    if (!loggedIds.Contains(id))
                        loggedIds.Add(id);
                    break;
                case false:
                    if (loggedIds.Contains(id))
                        loggedIds.Remove(id);
                    break;
            }

            string logids = string.Join(",", loggedIds);
            settings.loggedIds.Value = logids;
            Base.Settings.SaveSettings();
        }

        internal bool IsLogged(string id) { return loggedIds.Contains(id); }

        internal StateVariable GetStateVariableById(string id)
        {
            if (this.StateVariables == null) return null;
            foreach (var stateVariable in this.StateVariables)
                if (stateVariable.Id == id)
                    return stateVariable;
            return null;
        }

        internal void Update(Delta query)
        {
            foreach(var stateVariable in query.StateVariables)
            {
                var variable = this.GetStateVariableById(stateVariable.Id);
                if (variable != null)
                {
                    SetCurrentState(stateVariable, variable);
                }
            }
        }

        internal Command GetCommandById(string id)
        {
            if (this.CommandCategories == null) return null;
            foreach (var commandCategory in this.CommandCategories)
                foreach (var command in commandCategory.Commands)
                    if (command.Id == id)
                        return command;
            return null;
        }

        private void SetCurrentState(StateVariable source, StateVariable destination)
        {
            string oldValue = destination.Value;
            destination.Value = source.Value;
            if (!intervalLogging && loggedIds.Contains(destination.Id) && oldValue != source.Value && !stopLogging)
            {
                StringBuilder sb = new StringBuilder();
                string timestamp = GetTimeStamp();
                string logLine = GetVariableLogLine(destination.Id);
                if (!string.IsNullOrWhiteSpace(logLine))
                {
                    sb.Append(timestamp);
                    sb.AppendLine(logLine);
                    lock (_flock_)
                    {
                        try
                        {
                            File.AppendAllText(logFilename, sb.ToString());
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }

        static string FileName => Base.Settings.PathAppData + "DeltaPLCCommands.dat";
        public bool ExportCommandList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var commandCategory in this.CommandCategories)
                foreach (var command in commandCategory.Commands)
                {
                    foreach (var parameter in command.Parameters)
                    {
                        sb.Append("c");
                        sb.Append("\t");
                        sb.Append($"{command.Id}|{parameter.Id}");
                        sb.Append("\t");
                        //sb.AppendLine($"{command.DisplayName} ({parameter.DisplayName})");
                        sb.Append($"{command.DisplayName}");
                        sb.Append("\t");
                        sb.AppendLine(parameter.ValueType);
                    }
                }
            foreach (var variable in this.StateVariables)
            {
                sb.Append("v");
                sb.Append("\t");
                sb.Append(variable.Id);
                sb.Append("\t");
                sb.Append(variable.DisplayName);
                sb.Append("\t");
                sb.AppendLine(variable.ValueType);
            }
            try
            {
                File.WriteAllText(FileName, sb.ToString());
            }
            catch (Exception e)
            {
                return Base.Functions.ErrorF("Failed to export Delta PLC commands to file. ", e);
            }
            return true;
        }

        public static string[] ImportDeltaCommands()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    string[] data = File.ReadAllLines(FileName);
                    return data;
                }
                catch (Exception e)
                {
                    Base.Functions.ErrorF("Failed to import Delta PLC commands from file. ");
                }
            }
            return null;
        }
    }
}
