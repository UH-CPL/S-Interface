using System;
using System.Collections.Generic;
using System.Linq;
using PluginInterface;
using System.Collections;
using System.Security.Permissions;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
//using Blink1Lib;
//using System.Threading;
using RDotNet;
using ThingM.Blink1;
using System.ComponentModel;
using System.IO;


// Hue color codes:
// www.developers.meethue.com/documentation/hue-xy-values
namespace RealTimeGraph
{
    public enum ActionCode
    {
        Stimulus,
        Baseline,
        Stress,
        Relaxed,
        Failure,
        None
    }

    public enum Biofeedback
    {
        Basic,
        Advanced1,
        SelfStart1,
        SelfStart2,
        None
    }

    public struct DataPoint
    {
        public int frameNum;
        public float time;
        public float data1;
        public float data2;
    }

    public struct Action
    {
        public float startTime;
        public float endTime;
        public ActionCode code;
    }

    [Serializable]
    public class RealTimeGraph : IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "Real Time Graph";
        string myDescription = "Plots Real Time Graph";
        string myAuthor = "Ashik Khatri";
        string myVersion = "1.1.1";
        IPluginHost myHost = null;
        int myID = -1;
        System.Windows.Forms.UserControl myMainInterface;
        //System.Windows.Forms.UserControl control;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        ArrayList dataPointsList = new ArrayList();
        ArrayList actionsList = new ArrayList();
        int curFrameNum;
        float curTime;
        float curData1;
        float curData2;
        bool isPerspiration;

        List<int> processedFrames = new List<int>();


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
            get { return control; }
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
        #region Real Time Graph Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Due to XML speficications all XML summaries for members must be put before the object declaration
        // therefore in the following region the comments for each member is on the previous line.  This
        // can be confusing, because the summary on the same line as a member is for the next member below it.
        // This format was adopted to keep the code readable.  
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        ///<summary> Input pin for data                                            </summary>                                            
        protected IPin dataInputPin;
        ///<summary> Input pin for data2                                            </summary>                                            
        protected IPin dataInputPin2;
        ///<summary> Input pin for frame number                                            </summary> 
        protected IPin frameInputPin;
        ///<summary> Input pin for start/stop trigger                                  </summary> 
        protected IPin startStopInputPin;
        ///<summary> Input pin to select a channel from the switch plugin                     </summary>
        protected IPin channelSelectorPin;
        protected IPin filenameInputPin;
        protected IPin biofeedbackInputPin;

        protected IPin UploadPPOutputPin;
        protected IPin AVIFrameRateOutputPin;


        ///<summary> The graphical user interface that plots the graph                     </summary>
        protected RealTimeGraphControl control;

        bool checkData = false;
        bool checkData2 = false;
        bool checkTime = false;
        bool isFirst = true;
        //double mean = 0;
        //double SD = 0;

        Dictionary<float, List<float>> intervals =   new Dictionary<float, List<float>>();


        float startSeconds = 0;
        float intialStartSeconds = 0;
        bool isCalibration = false;
        List<float> listOfMeasurements = new List<float>();
        ArrayList listOfMeasurementsTemp = new ArrayList();
        REngine engine = REngine.GetInstance();

        Blink1 blink1;

        float measurement = 0;
        float measurement2 = 0;
        float baseline = 0;
        float xAxisValue = 0;
        bool flagForColorChange = false;
        //int blState = 1, relxState = 2, stressState = 3;
        enum states: int { BL=1, RLX =2, STRESS =3}
        int currentState = 0;
        public bool isSerialized = false;
        //a counter to be used for cycles
        int counter = 0;
        protected string YAxisLabelSerial = "";
        protected string YAxisLabelBreathingSerial = "";
        protected string TitleSerial = "";
        protected string Graph1NameSerial = "";
        protected string Graph2NameSerial = "";
        protected bool RollingGraphSerial = false;
        protected bool Graph1ColorSerial = false;
        protected bool Graph2ColorSerial = false;
        protected bool UseTimeForXAxisSerial = false;

        ///<summary> y-Axis label for the graph    </summary>
        protected string yAxisLabel = "";
        ///<summary> y-Axis label for the breathing graph    </summary>
        protected string yAxisLabelBreathing = "";
        ///<summary> y-Axis label for the Perspiration graph    </summary>
        protected string yAxisLabelPerspiration = "";
        ///<summary> Title for the graph    </summary>
        protected string title = "";
        ///<summary> x-Axis max Scale setting of the graph   ///</summary>
        protected double xMaxScale;
        ///<summary> x-Axis min Tick setting of the graph   ///</summary>
        protected double xMinTick;
        ///<summary> x-Axis max Tick setting of the graph   ///</summary>
        protected double xMaxTick;
        /// <summary> a name for the graph 1        </summary>
        protected string graph1Name = "";
        /// <summary> a name for the graph 2        </summary>
        protected string graph2Name = "";
        /// <summary> sets whether the graph is rolling or collecting       </summary>
        protected bool rollingGraph = false;
        /// <summary> sets whether the graph1 is red (true) or blue (false)       </summary>
        protected bool graph1Color = false;
        /// <summary> sets whether the graph2 is red (true) or blue (false)       </summary>
        protected bool graph2Color = false;
        /// <summary> sets whether the graph uses time values for the x-Axis       </summary>
        protected bool useTimeForXAxis = false;
        /// <summary> triggers the start/stop event       </summary>
        protected bool startProcessing = false;
        /// <summary> the selected channel       </summary>
        protected int channelSelector = 0;

        public bool paused = false;
        

        WMPLib.WindowsMediaPlayer wplayer;
        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker bw2 = new BackgroundWorker();

        string fileName = null;
        

        string subjectName = null;
        ActionCode curActionCode = ActionCode.None;
        bool isBioFeedback = false;
        int stressFramesCount = 0;
        int relaxFramesCount = 0;
        const int REQUIREDSUCCESSIVEFRAMES = 3;
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////            
        public event TwoValuesFloatDelegate TwoMeasuredValueChanged;
        public event OneValueFloatDelegate OneMeasuredValueChanged;
        public event StringDelegate YAxisLabelChanged;
        public event StringDelegate YAxisLabelBreathingChanged;
        public event StringDelegate TitleChanged;
        public event StringDelegate Graph1NameChanged;
        public event StringDelegate Graph2NameChanged;
        public event BooleanDelegate RollingGraphChanged;
        public event BooleanDelegate Graph1ColorChanged;
        public event BooleanDelegate Graph2ColorChanged;
        public event BooleanDelegate UseTimeForXAxisChanged;
        public event BooleanDelegate StartStopChanged;
        public event IntegerDelegate ChannelSelectionChanged;
        public event BooleanDelegate ShowHideWaitForm;
        public event BooleanDelegate EnableDisableStressors;
        public event BiofeedbackDelegate BiofeedbackAlgorithmChanged;
        public event StringDelegate FileNameChanged;

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Properties
        ///////////////////////////////////////////////////////////////////////////////////////////            


