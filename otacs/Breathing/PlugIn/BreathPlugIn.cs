/// TODO:
/// 1. Need to upgrade FloatData with DataBoundColoredFloatData
/// 2. Figure out what is this: FilterGraph.OutputPinSendData(overlayMROIOutputPin, selectionOverlay);
/// 5. Return the If..Else at Ln1588 condition when ThermalVisualizer plugin is ready

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

using BreathImageProcessing;
using System.Diagnostics;
using System.IO;
using System.Collections;
//using ThreeDimBreath;

using PluginInterface;

namespace PlugIn
{

    public delegate void PluginStatusDelegate(string status);

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Enumerations
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    enum SelectionState { waitingForFirstPoint, waitingForSecondPoint };


    [Serializable]
    //////////////////////////////////////////////////////////////////////////////////////////////
    public class Plugin : IPlugin, ISerializable
    //////////////////////////////////////////////////////////////////////////////////////////////
    {
        #region Members

        #region Pins

        //input pins
        IPin thermalTROIInputPin;
        IPin frameNumberPin;
        //IPin timeStampPin;
        IPin mousePin;
        IPin filenamePin;
        IPin objOutputPathPin;
        //IPin totalFramesInputPin;
        IPin ProcessInitInputPin;
        IPin frameTimeStamp;

        //output pins
        IPin breathTemperaturePin;
        IPin breathRatePin;
        IPin breathApneaPin;
        IPin overlayMROIOutputPin;

        // output pins to 3D Visualization
        IPin thermalMROIOutputpin;
        IPin MROIHeight;
        IPin MROIWidth;
        //IPin ROIOutputPin;



        // ADI BRT offline
        IPin ADI_BRTOutputPin;
        IPin ADI_BRTrateOutputPin;


        #endregion


        #region  Thermal Interface

        PolygonData selectionOverlay; ///<summary> Current state of the selection                                       </summary>
        SelectionState selectionState;       ///<summary> The first selected point                                             </summary>
        PointF selectionPoint1;      ///<summary> The second selected state                                            </summary>
        PointF selectionPoint2;      ///<summary> Provides multi-threaed protection for the plug-in's critical data    </summary>
        Mutex mutex;
        Mutex mutex_FileRead;

        #endregion


        #region Private

        //IRIS
        private float breathTemperature;
        private float breathRate;
        private float breathApnea;
        private string filename;
        private string strOutputPath;

        //private float frameNumber;
        private double dMaxPowerScale;

        //private float frameNumber;
        private DateTime timeStamp;
        private TimeSpan offsetFromStart;
        private TimeSpan prevOffsetFromStart;

        //breath computation
        private readonly int firstStage;
        private readonly int lastStage;
        private readonly int windowSizeMeanEdge;
        private readonly int powerScale;

        private readonly int samplingRate;
        // private readonly int millisecondInSecond;


        //for IRIS
        private BreathIPL breathData = new BreathIPL();
        private BreathComputation breathComputation = new BreathComputation();
        private SaveRecord saveRecord = null;
        //GUI
        private PluginGUI breathGUI = null;
        private FloatData breathTemperatureOutputData;
        private FloatData breathRateOutputData;
        private FloatData breathApneaOutputData;

        // 3D
        private ThermalData mMROIBoundFloatArray;
        private IntegerData mMROIBoundHeight;
        private IntegerData mMROIBoundWidth;

        FloatData measurmentBRT;
        FloatData measurmentBRTrate;


        private double[] imageXGradientProjectionMean;
        private double[] imageYGradientProjectionMean;

        //MROI
        private bool bBaselineMROI;

        //frame #
        private long currentFrameNumber;
        private long processedFrameNumber;
        private long firstFrameNumber;
        private long lSyncFrame;
        private int iIteration;

        //timestamp
        //private long firstFrameTimeStampInMilisecond;
        //private long timeStampInMilisecond;
        private double firstFrameTimeStampInSecond;
        private double tempTime;


        //private float totalNumberOfFrames;
        //private float resamplingInterval;
        // private bool bResample;

        //
        //private float breathGroundTruthTemperature;
        //private float breathGroundTruthRate;

        //output filename
        private string filenameOutput;
        private readonly string filenameOutputExtension;

        //2D Graph scale;
        float upperLimit;
        float lowerLimit;
        //float count;

        bool isNewFrame;
        bool isLimitNotset;
        bool isMROIselected;


        bool isFirstComputedFrame;

        // For realtime simulation
        Random oRandom;

        // For sampling
        Sampling oSampler;

        //ADI BRT offline 
        //public float[] timeBRT;
        //public float[] dataBRTRaw;
        //public float[] dataBRTrate;

        public ArrayList timeBRT;
        public ArrayList dataBRTRaw;
        public ArrayList dataBRTrate;

        public float[,] dataBRTRaw_resampled;
        bool isBRTFileExisit;
        bool noThermalData;
        bool processStarted;
        bool IsoverlayClean;

        #endregion


        #region Public events

        public event PluginStatusDelegate StatusChange;
        //public event IntDelegate ThresholdChange;

        #endregion


        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "BreathPlugin";
        string myDescription = "Breathing Temperature Computation";
        string myAuthor = "Duc Duon";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        //System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        int iMyID;


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

        public string Name
        {
            get { return myName; }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return breathGUI; }
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

        public int MyID
        {
            set { iMyID = value; }
            get { return iMyID; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        #endregion


        #region Constructor

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///     Default constructor
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public Plugin()
        {
            #region Breathing
            //readonly
            //millisecondInSecond = 1000;
            samplingRate = 5 * 5; //change this to be 5 fps later
            // resamplingInterval = 1.0f / samplingRate; //* millisecondInSecond

            firstStage = 64 * 4;
            lastStage = 64 * 4; //256 128
            windowSizeMeanEdge = 50; //200
            powerScale = 15 * 5; //15 30*4
            filenameOutputExtension = @".breathing";
            #endregion
        }

        public void Initialize()
        {
            CreateMembers();
            //processStarted = true;  // TBD
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public Plugin(SerializationInfo info, StreamingContext context)
        {
            #region Breathing
            //readonly
            //millisecondInSecond = 1000;
            samplingRate = 5 * 5; //change this to be 5 fps later
            // resamplingInterval = 1.0f / samplingRate; //* millisecondInSecond

            firstStage = 64 * 4;
            lastStage = 64 * 4; //256 128
            windowSizeMeanEdge = 50; //200
            powerScale = 15 * 5; //15 30*4
            filenameOutputExtension = @".breathing";
            #endregion

            //CreateMembers();

            try
            {
                filename = (string)info.GetValue("filename", typeof(string));
                //breathTemperature = (int)info.GetValue("breathTemperature", typeof(int));
                //SetThresholdValue(breathTemperature);
            }
            catch { }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///     Setup Plug-in GUI
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void CreateMembers()
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {
            #region IRIS

            //create the input pins
            thermalTROIInputPin = Host.LoadOrCreatePin("Thermal", PinCategory.Critical, new Type[] { typeof(IFilterData), typeof(IThermalData) });
            mousePin = Host.LoadOrCreatePin("Mouse", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IMouseEventData) });
            filenamePin = Host.LoadOrCreatePin("File", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IStringData) });
            objOutputPathPin = Host.LoadOrCreatePin("Output Path", PinCategory.Optional, new Type[] { typeof(IStringData) });

            frameNumberPin = Host.LoadOrCreatePin("Frame Inform", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFrameInfo) }); //, typeof(IDateTimeData), typeof(ITimeSpanData) });
            //totalFramesInputPin = Host.LoadOrCreatePin("Total Frames", true, PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(ILongData), typeof(ITimeStampData), typeof(ITimeSpanData) });
            ProcessInitInputPin = Host.LoadOrCreatePin("Process Initiator", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IIntegerData) });   //Start writing as soon as this pin is one

            inPins.Add(thermalTROIInputPin);
            inPins.Add(mousePin);
            inPins.Add(filenamePin);
            inPins.Add(objOutputPathPin);
            inPins.Add(frameNumberPin);
            inPins.Add(ProcessInitInputPin);

            //create the output pins
            breathTemperaturePin = Host.LoadOrCreatePin("Temperature", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFloatData) });
            breathRatePin = Host.LoadOrCreatePin("Rate", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFloatData) });
            breathApneaPin = Host.LoadOrCreatePin("Apnea", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFloatData) });


            thermalMROIOutputpin = Host.LoadOrCreatePin("MROI Data", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IThermalData) });
            MROIHeight = Host.LoadOrCreatePin("MROI Height", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IIntegerData) });
            MROIWidth = Host.LoadOrCreatePin("MROI Width", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IIntegerData) });

            overlayMROIOutputPin = Host.LoadOrCreatePin("MROI overlay", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IOverlayData) });
            //ROIOutputPin = Host.LoadOrCreatePin("MROI ROI", false, PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IImageDescription), typeof(IImageDataFloatArray) });

            ADI_BRTOutputPin = Host.LoadOrCreatePin("ADI_BRT Wave[Offline]", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFloatData) });
            ADI_BRTrateOutputPin = Host.LoadOrCreatePin("ADI_BRT Rate[Offline]", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(IFloatData) });

            outPins.Add(breathTemperaturePin);
            outPins.Add(breathRatePin);
            outPins.Add(breathApneaPin);
            outPins.Add(thermalMROIOutputpin);
            outPins.Add(MROIHeight);
            outPins.Add(MROIWidth);
            outPins.Add(overlayMROIOutputPin);
            outPins.Add(ADI_BRTOutputPin);
            outPins.Add(ADI_BRTrateOutputPin);

            //MROI overlay
            //selectionOverlay = new OverlayPolygonData(0, this, 5, 3, Color.Red, Color.Black);
            selectionState = SelectionState.waitingForFirstPoint;
            selectionPoint1 = new PointF();
            selectionPoint2 = new PointF();

            //multithread
            mutex = new Mutex();
            mutex_FileRead = new Mutex();


            breathGUI = new PluginGUI(this);
            //add GUI
            //SetUserInterface(breathGUI);

            if (StatusChange != null)
                StatusChange("Waiting ...");

            #endregion


            #region Breathing

            //manual MROI
            bBaselineMROI = false;

            //frame number
            currentFrameNumber = 0;
            processedFrameNumber = 0;
            firstFrameNumber = 0;
            iIteration = 1;
            strOutputPath = ".\\"; // temp
            // bResample = true;
            //bResample = true;

            //timestamp
            firstFrameTimeStampInSecond = 0;

            //2D Graph scale;
            upperLimit = -100;
            lowerLimit = 100;
            //count = 100;

            isNewFrame = false;
            isLimitNotset = true;
            isMROIselected = false;

            // Realtime simulation
            oRandom = new Random();
            isFirstComputedFrame = true;

            // Sampler
            oSampler = new Sampling((double)1.0 / samplingRate, breathComputation.getSlidingWindownRef(), lastStage, breathComputation.computeMeanMROITemperature);

            //CWT
            ////breathComputation.initBreathComputation(firstStage, lastStage, 6);
            //breathComputation.initBreathComputation(firstStage, lastStage, samplingRate);
            breathComputation.initBreathComputation(firstStage, lastStage, samplingRate, powerScale);

            // ADI BRT offline processing
            timeBRT = new ArrayList();
            dataBRTRaw = new ArrayList(); // BRT waveform 
            dataBRTrate = new ArrayList(); // BRT rate

            isBRTFileExisit = false;
            noThermalData = false;
            processStarted = false;
            IsoverlayClean = true;

            measurmentBRT = new FloatData(0.0f); //"BRT wave");
            measurmentBRTrate = new FloatData(0.0f); //"BRT rate");

            //totalNumberOfFrames = 0;

            #endregion

            #region "3D Breath"
            //mMROIBoundFloatArray = new ThermalData();
            //mMROIBoundHeight = new IntegerData(0);
            //mMROIBoundWidth = new IntegerData(0); ;
            #endregion

        }

        #endregion


        #region Process Data

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Method to process all incoming data sent by other IRIS plugins to input pins on this plugin.
        /// </summary>
        ///     <param name="pin"  > The input pin on this plugin that is getting the data </param>
        ///     <param name="input"> The data being sent </param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Process(IPin pin, IPinData input)
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            if (pin == objOutputPathPin)
            {
                if (input != null)
                {
                    string s = ((IStringData)input).Data;
                    if (!string.IsNullOrEmpty(s) && !s.Equals(strOutputPath, StringComparison.OrdinalIgnoreCase))
                    {
                        strOutputPath = s;
                        if (!Directory.Exists(strOutputPath))
                        {
                            Directory.CreateDirectory(strOutputPath);
                        }
                        if (!string.IsNullOrEmpty(filename))
                        {
                            filenameOutput = strOutputPath + "\\" + Path.GetFileName(filename) + filenameOutputExtension;
                            if (saveRecord != null)
                            {
                                saveRecord.flushBufferToFile();
                                saveRecord = null;
                            }
                        }
                    }
                }
            }

            //process the mouse
            if (pin == filenamePin)
            {
                if (input != null)
                {
                    filename = ((IStringData)input).Data;
                    filename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename);

                    if (string.IsNullOrEmpty(strOutputPath))
                    { filenameOutput = filename + filenameOutputExtension; }
                    else
                    { filenameOutput = strOutputPath + "\\" + Path.GetFileName(filename) + filenameOutputExtension; }

                    if (saveRecord != null)
                    {
                        saveRecord.flushBufferToFile();
                        saveRecord = null;
                    }

                    //ADI BRT offline 
                    if (mutex_FileRead.WaitOne())
                    {
                        Application.UseWaitCursor = true;
                        if (dataBRTRaw_resampled != null)
                        { dataBRTRaw_resampled = null; }

                        ReadBRTData();
                        mutex_FileRead.ReleaseMutex();
                        Application.UseWaitCursor = false;

                    }

                    //manual MROI
                    //bBaselineMROI = false;

                    //frame number
                    currentFrameNumber = 0;
                    processedFrameNumber = 0;
                    firstFrameNumber = 0;
                    //bResample = true;

                    //timestamp
                    firstFrameTimeStampInSecond = 0;

                    //2D Graph scale;
                    upperLimit = -100;
                    lowerLimit = 100;

                    isNewFrame = false;
                    isLimitNotset = true;
                    isMROIselected = false;
                    breathComputation.resetBuffer();
                    breathComputation.computeOnce = true;

                    //update the overlay
                    //FilterGraph.OutputPinSendData(overlayMROIOutputPin, selectionOverlay);

                    breathComputation.RateComputed = -1;
                    breathComputation.MeanMROITemerature = -1;
                    breathComputation.StopChannelPower = -1;

                    Host.SendData(breathTemperaturePin, null, this);
                    Host.SendData(breathRatePin, null, this);
                    Host.SendData(breathApneaPin, null, this);
                }
            }

            //process filename and timestamp
            if (pin == frameNumberPin)
            {
                if (input != null)
                {
                    currentFrameNumber = (long)((IFrameInfo)input).FrameNumber;
                    timeStamp = ((IFrameInfo)input).Time;
                    
                    if (offsetFromStart != null)
                        prevOffsetFromStart = offsetFromStart;
                    
                    offsetFromStart = ((IFrameInfo)input).Span;
                    //filterGraph.CycleNumber;

                    if (processedFrameNumber != currentFrameNumber)
                    { isNewFrame = true; }
                }
            }

            /*if (pin == totalFramesInputPin)
            {
                if (input != null)
                {
                   // totalNumberOfFrames = ((ILongData)input).Value;
                }
            }*/

            //process the mouse
            if (pin == mousePin)
                ProcessMouse((IMouseEventData)input);
            else

            //process the thermal input image
            if (pin == thermalTROIInputPin)
            {
                //Display the image
                //DisplayMROI((IImageDescription)input, ((IImageDataFloatArray)input).Data);

                IsoverlayClean = false;
                //process the thermal 
                if (input != null)
                {
                    if (processStarted == true)
                    {
                        if (noThermalData == false)
                        {
                            ProcessThermal((IThermalData)input);
                        }
                        else
                        {
                            Host.SendData(breathTemperaturePin, new FloatData(breathTemperature), this);
                            Host.SendData(breathRatePin, new FloatData(breathRate), this);
                            Host.SendData(breathApneaPin, null, this);

                            // Save record
                            // Reset breathTemperature/breathRate to null 
                            this.breathRate = (float)0.0f;
                            SaveRecordToBuffer(new SampledItem[1]{new SampledItem(offsetFromStart.Seconds, breathTemperature, currentFrameNumber)});

                            if (saveRecord.getRows() >= 20)
                            {
                                SaveRecordToFile();
                            }
                        }
                    }
                }

                //signal that the processing is finished
                Host.SignalCriticalProcessingIsFinished(thermalTROIInputPin, this);
            }

            if (pin == ProcessInitInputPin)
            {
                if (input != null)
                {
                    //get the new state
                    int newProcessingState = ((IIntegerData)input).Value;

                    switch (newProcessingState)
                    { 
                        // Start
                        case 1:
                            processStarted = true;
                            noThermalData = false;
                            break;
                        // Stop
                        case 0:
                            processStarted = false;
                            SaveRecordToFile();
                            saveRecord = null;
                            IsoverlayClean = true;
                            isNewFrame = false;
                            isLimitNotset = true;
                            breathComputation.resetBuffer();
                            breathComputation.computeOnce = true;

                            breathComputation.RateComputed = 0;
                            breathComputation.MeanMROITemerature = 0;
                            breathComputation.StopChannelPower = 0;

                            if (selectionOverlay == null)
                                selectionOverlay = new PolygonData();

                            selectionOverlay.clearAll();
                            selectionOverlay.addVertice(new PointF(0f, 0f));
                            selectionOverlay.addVertice(new PointF(0f, 0f));
                            selectionOverlay.addVertice(new PointF(0f, 0f));
                            selectionOverlay.addVertice(new PointF(0f, 0f));

                            //update the overlay
                            Host.SendData(overlayMROIOutputPin, selectionOverlay, this);
                            isMROIselected = false;
                            noThermalData = true;
                            break;
                        // Pause
                        case 2:
                            IsoverlayClean = true;
                            isNewFrame = false;
                            isLimitNotset = true;
                            breathComputation.resetBuffer();
                            breathComputation.computeOnce = true;

                            breathComputation.RateComputed = 0;
                            breathComputation.MeanMROITemerature = 0;
                            breathComputation.StopChannelPower = 0;

                            noThermalData = true;
                            break;
                    }
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// Process thermal data
        /// </summary>
        ///     <param name="imageDescription"> Description of the input thermal image</param>
        ///     <param name="thermalData"     > The thermal pixels from the thermal image</param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void ProcessThermal(IThermalData thermalImageData)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {
            //make sure the data is valid
            if (thermalImageData == null)
                //SendData(breathTemperaturePin, null);
                return;

            //computation
            if ((processedFrameNumber == 0 || processedFrameNumber != currentFrameNumber) && isNewFrame)
            {
                if (processedFrameNumber == 0)
                {
                    firstFrameTimeStampInSecond = offsetFromStart.TotalSeconds;
                    firstFrameNumber = currentFrameNumber;
                    oSampler.setTimeOffsetInterval(firstFrameTimeStampInSecond);
                }

                //wait for exclusive access to the plug-in's critical data
                if (mutex.WaitOne())
                {
                    isNewFrame = false;
                    breathData.Top = 0;
                    breathData.Left = 0;
                    breathData.TROIWidth = thermalImageData.Width;
                    breathData.TROIHeight = thermalImageData.Height;
                    breathData.TROIData = thermalImageData.Data;

                    //auto MROI
                    // if (bBaselineMROI == false)
                    //  ProcessAutoMROI(description, thermalImageData);

                    //breathing rate
                    if (isMROIselected)
                    { 
                        // Update SampleRate;
                        long lFrameOffset = currentFrameNumber - firstFrameNumber;
                        breathComputation.SampleRate = (int)(lFrameOffset / (offsetFromStart.TotalSeconds - firstFrameTimeStampInSecond));

                        ComputeBreathMeasurement(thermalImageData);


                        Host.SendData(overlayMROIOutputPin, selectionOverlay, this);
                    }

                    // processedFrameNumber = currentFrameNumber;


                    //release exclusive access to the plug-in's critical data
                    mutex.ReleaseMutex();
                }//end of mutex
            }//end of process
            //else
            //{
            //    SendData(breathTemperaturePin, null);
            //    SendData(breathRatePin, null);
            //    SendData(breathApneaPin, null);
            //}

        }


        /// <summary>
        ///  Save to file
        /// </summary>
        public void SaveRecordToBuffer(SampledItem[] oSis)
        {

            if (saveRecord == null)
                saveRecord = new SaveRecord(filenameOutput);

            if (oSampler.checkRemoveLast())
                saveRecord.removeLastRecord();

            for (int i = 0; i < oSis.Length; i++)
            {
                //save the record to buffer
                saveRecord.saveRecordToBuffer(new object[] { oSis[i].iFrameNumber, oSis[i].dTimestamp,// currentFrameNumber, offsetFromStart.TotalSeconds, 
                                    oSis[i].oComputedData, // breathTemperature, 
                                    (breathRate.CompareTo(float.Epsilon)<= 0?"":breathRate.ToString()), 
                                    (dMaxPowerScale.CompareTo(float.Epsilon)<= 0?"":dMaxPowerScale.ToString())});
            }
        }

        /// <summary>
        ///  Save to file
        /// </summary>
        public void SaveRecordToFile()
        {
            if (saveRecord == null)
                saveRecord = new SaveRecord(filenameOutput);

            //save the record to buffer
            saveRecord.flushBufferToFile();
        }

        /// <summary>
        /// Trigger the GUI to display
        /// </summary>
        /// <param name="processing"></param>
        public void StartProcessing(int processing)
        {

            if (processing == 1)
            {
                if (StatusChange != null)
                    StatusChange("Processing ...");
            }
            else
            {
                if (StatusChange != null)
                    StatusChange("Processing Completed");
            }

        }

        void ReadBRTData()
        {
            string BRTFilename = filename + "-BRT[Resampled].txt";
            StreamReader SR;
            string S;
            float tempV = -10000;

            BRTFilename = filename + "-BRT.txt";
            if (!File.Exists(BRTFilename))
            {
                // MessageBox.Show("GSR File Does not exist " + GSRFilename);
                isBRTFileExisit = false;
                return;
            }

            isBRTFileExisit = true;

            char[] delimiterChars = { ' ', '\t', '\r' };

            timeBRT.Clear();
            dataBRTRaw.Clear();
            dataBRTrate.Clear();

            string Resampled = filename + "-BRT[Resampled].txt";

            if (File.Exists(Resampled))
            {
                SR = File.OpenText(Resampled);
                SR.ReadLine();
                int sID = 0;
                while ((S = SR.ReadLine()) != null)
                {
                    string[] words = S.Split(delimiterChars);

                    if (words[0] != "NaN") { timeBRT.Add(double.Parse(words[0])); }
                    else { timeBRT.Add(tempV); }

                    if (words[1] != "NaN") { dataBRTRaw.Add(double.Parse(words[1])); }
                    else { dataBRTRaw.Add(tempV); }

                    if (words[2] != "NaN") { dataBRTrate.Add(double.Parse(words[2])); }
                    else { dataBRTrate.Add(tempV); }


                    sID++;
                }
                SR.Close();

                // Load data
                if (dataBRTRaw_resampled == null)
                    dataBRTRaw_resampled = new float[timeBRT.Count, 3];
                for (sID = 0; sID < timeBRT.Count; sID++)
                {
                    dataBRTRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeBRT[sID]);
                    dataBRTRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataBRTRaw[sID]);
                    dataBRTRaw_resampled[sID, 2] = (float)System.Convert.ToDouble(dataBRTrate[sID]);
                }
            }
            else
            {
                SR = File.OpenText(BRTFilename);

                // Get starting timestamp
                SR.ReadLine();
                string strAdiL = SR.ReadLine();
                string[] strTs = strAdiL.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                // Convert to milliseconds
                DateTime mAdiDt = DateTime.Parse(strTs[strTs.Length - 1]);
                double dAdiMilli = (double)(((mAdiDt.Hour * 60) + mAdiDt.Minute) * 60 + mAdiDt.Second) + (double)(mAdiDt.Millisecond / 1000.0);

                // Skip next 6 lines
                SR.ReadLine();
                SR.ReadLine();
                SR.ReadLine();
                SR.ReadLine();
                SR.ReadLine();
                SR.ReadLine();

                //timeBRT = null;
                //dataBRTRaw = null;
                //dataBRTrate = null;

                //dataGSRRaw_resampled.Clear();
                //dataGSRNorm.Clear();

                //StreamReader textFile = new StreamReader(BRTFilename);
                //string fileContents = textFile.ReadToEnd();
                //textFile.Close();

                //string[] lines = fileContents.Split('\n');

                //timeBRT     = new float[lines.Length-1];
                //dataBRTRaw  = new float[lines.Length - 1];
                //dataBRTrate = new float[lines.Length - 1];

                //for (int l=0; l<lines.Length-1; l++)
                //{

                //    string[] words = lines[l].Split(delimiterChars);

                //    timeBRT[l] = -1;
                //    if (words[0] != "NaN") { timeBRT[l] = (float.Parse(words[0])); }
                //    dataBRTRaw[l] = -1;
                //    if (words[1] != "NaN") { dataBRTRaw[l] = (float.Parse(words[1])); }
                //    dataBRTrate[l] = -1;
                //    if (words[2] != "NaN") { dataBRTrate[l] = (float.Parse(words[2])); }

                //}

                // read each line (format: {timestamp pulse})
                while ((S = SR.ReadLine()) != null)
                {
                    string[] words = S.Split(delimiterChars);

                    if (words[0] != "NaN") { timeBRT.Add(double.Parse(words[0]) + dAdiMilli); }
                    else { timeBRT.Add(tempV); }

                    if (words[1] != "NaN") { dataBRTRaw.Add(double.Parse(words[1])); }
                    else { dataBRTRaw.Add(tempV); }

                    if (words[2] != "NaN") { dataBRTrate.Add(double.Parse(words[2])); }
                    else { dataBRTrate.Add(tempV); }

                }
                SR.Close();

                //Reseample the data
                ResampleBRT_Wrt_THR();

                // Store resampled data
                //Write GSR into file 
                WriteResampledBRT();
            }
        }

        private void WriteResampledBRT()
        {

            string BRTresampled_FileName = filename + "-BRT[Resampled].txt";
            StreamWriter SR_BRT = null;
            try
            {
                FileStream fileBRT = new FileStream(BRTresampled_FileName, FileMode.OpenOrCreate, FileAccess.Write);
                SR_BRT = new StreamWriter(fileBRT);
            }
            catch { }

            string oneLine;

            if (SR_BRT == null)
                return;

            // Write starting synchronized frame
            SR_BRT.WriteLine(lSyncFrame);

            for (int i = 0; i < dataBRTRaw_resampled.GetLength(0); i++)
            {
                oneLine = dataBRTRaw_resampled[i, 0].ToString() + '\t' +
                          dataBRTRaw_resampled[i, 1].ToString() + '\t' +
                          dataBRTRaw_resampled[i, 2].ToString();

                SR_BRT.WriteLine(oneLine);
            }

            try
            {
                SR_BRT.Close();

            }
            catch { }
        }

        private void ResampleBRT_Wrt_THR()
        {
            //Read inf file
            string subINF = filename + ".inf";
            if (!System.IO.File.Exists(subINF))
            {
                //MessageBox.Show("File Does not exist " + subINF);
                return;
            }

            int numberOfFramesIn_INF_file = 0;

            //////////////////////////////////
            // Read INF & Sample ADI
            float[] absTimeStamp; //1 column absolute time stamps (For example, 0, 0.01..)
            absTimeStamp = GetTimeStampFrom_INF_file(subINF, ref numberOfFramesIn_INF_file);

            // Sample the GSR based on the thermal data time stamps
            dataBRTRaw_resampled = Sampled_ADI(absTimeStamp);
            //////////////////////////////////
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
            float offset;

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

            absTime[0] = ((nThr_ref * 60) + nTmin_ref) * 60 + dTsec_ref; // First abs time stamp is special case 
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
            float[,] dataBRTRaw_resampled = new float[tSamples, 3]; //Store time, waveform, rate
            int BRTsize = dataBRTRaw.Count;
            int startID = 0;

            lSyncFrame = -1;
            for (int sID = 0; sID < tSamples; sID++)
            {
                if (absTimeStamp[sID] < (float)System.Convert.ToDouble(timeBRT[startID]))
                    continue;

                if (lSyncFrame == -1)
                    lSyncFrame = sID;

                for (int gsrID = startID; gsrID < BRTsize; gsrID++)
                {
                    // Absolute ADI timestamp
                    float fTmp = (float)System.Convert.ToDouble(timeBRT[gsrID]);

                    if (fTmp >= absTimeStamp[sID])
                    {
                        dataBRTRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeBRT[gsrID]);
                        dataBRTRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataBRTRaw[gsrID]);
                        dataBRTRaw_resampled[sID, 2] = (float)System.Convert.ToDouble(dataBRTrate[gsrID]);
                        startID = gsrID + 1;
                        break;
                    }


                    //////////////////////////////////////////////////////////////////
                    // Jin's Code
                    //float diff = (float)(Math.Abs(absTimeStamp[sID] - (float)System.Convert.ToDouble(timeBRT[gsrID])));
                    //if (diff <= 0.0001) //0.00001
                    //{
                    //    dataBRTRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeBRT[gsrID]);
                    //    dataBRTRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataBRTRaw[gsrID]);
                    //    dataBRTRaw_resampled[sID, 2] = (float)System.Convert.ToDouble(dataBRTrate[gsrID]);

                    //    startID = gsrID;
                    //    break;
                    //}

                    //else if ((float)System.Convert.ToDouble(timeBRT[gsrID]) > absTimeStamp[sID])
                    //{
                    //    dataBRTRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeBRT[gsrID - 1]);
                    //    dataBRTRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataBRTRaw[gsrID - 1]);
                    //    dataBRTRaw_resampled[sID, 2] = (float)System.Convert.ToDouble(dataBRTrate[gsrID - 1]);
                    //    startID = gsrID;
                    //    break;
                    //}
                    //else
                    //{
                    //    if (gsrID > 1)
                    //    {
                    //        dataBRTRaw_resampled[sID, 0] = (float)System.Convert.ToDouble(timeBRT[gsrID - 1]);
                    //        dataBRTRaw_resampled[sID, 1] = (float)System.Convert.ToDouble(dataBRTRaw[gsrID - 1]);
                    //        dataBRTRaw_resampled[sID, 2] = (float)System.Convert.ToDouble(dataBRTrate[gsrID - 1]);

                    //    }
                    //}
                    /////////////////////////////////////////////////////////////

                }//End of gsrID 
            }//End of sID 

            return dataBRTRaw_resampled;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// Breath measurement
        /// </summary>
        /// <param name="description"></param>
        /// <param name="thermalImageData"></param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void ComputeBreathMeasurement(IThermalData thermalImageData)
        {
            // Simulate realtime: frame is missing with 50%
            //if (oRandom.Next(100) < 50)
            //    return;

            if (processedFrameNumber == currentFrameNumber)
                return;

            //MROI data
            DataMROI(); //MROI data

            //#region "Testing"

            // Get nostril thermal signature by threshold/histogram
            // mNostrilFilter.process(breathData.MROIData, breathData.MROIWidth, breathData.MROIHeight);
            // mNostrilFilter.ThermalData = breathData.MROIData;
            // mNostrilFilter.ImageHeight = breathData.MROIHeight;
            // mNostrilFilter.ImageWidth = breathData.MROIWidth;
            // mNostrilFilter.writeNostrilSignature("c:\\images\\nostril.tiff");

            if (mMROIBoundFloatArray == null)
            {
                mMROIBoundFloatArray = new ThermalData(breathData.MROIData, breathData.MROIHeight, breathData.MROIWidth);
            }
            else
            {
                mMROIBoundFloatArray.SetFields(breathData.MROIData, breathData.MROIHeight, breathData.MROIWidth);
            }

            Host.SendData(thermalMROIOutputpin, mMROIBoundFloatArray, this);
            Host.SendData(MROIHeight, mMROIBoundHeight, this);
            Host.SendData(MROIWidth, mMROIBoundWidth, this);

            //// Get edge map of nostril - NOTE: computational expense
            //////////////////////////////
            //CannyEdgeDetector mCanny = new CannyEdgeDetector();
            //// mCanny.setBinaryThreshold((float)(breathData.highestTempratureValue + breathData.lowestTempratureValue) / (float)2.0);
            //mCanny.process(breathData.MROIGrayscaleData, breathData.MROIHeight, breathData.MROIWidth);

            //#endregion


            //breathing rate and temperature in MROI
            breathComputation.ThermalData = breathData.MROIData;

            tempTime = offsetFromStart.TotalSeconds;

            SampledItem[] oSis = null;
            
            if (processedFrameNumber == 0 || processedFrameNumber + 1 == currentFrameNumber)
            {
                oSis = oSampler.sampleWithoutInterpolate(currentFrameNumber, tempTime);
                processedFrameNumber = currentFrameNumber;
                // oSampler.NoInterpolationFrames = oSampler.NoInterpolationFrames + 1;
                //oSampler.totalNumberOfFrames = (int)totalNumberOfFrames;
            }
            else
            {
                // oSampler.NoInterpolationFrames = 0;
                oSis = oSampler.sampleWithInterpolate(ref processedFrameNumber, currentFrameNumber, tempTime);

            }
            
            //oSis = oSampler.sampleWithInterpolate(ref processedFrameNumber, currentFrameNumber, tempTime);

            //breathing rate and temperature in MROI
            //breathComputation.ThermalData = breathData.MROIData;
            //breathComputation.getMeanMROITemperature();

            //CWT
            breathComputation.breathRateCWT();

            dMaxPowerScale = (double)(breathComputation.MaxPowerScale);

            breathRate = (float)(breathComputation.RateComputed);
            breathTemperature = (float)(breathComputation.MeanMROITemerature);
            //if ((resamplingFrameNumber % (samplingRate * 15)) == 0)
            breathApnea = (float)(breathComputation.StopChannelPower);

            breathGUI.BreathingRate = breathRate;
            breathGUI.BreathTemperature = breathTemperature;
            breathGUI.BreathApnea = breathApnea;
            StartProcessing(1);

            //Console.WriteLine("breathing rate: " + breathRate.ToString());
            //Console.WriteLine("frame number: " + processedFrameNumber.ToString());
            //Console.WriteLine("breathing rate: " + breathRate.ToString());
            //send results to 2D Graph
            upperLimit = 35;
            lowerLimit = 30;

            if (isLimitNotset)
            {
                upperLimit = (float)breathTemperature + 1.5f;
                lowerLimit = (float)breathTemperature - 1.5f;
                isLimitNotset = false;

            }

            //if (count == 100)
            //{
            //    count = 0;

            //    //if (upperLimit < breathTemperature)
            //        upperLimit = (float)breathTemperature + 0.5f;

            //    //if (lowerLimit > breathTemperature)
            //        lowerLimit = (float)breathTemperature - 0.5f;

            //}
            //else
            //{
            //    count++;
            //}
            breathTemperatureOutputData = new FloatData(0.0f);// "Temperature (°C)", Color.Blue, 0, 500, lowerLimit, upperLimit); //31, 36);
            breathRateOutputData = new FloatData(0.0f); // ("Rate (cpm)", Color.Green, 0, 500, 5, 25);
            breathApneaOutputData = new FloatData(0.0f); // DataBoundColoredFloatData("Power", Color.Red, 0, 500, 0, 1000);

            breathTemperatureOutputData.Value = breathTemperature;
            breathRateOutputData.Value = breathRate;
            breathApneaOutputData.Value = breathApnea;

            Host.SendData(breathTemperaturePin, breathTemperatureOutputData, this);
            Host.SendData(breathRatePin, breathRateOutputData, this);
            Host.SendData(breathApneaPin, breathApneaOutputData, this);

            int FrameNum = (int)currentFrameNumber;
            if (dataBRTRaw_resampled != null && isBRTFileExisit && FrameNum >= 0 && FrameNum <= dataBRTRaw_resampled.GetLength(0))
            {
                float tempBRT = 0;
                // BRT waveform 
                tempBRT = (float)Convert.ToDouble(dataBRTRaw_resampled[FrameNum, 1]);
                measurmentBRT.Value = tempBRT;
                Host.SendData(ADI_BRTOutputPin, measurmentBRT, this);

                //BRT rate 
                tempBRT = (float)Convert.ToDouble(dataBRTRaw_resampled[FrameNum, 2]);
                measurmentBRTrate.Value = tempBRT;
                Host.SendData(ADI_BRTrateOutputPin, measurmentBRTrate, this);
            }
            else
            {
                Host.SendData(ADI_BRTOutputPin, null, this);
                Host.SendData(ADI_BRTrateOutputPin, null, this);
            }

            //output the results
            //if (filenameOutput != null)
            //    SaveRecordToFile();

            //else 
            //{
            //    //DataMROI();//MROI data

            //    //breathing rate and temperature in MROI
            //    //if (breathData.MROIData != null)
            //    //{
            //       // breathComputation.ThermalData = breathData.MROIData;
            //       // breathComputation.getMeanMROITemperature();
            //       // breathTemperature = (float)(breathComputation.MeanMROITemerature);

            //       // SendData(breathTemperaturePin, new FloatData(breathTemperature));
            //    //}

            //}

            if (iIteration > 0)
            {
                if (currentFrameNumber >= processedFrameNumber)
                {
                    // Save record
                    SaveRecordToBuffer(oSis);

                    if (saveRecord.getRows() >= 20)
                    {
                        SaveRecordToFile();
                    }
                }
                else
                {
                    SaveRecordToFile();

                    // Reduce the iteration
                    iIteration--;
                    //// Create a new record file
                    //filenameOutput = filename + @".output" + iIteration + ".breathing";

                    //// Create a new record
                    //saveRecord = null;
                    //SaveRecordToBuffer();
                }
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// Use the input thermal image to initialize MROI
        /// </summary>
        /// <param name="description"></param>
        /// <param name="thermalImageData"></param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void ProcessAutoMROI(IThermalData thermalImageData)
        {

            ////auto edge detection
            if (processedFrameNumber < windowSizeMeanEdge) //200
            {
                breathData.TROIData = thermalImageData.Data;

                //edge projection
                if (imageXGradientProjectionMean == null)
                    imageXGradientProjectionMean = new double[breathData.TROIWidth];

                if (imageYGradientProjectionMean == null)
                    imageYGradientProjectionMean = new double[breathData.TROIHeight];


                //sobel edge detection
                edgeDetection(processedFrameNumber, imageXGradientProjectionMean, imageYGradientProjectionMean);

            }
            else if (processedFrameNumber == windowSizeMeanEdge) //200
            {
                int top = breathData.Top + 1;
                int left = breathData.Left + 1;
                int right = breathData.Left + breathData.TROIWidth - 1;
                int bottom = breathData.Top + breathData.TROIHeight - 1;

                #region MROI Projection


                //compute the peak from projections
                int len = imageXGradientProjectionMean.Length;
                int mid = len / 2;

                //left boundary
                double maxProjection = -1;
                int tempPeak = left;
                for (int i = 0; i < mid; i++)
                {
                    if (imageXGradientProjectionMean[i] > maxProjection)
                    {
                        maxProjection = imageXGradientProjectionMean[i];
                        tempPeak = i + 1;
                    }
                }
                if (tempPeak > left && tempPeak < mid)
                    left = tempPeak;

                //right boundary
                maxProjection = -1;
                tempPeak = right;
                for (int i = mid; i < len; i++)
                {
                    if (imageXGradientProjectionMean[i] > maxProjection)
                    {
                        maxProjection = imageXGradientProjectionMean[i];
                        tempPeak = i + 1;
                    }
                }
                if (tempPeak > mid && tempPeak < right)
                    right = tempPeak;


                //bottom
                len = imageYGradientProjectionMean.Length;
                maxProjection = -1;
                tempPeak = bottom;
                for (int i = 0; i < len; i++)
                {
                    if (imageYGradientProjectionMean[i] > maxProjection)
                    {
                        maxProjection = imageYGradientProjectionMean[i];
                        tempPeak = i + 1;
                    }
                }
                if (tempPeak < bottom)
                    bottom = (tempPeak + bottom) / 2;

                //top
                tempPeak = bottom - (right - left) / 3;
                top = (top + 2 * tempPeak) / 3;
                //top = (top + tempPeak) / 2;

                #endregion


                //set the point from the mouse cursor position
                selectionPoint1.X = left;
                selectionPoint1.Y = top;
                selectionPoint2.X = right;
                selectionPoint2.Y = bottom;

                if (this.selectionOverlay == null)
                {
                    selectionOverlay = new PolygonData();
                    selectionOverlay.BorderColor = Color.White;
                    selectionOverlay.Thick = 1.0f;
                }
                selectionOverlay.clearAll();

                selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint1.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint1.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint2.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint2.Y));
                //selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint1.Y));

                //update the overlay
                Host.SendData(overlayMROIOutputPin, selectionOverlay, this);
                //FilterGraph.OutputPinSendData(overlayMROIOutputPin, selectionOverlay);

                bBaselineMROI = false;
            }


        }


        /// <summary>
        ///     Edge detection
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <param name="xGradientProjectionMean"></param>
        /// <param name="yGradientProjectionMean"></param>
        /// <param name="description"></param>
        /// <param name="thermalImageData"></param>
        private void edgeDetection(long frameNumber, double[] xGradientProjectionMean, double[] yGradientProjectionMean)
        {

            breathData.sobelEdge();

            //left and right edges
            for (int j = 0; j < breathData.TROIWidth; j++)
                xGradientProjectionMean[j] = (xGradientProjectionMean[j] * frameNumber
                    + breathData.imageXGradientProjection[j] * 1.0 / breathData.TROIWidth) / (frameNumber + 1);
            //base edge
            for (int j = 0; j < breathData.TROIHeight; j++)
                yGradientProjectionMean[j] = (yGradientProjectionMean[j] * frameNumber
                    + breathData.imageYGradientProjection[j] * 1.0 / breathData.TROIHeight) / (frameNumber + 1);

        }



        /// <summary>
        /// MROI
        /// </summary>
        private void DataMROI()
        {
            //init as TROI
            int topMROI = 0;
            int leftMROI = 0;
            int heightMROI = breathData.TROIHeight;
            int widthMROI = breathData.TROIWidth;

            float[] imageMROIDataBindings = null;

            if (selectionPoint2.Y != selectionPoint1.Y &&
                    selectionPoint2.X != selectionPoint1.X)
            {
                topMROI = (int)selectionPoint1.Y;
                leftMROI = (int)selectionPoint1.X;
                heightMROI = (int)(selectionPoint2.Y - selectionPoint1.Y);
                widthMROI = (int)(selectionPoint2.X - selectionPoint1.X);

            }

            if (imageMROIDataBindings == null)
                imageMROIDataBindings = new float[widthMROI * heightMROI];

            // TODO: uncomment when Thermal Visualizer is ready
            if ((breathData.TROIHeight >= (topMROI + heightMROI)) &&
               (breathData.TROIWidth >= (leftMROI + widthMROI)))
            {
                for (int i = 0; i < heightMROI; i++)
                    for (int j = 0; j < widthMROI; j++)
                        imageMROIDataBindings[widthMROI * i + j] =
                            breathData.TROIData[breathData.TROIWidth * (i + topMROI) + leftMROI + j];
            }

            // TODO: to be removed whtn Thermal Visualizer is ready
            //Array.Copy(breathData.TROIData, imageMROIDataBindings, widthMROI * heightMROI);

            breathData.MROIData = imageMROIDataBindings;
            breathData.MROIHeight = heightMROI;
            breathData.MROIWidth = widthMROI;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// Process the mouse data from the mouse input pin.
        /// </summary>
        ///     <param name="data">The mouse data</param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void ProcessMouse(IMouseEventData data)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {


            //wait for exclusive access to the plug-in's critical data
            if (mutex.WaitOne())
            {

                //take action based on the current state of the module
                switch (selectionState)
                {

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #region Perform mouse selection
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////		
                    case SelectionState.waitingForFirstPoint:
                        /////////////////////////////////////////////////////////////////////////		

                        //get the event type
                        if (data.Category == MouseEventCategory.MouseDown && data.Data.Button == MouseButtons.Left)
                        {
                            //set the point from the mouse cursor position
                            selectionPoint1.X = data.NormalizedXLocation;
                            selectionPoint1.Y = data.NormalizedYLocation;
                            selectionPoint2.X = data.NormalizedXLocation;
                            selectionPoint2.Y = data.NormalizedYLocation;

                            //change the state
                            selectionState = SelectionState.waitingForSecondPoint;
                        }

                        break;

                    /////////////////////////////////////////////////////////////////////////		
                    case SelectionState.waitingForSecondPoint:
                        /////////////////////////////////////////////////////////////////////////		

                        //check to see if the args is valid (it is null for enter and leave events)
                        if (data.Data != null)
                        {
                            // #1 IS HERE
                        }

                        //check to see if the mouse has been let up ... if so then the selection is finished
                        if (data.Category == MouseEventCategory.MouseUp && data.Data.Button == MouseButtons.Left)
                        {
                            SelectionMade();
                            selectionState = SelectionState.waitingForFirstPoint;

                            //////// Start #1
                            //get the second point from the mouse cursor position
                            selectionPoint2.X = data.NormalizedXLocation;
                            selectionPoint2.Y = data.NormalizedYLocation;

                            isMROIselected = true;
                            //////// End #1
                        }

                        //set the overlay data from the selection points
                        if (selectionOverlay == null)
                        {
                            selectionOverlay = new PolygonData();
                            selectionOverlay.BorderColor = Color.White;
                            selectionOverlay.Thick = 1.0f;
                        }
                        selectionOverlay.clearAll();
                        selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint1.Y));
                        selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint1.Y));
                        selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint2.Y));
                        selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint2.Y));
                        //selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint1.Y));


                        //update the overlay
                        Host.SendData(overlayMROIOutputPin,(IPolygonData) selectionOverlay, this);
                        //FilterGraph.OutputPinSendData(overlayMROIOutputPin, selectionOverlay);
                        //DisplayMROI();

                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                }


                //release exclusive access to the plug-in's critical data
                mutex.ReleaseMutex();
            }

        }

        //void DisplayMROI(IImageDescription imageDescription, float[] thermalData)
        //    {
        //        if (thermalData != null)
        //        {

        //           float []data = new float[imageDescription.Width * imageDescription.Height];
        //           ThermalImageData outputROI = new ThermalImageData(imageDescription.Width, imageDescription.Height, imageDescription.Width, imageDescription.ColorBands);
        //           outputROI.SetData(data);

        //           for (int j = 0; j < imageDescription.Height; j++) //rows
        //            for (int i = 0; i < imageDescription.Width; i++) //cols
        //            {
        //                int thermalAddress = (i * imageDescription.ColorBands) + (j * imageDescription.Stride);
        //                if (data != null && thermalAddress <= outputROI.Data.Length)
        //                {

        //                    data[thermalAddress] = thermalData[thermalAddress];
        //                }
        //            }

        //                    //float therhold = 0.05f;
        //                    //if (normalizedEnergyProperty) { therhold = 0; }
        //                    ////Perspiration
        //                    //if (j <= PerspirationROI.GetLength(0) &&
        //                    //    i <= PerspirationROI.GetLength(1) &&
        //                    //    PerspirationROI[j, i] > therhold)
        //                    //{
        //                    //    float temp = ((float)((PerspirationROI[j, i] / 1) * 6) +30);
        //                    //    if (temp > 36)
        //                    //    {
        //                    //        temp = 36;
        //                    //    }

        //                    //    data[thermalAddress] = temp;
        //                    //}
        //                //}

        //            //}


        //            //SendData(ROIOutputPin, outputROI);
        //            //data = null;
        //            //outputROI = null;
        //        }
        //        else
        //        {
        //            //SendData(ROIOutputPin, null);
        //        }

        //    }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// This method is called when the user selects the second point in the selection rectangle
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void SelectionMade()
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {
            bBaselineMROI = true;

            //reset auto detection
            imageXGradientProjectionMean = null;
            imageYGradientProjectionMean = null;

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        /// This method is called when the connection graph requests a plug-in reset
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public void Dispose()
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {
            ////wait for exclusive access to the plug-in's critical data
            //if (mutex.WaitOne())
            //{
            //    //call the base
            //    //base.Reset();

            //    //reset the selection
            //    selectionPoint1 = new PointF();
            //    selectionPoint2 = new PointF();

            //    //reset the selection state
            //    selectionState = SelectionState.waitingForFirstPoint;

            //    //reset the overlay
            //    PolygonData mEmpty = new PolygonData();
            //    Host.SendData(overlayMROIOutputPin, mEmpty, this);

            //    //breath computation
            //    bBaselineMROI = false;
            //    breathData = null;
            //    breathComputation = null;
            //    saveRecord = null;

            //    //release exclusive access to the plug-in's critical data
            //    mutex.ReleaseMutex();
            //}
        }

        #endregion

        #region ISerializable Members

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///     Saves important information about the plugin into a data stream
        /// </summary>
        /// <para name="info"> data stream </param>
        /// <param name="context"> data stream context </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        {
            //call the base class
            //base.GetObjectData(info, context);

            //info.AddValue("breathTemperature", breathTemperature);
            //info.AddValue("breathRate", breathRate);
            //info.AddValue("breathApnea", breathApnea);
            info.AddValue("filename", filename);
        }

        #endregion



    }


}

