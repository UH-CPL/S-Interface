using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Threading;

namespace Thermal_File
{
    [Serializable]
    public class ThermalFile : IPlugin, ISerializable
    {
        
		
		//Declarations of all our internal plugin variables
		string myName = "Thermal File";
		string myDescription = "Read thermal data file";
		string myAuthor = "Peggy Lindner";
		string myVersion = "1.0.1";
		IPluginHost myHost = null;
        int myID = -1;
		System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();

        IPin outImage = null;
        IPin outThermal = null;
        IPin outFrameNum = null;
        IPin outFileName = null;
        IPin outTotalFrames = null;
        IPin outTimeStamp = null;
        IPin outFrameInfo = null;

        string datFilename = null;
        string infFilename = null;
        string calibrationDataFile = null;
        string sfmovFilename = null;
        FileStream FS = null;
        int MyWidth = 0;
        int MyHeight = 0;
        int myTotFrames = 0;
        byte[] rgbRED = new byte[256];
        byte[] rgbGREEN = new byte[256];
        byte[] rgbBLUE = new byte[256];
        DateTime[] tsArray = null;
        int curFrameNum = 0;
        public bool Play = false;
        float Min;
        float Max;
        FrameInfo frameInfo = new FrameInfo();
        public int CurSelection;
        bool isSerialized = false;

        private iacfUtil.File file = new iacfUtil.File();
        private iacfUtil.ObjectParameters objPars = new iacfUtil.ObjectParameters();
        private bool calibrated = false;
        private ushort[] frame = null;
        private byte[] imgArray;
        float coldTemp = 0;
        float coldCount = 0;
        float hotTemp = 0;
        float hotCount = 0;
        ThermalData tempData = null;

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////
        public event IntDelegate TotalFramesChanged;
        public event IntDelegate CurFrameNumChanged;
        public event IntDelegate CurSelectionChanged;
        public event StringDelegate CurTimeStampChanged;
        public event StringDelegate TotalLengthChanged;
       

        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public int MyTotFrames
        {
            get { return myTotFrames; }
            set 
            { 
                myTotFrames = value;
                if (TotalFramesChanged != null)
                {
                    TotalFramesChanged(myTotFrames);
                }
            }
        }

        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public int CurFrameNum
        {
            get { return curFrameNum; }
            set
            {
                curFrameNum = value;
                if (CurFrameNumChanged != null)
                {
                    CurFrameNumChanged(curFrameNum);
                }
            }
        }

        
		/// <summary>
		/// Description of the Plugin's purpose
		/// </summary>
		public string Description
		{
			get {return myDescription;}
		}

		/// <summary>
		/// Author of the plugin
		/// </summary>
		public string Author
		{
			get	{return myAuthor;}
		
		}

		/// <summary>
		/// Host of the plugin.
		/// </summary>
		public IPluginHost Host
		{
			get {return myHost;}
			set	{myHost = value;}
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
			get {return myName;}
		}

		public System.Windows.Forms.UserControl MainInterface
		{
			get {return myMainInterface;}
		}

		public string Version
		{
			get	{return myVersion;}
		}

        public ArrayList InputPins
        {
            get { return inPins; }
        }

        public ArrayList OutputPins
        {
            get { return outPins; }
        }
		
