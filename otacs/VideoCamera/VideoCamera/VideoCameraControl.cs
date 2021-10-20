
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;

namespace VideoCameraPlugin
{


    public partial class VideoCameraControl : UserControl
    {
        
        VideoCamera plugin;

        public string file_name = null;

        public bool record = false;

        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;

        VideoFileWriter writer = new VideoFileWriter();
        int iWidth = 0;
        int iHeight = 0;
        int totalFramesLastSec = 0;
        // Camera choice
        int deviceFrameRate = 0, aviFrameRate = 0;

        
        Bitmap addImg;

        DateTime startTimeRecording;

        public VideoCameraControl()
        {
            InitializeComponent();
        }

        public VideoCameraControl(VideoCamera setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;

            plugin.ProcessingStateChanged += new BoolDelegate(plugin_ProcessingStateChanged);
            plugin.FileNameChanged += new StringDelegate(plugin_FileNameChanged);

            
        }

        

        void plugin_ProcessingStateChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(plugin_ProcessingStateChanged), value);
                return;

            }

            record = value;
            if(record)
            {
                btnStartStopRecord.Enabled = false;
            }
            else
            {
                btnStartStopRecord.Enabled = true;
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

        private void VideoCameraControl_Load(object sender, System.EventArgs e)
        {
            // Fill camera list combobox with available cameras
            FillCameraList();

            if (plugin.AviNum != null)
            {
                comboBoxAVINumber.Text = plugin.AviNum;
            }

            this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
            
        }

        #region Camera and resolution selection

        private void FillCameraList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                comboBoxCameraList.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                int cameraIndex = -1;
                for(int i= 0; i< videoDevices.Count; i++)
                {
                    FilterInfo device = videoDevices[i];
                    comboBoxCameraList.Items.Add(device.Name);
                    if(plugin.DeviceName != null)
                    {
                        if (device.Name.Contains(plugin.DeviceName))
                        {
                            cameraIndex = i;
                        }
                    }
                    
                }

                if(cameraIndex != -1)
                {
                    comboBoxCameraList.SelectedIndex = cameraIndex; //make dafault
                }

                


            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                comboBoxCameraList.Items.Add("No capture device on your system");
            }

        }

        private void ConnectCamera(int index)
        {
            if (DeviceExist)
            {
                videoSource = new VideoCaptureDevice(videoDevices[index].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                CloseVideoSource();
                //videoSource.DesiredFrameSize = new Size(1024, 120);
                //videoSource.DesiredFrameRate = 10;
                videoSource.Start();

                timerRecord.Start();

                
            }
        }

        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = img;
            if((iWidth == 0) || (iHeight == 0))
            {
                iWidth = img.Width;
                iHeight = img.Height;
            }

            if(record && writer != null)
            {


                Bitmap img2 = (Bitmap)eventArgs.Frame.Clone();
                //Bitmap img2 = img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
                try
                {
                    writer.WriteVideoFrame(img2);
                    totalFramesLastSec++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "VideoCameraControl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                

                /*
                if(totalFramesLastSec < aviFrameRate)
                {
                    Bitmap lastImg = (Bitmap)eventArgs.Frame.Clone();
                    //addImg = (Bitmap)lastImg.Clone();

                    writer.WriteVideoFrame(lastImg);
                    totalFramesLastSec++;
                }
                else
                {
                    Console.WriteLine("Extra frame detected in visual video recording");
                }
                */
            }

        }

        private void comboBoxCameraList_SelectedIndexChanged(object sender, EventArgs e)
        {
            iWidth = 0;
            iHeight = 0;

            CloseVideoSource();
            ConnectCamera(comboBoxCameraList.SelectedIndex);
            plugin.DeviceName = comboBoxCameraList.SelectedItem.ToString();
        }

        

        #endregion

        private void timerRecord_Tick(object sender, EventArgs e)
        {
            if(videoSource != null)
            {
                if(aviFrameRate == 0)
                {
                    deviceFrameRate = videoSource.FramesReceived;
                    lblFrameRate.Text = deviceFrameRate.ToString() + " fps";
                }
                else
                {
                    lblFramesLastSec.Text = totalFramesLastSec.ToString();

                    /*
                    if(totalFramesLastSec < aviFrameRate)
                    {
                        while(totalFramesLastSec < aviFrameRate)
                        {
                            
                            //Bitmap addImg = (Bitmap)lastImg.Clone();
                            //writer.WriteVideoFrame(addImg);
                            Console.WriteLine("Missing frame added in visual video recording");
                            totalFramesLastSec++;
                        }
                    }
                    */
                    totalFramesLastSec = 0;
                }
            }
        }

        private void btnStartStopRecord_Click(object sender, EventArgs e)
        {   
            record = !record;
            StartStopRecording();
        }

        private void StartStopRecording()
        {
            
            if(record)
            {
                string aviNum = "avi1";
                if (plugin.AviNum != null)
                {
                    aviNum = plugin.AviNum;
                }

                if (file_name == null)
                {
                    file_name = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                    + "\\VisualVideo." + aviNum + ".avi";

                    //file_name = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                    //+ "\\VisualVideo.mp4";
                }
                else
                {
                    if(!file_name.Contains(".avi"))
                    {
                        //file_name += ".avi1.avi";
                        file_name += "." + aviNum + ".avi";
                    }    
                }
                if (System.IO.File.Exists(file_name))
                {
                    System.IO.File.Delete(file_name);
                }

                if (iWidth != 0 && iHeight != 0)
                {
                    totalFramesLastSec = 0;

                    aviFrameRate = deviceFrameRate;
                    writer.Open(file_name, iWidth, iHeight, aviFrameRate, VideoCodec.MPEG4);
                    btnStartStopRecord.Text = "Stop Recording...";

                    startTimeRecording = DateTime.Now;
                    //comboBoxCameraList.Enabled = false;
                    //btnCameraSettings.Enabled = false;
                    //timerRecord.Stop();
                }

            }
            else
            {
                btnStartStopRecord.Text = "Start Recording";
                plugin.NotifyOutputPin();

                writer.Close();

                DateTime stopTimeRecording = DateTime.Now;
                double totalRecordingSecs = (stopTimeRecording.Subtract(startTimeRecording)).TotalSeconds;
                lblRecordTime.Text = totalRecordingSecs.ToString("0.00");
                //comboBoxCameraList.Enabled = true;
                //btnCameraSettings.Enabled = true;
            }
            
        }

        

        

        

        private void btnCameraSettings_Click(object sender, EventArgs e)
        {
            if (videoSource != null)
            {
                videoSource.DisplayPropertyPage(IntPtr.Zero);
            }
        }

        private void VideoCameraControl_Leave(object sender, EventArgs e)
        {
            //CloseVideoSource();
        }

        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVideoSource();
        }

        private void comboBoxAVINumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            string aviNum = comboBoxAVINumber.SelectedItem.ToString();
            plugin.AviNum = aviNum;
        }
    }
}
