using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeltaPlugin
{
    public partial class RibbonItem : UserControl
    {
        public string ItemName
        {
            get { return nameLabel.Text; }
            set { nameLabel.Text = value; }
        }

        public string Value
        {
            get { return stateLabel.Text; }
            set { stateLabel.Text = value; }
        }

        public Color ValueBackgroundColor
        {
            get { return stateLabel.BackColor; }
            set { stateLabel.BackColor = value; }
        }

        public RibbonItem()
        {
            InitializeComponent();
            stateLabel.ForeColor = Color.Black;
            nameLabel.ForeColor = Color.Black;
            Base.Functions.FixForm(this);
        }
    }
}
