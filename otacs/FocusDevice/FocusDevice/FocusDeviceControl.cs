using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FocusDevice
{
    public partial class FocusDeviceControl : UserControl
    {
        FocusDevicePlugin plugin;
        public FocusDeviceControl()
        {
            InitializeComponent();
        }

        public FocusDeviceControl(FocusDevicePlugin setplugin)
        {
            InitializeComponent();
            trackBarFocusPos.Maximum = 100;
            trackBarFocusPos.Minimum = 0;
            
            plugin = setplugin;
            plugin.FocusComm.PositionChanged += new FloatDelegate(OnFocusPositionChanged);
            plugin.ConnectedPortChanged += new IntegerDelegate(OnConnectedPortChanged);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnFocusPositionChanged(float value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new FloatDelegate(OnFocusPositionChanged), value);
                return;
            }

            //display the value
            trackBarFocusPos.Value = (int)(value);
            plugin.IsChangingFocus = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnConnectedPortChanged(int value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new IntegerDelegate(OnConnectedPortChanged), value);
                return;
            }

            //display the value
            comboBoxPorts.SelectedIndex = value-1;
            ConnectFocusDevice(value);
        }

        void ConnectFocusDevice(int selectedPort)
        {
            try
            {
                plugin.FocusComm.Initialize(selectedPort);
                lblStatus.Text = "Connected to focus device on COM" + selectedPort.ToString();
                plugin.ConnectedPort = selectedPort;
            }
            catch (Exception exc)
            {
                lblStatus.Text = exc.Message;
                plugin.ConnectedPort = -1;
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectFocusDevice(comboBoxPorts.SelectedIndex + 1);
            
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            plugin.FocusComm.FocusOut();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            plugin.FocusComm.FocusIn();
            //plugin.FocusComm.GotoPosition(100);
        }

        private void trackBarFocusPos_Scroll(object sender, EventArgs e)
        {
            //Console.WriteLine(trackBarFocusPos.Value);
            plugin.FocusComm.GotoPosition((double)trackBarFocusPos.Value);
        }

        private void AutoFocusButton_Click(object sender, EventArgs e)
        {
            plugin.IsAutoFocusing = true;
            //plugin.FocusComm.GotoPosition(20);
        }
    }
}
