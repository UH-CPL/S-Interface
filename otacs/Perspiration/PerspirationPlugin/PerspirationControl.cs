using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PerspirationPlugin
{
    public partial class PerspirationControl : UserControl
    {
        Perspiration plugin;

        public PerspirationControl()
        {
            InitializeComponent();
        }

        public PerspirationControl(Perspiration setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;

            // Listen to the delegate
            plugin.normalizedEnergyChanged += new BoolDelegate(OnnormalizedEnergyChanged);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnnormalizedEnergyChanged(bool value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(OnnormalizedEnergyChanged), value);
                return;
            }

            //Update the checkbox 
            if (value)
            {
                checkBoxNormEnergy.Checked = true;
                checkBoxNormEnergy.CheckState = CheckState.Checked;
            }
            else
            {
                checkBoxNormEnergy.Checked = false;
                checkBoxNormEnergy.CheckState = CheckState.Unchecked;
            }

        }

        private void checkBoxNormEnergy_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNormEnergy.Checked)
            { plugin.normalizedEnergyProperty = true; }
            else
            { plugin.normalizedEnergyProperty = false; }

        }

    }
}
