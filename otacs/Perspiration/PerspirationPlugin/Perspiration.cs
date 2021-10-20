using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;


namespace PerspirationPlugin
{

    public delegate void BoolDelegate(bool value);

    public class LinearPolynomial
    {
        public float m; // Slope
        public float c; // Intercept 
    }

    [Serializable]
    public class Perspiration : IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "Perspiration";
        string myDescription = "Perspiration Computation";
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
        #region Perspiration Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        //Input pins 
        protected IPin thermalImageInputPin;
        protected IPin mouseInputPin;
        // protected IPin trackerFittingInputPin;
        protected IPin fileNameInputPin;
        protected IPin frameNumberInputPin;
        //protected IPin recOverlayInputPin;
        protected IPin ProcessInitInputPin;

        //Output pins 
        protected IPin RawEnergyOutputPin;
        protected IPin WaveletEnergyOutputPin;
        protected IPin numOfSegPixelsPin;
        protected IPin overlayOutputPin;
        protected IPin ROIOutputPin;
        protected IPin TrackFileInitOutputPin;
        protected IPin RateOutputPin;

        //IPin inImage = null;
        //IPin outImage = null;
        //int Perspirationval = 100;

        //Data array 
        //protected OverlayImageData overlay;
        //protected OverlayImageData overlayMaxillaryROI;
        protected ThermalData outputROI;

        FloatData persPixels;
        FloatData measurment;
        FloatData measurmentGSR;

        //PointF[] ROI_Corners;
        //IOverlayPolygonData ROI;

        bool showPerspiration;
        bool normalizedEnergy;


        public ArrayList timeGSR;
        public ArrayList dataGSRRaw;
        public float[,] dataGSRRaw_resampled;

        bool isGSRFileExisit;

        private static Mutex mut;
        private static Mutex mutWriteFile;

        int frameCounter;

        //float trackerFitting;
        string thermalFileName;
        string thermalFilePath;
        string thermalFileNameOnly;

        int frameNumber;
        //DateTime timeStamp;

        string TSR_FileName;
        string GSR_FileName;

        float[,] MaxillaryROI = null;
        float[,] PerspirationROI = null;

        //bool isReset;
        int processStarted;
        bool isOverlayClean;
        int TrackFileInitiator;

        int[,] b;
        int[,] db;
        //File Pointer for Thermal Perspiration 
        StreamWriter SR_TSR;

        //File Pointer for GSR
        StreamWriter SR_GSR;

        PointF MROITopLeft;
        int MROIHeight;
        int MROIWidth;
        bool MROITopLeftSelected;

        bool isSerialized = false;
        public bool normalizedEnergyPropertySerial;


        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        public event BoolDelegate normalizedEnergyChanged;

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Properties
        ///////////////////////////////////////////////////////////////////////////////////////////

        public bool showPerspirationProperty
        {
            get { return showPerspiration; }
            set
            {
                showPerspiration = value;
                //if (showPerspirationPathChanged != null)
                //{ showPerspirationPathChanged(showPerspiration); }
            }
        }

