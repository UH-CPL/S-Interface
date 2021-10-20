using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PerformanceData
{
    public partial class PerformanceDataControl : UserControl
    {
        PeformanceDataPlugin plugin;
        private WaitForm waitForm = null;
        DateTime startTime;
        uint CanIF = 0;
        char[] Initstring = "ED000200 ; 500".ToCharArray();
        private string canalMsgData = "";
        public static CanalMsg RxMsg = new CanalMsg();

        public PerformanceDataControl()
        {
            InitializeComponent();
        }

        public PerformanceDataControl(PeformanceDataPlugin setplugin)
        {
            InitializeComponent();

            plugin = setplugin;
            plugin.RecordTriggerChanged += new IntegerDelegate(OnRecordTriggerChanged);

            plugin.FileNameChanged += new StringDelegate(OnFileNameChanged);
            plugin.ShowHideWaitForm += new BoolDelegate(OnShowHideWaitForm);

            btnStartStopRecording.Text = "Start Recording";

            ConnectToCAN();
        }

        void OnShowHideWaitForm(bool show)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(OnShowHideWaitForm), show);
                return;
            }

            if (show)
            {
                waitForm = new WaitForm();
                waitForm.lblMessage.Text = "Writing performance data...";
                waitForm.Show();
            }
            else
            {
                waitForm.Close();
                waitForm = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region USB2CAN Communication
        ///////////////////////////////////////////////////////////////////////////////////////////

        void ConnectToCAN()
        {
            textBoxDisplay.Text = "Start";
            if ((CanIF = usb2can.CanalOpen(Initstring, 0)) <= 0)
            {
                textBoxDisplay.AppendText(Environment.NewLine);
                textBoxDisplay.AppendText("Failed to open USB2CAN interface.");
                CanIF = 0;
            }
            else
            {
                textBoxDisplay.AppendText(Environment.NewLine);
                textBoxDisplay.AppendText("Init O.K. :" + CanIF.ToString());
            }
        }

        void ReceiveCANData()
        {
            int Err = usb2can.CanalDataAvailable(CanIF);
            textBoxDisplay.AppendText(Environment.NewLine);
            textBoxDisplay.Text = "Rx Count :" + Err.ToString();
            if (Err > 0)
            {
                DateTime curTime = DateTime.Now;
                float timeRecording = (float)(curTime.Subtract(startTime)).TotalSeconds;
                var dataPoint = new PerformanceDataPoint();
                dataPoint.time = timeRecording;
                while (usb2can.CanalDataAvailable(CanIF) > 0)
                {
                    usb2can.CanalReceive(CanIF, ref RxMsg);
                    textBoxDisplay.AppendText(Environment.NewLine);
                    string strData = "";
                    strData += timeRecording.ToString() + "," + RxMsg.id.ToString() + ",";
                    //strData += string.Format(" {0:X2}", RxMsg.data0);
                    //strData += string.Format(" {0:X2}", RxMsg.data1);
                    //strData += string.Format(" {0:X2}", RxMsg.data2);
                    //strData += string.Format(" {0:X2}", RxMsg.data3);
                    //strData += string.Format(" {0:X2}", RxMsg.data4);
                    //strData += string.Format(" {0:X2}", RxMsg.data5);
                    //strData += string.Format(" {0:X2}", RxMsg.data6);
                    //strData += string.Format(" {0:X2}", RxMsg.data7);

                    if (RxMsg.id == (uint)PerformanceDataID.GPS + 1)
                    {
                        string latHexString = BytesToString(RxMsg.data0, RxMsg.data1, RxMsg.data2, RxMsg.data3);
                        float lat = HexToFloat(latHexString);
                        string lonHexString = BytesToString(RxMsg.data4, RxMsg.data5, RxMsg.data6, RxMsg.data7);
                        float lon = HexToFloat(lonHexString);
                        strData += latHexString + "," + lonHexString;
                        dataPoint.latitude = lat;
                        dataPoint.longitude = lon;
                    }
                    else if (RxMsg.id == (uint)PerformanceDataID.SteeringAcceleration + 1)
                    {
                        string steeringString = BytesToString(RxMsg.data0, RxMsg.data1, RxMsg.data2, RxMsg.data3);
                        float steering = HexToFloat(steeringString);
                        string accelerationString = BytesToString(RxMsg.data4, RxMsg.data5, RxMsg.data6, RxMsg.data7);
                        float acceleration = HexToFloat(accelerationString);
                        dataPoint.steering = steering;
                        dataPoint.acceleration = acceleration;
                        strData += steeringString + "," + accelerationString;
                    }
                    else if (RxMsg.id == (uint)PerformanceDataID.BrakeSpeed + 1)
                    {
                        string brakeString = BytesToString(RxMsg.data0, RxMsg.data1, RxMsg.data2, RxMsg.data3);
                        float brake = HexToFloat(brakeString);
                        string speedString = BytesToString(RxMsg.data4, RxMsg.data5, RxMsg.data6, RxMsg.data7);
                        float speed = HexToFloat(speedString);
                        dataPoint.brake = brake;
                        dataPoint.speed = speed;
                        strData += brakeString + "," + speedString;
                    }
                    textBoxDisplay.AppendText(strData);
                    //canalMsgData += strData + Environment.NewLine;
                }
                // plugin.AddCANData(RxMsg);
                plugin.AddPerformanceData(dataPoint);
            }
        }

        private float HexToFloat(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] bytes = BitConverter.GetBytes(num);
            float myFloat = BitConverter.ToSingle(bytes, 0);
            return myFloat;
        }

        private string BytesToString(byte byte1, byte byte2, byte byte3, byte byte4)
        {
            string hexString = string.Format(" {0:X2}", byte1) + string.Format(" {0:X2}", byte2) + string.Format(" {0:X2}", byte3) + string.Format(" {0:X2}", byte4);
            hexString = Regex.Replace(hexString, @"\s+", "");
            return hexString;
        }

        void CloseCANConnection()
        {
            if (CanIF > 0)
            {
                usb2can.CanalClose(CanIF);
            }
            CanIF = 0;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

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

            if (value == 1)
            {
                btnStartStopRecording.Enabled = false;
                btnStartStopRecording.Text = "Stop Recording...";
            }
            else
            {
                btnStartStopRecording.Enabled = true;
                btnStartStopRecording.Text = "Start Recording";
            }
            StartStopRecording();
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
            textBoxFileName.Text = name;
        }

        private void StartStopRecording()
        {
            //plugin.AddRawPerformanceData(canalMsgData);
            plugin.StartStopRecording();
            if (plugin.IsRecording)
            {
                if(CanIF <= 0)
                {
                    ConnectToCAN();
                }
                btnStartStopRecording.Text = "Stop Recording...";
                startTime = DateTime.Now;
                canalMsgData = "";
                timerCAN.Start();
            }
            else
            {
                btnStartStopRecording.Text = "Start Recording";
                timerCAN.Stop();
                // CloseCANConnection();
            }
        }
        
        private void btnStartStopRecording_Click(object sender, EventArgs e)
        {
            plugin.IsRecording = !plugin.IsRecording;
            StartStopRecording();
            
        }

        private void timerCAN_Tick(object sender, EventArgs e)
        {
            if(CanIF > 0)
            {
                ReceiveCANData();
            }
        }

        private void PerformanceDataControl_Leave(object sender, EventArgs e)
        {
            CloseCANConnection();
        }
    }
}
