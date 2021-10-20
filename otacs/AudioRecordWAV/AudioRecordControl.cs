using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using System.ComponentModel.Composition;
using System.IO;

namespace AudioRecordWAV
{
    public partial class AudioRecordControl : UserControl
    {
        AudioRecordPlugin plugin;
        public string file_name = null;
        public bool record = false;

        private IWaveIn waveIn;
        private WaveFileWriter writer;

        public AudioRecordControl()
        {
            InitializeComponent();
        }

        public AudioRecordControl(AudioRecordPlugin setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;

            plugin.ProcessingStateChanged += new BoolDelegate(plugin_ProcessingStateChanged);
            plugin.FileNameChanged += new StringDelegate(plugin_FileNameChanged);

            /*
            comboWasapiDevices.SelectedIndexChanged -= comboWasapiDevices_SelectedIndexChanged;
            LoadWasapiDevicesCombo();
            comboWasapiDevices.SelectedIndexChanged += comboWasapiDevices_SelectedIndexChanged;
            */
            

            
        }

        void plugin_ProcessingStateChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(plugin_ProcessingStateChanged), value);
                return;

            }

            record = value;
            if (record)
            {
                btnRecordStartStop.Enabled = false;
            }
            else
            {
                btnRecordStartStop.Enabled = true;
            }
            StartStopRecording();
        }

        void plugin_FileNameChanged(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(plugin_FileNameChanged), value);
                return;
            }

            file_name = value;

        }

        void StartStopRecording()
        {
            if (record)
            {
                Cleanup();

                if (waveIn == null)
                {
                    CreateWaveInDevice();
                }

                string outputFileName = null;
                if(file_name == null)
                {
                    outputFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\AudioFile.wav"; 
                }
                else
                {
                    outputFileName = file_name + ".wav";
                }
                writer = new WaveFileWriter(outputFileName, waveIn.WaveFormat);
                waveIn.StartRecording();

                btnRecordStartStop.Text = "Stop Recording...";
            }
            else
            {
                if (waveIn != null) waveIn.StopRecording();
                btnRecordStartStop.Text = "Start Recording";
            }
        }

        private void LoadWasapiDevicesCombo()
        {
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();

            comboWasapiDevices.DataSource = devices;
            comboWasapiDevices.DisplayMember = "FriendlyName";

            if(plugin.DeviceName != null)
            {
                int index = 0;
                for(int i=0; i< devices.Count; i++)
                {
                    MMDevice device = devices[i];
                    if(device.FriendlyName == plugin.DeviceName)
                    {
                        index = i;
                        break;
                    }
                }
                comboWasapiDevices.SelectedIndex = index;
            }
        }

        private void CreateWaveInDevice()
        {
            /*
            var device = (MMDevice)comboWasapiDevices.SelectedItem;
            waveIn = new WasapiCapture(device);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            */

            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(8000, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
        }

        private void Cleanup()
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
            FinalizeWaveFile();
        }

        private void FinalizeWaveFile()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Debug.WriteLine("Data Available");
                this.BeginInvoke(new EventHandler<WaveInEventArgs>(OnDataAvailable), sender, e);
            }
            else
            {
                //Debug.WriteLine("Flushing Data Available");
                if(writer != null)
                {
                    writer.Write(e.Buffer, 0, e.BytesRecorded);
                }
                
                //int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);
            }
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
            }
            else
            {
                FinalizeWaveFile();
                plugin.NotifyOutputPin();
                if (e.Exception != null)
                {
                    MessageBox.Show(String.Format("A problem was encountered during audio recording {0}",
                                                  e.Exception.Message));
                }
            }
        }

        private void btnRecordStartStop_Click(object sender, EventArgs e)
        {
            record = !record;
            StartStopRecording();
        }

        private void comboWasapiDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var device = (MMDevice)comboWasapiDevices.SelectedItem;
            string deviceName = device.FriendlyName;
            plugin.DeviceName = deviceName;
        }
    }
}
