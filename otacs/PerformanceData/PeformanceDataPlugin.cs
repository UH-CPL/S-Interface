using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace PerformanceData
{
    public struct DataPoint
    {
        public float time;
        public float data;
    }

    public struct PerformanceDataPoint
    {
        public float time;
        public float latitude;
        public float longitude;
        public float steering;
        public float acceleration;
        public float brake;
        public float speed;
    }

    public struct GPSDataPoint
    {
        public float time;
        public float latitude;
        public float longitude;
    }

    public enum PerformanceDataID
    {
        GPS,
        SteeringAcceleration,
        BrakeSpeed,
        None
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    //Delegates
    ///////////////////////////////////////////////////////////////////////////////////////////
    public delegate void StringDelegate(string value);
    public delegate void DoubleDelegate(double value);
    public delegate void BoolDelegate(bool value);
    public delegate void IntegerDelegate(int value);
    public delegate void VoidDelegate();

    [Serializable]
    public class PeformanceDataPlugin : IPlugin, ISerializable
    {
        //Declarations of all our internal plugin variables
        string myName = "Performance Data";
        string myDescription = "Performance Data Plugin";
        string myAuthor = "Pradeep & Ashik";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        int myID = -1;
        bool serialized = false;
        bool isStreaming = false;

        ArrayList listCANData = new ArrayList();


        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public string Description
        {
            get { return myDescription; }
        }



        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author
        {
            get { return myAuthor; }

        }

        /// <summary>
        /// Host of the plugin.
        /// </summary>
        public IPluginHost Host
        {
            get { return myHost; }
            set { myHost = value; }
        }

        /// <summary>
        /// Unique ID.
        /// </summary>
        public int MyID
        {
            get { return myID; }
            set { myID = value; }
        }

        public string Name
        {
            get { return myName; }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return myMainInterface; }
        }

        public string Version
        {
            get { return myVersion; }
        }

        public ArrayList InputPins
        {
            get { return inPins; }
        }

        public ArrayList OutputPins
        {
            get { return outPins; }
        }

        public bool IsStreaming
        {
            get { return isStreaming; }
        }

        public event StringDelegate FileNameChanged;
        public event IntegerDelegate RecordTriggerChanged;
        public event BoolDelegate ShowHideWaitForm;

        bool isRecording = false;
        string myFileName;

        public bool IsRecording
        {
            get { return isRecording; }
            set { isRecording = value; }
        }

        public string MyFileName
        {
            get { return myFileName; }
            set
            {
                myFileName = value;
                if (myFileName != null)
                {
                    this.Host.SendData(outFileName, new StringData(myFileName), this);
                }
            }
        }

        IPin triggerInputPin = null;
        IPin infileName = null;

        IPin outFileName = null;

        List<DataPoint> steeringData = new List<DataPoint>();
        List<GPSDataPoint> gpsData = new List<GPSDataPoint>();
        List<DataPoint> accelerationData = new List<DataPoint>();
        List<DataPoint> brakeData = new List<DataPoint>();
        List<DataPoint> speedData = new List<DataPoint>();
        DateTime startTime;
        private string rawPerformanceData = "";
        List<PerformanceDataPoint> PerformanceDP = new List<PerformanceDataPoint>();

        private BackgroundWorker bw = new BackgroundWorker();

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first

            triggerInputPin = Host.LoadOrCreatePin("Request", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(triggerInputPin);
            infileName = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            inPins.Add(infileName);

            outFileName = Host.LoadOrCreatePin("Filename", PinCategory.Optional, new Type[] { typeof(IStringData) });
            outPins.Add(outFileName);

            myMainInterface = new PerformanceDataControl(this);

            MyFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            MyFileName += "\\PerformanceData.perf";
            FileNameChanged(MyFileName);

            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_WorkComplete;


        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            if (pin == triggerInputPin)
            {
                if (input != null)
                {
                    //get the action
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {

                        IsRecording = true;

                    }
                    else if (trigger == 0)
                    {
                        IsRecording = false;
                    }
                    RecordTriggerChanged(trigger);
                }
            }

            if (pin == infileName)
            {
                if (input != null)
                {
                    //get the input string
                    MyFileName = ((IStringData)input).Data;
                    FileNameChanged(MyFileName);
                }
            }
        }


        public void StartStopRecording()
        {
            if (IsRecording)
            {
                startTime = DateTime.Now;
                listCANData.Clear();
                steeringData.Clear();
                accelerationData.Clear();
                brakeData.Clear();
                speedData.Clear();
                gpsData.Clear();
                PerformanceDP.Clear();
            }
            else
            {
                bw.RunWorkerAsync();
                ShowHideWaitForm(true);
            }
        }

        private void CreateGPSFile()
        {
            string filePath;

            if (MyFileName != null)
            {
                filePath = MyFileName;
                if (!filePath.Contains(".gps"))
                {
                    filePath += ".gps";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Location";
                xlWorkSheetPP.Cells[9, 1] = "Time";
                xlWorkSheetPP.Cells[9, 2] = "Latitude";
                xlWorkSheetPP.Cells[9, 3] = "Longitude";

                int curRow = 10;
                foreach (GPSDataPoint curData in gpsData)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.latitude;
                    xlWorkSheetPP.Cells[curRow, 3] = curData.longitude;
                    curRow++;
                }

                xlWorkBookPP.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void CreateSteeringFile()
        {
            string filePath;

            if (MyFileName != null)
            {
                filePath = MyFileName;
                if (!filePath.Contains(".steer"))
                {
                    filePath += ".steer";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Steering";
                xlWorkSheetPP.Cells[9, 1] = "Time";
                xlWorkSheetPP.Cells[9, 2] = "Steering";

                int curRow = 10;
                foreach (DataPoint curData in steeringData)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.data;
                    curRow++;
                }

                xlWorkBookPP.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void CreateAccelerationFile()
        {
            string filePath;

            if (MyFileName != null)
            {
                filePath = MyFileName;
                if (!filePath.Contains(".accel"))
                {
                    filePath += ".accel";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Acceleration";
                xlWorkSheetPP.Cells[9, 1] = "Time";
                xlWorkSheetPP.Cells[9, 2] = "Acceleration";

                int curRow = 10;
                foreach (DataPoint curData in accelerationData)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.data;
                    curRow++;
                }

                xlWorkBookPP.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void CreteBrakeFile()
        {
            string filePath;

            if (MyFileName != null)
            {
                filePath = MyFileName;
                if (!filePath.Contains(".brake"))
                {
                    filePath += ".brake";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Brake";
                xlWorkSheetPP.Cells[9, 1] = "Time";
                xlWorkSheetPP.Cells[9, 2] = "Brake";

                int curRow = 10;
                foreach (DataPoint curData in brakeData)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.data;
                    curRow++;
                }

                xlWorkBookPP.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void CreateSpeedFile()
        {
            string filePath;

            if (MyFileName != null)
            {
                filePath = MyFileName;
                if (!filePath.Contains(".speed"))
                {
                    filePath += ".speed";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Speed";
                xlWorkSheetPP.Cells[9, 1] = "Time";
                xlWorkSheetPP.Cells[9, 2] = "Speed";

                int curRow = 10;
                foreach (DataPoint curData in speedData)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.data;
                    curRow++;
                }

                xlWorkBookPP.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void SaveCANData()
        {
            string CANFilePath;

            if (MyFileName != null)
            {

                CANFilePath = MyFileName;
                if (!CANFilePath.Contains(".perf"))
                {
                    CANFilePath += ".perf";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(CANFilePath))
                {
                    System.IO.File.Delete(CANFilePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet.Cells[1, 1] = "ID";
                xlWorkSheet.Cells[1, 2] = "Timestamp";
                xlWorkSheet.Cells[1, 3] = "Data0";
                xlWorkSheet.Cells[1, 4] = "Data1";
                xlWorkSheet.Cells[1, 5] = "Data2";
                xlWorkSheet.Cells[1, 6] = "Data3";
                xlWorkSheet.Cells[1, 7] = "Data4";
                xlWorkSheet.Cells[1, 8] = "Data5";
                xlWorkSheet.Cells[1, 9] = "Data6";
                xlWorkSheet.Cells[1, 10] = "Data7";

                int curRow = 2;
                foreach (CanalMsg rxMsg in listCANData)
                {
                    xlWorkSheet.Cells[curRow, 1] = rxMsg.id.ToString();
                    xlWorkSheet.Cells[curRow, 2] = rxMsg.timestamp.ToString();
                    xlWorkSheet.Cells[curRow, 3] = string.Format(" {0:X2}", rxMsg.data0);
                    xlWorkSheet.Cells[curRow, 4] = string.Format(" {0:X2}", rxMsg.data1);
                    xlWorkSheet.Cells[curRow, 5] = string.Format(" {0:X2}", rxMsg.data2);
                    xlWorkSheet.Cells[curRow, 6] = string.Format(" {0:X2}", rxMsg.data3);
                    xlWorkSheet.Cells[curRow, 7] = string.Format(" {0:X2}", rxMsg.data4);
                    xlWorkSheet.Cells[curRow, 8] = string.Format(" {0:X2}", rxMsg.data5);
                    xlWorkSheet.Cells[curRow, 9] = string.Format(" {0:X2}", rxMsg.data6);
                    xlWorkSheet.Cells[curRow, 10] = string.Format(" {0:X2}", rxMsg.data7);

                    curRow++;
                }

                xlWorkBook.SaveAs(CANFilePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void CreatePerformanceFile()
        {
            string CANFilePath;

            if (MyFileName != null)
            {
                CANFilePath = MyFileName;
                if (!CANFilePath.Contains(".res"))
                {
                    CANFilePath += ".res";
                }

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                if (System.IO.File.Exists(CANFilePath))
                {
                    System.IO.File.Delete(CANFilePath);
                }

                Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
                object misValuePP = System.Reflection.Missing.Value;

                xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
                xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

                xlWorkSheetPP.Cells[8, 1] = "Vehicle Output";
                xlWorkSheetPP.Cells[9, 1] = "Frame";
                xlWorkSheetPP.Cells[9, 2] = "Time";
                xlWorkSheetPP.Cells[9, 3] = "Steering";
                xlWorkSheetPP.Cells[9, 4] = "Braking";
                xlWorkSheetPP.Cells[9, 5] = "Acceleration";
                xlWorkSheetPP.Cells[9, 6] = "Speed";
                xlWorkSheetPP.Cells[9, 7] = "Latitude";
                xlWorkSheetPP.Cells[9, 8] = "longitude";

                int frame = 1;
                int curRow = 10;
                foreach (PerformanceDataPoint curData in PerformanceDP)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = frame;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 3] = curData.steering;
                    xlWorkSheetPP.Cells[curRow, 4] = curData.brake;
                    xlWorkSheetPP.Cells[curRow, 5] = curData.acceleration;
                    xlWorkSheetPP.Cells[curRow, 6] = curData.speed;
                    xlWorkSheetPP.Cells[curRow, 7] = curData.latitude;
                    xlWorkSheetPP.Cells[curRow, 8] = curData.longitude;
                    curRow++;
                    frame++;
                }

                xlWorkBookPP.SaveAs(CANFilePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
                xlWorkBookPP.Close(true, misValuePP, misValuePP);
                xlApp.Quit();

                releaseObject(xlWorkSheetPP);
                releaseObject(xlWorkBookPP);
                releaseObject(xlApp);
            }
        }

        private void CreateRawPerformanceFile()
        {
            string CANFilePath;

            if (MyFileName != null)
            {
                CANFilePath = MyFileName;
                if (!CANFilePath.Contains(".rawres"))
                {
                    CANFilePath += ".rawres";
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(CANFilePath))
                {
                    file.WriteLine(rawPerformanceData);
                }
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
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

        public void AddPerformanceData(PerformanceDataPoint dp)
        {
            PerformanceDP.Add(dp);
        }

        public void AddRawPerformanceData(string data)
        {
            rawPerformanceData = data;
        }

        public void AddCANData(CanalMsg rxMsg)
        {
            listCANData.Add(rxMsg);

            DateTime curTime = DateTime.Now;
            float timeRecording = (float)(curTime.Subtract(startTime)).TotalSeconds;

            if (rxMsg.id == (uint)PerformanceDataID.GPS + 1)
            {
                string latHexString = BytesToString(rxMsg.data0, rxMsg.data1, rxMsg.data2, rxMsg.data3);
                float lat = HexToFloat(latHexString);
                string lonHexString = BytesToString(rxMsg.data4, rxMsg.data5, rxMsg.data6, rxMsg.data7);
                float lon = HexToFloat(lonHexString);

                GPSDataPoint gpsDP = new GPSDataPoint();
                gpsDP.time = timeRecording;
                gpsDP.latitude = lat;
                gpsDP.longitude = lon;

                gpsData.Add(gpsDP);
            }
            else if (rxMsg.id == (uint)PerformanceDataID.SteeringAcceleration + 1)
            {
                string steeringString = BytesToString(rxMsg.data0, rxMsg.data1, rxMsg.data2, rxMsg.data3);
                float steering = HexToFloat(steeringString);
                DataPoint steeringDP = new DataPoint();
                steeringDP.time = timeRecording;
                steeringDP.data = steering;
                steeringData.Add(steeringDP);

                string accelerationString = BytesToString(rxMsg.data4, rxMsg.data5, rxMsg.data6, rxMsg.data7);
                float acceleration = HexToFloat(accelerationString);
                DataPoint accelerationDP = new DataPoint();
                accelerationDP.time = timeRecording;
                accelerationDP.data = acceleration;
                accelerationData.Add(accelerationDP);
            }
            else if (rxMsg.id == (uint)PerformanceDataID.BrakeSpeed + 1)
            {
                string brakeString = BytesToString(rxMsg.data0, rxMsg.data1, rxMsg.data2, rxMsg.data3);
                float brake = HexToFloat(brakeString);
                DataPoint brakeDP = new DataPoint();
                brakeDP.time = timeRecording;
                brakeDP.data = brake;
                brakeData.Add(brakeDP);

                string speedString = BytesToString(rxMsg.data4, rxMsg.data5, rxMsg.data6, rxMsg.data7);
                float speed = HexToFloat(speedString);
                DataPoint speedDP = new DataPoint();
                speedDP.time = timeRecording;
                speedDP.data = speed;
                speedData.Add(speedDP);
            }

        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //SaveCANData();
            //CreateGPSFile();
            //CreateAccelerationFile();
            //CreateSpeedFile();
            //CreateSteeringFile();
            //CreteBrakeFile();
            CreatePerformanceFile();
            //CreateRawPerformanceFile();
        }

        private void bw_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            ShowHideWaitForm(false);
            //this.Host.SendData(UploadPPOutputPin, new IntegerData(1), this);
            //myMainInterface.HideWaitForm();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public PeformanceDataPlugin(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                //LDeviceInfo = (PvDeviceInfo)info.GetValue("deviceinfo", typeof(PvDeviceInfo));
                serialized = true;
            }
            catch
            {
                //LDeviceInfo = null;
                serialized = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public PeformanceDataPlugin()
        ///////////////////////////////////////////////////////////////////////////////////////////
        {

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //if(LDeviceInfo != null)
            //info.AddValue("deviceinfo", LDeviceInfo, typeof(PvDeviceInfo));


        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}