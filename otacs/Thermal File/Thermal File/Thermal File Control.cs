using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Thermal_File
{
    public partial class Thermal_File_Control : UserControl
    {
        ThermalFile plugin;

        public Thermal_File_Control()
        {
            InitializeComponent();
        }

        public Thermal_File_Control(ThermalFile setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;

            //listen to the plugin events
            plugin.TotalFramesChanged += new IntDelegate(OnTotalFramesChanged);
            plugin.CurFrameNumChanged += new IntDelegate(OnCurFrameNumChanged);
            plugin.TotalLengthChanged += new StringDelegate(OnTotalLengthChanged);
            plugin.CurTimeStampChanged += new StringDelegate(OnCurTimeStampChanged);
            }


        

        
        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnCurTimeStampChanged(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnCurTimeStampChanged), value);
                return;
            }

            //display the value
            lblSeconds.Visible = true;
            lblCurTimeStamp.Text = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnTotalLengthChanged(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnTotalLengthChanged), value);
                return;
            }

            //display the value
            lblLength.Text = value;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnTotalFramesChanged(int value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new IntDelegate(OnTotalFramesChanged), value);
                return;
            }

            //display the value
            lblTotalFrames.Text = value.ToString();
            trackBarThermalVideo.Minimum = 0;
            trackBarThermalVideo.Maximum = value;
            //trackBarThermalVideo.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnCurFrameNumChanged(int value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new IntDelegate(OnCurFrameNumChanged), value);
                return;
            }

            //display the value
            lblCurFrameNum.Text = value.ToString();
            trackBarThermalVideo.Value = value;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            //open file dialog with options
            dialog.Filter = "Thermal Data (*.dat;*.sfmov)|*.dat;*.sfmov|" +
                                            "All files (*.*)|*.*";
            dialog.Title = "Select a thermal data file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                lblFileName.Text = dialog.FileName;
                if (dialog.FileName.EndsWith("dat"))
                {
                    plugin.CurSelection = 0;
                    plugin.ReadDatFiles(dialog.FileName);
                }
                else if (dialog.FileName.EndsWith("sfmov"))
                {
                    plugin.CurSelection = 1;
                    plugin.ReadSFMOVFile(dialog.FileName);
                }
                else
                {
                    MessageBox.Show("Please select a thermal data file of type [*.dat] or [*.sfmov]", "Wrong thermal data file",
                                                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            plugin.Play = !plugin.Play;
        }

        private void trackBarThermalVideo_Scroll(object sender, EventArgs e)
        {
            plugin.CurFrameNum = (int)trackBarThermalVideo.Value;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            plugin.CurFrameNum++;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            plugin.CurFrameNum--;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            plugin.CurFrameNum = plugin.MyTotFrames - 1;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            plugin.CurFrameNum = 0;
        }

        private void radButtonDatlFile_Click(object sender, EventArgs e)
        {
            plugin.CurSelection = 0;
        }

        private void radButtonSFMOV_Click(object sender, EventArgs e)
        {
            plugin.CurSelection = 1;
        }
    }
}