		public void Initialize()
		{
			//This is the first Function called by the host...
			//Put anything needed to start with here first
            calibrationDataFile = Application.StartupPath + @"\Data\CalibrationData.txt";
            outImage = Host.LoadOrCreatePin("Image", PinCategory.Optional, new Type[] { typeof(IImageData) });
            outPins.Add(outImage);
            outThermal = Host.LoadOrCreatePin("Thermal Data", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            outPins.Add(outThermal);
            outFileName = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            outPins.Add(outFileName);
            outFrameNum = Host.LoadOrCreatePin("Frame Number", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(outFrameNum);
            outTimeStamp = Host.LoadOrCreatePin("Time Stamp", PinCategory.Optional, new Type[] { typeof(IDateTimeData) });
            outPins.Add(outTimeStamp);
            outTotalFrames = Host.LoadOrCreatePin("Total Frames", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(outTotalFrames);
            outFrameInfo = Host.LoadOrCreatePin("FrameInfo", PinCategory.Optional, new Type[] { typeof(IFrameInfo) });
            outPins.Add(outFrameInfo);
           
            myMainInterface = new Thermal_File_Control(this);

            FillRGB();
            objPars.Default();
            //Directory.SetCurrentDirectory(@"C:\Users\Athemosadmin\Desktop\OTACS\Other\");
            //ReadCalibrationData();

            
            CurSelection = 0;
           
		}
		
		public void Dispose()
		{
			//Put any cleanup code in here for when the program is stopped
            if (FS != null)
            {
                FS.Close();
            }
		}

        public void Process(IPin pin, IPinData input)
        {
            //Thread.Sleep(50);
            //Put process code here

            if (CurSelection == 0)
            {
                if (tsArray != null && datFilename != null && CurFrameNum < MyTotFrames)
                {


                    float[] dataFloatArray = GetFrame(FS, CurFrameNum, MyWidth, MyHeight);
                    this.Host.SendData(outThermal, new ThermalData(dataFloatArray, MyHeight, MyWidth), this);

                    double curTime = ((TimeSpan)tsArray[CurFrameNum].Subtract(tsArray[0])).TotalSeconds;
                    if (CurTimeStampChanged != null)
                    {
                        CurTimeStampChanged(curTime.ToString() + "         ");
                    }
                    this.Host.SendData(outTimeStamp, new DateTimeData(tsArray[CurFrameNum]), this);
                    this.Host.SendData(outFrameNum, new IntegerData(CurFrameNum), this);
                    frameInfo.FrameNumber = CurFrameNum;
                    frameInfo.Span = (TimeSpan)tsArray[CurFrameNum].Subtract(tsArray[0]);
                    frameInfo.Time = tsArray[CurFrameNum];
                    this.Host.SendData(outFrameInfo, frameInfo, this);

                    if (outImage.Connected)
                    {
                        byte[] imgArray = new byte[MyWidth * MyHeight * 4];
                        ColorMap(dataFloatArray, ref imgArray, MyWidth, MyHeight);
                        this.Host.SendData(outImage, new ImageData(imgArray, MyHeight, MyWidth), this);
                    }

                    if (Play)
                    {
                        CurFrameNum++;
                    }
                }
                else
                {
                    if (CurFrameNum >= MyTotFrames)
                    {
                        if (FS != null)
                            FS.Close();
                    }
                }
            }
            else if (CurSelection == 1)
            {
                if (tsArray != null && sfmovFilename != null && CurFrameNum < MyTotFrames)
                {
                    ulong timestamp = 0;
                    file.Read((uint)CurFrameNum, ref frame, out timestamp);

                    tsArray[CurFrameNum] = DateTime.FromFileTimeUtc((long)timestamp);

                    if (outThermal.Connected)
                    {
                        ConvertToTemperatures();
                        this.Host.SendData(outThermal, tempData, this);
                    }

                    //    float[] dataFloatArray = GetFrame(FS, CurFrameNum, MyWidth, MyHeight);
                    //    this.Host.SendData(outThermal, new ThermalData(dataFloatArray, MyHeight, MyWidth), this);

                    double curTime = ((TimeSpan)tsArray[CurFrameNum].Subtract(tsArray[0])).TotalSeconds;
                    if (CurTimeStampChanged != null)
                    {
                        CurTimeStampChanged(curTime.ToString("#0.00") + "         ");
                    }
                    this.Host.SendData(outTimeStamp, new DateTimeData(tsArray[CurFrameNum]), this);
                    this.Host.SendData(outFrameNum, new IntegerData(CurFrameNum), this);
                    frameInfo.FrameNumber = CurFrameNum;
                    frameInfo.Span = (TimeSpan)tsArray[CurFrameNum].Subtract(tsArray[0]);
                    frameInfo.Time = tsArray[CurFrameNum];
                    this.Host.SendData(outFrameInfo, frameInfo, this);

                    if (outImage.Connected)
                    {
                        SFMOVColorMap(frame, ref imgArray, MyWidth, MyHeight);
                        this.Host.SendData(outImage, new ImageData(imgArray, MyHeight, MyWidth), this);
                    }

                    if (Play)
                    {
                        CurFrameNum++;
                    }
                }
            }
           
        }

        public void SFMOVColorMap(ushort[] dataArray, ref byte[] imageArray, int Width, int Height)
        {
            int pixelSize = 4;
            Min = dataArray.Min();
            Max = dataArray.Max();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int iPixelValue = (int)((dataArray[i * Width + j] - Min) * 255 / (Max - Min));
                    imageArray[i * (Width * pixelSize) + (j * pixelSize)] = rgbRED[iPixelValue];
                    imageArray[i * (Width * pixelSize) + (j * pixelSize + 1)] = rgbGREEN[iPixelValue];
                    imageArray[i * (Width * pixelSize) + (j * pixelSize + 2)] = rgbBLUE[iPixelValue];
                    imageArray[i * (Width * pixelSize) + (j * pixelSize + 3)] = 255;
                }
            }
        }

        /// <summary>
        /// Read thermal data from smfov file type.
        /// </summary>
        /// <param name="fileName">Path to the smfov file.</param>
        public void ReadSFMOVFile(string fileName)
        {
            sfmovFilename = fileName;

            if (!file.Open(sfmovFilename))
            {
                return;
            }
            String dirctory = Path.GetDirectoryName(sfmovFilename);
            String calfile = dirctory + "\\CalibrationData.txt";
            if (File.Exists(calfile))
            {
                calibrationDataFile = calfile;
                ReadCalibrationData();
            }
            else
            {
                //if no calibration file was found with the SFMOV file
                //Then read the default calibration file that is in the OTACS folder
                ReadCalibrationData();
            }


            file.ObjectParameters = objPars;

            ushort width, height;
            uint totframes;
            file.Info(out width, out height, out totframes, out calibrated);

            MyWidth = width;
            MyHeight = height;
            MyTotFrames = (int)totframes;

            this.Host.SendData(outFileName, new StringData(sfmovFilename), this);
            this.Host.SendData(outTotalFrames, new IntegerData(MyTotFrames), this);

            tsArray = new DateTime[MyTotFrames];
            frame = new ushort[width * height];
            if (outImage.Connected)
            {
                imgArray = new byte[MyWidth * MyHeight * 4];
            }

            if (outThermal.Connected)
            {
                tempData = new ThermalData();
                tempData.Height = height;
                tempData.Width = width;
                tempData.Data = new float[height * width];
            }

            Play = true;
        }

        public void ReadDatFiles(string fileName)
        {
            datFilename = fileName;
            infFilename = datFilename.Substring(0, datFilename.LastIndexOf('.'));
            infFilename += ".inf";
            int Width = 0, Height = 0, TotFrames = 0;
            tsArray = ReadInfFile(infFilename, ref Width, ref Height, ref TotFrames);

            if (tsArray != null)
            {
                MyWidth = Width;
                MyHeight = Height;
                MyTotFrames = TotFrames;
                CurFrameNum = 0;
                Play = true;

                double length = ((TimeSpan)tsArray[TotFrames - 1].Subtract(tsArray[0])).TotalSeconds;
                if (TotalLengthChanged != null)
                {
                    TotalLengthChanged(length.ToString() + "   seconds");
                }
                this.Host.SendData(outFileName, new StringData(datFilename), this);
                this.Host.SendData(outTotalFrames, new IntegerData(MyTotFrames), this);

                if (FS != null)
                {
                    FS.Close();
                    FS = null;
                }


                FS = new FileStream(datFilename, FileMode.Open, FileAccess.Read);
                //filePath, FileMode.Open, FileAccess.Read
            }

            //Play = true;
        }

        public DateTime[] ReadInfFile(string fileName, ref int Width, ref int Height, ref int TotalFrames)
        {
            DateTime[] timeStampArray = null;

            if (fileName != null)
            {
                // Variable declarations 
                int parseCounter = 0;
                int timeStampCounter = 0;
                int Month = 12; // Deafault Month
                StreamReader SR;
                string S;
                char[] delimiterChars = { ' ', '\t', ':', '.' };
                TotalFrames = 0;


                // Open the .inf file
                SR = File.OpenText(fileName);

                // read each line from the inf file
                while ((S = SR.ReadLine()) != null)
                {
                    // Tokenize the words in each line
                    string[] words = S.Split(delimiterChars);

                    // Very first line contains the total number of frames
                    if (parseCounter == 0)
                    {
                        // Copy the total frames into the variable
                        TotalFrames = int.Parse(words[0]);
                        timeStampArray = new DateTime[TotalFrames];
                        parseCounter++;
                    }
                    else if (parseCounter == 1)
                    {
                        // Copy the Width into the variable
                        Width = int.Parse(words[0]);
                        parseCounter++;
                    }
                    else if (parseCounter == 2)
                    {
                        // Copy the Height into the variable
                        Height = int.Parse(words[0]);
                        parseCounter++;
                    }
                    else if (parseCounter == 3)
                    {
                        //Skip
                        parseCounter++;
                    }
                    else if (parseCounter == 4)
                    {
                        // parse the timestamp on each line

                        if (words[1] == "Jan")
                            Month = 1;
                        else if (words[1] == "Feb")
                            Month = 2;
                        else if (words[1] == "Mar")
                            Month = 3;
                        else if (words[1] == "Apr")
                            Month = 4;
                        else if (words[1] == "May")
                            Month = 5;
                        else if (words[1] == "Jun")
                            Month = 6;
                        else if (words[1] == "Jul")
                            Month = 7;
                        else if (words[1] == "Aug")
                            Month = 8;
                        else if (words[1] == "Sep")
                            Month = 9;
                        else if (words[1] == "Oct")
                            Month = 10;
                        else if (words[1] == "Nov")
                            Month = 11;
                        else if (words[1] == "Dec")
                            Month = 12;

                        // Copy the timestamp
                        timeStampArray[timeStampCounter] = new DateTime(int.Parse(words[7]), Month, int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]), int.Parse(words[6]));
                        timeStampCounter++;

                        // Increase counter
                        parseCounter++;
                    }
                    else
                    {
                        // read timestamp for remaining frames
                        if (timeStampCounter < TotalFrames)
                        {
                            // Copy the timestamp of first frame so that we can find the offset for remaining frames
                            timeStampArray[timeStampCounter] = new DateTime(int.Parse(words[7]), Month, int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]), int.Parse(words[6]));
                            timeStampCounter++;
                            // Increase counter
                            parseCounter++;
                        }

                    }
                }


