using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Drawing;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace deformTracker
{
    public delegate void FloatDelegate(float value);

    [Serializable]
    public class MainProgram : IPlugin, ISerializable
    {
        //Declarations of all our internal plugin variables
        string myName = "DeformableTracker";
        string myDescription = "Applies the deformable tracker";
        string myAuthor = "Yan Zhou";
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




        public enum MAX { Thermal = 36, Visual = 255, MRI = 3 };
        public enum MIN { Thermal = 28, Visual = 0, MRI = 3 };
        public enum SelectionState { waitingForFirstPoint, waitingForSecondPoint };
        const float PI = (float)3.1415926;
        float[] globalRotationAngel = new float[13] { -PI/4, -PI*5/24, -PI*1/6, -PI*1/8, -PI/12, -PI/24, 
                                                            0, PI/24, PI/12, PI/8, PI/6, 5*PI/24, PI/4};
        int[, ,] meshOverlayDistribution = new int[4, 4, 2] { { { 0, 0 }, { 0, -1 }, { -1, -1 }, { -1, 0 } }, { { 0, 1 }, { 0, 0 }, { -1, 0 }, { -1, 1 } }, { { 1, 1 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, { { 1, 0 }, { 1, -1 }, { 0, -1 }, { 0, 0 } } };

        protected IPin thermalImageInputPin;
        protected IPin keepTrackingInputPin;
        protected IPin mouseDataInputPin;

        protected IPin trackerInfoOutputPin;
        protected IPin trackerFittingOutputPin;
        protected IPin trackingQualityOutputPin;
        protected IPin tissueDeformationOutputPin;
        protected IPin stateOverlayOutputPin;
        protected IPin thermalImageOutputPin;
        //protected IPin meshDataOutputPin;
        protected IPin meshOverlayOutputPin;
        //the variable below is for displaying center location in MRI tracking.
        //protected IPin centerOverlayOutputPin;
        //
     
        //the following two variables are for testing template method
        //protected IPin templateOutputPin;
        protected ThermalData outputTemplate; 

        protected OverlayPolygonData stateOverlay;
        protected OverlayPolygonData showStateOverlay;
        //the variable below is for displaying center location in MRI tracking.
        protected OverlayPolygonData showCenterOverlay;
        public InterestPoint interestPoint;
        //
        protected OverlayPolygonData[,] meshOverlay;
        protected OverlayPolygonData[,] showMeshOverlay;
        protected OverlayPolygonData artificialOverlay;
        protected OverlayPolygonData[,] indiTrackerStateOverlay;
        protected OverlayPolygonData[,] showIndiTrackerStateOverlay;
        protected ThermalData showOutputImage;
        public RectangleData stateInfo;
        public RectangleData trackerInfo;
        protected FloatData confidence;
        protected FloatData trackingQuality;
        protected FloatData tissueDeformation;
        public SelectionState selectionState;
        PointF selectionPoint1;
        PointF selectionPoint2;
        PointF backupSelectionPoint1;
        PointF backupSelectionPoint2;

        //need to eliminate this.
        public float[] normalizedImageData;
        public float[] unNormalizedImageData;
        public float[] reSampledImageData;
        protected ImageInfo imageInfo;
        protected ImageInfo downSampledImageInfo;
        protected int trackersColNum;
        protected int trackersRowNum;
        public int downSampleRate;
        public SingleTracker[,] singleTrackerArray;
        public TrackerInitialState trackerInitialState;
        

        public bool mousedone = true;
        public bool trackstatus = false;
        public bool firstTimeUse = false;
        public bool displayIndiState = false;
        public bool displayMeshOverlays = true;
        public bool recordData = false;
        public bool updateTemplate = true;
        //public bool enableGlobalSearching = true;
        public int localSearchRange = 1;
        public int globalSearchRange = 8;
        public int setTrackersColNum = 0;
        public int setTrackersRowNum = 0;
        public int artificialIndex = 6;
        public float max = (float)MAX.Thermal;
        public float min = (float)MIN.Thermal;
        public TrackerControl trackerControl;
        public PolygonData OTACSstateOverlay;
        public PolygonData OTACSMeshOverlay;
        public float motionSimilarityScore = 0;
        public float avgMotion = 0;
        public event FloatDelegate motionSimilarityChanged;
        public event FloatDelegate motionMeanChanged;

        public float motionSimilar
        {
            get
            {
                return motionSimilarityScore;
            }

            set
            {
                motionSimilarityScore = value;

                //call the XPositionChanged event
                if (motionSimilarityChanged != null)
                {
                    motionSimilarityChanged(motionSimilarityScore);
                    tissueDeformation.Value = motionSimilarityScore;
                    
                }
            }
        }

        public float motionMean
        {
            get
            {
                return avgMotion;
            }

            set
            {
                avgMotion = value;

                //call the XPositionChanged event
                if (motionMeanChanged != null)
                {
                    motionMeanChanged(avgMotion);
                    trackingQuality.Value = avgMotion;
                    //this.Host.SendData(trackingQualityOutputPin, trackingQuality, this);
                }


            }
        }

        public void Initialize()
        {

            myMainInterface = new TrackerControl(this);

            //input pins
            thermalImageInputPin = Host.LoadOrCreatePin("Thermal Video", PinCategory.Critical, new Type[] { typeof(IThermalData) });
            inPins.Add(thermalImageInputPin);
            mouseDataInputPin = Host.LoadOrCreatePin("Mouse Data", PinCategory.Optional, new Type[] { typeof(IMouseEventData) });
            inPins.Add(mouseDataInputPin);
            keepTrackingInputPin = Host.LoadOrCreatePin("Keep Tracking", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(keepTrackingInputPin);

            //output pins
            stateOverlayOutputPin = Host.LoadOrCreatePin("State Overlay", PinCategory.Optional, new Type[] { typeof(IPolygonData) });
            outPins.Add(stateOverlayOutputPin);
            trackerInfoOutputPin = Host.LoadOrCreatePin("Target", PinCategory.Optional, new Type[] { typeof(IRectangleData) });
            outPins.Add(trackerInfoOutputPin);
            thermalImageOutputPin = Host.LoadOrCreatePin("Output Image", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            outPins.Add(thermalImageOutputPin);
            trackerFittingOutputPin = Host.LoadOrCreatePin("Confidence", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(trackerFittingOutputPin);
            trackingQualityOutputPin = Host.LoadOrCreatePin("Tracking Quality", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(trackingQualityOutputPin);
            tissueDeformationOutputPin = Host.LoadOrCreatePin("Tissue Deformation", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(tissueDeformationOutputPin);
            meshOverlayOutputPin = Host.LoadOrCreatePin("Mesh Overlay", PinCategory.Optional, new Type[] { typeof(IPolygonData) });
            outPins.Add(meshOverlayOutputPin);

            stateOverlay = new OverlayPolygonData();
            showStateOverlay = new OverlayPolygonData();
            artificialOverlay = new OverlayPolygonData();
            OTACSstateOverlay = new PolygonData();
            OTACSMeshOverlay = new PolygonData();

            //MRI tracking
            showCenterOverlay = new OverlayPolygonData();
            interestPoint = new InterestPoint();
            //
             
            stateInfo = new RectangleData();
            trackerInfo = new RectangleData();
            confidence = new FloatData(100);
            trackingQuality = new FloatData(0);
            tissueDeformation = new FloatData(0);
            selectionState = SelectionState.waitingForFirstPoint;

          
           
          
         

        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            if (pin == keepTrackingInputPin)
            {
                if (input != null)
                {
                    //get the action
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 0)
                    {
                        trackstatus = false;
                    }
                }
            }

            if (pin == thermalImageInputPin)
            {
                    //make sure the input is valid
                    if (input != null)
                    {
                        //float[] inputImageData = ((IImageDataFloatArray)input).Data;
                        if (firstTimeUse == false)
                        {
                            imageInfo = new ImageInfo();
                            firstTimeUse = true;
                        }

                        //step1: get the image ready
                        getImageReadyToProcess(imageInfo, input);

                        //step2: tracking
                        tracking();

                        //step3: display
                        display();


                        //step4: post-processing.
                        reLocateFailureTrackers();
                        
                    }
                    else
                    {
                        this.Host.SendData(thermalImageOutputPin, null, this);
                        this.Host.SendData(stateOverlayOutputPin, null, this);
                        //SendData(meshOverlayOutputPin, null);
                        this.Host.SendData(trackerInfoOutputPin, null, this);
                        this.Host.SendData(trackerFittingOutputPin, null, this);
                        this.Host.SendData(tissueDeformationOutputPin, tissueDeformation, this);
                        this.Host.SendData(trackingQualityOutputPin, trackingQuality, this);
                        
                        
                    }
                    this.Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);
            }

            //process the mouse
            if (pin == mouseDataInputPin)
            {
                if (input != null)
                    ProcessMouse((IMouseEventData)input);
                //this.Host.SignalCriticalProcessingIsFinished(mouseDataInputPin, this);
            }

  
        }





        private void display()
        {
            if (trackstatus == true && mousedone == true)
            {
                float totalMatchScore = 0;
                for (int i = 0; i < trackersColNum; i++)
                {
                    for (int j = 0; j < trackersRowNum; j++)
                    {
                        if (singleTrackerArray[i, j].isSurvivor == true)
                        {
                            computeOverlayFromTrackerState(indiTrackerStateOverlay[i, j], singleTrackerArray[i, j].centerX, singleTrackerArray[i, j].centerY, (singleTrackerArray[i, j].rotation + globalRotationAngel[artificialIndex]), singleTrackerArray[i, j].height, singleTrackerArray[i, j].width);
                            //below are for MRI tracking
                            singleTrackerArray[i, j].distToPoint = Math.Sqrt((singleTrackerArray[i, j].centerX - interestPoint.x) * (singleTrackerArray[i, j].centerX - interestPoint.x) + (singleTrackerArray[i, j].centerY - interestPoint.y) * (singleTrackerArray[i, j].centerY - interestPoint.y));
                            totalMatchScore = (float)(totalMatchScore + singleTrackerArray[i, j].bestMatchScore / (singleTrackerArray[i, j].distToPoint + 1));
                            //
                            //totalMatchScore = totalMatchScore + singleTrackerArray[i, j].bestMatchScore;

                        }
                        else
                        {
                            indiTrackerStateOverlay[i, j].x[0] = 0;
                            indiTrackerStateOverlay[i, j].y[0] = 0;
                            indiTrackerStateOverlay[i, j].x[1] = 0;
                            indiTrackerStateOverlay[i, j].y[1] = 0;
                            indiTrackerStateOverlay[i, j].x[2] = 0;
                            indiTrackerStateOverlay[i, j].y[2] = 0;
                            indiTrackerStateOverlay[i, j].x[3] = 0;
                            indiTrackerStateOverlay[i, j].y[3] = 0;
                            indiTrackerStateOverlay[i, j].x[4] = 0;
                            indiTrackerStateOverlay[i, j].y[4] = 0;

                        }

                        if (displayIndiState == true)
                        {
                            computeOverlayAfterDownSampling(indiTrackerStateOverlay[i, j], showIndiTrackerStateOverlay[i, j], downSampleRate);
                        
                        }
                        else
                        {
                           
                        }

                    }

                }


                
                computeOverallState(stateOverlay, singleTrackerArray, trackersColNum, trackersRowNum, totalMatchScore);  
                computeOverlayAfterDownSampling(stateOverlay, showStateOverlay, downSampleRate);
                if (displayMeshOverlays == true)
                {
                    computeMeshOverlay(singleTrackerArray, indiTrackerStateOverlay, meshOverlay, trackersColNum, trackersRowNum);
                    for (int i = 0; i < trackersColNum; i++)
                    {
                        for (int j = 0; j < trackersRowNum; j++)
                        {
                            computeOverlayAfterDownSampling(meshOverlay[i, j], showMeshOverlay[i, j], downSampleRate);
                            formatToOTACSPolygonData(showMeshOverlay[i, j], OTACSMeshOverlay);
                            this.Host.SendData(meshOverlayOutputPin, (IPolygonData)OTACSMeshOverlay, this);
                        }

                    }
                }   

                computeArtificialOverallState(showStateOverlay);
                formatToOTACSPolygonData(showStateOverlay, OTACSstateOverlay);
                this.Host.SendData(stateOverlayOutputPin, (IPolygonData)OTACSstateOverlay, this);
                this.Host.SendData(tissueDeformationOutputPin, tissueDeformation, this);
                this.Host.SendData(trackingQualityOutputPin, trackingQuality, this);
                
                //MRI tracking
                //FilterGraph.OutputPinSendData(centerOverlayOutputPin, showCenterOverlay);
                //
                
                displayOutputImage();
                displayOutputTemplate(singleTrackerArray[0,0], imageInfo.colorbands);
            }
            else
            {
                this.Host.SendData(thermalImageOutputPin, null, this);
                this.Host.SendData(stateOverlayOutputPin, null, this);
               // SendData(meshOverlayOutputPin, null);
                //SendData(templateOutputPin, null);
                this.Host.SendData(trackerInfoOutputPin, null, this);
                this.Host.SendData(trackerFittingOutputPin, null, this);
            }
  

        }

        private void displayOutputTemplate(SingleTracker singleTracker, int colorbands)
        {
            //float max = (float)MAX.Thermal;
            //float min = (float)MIN.Thermal;
            float[] data2 = new float[singleTracker.height * singleTracker.width];
            for (int m = 0; m < singleTracker.height; m++)
            {
                for (int n = 0; n < singleTracker.width; n++)
                {
                    data2[m * (singleTracker.width - 1) + n] = singleTracker.currentTemplate[m, n] * (max - min) / 255 + min;
                }
            }
            //outputTemplate = new ThermalImageData(singleTracker.width - 1, singleTracker.height - 1, singleTracker.width - 1, colorbands);
            //outputTemplate.SetData(data2);
            //SendData(templateOutputPin, outputTemplate);
        }

        private void computeMeshOverlay(SingleTracker[,] singleTrackerArray, OverlayPolygonData[,] indiTrackerStateOverlay, OverlayPolygonData[,] meshOverlay, int trackersColNum, int trackersRowNum)
        {
            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    if (singleTrackerArray[i, j].isSurvivor == true)
                    {
                        //compute 2
                        for (int k = 0; k < 4; k++)
                        {
                            computeMeshJointPoint(meshOverlay[i, j], singleTrackerArray, indiTrackerStateOverlay, i, j, k);
                        }
                        //produce 0, 1, 3, 4
                        meshOverlay[i, j].x[4] = meshOverlay[i, j].x[0];
                        meshOverlay[i, j].y[4] = meshOverlay[i, j].y[0];
                    }
                    else
                    {
                        meshOverlay[i, j].x[0] = 0;
                        meshOverlay[i, j].y[0] = 0;
                        meshOverlay[i, j].x[1] = 0;
                        meshOverlay[i, j].y[1] = 0;
                        meshOverlay[i, j].x[2] = 0;
                        meshOverlay[i, j].y[2] = 0;
                        meshOverlay[i, j].x[3] = 0;
                        meshOverlay[i, j].y[3] = 0;
                        meshOverlay[i, j].x[4] = meshOverlay[i, j].x[0];
                        meshOverlay[i, j].y[4] = meshOverlay[i, j].y[0];
                    }
                }
            }
        }

        private void computeMeshJointPoint(OverlayPolygonData oneOverlay, SingleTracker[,] singleTrackerArray, OverlayPolygonData[,] indiTrackerStateOverlay, int i, int j, int NthPoint)
        {
            double totalScore = 0;

            //compute total score.
            for (int k = 0; k < 4; k++)
            {
                int newI = i + meshOverlayDistribution[NthPoint, k, 0];
                int newJ = j + meshOverlayDistribution[NthPoint, k, 1];

                if (newI >= 0 && newJ >= 0 && newI < trackersColNum && newJ < trackersRowNum)
                {
                    if (singleTrackerArray[newI, newJ].isSurvivor == true)
                    {
                        totalScore = totalScore + singleTrackerArray[newI, newJ].bestMatchScore;
                       
                        
                    }
                }
                
            }

            //compute the joint point.
            oneOverlay.x[NthPoint] = 0;
            oneOverlay.y[NthPoint] = 0;

            for (int k = 0; k < 4; k++)
            {
                int newI = i + meshOverlayDistribution[NthPoint, k, 0];
                int newJ = j + meshOverlayDistribution[NthPoint, k, 1];

                if (newI >= 0 && newJ >= 0 && newI < trackersColNum && newJ < trackersRowNum)
                {
                    if (singleTrackerArray[newI, newJ].isSurvivor == true)
                    {
                        oneOverlay.x[NthPoint] = (float)(oneOverlay.x[NthPoint] + (singleTrackerArray[newI, newJ].bestMatchScore / totalScore) * indiTrackerStateOverlay[newI, newJ].x[k]);
                        oneOverlay.y[NthPoint] = (float)(oneOverlay.y[NthPoint] + (singleTrackerArray[newI, newJ].bestMatchScore / totalScore) * indiTrackerStateOverlay[newI, newJ].y[k]);
                    }
                }

            }
            
        }

        private void computeArtificialOverallState(OverlayPolygonData showStateOverlay)
        {
            float tg = (stateOverlay.y[2] - stateOverlay.y[3]) / (stateOverlay.x[2] - stateOverlay.x[3]);
            double showAngle = Math.Atan((double)tg);
            showAngle = showAngle + globalRotationAngel[artificialIndex];
            int originalWidth = trackerInitialState.trackerInitialWidth;
            int originalHeight = trackerInitialState.trackerInitialHeight;
            int originalCenterX = (int)(stateInfo.CenterX * downSampleRate);
            int originalCenterY = (int)(stateInfo.CenterY * downSampleRate);
            computeOverlayFromTrackerState(showStateOverlay, originalCenterX, originalCenterY, (float)showAngle, originalHeight, originalWidth);
            //MRI tracking
            computeCenterOverlay(showCenterOverlay, originalCenterX, originalCenterY, interestPoint);
            //
            
            if (recordData == true)
            {
                /*
                using (StreamWriter sr = File.AppendText(path1))
                {
                    sr.WriteLine("{0}, {1}", ((int)showCenterOverlay.x[0]).ToString(), ((int)showCenterOverlay.y[0]).ToString(), path1);
                    
                }
                 */
                /*
                using (StreamWriter sr = File.AppendText(path2))
                {
                    sr.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", originalCenterX.ToString(), originalCenterY.ToString(), originalHeight.ToString(), originalWidth.ToString(), showAngle.ToString(), trackersColNum.ToString(), trackersRowNum.ToString(), path2);
                }
                */
            }
            
        }

        private void computeCenterOverlay(OverlayPolygonData showCenterOverlay, int originalCenterX, int originalCenterY, InterestPoint interestPoint)
        {
            showCenterOverlay.x[0] = originalCenterX + interestPoint.distX;
            showCenterOverlay.y[0] = originalCenterY + interestPoint.distY;
            interestPoint.x = (int)showCenterOverlay.x[0];
            interestPoint.y = (int)showCenterOverlay.y[0];
            showCenterOverlay.x[1] = showCenterOverlay.x[0] + 1;
            showCenterOverlay.y[1] = showCenterOverlay.y[0];
            showCenterOverlay.x[2] = showCenterOverlay.x[1];
            showCenterOverlay.y[2] = showCenterOverlay.y[0] + 1;
            showCenterOverlay.x[3] = showCenterOverlay.x[0];
            showCenterOverlay.y[3] = showCenterOverlay.y[2];
            showCenterOverlay.x[4] = showCenterOverlay.x[0];
            showCenterOverlay.y[4] = showCenterOverlay.y[0];
        }

    

        private void displayOutputImage()
        {

            //float max = (float)MAX.Thermal;
            //float min = (float)MIN.Thermal;
            float tg = (showStateOverlay.y[2] - showStateOverlay.y[3]) / (showStateOverlay.x[2] - showStateOverlay.x[3]);      
            double showAngle = Math.Atan((double)tg);
            double sinine = Math.Sin(showAngle);
            double cosine = Math.Cos(showAngle);
            int originalWidth = trackerInitialState.trackerInitialWidth;
            int originalHeight = trackerInitialState.trackerInitialHeight;
            int originalCenterX = (int)(stateInfo.CenterX * downSampleRate);
            int originalCenterY = (int)(stateInfo.CenterY * downSampleRate);
            float[] temporal_data = new float[originalHeight * originalWidth];
            int halfwidth = (int)(originalWidth / 2 + 0.5);
            int halfheight = (int)(originalHeight / 2 + 0.5);

            bool isValidData = false;
            for (int m = (int)(originalCenterX - halfwidth); m < (int)(originalCenterX + halfwidth); m++)
            {
                for (int n = (int)(originalCenterY - halfheight); n < (int)(originalCenterY + halfheight); n++)
                {

                    int new_x = (int)((m - originalCenterX) * cosine - (n - originalCenterY) * sinine);
                    int new_y = (int)((m - originalCenterX) * sinine + (n - originalCenterY) * cosine);

                    int thermalAddress = (int)((originalCenterX + new_x) * imageInfo.colorbands + (originalCenterY + new_y) * imageInfo.stride);
                    int temp = (int)((n - originalCenterY + halfheight) * originalWidth + m - originalCenterX + halfwidth);
                    if (temp >= 0 && temp < temporal_data.Length)
                    {
                        if (thermalAddress >= normalizedImageData.Length || thermalAddress < 0)
                        {
                            temporal_data[temp] = 0F;
                        }
                        else
                        {
                            temporal_data[temp] = normalizedImageData[thermalAddress] * (max - min) / 255 + min;
                        }

                        if(temporal_data[temp] > 0)
                        {
                            isValidData = true;
                        }
                    }

                }
            }

            if(isValidData)
            {
                showOutputImage = new ThermalData(temporal_data, (int)originalHeight, (int)originalWidth);

                trackerInfo.Width = originalWidth;
                trackerInfo.Height = originalHeight;
                trackerInfo.CenterX = originalCenterX;
                trackerInfo.CenterY = originalCenterY;
                trackerInfo.Rotation = showAngle;
                this.Host.SendData(trackerInfoOutputPin, trackerInfo, this);

                //showOutputImage.SetData(temporal_data);
                this.Host.SendData(thermalImageOutputPin, showOutputImage, this);

                confidence.Value = 100F;
                this.Host.SendData(trackerFittingOutputPin, confidence, this);

            }
            else
            {
                this.Host.SendData(thermalImageOutputPin, null, this);
                this.Host.SendData(stateOverlayOutputPin, null, this);
                // SendData(meshOverlayOutputPin, null);
                //SendData(templateOutputPin, null);
                this.Host.SendData(trackerInfoOutputPin, null, this);
                this.Host.SendData(trackerFittingOutputPin, null, this);
            }
            
        }

        private void computeOverlayAfterDownSampling(OverlayPolygonData downSampledData, OverlayPolygonData showedData, int downSampleRate)
        {
            showedData.x[0] = downSampleRate * downSampledData.x[0];
            showedData.y[0] = downSampleRate * downSampledData.y[0];
            showedData.x[1] = downSampleRate * downSampledData.x[1];
            showedData.y[1] = downSampleRate * downSampledData.y[1];
            showedData.x[2] = downSampleRate * downSampledData.x[2];
            showedData.y[2] = downSampleRate * downSampledData.y[2];
            showedData.x[3] = downSampleRate * downSampledData.x[3];
            showedData.y[3] = downSampleRate * downSampledData.y[3];
            showedData.x[4] = showedData.x[0];
            showedData.y[4] = showedData.y[0];
        }

        private void computeOverallState(OverlayPolygonData stateOverlay, SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum, float totalMatchScore)
        {
            OverlayPolygonData tempOverlay = new OverlayPolygonData();

            if (isSurvivorGroupNull() == false)
            {

                stateOverlay.x[0] = 0;
                stateOverlay.y[0] = 0;
                stateOverlay.x[1] = 0;
                stateOverlay.y[1] = 0;
                stateOverlay.x[2] = 0;
                stateOverlay.y[2] = 0;
                stateOverlay.x[3] = 0;
                stateOverlay.y[3] = 0;


                for (int i = 0; i < trackersColNum; i++)
                {
                    for (int j = 0; j < trackersRowNum; j++)
                    {
                        if (singleTrackerArray[i, j].isSurvivor == true)
                        {
                            //center moves
                            //int movedCenterX = (int)(stateInfo.CenterX + singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX);
                            //int movedCenterY = (int)(stateInfo.CenterY + singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY);
                            int movedCenterX, movedCenterY;
                            computeMovedCenter(singleTrackerArray[i, j], stateInfo, out movedCenterX, out movedCenterY);
                            computeOverlayFromTrackerState(tempOverlay, movedCenterX, movedCenterY, singleTrackerArray[i, j].rotation, (int)stateInfo.Height, (int)stateInfo.Width);
                            //float weight = singleTrackerArray[i, j].bestMatchScore / totalMatchScore;
                            float weight = (float)(singleTrackerArray[i, j].bestMatchScore / (singleTrackerArray[i, j].distToPoint + 1)) / totalMatchScore;
                            
                            stateOverlay.x[0] = stateOverlay.x[0] + weight * tempOverlay.x[0];
                            stateOverlay.y[0] = stateOverlay.y[0] + weight * tempOverlay.y[0];
                            stateOverlay.x[1] = stateOverlay.x[1] + weight * tempOverlay.x[1];
                            stateOverlay.y[1] = stateOverlay.y[1] + weight * tempOverlay.y[1];
                            stateOverlay.x[2] = stateOverlay.x[2] + weight * tempOverlay.x[2];
                            stateOverlay.y[2] = stateOverlay.y[2] + weight * tempOverlay.y[2];
                            stateOverlay.x[3] = stateOverlay.x[3] + weight * tempOverlay.x[3];
                            stateOverlay.y[3] = stateOverlay.y[3] + weight * tempOverlay.y[3];
                        }

                    }
                }

                stateOverlay.x[4] = stateOverlay.x[0];
                stateOverlay.y[4] = stateOverlay.y[0];
                stateInfo.CenterX = (int)((stateOverlay.x[0] + stateOverlay.x[1] + stateOverlay.x[2] + stateOverlay.x[3]) / 4 + 0.5);
                stateInfo.CenterY = (int)((stateOverlay.y[0] + stateOverlay.y[1] + stateOverlay.y[2] + stateOverlay.y[3]) / 4 + 0.5);
            }
  
        }

        private void computeMovedCenter(SingleTracker singleTracker, RectangleData stateInfo, out int movedCenterX, out int movedCenterY)
        {

            //this function can be simplified.
            int centerXBeforeRotation = (int)(singleTracker.centerX + stateInfo.Width / 2 - (2 * singleTracker.rowInd + 1) * stateInfo.Width / (2 * trackersRowNum));
            int centerYBeforeRotation = (int)(singleTracker.centerY + stateInfo.Height / 2 - (2 * singleTracker.colInd + 1) * stateInfo.Height / (2 * trackersColNum));


            //singleTrackerArray[i, j].centerX = (int)(stateInfo.CenterX - stateInfo.Width / 2 + (2 * j + 1) * stateInfo.Width / (2 * trackersRowNum));
            //singleTrackerArray[i, j].centerY = (int)(stateInfo.CenterY - stateInfo.Height / 2 + (2 * i + 1) * stateInfo.Height / (2 * trackersColNum));


            double cosine = Math.Cos(singleTracker.rotation);
            double sine = Math.Sin(singleTracker.rotation);

            movedCenterX = (int)(singleTracker.centerX + (centerXBeforeRotation - singleTracker.centerX) * cosine + (centerYBeforeRotation - singleTracker.centerY) * sine);
            movedCenterY = (int)(singleTracker.centerY + (centerXBeforeRotation - singleTracker.centerX) * -sine + (centerYBeforeRotation - singleTracker.centerY) * cosine);

        }

        public void reLocateFailureTrackers()
        {
            if (mousedone == true && trackstatus == true)
            {
                if (trackersColNum > 1 || trackersRowNum > 1)
                {
                    if (isSurvivorGroupNull() == false)
                    {
                        for (int i = 0; i < trackersColNum; i++)
                        {
                            for (int j = 0; j < trackersRowNum; j++)
                            {
                                if (singleTrackerArray[i, j].isSurvivor == false)
                                {
                                    singleTrackerArray[i, j].centerX = (int)(stateInfo.CenterX - stateInfo.Width / 2 + (2 * j + 1) * stateInfo.Width / (2 * trackersRowNum));
                                    singleTrackerArray[i, j].centerY = (int)(stateInfo.CenterY - stateInfo.Height / 2 + (2 * i + 1) * stateInfo.Height / (2 * trackersColNum));
                                    singleTrackerArray[i, j].rotation = 0;
                                    singleTrackerArray[i, j].rotationIndex = 6;
                                    singleTrackerArray[i, j].previousCenterX = singleTrackerArray[i, j].centerX;
                                    singleTrackerArray[i, j].previousCenterY = singleTrackerArray[i, j].centerY;
                                    singleTrackerArray[i, j].previousRotationIndex = 6;

                                }

                            }
                        }
                        localSearchRange = 1;
                    }
                    else
                    {
                        for (int i = 0; i < trackersColNum; i++)
                        {
                            for (int j = 0; j < trackersRowNum; j++)
                            {
                                singleTrackerArray[i, j].centerX = singleTrackerArray[i, j].initialCenterX;
                                singleTrackerArray[i, j].centerY = singleTrackerArray[i, j].initialCenterY;
                                singleTrackerArray[i, j].rotation = 0;
                                singleTrackerArray[i, j].rotationIndex = 6;
                                singleTrackerArray[i, j].previousCenterX = singleTrackerArray[i, j].initialCenterX;
                                singleTrackerArray[i, j].previousCenterY = singleTrackerArray[i, j].initialCenterY;
                                singleTrackerArray[i, j].previousRotationIndex = 6;
                            }
                        }

                        localSearchRange = 5;

                    }
                }
            }

        }

        public bool isSurvivorGroupNull()
        {
            
            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    if (singleTrackerArray[i, j].isSurvivor == true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void computeOverlayFromTrackerState(OverlayPolygonData stateOverlay, int center_x, int center_y, float rotation, int singleHeight, int singleWidth)
        {
            double sinine = Math.Sin(rotation);
            double cosine = Math.Cos(rotation);
      
            stateOverlay.x[0] = (float)(center_x + (singleHeight * sinine - singleWidth * cosine) / 2);
            stateOverlay.y[0] = (float)(center_y - (singleWidth * sinine + singleHeight * cosine) / 2);
            stateOverlay.x[1] = (float)(center_x + (singleHeight * sinine + singleWidth * cosine) / 2);
            stateOverlay.y[1] = (float)(center_y + (singleWidth * sinine - singleHeight * cosine) / 2);
            stateOverlay.x[2] = (float)(center_x + (singleWidth * cosine - singleHeight * sinine) / 2);
            stateOverlay.y[2] = (float)(center_y + (singleWidth * sinine + singleHeight * cosine) / 2);
            stateOverlay.x[3] = (float)(center_x - (singleWidth * cosine + singleHeight * sinine) / 2);
            stateOverlay.y[3] = (float)(center_y + (singleHeight * cosine - singleWidth * sinine) / 2);
            stateOverlay.x[4] = stateOverlay.x[0];
            stateOverlay.y[4] = stateOverlay.y[0];
        }

    

        private void tracking()
        {
            if (trackstatus == true && mousedone == true)
            {

                TrackingLogic trackingLogic = new TrackingLogic();
                computeReSampledImageData(reSampledImageData, normalizedImageData, imageInfo, downSampledImageInfo);
                trackingLogic.computeIndividualState(reSampledImageData, singleTrackerArray, trackersColNum, trackersRowNum, downSampledImageInfo, localSearchRange);
                CollaborativeNetworkLogic collaborativeNetworkLogic = new CollaborativeNetworkLogic();
                collaborativeNetworkLogic.computeSurvivorGroup(singleTrackerArray, trackersColNum, trackersRowNum, interestPoint);
                motionSimilarityScore = decideChangingPattern(singleTrackerArray, trackersColNum, trackersRowNum);
                motionSimilar = motionSimilarityScore;
                /*
                if (enableGlobalSearching == true)
                {
                    if (singleTrackerArray[0, 0].isSurvivor == false)
                    {
                        trackingLogic.computeGlobalIndividualState(reSampledImageData, singleTrackerArray, trackersColNum, trackersRowNum, downSampledImageInfo, globalSearchRange);
                    }
                }
                */
                if (updateTemplate == true)
                {
                    TemplateModel templateModel = new TemplateModel();
                    templateModel.UpdateTemplates(singleTrackerArray, trackersColNum, trackersRowNum);
                }
            }
        }

        private  float decideChangingPattern(SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum)
        {
            float threshold = 3;
            int countWin = 0;
            float mean = 0;
            float std = 0;
            float maxMotion = 10;
            float maxRotation = PI / 2;
            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    if (singleTrackerArray[i, j].isSurvivor == true)
                    {
                        float temp1 = singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX;
                        float temp2 = singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY;
                        mean = (float)(mean + Math.Sqrt(temp1 * temp1 + temp2 * temp2));
                        //mean = mean + Math.Sqrt((singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX) ^ 2 + (singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY) ^ 2);
                        countWin = countWin + 1;
                    }
                }
            }
            mean = mean / countWin;
            avgMotion = mean;
            motionMean = avgMotion;

            for (int m = 0; m < trackersColNum; m++)
            {
                for (int n = 0; n < trackersRowNum; n++)
                {
                    if (singleTrackerArray[m, n].isSurvivor == true)
                    {
                        float temp1 = singleTrackerArray[m, n].centerX - singleTrackerArray[m, n].previousCenterX;
                        float temp2 = singleTrackerArray[m, n].centerY - singleTrackerArray[m, n].previousCenterY;
                        float temp3 = (float)Math.Sqrt(temp1 * temp1 + temp2 * temp2);
                        std = (float)(std + Math.Sqrt((temp3 - mean) * (temp3 - mean)));
                        //std = std + ((singleTrackerArray[m, n].centerX - singleTrackerArray[m, n].previousCenterX - mean))^2;
                        //std = std + (Math.Sqrt((singleTrackerArray[m, n].centerX - singleTrackerArray[m, n].previousCenterX) ^ 2 + (singleTrackerArray[m, n].centerY - singleTrackerArray[m, n].previousCenterY) ^ 2) - mean) ^ 2;
                    }
                }
            }

            if (trackersColNum == 1 && trackersRowNum == 1)
            {
                countWin = 1;
            }

            std = (float)Math.Sqrt(std / countWin);

            return std;
        }

        private void getImageReadyToProcess(ImageInfo imageInfo, IPinData input)
        {
            ThermalData curThermalData = (ThermalData)input;
            setImageInfo(imageInfo, curThermalData);
            float[] inputThermalData = curThermalData.Data;
            normalizeImageData(imageInfo, inputThermalData);
        }

        private void normalizeImageData(ImageInfo imageInfo, float[] inputThermalData)
        {

            int dataLength = imageInfo.imageWidth * imageInfo.imageHeight;
            //float max = (float)MAX.Thermal;
            //float min = (float)MIN.Thermal;

            if (normalizedImageData == null && dataLength > 0)
            {
                normalizedImageData = new float[dataLength];
            }
            if (unNormalizedImageData == null && dataLength > 0)
            {
                unNormalizedImageData = new float[dataLength];
            }

            for (int i = 0; i < dataLength; i++)
            {
                unNormalizedImageData[i] = inputThermalData[i];

                if (inputThermalData[i] <= min)
                {
                    normalizedImageData[i] = 0.5F;
                }
                else if (inputThermalData[i] >= max)
                {
                    normalizedImageData[i] = 255;
                }
                else
                {
                    normalizedImageData[i] = (inputThermalData[i] - min) * 255 / (max - min);
                }

            }

        }


        private void setImageInfo(ImageInfo imageInfo, ThermalData input)
        {
            //imageInfo.colorbands = ((IImageDescription)input).ColorBands;
            imageInfo.colorbands = 1;
            imageInfo.stride = input.Width;
            imageInfo.imageWidth = input.Width;
            imageInfo.imageHeight = input.Height;
            imageInfo.dataLength = input.Width * input.Height;
        }

        public void ProcessMouse(IMouseEventData mouseData)
        {
            switch (selectionState)
            {
                case SelectionState.waitingForFirstPoint:

                    //get the event type
                    if (mouseData.Category == MouseEventCategory.MouseDown)
                    {
                        if (mouseData != null)
                        {
                            mousedone = false;
                            trackstatus = false;
                            //set the point from the mouse cursor position
                            if (mouseData.Data.Button == MouseButtons.Left)
                            {
                                selectionPoint1.X = mouseData.NormalizedXLocation;
                                selectionPoint1.Y = mouseData.NormalizedYLocation;
                                selectionPoint2.X = mouseData.NormalizedXLocation + 1;
                                selectionPoint2.Y = mouseData.NormalizedYLocation + 1;

                            }
 

                            //selectionPoint1.X = mouseData.Data.X;
                            //selectionPoint1.Y = mouseData.Data.Y;
                            //selectionPoint2.X = mouseData.Data.X + 1;
                            //selectionPoint2.Y = mouseData.Data.Y + 1;

                            //change the state
                            selectionState = SelectionState.waitingForSecondPoint;

                        }
                    }
                    break;

                case SelectionState.waitingForSecondPoint:
                    //check to see if the args is valid (it is null for enter and leave events)
                    if (mouseData != null && mouseData.Category == MouseEventCategory.MouseUp)
                    {
                        //get the second point from the mouse cursor position
                        if (mouseData.Data.Button == MouseButtons.Left)
                        {
                            selectionPoint2.X = mouseData.NormalizedXLocation;
                            selectionPoint2.Y = mouseData.NormalizedYLocation;
                        }
                        else if (mouseData.Data.Button == MouseButtons.Right)
                        {
                            selectionPoint1.X = (float)(mouseData.NormalizedXLocation - 0.5 * Math.Abs(backupSelectionPoint1.X - backupSelectionPoint2.X));
                            selectionPoint1.Y = (float)(mouseData.NormalizedYLocation - 0.5 * Math.Abs(backupSelectionPoint1.Y - backupSelectionPoint2.Y));
                            selectionPoint2.X = (float)(mouseData.NormalizedXLocation + 0.5 * Math.Abs(backupSelectionPoint1.X - backupSelectionPoint2.X));
                            selectionPoint2.Y = (float)(mouseData.NormalizedYLocation + 0.5 * Math.Abs(backupSelectionPoint1.Y - backupSelectionPoint2.Y));
                        }
                        //selectionPoint2.X = mouseData.Data.X;
                        //selectionPoint2.Y = mouseData.Data.Y;
                        setInitialStateOverlay(selectionPoint1, selectionPoint2);
                        formatToOTACSPolygonData(stateOverlay, OTACSstateOverlay);
                        this.Host.SendData(stateOverlayOutputPin, (IPolygonData)OTACSstateOverlay, this);
                    }

                    //check to see if the mouse has been let up ... if so then the selection is finished
                    if (mouseData.Category == MouseEventCategory.MouseUp)
                    {
                        selectionState = SelectionState.waitingForFirstPoint;
                        mousedone = true;

                        resetTrackers(stateOverlay);

                    }
                    break;
                default:
                    trackstatus = false;
                    break;
            }

        }

        private void formatToOTACSPolygonData(OverlayPolygonData stateOverlay, PolygonData OTACSstateOverlay)
        {
            
            if (OTACSstateOverlay != null)
            {
                try
                {
                    OTACSstateOverlay.clearAll();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

             
            }

            //if (stateOverlay != null)
            //{
            //    try
            //    {
            //        PointF coord0 = new PointF(stateOverlay.x[0], stateOverlay.y[0]);
            //        PointF coord1 = new PointF(stateOverlay.x[1], stateOverlay.y[1]);
            //        PointF coord2 = new PointF(stateOverlay.x[2], stateOverlay.y[2]);
            //        PointF coord3 = new PointF(stateOverlay.x[3], stateOverlay.y[3]);

            //        OTACSstateOverlay.addVertice(coord0);
            //        OTACSstateOverlay.addVertice(coord1);
            //        OTACSstateOverlay.addVertice(coord2);
            //        OTACSstateOverlay.addVertice(coord3);
            //        OTACSstateOverlay.Thick = 3;
            //        OTACSstateOverlay.BorderColor = Color.Red;

            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("{0} Exception caught.", e);
            //    }


            //}

            PointF coord0 = new PointF(stateOverlay.x[0], stateOverlay.y[0]);
            PointF coord1 = new PointF(stateOverlay.x[1], stateOverlay.y[1]);
            PointF coord2 = new PointF(stateOverlay.x[2], stateOverlay.y[2]);
            PointF coord3 = new PointF(stateOverlay.x[3], stateOverlay.y[3]);

            OTACSstateOverlay.addVertice(coord0);
            OTACSstateOverlay.addVertice(coord1);
            OTACSstateOverlay.addVertice(coord2);
            OTACSstateOverlay.addVertice(coord3);
            OTACSstateOverlay.Thick = 3;
            OTACSstateOverlay.BorderColor = Color.Red;



        }

        public void resetTrackers(OverlayPolygonData stateOverlay)
        {
            if (recordData == true)
            {
                /*
                using (StreamWriter sr = File.AppendText(path1))
                {
                    sr.WriteLine("{0}, {1}", ((int)showCenterOverlay.x[0]).ToString(), ((int)showCenterOverlay.y[0]).ToString(), path1);
                    
                }
                 */

                //using (StreamWriter sr = File.AppendText(path2))
                //{
                //    sr.WriteLine("{0}, {1}, {2}, {3}", stateOverlay.x[0].ToString(), stateOverlay.y[0].ToString(), stateOverlay.x[2].ToString(), stateOverlay.y[2].ToString(), path2);
                //}

            }
            
            stateInfo.Width = (int)(stateOverlay.x[1] - stateOverlay.x[0]);
            stateInfo.Height = (int)(stateOverlay.y[3] - stateOverlay.y[0]);
            //this is for experiments
            //stateInfo.Width = 30;
            //stateInfo.Height = 20;
            //
            stateInfo.CenterX = (stateOverlay.x[0] + stateOverlay.x[1]) / 2;
            stateInfo.CenterY = (stateOverlay.y[1] + stateOverlay.y[2]) / 2;
            //this is for experiments
            //stateInfo.CenterX = 127;
            //stateInfo.CenterY = 109;
            //
            stateInfo.Rotation = 0F;
            trackerInitialState = new TrackerInitialState((int)stateInfo.Width, (int)stateInfo.Height, (int)stateInfo.CenterX, (int)stateInfo.CenterY); 

            this.Host.SendData(trackerInfoOutputPin, stateInfo, this);

            confidence.Value = 100;
            this.Host.SendData(trackerFittingOutputPin, confidence, this);

            computeOptimalTrackersNum(stateInfo, out trackersColNum, out trackersRowNum, out downSampleRate);
            initializeReSampledImageData(stateInfo, downSampleRate, normalizedImageData);
            SetIndividualTracker(trackersColNum, trackersRowNum, stateInfo);

            trackstatus = true;

            
           
            //SendData(stateOverlayOutputPin, stateOverlay);


             
        }

        public void initializeReSampledImageData(RectangleData stateInfo, int downSampleRate, float[] normalizedImageData)
        {


            //this following line may be moved to other places.
            //showOutputImage = new ThermalImageData((int)stateInfo.Width, (int)stateInfo.Height, imageInfo.stride, imageInfo.colorbands);
           

            stateInfo.CenterX = stateInfo.CenterX / downSampleRate;
            stateInfo.CenterY = stateInfo.CenterY / downSampleRate;
            stateInfo.Height = stateInfo.Height / downSampleRate;
            stateInfo.Width = stateInfo.Width / downSampleRate;


            downSampledImageInfo = new ImageInfo();
            downSampledImageInfo.imageHeight = imageInfo.imageHeight / downSampleRate;
            downSampledImageInfo.imageWidth = imageInfo.imageWidth / downSampleRate;
            downSampledImageInfo.colorbands = imageInfo.colorbands;
            downSampledImageInfo.stride = imageInfo.stride / downSampleRate;
            downSampledImageInfo.dataLength = downSampledImageInfo.imageHeight * downSampledImageInfo.stride;

            reSampledImageData = new float[downSampledImageInfo.dataLength];
            computeReSampledImageData(reSampledImageData, normalizedImageData, imageInfo, downSampledImageInfo);


            
        }

        private void computeReSampledImageData(float[] reSampledImageData, float[] normalizedImageData, ImageInfo imageInfo, ImageInfo downSampledImageInfo)
        {
            for (int i = 0; i < imageInfo.imageHeight; i = i + downSampleRate)
            {
                for (int j = 0; j < imageInfo.imageWidth; j = j + downSampleRate)
                {
                    int tempAddr = j / downSampleRate * downSampledImageInfo.colorbands + i / downSampleRate * downSampledImageInfo.stride;
                    if (tempAddr < downSampledImageInfo.dataLength)
                    {
                        reSampledImageData[tempAddr] = normalizedImageData[j * imageInfo.colorbands + i * imageInfo.stride];
                    }
                }
            }
        }


        public void SetIndividualTracker(int trackersColNum, int trackersRowNum, RectangleData stateInfo)
        {
            singleTrackerArray = new SingleTracker[trackersColNum, trackersRowNum];
            indiTrackerStateOverlay = new OverlayPolygonData[trackersColNum, trackersRowNum];
            showIndiTrackerStateOverlay = new OverlayPolygonData[trackersColNum, trackersRowNum];
            meshOverlay = new OverlayPolygonData[trackersColNum, trackersRowNum];
            showMeshOverlay = new OverlayPolygonData[trackersColNum, trackersRowNum];

            int count = 11;
            int singleTrackerHeight = (int)stateInfo.Height / trackersColNum;
            if (singleTrackerHeight % 2 == 1)
            {
                singleTrackerHeight = singleTrackerHeight + 1;
            }
            int singleTrackerWidth = (int)stateInfo.Width / trackersRowNum;
            if (singleTrackerWidth % 2 == 1)
            {
                singleTrackerWidth = singleTrackerWidth + 1;
            }


            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {                  
                    singleTrackerArray[i, j] = new SingleTracker(singleTrackerHeight, singleTrackerWidth);
                    indiTrackerStateOverlay[i, j] = new OverlayPolygonData();
                    showIndiTrackerStateOverlay[i, j] = new OverlayPolygonData();
                    meshOverlay[i, j] = new OverlayPolygonData();
                    showMeshOverlay[i, j] = new OverlayPolygonData();
                    singleTrackerArray[i, j].colInd = i;
                    singleTrackerArray[i, j].rowInd = j;
                    singleTrackerArray[i, j].rotation = 0;
                    singleTrackerArray[i, j].rotationIndex = 6;

                    singleTrackerArray[i, j].centerX = (int)(stateInfo.CenterX - stateInfo.Width / 2 + (2 * j + 1) * stateInfo.Width / (2 * trackersRowNum));
                    singleTrackerArray[i, j].centerY = (int)(stateInfo.CenterY - stateInfo.Height / 2 + (2 * i + 1) * stateInfo.Height / (2 * trackersColNum));
                    singleTrackerArray[i, j].initialCenterX = singleTrackerArray[i, j].centerX;
                    singleTrackerArray[i, j].initialCenterY = singleTrackerArray[i, j].centerY;
                    singleTrackerArray[i, j].initializeTemplates(reSampledImageData, singleTrackerArray[i, j].originalTemplate, singleTrackerArray[i, j].currentTemplate, downSampledImageInfo);
                }
            }
       

        }

        private void computeOptimalTrackersNum(RectangleData stateInfo, out int trackersColNum, out int trackersRowNum, out int downSampleRate)
        {

            trackersColNum = (int)stateInfo.Height / 32;
            trackersRowNum = (int)stateInfo.Width / 32;
            downSampleRate = 2;

            if (trackersColNum < 1)
            {
                trackersColNum = 1;
            }
            if (trackersRowNum < 1)
            {
                trackersRowNum = 1;
            }

            if (trackersColNum == 1 && trackersRowNum == 1)
            {
                downSampleRate = 1;
                
            }

            if (setTrackersColNum != 0 && setTrackersRowNum != 0)
            {
                //downSampleRate = 1;
                trackersColNum = setTrackersColNum;
                trackersRowNum = setTrackersRowNum;
            }


   
        }

        private void setInitialStateOverlay(PointF selectionPoint1, PointF selectionPoint2)
        {
            backupSelectionPoint1.X = selectionPoint1.X;
            backupSelectionPoint1.Y = selectionPoint1.Y;
            backupSelectionPoint2.X = selectionPoint2.X;
            backupSelectionPoint2.Y = selectionPoint2.Y;

            float tempx0, tempy0, tempx2, tempy2;

            if (selectionPoint1.X > selectionPoint2.X && selectionPoint1.Y > selectionPoint2.Y)
            {
                tempx0 = selectionPoint2.X;
                tempy0 = selectionPoint2.Y;
                tempx2 = selectionPoint1.X;
                tempy2 = selectionPoint1.Y;
            }
            else if (selectionPoint1.X < selectionPoint2.X && selectionPoint1.Y < selectionPoint2.Y)
            {
                tempx0 = selectionPoint1.X;
                tempy0 = selectionPoint1.Y;
                tempx2 = selectionPoint2.X;
                tempy2 = selectionPoint2.Y;
            }
            else if (selectionPoint1.X < selectionPoint2.X && selectionPoint1.Y > selectionPoint2.Y)
            {
                tempx0 = selectionPoint1.X;
                tempy0 = selectionPoint2.Y;
                tempx2 = selectionPoint2.X;
                tempy2 = selectionPoint1.Y;
            }
            else if (selectionPoint1.X > selectionPoint2.X && selectionPoint1.Y < selectionPoint2.Y)
            {
                tempx0 = selectionPoint2.X;
                tempy0 = selectionPoint1.Y;
                tempx2 = selectionPoint1.X;
                tempy2 = selectionPoint2.Y;
            }
            else
            {
                tempx0 = 1;
                tempy0 = 1;
                tempx2 = 2;
                tempy2 = 2;
                trackstatus = false;
            }

            stateOverlay.x[0] = tempx0;
            stateOverlay.y[0] = tempy0;

            stateOverlay.x[1] = tempx2;
            stateOverlay.y[1] = tempy0;

            stateOverlay.x[2] = tempx2;
            stateOverlay.y[2] = tempy2;

            stateOverlay.x[3] = tempx0;
            stateOverlay.y[3] = tempy2;

            stateOverlay.x[4] = tempx0;
            stateOverlay.y[4] = tempy0;
        }
        
        
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public MainProgram()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public MainProgram(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
          
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


    }
}
