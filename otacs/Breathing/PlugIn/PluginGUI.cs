using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PlugIn
{
    //////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PluginGUI : UserControl
    //////////////////////////////////////////////////////////////////////////////////////////////
    {
        //////////////////////////////////////////////////////////////////////////////////////////////
        //Members
        //////////////////////////////////////////////////////////////////////////////////////////////
            Plugin plugin;

        #region private members

            private float _breathRate;
            private float _breathTemperature;
            private float _breathApnea;

        #endregion

        #region public properties

            public float BreathingRate
            {
                set
                {
                    if (value > 0)
                        _breathRate = value;
                    else
                        _breathRate = 0;
                }

                //get
                //{
                //    return _breathRate;
                //}
            }

            public float BreathTemperature
            {
                set
                {
                    if (value > 0)
                        _breathTemperature = value;
                    else
                        _breathTemperature = 0;
                }

                //get
                //{
                //    return _breathTemperature;
                //}
            }

            public float BreathApnea
            {
                set
                {
                    if (value > 0)
                        _breathApnea = value;
                    else
                        _breathApnea = 0;
                }

                //get
                //{
                //    return _breathApnea;
                //}
            }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////
        public PluginGUI(Plugin setPlugin)
        //////////////////////////////////////////////////////////////////////////////////////////////
        {
            InitializeComponent();

            plugin = setPlugin;

            plugin.StatusChange += new PluginStatusDelegate(plugin_StatusChange);

        }


        delegate void StringDelegate(string text);

        //////////////////////////////////////////////////////////////////////////////////////////////
        void plugin_StatusChange(string status)
        //////////////////////////////////////////////////////////////////////////////////////////////
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(plugin_StatusChange), status);
                return;
            }

            //update breathing rate
            if (_breathRate > 0)
                rateLabel.Text = _breathRate.ToString("0");
                //rateLabel.Text = _breathRate.ToString();
            else
                rateLabel.Text = "";

            //update breathing temperature
            if (_breathTemperature > 0) 
                temperatureLabel.Text = _breathTemperature.ToString();
            else
                temperatureLabel.Text = "";

            //update breathing temperature
            if (_breathApnea > 0)
                ApneaLabel.Text = _breathApnea.ToString();
            else
                ApneaLabel.Text = "";

            
            //update status bar
            statusLabel.Text = status;

            //refresh the labels
            rateLabel.Refresh();
            temperatureLabel.Refresh();
            statusLabel.Refresh();

        }

    }
}