                SR.Close();
            }

            return timeStampArray;
        }


        public void FillRGB()
        {
            for (int i = 0; i <= 32; i++)
            {
                int iRedValue = (int)i * 255 / 48;


                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = 1;
                rgbBLUE[i] = 1;

            }

            for (int i = 33; i <= 48; i++)
            {
                int iRedValue = (int)i * 255 / 48;
                int iGreenValue = (int)(i - 33) * 255 / (112 - 33);


                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = 1;

            }

            for (int i = 49; i <= 112; i++)
            {
                int iGreenValue = (int)(i - 33) * 255 / (112 - 33);
                rgbRED[i] = 255;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = 1;

            }

            for (int i = 113; i <= 128; i++)
            {
                int iRedValue = (int)(144 - i) * 255 / (144 - 113);

                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = 255;
                rgbBLUE[i] = 1;

            }

            for (int i = 129; i <= 144; i++)
            {
                int iRedValue = (int)(144 - i) * 255 / (144 - 113);
                int iBlueValue = (int)(i - 129) * 255 / (176 - 129);

                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = 255;
                rgbBLUE[i] = (byte)iBlueValue;

            }

            for (int i = 145; i <= 160; i++)
            {
                int iGreenValue = (int)(192 - i) * 255 / (192 - 145);
                int iBlueValue = (int)(i - 129) * 255 / (176 - 129);

                rgbRED[i] = 1;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = (byte)iBlueValue;

            }

            for (int i = 161; i <= 176; i++)
            {
                int iRedValue = (int)(i - 161) * 255 / (208 - 161);
                int iGreenValue = (int)(192 - i) * 255 / (192 - 145);
                int iBlueValue = (int)(i - 129) * 255 / (176 - 129);

                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = (byte)iBlueValue;

            }

            for (int i = 177; i <= 192; i++)
            {
                int iRedValue = (int)(i - 161) * 255 / (208 - 161);
                int iGreenValue = (int)(192 - i) * 255 / (192 - 145);

                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = 255;
            }

            for (int i = 193; i <= 208; i++)
            {
                int iRedValue = (int)(i - 161) * 255 / (208 - 161);


                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = 1;
                rgbBLUE[i] = 255;

            }

            for (int i = 209; i <= 224; i++)
            {

                int iGreenValue = (int)(i - 209) * 255 / (255 - 209);


                rgbRED[i] = 255;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = 255;

            }

            for (int i = 225; i <= 255; i++)
            {
                int iRedValue = (int)(255 - i) * 255 / (255 - 225);
                int iGreenValue = (int)(i - 209) * 255 / (255 - 209);

                rgbRED[i] = (byte)iRedValue;
                rgbGREEN[i] = (byte)iGreenValue;
                rgbBLUE[i] = 255;
            }
        }

        public void ColorMap(float[] dataArray, ref byte[] imageArray, int Width, int Height)
        {
            /*
            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            //set the number of bytes per pixel
            int pixelSize = 4;

            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    //get the data from the original image
                    byte* Row = (byte*)bmd.Scan0 +
                       (y * bmd.Stride);

                    for (int x = 0; x < bmd.Width; x++)
                    {
                        int iPixelValue = (int)((dataArray[y * Width + x] - Min) * 255 / (Max - Min));

                        //set the image's pixel to black
                        Row[x * pixelSize] = rgbBLUE[iPixelValue]; //B
                        Row[x * pixelSize + 1] = rgbGREEN[iPixelValue]; //G
                        Row[x * pixelSize + 2] = rgbRED[iPixelValue]; //R
                        Row[x * pixelSize + 3] = 255; //A
                    }
                }
            }

            //unlock the bitmaps
            bmp.UnlockBits(bmd);

            return bmp;
             */


            int pixelSize = 4;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int iPixelValue = (int)((dataArray[i * Width + j] - Min) * 255 / (Max - Min));
                    //Hadi: try and catch are added
                    try
                    {
                        imageArray[i * (Width * pixelSize) + (j * pixelSize)] = rgbRED[iPixelValue];
                        imageArray[i * (Width * pixelSize) + (j * pixelSize + 1)] = rgbGREEN[iPixelValue];
                        imageArray[i * (Width * pixelSize) + (j * pixelSize + 2)] = rgbBLUE[iPixelValue];
                        imageArray[i * (Width * pixelSize) + (j * pixelSize + 3)] = 255;
                    }
                    catch (IndexOutOfRangeException e)
                    { 
                    }
                    //Hadi/
                }
            }


        }

        public float[] GetFrame(FileStream FS, int frameNum, int Width, int Height)
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
                
                if (dataArray[i] > Max)
                {
                    Max = dataArray[i];
                }
                if (dataArray[i] < Min)
                {
                    Min = dataArray[i];
                }
                 
                index += 2;
            }

            return dataArray;
        }



        private void ReadCalibrationData()
        {
            try
            {
                StreamReader sr = File.OpenText(calibrationDataFile);
                coldTemp = float.Parse(sr.ReadLine());
                coldCount = float.Parse(sr.ReadLine());
                hotTemp = float.Parse(sr.ReadLine());
                hotCount = float.Parse(sr.ReadLine());
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Can't find calibration data file.");
            } 
        }


        private void ConvertToTemperatures()
        {
            for (int x = 0; x < MyHeight; x++)
            {
                for (int y = 0; y < MyWidth; y++)
                {
                    float curCount = (float)frame[x * MyWidth + y] + 1500;
                    float temp = (((curCount - coldCount) * hotTemp) + ((hotCount - curCount) * coldTemp)) / (hotCount - coldCount);
                    if (temp < 0)
                    {
                        temp = 0;
                    }
                    tempData.Data[x * MyWidth + y] = temp - 10;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public ThermalFile()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
        public ThermalFile(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //Initialize();
            try
            {
                CurSelection = (int)info.GetInt64("selection");
                isSerialized = true;
            }
            catch
            {
                isSerialized = false;
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //call the base
            //base.GetObjectData(info, context);
            info.AddValue("selection", CurSelection);
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}