        public string YAxisLabel
        {
            get
            {
                return yAxisLabel;
            }
            set
            {
                yAxisLabel = value;
                //call the yAxisLabelChangedEvent
                if (YAxisLabelChanged != null)
                    YAxisLabelChanged(yAxisLabel);
            }
        }

        public string YAxisLabelBreathing
        {
            get
            {
                return yAxisLabelBreathing;
            }
            set
            {
                yAxisLabelBreathing = value;
                //call the yAxisLabelChangedEvent
                if (YAxisLabelBreathingChanged != null)
                    YAxisLabelBreathingChanged(yAxisLabelBreathing);
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                //call the TitleChangedEvent
                if (TitleChanged != null)
                    TitleChanged(title);
            }
        }

        public string Graph1Name
        {
            get
            {
                return graph1Name;
            }
            set
            {
                graph1Name = value;
                //call the graph1NameChangedEvent
                if (Graph1NameChanged != null)
                    Graph1NameChanged(graph1Name);
            }
        }

        public string Graph2Name
        {
            get
            {
                return graph2Name;
            }
            set
            {
                graph2Name = value;
                //call the graph2NameChangedEvent
                if (Graph2NameChanged != null)
                    Graph2NameChanged(graph2Name);
            }
        }

        public bool RollingGraph
        {
            get
            {
                return rollingGraph;
            }
            set
            {
                rollingGraph = value;
                //call the RollingGraphChangedEvent
                if (RollingGraphChanged != null)
                    RollingGraphChanged(rollingGraph);
            }
        }


        public bool Graph1Color
        {
            get
            {
                return graph1Color;
            }
            set
            {
                graph1Color = value;
                //call the garph1ColorChangedEvent
                if (Graph1ColorChanged != null)
                    Graph1ColorChanged(graph1Color);
            }
        }

        public bool Graph2Color
        {
            get
            {
                return graph2Color;
            }
            set
            {
                graph2Color = value;
                //call the garph2ColorChangedEvent
                if (Graph2ColorChanged != null)
                    Graph2ColorChanged(graph2Color);
            }
        }

        public bool UseTimeForXAxis
        {
            get
            {
                return useTimeForXAxis;
            }
            set
            {
                useTimeForXAxis = value;
                //call the useTimeForXAxisChangedEvent
                if (UseTimeForXAxisChanged != null)
                    UseTimeForXAxisChanged(useTimeForXAxis);
            }
        }

        public bool Start
        {
            get
            {
                return startProcessing;
            }
            set
            {
                startProcessing = value;
                //call the StartStopChangedEvent
                //if (StartStopChanged != null)
                    //StartStopChanged(startProcessing);
            }
        }

        public int ChannelSelector
        {
            get
            {
                return channelSelector;
            }
            set
            {
                channelSelector = value;
                //call the ChannelSelectionChangedEvent
                if (ChannelSelectionChanged != null)
                    ChannelSelectionChanged(channelSelector);
            }
        }

        Biofeedback biofeedbackAlgorithm;
        public Biofeedback BiofeedbackAlgorithm
        {
            get
            {
                return biofeedbackAlgorithm;
            }
            set
            {
                biofeedbackAlgorithm = value;
            }
        }

        string waitFormMessage;
        public string WaitFormMessage
        {
            get
            {
                return waitFormMessage;
            }
            set
            {
                waitFormMessage = value;
            }
        }

        Biofeedback_P1 biofeedbackAdvanced1 = new Biofeedback_P1();
        bool isReadingNormalSignal = false;

        Biofeedback_Selfstart1 biofeedbackSelfStart1 = null;
        Biofeedback_Selfstart2 biofeedbackSelfStart2 = null;

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Plugin Required Methods
        ///////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
            
