namespace DeltaPlugin
{
    partial class DeltaCommandGUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabs1 = new Base.GUI.tabs();
            this.setterFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.setEnabled = new Base.CheckBoolParameterField();
            this.setBlock = new System.Windows.Forms.FlowLayoutPanel();
            this.setCommandSelector = new Base.StringParameterField();
            this.setCommand = new Base.StringParameterField();
            this.value = new Base.StringParameterField();
            this.boolValue = new Base.GUI.BoolParameterField();
            this.getterFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.getEnabled = new Base.CheckBoolParameterField();
            this.getBlock = new System.Windows.Forms.FlowLayoutPanel();
            this.getCommandSelector = new Base.StringParameterField();
            this.getCommand = new Base.StringParameterField();
            this.plcVariable = new Base.StringParameterField();
            this.flowLayoutPanel1.SuspendLayout();
            this.setterFlow.SuspendLayout();
            this.setBlock.SuspendLayout();
            this.getterFlow.SuspendLayout();
            this.getBlock.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.tabs1);
            this.flowLayoutPanel1.Controls.Add(this.setterFlow);
            this.flowLayoutPanel1.Controls.Add(this.getterFlow);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(342, 271);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // tabs1
            // 
            this.tabs1.AutoSize = true;
            this.tabs1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tabs1.Location = new System.Drawing.Point(3, 3);
            this.tabs1.MaximumSize = new System.Drawing.Size(350, 0);
            this.tabs1.Name = "tabs1";
            this.tabs1.Size = new System.Drawing.Size(296, 32);
            this.tabs1.Tab1Enabled = true;
            this.tabs1.Tab1Name = "Set";
            this.tabs1.Tab1Visible = true;
            this.tabs1.Tab2Enabled = true;
            this.tabs1.Tab2Name = "Get";
            this.tabs1.Tab2Visible = true;
            this.tabs1.Tab3Enabled = false;
            this.tabs1.Tab3Name = "";
            this.tabs1.Tab3Visible = true;
            this.tabs1.Tab4Enabled = true;
            this.tabs1.Tab4Name = "";
            this.tabs1.Tab4Visible = false;
            this.tabs1.Tab5Enabled = true;
            this.tabs1.Tab5Name = "";
            this.tabs1.Tab5Visible = false;
            this.tabs1.Tab6Enabled = true;
            this.tabs1.Tab6Name = "";
            this.tabs1.Tab6Visible = false;
            this.tabs1.TabIndex = 1;
            // 
            // setterFlow
            // 
            this.setterFlow.AutoSize = true;
            this.setterFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setterFlow.Controls.Add(this.setEnabled);
            this.setterFlow.Controls.Add(this.setBlock);
            this.setterFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.setterFlow.Location = new System.Drawing.Point(0, 38);
            this.setterFlow.Margin = new System.Windows.Forms.Padding(0);
            this.setterFlow.Name = "setterFlow";
            this.setterFlow.Size = new System.Drawing.Size(342, 129);
            this.setterFlow.TabIndex = 2;
            // 
            // setEnabled
            // 
            this.setEnabled._title = "setEnabled";
            this.setEnabled.AutoSize = true;
            this.setEnabled.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setEnabled.Location = new System.Drawing.Point(3, 3);
            this.setEnabled.Name = "setEnabled";
            this.setEnabled.Size = new System.Drawing.Size(306, 23);
            this.setEnabled.TabIndex = 2;
            this.setEnabled.Value = false;
            this.setEnabled.ParameterFieldValueChanged += new Base.GUI.ParameterFieldValueChangedDelegate(this.SetGetEnabled_ParameterFieldValueChanged);
            // 
            // setBlock
            // 
            this.setBlock.AutoSize = true;
            this.setBlock.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setBlock.Controls.Add(this.setCommandSelector);
            this.setBlock.Controls.Add(this.setCommand);
            this.setBlock.Controls.Add(this.value);
            this.setBlock.Controls.Add(this.boolValue);
            this.setBlock.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.setBlock.Location = new System.Drawing.Point(0, 29);
            this.setBlock.Margin = new System.Windows.Forms.Padding(0);
            this.setBlock.Name = "setBlock";
            this.setBlock.Size = new System.Drawing.Size(342, 100);
            this.setBlock.TabIndex = 1;
            // 
            // setCommandSelector
            // 
            this.setCommandSelector._title = "setCommandSelector";
            this.setCommandSelector.AutoSize = true;
            this.setCommandSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setCommandSelector.CanBeEmpty = false;
            this.setCommandSelector.Location = new System.Drawing.Point(0, 0);
            this.setCommandSelector.Margin = new System.Windows.Forms.Padding(0);
            this.setCommandSelector.Name = "setCommandSelector";
            this.setCommandSelector.Size = new System.Drawing.Size(303, 27);
            this.setCommandSelector.TabIndex = 0;
            this.setCommandSelector.ValueOnlyFromList = true;
            this.setCommandSelector.StringParameterFieldChanged += new Base.StringParameterFieldChangedDelegate(this.CommandSelector_StringParameterFieldChanged);
            // 
            // setCommand
            // 
            this.setCommand._title = "setCommand";
            this.setCommand.AutoSize = true;
            this.setCommand.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setCommand.CanBeEmpty = false;
            this.setCommand.Location = new System.Drawing.Point(0, 27);
            this.setCommand.Margin = new System.Windows.Forms.Padding(0);
            this.setCommand.Name = "setCommand";
            this.setCommand.Size = new System.Drawing.Size(303, 24);
            this.setCommand.TabIndex = 0;
            this.setCommand.ValueOnlyFromList = false;
            // 
            // value
            // 
            this.value._title = "value";
            this.value.AutoSize = true;
            this.value.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.value.CanBeEmpty = false;
            this.value.Location = new System.Drawing.Point(0, 51);
            this.value.Margin = new System.Windows.Forms.Padding(0);
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(303, 24);
            this.value.TabIndex = 0;
            this.value.ValueOnlyFromList = false;
            // 
            // boolValue
            // 
            this.boolValue._title = "boolValue";
            this.boolValue._value_title1 = "radioButton1";
            this.boolValue._value_title2 = "radioButton2";
            this.boolValue.AutoSize = true;
            this.boolValue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.boolValue.IsCheckedFirst = false;
            this.boolValue.IsCheckedSecond = false;
            this.boolValue.Location = new System.Drawing.Point(0, 75);
            this.boolValue.Margin = new System.Windows.Forms.Padding(0);
            this.boolValue.Name = "boolValue";
            this.boolValue.Shift = 0;
            this.boolValue.Shift2 = 0;
            this.boolValue.Size = new System.Drawing.Size(342, 25);
            this.boolValue.TabIndex = 1;
            this.boolValue.ParameterFieldValueChanged += new Base.GUI.ParameterFieldValueChangedDelegate(this.BoolValue_ParameterFieldValueChanged);
            // 
            // getterFlow
            // 
            this.getterFlow.AutoSize = true;
            this.getterFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getterFlow.Controls.Add(this.getEnabled);
            this.getterFlow.Controls.Add(this.getBlock);
            this.getterFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.getterFlow.Location = new System.Drawing.Point(0, 167);
            this.getterFlow.Margin = new System.Windows.Forms.Padding(0);
            this.getterFlow.Name = "getterFlow";
            this.getterFlow.Size = new System.Drawing.Size(312, 104);
            this.getterFlow.TabIndex = 2;
            // 
            // getEnabled
            // 
            this.getEnabled._title = "getEnabled";
            this.getEnabled.AutoSize = true;
            this.getEnabled.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getEnabled.Location = new System.Drawing.Point(3, 3);
            this.getEnabled.Name = "getEnabled";
            this.getEnabled.Size = new System.Drawing.Size(306, 23);
            this.getEnabled.TabIndex = 2;
            this.getEnabled.Value = false;
            this.getEnabled.ParameterFieldValueChanged += new Base.GUI.ParameterFieldValueChangedDelegate(this.SetGetEnabled_ParameterFieldValueChanged);
            // 
            // getBlock
            // 
            this.getBlock.AutoSize = true;
            this.getBlock.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getBlock.Controls.Add(this.getCommandSelector);
            this.getBlock.Controls.Add(this.getCommand);
            this.getBlock.Controls.Add(this.plcVariable);
            this.getBlock.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.getBlock.Location = new System.Drawing.Point(0, 29);
            this.getBlock.Margin = new System.Windows.Forms.Padding(0);
            this.getBlock.Name = "getBlock";
            this.getBlock.Size = new System.Drawing.Size(303, 75);
            this.getBlock.TabIndex = 3;
            // 
            // getCommandSelector
            // 
            this.getCommandSelector._title = "getCommandSelector";
            this.getCommandSelector.AutoSize = true;
            this.getCommandSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getCommandSelector.CanBeEmpty = false;
            this.getCommandSelector.Location = new System.Drawing.Point(0, 0);
            this.getCommandSelector.Margin = new System.Windows.Forms.Padding(0);
            this.getCommandSelector.Name = "getCommandSelector";
            this.getCommandSelector.Size = new System.Drawing.Size(303, 27);
            this.getCommandSelector.TabIndex = 0;
            this.getCommandSelector.ValueOnlyFromList = true;
            this.getCommandSelector.StringParameterFieldChanged += new Base.StringParameterFieldChangedDelegate(this.CommandSelector_StringParameterFieldChanged);
            // 
            // getCommand
            // 
            this.getCommand._title = "getCommand";
            this.getCommand.AutoSize = true;
            this.getCommand.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getCommand.CanBeEmpty = false;
            this.getCommand.Location = new System.Drawing.Point(0, 27);
            this.getCommand.Margin = new System.Windows.Forms.Padding(0);
            this.getCommand.Name = "getCommand";
            this.getCommand.Size = new System.Drawing.Size(303, 24);
            this.getCommand.TabIndex = 0;
            this.getCommand.ValueOnlyFromList = false;
            // 
            // plcVariable
            // 
            this.plcVariable._title = "plcVariable";
            this.plcVariable.AutoSize = true;
            this.plcVariable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.plcVariable.CanBeEmpty = false;
            this.plcVariable.Location = new System.Drawing.Point(0, 51);
            this.plcVariable.Margin = new System.Windows.Forms.Padding(0);
            this.plcVariable.Name = "plcVariable";
            this.plcVariable.Size = new System.Drawing.Size(303, 24);
            this.plcVariable.TabIndex = 0;
            this.plcVariable.ValueOnlyFromList = false;
            // 
            // DeltaCommandGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DeltaCommandGUI";
            this.Size = new System.Drawing.Size(342, 271);
            this.VisibleChanged += new System.EventHandler(this.DeltaCommandGUI_VisibleChanged);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.setterFlow.ResumeLayout(false);
            this.setterFlow.PerformLayout();
            this.setBlock.ResumeLayout(false);
            this.setBlock.PerformLayout();
            this.getterFlow.ResumeLayout(false);
            this.getterFlow.PerformLayout();
            this.getBlock.ResumeLayout(false);
            this.getBlock.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Base.StringParameterField setCommandSelector;
        private Base.StringParameterField setCommand;
        private Base.StringParameterField value;
        private Base.GUI.tabs tabs1;
        private System.Windows.Forms.FlowLayoutPanel setterFlow;
        private System.Windows.Forms.FlowLayoutPanel getterFlow;
        private Base.StringParameterField getCommandSelector;
        private Base.StringParameterField getCommand;
        private Base.StringParameterField plcVariable;
        private Base.CheckBoolParameterField setEnabled;
        private System.Windows.Forms.FlowLayoutPanel setBlock;
        private Base.CheckBoolParameterField getEnabled;
        private System.Windows.Forms.FlowLayoutPanel getBlock;
        private Base.GUI.BoolParameterField boolValue;
    }
}
