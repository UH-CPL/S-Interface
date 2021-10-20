using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.Serialization;


namespace ThermalVisualizerPlugin
{
    public delegate void FloatDelegate(float value);
    public delegate void BoolDelegate(bool value);

    [Serializable]
    public class ThermalVisualizer : IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "ThermalVisualizer";
        string myDescription = "Visualizing Thermal Data";
        string myAuthor = "Dvijesh Shastri";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        int myID = -1;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();

               
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
        #region ThermalVisualizer Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        
        protected IPin thermalImageInputPin;
        protected IPin imageOutputPin;
        private byte[] rgbRED, rgbGREEN, rgbBLUE;
        //private static Mutex mut;
        protected float MaxTemp;
        protected float MinTemp;
        public bool isSerialized = false;
        public bool isFirstFrame = true;
        bool isOptimized;
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        public event FloatDelegate MaxTempChanged;
        public event FloatDelegate MinTempChanged;
        public event BoolDelegate OptimizedChanged;

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Properties
        ///////////////////////////////////////////////////////////////////////////////////////////

        public float MaxTempProperty
        {
            get { return MaxTemp; }
            set
            {
                MaxTemp = value;
                if (MaxTempChanged != null)
                { MaxTempChanged(MaxTemp); }
            }
        }

        public bool IsOptimized
        {
            get { return isOptimized; }
            set
            {
                isOptimized = value;
                
            }
        }

        public float MinTempProperty
        {
            get { return MinTemp; }
            set
            {
                MinTemp = value;
                if (MinTempChanged != null)
                { MinTempChanged(MinTemp); }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Plugin Required Methods
        ///////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first

            thermalImageInputPin = Host.LoadOrCreatePin("Thermal", PinCategory.Critical, new Type[] { typeof(IThermalData) });
            inPins.Add(thermalImageInputPin);

            

            imageOutputPin = Host.LoadOrCreatePin("Image", PinCategory.Optional, new Type[] { typeof(IImageData) });
            outPins.Add(imageOutputPin);

            myMainInterface = new ThermalVisualizerControl(this);
            rgbRED   = new byte[256];
            rgbGREEN = new byte[256];
            rgbBLUE  = new byte[256];

            MaxTempProperty = 40.0F;
            MinTempProperty = 18.0F; 

            //mut = new Mutex();
           
            FillRGB();

            if (!isSerialized)
            {
                IsOptimized = false;
            }

            OptimizedChanged(IsOptimized);
            
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            //Put process code here

            if (pin == thermalImageInputPin)
            {
                if (IsOptimized)
                {
                    PrepareHalfVisualization((ThermalData)input);
                }
                else
                {
                    PrepareVisualization((ThermalData)input);
                }
                this.Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);
                
                //if (mut.WaitOne())
                //{
                    //PrepareVisualization((ThermalData)input);
                    //PrepareHalfVisualization((ThermalData)input);
                    //this.Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);
                    //mut.ReleaseMutex();
                //}
                
                //this.Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);
            }
        }

