using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using AviFile;
using PluginInterface;
using System.ComponentModel;

using AForge.Video.FFMPEG;

namespace AVIWriterCompressedMulti
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    //Delegates
    ///////////////////////////////////////////////////////////////////////////////////////////
    public delegate void FloatDelegate(float value);
    public delegate void IntDelegate(int value);
    public delegate void StringDelegate(string name);
    public delegate void BoolDelegate(bool value);
    public delegate void VoidDelegate();

    [Serializable]
    ///////////////////////////////////////////////////////////////////////////////////////////
    public class AVIWriterCompressMulti : IPlugin, ISerializable //,BaseFilter
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "AVI Writer Compress Multi";
        string myDescription = "AVI Writer Compress Multi";
        string myAuthor = "Hadi";
        string myVersion = "2.0.0";
        IPluginHost myHost = null;
        int myID = -1;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        Queue myQ = new Queue();

        VideoFileWriter writer = new VideoFileWriter();

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

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region AVI writer Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //input and output pins
        IPin filenameInputPin;
        IPin folderPathInputPin;
        IPin FrameNumInputPin;
        IPin TotalFramesInputPin;
        IPin videoInputPin;
        IPin triggerInputPin;
        IPin ThermalInputPin;

        protected IPin UploadAVIOutputPin;

        //IPin filenameInputPin2;
        //IPin videoInputPin2;

        ////////////////////////////////////////
        // Instance Variables
        ////////////////////////////////////////

        public string saveFileName;
        public string folderPath;
        public string appendString;
        public bool isAddDummyFrames;
        long curFrameNum;
        long totalFrames;
        bool isNewFrame;
        bool isLastFrameDealt;
        public bool writeAVIFile;
        Bitmap curBitmap;
        AviManager aviManager;
        VideoStream aviStream;
        public string saveFileName2;
        public string folderPath2;
        public string appendString2;
        //long curFrameNum2;
        //bool isNewFrame2;
        //bool isLastFrameDealt2;
        //Bitmap curBitmap2;

        bool isCreatingAVI = false;
        List<int> processedFrames = new List<int>();
        List<int> droppableFrames = new List<int>();
        int frameRate = 5;
        private BackgroundWorker bw = new BackgroundWorker();

        // User Control for GUI
        protected AVIWriterCompressMultiControl control;

        BinaryWriter fwriter = null;
        int imgWidth;
        int imgHeight;
        int frameCount = 0;
        float Min;
        float Max;

        byte[] rgbRED = new byte[256];
        byte[] rgbGREEN = new byte[256];
        byte[] rgbBLUE = new byte[256];

        ArrayList timeStampArray = new ArrayList();

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        // Event to change FileName
        public event StringDelegate saveFileNameChanged;
        public event StringDelegate saveFileNameChanged2;
        public event StringDelegate appendStringChanged;
        public event BoolDelegate ShowHideWaitForm;
        //public event VoidDelegate UpdateAVIWritingStatus;

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////
        #region Plugin Required Methods
        /////////////////////////////////////////////////////////////////////////////
        public void Initialize()
        {
            videoInputPin = myHost.LoadOrCreatePin("Video", PinCategory.Optional, new Type[] { typeof(IBmpData) });
            inPins.Add(videoInputPin);

            filenameInputPin = myHost.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IStringData) });
            inPins.Add(filenameInputPin);

            folderPathInputPin = myHost.LoadOrCreatePin("Folder Path", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IStringData) });
            inPins.Add(folderPathInputPin);

            FrameNumInputPin = myHost.LoadOrCreatePin("Frame Number", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(ILongData) });
            inPins.Add(FrameNumInputPin);

            TotalFramesInputPin = myHost.LoadOrCreatePin("Total Frames", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(ILongData) });
            inPins.Add(TotalFramesInputPin);

            //triggerInputPin = myHost.LoadOrCreatePin("Trigger", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IBoolData) }, new Type[] { typeof(ITimeStampData) });
            triggerInputPin = myHost.LoadOrCreatePin("Trigger", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IIntegerData) });
            inPins.Add(triggerInputPin);

            ThermalInputPin = Host.LoadOrCreatePin("Thermal", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            inPins.Add(ThermalInputPin);

            UploadAVIOutputPin = Host.LoadOrCreatePin("Upload AVI", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(UploadAVIOutputPin);

            /*
            videoInputPin2 = myHost.LoadOrCreatePin("Video2", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IBmpData)});
            inPins.Add(videoInputPin2);
            */

            /*
            filenameInputPin2 = myHost.LoadOrCreatePin("File Name2", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IStringData) });
            inPins.Add(filenameInputPin2);
            */

            isAddDummyFrames = true;
            curFrameNum = -1;
            totalFrames = long.MaxValue;
            isNewFrame = false;
            //isNewFrame2 = false;
            saveFileName = null;
            //saveFileName2 = null;
            folderPath = null;
            curBitmap = null;
            //curBitmap2 = null;
            aviManager = null;
            aviStream = null;
            writeAVIFile = false;
            isLastFrameDealt = false;
            //isLastFrameDealt2 = false;
            //appendString = null;


            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_WorkComplete;

            myMainInterface = new AVIWriterCompressMultiControl(this);

            FillRGB();

        }
        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Process(IPin pin, IPinData input)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure the data is for the filenameInputPin
            if (pin == filenameInputPin)
            {
                if (input != null)
                {
                    //get the input string
                    string tempStr;
                    tempStr = ((IStringData)input).Data; // .Value

                    if (folderPath == null)
                    {
                        if (tempStr.LastIndexOf('.') != -1)
                        {
                            saveFileName = tempStr.Substring(0, tempStr.LastIndexOf('.'));
                        }
                        else
                        {
                            saveFileName = tempStr;
                        }
                        if (appendString != null)
                        {
                            saveFileName += appendString;
                        }
                        saveFileName2 = saveFileName + ".ppdat";
                        saveFileName += ".avi";
                    }
                    else
                    {
                        string[] subjName = tempStr.Split(new Char[] { '\\' });
                        if (appendString != null)
                        {
                            saveFileName2 = folderPath + "\\" + subjName[subjName.Length - 1] + appendString + ".ppdat";
                            saveFileName = folderPath + "\\" + subjName[subjName.Length - 1] + appendString + ".avi";
                        }
                        else
                        {
                            saveFileName2 = folderPath + "\\" + subjName[subjName.Length - 1] + ".ppdat";
                            saveFileName = folderPath + "\\" + subjName[subjName.Length - 1] + ".avi";
                        }
                    }

                    saveFileNameChanged(saveFileName);
                }
            }


            //Host.SignalCriticalProcessingIsFinished(filenameInputPin, this);  
            /*
            //make sure the data is for the filenameInputPin
            if (pin == filenameInputPin2)
            {
                if (input != null)
                {
                    //get the input string
                    string tempStr;
                    tempStr = ((IStringData)input).Data; //Value

                    if (folderPath == null)
                    {
                        if (tempStr.LastIndexOf('.') != -1)
                        {
                            saveFileName2 = tempStr.Substring(0, tempStr.LastIndexOf('.'));
                        }
                        else
                        {
                            saveFileName2 = tempStr;
                        }
                        if (appendString2 != null)
                        {
                            saveFileName2 += appendString2;
                        }
                        saveFileName2 += ".avi";
                    }
                    else
                    {
                        string[] subjName = tempStr.Split(new Char[] { '\\' });
                        if (appendString2 != null)
                        {
                            saveFileName2 = folderPath + "\\" + subjName[subjName.Length - 1] + appendString2 + ".avi";
                        }
                        else
                        {
                            saveFileName2 = folderPath + "\\" + subjName[subjName.Length - 1] + ".avi";
                        }

                    }

                    saveFileNameChanged2(saveFileName2);
                }
            }

            Host.SignalCriticalProcessingIsFinished(filenameInputPin2, this);  
            */
            //make sure the data is for the filenameInputPin
            if (pin == folderPathInputPin)
            {
                if (input != null)
                {
                    //get the input string
                    folderPath = ((IStringData)input).Data; //Value

                    if (saveFileName != null)
                    {
                        string aviFileName = Path.GetFileName(saveFileName);
                        saveFileName = folderPath + "\\" + aviFileName;
                        if (appendString != null)
                        {
                            string tempStr = saveFileName;
                            saveFileName = tempStr.Substring(0, tempStr.LastIndexOf('.'));
                            saveFileName += appendString;
                            saveFileName += ".avi";
                        }
                        saveFileNameChanged(saveFileName);
                    }

                    if (saveFileName2 != null)
                    {
                        string aviFileName = Path.GetFileName(saveFileName2);
                        saveFileName2 = folderPath + "\\" + aviFileName;
                        if (appendString2 != null)
                        {
                            string tempStr = saveFileName2;
                            saveFileName2 = tempStr.Substring(0, tempStr.LastIndexOf('.'));
                            saveFileName2 += appendString2;
                            saveFileName2 += ".ppdat";
                        }
                        saveFileNameChanged2(saveFileName2);
                    }
                }
            }
            //Host.SignalCriticalProcessingIsFinished(folderPathInputPin, this); 

            //make sure the data is for the FrameNumInputPin
            if (pin == FrameNumInputPin)
            {
                if (input != null)
                {
                    long temp = ((IntegerData)input).Value;
                    if (writeAVIFile)
                    {
                        if (temp == totalFrames - 1)
                        {
                            isNewFrame = false;
                            //isNewFrame2 = false;
                            if (!isLastFrameDealt)
                            {
                                isLastFrameDealt = true;
                                //StartStopAVIWriting();
                                //UpdateAVIWritingStatus();                            
                            }
                        }
                        else
                        {
                            if (temp > curFrameNum)
                            {
                                curFrameNum = temp;
                                isNewFrame = true;
                            }
                            else
                            {
                                isNewFrame = false;
                                //isNewFrame2 = false;
                            }
                        }
                    }

                    //Host.SignalCriticalProcessingIsFinished(FrameNumInputPin, this);

                    //SignalCriticalProcessingIsFinished();                                     
                }
            }

            //Host.SignalCriticalProcessingIsFinished(FrameNumInputPin, this);  

            //make sure the data is for the FrameNumInputPin
            if (pin == TotalFramesInputPin)
            {
                if (input != null)
                {
                    int totalSecs = ((IntegerData)input).Value;
                    double temp = (double)frameCount / (double)totalSecs;
                    frameRate = (int)Math.Floor(temp);
                    /*
                    float totalActualSecs = (float)frameCount / (float)frameRate;
                    float extraSecs = totalActualSecs - totalSecs;
                    if(extraSecs > 0)
                    {
                        int totalFramesToDrop = (int) (extraSecs * frameRate);
                        if(totalFramesToDrop > 0)
                        {
                            int dropRate = (int) ((float)frameCount / (float)totalFramesToDrop);

                            int dropFrame = 0;
                            while (dropFrame <= frameCount)
                            {
                                droppableFrames.Add(dropFrame);
                                dropFrame += dropRate;
                            }
                        }

                        
                    }
                    */

                    //WriteAVI();
                }
            }
            //Host.SignalCriticalProcessingIsFinished(TotalFramesInputPin, this);

            //make sure the data is for the FrameNumInputPin
            if (pin == ThermalInputPin)
            {
                if (input != null)
                {
                    //if (isNewFrame && writeAVIFile && !isInProcessedFrames((int)curFrameNum)) //isNewFrame && writeAVIFile
                    if (isNewFrame && writeAVIFile) //isNewFrame && writeAVIFile
                    {
                        processedFrames.Add((int)curFrameNum);

                        ThermalData thermal = (ThermalData)input;
                        float[] thermalRawData = thermal.Data;
                        imgWidth = thermal.Width;
                        imgHeight = thermal.Height;

                        UInt16[] udata = new UInt16[thermalRawData.Length];
                        for (int i = 0; i < thermalRawData.Length; i++)
                        {
                            udata[i] = (UInt16)(thermalRawData[i] * 1000);
                        }

                        // create a byte array and copy the floats into it...
                        var byteArray = new byte[udata.Length * 2];
                        Buffer.BlockCopy(udata, 0, byteArray, 0, byteArray.Length);

                        try
                        {
                            if (fwriter != null)
                            {
                                string timestamp = DateTime.Now.ToString("ddd MMM dd hh:mm:ss.fff yyyy");
                                timeStampArray.Add(timestamp);

                                fwriter.Write(byteArray);
                                frameCount++;
                            }
                        }
                        catch
                        {

                        }
                    }

                }
            }


            //check to see if the data is for the thermal pin
            if (pin == videoInputPin)
            {
                //check to see if the input is valid
                if (input != null)
                {
                    if (isNewFrame && writeAVIFile && !isInProcessedFrames((int)curFrameNum)) //isNewFrame && writeAVIFile
                    {
                        processedFrames.Add((int)curFrameNum);
                        BmpData bmpData = (BmpData)input;
                        curBitmap = (Bitmap)bmpData.Value;

                        if (curBitmap != null)
                        {
                            myQ.Enqueue(curBitmap);

                            /*
                            if (aviManager == null && aviStream == null)
                            {
                                if (!isCreatingAVI)
                                {
                                    isCreatingAVI = true;

                                    if (saveFileName == null)
                                    {
                                        saveFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                                        + "\\PerspirationVideo.avi";
                                    }
                                    if (System.IO.File.Exists(saveFileName))
                                    {
                                        System.IO.File.Delete(saveFileName);
                                    }
                                    aviManager = new AviManager(saveFileName, false);
                                    //System.Threading.Thread.Sleep(5);
                                    // aviStream = aviManager.AddVideoStream(false, 25, curBitmap);
                                    aviStream = aviManager.AddVideoStream(true, 4, curBitmap);
                                }
                               
                            }
                            else
                            {
                                //aviStream.AddFrame(new Bitmap(curBitmap));
                                
                                //myQ.Enqueue(curBitmap);
                            }
                            */

                        }
                        /*
                        if (myQ.Count == 100)
                        {
                            aviStream.AddFrame(new Bitmap((Bitmap)myQ.Dequeue()));
                        }
                        */
                    }
                }
                //Host.SignalCriticalProcessingIsFinished(videoInputPin, this);
            }
            //Host.SignalCriticalProcessingIsFinished(videoInputPin,this);

            //check to see if the data is for the thermal pin
            /*
            if (pin == videoInputPin2)
            {
                //check to see if the input is valid
                if (input != null)
                { 
                    //if (isNewFrame2 && writeAVIFile)
                    {
                        curBitmap2 = CopyDataToBitmap2((IImageDescription)input, (IBmpData)input);                 
                    }                   
                }
            }
            Host.SignalCriticalProcessingIsFinished(videoInputPin2,this);
            */
            //make sure the data is for the triggerInputPin

            if (pin == triggerInputPin)
            {
                if (input != null)
                {
                    //get the action
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        CreateDebugFile("Start called at " + DateTime.Now.ToLongTimeString());
                        curFrameNum = -1;
                        isLastFrameDealt = false;
                        isCreatingAVI = false;
                        processedFrames.Clear();
                        droppableFrames.Clear();
                        timeStampArray.Clear();
                        writeAVIFile = true;

                        frameCount = 0;
                        if (fwriter == null)
                        {
                            if (saveFileName2 == null)
                            {
                                saveFileName2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                                + "\\PerspirationVideo.ppdat";
                            }

                            fwriter = new BinaryWriter(new FileStream(saveFileName2, FileMode.Create));
                        }
                    }
                    else if (trigger == 0)
                    {
                        CreateDebugFile("Stop called at " + DateTime.Now.ToLongTimeString());

                        if (fwriter != null)
                        {
                            fwriter.Close();
                            fwriter = null;
                        }

                        if (writeAVIFile)
                        {
                            writeAVIFile = false;

                            //Console.WriteLine("CALLING WriteAVI");
                            WriteAVI();
                        }
                       
                    }
                }
            }

            //Host.SignalCriticalProcessingIsFinished(triggerInputPin,this);
            //SignalCriticalProcessingIsFinished();

        }

        /////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////


        private void WriteAVI()
        {
            bw.RunWorkerAsync();
            ShowHideWaitForm(true);

            

        }

        private bool isInProcessedFrames(int frameNum)
        {
            foreach(int frame in processedFrames)
            {
                if(frame == frameNum)
                {
                    return true;
                }
            }

            return false;
        }

        private bool isInDroppableFrames(int frameNum)
        {
            foreach (int frame in droppableFrames)
            {
                if (frame == frameNum)
                {
                    return true;
                }
            }

            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        public void StartStopAVIWriting()
        /////////////////////////////////////////////////////////////////////////////////////////
        {
            if (!writeAVIFile)
            {
                curFrameNum = -1;
                isLastFrameDealt = false;
                writeAVIFile = true;

                //testArray = new ArrayList();
                //testArray2 = new ArrayList();

            }
            else
            {
                //writeAVIFile = false;
                if(myQ.Count > 0)
                {
                    while (myQ.Count != 0)
                    {
                        aviStream.AddFrame((Bitmap)myQ.Dequeue());
                    }
                }
                

                if (aviManager != null)
                {
                    aviManager.Close();
                    aviManager = null;
                    aviStream = null;
                }
            }



            //Hadi/

            /*
            aviManager = new AviManager(saveFileName2, false);
            aviStream = null;
            Bitmap bmp = (Bitmap)testArray2[0];
            aviStream = aviManager.AddVideoStream(true, 30, bmp);
            for (int i = 1; i < testArray2.Count; i++)
            {
                bmp = (Bitmap)testArray2[i];
                aviStream.AddFrame(bmp);
            }
            testArray2.Clear();

            if (aviManager != null)
            {
                aviManager.Close();
                aviManager = null;
            }
            */


        }

        public void FillRGB()
        {

            for (int i = 0; i <= 0x20; i++)
            {
                int num2 = (i * 0xff) / 0x30;
                this.rgbRED[i] = (byte)num2;
                this.rgbGREEN[i] = 1;
                this.rgbBLUE[i] = 1;
            }
            for (int j = 0x21; j <= 0x30; j++)
            {
                int num4 = (j * 0xff) / 0x30;
                int num5 = ((j - 0x21) * 0xff) / 0x4f;
                this.rgbRED[j] = (byte)num4;
                this.rgbGREEN[j] = (byte)num5;
                this.rgbBLUE[j] = 1;
            }
            for (int k = 0x31; k <= 0x70; k++)
            {
                int num7 = ((k - 0x21) * 0xff) / 0x4f;
                this.rgbRED[k] = 0xff;
                this.rgbGREEN[k] = (byte)num7;
                this.rgbBLUE[k] = 1;
            }
            for (int m = 0x71; m <= 0x80; m++)
            {
                int num9 = ((0x90 - m) * 0xff) / 0x1f;
                this.rgbRED[m] = (byte)num9;
                this.rgbGREEN[m] = 0xff;
                this.rgbBLUE[m] = 1;
            }
            for (int n = 0x81; n <= 0x90; n++)
            {
                int num11 = ((0x90 - n) * 0xff) / 0x1f;
                int num12 = ((n - 0x81) * 0xff) / 0x2f;
                this.rgbRED[n] = (byte)num11;
                this.rgbGREEN[n] = 0xff;
                this.rgbBLUE[n] = (byte)num12;
            }
            for (int num13 = 0x91; num13 <= 160; num13++)
            {
                int num14 = ((0xc0 - num13) * 0xff) / 0x2f;
                int num15 = ((num13 - 0x81) * 0xff) / 0x2f;
                this.rgbRED[num13] = 1;
                this.rgbGREEN[num13] = (byte)num14;
                this.rgbBLUE[num13] = (byte)num15;
            }
            for (int num16 = 0xa1; num16 <= 0xb0; num16++)
            {
                int num17 = ((num16 - 0xa1) * 0xff) / 0x2f;
                int num18 = ((0xc0 - num16) * 0xff) / 0x2f;
                int num19 = ((num16 - 0x81) * 0xff) / 0x2f;
                this.rgbRED[num16] = (byte)num17;
                this.rgbGREEN[num16] = (byte)num18;
                this.rgbBLUE[num16] = (byte)num19;
            }
            for (int num20 = 0xb1; num20 <= 0xc0; num20++)
            {
                int num21 = ((num20 - 0xa1) * 0xff) / 0x2f;
                int num22 = ((0xc0 - num20) * 0xff) / 0x2f;
                this.rgbRED[num20] = (byte)num21;
                this.rgbGREEN[num20] = (byte)num22;
                this.rgbBLUE[num20] = 0xff;
            }
            for (int num23 = 0xc1; num23 <= 0xd0; num23++)
            {
                int num24 = ((num23 - 0xa1) * 0xff) / 0x2f;
                this.rgbRED[num23] = (byte)num24;
                this.rgbGREEN[num23] = 1;
                this.rgbBLUE[num23] = 0xff;
            }
            for (int num25 = 0xd1; num25 <= 0xe0; num25++)
            {
                int num26 = ((num25 - 0xd1) * 0xff) / 0x2e;
                this.rgbRED[num25] = 0xff;
                this.rgbGREEN[num25] = (byte)num26;
                this.rgbBLUE[num25] = 0xff;
            }
            for (int num27 = 0xe1; num27 <= 0xff; num27++)
            {
                int num28 = ((0xff - num27) * 0xff) / 30;
                int num29 = ((num27 - 0xd1) * 0xff) / 0x2e;
                this.rgbRED[num27] = (byte)num28;
                this.rgbGREEN[num27] = (byte)num29;
                this.rgbBLUE[num27] = 0xff;
            }
        }

        public Bitmap GetFrame(FileStream FS, int frameNum, int Width, int Height, int bmpWidth, int bmpHeight)
        {
            float[] dataArray = new float[Width * Height];
            byte[] data = new byte[Width * Height * 2];

            long lH = (long)Height;
            long lW = (long)Width;
            long lFN = (long)frameNum;
            long offset = (((lW * lH) * lFN) * 2L) - FS.Position;
            //Hadi
            try
            {
                FS.Seek(offset, SeekOrigin.Current);
            }
            catch (IOException e)
            {

            }
            finally
            {
            }

            FS.Seek(0, SeekOrigin.Current);
            FS.Read(data, 0, (Width * Height) * 2);

            int index = 0;
            Min = float.MaxValue;
            Max = float.MinValue;
            for (int i = 0; i < Width * Height; i++)
            {
                dataArray[i] = ((float)(data[index] + (data[index + 1] << 8))) / 1000f;

                if (dataArray[i] > Max && dataArray[i] < 40)
                {
                    Max = dataArray[i];
                }
                if (dataArray[i] < Min && dataArray[i]!=0)
                {
                    Min = dataArray[i];
                }

                index += 2;
            }

            return GetBitmap(dataArray, Width, Height, bmpWidth, bmpHeight);
        }

        private Bitmap GetBitmap(float[] fData, int Width, int Height, int bmpWidth, int bmpHeight)
        {
            byte[] imageArray = new byte[(Width * Height) * 4];
            int num = 4;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int curAddress = (i * Width) + j;
                    int index = (int)(((fData[curAddress] - Min) * 255f) / (Max - Min));
                    if (index > 255) { index = 255; }
                    if (index < 0) { index = 0; }
                    imageArray[(i * (Width * num)) + (j * num)] = this.rgbRED[index];
                    imageArray[(i * (Width * num)) + ((j * num) + 1)] = this.rgbGREEN[index];
                    imageArray[(i * (Width * num)) + ((j * num) + 2)] = this.rgbBLUE[index];
                    imageArray[(i * (Width * num)) + ((j * num) + 3)] = 0xff;


                }
            }

            Bitmap bmp = new Bitmap(bmpWidth, bmpHeight);
            System.Drawing.Imaging.BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            //set the number of bytes per pixel
            int pixelSize = 4;

            unsafe
            {
                for (int y = 0; y < Height; y++)
                {
                    //get the data from the original image
                    byte* Row = (byte*)bmd.Scan0 +
                       (y * bmd.Stride);

                    for (int x = 0; x < Width; x++)
                    {
                        //set the image's pixel channels
                        Row[x * pixelSize] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 2)]; //B
                        Row[x * pixelSize + 1] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 1)]; //G
                        Row[x * pixelSize + 2] = imageArray[y * (Width * pixelSize) + (x * pixelSize)]; //R
                        Row[x * pixelSize + 3] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 3)]; //A
                    }
                }
            }

            //unlock the bitmaps
            bmp.UnlockBits(bmd);

            return bmp;
        }

        private void CreateDebugFile(string debugData)
        {
            //string sampleFilePath;

            //sampleFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //sampleFilePath += "\\debug_thermal.txt";

            //using (StreamWriter sw = File.AppendText(sampleFilePath))
            //{
            //    sw.WriteLine(debugData);
            //}
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateDebugFile("DoWork called at " + DateTime.Now.ToLongTimeString());
            if (!isCreatingAVI)
            {
                CreateDebugFile("Writing AVI at " + DateTime.Now.ToLongTimeString());
                isCreatingAVI = true;
                FileStream FS = new FileStream(saveFileName2, FileMode.Open, FileAccess.Read);

                if (saveFileName == null)
                {
                    saveFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                    + "\\PerspirationVideo.avi";
                }
                if (System.IO.File.Exists(saveFileName))
                {
                    System.IO.File.Delete(saveFileName);
                }

                int bmpWidth = imgWidth + imgWidth%2;
                int bmpHeight = imgHeight + imgHeight%2;

                writer.Open(saveFileName, bmpWidth, bmpHeight, 7, VideoCodec.MPEG4);
                
                for (int i = 0; i < frameCount; i++)
                {
                    /*
                    if(!isInDroppableFrames(i))
                    {
                        Bitmap bmp = GetFrame(FS, i, imgWidth, imgHeight, bmpWidth, bmpHeight);
                        writer.WriteVideoFrame(bmp);
                    }
                    */
                    Bitmap bmp = GetFrame(FS, i, imgWidth, imgHeight, bmpWidth, bmpHeight);
                    writer.WriteVideoFrame(bmp);
                }

                writer.Close();

                if (FS != null)
                {
                    FS.Close();
                    FS = null;
                }

                string infFileName = saveFileName + ".inf";

                if (System.IO.File.Exists(infFileName))
                {
                    System.IO.File.Delete(infFileName);
                }

                string infStr = "";
                infStr += timeStampArray.Count.ToString() + "\n";
                foreach (string timeStamp in timeStampArray)
                {
                    infStr += "\n" + timeStamp;
                }
                File.WriteAllText(infFileName, infStr);

                /*
                // Write the string to a file.
                string testFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                    + "\\Test.txt";
                System.IO.StreamWriter file = new System.IO.StreamWriter(testFileName);

                file.WriteLine("<< " + frameCount + " >>");
                file.WriteLine("<< " + processedFrames.Count + " >>");
                foreach (int frameNum in processedFrames)
                {
                    file.WriteLine(frameNum.ToString());
                }
                file.Close();
                */

                /*
                if (aviManager == null)
                {
                    if (saveFileName == null)
                    {
                        saveFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                                        + "\\PerspirationVideo.avi";
                    }
                    if (System.IO.File.Exists(saveFileName))
                    {
                        System.IO.File.Delete(saveFileName);
                    }
                    aviManager = new AviManager(saveFileName, false);
                    System.Threading.Thread.Sleep(5);
                    // aviStream = aviManager.AddVideoStream(false, 25, curBitmap);
                    //aviStream = aviManager.AddVideoStream(true, frameRate, (Bitmap)myQ.Dequeue());
                    Bitmap bmp = GetFrame(FS, 0, imgWidth, imgHeight);
                    aviStream = aviManager.AddVideoStream(true, frameRate, bmp);
                }

                
                for(int i=1; i<frameCount; i++)
                {
                    Bitmap bmp = GetFrame(FS, i, imgWidth, imgHeight);
                    aviStream.AddFrame(bmp);
                }

                if (aviManager != null)
                {
                    aviManager.Close();
                    aviManager = null;
                    aviStream = null;
                }

                if (FS != null)
                {
                    FS.Close();
                    FS = null;
                }

                */
            }

        }

        private void bw_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowHideWaitForm(false);
            this.Host.SendData(UploadAVIOutputPin, new IntegerData(1), this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        public AVIWriterCompressMulti()
        {
            appendString = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public AVIWriterCompressMulti(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //ThresholdVal = (int)info.GetInt64("title");
            try
            {
                string tempStr;
                tempStr = (string)info.GetString("appendStr");
                if (tempStr != null)
                {
                    appendString = tempStr;
                    appendStringChanged(appendString);
                }
            }
            catch
            {

            }

        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //call the base
            //ISerializable.GetObjectData(info, context);
            info.AddValue("appendStr", appendString);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////         
    }
}
