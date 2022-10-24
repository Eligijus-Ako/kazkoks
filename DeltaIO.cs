using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaPlugin
{
    public class DeltaIO : IIODevice
    {
        string name;
        string commandID;
        string paramID;

        bool isDigital;
        bool isOutput;

        bool bValue;
        double aValue;

        public delegate bool GetFunc<T>(string id, out T value);
        Func<string, string, string, bool> SetValue = null;
        GetFunc<double> GetValueD = null;
        GetFunc<bool> GetValueB = null;

        public DeltaIO(string name, string commandID, string paramID, bool isDigital, bool isOutput, DeltaPlugin deltaPlugin)
        {
            this.name = name;
            this.commandID = commandID;
            this.paramID = paramID;

            this.isDigital = isDigital;
            this.isOutput = isOutput;

            bValue = false;
            aValue = 0;

            SetValue = deltaPlugin.SetValue;
            GetValueD = deltaPlugin.GetValue;
            GetValueB = deltaPlugin.GetValue;
        }

        public bool CanReadAnalogOuput() => true;
        public bool CanReadDigitalOuput() => true;
        public bool Connect() => true;
        public void Disconnect() { }

        public bool GetAnalogInput(int port, ref double value)
        {
            return GetValueD(paramID, out value);
        }

        public bool GetAnalogInputPortCount(ref int port_count)
        {
            port_count = !isOutput && !isDigital ? 1 : 0;
            return true;
        }

        public bool GetAnalogMinMaxValue(int port, ref double min_value, ref double max_value)
        {
            min_value = 0;
            max_value = 100;
            return true;
        }

        public bool GetAnalogOutput(int port, ref double value)
        {
            value = aValue;
            return true;
        }

        public bool GetAnalogOutputPortCount(ref int port_count)
        {
            port_count = isOutput && !isDigital ? 1 : 0;
            return true;
        }

        public bool GetDigitalInput(int port, ref bool value)
        {
            //double aValue;
            //bool ok = GetValue(paramID, out aValue);
            //value = aValue > 0;
            //return ok;
            return GetValueB(paramID, out value);
        }

        public bool GetDigitalInputPortCount(ref int port_count)
        {
            port_count = !isOutput && isDigital ? 1 : 0;
            return true;
        }

        public bool GetDigitalOutput(int port, ref bool value)
        {
            value = bValue;
            return true;
        }

        public bool GetDigitalOutputPortCount(ref int port_count)
        {
            port_count = isOutput && isDigital ? 1 : 0;
            return true;
        }

        public string GetErrorMessage() => Functions.GetLastErrorMessage();

        public string GetName() => name;

        public bool SetAnalogOutput(int port, double value)
        {
            if (SetValue == null) return Functions.ErrorF("No SetValue function is assigned for Delta IO.");

            aValue = value;
            return SetValue(commandID, paramID, value.ToString());
        }

        public bool SetDigitalOutput(int port, bool value)
        {
            if (SetValue == null) return Functions.ErrorF("No SetValue function is assigned for Delta IO.");

            bValue = value;
            return SetValue(commandID, paramID, value ? "1" : "0");
        }
    }
}