            try
            {
                REngine.SetEnvironmentVariables();
                blink1 = new Blink1();
                Console.WriteLine("Opening the first Blink(1) found.");
                blink1.Open();
                int versionNumber = blink1.GetVersion();
                Console.WriteLine("Blink(1) device is at version: {0}.", versionNumber.ToString());
                Console.WriteLine("Set Blink(1) off.\n");
                blink1.SetColor(0, 0, 0);

                //System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                //player.SoundLocation = "waves.mp3";
                //player.PlayLooping();

                //playWaveSound();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ///<summary> 
            /// The graphical user interface that plots the graph
            /// </summary>
            dataInputPin = Host.LoadOrCreatePin("Data 1", PinCategory.Critical, new Type[] { typeof(IFloatData) });
            inPins.Add(dataInputPin);

            dataInputPin2 = Host.LoadOrCreatePin("Data 2", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            inPins.Add(dataInputPin2);

            frameInputPin = Host.LoadOrCreatePin("Frame/Time", PinCategory.Optional, new Type[] { typeof(IFrameInfo) });
            inPins.Add(frameInputPin);

            startStopInputPin = Host.LoadOrCreatePin("Start/Stop", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(startStopInputPin);

            channelSelectorPin = Host.LoadOrCreatePin("Channel Selector", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(channelSelectorPin);
            //outImage = Host.LoadOrCreatePin("Image", PinCategory.Optional, new Type[] { typeof(IImageData) });
            //outPins.Add(outImage);

            filenameInputPin = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            inPins.Add(filenameInputPin);

            biofeedbackInputPin = Host.LoadOrCreatePin("Biofeedback", PinCategory.Optional, new Type[] { typeof(IBoolData) });
            inPins.Add(biofeedbackInputPin);

            UploadPPOutputPin = Host.LoadOrCreatePin("Upload pp", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(UploadPPOutputPin);

            AVIFrameRateOutputPin = myHost.LoadOrCreatePin("Frame Rate", PinCategory.Optional, new Type[] { typeof(IFilterData), typeof(ILongData) });
            outPins.Add(AVIFrameRateOutputPin);

            //myMainInterface = new ThresholdControl(this);
            //create the user interface
            control = new RealTimeGraphControl(this);
            //set the default values for the graph
            if (YAxisLabel == "")
            { YAxisLabel = "Value"; }
            if (YAxisLabelBreathing == "")
            { YAxisLabelBreathing = "Temperature [oC]"; }
            //if(Title=="")
            //{Title = "Graph";}
            //if (Graph1Name == "")
            //{ Graph1Name = "Graph 1"; }
            //if (Graph2Name == "")
            //{ Graph2Name = "Graph 2"; }
            Graph1Color = true;
            Graph2Color = false;
            RollingGraph = true;
            UseTimeForXAxis = true;
            startProcessing = false;
            if (isSerialized)
            {
                YAxisLabel = YAxisLabelSerial;
                YAxisLabelBreathing = YAxisLabelBreathingSerial;
                Title = TitleSerial;
                Graph1Name = Graph1NameSerial;
                Graph2Name = Graph2NameSerial;
                RollingGraph = RollingGraphSerial;
                Graph1Color = Graph1ColorSerial;
                Graph2Color = Graph2ColorSerial;
                UseTimeForXAxis = UseTimeForXAxisSerial;

                if(BiofeedbackAlgorithm != Biofeedback.None)
                {
                    BiofeedbackAlgorithmChanged(BiofeedbackAlgorithm);
                }
            }

            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_WorkComplete;
            EnableDisableStressors(isBioFeedback);

            bw2.DoWork += bw2_DoWork;
            bw2.RunWorkerCompleted += bw2_WorkComplete;

        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
            engine.Dispose();
            try
            {
                blink1.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void playWaveSound()
        {
            if(wplayer == null)
            {
                wplayer = new WMPLib.WindowsMediaPlayer();
                //wplayer.URL = @"C:\Users\CPLDemo\Desktop\Pradeep\Biofeedback\OTACS Development\RealTime Graph\RealTimeGraph\waves.mp3";
                wplayer.URL = @"C:\OTACS\waves.mp3";
            }

            if (wplayer.playState != WMPLib.WMPPlayState.wmppsPlaying)
            {
                wplayer.controls.play();
            }
        }

        public void stopPlayingWaveSound()
        {
            if (wplayer != null)
            {
                if (wplayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    wplayer.controls.stop();
                }
            }

            
        }


        public int ComputeArousalState(float currentTime)
        {
            double pValue = 0;
            double meanNew = 0, meanBaseline =0;
            float keyToDelete = -100;
            foreach (KeyValuePair<float, List<float>> pair in intervals)
            {

                if(currentTime - pair.Key > 30 )
                {
                    engine.Initialize();
                    //float[] x = pair.Value.ToArra;
                    var result = string.Join(",", pair.Value);
                    result = "group1 <- c(" + result + ")";
                    
                    NumericVector group1 = engine.Evaluate(result).AsNumeric();
                    var result2 = string.Join(",", listOfMeasurements);
                    result2 = "group2 <- c(" + result2 + ")";
                    NumericVector group2 = engine.Evaluate(result2).AsNumeric();

                    // Test difference of mean and get the P-value.
                    GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
                    double p = testResult["p.value"].AsNumeric().First();
                    meanNew = testResult["estimate"].AsNumeric().First();

                    meanBaseline = testResult["estimate"].AsNumeric().Last();
                    baseline = (float)meanBaseline;
                    /*
                    Console.WriteLine("Group1 count: " + pair.Value.Count);
                    Console.WriteLine("Baseline/Group2 count: " + listOfMeasurements.Count);
                    Console.WriteLine("Map count: " + intervals.Count);
                    Console.WriteLine("new : " + meanNew +  "  BSL " + meanBaseline);
                    Console.WriteLine("Group1: [{0}]", string.Join(", ", group1));
                    Console.WriteLine("Group2: [{0}]", string.Join(", ", group2));
                    Console.Write
                    Line("P-value = {0:0.000}", p);
                    */
                    pValue = p;
                    keyToDelete = pair.Key;
                }
            }

            if(keyToDelete != -100)
            {
                intervals.Remove(keyToDelete);
            }
            else
            {
                return -1; // none of intervals have more than 30 seconds yet
            }


            if(pValue > 0.05)
            {
                //Console.WriteLine("pValue > 0.05");
                return (int)states.BL;
            }
            else
            {
                //Console.WriteLine("meanNew = " + meanNew + "; meanBaseline = " + meanBaseline);
                if (meanNew > meanBaseline)
                {
                    return (int)states.STRESS;
                }
                else
                {
                    return (int)states.RLX;
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

        private void CreateExcelFile()
        {
            this.Host.SendData(UploadPPOutputPin, new IntegerData(0), this);

            bw.RunWorkerAsync();
            WaitFormMessage = "Creating perspiration signal...";
            ShowHideWaitForm(true);
        }

        public void Process(IPin pin, IPinData input)
        {
            if (pin == biofeedbackInputPin)
            {
                //check to see if the input is valid
                if (input != null)
                {
                    isBioFeedback = ((IBoolData)input).Data;
                    EnableDisableStressors(isBioFeedback);

                    if(isBioFeedback)
                    {
                        if(BiofeedbackAlgorithm == Biofeedback.Advanced1)
                        {
                            ReadNormalSignalIfNecessary();
                            //bool processNormalDriveSignal = false;

                            //if(biofeedbackAdvanced1.LCL == 0 && biofeedbackAdvanced1.UCL == 0)
                            //{
                            //    processNormalDriveSignal = true;
                            //}
                            //else
                            //{
                            //    if(biofeedbackAdvanced1.SubjectName != subjectName)
                            //    {
                            //        processNormalDriveSignal = true;
                            //    }
                            //}

                            //if (processNormalDriveSignal)
                            //{
                            //    bw2.RunWorkerAsync();
                            //    WaitFormMessage = "Processing Normal Drive signal for Baseline...";
                            //    ShowHideWaitForm(true);
                            //}
                        }
                    }
                }
            }

            if (pin == filenameInputPin)
            {
                //check to see if the input is valid
                if (input != null)
                {
                    fileName = ((IStringData)input).Data;
                    if (fileName.LastIndexOf('.') != -1)
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
                    }

                    FileNameChanged(fileName);

                    subjectName = Path.GetFileNameWithoutExtension(fileName);
                    int startIndex = subjectName.IndexOf("Subject");
                    int endIndex = subjectName.IndexOf("_");
                    subjectName = subjectName.Substring(startIndex, endIndex - startIndex);

                    if(isBioFeedback && BiofeedbackAlgorithm == Biofeedback.Advanced1)
                        ReadNormalSignalIfNecessary();
                }
            }

            counter++;
            if (channelSelectorPin.Connected == true)
            {
                if (pin == channelSelectorPin)
                {
                    ChannelSelectionChanged(((IIntegerData)input).Value);
                }
            }
            if (startStopInputPin.Connected == false)
            {
                startProcessing = true;
            }

            //check to see if the data is for our pins           
            if (pin == dataInputPin || pin == dataInputPin2 || pin == frameInputPin || pin == startStopInputPin)
            {
                if (pin == startStopInputPin)
                {
                    if (input != null)
                    {
                        int trigger = ((IIntegerData)input).Value;
                        if (trigger == 0)
                        {
                            paused = false;
                            startProcessing = false;

                            if(isBioFeedback)
                            {
                                try
                                {
                                    blink1.Close();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                stopPlayingWaveSound();  
                            }

                            if (isPerspiration)
                            {
                                DataPoint lastDP = (DataPoint)dataPointsList[dataPointsList.Count - 1];
                                /*
                                int frameRate = (int)Math.Floor(dataPointsList.Count / lastDP.time);
                                if(frameRate == 0)
                                {
                                    frameRate = 1;
                                }
                                */
                                this.Host.SendData(AVIFrameRateOutputPin, new IntegerData((int)lastDP.time), this);
                                EndAllPendingActions();
                                CreateExcelFile();
                            }

                            checkData = false;
                            checkData2 = false;
                            checkTime = false;
                            isFirst = true;
                            //double mean = 0;
                            //double SD = 0;

                            startSeconds = 0;
                            intialStartSeconds = 0;
                            isCalibration = false;
                            baseline = 0;
                            stressFramesCount = 0;
                            relaxFramesCount = 0;

                            
                        }
                        else if (trigger == 1)
                        {
                            listOfMeasurements.Clear();
                            processedFrames.Clear();
                            dataPointsList.Clear();
                            actionsList.Clear();
                            curActionCode = ActionCode.None;
                            

                            if (isBioFeedback)
                            {
                                try
                                {
                                    blink1.Open();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                if(BiofeedbackAlgorithm == Biofeedback.SelfStart1)
                                {
                                    biofeedbackSelfStart1 = null;
                                    biofeedbackSelfStart1 = new Biofeedback_Selfstart1();
                                }
                                else if (BiofeedbackAlgorithm == Biofeedback.SelfStart2)
                                {
                                    biofeedbackSelfStart2 = null;
                                    biofeedbackSelfStart2 = new Biofeedback_Selfstart2();
                                }
                            }

                            startProcessing = true;
                            paused = false;

                        }
                        else if (trigger == 2)
                        {
                            paused = true;
                        }

                        StartStopChanged(startProcessing);
                    }
                }

                if (pin == dataInputPin && startProcessing)
                {
                    //check to see if the input is valid
                    if (input != null && !checkData)
                    {
                        measurement = ((IFloatData)input).Value;
                        checkData = true;
                        curData1 = measurement;
                    }
                }

                if (pin == dataInputPin2 && startProcessing)
                {
                    //check to see if the input is valid
                    if (input != null && !checkData2)
                    {
                        measurement2 = ((IFloatData)input).Value;
                        checkData2 = true;
                        curData2 = measurement2;
                    }
                }

                if (pin == frameInputPin && startProcessing)
                {
                    if (input != null && !checkTime)
                    {
                        try
                        {
                            if (UseTimeForXAxis)
                            {
                                TimeSpan offsetFromStart = ((IFrameInfo)input).Span;
                                xAxisValue = (float)offsetFromStart.TotalSeconds;
                                checkTime = true;
                            }
                            else
                            {
                                xAxisValue = (int)((IFrameInfo)input).FrameNumber;
                                checkTime = true;
                            }

                            curFrameNum = (int)((IFrameInfo)input).FrameNumber;
                            curTime = (float)(((IFrameInfo)input).Span).TotalSeconds;
                        }
                        catch (Exception Exception)
                        {
                            string test = Exception.Message;
                        }
                    }
                }
                //check wether cycles or frames are used as input for the xaxis
                if (frameInputPin.Connected == false)
                {
                    xAxisValue = counter;
                    checkTime = true;
                }
                //in the case of not using data2
                if (dataInputPin2.Connected == false)
                {
                    if (checkData && checkTime && dataPointsList!=null)
                    {
                        DataPoint newData = new DataPoint();
                        newData.frameNum = curFrameNum;
                        newData.time = curTime;
                        // newData.data1 = curData1;
                        newData.data2 = baseline;
                        newData.data1 = measurement;

                        if (isValidDP(newData))
                        {
                            if(!isInProcessedFrames((int)curFrameNum))
                            {
                                processedFrames.Add((int)curFrameNum);

                                if (measurement < 1)
                                {
                                    dataPointsList.Add(newData);
                                }
                            }
                            

                            //if (dataInputPin2.Connected == false) // if this is not breath
                            //{
                                isPerspiration = true;

                                //OneMeasuredValueChanged(measurement, xAxisValue);
                                if (isBioFeedback)
                                {

                                    if (BiofeedbackAlgorithm == Biofeedback.Basic)
                                    {
                                        TwoMeasuredValueChanged(measurement, baseline, xAxisValue);

                                        //Console.WriteLine("x value  " + xAxisValue);
                                        if (isFirst && (xAxisValue > 1))
                                        {
                                            intialStartSeconds = xAxisValue;
                                            isFirst = false;
                                        }
                                        if ((xAxisValue - intialStartSeconds) > 30)
                                        {
                                            if (isCalibration)
                                            {
                                                //Console.WriteLine("Is calibrating: Difference  " + (xAxisValue - intialStartSeconds));
                                                float sum = 0;
                                                foreach (float val in listOfMeasurements)
                                                {
                                                    sum += val;
                                                }
                                                // mean = sum / listOfMeasurements.Count;
                                                // SD = StandardDeviation(listOfMeasurements);
                                                isCalibration = false;
                                                // Console.WriteLine("Mean calculation complete = " + mean + "   SD: " + SD);
                                                startSeconds = xAxisValue;
                                                intervals.Add(startSeconds, new List<float>());
                                            }
                                            //Console.WriteLine("HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
                                            //OneMeasuredValueChanged(measurement, xAxisValue);

                                            //Console.WriteLine("measurem " + measurement + "   mean: " + mean + "  SD" + SD);

                                            if ((xAxisValue - startSeconds) > 10)
                                            {
                                                Console.WriteLine("\n----------------\nDifference  " + (xAxisValue - intialStartSeconds));
                                                startSeconds = xAxisValue;
                                                intervals.Add(startSeconds, new List<float>());
                                                int emotionState = ComputeArousalState(startSeconds);


                                                foreach (KeyValuePair<float, List<float>> pair in intervals)
                                                {
                                                    pair.Value.Add(measurement);
                                                }

                                                if (emotionState == (int)states.STRESS)
                                                {
                                                    if (curActionCode != ActionCode.Stress)
                                                    {
                                                        CreateNewAction(ActionCode.Stress, true);
                                                        curActionCode = ActionCode.Stress;
                                                    }


                                                    DataPoint curDP = (DataPoint)dataPointsList[dataPointsList.Count - 1];
                                                    //Console.WriteLine("Current State = " + emotionState + " = Stressed");
                                                    //var state = JObject.Parse("{on: true, bri: 255, xy: [0.7,0.2986]}");
                                                    try
                                                    {
                                                        blink1.FadeToColor(1000, 255, 0, 0, false); // red

                                                        playWaveSound();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);
                                                    }

                                                }
                                                else if (emotionState == (int)states.RLX)
                                                {
                                                    if (curActionCode != ActionCode.Relaxed)
                                                    {
                                                        CreateNewAction(ActionCode.Relaxed, true);
                                                        curActionCode = ActionCode.Relaxed;
                                                    }

                                                    DataPoint curDP = (DataPoint)dataPointsList[dataPointsList.Count - 1];
                                                    //Console.WriteLine("Current State = " + emotionState + " = Relaxed");
                                                    //var state = JObject.Parse("{on: true, bri: 255, xy: [0.214,0.709]}");

                                                    try
                                                    {
                                                        blink1.FadeToColor(1000, 0, 0, 0, false); // green

                                                        stopPlayingWaveSound();
                                                        //playWaveSound();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);
                                                    }
                                                }
                                                else if (emotionState != -1)
                                                {
                                                    if (curActionCode != ActionCode.Baseline)
                                                    {
                                                        CreateNewAction(ActionCode.Baseline, true);
                                                        curActionCode = ActionCode.Baseline;
                                                    }

                                                    DataPoint curDP = (DataPoint)dataPointsList[dataPointsList.Count - 1];
                                                    //Console.WriteLine("Current State = " + emotionState + " = baseline/normal");
                                                    //var state = JObject.Parse("{on: true, bri: 100,xy: [0.3227, 0.329]}");
                                                    try
                                                    {
                                                        blink1.FadeToColor(1000, 0, 0, 0, false); // gray


                                                        stopPlayingWaveSound();
                                                        //playWaveSound();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (KeyValuePair<float, List<float>> pair in intervals)
                                                {
                                                    pair.Value.Add(measurement);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            listOfMeasurements.Add(measurement);
                                        }
                                    }
                                    else if (BiofeedbackAlgorithm == Biofeedback.Advanced1)
                                    {
                                        TwoMeasuredValueChanged(measurement, biofeedbackAdvanced1.UCL, xAxisValue);

                                        int windowSize = biofeedbackAdvanced1.WindowSize();
                                        if (listOfMeasurements.Count() < windowSize)
                                        {
                                            listOfMeasurements.Add(measurement);
                                        }
                                        else
                                        {
                                            listOfMeasurements.RemoveAt(0);
                                            listOfMeasurements.Add(measurement);
                                            float avg = 0;
                                            foreach (float item in listOfMeasurements)
                                            {
                                                avg += item;
                                            }
                                            avg /= (float)listOfMeasurements.Count();

                                            if (avg > biofeedbackAdvanced1.UCL)
                                            {
                                                if (stressFramesCount >= REQUIREDSUCCESSIVEFRAMES)
                                                {
                                                    relaxFramesCount = 0;

                                                    if (curActionCode != ActionCode.Stress)
                                                    {
                                                        CreateNewAction(ActionCode.Stress, true);
                                                        curActionCode = ActionCode.Stress;
                                                    }

                                                    try
                                                    {
                                                        blink1.FadeToColor(1000, 255, 0, 0, false); // red

                                                        playWaveSound();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);
                                                    }
                                                }
                                                else
                                                {
                                                    stressFramesCount++;
                                                }
                                            }
                                            else
                                            {
                                                if (relaxFramesCount >= REQUIREDSUCCESSIVEFRAMES)
                                                {
                                                    stressFramesCount = 0;

                                                    if (curActionCode != ActionCode.Baseline)
                                                    {
                                                        CreateNewAction(ActionCode.Baseline, true);
                                                        curActionCode = ActionCode.Baseline;
                                                    }

                                                    try
                                                    {
                                                        blink1.FadeToColor(1000, 0, 0, 0, false); // gray
                                                        //blink1.Complete();

                                                        stopPlayingWaveSound();
                                                        //playWaveSound();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);
                                                    }
                                                }
                                                else
                                                {
                                                    relaxFramesCount++;
                                                }
                                            }

                                        }
                                    }
                                    else if (BiofeedbackAlgorithm == Biofeedback.SelfStart1)
                                    {
                                        OneMeasuredValueChanged(measurement, xAxisValue);
                                        biofeedbackSelfStart1.CheckStress(measurement, xAxisValue);
                                        if(biofeedbackSelfStart1.IsStressed)
                                        {
                                            try
                                            {
                                                blink1.FadeToColor(1000, 255, 0, 0, false); // red

                                                playWaveSound();
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                            if (curActionCode != ActionCode.Stress)
                                            {
                                                CreateNewAction(ActionCode.Stress, true);
                                                curActionCode = ActionCode.Stress;
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                blink1.FadeToColor(1000, 0, 0, 0, false); // gray
                                                                                          //blink1.Complete();

                                                stopPlayingWaveSound();
                                                //playWaveSound();
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                            if (curActionCode != ActionCode.Baseline)
                                            {
                                                CreateNewAction(ActionCode.Baseline, true);
                                                curActionCode = ActionCode.Baseline;
                                            }
                                        }
                                    }
                                    else if (BiofeedbackAlgorithm == Biofeedback.SelfStart2 && measurement < 1)
                                    {
                                        OneMeasuredValueChanged(measurement, xAxisValue);
                                        biofeedbackSelfStart2.CheckStress(measurement, xAxisValue);
                                        if (biofeedbackSelfStart2.IsStressed)
                                        {
                                            try
                                            {
                                                blink1.FadeToColor(1000, 255, 0, 0, false); // red

                                                playWaveSound();
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                            if (curActionCode != ActionCode.Stress)
                                            {
                                                CreateNewAction(ActionCode.Stress, true);
                                                curActionCode = ActionCode.Stress;
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                blink1.FadeToColor(1000, 0, 0, 0, false); // gray
                                                                                          //blink1.Complete();

                                                stopPlayingWaveSound();
                                                //playWaveSound();
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                            if (curActionCode != ActionCode.Baseline)
                                            {
                                                CreateNewAction(ActionCode.Baseline, true);
                                                curActionCode = ActionCode.Baseline;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    OneMeasuredValueChanged(measurement, xAxisValue);
                                }

                                checkData = false;
                                checkTime = false;

                            /*
                            }

                            else  
                            {
                                
                            }
                            */
                        }

                        
                    }
                }
                else // this is breathing
                {
                    /*
                    if (checkData && checkData2 && checkTime && startProcessing)
                    {
                        TwoMeasuredValueChanged(measurement, measurement2, xAxisValue);
                        checkData = false;
                        checkData2 = false;
                        checkTime = false;
                    }
                    */

                    if(checkData && checkTime)
                    {
                        isPerspiration = false;
                        OneMeasuredValueChanged(measurement, xAxisValue);
                        //listOfMeasurementsTemp.Clear();
                        Console.WriteLine("measurement2: " + measurement2);

                        if (isBioFeedback)
                        {
                            if (measurement2 >= 20) // fast breathing 
                            {
                                try
                                {
                                    var state = JObject.Parse("{on: true, bri: 255, xy: [0.7,0.2986]}"); // red
                                    blink1.FadeToColor(1000, 255, 0, 0, false);

                                    playWaveSound();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else if (measurement2 < 10 && measurement2 > -1) // going to sleep or apnea
                            {
                                try
                                {
                                    var state = JObject.Parse("{on: true, bri: 255, xy: [0.3787,0.1724]}"); // magenta
                                    blink1.FadeToColor(1000, 255, 0, 255, false);

                                    playWaveSound();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else if (measurement2 >= 10 && measurement2 < 20) // normal breathing 
                            {
                                try
                                {
                                    var state = JObject.Parse("{on: true, bri: 100,xy: [0.3227, 0.329]}"); // grey
                                    blink1.FadeToColor(1000, 128, 128, 128, false);

                                    stopPlayingWaveSound();
                                    //playWaveSound();

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                        checkData = false;
                        checkData2 = false;
                        checkTime = false;
                    }
                }
                //let the system know that the next system cycle can begin
                this.Host.SignalCriticalProcessingIsFinished(dataInputPin, this);
            }
        }

        private bool isValidDP(DataPoint newDP)
        {
            if(newDP.data1 > 1)
            {
                return false;
            }

            if ((dataPointsList.Count < 10) && (newDP.frameNum > 50))
                return false;
            return true;
        }

        private bool isInProcessedFrames(int frameNum)
        {
            foreach (int frame in processedFrames)
            {
                if (frame == frameNum)
                {
                    return true;
                }
            }

            return false;
        }


        public static float StandardDeviation(ArrayList valueList)
        {
            float M = 0;
            float S = 0;
            int k = 1;
            foreach (float value in valueList)
            {
                float tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return (float) Math.Sqrt(S / (k - 2));
        }

        //        SignalCriticalProcessingIsFinished();
        //    //Put process code here

        //    if (pin == inImage)
        //    {
        //        int pixelSize = 4;
        //        ImageData curImgData = (ImageData)input;
        //        byte[] newImgArray = new byte[curImgData.Width * curImgData.Height * 4];
        //        for (int i = 0; i < curImgData.Height; i++)
        //        {
        //            for (int j = 0; j < curImgData.Width; j++)
        //            {
        //                byte RVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize)];
        //                byte GVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 1)];
        //                byte BVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 2)];
        //                byte AVal = curImgData.Data[i * (curImgData.Width * pixelSize) + (j * pixelSize + 3)];

        //                if (RVal < ThresholdVal)
        //                {
        //                    RVal = 0;
        //                    GVal = 0;
        //                    BVal = 0;
        //                }

        //                newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize)] = RVal;
        //                newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 1)] = GVal;
        //                newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 2)] = BVal;
        //                newImgArray[i * (curImgData.Width * pixelSize) + (j * pixelSize + 3)] = AVal;
        //            }
        //        }
        //        this.Host.SendData(outImage, new ImageData(newImgArray, curImgData.Height, curImgData.Width), this);


        //        this.Host.SignalCriticalProcessingIsFinished(inImage, this);
        //    }

        //}

        private void CreatePPFile()
        {
            string ppFilePath;

            if (fileName != null)
            {
                ppFilePath = fileName + ".pp";
            }
            else
            {
                ppFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                if (isPerspiration)
                    ppFilePath += "\\PerspirationSig.pp";
                else
                    ppFilePath += "\\BreathingSignal.brtl";
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                Console.WriteLine("Excel is not properly installed!!");
                return;
            }

            if (System.IO.File.Exists(ppFilePath))
            {
                System.IO.File.Delete(ppFilePath);
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBookPP;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetPP;
            object misValuePP = System.Reflection.Missing.Value;

            xlWorkBookPP = xlApp.Workbooks.Add(misValuePP);
            xlWorkSheetPP = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBookPP.Worksheets.get_Item(1);

            if (isPerspiration)
            {
                xlWorkSheetPP.Cells[8, 1] = "Perinasal Perspiration";
                xlWorkSheetPP.Cells[9, 1] = "Frame#";
                xlWorkSheetPP.Cells[9, 2] = "Time";
                xlWorkSheetPP.Cells[9, 3] = "Perspiration";
                if(isBioFeedback)
                {
                    if (BiofeedbackAlgorithm == Biofeedback.Basic)
                    {
                        xlWorkSheetPP.Cells[9, 4] = "Baseline";
                    }
                    else if (BiofeedbackAlgorithm == Biofeedback.Advanced1)
                    {
                        xlWorkSheetPP.Cells[9, 4] = "UCL";
                        xlWorkSheetPP.Cells[9, 5] = "LCL";
                    }
                    /*
                    else if (BiofeedbackAlgorithm == Biofeedback.SelfStart1)
                    {
                        xlWorkSheetPP.Cells[9, 4] = "Time";
                        xlWorkSheetPP.Cells[9, 5] = "DP";
                        xlWorkSheetPP.Cells[9, 6] = "Kn";
                        xlWorkSheetPP.Cells[9, 7] = "Ln";
                        xlWorkSheetPP.Cells[9, 8] = "Time";
                        xlWorkSheetPP.Cells[9, 9] = "Stress";
                    }
                    else if (BiofeedbackAlgorithm == Biofeedback.SelfStart2)
                    {
                        xlWorkSheetPP.Cells[9, 4] = "Time";
                        xlWorkSheetPP.Cells[9, 5] = "DP";
                        xlWorkSheetPP.Cells[9, 6] = "PCn";
                        xlWorkSheetPP.Cells[9, 7] = "Time";
                        xlWorkSheetPP.Cells[9, 8] = "Stress";
                    }
                    */
                }
                /*
                xlWorkSheet.Cells[9, 4] = "Biofeedback";
                xlWorkSheet.Cells[9, 5] = "Stressor";
                */

                int curRow = 10;
                foreach (DataPoint curData in dataPointsList)
                {
                    xlWorkSheetPP.Cells[curRow, 1] = curData.frameNum;
                    xlWorkSheetPP.Cells[curRow, 2] = curData.time;
                    xlWorkSheetPP.Cells[curRow, 3] = curData.data1;
                    if (isBioFeedback)
                    {
                        if (BiofeedbackAlgorithm == Biofeedback.Basic)
                        {
                            xlWorkSheetPP.Cells[curRow, 4] = curData.data2;
                        }
                        else if (BiofeedbackAlgorithm == Biofeedback.Advanced1)
                        {
                            xlWorkSheetPP.Cells[curRow, 4] = biofeedbackAdvanced1.UCL;
                            xlWorkSheetPP.Cells[curRow, 5] = biofeedbackAdvanced1.LCL;
                        }
                        /*
                        else if (BiofeedbackAlgorithm == Biofeedback.SelfStart1)
                        {
                            int index = curRow - 10;
                            xlWorkSheetPP.Cells[curRow, 4] = index;
                            if (index < biofeedbackSelfStart1.ListDataPoints.Count)
                            {
                                xlWorkSheetPP.Cells[curRow, 5] = biofeedbackSelfStart1.ListDataPoints[index];
                            }
                            if (index < biofeedbackSelfStart1.ListKn.Count)
                            {
                                xlWorkSheetPP.Cells[curRow, 6] = biofeedbackSelfStart1.ListKn[index];
                            }
                            if (index < biofeedbackSelfStart1.ListLn.Count)
                            {
                                xlWorkSheetPP.Cells[curRow, 7] = biofeedbackSelfStart1.ListLn[index];
                            }

                            foreach(Action action in actionsList)
                            {
                                if(action.code == ActionCode.Stress)
                                {
                                    xlWorkSheetPP.Cells[10, 8] = action.startTime;
                                    xlWorkSheetPP.Cells[10, 9] = 0;
                                    xlWorkSheetPP.Cells[11, 8] = action.endTime;
                                    xlWorkSheetPP.Cells[11, 9] = 0;
                                }
                            }
                        }
                        else if (BiofeedbackAlgorithm == Biofeedback.SelfStart2)
                        {
                            int index = curRow - 10;
                            xlWorkSheetPP.Cells[curRow, 4] = index;
                            if (index < biofeedbackSelfStart2.ListDataPoints.Count)
                            {
                                xlWorkSheetPP.Cells[curRow, 5] = biofeedbackSelfStart2.ListDataPoints[index];
                            }
                            if (index < biofeedbackSelfStart2.ListPCn.Count)
                            {
                                xlWorkSheetPP.Cells[curRow, 6] = biofeedbackSelfStart2.ListPCn[index];
                            }

                            foreach (Action action in actionsList)
                            {
                                if (action.code == ActionCode.Stress)
                                {
                                    xlWorkSheetPP.Cells[10, 7] = action.startTime;
                                    xlWorkSheetPP.Cells[10, 8] = 0;
                                    xlWorkSheetPP.Cells[11, 7] = action.endTime;
                                    xlWorkSheetPP.Cells[11, 8] = 0;
                                }
                            }
                        }
                        */
                    }
                    /*
                    xlWorkSheet.Cells[curRow, 4] = curData.bioFeedbackStatus;
                    xlWorkSheet.Cells[curRow, 5] = curData.stressorStatus;
                    */
                    curRow++;
                }

            }
            CreateDebugFile("About to save pp file", "debugrtg");
            xlWorkBookPP.SaveAs(ppFilePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValuePP, misValuePP, misValuePP, misValuePP, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValuePP, misValuePP, misValuePP, misValuePP, misValuePP);
            xlWorkBookPP.Close(true, misValuePP, misValuePP);
            xlApp.Quit();

            releaseObject(xlWorkSheetPP);
            releaseObject(xlWorkBookPP);
            releaseObject(xlApp);
        }

        private void CreateSTMFile()
        {
            string stmFilePath;

            if (fileName != null)
            {
                stmFilePath = fileName + ".stm";
            }
            else
            {
                stmFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                stmFilePath += "\\Stimulus.stm";
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                Console.WriteLine("Excel is not properly installed!!");
                return;
            }

            if (System.IO.File.Exists(stmFilePath))
            {
                System.IO.File.Delete(stmFilePath);
            }

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 8] = "ActionType Code:";
            xlWorkSheet.Cells[2, 8] = ActionCode.Stimulus;
            xlWorkSheet.Cells[2, 9] = ActionCode.Stimulus.ToString();
            xlWorkSheet.Cells[3, 8] = ActionCode.Baseline;
            xlWorkSheet.Cells[3, 9] = ActionCode.Baseline.ToString() + " Light";
            xlWorkSheet.Cells[4, 8] = ActionCode.Stress;
            xlWorkSheet.Cells[4, 9] = ActionCode.Stress.ToString() + " Light";
            xlWorkSheet.Cells[5, 8] = ActionCode.Relaxed;
            xlWorkSheet.Cells[5, 9] = ActionCode.Relaxed.ToString() + " Light";
            xlWorkSheet.Cells[6, 8] = ActionCode.Failure;
            xlWorkSheet.Cells[6, 9] = ActionCode.Failure.ToString();

            xlWorkSheet.Cells[9, 1] = "StartTime";
            xlWorkSheet.Cells[9, 2] = "EndTime";
            xlWorkSheet.Cells[9, 3] = "StartTime";
            xlWorkSheet.Cells[9, 4] = "EndTime";
            xlWorkSheet.Cells[9, 5] = "Event Switch";
            xlWorkSheet.Cells[9, 6] = "Action Type";
            xlWorkSheet.Cells[9, 7] = "Question Number";

            int curRow = 10;
            foreach (Action action in actionsList)
            {
                xlWorkSheet.Cells[curRow, 1] = action.startTime;
                xlWorkSheet.Cells[curRow, 2] = action.endTime;
                xlWorkSheet.Cells[curRow, 3] = action.startTime;
                xlWorkSheet.Cells[curRow, 4] = action.endTime;
                xlWorkSheet.Cells[curRow, 5] = 1;
                xlWorkSheet.Cells[curRow, 6] = action.code;
                if(action.code.ToString().Contains("Stimulus") || action.code.ToString().Contains("Failure"))
                    xlWorkSheet.Cells[curRow, 7] = action.code.ToString();
                else
                    xlWorkSheet.Cells[curRow, 7] = action.code.ToString() + " Light";

                curRow++;
            }
            CreateDebugFile("About to save stm file", "debugrtg");
            xlWorkBook.SaveAs(stmFilePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }
        
        private void ReadNormalSignalIfNecessary()
        {
            if(!isReadingNormalSignal)
            {
                isReadingNormalSignal = true;
                if (fileName != null)
                {
                    bool processNormalDriveSignal = false;

                    if (biofeedbackAdvanced1.LCL == 0 && biofeedbackAdvanced1.UCL == 0)
                    {
                        processNormalDriveSignal = true;
                    }
                    else
                    {
                        if (biofeedbackAdvanced1.SubjectName != subjectName)
                        {
                            processNormalDriveSignal = true;
                        }
                    }

                    if (processNormalDriveSignal)
                    {
                        if (biofeedbackAdvanced1.ReadNormalDriveSignal(fileName))
                        {
                            biofeedbackAdvanced1.CalculateTresholds();
                            baseline = biofeedbackAdvanced1.UCL;
                        }
                        else
                        {
                            BiofeedbackAlgorithm = Biofeedback.Basic;
                        }
                    }
                }
                isReadingNormalSignal = false;
            }
            
        }

        private void bw2_DoWork(object sender, DoWorkEventArgs e)
        {
            if(biofeedbackAdvanced1.ReadNormalDriveSignal(fileName))
            {
                biofeedbackAdvanced1.CalculateTresholds();
                baseline = biofeedbackAdvanced1.UCL;
            }
            else
            {
                BiofeedbackAlgorithm = Biofeedback.Basic;
            }

        }

        private void bw2_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            ShowHideWaitForm(false);

        }

        private void CreateDebugFile(string debugData, string fileExtension)
        {
            string sampleFilePath;

            if (fileName != null)
            {
                sampleFilePath = fileName + "." + fileExtension;
            }
            else
            {
                sampleFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                sampleFilePath += "\\debug.ashik";
            }

            using (StreamWriter sw = File.AppendText(sampleFilePath))
            {
                sw.WriteLine(debugData);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateDebugFile("About to write perspiration signal", "debugrtg");
            CreatePPFile();
            CreateSTMFile();
        }

        private void bw_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            ShowHideWaitForm(false);
            this.Host.SendData(UploadPPOutputPin, new IntegerData(1), this);
            //myMainInterface.HideWaitForm();
        }

        public void StartStopStressorAction(ActionCode stressorActionCode, bool start)
        {
            if(start)
            {
                CreateNewAction(stressorActionCode, false);

                if(isBioFeedback)
                {
                    if(BiofeedbackAlgorithm == Biofeedback.SelfStart1)
                    {
                        biofeedbackSelfStart1.ISStimulus = true;
                    }
                    else if (BiofeedbackAlgorithm == Biofeedback.SelfStart2)
                    {
                        biofeedbackSelfStart2.ISStimulus = true;
                    }
                }
            }
            else
            {
                int index = GetIndexofCurrentAction(stressorActionCode);
                if(index != -1)
                {
                    Action stressorAction = (Action)actionsList[index];
                    stressorAction.endTime = curTime;
                    actionsList[index] = stressorAction;
                }
            }
            
        }

        private int GetIndexofCurrentAction(ActionCode actionCode)
        {
            int index = -1;
            for(int i= actionsList.Count-1; i>=0; i--)
            {
                Action action = (Action)actionsList[i];
                if((action.code == actionCode) && (action.endTime == -1))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void EndAllPendingActions()
        {
            for (int i = 0; i < actionsList.Count; i++)
            {
                Action action = (Action)actionsList[i];
                if(action.endTime == -1)
                {
                    action.endTime = curTime;
                    actionsList[i] = action;
                }
            }
        }

        private void EndAllPendingBiofeedbackActions(float endTime)
        {
            for (int i = 0; i < actionsList.Count; i++)
            {
                Action action = (Action)actionsList[i];
                if ((action.endTime == -1) && ((action.code == ActionCode.Baseline) || (action.code == ActionCode.Relaxed) || (action.code == ActionCode.Stress)))
                {
                    action.endTime = endTime;
                    actionsList[i] = action;
                }
            }
        }

        private void CreateNewAction(ActionCode newActionCode, bool isBiofeedback)
        {
            if(isBiofeedback)
            {
                EndAllPendingBiofeedbackActions(curTime);
            }
            
            Action newAction = new Action();
            newAction.startTime = curTime;
            newAction.endTime = -1;
            newAction.code = newActionCode;
            actionsList.Add(newAction);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public RealTimeGraph()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public RealTimeGraph(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //ThresholdVal = (int)info.GetInt64("title");
            //try to load members from the saved info stream
            //try
            //{
            //    YAxisLabel = (string)info.GetString("yaxislabel");
            //    YAxisLabelBreathing = (string)info.GetString("yaxislabelbreathing");
            //    Title = (string)info.GetString("graphtitle");
            //    Graph1Name = (string)info.GetString("graph1Name");
            //    Graph2Name = (string)info.GetString("graph2Name");
            //    RollingGraph = (bool)info.GetBoolean("isRollingGraph");
            //    Graph1Color = (bool)info.GetBoolean("graph1Color");
            //    Graph2Color = (bool)info.GetBoolean("graph2Color");
            //    UseTimeForXAxis = (bool)info.GetBoolean("useTimeForXAxis");
            //}
            //catch
            //{
            //} 
            try
            {
                YAxisLabelSerial = (string)info.GetString("yaxislabel");
                YAxisLabelBreathingSerial = (string)info.GetString("yaxislabelbreathing");
                TitleSerial = (string)info.GetString("graphtitle");
                Graph1NameSerial = (string)info.GetString("graph1Name");
                Graph2NameSerial = (string)info.GetString("graph2Name");
                RollingGraphSerial = (bool)info.GetBoolean("isRollingGraph");
                Graph1ColorSerial = (bool)info.GetBoolean("graph1Color");
                Graph2ColorSerial = (bool)info.GetBoolean("graph2Color");
                UseTimeForXAxisSerial = (bool)info.GetBoolean("useTimeForXAxis");
                
            }
            catch
            {
            }

            try
            {
                BiofeedbackAlgorithm = (Biofeedback)info.GetInt32("biofeedback");
            }
            catch
            {
                BiofeedbackAlgorithm = Biofeedback.None;
            }
            isSerialized = true;

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //info.AddValue("title", ThresholdVal);
            info.AddValue("yaxislabel", YAxisLabel);
            info.AddValue("yaxislabelbreathing", YAxisLabelBreathing);
            info.AddValue("graphtitle", Title);
            info.AddValue("graph1Name", Graph1Name);
            info.AddValue("graph2Name", Graph2Name);
            info.AddValue("isRollingGraph", RollingGraph);
            info.AddValue("graph1Color", Graph1Color);
            info.AddValue("graph2Color", Graph2Color);
            info.AddValue("useTimeForXAxis", UseTimeForXAxis);
            info.AddValue("biofeedback", biofeedbackAlgorithm);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


    }


}
