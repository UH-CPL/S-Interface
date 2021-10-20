using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections;
using PvDotNet;
using PvGUIDotNet;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace GigEGrabber
{
    [Serializable]
    public class GigEGrabberPlugin : IPlugin, ISerializable
    {
        //Declarations of all our internal plugin variables
        string myName = "GiGE Thermal Camera";
        string myDescription = "GiGE Thermal Camera Plugin";
        string myAuthor = "Pradeep Buddharaju";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        int myID = -1;
        bool serialized = false;
        bool isStreaming = false;

        

        PvDeviceInfo lDeviceInfo;

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

        public PvDeviceInfo LDeviceInfo
        {
            get { return lDeviceInfo; }
            set
            {
                lDeviceInfo = value;
                
                if (lDeviceInfo != null)
                {
                    StartStreaming();
                }
                
            }
        }

        public event StringDelegate CameraInterfaceChanged;
        public event StringDelegate FrameNumChanged;
        public event StringDelegate TimestampChanged;
        public event StringDelegate FileNameChanged;
        public event IntegerDelegate RecordTriggerChanged;

        private const UInt16 cBufferCount = 16;

        private PvDevice mDevice = null;
        private PvStream mStream = null;

        private Thread mThread = null;
        private bool mIsStopping = false;

        int curFrameNum = 0;
        FrameInfo frameInfo = new FrameInfo();
        DateTime firstFrameTimestamp;
        float[] fdata = null;
        int Width = 0;
        int Height = 0;
        bool isNewFrame = false;
        bool isRecording = false;
        string myFileName;

        BinaryWriter fwriter = null;
        ArrayList timeStampArray = new ArrayList();

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
                if(myFileName != null)
                {
                    this.Host.SendData(outFileName, new StringData(myFileName), this);
                }
            }
        }

        IPin triggerInputPin = null;
        IPin infileName = null;
        IPin thermalOutput = null;
        IPin outFrameInfo = null;
        IPin outFileName = null;
        IPin outFrameNumber = null;

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first

            triggerInputPin = Host.LoadOrCreatePin("Request", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(triggerInputPin);
            infileName = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            inPins.Add(infileName);

            thermalOutput = Host.LoadOrCreatePin("Video Output", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            outPins.Add(thermalOutput);

            outFrameInfo = Host.LoadOrCreatePin("Frame Info", PinCategory.Optional, new Type[] { typeof(IFrameInfo) });
            outPins.Add(outFrameInfo);
            outFileName = Host.LoadOrCreatePin("Filename", PinCategory.Optional, new Type[] { typeof(IStringData) });
            outPins.Add(outFileName);
            outFrameNumber = Host.LoadOrCreatePin("Frame Number", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(outFrameNumber);


            myMainInterface = new GigEGrabberControl(this);

            if (serialized && LDeviceInfo!=null)
            {
                CameraInterfaceChanged(LDeviceInfo.DisplayID);
            }
            else
            {
                CameraInterfaceChanged("None");
            }

        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped

            StopStreaming();
            Disconnect();
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
                        timeStampArray.Clear();
                        curFrameNum = 0;
                        IsRecording = true;

                    }
                    else if (trigger == 0)
                    {
                        IsRecording = false;
                    }

                    StartStopRecording();
                    RecordTriggerChanged(trigger);
                }
            }

            if (pin == infileName)
            {
                if (input != null)
                {
                    //get the input string
                    string tempStr;
                    tempStr = ((IStringData)input).Data;
                    FileNameChanged(tempStr);
                }
            }

            if(isStreaming && isNewFrame)
            {
                this.Host.SendData(thermalOutput, new ThermalData(fdata, Height, Width), this);

                if(IsRecording)
                {
                    DateTime curTime = DateTime.Now;
                    if (curFrameNum == 0)
                    {
                        firstFrameTimestamp = curTime;
                    }
                    frameInfo.FrameNumber = curFrameNum;
                    frameInfo.Span = (TimeSpan)curTime.Subtract(firstFrameTimestamp);
                    frameInfo.Time = curTime;
                    this.Host.SendData(outFrameNumber, new IntegerData(curFrameNum), this);
                    this.Host.SendData(outFrameInfo, frameInfo, this);

                    if (isStreaming)
                    {
                        FrameNumChanged(curFrameNum.ToString());
                        TimestampChanged(frameInfo.Time.ToString("hh:mm:ss.fff tt"));
                    }

                    curFrameNum++;
                }
                else
                {
                    this.Host.SendData(outFrameNumber, null, this);
                    this.Host.SendData(outFrameInfo, null, this);

                    FrameNumChanged("");
                    TimestampChanged("");
                }
                
                isNewFrame = false;
            }
            else
            {
                this.Host.SendData(outFrameNumber, null, this);
                this.Host.SendData(outFrameInfo, null, this);

                this.Host.SendData(thermalOutput, null, this);
            }
        }

        public void StartStopRecording()
        {
            if (IsRecording)
            {
                if (fwriter == null)
                {
                    string datFileName = MyFileName;
                    int counter = 1;
                    while(File.Exists(datFileName + ".dat"))
                    {
                        datFileName = datFileName + counter.ToString();
                        counter++;
                    }
                    fwriter = new BinaryWriter(new FileStream(datFileName + ".dat", FileMode.Create));
                }
            }
            else
            {
                if (fwriter != null)
                {
                    fwriter.Close();
                    fwriter = null;
                }

                string infStr = "";
                infStr += timeStampArray.Count.ToString() + "\n" + Width.ToString() + "\n" + Height.ToString() + "\n";
                foreach(string timeStamp in timeStampArray)
                {
                    infStr += "\n" + timeStamp;
                }
                File.WriteAllText(MyFileName+".inf", infStr);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Camera Streaming Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public void StartStopStreaming()
        {
            if(isStreaming)
            {
                //isStreaming = false;
                StopStreaming();
                Disconnect();
                
            }
            else
            {
                //isStreaming = true;
                StartStreaming();
                
            }
        }

        private bool ConnectCamera()
        {
            if (lDeviceInfo == null)
                return false;

            try
            {
                // Connect device.
                mDevice = PvDevice.CreateAndConnect(lDeviceInfo);

                // Open stream.
                mStream = PvStream.CreateAndOpen(lDeviceInfo.ConnectionID);
                if (mStream == null)
                {
                    return false;
                }
            }
            catch (PvException ex)
            {
                return false;
            }

            return true;
        }


        private bool ConfigureCameraStream()
        {
            try
            {
                // Perform GigE Vision only configuration
                PvDeviceGEV lDGEV = mDevice as PvDeviceGEV;
                if (lDGEV != null)
                {
                    // Negotiate packet size
                    lDGEV.NegotiatePacketSize();

                    // Set stream destination.
                    PvStreamGEV lSGEV = mStream as PvStreamGEV;
                    lDGEV.SetStreamDestination(lSGEV.LocalIPAddress, lSGEV.LocalPort);
                }

                // Read payload size, preallocate buffers of the pipeline.
                Int64 lPayloadSize = mDevice.PayloadSize;

                PvGenEnum lPixelFormat = mDevice.Parameters.GetEnum("PixelFormat");
                lPixelFormat.ValueString = "Mono14";

                PvGenEnum lTestPattern = mDevice.Parameters.GetEnum("TestPattern");
                lTestPattern.ValueString = "Off";

                // Get minimum buffer count, creates and allocates buffer.
                UInt32 lBufferCount = (mStream.QueuedBufferMaximum < cBufferCount) ? mStream.QueuedBufferMaximum : cBufferCount;
                PvBuffer[] lBuffers = new PvBuffer[lBufferCount];
                for (UInt32 i = 0; i < lBufferCount; i++)
                {
                    lBuffers[i] = new PvBuffer();
                    lBuffers[i].Alloc((UInt32)lPayloadSize);
                }

                // Queue all buffers in the stream.
                for (UInt32 i = 0; i < lBufferCount; i++)
                {
                    mStream.QueueBuffer(lBuffers[i]);
                }
            }
            catch (PvException ex)
            {
                //Step6Disconnecting();
                return false;
            }

            return true;
        }

        private void StartStreaming()
        {
            if(ConnectCamera() && ConfigureCameraStream())
            {
                // Start display thread.
                mThread = new Thread(new ParameterizedThreadStart(ThreadProc));
                GigEGrabberPlugin lP1 = this;
                object[] lParameters = new object[] { lP1 };
                mIsStopping = false;
                mThread.Start(lParameters);

                // Enables streaming before sending the AcquisitionStart command.
                mDevice.StreamEnable();

                // Start acquisition on the device
                mDevice.Parameters.ExecuteCommand("AcquisitionStart");

                isStreaming = true;
            }
        }

        private bool StopStreaming()
        {
            if(mDevice!=null && mStream != null)
            {
                try
                {
                    PvBuffer lBuffer = null;
                    PvResult lOperationResult = new PvResult(PvResultCode.OK);
                    PvResult lResult = new PvResult(PvResultCode.OK);

                    // Stop acquisition.
                    mDevice.Parameters.ExecuteCommand("AcquisitionStop");

                    // Disable streaming after sending the AcquisitionStop command.
                    mDevice.StreamDisable();

                    // Stop display thread.
                    mIsStopping = true;
                    mThread.Join();
                    mThread = null;

                    // Abort all buffers in the stream and dequeue
                    mStream.AbortQueuedBuffers();
                    for (int i = 0; i < mStream.QueuedBufferCount; i++)
                    {
                        lResult = mStream.RetrieveBuffer(ref lBuffer, ref lOperationResult);
                        if (lResult.IsOK)
                        {
                            lBuffer = null;
                        }
                    }

                    isStreaming = false;
                }
                catch (PvException lExc)
                {
                    return false;
                }
            }
            
            return true;
        }

        private void Disconnect()
        {
            if (mStream != null)
            {
                // Close and release stream
                mStream.Close();
                mStream = null;
            }

            if (mDevice != null)
            {
                // Disconnect and release device
                mDevice.Disconnect();
                mDevice = null;
            }
        }

        private static void ThreadProc(object aParameters)
        {
            object[] lParameters = (object[])aParameters;
            GigEGrabberPlugin lThis = (GigEGrabberPlugin)lParameters[0];

            for (;;)
            {
                if (lThis.mIsStopping)
                {
                    // Signaled to terminate thread, return.
                    return;
                }

                PvBuffer lBuffer = null;
                PvResult lOperationResult = new PvResult(PvResultCode.OK);

                // Retrieve next buffer from acquisition pipeline
                PvResult lResult = lThis.mStream.RetrieveBuffer(ref lBuffer, ref lOperationResult, 100);
                if (lResult.IsOK)
                {
                    // We have an image - do some processing (...) and VERY IMPORTANT,
                    lThis.ProcessBuffer(lBuffer);

                    // re-queue the buffer in the stream object.
                    lThis.mStream.QueueBuffer(lBuffer);
                }
            }
        }

        private unsafe void ProcessBuffer(PvBuffer aBuffer)
        {
            uint size = aBuffer.AcquiredSize;
            byte[] managedArray = new byte[size];
            byte* pointer = aBuffer.DataPointer;
            Marshal.Copy((IntPtr)pointer, managedArray, 0, (int)size);

            // Convert raw byte array into short array whose each element takes 2 bytes 
            short[] sdata = new short[managedArray.Length / 2];
            System.Buffer.BlockCopy(managedArray, 0, sdata, 0, managedArray.Length);

            if (fdata == null)
            {
                fdata = new float[sdata.Length];
                Height = (int)aBuffer.Image.Height;
                Width = (int)aBuffer.Image.Width;
            }
            // Convert to temperature
            for (int i = 0; i < sdata.Length; i++)
            {
                // fdata[i] = (float)(sdata[i] * 0.04 - 272.15) - 3.0f;
                fdata[i] = (float)(sdata[i] * 0.04 - 272.15);
            }
            isNewFrame = true;

            /*
            if(IsRecording && fwriter!=null)
            {
                if(curFrameNum == 0)
                {
                    fwriter.Write((int)aBuffer.Image.Width);
                    fwriter.Write((int)aBuffer.Image.Height);
                    fwriter.Write((int)aBuffer.Image.BitsPerPixel);
                }

                //fwriter.Write(frameInfo.Span.TotalSeconds);
                fwriter.Write(managedArray);
            }
            */

            if (IsRecording)
            {

                UInt16[] udata = new UInt16[fdata.Length];
                for (int i = 0; i < fdata.Length; i++)
                {
                    udata[i] = (UInt16)(fdata[i] * 1000);
                }

                // create a byte array and copy the floats into it...
                var byteArray = new byte[udata.Length * 2];
                Buffer.BlockCopy(udata, 0, byteArray, 0, byteArray.Length);

                try
                {
                    string timestamp = DateTime.Now.ToString("ddd MMM dd hh:mm:ss.fff yyyy");
                    timeStampArray.Add(timestamp);
                    if(fwriter != null)
                    {
                        fwriter.Write(byteArray);
                    }
                }
                catch
                {

                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public GigEGrabberPlugin(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            
            try
            {
                LDeviceInfo = (PvDeviceInfo)info.GetValue("deviceinfo", typeof(PvDeviceInfo));
                serialized = true;
            }
            catch
            {
                LDeviceInfo = null;
                serialized = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public GigEGrabberPlugin()
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
