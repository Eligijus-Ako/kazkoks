namespace DeltaPlugin
{
    partial class DeltaPluginSettingsGUI
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
            this.enabled = new Base.CheckBoolParameterField();
            this.settingsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.pollTime = new Base.DoubleParameterField();
            this.headerGroupBox1 = new Base.HeaderGroupBox();
            this.logFilename = new Base.StringParameterField();
            this.browseButton = new Base.NoSelectButton();
            this.logMethod = new Base.GUI.BoolParameterField();
            this.logInterval = new Base.DoubleParameterField();
            this.flowLayoutPanel1.SuspendLayout();
            this.settingsFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.enabled);
            this.flowLayoutPanel1.Controls.Add(this.settingsFlow);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(312, 175);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // enabled
            // 
            this.enabled._title = "enabled";
            this.enabled.AutoSize = true;
            this.enabled.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enabled.Location = new System.Drawing.Point(3, 3);
            this.enabled.Name = "enabled";
            this.enabled.Size = new System.Drawing.Size(306, 23);
            this.enabled.TabIndex = 0;
            this.enabled.Value = false;
            this.enabled.ParameterFieldValueChanged += new Base.GUI.ParameterFieldValueChangedDelegate(this.Enabled_ParameterFieldValueChanged);
            // 
            // settingsFlow
            // 
            this.settingsFlow.AutoSize = true;
            this.settingsFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsFlow.Controls.Add(this.pollTime);
            this.settingsFlow.Controls.Add(this.headerGroupBox1);
            this.settingsFlow.Controls.Add(this.logFilename);
            this.settingsFlow.Controls.Add(this.browseButton);
            this.settingsFlow.Controls.Add(this.logMethod);
            this.settingsFlow.Controls.Add(this.logInterval);
            this.settingsFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.settingsFlow.Location = new System.Drawing.Point(0, 29);
            this.settingsFlow.Margin = new System.Windows.Forms.Padding(0);
            this.settingsFlow.Name = "settingsFlow";
            this.settingsFlow.Size = new System.Drawing.Size(312, 146);
            this.settingsFlow.TabIndex = 1;
            // 
            // pollTime
            // 
            this.pollTime._title = "pollTime";
            this.pollTime.AutoSize = true;
            this.pollTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pollTime.DecimalPlaces = 0;
            this.pollTime.Location = new System.Drawing.Point(0, 0);
            this.pollTime.Margin = new System.Windows.Forms.Padding(0);
            this.pollTime.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.pollTime.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.pollTime.Name = "pollTime";
            this.pollTime.Size = new System.Drawing.Size(303, 23);
            this.pollTime.TabIndex = 2;
            this.pollTime.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // headerGroupBox1
            // 
            this.headerGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.headerGroupBox1.Location = new System.Drawing.Point(3, 26);
            this.headerGroupBox1.Name = "headerGroupBox1";
            this.headerGroupBox1.Size = new System.Drawing.Size(300, 19);
            this.headerGroupBox1.TabIndex = 3;
            this.headerGroupBox1.TabStop = false;
            this.headerGroupBox1.Text = "Logging";
            // 
            // logFilename
            // 
            this.logFilename._title = "logFilename";
            this.logFilename.AutoSize = true;
            this.logFilename.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.logFilename.CanBeEmpty = false;
            this.logFilename.Location = new System.Drawing.Point(0, 48);
            this.logFilename.Margin = new System.Windows.Forms.Padding(0);
            this.logFilename.Name = "logFilename";
            this.logFilename.Size = new System.Drawing.Size(303, 24);
            this.logFilename.TabIndex = 4;
            this.logFilename.ValueOnlyFromList = false;
            // 
            // browseButton
            // 
            this.browseButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.browseButton.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.browseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(181)))));
            this.browseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Location = new System.Drawing.Point(224, 75);
            this.browseButton.Margin = new System.Windows.Forms.Padding(3, 3, 13, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 7;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // logMethod
            // 
            this.logMethod._title = "logMethod";
            this.logMethod._value_title1 = "On Update";
            this.logMethod._value_title2 = "At Interval";
            this.logMethod.AutoSize = true;
            this.logMethod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.logMethod.IsCheckedFirst = false;
            this.logMethod.IsCheckedSecond = false;
            this.logMethod.Location = new System.Drawing.Point(0, 101);
            this.logMethod.Margin = new System.Windows.Forms.Padding(0);
            this.logMethod.Name = "logMethod";
            this.logMethod.Shift = 0;
            this.logMethod.Shift2 = 0;
            this.logMethod.Size = new System.Drawing.Size(312, 22);
            this.logMethod.TabIndex = 5;
            this.logMethod.ParameterFieldValueChanged += new Base.GUI.ParameterFieldValueChangedDelegate(this.LogMethod_ParameterFieldValueChanged);
            // 
            // logInterval
            // 
            this.logInterval._title = "logInterval";
            this.logInterval.AutoSize = true;
            this.logInterval.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.logInterval.DecimalPlaces = 0;
            this.logInterval.Location = new System.Drawing.Point(0, 123);
            this.logInterval.Margin = new System.Windows.Forms.Padding(0);
            this.logInterval.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.logInterval.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.logInterval.Name = "logInterval";
            this.logInterval.Size = new System.Drawing.Size(303, 23);
            this.logInterval.TabIndex = 6;
            this.logInterval.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // DeltaPluginSettingsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DeltaPluginSettingsGUI";
            this.Size = new System.Drawing.Size(315, 178);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.settingsFlow.ResumeLayout(false);
            this.settingsFlow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Base.CheckBoolParameterField enabled;
        private System.Windows.Forms.FlowLayoutPanel settingsFlow;
        private Base.DoubleParameterField pollTime;
        private Base.HeaderGroupBox headerGroupBox1;
        private Base.StringParameterField logFilename;
        private Base.NoSelectButton browseButton;
        private Base.GUI.BoolParameterField logMethod;
        private Base.DoubleParameterField logInterval;
    }
}