        void PrepareVisualization(ThermalData thermalData)
        {
            if (thermalData != null)
            {

                if(isFirstFrame)
                {
                    FindMaxMin(thermalData);
                    isFirstFrame = false;
                }
                int num = 4;
                byte[] imageArray = new byte[(thermalData.Width * thermalData.Height) * num];
               
                int Height = thermalData.Height;
                int Width = thermalData.Width;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {

                        if (thermalData.Data[(i * Width) + j] > MinTempProperty && thermalData.Data[(i * Width) + j] < MaxTempProperty)
                        {
                            int index = (int)(((thermalData.Data[(i * Width) + j] - MinTempProperty) * 255f) / (MaxTempProperty - MinTempProperty));
                            if (index > 255) { index = 255; }
                            if (index < 0) { index = 0; }
                            imageArray[(i * (Width * num)) + (j * num)] = this.rgbRED[index];
                            imageArray[(i * (Width * num)) + ((j * num) + 1)] = this.rgbGREEN[index];
                            imageArray[(i * (Width * num)) + ((j * num) + 2)] = this.rgbBLUE[index];
                            imageArray[(i * (Width * num)) + ((j * num) + 3)] = 0xff;
                        }
                        else if (thermalData.Data[(i * Width) + j] < MinTempProperty)
                        {
                            imageArray[(i * (Width * num)) + (j * num)] = 0; //Red
                            imageArray[(i * (Width * num)) + ((j * num) + 1)] = 0; //Green
                            imageArray[(i * (Width * num)) + ((j * num) + 2)] = 255;//Blue
                            imageArray[(i * (Width * num)) + ((j * num) + 3)] = 0xff;//
                        }
                        else if (thermalData.Data[(i * Width) + j] > MaxTempProperty)
                        {
                            imageArray[(i * (Width * num)) + (j * num)] = 255; //Red
                            imageArray[(i * (Width * num)) + ((j * num) + 1)] = 0; //Green
                            imageArray[(i * (Width * num)) + ((j * num) + 2)] = 0; //Blue
                            imageArray[(i * (Width * num)) + ((j * num) + 3)] = 0xff;
                        }
                    }
                }

                this.Host.SendData(imageOutputPin, new ImageData(imageArray, Height, Width), this);
            }
            else
            { this.Host.SendData(imageOutputPin, null, this); }


        }


        void PrepareHalfVisualization(ThermalData thermalData)
        {
            if (thermalData != null)
            {
                if(isFirstFrame)
                {
                    FindMaxMin(thermalData);
                    isFirstFrame = false;
                }
                int num = 4;
                int reduction = 2;

                int imgHeight = thermalData.Height / reduction;
                int imgWidth = thermalData.Width / reduction;

                

                byte[] imageArray = new byte[(imgWidth * imgHeight) * num];


                for (int i = 0; i < imgHeight; i++)
                {
                    for (int j = 0; j < imgWidth; j++)
                    {
                        int curAddress = ((i * reduction) * (thermalData.Width)) + (j * reduction);
                        if (thermalData.Data[curAddress] > MinTempProperty && thermalData.Data[curAddress] < MaxTempProperty)
                        {
                            int index = (int)(((thermalData.Data[curAddress] - MinTempProperty) * 255f) / (MaxTempProperty - MinTempProperty));
                            if (index > 255) { index = 255; }
                            if (index < 0) { index = 0; }
                            imageArray[(i * (imgWidth * num)) + (j * num)] = this.rgbRED[index];
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 1)] = this.rgbGREEN[index];
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 2)] = this.rgbBLUE[index];
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 3)] = 0xff;
                        }
                        else if (thermalData.Data[curAddress] < MinTempProperty)
                        {
                            imageArray[(i * (imgWidth * num)) + (j * num)] = 0; //Red
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 1)] = 0; //Green
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 2)] = 255;//Blue
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 3)] = 0xff;//
                        }
                        else if (thermalData.Data[curAddress] > MaxTempProperty)
                        {
                            imageArray[(i * (imgWidth * num)) + (j * num)] = 255; //Red
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 1)] = 0; //Green
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 2)] = 0; //Blue
                            imageArray[(i * (imgWidth * num)) + ((j * num) + 3)] = 0xff;
                        }

                        
                    }
                }

                ImageData sendImage = new ImageData(imageArray, imgHeight, imgWidth);
                sendImage.Scale = reduction;
                this.Host.SendData(imageOutputPin, sendImage, this);
            }
            else
            { this.Host.SendData(imageOutputPin, null, this); }

            /*

                if (isFirstFrame)
                {
                    FindMaxMin(thermalData);
                    isFirstFrame = false;
                }
                int num = 4;
                int reduction = 2;

                int Height = thermalData.Height / reduction;
                int Width = thermalData.Width / reduction;

                if (curImage == null)
                {
                    
                    
                    byte[] byteArray = new byte[(Height * Width) * num];
                    curImage = new ImageData(byteArray, Height, Width);
                }

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        int curAddress = ((i * reduction) * (Width * reduction)) + (j * reduction);
                        if (thermalData.Data[curAddress] > MinTempProperty && thermalData.Data[curAddress] < MaxTempProperty)
                        {
                            int index = (int)(((thermalData.Data[curAddress] - MinTempProperty) * 255f) / (MaxTempProperty - MinTempProperty));
                            if (index > 255) { index = 255; }
                            if (index < 0) { index = 0; }
                            curImage.Data[(i * (Width * num)) + (j * num)] = this.rgbRED[index];
                            curImage.Data[(i * (Width * num)) + ((j * num) + 1)] = this.rgbGREEN[index];
                            curImage.Data[(i * (Width * num)) + ((j * num) + 2)] = this.rgbBLUE[index];
                            curImage.Data[(i * (Width * num)) + ((j * num) + 3)] = 0xff;
                        }
                        else if (thermalData.Data[curAddress] < MinTempProperty)
                        {
                            curImage.Data[(i * (Width * num)) + (j * num)] = 0; //Red
                            curImage.Data[(i * (Width * num)) + ((j * num) + 1)] = 0; //Green
                            curImage.Data[(i * (Width * num)) + ((j * num) + 2)] = 255;//Blue
                            curImage.Data[(i * (Width * num)) + ((j * num) + 3)] = 0xff;//
                        }
                        else if (thermalData.Data[curAddress] > MaxTempProperty)
                        {
                            curImage.Data[(i * (Width * num)) + (j * num)] = 255; //Red
                            curImage.Data[(i * (Width * num)) + ((j * num) + 1)] = 0; //Green
                            curImage.Data[(i * (Width * num)) + ((j * num) + 2)] = 0; //Blue
                            curImage.Data[(i * (Width * num)) + ((j * num) + 3)] = 0xff;
                        }
                    }
                }

                this.Host.SendData(imageOutputPin, curImage, this);
            }
            else
            { this.Host.SendData(imageOutputPin, null, this); }

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

        public void FindMaxMin(ThermalData thermalData)
        {
            int Height = thermalData.Height;
            int Width = thermalData.Width;
            float tMin = float.MaxValue;
            float tMax = float.MinValue;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if ((thermalData.Data[(i * Width) + j] < tMin)&&(thermalData.Data[(i * Width) + j] > 20)&&(thermalData.Data[(i * Width) + j] !=0))
                        { tMin = thermalData.Data[(i * Width) + j]; }

                        if ((thermalData.Data[(i * Width) + j] > tMax)&&(thermalData.Data[(i * Width) + j] < 40)&&(thermalData.Data[(i * Width) + j] !=0))
                        { tMax = thermalData.Data[(i * Width) + j]; }
                    }
                }

                MinTempProperty = tMin;
                MaxTempProperty = tMax;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

           ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public ThermalVisualizer()
        {
            //
            // TODO: Add constructor logic here
            //
           


        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public ThermalVisualizer(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                IsOptimized = (bool)info.GetBoolean("isoptimized");
                isSerialized = true;
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
            info.AddValue("isoptimized", IsOptimized);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}
