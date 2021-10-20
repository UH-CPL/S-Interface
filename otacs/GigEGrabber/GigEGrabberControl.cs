using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PvDotNet;
using PvGUIDotNet;

namespace GigEGrabber
{
    public partial class GigEGrabberControl : UserControl
    {

        GigEGrabberPlugin plugin;
        string folderPath;
        string fileName;

        public GigEGrabberControl()
        {
            InitializeComponent();
        }

        public GigEGrabberControl(GigEGrabberPlugin setplugin)
        {
            InitializeComponent();

            plugin = setplugin;
            plugin.CameraInterfaceChanged += new StringDelegate(OnCameraInterfaceChanged);
            plugin.FrameNumChanged += new StringDelegate(OnFrameNumChanged);
            plugin.TimestampChanged += new StringDelegate(OnTimestampChanged);
            plugin.RecordTriggerChanged += new IntegerDelegate(OnRecordTriggerChanged);

            //folderPath = System.Environment.GetFolderPath(
                    //System.Environment.SpecialFolder.Personal);
            folderPath = "C:\\SIM_Study";
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            plugin.MyFileName = folderPath + "\\" + fileName;

            plugin.FileNameChanged += new StringDelegate(OnFileNameChanged);
            //plugin.ImageAvailable += new Action<Bitmap>(plugin_ImageAvailable);

            textBoxPath.Text = folderPath;
            textBoxFileName.Text = fileName + ".dat";

            btnStartStopRecording.Text = "Start Recording";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnRecordTriggerChanged(int value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new IntegerDelegate(OnRecordTriggerChanged), value);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;

            if(value == 1)
            {
                btnStartStopRecording.Enabled = false;
                btnStartStopRecording.Text = "Stop Recording...";
            }
            else
            {
                btnStartStopRecording.Enabled = true;
                btnStartStopRecording.Text = "Start Recording";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnFileNameChanged(string name)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnFileNameChanged), name);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;

            fileName = name;
            textBoxFileName.Text = fileName + ".dat";
            plugin.MyFileName = folderPath + "\\" + fileName;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnFrameNumChanged(string name)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnFrameNumChanged), name);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;

            lblFrameNum.Text = name;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnTimestampChanged(string name)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnTimestampChanged), name);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;

            lblTimestamp.Text = name;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnCameraInterfaceChanged(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnCameraInterfaceChanged), value);
                return;
            }

            //display the value
            lblCurrentInterface.Text = value;

        }

        private void btnUpdateDevice_Click(object sender, EventArgs e)
        {
            // Pop device finder, let user select a device.
            PvDeviceFinderForm lForm = new PvDeviceFinderForm();
            if ((lForm.ShowDialog() == DialogResult.OK) && (lForm.Selected != null))
            {
                plugin.LDeviceInfo = lForm.Selected as PvDeviceInfo;
                lblCurrentInterface.Text = plugin.LDeviceInfo.DisplayID;
            }
        }

        private void btnBrowseFolderPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    folderPath = fbd.SelectedPath;
                    textBoxPath.Text = folderPath;

                    plugin.MyFileName = folderPath + "\\" + fileName;
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (plugin.LDeviceInfo != null)
                plugin.StartStopStreaming();
        }

        private void btnStartStopRecording_Click(object sender, EventArgs e)
        {
            plugin.IsRecording = !plugin.IsRecording;

            if (plugin.IsRecording)
            {
                btnStartStopRecording.Text = "Stop Recording...";
            }
            else
            {
                btnStartStopRecording.Text = "Start Recording";
            }

            plugin.StartStopRecording();
        }
    }
}