        public bool normalizedEnergyProperty
        {
            get { return normalizedEnergy; }
            set
            {
                normalizedEnergy = value;
                if (normalizedEnergyChanged != null)
                { normalizedEnergyChanged(normalizedEnergy); }
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

            //Create input pins
            thermalImageInputPin = Host.LoadOrCreatePin("Thermal Image", PinCategory.Critical, new Type[] { typeof(IThermalData) });
            inPins.Add(thermalImageInputPin);
            fileNameInputPin = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) }); //File name
            inPins.Add(fileNameInputPin);
            frameNumberInputPin = Host.LoadOrCreatePin("Frame Number", PinCategory.Critical, new Type[] { typeof(ILongData) });    //Frame number      
            inPins.Add(frameNumberInputPin);
            ProcessInitInputPin = Host.LoadOrCreatePin("Process Initiator", PinCategory.Optional, new Type[] { typeof(IIntegerData) });   //Start writing as soon as this pin is one
            inPins.Add(ProcessInitInputPin);
            mouseInputPin = myHost.LoadOrCreatePin("Mouse", PinCategory.Optional, new Type[] { typeof(IMouseEventData) });
            inPins.Add(mouseInputPin);
            //mouseInputPin        = LoadOrCreatePin("Mouse", true, PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IMouseEventData) });
            //trackerFittingInputPin = LoadOrCreatePin("Confidence", true, PinCategory.Critical, new Type[] { typeof(IFilterData), typeof(IFloatData) }); //Tracking confidence(fitting) 
            //recOverlayInputPin     = LoadOrCreatePin("RecOverlay", true, PinCategory.Critical, new Type[] { typeof(IFilterData), typeof(IOverlayData), typeof(IOverlayPolygonData) });


            //Create output pins
            RawEnergyOutputPin = Host.LoadOrCreatePin("Raw Energy", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(RawEnergyOutputPin);
            WaveletEnergyOutputPin = Host.LoadOrCreatePin("GSR [For Offline]", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(WaveletEnergyOutputPin);
            numOfSegPixelsPin = Host.LoadOrCreatePin("Segmentated Pixels", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(numOfSegPixelsPin);
            ROIOutputPin = Host.LoadOrCreatePin("Perspiration ROI", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            outPins.Add(ROIOutputPin);
            overlayOutputPin = myHost.LoadOrCreatePin("Overlay", PinCategory.Optional, new Type[] { typeof(IOverlayData) });
            outPins.Add(overlayOutputPin);
            TrackFileInitOutputPin = Host.LoadOrCreatePin("TrackFile Initiator", PinCategory.Optional, new Type[] { typeof(IIntegerData) });   //Start writing as soon as this pin is one
            outPins.Add(TrackFileInitOutputPin);
            RateOutputPin = Host.LoadOrCreatePin("Rate ", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(RateOutputPin);



            //Create the measurement objects
            persPixels = new FloatData();
            //persPixels.Description = "Total perspiration pixels ";
            persPixels.Value = 0.0f;

            measurment = new FloatData();
            //measurment.Description = "Perspiration energy";
            measurment.Value = 0.0f;

            measurmentGSR = new FloatData();
            //measurmentGSR.Description = "GSR";
            measurmentGSR.Value = 0.0f;

            //ROI_Corners = new PointF[4];


            //overlayColorOption  = new OverlayColorBand();    //Create ovelay color option object
            //featureSegmentation = new FeatureSegmentation(); //Create feature segmenation object
            //cannyEdages = new CannyEdges();
            //snake = new Snake();

            // threshold = 10;//Set the default values for the thresholds
            thermalFileName = null;  //Thermal File Name
            thermalFilePath = null;
            thermalFileNameOnly = null;

            frameNumber = 0;

            TSR_FileName = null;
            GSR_FileName = null;

            timeGSR = new ArrayList();
            dataGSRRaw = new ArrayList();
            //dataGSRRaw_resampled = new ArrayList();

            isGSRFileExisit = false;

            mut = new Mutex();
            mutWriteFile = new Mutex();

            //lineNumber = 0;
            frameCounter = 1;

            //isReset = false;
            processStarted = 0;
            showPerspirationProperty = true;
            normalizedEnergyProperty = false;
            isOverlayClean = false;
            TrackFileInitiator = 0;
            //File Pointer for Thermal Perspiration 
            SR_TSR = null;

            //File Pointer for GSR
            SR_GSR = null;


            myMainInterface = new PerspirationControl(this);


            if (isSerialized)
            {
                normalizedEnergyProperty = normalizedEnergyPropertySerial;
            }


            // //size-7
            // b = new int[7, 7]
            //      {
            //        //1 2 3 4 5 6 7
            //         {0,0,1,1,1,0,0}, //1
            //         {0,1,1,1,1,1,0}, //2
            //         {1,1,1,1,1,1,1}, //3
            //         {1,1,1,1,1,1,1}, //4
            //         {1,1,1,1,1,1,1}, //5
            //         {0,1,1,1,1,1,0}, //6
            //         {0,0,1,1,1,0,0}, //7
            //      };

            //db = new int[7, 7]
            //     {
            //       //1 2 3 4 5 6 7
            //        {0,0,1,1,1,0,0}, //1
            //        {0,1,0,0,0,1,0}, //2
            //        {1,0,0,0,0,0,1}, //3
            //        {1,0,0,0,0,0,1}, //4
            //        {1,0,0,0,0,0,1}, //5
            //        {0,1,0,0,0,1,0}, //6
            //        {0,0,1,1,1,0,0}, //7
            //     };

            //size-9
            b = new int[9, 9]
                {               
                  // 1 2 3 4 5 6 7 8 9  
                    {0,0,0,1,1,1,0,0,0}, //1
                    {0,0,1,1,1,1,1,0,0}, //2
                    {0,1,1,1,1,1,1,1,0}, //3
                    {1,1,1,1,1,1,1,1,1}, //4
                    {1,1,1,1,1,1,1,1,1}, //5
                    {1,1,1,1,1,1,1,1,1}, //6
                    {0,1,1,1,1,1,1,1,0}, //7
                    {0,0,1,1,1,1,1,0,0}, //8
                    {0,0,0,1,1,1,0,0,0}, //9
                };

            db = new int[9, 9]
                {
                  // 1 2 3 4 5 6 7 8 9  
                    {0,0,0,1,1,1,0,0,0}, //1
                    {0,0,1,0,0,0,1,0,0}, //2
                    {0,1,0,0,0,0,0,1,0}, //3
                    {1,0,0,0,0,0,0,0,1}, //4
                    {1,0,0,0,0,0,0,0,1}, //5
                    {1,0,0,0,0,0,0,0,1}, //6
                    {0,1,0,0,0,0,0,1,0}, //7
                    {0,0,1,0,0,0,1,0,0}, //8
                    {0,0,0,1,1,1,0,0,0}, //9
                 };

            //Size-11

            //b = new int[11, 11]
            //    {               
            //      // 1 2 3 4 5 6 7 8 9 0 1
            //        {0,0,0,1,1,1,1,1,0,0,0}, //1
            //        {0,0,1,1,1,1,1,1,1,0,0}, //2
            //        {0,1,1,1,1,1,1,1,1,1,0}, //3
            //        {1,1,1,1,1,1,1,1,1,1,1}, //4
            //        {1,1,1,1,1,1,1,1,1,1,1}, //5
            //        {1,1,1,1,1,1,1,1,1,1,1}, //6
            //        {1,1,1,1,1,1,1,1,1,1,1}, //7
            //        {1,1,1,1,1,1,1,1,1,1,1}, //8
            //        {0,1,1,1,1,1,1,1,1,1,0}, //9
            //        {0,0,1,1,1,1,1,1,1,0,0}, //0
            //        {0,0,0,1,1,1,1,1,0,0,0}, //1
            //    };

            //db = new int[11, 11]
            //    {
            //      // 1 2 3 4 5 6 7 8 9  
            //        {0,0,0,1,1,1,1,1,0,0,0}, //1
            //        {0,0,1,0,0,0,0,0,1,0,0}, //2
            //        {0,1,0,0,0,0,0,0,0,1,0}, //3
            //        {1,0,0,0,0,0,0,0,0,0,1}, //4
            //        {1,0,0,0,0,0,0,0,0,0,1}, //5
            //        {1,0,0,0,0,0,0,0,0,0,1}, //6
            //        {1,0,0,0,0,0,0,0,0,0,1}, //7
            //        {1,0,0,0,0,0,0,0,0,0,1}, //8
            //        {0,1,0,0,0,0,0,0,0,1,0}, //9
            //        {0,0,1,0,0,0,0,0,1,0,0}, //0
            //        {0,0,0,1,1,1,1,1,0,0,0}, //1
            //     };

        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {

            if (pin == thermalImageInputPin) //Make sure the data is for the thermalImageInputPin
            {
                //thermalDataReceived = true;
                if (input != null)
                {
                    // Overlay Canny edges on the image 
                    /*   if (showCannyProperty)
                       {
                           // Canny Edge detection 
                           IImageDescription imageDescription =  (IImageDescription)input;
                           float[] thermalTemp = new float[imageDescription.Height * imageDescription.Width];
                           CannyEdgeDetectionNDisplay((IImageDescription)input, ((IImageDataFloatArray)input).Data, ref thermalTemp);


                           GenerateOverlay((IImageDescription)input, thermalTemp, 255, 0, 0, 128);
                           SendData(overlayOutputPin, overlayMaxillaryROI);// Display image 
                           isCannyShowing = true;
                       }
                       else if(!showCannyProperty && isCannyShowing)   
                       {
                           //Reset the overlay array 
                           GenerateOverlay((IImageDescription)input, ((IImageDataFloatArray)input).Data, 0,0,0,0);
                           SendData(overlayOutputPin, overlayMaxillaryROI); // Display image 
                           isCannyShowing = false;
                       }*/
                }
                if (input != null /*&& thermalDataReceived && trackerFittingInputReceived*/)
                {
                    //trackerFittingInputReceived = false;
                    //thermalDataReceived = false;

                    //Compute the temperature measurement from the input data
                    //int remainder;
                    //int reminderNum = 1;
                    //int quotient = Math.DivRem(frameNumber, reminderNum, out remainder);
                    //if (remainder == 0)

                    if (processStarted == 1)//Process start
                    {

                        // ComputeMeasurement((IImageDescription)input, ((IImageDataFloatArray)input).Data);
                        /*                      ComputeMeasurement((ThermalData)input);
                                              //if (TrackFileInitiator != 1)
                                              {
                                                //  TrackFileInitiator = 1;
                                                  //BoolData tempV = new BoolData();// (true);//"TempInit");
                                                  //tempV.Description = "TempInit";
                                                  //tempV.Data  = true;
                                                  IntegerData tempV = new IntegerData(1);

                                                  this.Host.SendData(TrackFileInitOutputPin, tempV, this);
                                              }  */
                        if (MROIHeight >= ((IThermalData)input).Height || MROIWidth >= ((IThermalData)input).Width)
                        {
                            MROIWidth = 0;
                            MROIHeight = 0;
                            MROITopLeft.X = 0;
                            MROITopLeft.Y = 0;
                            UpdateRectangle(MROITopLeft, MROIWidth, MROIHeight, 0);
                            this.myHost.SendData(RawEnergyOutputPin, null, this);
                            this.myHost.SendData(WaveletEnergyOutputPin, null, this);
                            this.myHost.SendData(numOfSegPixelsPin, null, this);
                            this.myHost.SendData(RateOutputPin, null, this);
                        }
                        else
                        {
                            ComputeMeasurement((IThermalData)input, ((IThermalData)input).Data);
                            UpdateRectangle(MROITopLeft, MROIWidth, MROIHeight, 0);

                        }
                        FloatData tempRate = new FloatData(-1000000);
                        this.Host.SendData(RateOutputPin, tempRate, this); //For integrated configration:Con Send dummy value to set Rate Disply empty and disable
                    }
                    else if (processStarted == 0) //Process stop
                    {
                        DisplyMaxillaryWO_ROI((ThermalData)input);
                        this.Host.SendData(RawEnergyOutputPin, null, this);
                        this.Host.SendData(WaveletEnergyOutputPin, null, this);
                        this.Host.SendData(numOfSegPixelsPin, null, this);
                        this.Host.SendData(RateOutputPin, null, this);

                        //if (TrackFileInitiator != 0)
                        {
                            //  TrackFileInitiator = 0;
                            //BoolData tempV = new BoolData();// (true);//"TempInit");
                            //tempV.Description = "TempInit";
                            IntegerData tempV = new IntegerData(0);
                            //tempV.Data  = true;

                            this.Host.SendData(TrackFileInitOutputPin, tempV, this);
                        }
                    }
                    else //Process pause 
                    {
                        DisplyMaxillaryWO_ROI((ThermalData)input);

                        //FloatData tempF = new FloatData();
                        //tempF.Value = -1.0f;
                        this.Host.SendData(RawEnergyOutputPin, measurment, this);
                        this.Host.SendData(WaveletEnergyOutputPin, measurmentGSR, this);
                        this.Host.SendData(numOfSegPixelsPin, persPixels, this);

                        FloatData tempRate = new FloatData(-1000000);
                        this.Host.SendData(RateOutputPin, tempRate, this); //For integrated configration: Send dummy value to set Rate Disply empty and disable
                                                                           // if (TrackFileInitiator != 2)
                        {
                            //   TrackFileInitiator = 2;
                            IntegerData tempV = new IntegerData(2);
                            this.Host.SendData(TrackFileInitOutputPin, tempV, this);
                        }
                    }

                    //else
                    //{
                    //    SendData(RawEnergyOutputPin, null);
                    //    SendData(WaveletEnergyOutputPin, null);
                    //    SendData(numOfSegPixelsPin, null);

                    //}
                }
                else
                {

                    //send out the null data on critical pins if the user hasn't set both seed pixels yet
                    if (input != null && !isOverlayClean)
                    {

                        // DisplayMaxilaryROI((IImageDescription)input);
                        //isReset = false;
                    }
                    this.Host.SendData(ROIOutputPin, null, this);
                    this.Host.SendData(RawEnergyOutputPin, null, this);
                    this.Host.SendData(WaveletEnergyOutputPin, null, this);
                    this.Host.SendData(numOfSegPixelsPin, null, this);
                    this.Host.SendData(TrackFileInitOutputPin, null, this);
                    this.Host.SendData(RateOutputPin, null, this);
                    //SendData(numOfSegPixelsPin, null);

                }

            }


            if (pin == frameNumberInputPin)
            {
                //trackerFittingInputReceived = true;

                if (input != null)
                {
                    frameNumber = (int)((IIntegerData)input).Value;
                    //filterGraph.CycleNumber;

                    //timeStamp = ((ITimeStampData)input).TimeStamp;
                    //timeStampInMilisecond = timeStamp.Millisecond;

                }
            }
            if (pin == fileNameInputPin)
            {
                //trackerFittingInputReceived = true;

                if (input != null)
                {
                    string tempFileName = ((IStringData)input).Data;
                    if (thermalFileName == null) //Loading file for first time
                    {
                        thermalFileName = tempFileName;
                        //GetFileNameForGraphDisplay();
                        ReadGSRData();
                    }

                    if (thermalFileName != tempFileName) //New subject loaded, Reset the seetings 
                    {
                        thermalFileName = tempFileName;
                        ResetSettings();
                        //DisplayMaxilaryROI(imageDescription);
                        //GetFileNameForGraphDisplay();
                        ReadGSRData();
                    }

                }

            }
            /*if (pin == recOverlayInputPin)
            {
                trackerFittingInputReceived = true;
                
                if (input != null)
                {
                    ROI = (IOverlayPolygonData)input;
                    ROI_Corners[0].X = ROI.X[0];
                    ROI_Corners[0].Y = ROI.Y[0];

                    ROI_Corners[1].X = ROI.X[1];
                    ROI_Corners[1].Y = ROI.Y[1];

                    ROI_Corners[2].X = ROI.X[2];
                    ROI_Corners[2].Y = ROI.Y[2];

                    ROI_Corners[3].X = ROI.X[3];
                    ROI_Corners[3].Y = ROI.Y[3];
                }
            }*/

            if (pin == ProcessInitInputPin)
            {
                if (input != null)
                {
                    processStarted = ((IIntegerData)input).Value;
                    //isOverlayClean = processStarted;
                    if (processStarted == 1)
                        isOverlayClean = true;
                    else
                        isOverlayClean = false;

                }
            }

            //  SignalCriticalProcessingIsFinished();
            //check to see if all of the critical pins have been received 
            //if (thermalDataReceived && trackerFittingInputReceived)
            ////if ( thermalDataReceived)
            //{
            //    //reset the pin flags
            //        trackerFittingInputReceived = false;
            //        thermalDataReceived         = false;

            //}
            //SignalCriticalProcessingIsFinished(); //Tell IRIS that procesing has finished 
            this.Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);
            this.Host.SignalCriticalProcessingIsFinished(frameNumberInputPin, this);

            //Process Mouse input pin
            if (pin == mouseInputPin)
            {
                if (input != null)
                { ProcessMouse((IMouseEventData)input); }

            }
            this.myHost.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);



            /* if (pin == inImage)
             {
                 int pixelSize = 4;
                 ImageData curImgData = (ImageData)input;
                 byte[] newImgArray = new byte[curImgData.Width * curImgData.Height * 4];
                 for (int i = 0; i < curImgData.Height; i++)
                 {
                     for (int j = 0; j < curImgData.Width; j++)
                     {
                         byte RVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize)];
                         byte GVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 1)];
                         byte BVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 2)];
                         byte AVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 3)];

                         if (RVal < 10)
                         {
                             RVal = 0;
                             GVal = 0;
                             BVal = 0;
                         }

                         newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize)] = RVal;
                         newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 1)] = GVal;
                         newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 2)] = BVal;
                         newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 3)] = AVal;
                     }
                 }
                 this.Host.SendData(outImage, new ImageData(newImgArray, curImgData.Height, curImgData.Width), this);


                 this.Host.SignalCriticalProcessingIsFinished(inImage, this);
             }*/

        }
        void ProcessMouse(IMouseEventData data)
        {

            if (data.Category == MouseEventCategory.MouseDown && data.Data.Button == MouseButtons.Left)
            {
                MROITopLeft.X = data.NormalizedXLocation;
                MROITopLeft.Y = data.NormalizedYLocation;
                MROIWidth = 0;
                MROIHeight = 0;

                MROITopLeftSelected = true;
            }
            //Display ROI

            if (data.Data != null && MROITopLeftSelected)
            {
                int tempWidth = (int)Math.Abs(MROITopLeft.X - data.NormalizedXLocation);
                int tempHeight = (int)Math.Abs(MROITopLeft.Y - data.NormalizedYLocation);
                UpdateRectangle(MROITopLeft, tempWidth, tempHeight, 0);

            }
            // Finalize the width and height 
            if (data.Category == MouseEventCategory.MouseUp && data.Data.Button == MouseButtons.Left && MROITopLeftSelected)
            {
                MROIWidth = (int)Math.Abs(MROITopLeft.X - data.NormalizedXLocation);
                MROIHeight = (int)Math.Abs(MROITopLeft.Y - data.NormalizedYLocation);
                MROITopLeftSelected = false;
                UpdateRectangle(MROITopLeft, MROIWidth, MROIHeight, 0);



                //MROIIsSet = true;
                //isLeftROINew = true;

                //Initilize Left Nostril ROI 
                //MROI = new float[MROIHeight, MROIWidth];
            }
        }
        void UpdateRectangle(PointF TopLeft, int width, int height, int id)
        {
            PolygonData overlayRectangle = new PolygonData();
            overlayRectangle.addVertice(new PointF(TopLeft.X, TopLeft.Y));
            overlayRectangle.addVertice(new PointF(TopLeft.X + width, TopLeft.Y));
            overlayRectangle.addVertice(new PointF(TopLeft.X + width, TopLeft.Y + height));
            overlayRectangle.addVertice(new PointF(TopLeft.X, TopLeft.Y + height));
            overlayRectangle.addVertice(new PointF(TopLeft.X, TopLeft.Y));

            overlayRectangle.BorderColor = Color.Black;
            overlayRectangle.Thick = 1.0f;
            //Commented by me           
            //overlayRectangle[id].x[0] = TopLeft.X;
            //overlayRectangle[id].y[0] = TopLeft.Y;


            //overlayRectangle[id].x[3] = TopLeft.X + width;
            //overlayRectangle[id].y[3] = TopLeft.Y;

            //overlayRectangle[id].x[1] = TopLeft.X;
            //overlayRectangle[id].y[1] = TopLeft.Y + height;

            //overlayRectangle[id].x[2] = TopLeft.X + width;
            //overlayRectangle[id].y[2] = TopLeft.Y + height;

            //overlayRectangle[id].x[4] = overlayRectangle[id].x[0];
            //overlayRectangle[id].y[4] = overlayRectangle[id].y[0];

            this.myHost.SendData(overlayOutputPin, overlayRectangle, this);

        }

        void GetFileNameForGraphDisplay()
        {
            if (thermalFileName == null) { return; }

            string[] subjName;
            subjName = null;
            subjName = thermalFileName.Split(new Char[] { '\\' });

            //Create GSR and TSR file name
            thermalFilePath = "";
            for (int sID = 0; sID < subjName.Length - 1; sID++)
            {
                thermalFilePath = thermalFilePath + subjName[sID] + "\\";
            }

            TSR_FileName = thermalFilePath + "TSR_Output.txt";
            GSR_FileName = thermalFilePath + "ADI_Output.txt";

            thermalFileNameOnly = subjName[subjName.Length - 1];
        }

        void writeDataIntoFile(float time, float dataTSR, float dataGSR)
        {
            if (mutWriteFile.WaitOne())
            {
                string oneLine = "";

                //Write TSR into file 
                oneLine = time.ToString() + '\t' + dataTSR.ToString();

                try
                {
                    FileStream fileTSR = new FileStream(TSR_FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    SR_TSR = new StreamWriter(fileTSR);
                    // SR_GSR = File.CreateText(TSR_FileName);
                    SR_TSR.WriteLine(oneLine);
                    SR_TSR.Close();
                    fileTSR.Close();
                }
                catch { }

                //Write GSR into file 
                oneLine = time.ToString() + '\t' + dataGSR.ToString();

                try
                {
                    FileStream fileGSR = new FileStream(GSR_FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    SR_GSR = new StreamWriter(fileGSR);
                    SR_GSR.WriteLine(oneLine);
                    SR_GSR.Close();
                    fileGSR.Close();
                }
                catch { }


                mutWriteFile.ReleaseMutex();
            }
        }

        void ReadGSRData()
        {
            StreamReader SR;
            string GSRFilename = thermalFileName + "-GSR.txt";
            if (!File.Exists(GSRFilename))
            {
                // MessageBox.Show("GSR File Does not exist " + GSRFilename);
                isGSRFileExisit = false;
                return;
            }
            isGSRFileExisit = true;

            SR = File.OpenText(GSRFilename);
            char[] delimiterChars = { ' ', '\t' };

            timeGSR.Clear();
            dataGSRRaw.Clear();
            //dataGSRRaw_resampled.Clear();
            //dataGSRNorm.Clear();

            string S;
            float tempV = -10000;
            // read each line (format: {timestamp pulse})
            while ((S = SR.ReadLine()) != null)
            {
                string[] words = S.Split(delimiterChars);

                if (words[0] != "NaN") { timeGSR.Add(double.Parse(words[0])); }
                else { timeGSR.Add(tempV); }

                if (words[1] != "NaN") { dataGSRRaw.Add(double.Parse(words[1])); }
                else { dataGSRRaw.Add(tempV); }
                //dataGSRNorm.Add(double.Parse(words[2]));

            }
            SR.Close();

            //Reseample the data
            ResampleGSR_Wrt_TSR();

            // Store resampled data
            //Write GSR into file 
            WriteResampledGSR();
        }

        private void WriteResampledGSR()
        {

            string GSRresampled_FileName = thermalFileName + "-GSR[Resampled].txt";

            try
            {
                FileStream fileGSR = new FileStream(GSRresampled_FileName, FileMode.OpenOrCreate, FileAccess.Write);
                SR_GSR = new StreamWriter(fileGSR);
            }
            catch { }

            string oneLine;

            for (int i = 0; i < dataGSRRaw_resampled.GetLength(0); i++)
            {
                oneLine = dataGSRRaw_resampled[i, 0].ToString() + '\t' + dataGSRRaw_resampled[i, 1].ToString();
                SR_GSR.WriteLine(oneLine);
            }

            try
            {
                SR_GSR.Close();

            }
            catch { }
        }

        private void ResampleGSR_Wrt_TSR()
        {
            //Read inf file
            string subINF = thermalFileName + ".inf";
            if (!System.IO.File.Exists(subINF))
            {
                //MessageBox.Show("File Does not exist " + subINF);
                return;
            }
            float[] absTimeStamp; //1 column absolute time stamps (For example, 0, 0.01..)
            int numberOfFramesIn_INF_file = 0;
            absTimeStamp = GetTimeStampFrom_INF_file(subINF, ref numberOfFramesIn_INF_file);

            // Sample the GSR based on the thermal data time stamps
            dataGSRRaw_resampled = Sampled_ADI(absTimeStamp);
        }

        //Get absolute time stamps from INF file 
        private float[] GetTimeStampFrom_INF_file(string filename, ref int numberOfFrames)
        {
            //Open file 
            StreamReader SR;
            string S;
            SR = File.OpenText(filename);

            // 1) Get first line: number of frames 
            S = SR.ReadLine();
            numberOfFrames = (int)System.Convert.ToDecimal(S);

            float[] absTime;

            if (numberOfFrames == 0)
            {
                //MessageBox.Show("Number of Frames inside INF files is zero");
                //return absTime;
            }

            // 2) Get second line: image width
            S = SR.ReadLine();
            int width = (int)System.Convert.ToDecimal(S);

            // 3) Get third line: image height
            S = SR.ReadLine();
            int height = (int)System.Convert.ToDecimal(S);

            // 4) Skip fourth line: empty line
            S = SR.ReadLine();

            // 5) Get data: 5th line to end of the file
            absTime = new float[numberOfFrames];
            int row = 0;
            //First frame is reference frame
            S = SR.ReadLine();
            string[] temp;
            temp = S.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            float nThr_ref = (float)System.Convert.ToDouble(temp[3]);
            float nTmin_ref = (float)System.Convert.ToDouble(temp[4]);
            float dTsec_ref = (float)System.Convert.ToDouble(temp[5]);
            row++; //row = 1;
            //Second frame 
            S = SR.ReadLine();
            temp = S.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            float nThr = (float)System.Convert.ToDouble(temp[3]) - nThr_ref;
            float nTmin = (float)System.Convert.ToDouble(temp[4]) - nTmin_ref;
            float dTsec = (float)System.Convert.ToDouble(temp[5]) - dTsec_ref;
            if (nThr > 0) { nTmin = nTmin + 60 * nThr; }
            if (nTmin > 0) { dTsec = (float)(dTsec + 60.0 * nTmin); }
            row++; //row = 2

            absTime[0] = dTsec;  // First abs time stamp is special case 
            absTime[1] = dTsec + absTime[0];  // Second abs time stamp 


            while (S != null)
            {
                S = SR.ReadLine();
                if (S == null) { break; }

                temp = S.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 1)
                {
                    nThr = (float)System.Convert.ToDouble(temp[3]) - nThr_ref;
                    nTmin = (float)System.Convert.ToDouble(temp[4]) - nTmin_ref;
                    dTsec = (float)System.Convert.ToDouble(temp[5]) - dTsec_ref;
                    if (nThr > 0) { nTmin = nTmin + 60 * nThr; }
                    if (nTmin > 0) { dTsec = (float)(dTsec + 60.0 * nTmin); }
                    if (row < numberOfFrames) { absTime[row] = dTsec + absTime[0]; }
                    row++;
                }

            }
            SR.Close();


            return absTime;


        }

        private float[,] Sampled_ADI(float[] absTimeStamp)
        {
            int tSamples = absTimeStamp.Length;
            float[,] dataGSRRaw_resampled = new float[tSamples, 2]; //Store time and value 
            int GSRsize = dataGSRRaw.Count;
            int startID = 0;

            for (int sID = 0; sID < tSamples; sID++)
            {
                for (int gsrID = startID; gsrID < GSRsize; gsrID++)
                {
                    float diff = (float)(Math.Abs(absTimeStamp[sID] - (float)System.Convert.ToDouble(timeGSR[gsrID])));
                    if (diff <= 0.000010) //0.00001
                    {
                        dataGSRRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeGSR[gsrID]);
                        dataGSRRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataGSRRaw[gsrID]);

                        startID = gsrID;
                        break;
                    }

                    else if ((float)System.Convert.ToDouble(timeGSR[gsrID]) > absTimeStamp[sID])
                    {
                        dataGSRRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeGSR[gsrID - 1]);
                        dataGSRRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataGSRRaw[gsrID - 1]);

                        startID = gsrID;
                        break;
                    }
                    else
                    {
                        if (gsrID > 1)
                        {
                            dataGSRRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeGSR[gsrID - 1]);
                            dataGSRRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataGSRRaw[gsrID - 1]);
                        }
                    }

                }//End of gsrID 
            }//End of sID 

            return dataGSRRaw_resampled;
        }

        public void ResetSettings()
        {
            frameCounter = 1;
            //isReset = true;
        }

        float ComputePersEnergy(float[,] PerspirationROI, float[,] MaxillaryROI, ref int totalPixels)
        {
            float sum = 0;
            totalPixels = 0;
            int heightROI = PerspirationROI.GetLength(0);
            int widthROI = PerspirationROI.GetLength(1);

            for (int y = 0; y < heightROI; y++)
                for (int x = 0; x < widthROI; x++)
                {
                    if (PerspirationROI[y, x] > 0 && PerspirationROI[y, x] < 100)
                    {
                        sum = sum + PerspirationROI[y, x] * PerspirationROI[y, x];
                        totalPixels = totalPixels + 1;
                    }
                    else { PerspirationROI[y, x] = 0; }

                }// end of for x

            if (normalizedEnergyProperty)
            { sum = sum / totalPixels; }
            return sum;
        }
        void ComputeMeasurement(IThermalData imageDescription, float[] thermalData)
        {
            if (mut.WaitOne())
            {
                //int imgHeight = imageDescription.Height;
                //int imgWidth = imageDescription.Width;

                //int mROIHeight = (int)Math.Abs(ROI.Y[3] - ROI.Y[0] + 1);
                //int mROIWidth = (int)Math.Abs(ROI.X[1] - ROI.X[0] + 1);

                //if (mROIHeight > imgHeight) { mROIHeight = imgHeight; }
                //if (mROIWidth > imgWidth) { mROIWidth = imgWidth; }

                int mTROIHeight = imageDescription.Height;
                int mTROIWidth = imageDescription.Width;



                // Copy thermal data of the ROI
                MaxillaryROI = new float[MROIHeight, MROIWidth]; //Matlab --> 
                //GetROIData(ref MaxillaryROI, ROI_Corners[0], mROIWidth, mROIHeight, imageDescription, thermalData);
                GetROIData(ref MaxillaryROI, MROIWidth, MROIHeight, imageDescription, thermalData);

                //Localize perspiration 
                PerspirationROI = new float[MROIHeight, MROIWidth];//...
                PerspirationROI = Bottomhat(MaxillaryROI, 1);//Matlab --> maskBH: Unlike matlab code, A_ROI_mask is overwritten 
                //PerspirationROI = medianfilter(PerspirationROI, 3);

                //Compute Energy of the MaxillaryROI --> perspiration 
                float sumM = 0;//...
                int totalPixels = 0;//...
                sumM = ComputePersEnergy(PerspirationROI, MaxillaryROI, ref totalPixels); //thermaldata

                measurment.Value = sumM;//...
                this.Host.SendData(RawEnergyOutputPin, measurment, this);//...

                persPixels.Value = totalPixels;//...
                this.Host.SendData(numOfSegPixelsPin, persPixels, this);//...

                //Write value into file
                float tempGSR = 0;//...

                if (dataGSRRaw_resampled != null && isGSRFileExisit && frameNumber >= 0 && frameNumber <= dataGSRRaw_resampled.GetLength(0))
                {
                    tempGSR = (float)Convert.ToDouble(dataGSRRaw_resampled[frameNumber, 1]);//...
                    measurmentGSR.Value = tempGSR;//...
                    Console.WriteLine("measurementGSR = " + measurmentGSR.Value.ToString());
                    this.Host.SendData(WaveletEnergyOutputPin, measurmentGSR, this);//...
                }
                else
                {
                    this.Host.SendData(WaveletEnergyOutputPin, null, this);//...
                }


                // Display Data
                DisplayMaxilaryROI(imageDescription);
                //SendData(overlayOutputPin, overlayMaxillaryROI); 

                frameCounter++;//...

                mut.ReleaseMutex();//...
            }

        }


        void ComputeMeanMinMax(int[] data_1D, ref int dMin, ref int dMax, ref int dMean)
        {
            dMean = 0;
            dMin = 10000;
            dMax = -10000;
            for (int j = 0; j < data_1D.Length; j++)
            {
                dMean = dMean + data_1D[j];
                if (dMin > data_1D[j]) { dMin = data_1D[j]; }
                if (dMax < data_1D[j]) { dMax = data_1D[j]; }
            }
            dMean = (int)(Math.Round((int)dMean / (double)data_1D.Length));
        }

        float computeStd(int[] data_1D)
        {
            float dStd = 0;
            float dMean = 0;

            //Compute mean
            for (int j = 0; j < data_1D.Length; j++)
            { dMean = dMean + data_1D[j]; }

            dMean = (float)(Math.Round((int)dMean / (double)data_1D.Length));

            //Compute std 
            float sum = 0;
            for (int j = 0; j < data_1D.Length; j++)
            { sum = sum + ((data_1D[j] - dMean) * (data_1D[j] - dMean)); }

            dStd = (float)(Math.Sqrt(sum / (data_1D.Length - 1)));

            return dStd;
        }

        void checkNoseROIBoundary(ref PointF NostrilTopLeft, int NostrilWidth, int NostrilHeight, int imgWidth, int imgHeight)
        {
            //Check X limit 
            if (NostrilTopLeft.X < 1)
            { NostrilTopLeft.X = 2; }
            else if (NostrilTopLeft.X + NostrilWidth > imgWidth)
            { NostrilTopLeft.X = imgWidth - NostrilWidth - 2; }

            //Check Y limit 
            if (NostrilTopLeft.Y < 1)
            { NostrilTopLeft.Y = 2; }
            else if (NostrilTopLeft.Y + NostrilHeight > imgHeight)
            { NostrilTopLeft.Y = imgHeight - NostrilHeight - 2; }

        }

        void GetROIData(ref float[,] ROI, int Width, int Height, IThermalData imageDescription, float[] data)
        {
            int xOffset = (int)MROITopLeft.X;
            int yOffset = (int)MROITopLeft.Y;

            for (int j = 0; j < Height; j++) //rows
                for (int i = 0; i < Width; i++) //cols
                {
                    // int thermalAddress = ((i + xOffset) * imageDescription.ColorBands) + ((j + yOffset) * imageDescription.Stride);

                    int thermalAddress = ((i + xOffset) * 1) + ((j + yOffset) * imageDescription.Width);


                    if (thermalAddress < data.Length)
                    {
                        if (data[thermalAddress] == 0)
                        { ROI[j, i] = 100; } //= 100;} //Make the boundary hot spot to avoid being considered as perspiration (cold spot)
                        else
                        { ROI[j, i] = data[thermalAddress]; }
                    }
                }
        }



        void GetEdgePoints(float[,] NostrilROI, ref List<int> NoseEdgePointsX, ref List<int> NoseEdgePointsY)
        {
            int heightROI = NostrilROI.GetLength(0);
            int widthROI = NostrilROI.GetLength(1);

            int count = 1;
            for (int y = 0; y < heightROI; y++)
                for (int x = 0; x < widthROI; x++)
                {
                    if (NostrilROI[y, x] > 0)
                    {
                        NoseEdgePointsX.Add(x); // store x
                        NoseEdgePointsY.Add(y); // store y
                        count = count + 1;
                    } //end of if
                }// end of for x
        }

        void DisplyMaxillaryWO_ROI(ThermalData thermalData)
        {
            float[] data = new float[thermalData.Width * thermalData.Height];
            outputROI = new ThermalData(data, thermalData.Height, thermalData.Width);// thermalData.Width);//, 4 /*imageDescription.ColorBands*/);
            //outputROI.SetData(data);

            for (int j = 0; j < thermalData.Height; j++) //rows
                for (int i = 0; i < thermalData.Width; i++) //cols
                {
                    int thermalAddress = (i * 1 /*imageDescription.ColorBands*/) + (j * thermalData.Width /*imageDescription.Stride*/);
                    if (data != null && thermalAddress <= outputROI.Data.Length && thermalAddress <= thermalData.Data.Length)
                    {
                        //Initialize with zeors 
                        outputROI.Data[thermalAddress] = thermalData.Data[thermalAddress];

                    }

                }


            this.Host.SendData(ROIOutputPin, outputROI, this);
        }

        void DisplayMaxilaryROI(IThermalData thermalData)
        {




            float[] data = new float[thermalData.Width * thermalData.Height];
            outputROI = new ThermalData(data, thermalData.Height, thermalData.Width);// thermalData.Width);//, 4 /*imageDescription.ColorBands*/);
            //outputROI.SetData(data);

            for (int j = 0; j < thermalData.Height; j++) //rows
                for (int i = 0; i < thermalData.Width; i++) //cols
                {
                    int thermalAddress = (i * 1 /*imageDescription.ColorBands*/) + (j * thermalData.Width /*imageDescription.Stride*/);
                    if (data != null && thermalAddress <= outputROI.Data.Length && thermalAddress <= thermalData.Data.Length)
                    {
                        //Initialize with zeors 
                        outputROI.Data[thermalAddress] = thermalData.Data[thermalAddress];

                    }

                }

            for (int j = 0; j < MROIHeight; j++) //rows
                for (int i = 0; i < MROIWidth; i++) //cols
                {
                    int thermalAddress = (i + (int)MROITopLeft.X * 1) + ((j + (int)MROITopLeft.Y) * thermalData.Width);
                    float therhold = 0.05f;
                    if (PerspirationROI[j, i] > therhold)
                    {
                        float temp = ((float)((PerspirationROI[j, i] / 1) * 6) + 26.75f);
                        if (temp > 36)
                        {
                            temp = 36;
                        }
                        outputROI.Data[thermalAddress] = temp;
                    }

                }


            this.Host.SendData(ROIOutputPin, outputROI, this);
            //if (!overlayOutputPin.Connected)
            //    return;
            ////overlayMaxillaryROI = null;
            //if (overlayMaxillaryROI == null || overlayMaxillaryROI.Width != imageDescription.Width || overlayMaxillaryROI.Height != imageDescription.Height)
            //{
            //    overlayMaxillaryROI = new OverlayImageData(this, imageDescription.Width, imageDescription.Height);
            //}
            //int overlayAddress;
            //int xOffset, yOffset;

            if (showPerspirationProperty && processStarted == 1 && PerspirationROI != null && MaxillaryROI != null)
            {

                //                float[] data = new float[mROIWidth * mROIHeight];
                //                outputROI = new ThermalData(data,mROIHeight, mROIWidth);//, imageDescription.Width, imageDescription.ColorBands);
                //                //outputROI.SetData(data);
                //
                //                for (int j = 0; j < mROIHeight; j++) //rows
                //                    for (int i = 0; i < mROIWidth; i++) //cols
                //                    {
                //                        int thermalAddress = (i * 1/*imageDescription.ColorBands*/) + (j * mROIWidth /*imageDescription.Stride*/);
                //                        if (data != null && thermalAddress <= outputROI.Data.Length)
                //                        {
                //                            //Initialize with zeors 
                //                            data[thermalAddress] = 0;
                //
                //                            // Base image frame 
                //                           if (j <= MaxillaryROI.GetLength(0) &&
                //                                i <= MaxillaryROI.GetLength(1) &&
                //                                MaxillaryROI[j, i] > 0 && MaxillaryROI[j, i] < 100)
                //                            {
                //                                data[thermalAddress] = MaxillaryROI[j, i];
                //                            }  
                //                            float therhold = 0.05f;
                //                            if (normalizedEnergyProperty) { therhold = 0.05f; }
                //                            //Perspiration
                //                            if (j <= PerspirationROI.GetLength(0) &&
                //                                i <= PerspirationROI.GetLength(1) &&
                //                                PerspirationROI[j, i] > therhold)
                //                            {
                //                                float temp = ((float)((PerspirationROI[j, i] / 1) * 6) + 26.75f);
                //                                //(float)(0.05 * 6 + 28);
                //                                //
                //                                if (temp > 36)
                //                                {
                //                                    temp = 36;
                //                                }
                //
                //                                data[thermalAddress] = temp;
                //                            }
                //                        }
                //
                //                   }
                //
                //
                //               this.Host.SendData(ROIOutputPin, outputROI,this);
                //                //data = null;
                //                //outputROI = null;
            }
            else
            {
                this.Host.SendData(ROIOutputPin, null, this);
            }


        }

        /*unsafe*/
        void hausdorff(ref List<int> NoseEdgePointsX, ref List<int> NoseEdgePointsY, int NostrilHeight, int NostrilWidth, ref List<int> NoseEdgePointsTemplateX, ref List<int> NoseEdgePointsTemplateY, int minTemplateX, int maxTemplateX, int minTemplateY, int maxTemplateY, ref PointF newPos, float minDist)
        {
            minDist = 100000;
            newPos.X = 0;
            newPos.Y = 0;
            float minD;
            // find position where distance is minimum
            for (int r = -minTemplateY; r < (NostrilHeight - maxTemplateY); r = r + 2)
                for (int c = -minTemplateX; c < (NostrilWidth - maxTemplateX); c = c + 2)
                {
                    minD = compute_minimumDist(ref NoseEdgePointsX, ref NoseEdgePointsY, ref NoseEdgePointsTemplateX, ref NoseEdgePointsTemplateY, r, c);

                    if (minD < minDist)
                    {
                        minDist = minD;
                        newPos.X = c;
                        newPos.Y = r;
                    }
                }//for c
        }

        /*unsafe*/
        float compute_minimumDist(ref List<int> NoseEdgePointsX, ref List<int> NoseEdgePointsY, ref List<int> NoseEdgePointsTemplateX, ref List<int> NoseEdgePointsTemplateY, int r, int c)
        {
            int lenTemplate = NoseEdgePointsTemplateX.Count;
            int lenImg = NoseEdgePointsX.Count;

            float sumD = 0;
            float avgD = 0;
            float d;
            int x, y;
            float minD;
            for (int i = 0; i < lenTemplate; i++)
            {
                minD = 100000;
                for (int j = 0; j < lenImg; j++)
                {
                    if (i < NoseEdgePointsTemplateX.Count && j < NoseEdgePointsX.Count && i < NoseEdgePointsTemplateY.Count && j < NoseEdgePointsY.Count)
                    {
                        x = (NoseEdgePointsTemplateX[i] + c) - NoseEdgePointsX[j];
                        y = (NoseEdgePointsTemplateY[i] + r) - NoseEdgePointsY[j];
                    }
                    else
                    {
                        x = 1000;
                        y = 1000;
                    }
                    d = (float)(Math.Sqrt(x * x + y * y));

                    if (d < minD)
                    { minD = d; }
                }//% for j
                sumD = sumD + minD;

            }// for i


            avgD = sumD / lenTemplate;

            return avgD;

        }

        void computeLipsROI(int[] x, int[] y, int xMin, int xMax, int yMin, int yMax, int imgHeight, int imgWidth, ref PointF LipROITopLeft, ref int LipROIHeight, ref int LipROIWidth)
        {
            int offset = 15;

            // X Top left 
            int xLipsTopLeft = xMin - offset;
            int xLipsTopRight = xMax + offset;

            if (xLipsTopLeft <= 1) { xLipsTopLeft = 2; }
            if (xLipsTopRight >= imgWidth) { xLipsTopLeft = imgWidth - 2; }

            LipROITopLeft.X = xLipsTopLeft; //Set ROI X Top left cordinate 
            LipROIWidth = Math.Abs(xLipsTopLeft - xLipsTopRight); //ROI width 

            // Y Top left

            int yUpper = yMin - offset;
            int yBottom = yMax + offset;

            if (yUpper <= 1) { yUpper = 2; }
            if (yBottom >= imgHeight) { yBottom = imgHeight - 2; }

            LipROITopLeft.Y = yUpper; //Set ROI Y Top left cordinate 
            LipROIHeight = Math.Abs(yUpper - yBottom); //ROI Height 
        }

        /*unsafe*/
        float[,] SobelHorizontalEdge(float[,] ROI)
        {
            // Horizontal Strustrue element 
            float[,] sH = new float[3, 3]
                {
                   // C1 C2 C3
                    { 1, 2, 1}, //R1
                    { 0, 0, 0}, //R2
                    {-1,-2,-1}  //R3
                };

            int heightROI = ROI.GetLength(0);
            int widthROI = ROI.GetLength(1);

            float[,] Edges = new float[heightROI, widthROI];
            float temp;
            for (int i = 1; i < heightROI - 1; i++)
                for (int j = 1; j < widthROI - 1; j++)
                {
                    temp = ROI[i - 1, j - 1] * sH[0, 0] + ROI[i - 1, j] * sH[0, 1] + ROI[i - 1, j + 1] * sH[0, 2] +  // R1: C1, C2, C3
                           ROI[i + 1, j - 1] * sH[2, 0] + ROI[i + 1, j] * sH[2, 1] + ROI[i + 1, j + 1] * sH[2, 2];  // R3: C1, C2, C3

                    Edges[i, j] = temp * temp; // Square of the horizontal edges
                }


            return Edges;

        }

        float[,] medianfilter(float[,] ROI, int filterSize)
        {
            int heightROI = ROI.GetLength(0);
            int widthROI = ROI.GetLength(1);

            //float[,] img = new float[heightROI, widthROI];

            filterSize = 3;
            int Offset = Math.Abs((filterSize - 1) / 2);
            int midPoint = (int)((filterSize * filterSize) / 2 + 0.5);
            float[] signal1D = new float[filterSize * filterSize];
            int count;
            for (int i = Offset; i < heightROI - Offset; i++)
                for (int j = Offset; j < widthROI - Offset; j++)
                {
                    //Store 3x3 data into a 1D signal
                    count = 0;
                    for (int m = i - Offset; m <= i + Offset; m++)
                        for (int n = j - Offset; n <= j + Offset; n++)
                        {
                            signal1D[count] = ROI[m, n];
                            count++;
                        }
                    //Find median value

                    Array.Sort(signal1D);

                    ROI[i, j] = signal1D[midPoint];
                }

            return ROI;

        }

        /*unsafe*/
        float[,] Bottomhat(float[,] ROI, int ImageSizeLarge)
        {
            int heightROI = ROI.GetLength(0);
            int widthROI = ROI.GetLength(1);

            float[,] CBO;
            CBO = new float[heightROI, widthROI];

            //Matlab --> CBO = imdilate(imerode(Img, db), b);
            CBO = ImgErod(ROI, db);
            CBO = ImgDilateNfindMax(ROI, CBO, b); //Dilate and find maximum value of dialate and original

            return CBO;
        }

        /*unsafe*/
        float[,] ImgDilate(float[,] ROI)
        {
            int heightROI = ROI.GetLength(0);
            int widthROI = ROI.GetLength(1);

            float[,] img = new float[heightROI, widthROI];

            // Intialize Structuring Elements
            int[,] SE = new int[2, 11];
            for (int i = 0; i < 11; i++)
            {
                SE[0, i] = 1; //First Row
                SE[1, i] = 1; //Second Row
            }

            float temp;
            //int count;
            float max;
            for (int i = 1; i < heightROI - 1; i++)// round(2/2) = 1
                for (int j = 5; j < widthROI - 5; j++) // round(11/2) = 6
                {
                    temp = 0;
                    //count = 0;
                    max = -10000;
                    for (int m = 0; m <= 1; m++)
                        for (int n = -5; n <= 5; n++)
                        {
                            temp = ROI[i + m, j + n] * SE[m, n + 5];
                            if (max < temp)
                            { max = temp; }
                            //if(temp > 0){count++;break;}
                        }
                    img[i, j] = max;
                    //if (count > 0) { img[i, j] = temp; }
                    //else{img[i,j] = 0;}
                }

            return img;
        }

        /*unsafe*/
        float[,] ImgDilateNfindMax(float[,] ROI, float[,] CBO, int[,] SE)
        {
            int heightROI = CBO.GetLength(0);
            int widthROI = CBO.GetLength(1);

            float[,] Ob = new float[heightROI, widthROI];

            int hSE_Offset = (int)(Math.Floor(SE.GetLength(0) / 2.0)); //Height offset 
            int wSE_Offset = (int)(Math.Floor(SE.GetLength(1) / 2.0));//Width offset  


            float temp;
            float max;
            for (int i = hSE_Offset; i < heightROI - hSE_Offset; i++)// 
                for (int j = wSE_Offset; j < widthROI - wSE_Offset; j++) // 
                {
                    if (ROI[i, j] > 0 && ROI[i, j] < 100)
                    {
                        temp = 0;
                        max = -10000;
                        for (int m = -hSE_Offset; m <= hSE_Offset; m++)
                            for (int n = -wSE_Offset; n <= wSE_Offset; n++)
                            {
                                temp = CBO[i + m, j + n] * SE[m + hSE_Offset, n + wSE_Offset];
                                if (max < temp)
                                { max = temp; }
                            }
                        Ob[i, j] = max;

                        //Matlab --> Ob(i, j) = max(Img(i, j), CBO(i, j));
                        if (ROI[i, j] > Ob[i, j]) { Ob[i, j] = ROI[i, j]; }
                        //Matlan --> Img = Ob - Img;
                        Ob[i, j] = Ob[i, j] - ROI[i, j];
                    }
                    else { Ob[i, j] = 0; }
                }
            return Ob;
        }

        /*unsafe*/
        float[,] ImgErod(float[,] ROI, int[,] SE)
        {
            int heightROI = ROI.GetLength(0);
            int widthROI = ROI.GetLength(1);

            float[,] img = new float[heightROI, widthROI];

            int hSE_Offset = (int)(Math.Floor(SE.GetLength(0) / 2.0)); //Height offset 
            int wSE_Offset = (int)(Math.Floor(SE.GetLength(1) / 2.0));//Width offset  


            float temp;
            float min;
            for (int i = hSE_Offset; i < heightROI - hSE_Offset; i++)// 
                for (int j = wSE_Offset; j < widthROI - wSE_Offset; j++) // 
                {
                    if (ROI[i, j] > 0 && ROI[i, j] < 100)
                    {
                        temp = 0;
                        min = 10000;
                        for (int m = -hSE_Offset; m <= hSE_Offset; m++)
                            for (int n = -wSE_Offset; n <= wSE_Offset; n++)
                            {
                                temp = ROI[i + m, j + n] * SE[m + hSE_Offset, n + wSE_Offset];
                                if (temp > 0 && min > temp)
                                { min = temp; }
                            }
                        img[i, j] = min;
                    }
                    else { img[i, j] = 0; }
                }

            return img;
        }

        void computeLinearParameters(PointF P1, PointF P2, ref LinearPolynomial LP)
        {
            float difX = (float)(P2.X - P1.X);
            float difY = (float)(P2.Y - P1.Y);
            if (difX == 0) { difX = 15; }

            LP.m = difY / difX;
            LP.c = P1.Y - LP.m * P1.X;
        }

        void computeLinearParameters(int P1_X, int P1_Y, int P2_X, int P2_Y, ref LinearPolynomial LP)
        {
            float difX = (float)(P2_X - P1_X);
            float difY = (float)(P2_Y - P1_Y);
            if (difX == 0) { difX = 15; }

            LP.m = difY / difX;
            LP.c = P1_Y - LP.m * P1_X;
        }

        int FindWithinLimits(int X_1, int X_2, int xTemp)
        {
            int withinLimits = 0;
            if (X_1 > X_2)
            {
                if (xTemp >= X_2 && xTemp <= X_1)
                { withinLimits = 1; }
            }
            else
            {
                if (xTemp >= X_1 && xTemp <= X_2)
                { withinLimits = 1; }
            }

            return withinLimits;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public Perspiration()
        {
            //
            // TODO: Add constructor logic here
            //
            normalizedEnergyProperty = true;


        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Perspiration(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            normalizedEnergyPropertySerial = (bool)info.GetBoolean("Normalized");
            isSerialized = true;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            info.AddValue("Normalized", normalizedEnergyProperty);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////






        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Commneted code
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Commented code
        /*void CannyEdgeDetectionNDisplay(IImageDescription imageDescription, float[] thermalData, ref float[] thermalTemp)
     {
            
         // 1) Covert 1D into 2D 
         float[,] cannyTemp = new float[imageDescription.Height, imageDescription.Width];
            
         int thermalAddress; 

         for (int y = 0; y < imageDescription.Height; y++)
             for (int x = 0; x < imageDescription.Width; x++)
             {
                 thermalAddress = x * imageDescription.ColorBands + y * imageDescription.Stride;
                    
                 if (thermalData[thermalAddress] > 0)
                 {cannyTemp[y, x] = thermalData[thermalAddress];}
             }

       
         // 2) Canny edge 
         cannyTemp = cannyEdages.ComputeCannyEdges(cannyTemp); 
            
         // 3) Convert 2D into 1D 
         for (int y = 0; y < imageDescription.Height; y++)
             for (int x = 0; x < imageDescription.Width; x++)
             {
                 thermalAddress = x * imageDescription.ColorBands + y * imageDescription.Stride;

                 if (cannyTemp[y, x] > 0)
                 { thermalTemp[thermalAddress] = cannyTemp[y, x]; }
             }
              
     }*/

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

    }
}
