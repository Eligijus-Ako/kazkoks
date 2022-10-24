namespace DeltaPlugin
{
    partial class InfoItem
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
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.textLabel = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.log = new System.Windows.Forms.CheckBox();
            this.ribbon = new System.Windows.Forms.CheckBox();
            this.flow.SuspendLayout();
            this.SuspendLayout();
            // 
            // flow
            // 
            this.flow.AutoSize = true;
            this.flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flow.Controls.Add(this.textLabel);
            this.flow.Controls.Add(this.valueLabel);
            this.flow.Controls.Add(this.log);
            this.flow.Controls.Add(this.ribbon);
            this.flow.Location = new System.Drawing.Point(0, 0);
            this.flow.Margin = new System.Windows.Forms.Padding(0);
            this.flow.Name = "flow";
            this.flow.Size = new System.Drawing.Size(327, 23);
            this.flow.TabIndex = 0;
            // 
            // textLabel
            // 
            this.textLabel.Location = new System.Drawing.Point(3, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(150, 23);
            this.textLabel.TabIndex = 0;
            this.textLabel.Text = "Text";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueLabel
            // 
            this.valueLabel.Location = new System.Drawing.Point(159, 0);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(75, 23);
            this.valueLabel.TabIndex = 1;
            this.valueLabel.Text = "Value";
            this.valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log
            // 
            this.log.AutoSize = true;
            this.log.Location = new System.Drawing.Point(252, 3);
            this.log.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(15, 14);
            this.log.TabIndex = 2;
            this.log.UseVisualStyleBackColor = true;
            this.log.CheckedChanged += new System.EventHandler(this.Log_CheckedChanged);
            // 
            // ribbon
            // 
            this.ribbon.AutoSize = true;
            this.ribbon.Location = new System.Drawing.Point(297, 3);
            this.ribbon.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(15, 14);
            this.ribbon.TabIndex = 3;
            this.ribbon.UseVisualStyleBackColor = true;
            this.ribbon.CheckedChanged += new System.EventHandler(this.Ribbon_CheckedChanged);
            // 
            // InfoItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flow);
            this.Name = "InfoItem";
            this.Size = new System.Drawing.Size(327, 23);
            this.flow.ResumeLayout(false);
            this.flow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flow;
        private System.Windows.Forms.Label textLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.CheckBox log;
        private System.Windows.Forms.CheckBox ribbon;
    }
}
