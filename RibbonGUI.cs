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
    public partial class RibbonGUI : UserControl
    {
        private StringParameter ribbonItems;
        private Delta deltaPLC;

        public RibbonGUI(StringParameter ribbonItems, Delta deltaPLC)
        {
            InitializeComponent();
            this.ribbonItems = ribbonItems;
            this.deltaPLC = deltaPLC;

            FillRibbonItems();
        }

        private void FillRibbonItems()
        {
            foreach (var item in ribbonItems.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                AddItem(item);
            Functions.FixForm(this);
            UpdateBounds();
        }

        internal void AddItem(string itemName)
        {
            foreach(var stateVariable in deltaPLC.StateVariables)
            {
                if (stateVariable.Id == itemName)
                {
                    RibbonItem ribbonItem = new RibbonItem();
                    ribbonItem.ItemName = stateVariable.DisplayName;
                    ribbonItem.Name = itemName;
                    ribbonItem.Tag = stateVariable;
                    var PositionAddress = new[] { "Y77", "Y61" };
                    if (PositionAddress.Any(ribbonItem.Name.Contains))
                        ribbonItem.Value = stateVariable.IsBoolVariable ? ((bool)stateVariable.ParsedValue ? "Home" : "Out") : stateVariable.Value;
                    else
                        ribbonItem.Value = stateVariable.IsBoolVariable ? ((bool)stateVariable.ParsedValue ? "On" : "Off") : stateVariable.Value;
                    if (stateVariable.IsBoolVariable)
                        ribbonItem.ValueBackgroundColor = (bool)stateVariable.ParsedValue ? Color.LimeGreen : Color.Red;
                    ribbonFlow.Controls.Add(ribbonItem);
                }
            }
        }

        internal void RemoveItem(string itemName)
        {
            var items = ribbonFlow.Controls.Find(itemName, false);
            foreach (var item in items)
                ribbonFlow.Controls.Remove(item);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (ribbonFlow.Controls.Count > 0)
            {
                foreach(var control in ribbonFlow.Controls)
                {
                    if (control is RibbonItem)
                    {
                        RibbonItem ribbonItem = control as RibbonItem;
                        Delta.StateVariable stateVariable = (Delta.StateVariable)ribbonItem.Tag;
                        var PositionAddress = new[] { "Y77", "Y61" };
                        if (PositionAddress.Any(ribbonItem.Name.Contains))//(ribbonItem.Name.Contains(PositionAddress[0]) || ribbonItem.Name.Contains(PositionAddress[1]))// TAISYTA
                            ribbonItem.Value = stateVariable.IsBoolVariable ? ((bool)stateVariable.ParsedValue ? "Home" : "Out") : stateVariable.Value;
                        else
                            ribbonItem.Value = stateVariable.IsBoolVariable ? ((bool)stateVariable.ParsedValue ? "On" : "Off") : stateVariable.Value;
                        if (stateVariable.IsBoolVariable)
                            ribbonItem.ValueBackgroundColor = (bool)stateVariable.ParsedValue ? Color.LimeGreen : Color.Red;
                    }
                }
            }
        }
    }
}
