using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;

namespace FocusDevice
{
    [Serializable]
    public class FocusDevicePlugin : IPlugin, ISerializable
    {

        //Declarations of all our internal plugin variables
        string myName = "Focus Device";
        string myDescription = "Focus Device Plugin";
        string myAuthor = "Ashik Khatri";
        string myVersion = "1.1.0";
        IPluginHost myHost = null;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        int myID = -1;
        int connectedPort = -1;
        bool serialized = false;
        bool tryingToAutoFocus = false;
        IPin KeyInputPin;
        IPin ValueInputPin;
        
        IPin ValueOutPutPin;

        protected IPin thermalImageInputPin;
        //Output pins 
        public IPin SharpnessPin;
        //Data array 
        float tempdiff = 0;
        float temp_diff = 0;
        float tempdiff1 = 0;
        float tempdiff2 = 0;
        float tempdiff3 = 0;
        float tempdiff4 = 0;
        float tempdiff5 = 0;
        float tempdiff6 = 0;
        float tempdiff7 = 0;
        float tempdiff8 = 0;

        Dictionary<int, double> dictionary = new Dictionary<int, double>()
        {
            { 0, -1 },
            { 20, -1 },
            { 40, -1 },
            { 60, -1 },
            { 80, -1 },
            { 100, -1 }
        };

        FloatData meanTemp;

        FocusCommunication focusComm = new FocusCommunication();
        public FocusCommunication FocusComm
        {
            get { return focusComm; }
        }

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


        public int ConnectedPort
        {
            get { return connectedPort; }
            set { connectedPort = value; }
        }

        public bool IsAutoFocusing;

        public bool IsChangingFocus;

        public event IntegerDelegate ConnectedPortChanged;

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first
           
            KeyInputPin = Host.LoadOrCreatePin("Key Input", PinCategory.Optional, new Type[] { typeof(IKeyEventData) });
            inPins.Add(KeyInputPin);
            ValueInputPin = Host.LoadOrCreatePin("Value", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            inPins.Add(ValueInputPin);
            thermalImageInputPin = Host.LoadOrCreatePin("Thermal", PinCategory.Optional, new Type[] { typeof(IThermalData) });
            inPins.Add(thermalImageInputPin);
            IsAutoFocusing = false;
            IsChangingFocus = false;
            ValueOutPutPin = Host.LoadOrCreatePin("Value", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(ValueOutPutPin);
            this.FocusComm.PositionUpdated += new FloatDelegate(OnFocusPositionUpdated);
            meanTemp = new FloatData();
            myMainInterface = new FocusDeviceControl(this);

            if (serialized && ConnectedPort!=-1)
            {
                ConnectedPortChanged(ConnectedPort);
            }
            
         }

        private void OnFocusPositionUpdated(float value)
        {
            IsChangingFocus = false;
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            if (IsAutoFocusing)
            {
                if (pin == thermalImageInputPin) //Make sure the data is for the thermalImageInputPin
                {
                    if (input != null)
                    {
                        if(tryingToAutoFocus == false)
                        {
                            var tempDic = new Dictionary<int, double>();
                            tryingToAutoFocus = true;
                            foreach (var item in dictionary)
                            {
                                IsChangingFocus = true;
                                this.FocusComm.GotoPosition(item.Key);
                                //Stopwatch s = new Stopwatch();
                                //s.Start(); 
                                //while (s.Elapsed < TimeSpan.FromSeconds(2))
                                //{
                                //    Console.WriteLine(IsChangingFocus);
                                //}
                                //s.Stop();
                                while (IsChangingFocus == true)
                                {
                                    if (IsChangingFocus == false)
                                    {
                                        break;
                                    }
                                }
                                var value = ComputeMeasurement((IThermalData)input, ((IThermalData)input).Data);
                                tempDic.Add(item.Key, value);
                                Console.WriteLine(item.Key + ", " + value);
                            }
                            var max = tempDic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                            Console.WriteLine(max);
                            this.focusComm.GotoPosition(max);
                            IsAutoFocusing = false;
                            tryingToAutoFocus = false;
                        }
                        
                        //var myList = dictionary.ToList();
                        //myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));


                        // ComputeMeasurement((IThermalData)input, ((IThermalData)input).Data);
                    }
                    else
                    {
                        // Console.WriteLine("nothing to compute");
                        // this.myHost.SendData(SharpnessPin, null, this);
                    }

                }
            }
            
            // Host.SignalCriticalProcessingIsFinished(thermalImageInputPin, this);

            if (pin == KeyInputPin)
            {
                KeyEventData keyData = (KeyEventData)input;
                
                if (keyData.Data.KeyData == Keys.F1)
                {
                    this.FocusComm.FocusOut();
                }
                if (keyData.Data.KeyData == Keys.F2)
                {
                    this.FocusComm.FocusIn();
                }
                //Put process code here
                //this.Host.SendData(outPin2, null, this);

            }
             
        }

        public double ComputeMeasurement(IThermalData imageDescription, float[] thermalData)
        {

            int TROIHeight = imageDescription.Height;
            int TROIWidth = imageDescription.Width;
            //Console.WriteLine(TROIHeight + ", " + TROIWidth);
            //Console.WriteLine(thermalData.Length);

            for (int y = 1; y < TROIHeight - 1; y++)
            {
                for (int x = 1; x < TROIWidth - 1; x++)
                {
                    //Compute mean temperature
                    int thermalAddress1 = (x - 1) + (y - 1) * imageDescription.Width;
                    int thermalAddress2 = (x) + (y - 1) * imageDescription.Width;
                    int thermalAddress3 = (x + 1) + (y - 1) * imageDescription.Width;
                    int thermalAddress4 = (x - 1) + (y) * imageDescription.Width;
                    int thermalAddress5 = (x) + (y) * imageDescription.Width;
                    int thermalAddress6 = (x - 1) + (y + 1) * imageDescription.Width;
                    int thermalAddress7 = (x) + (y + 1) * imageDescription.Width;
                    int thermalAddress8 = (x + 1) + (y + 1) * imageDescription.Width;

                    if (thermalAddress8 < thermalData.Length)
                    {
                        tempdiff1 = thermalData[thermalAddress1] - thermalData[thermalAddress5];
                        if (tempdiff1 < 0)
                        {
                            tempdiff1 = tempdiff1 * (-1);
                        }
                        tempdiff2 = thermalData[thermalAddress2] - thermalData[thermalAddress5];
                        if (tempdiff2 < 0)
                        {
                            tempdiff2 = tempdiff2 * (-1);
                        }
                        tempdiff3 = thermalData[thermalAddress3] - thermalData[thermalAddress5];
                        if (tempdiff3 < 0)
                        {
                            tempdiff3 = tempdiff3 * (-1);
                        }
                        tempdiff4 = thermalData[thermalAddress4] - thermalData[thermalAddress5];
                        if (tempdiff4 < 0)
                        {
                            tempdiff4 = tempdiff4 * (-1);
                        }
                        tempdiff6 = thermalData[thermalAddress6] - thermalData[thermalAddress5];
                        if (tempdiff6 < 0)
                        {
                            tempdiff6 = tempdiff6 * (-1);
                        }
                        tempdiff7 = thermalData[thermalAddress7] - thermalData[thermalAddress5];
                        if (tempdiff7 < 0)
                        {
                            tempdiff7 = tempdiff7 * (-1);
                        }
                        tempdiff8 = thermalData[thermalAddress8] - thermalData[thermalAddress5];
                        if (tempdiff8 < 0)
                        {
                            tempdiff8 = tempdiff8 * (-1);
                        }

                    }
                    //              tempdiff = tempdiff1 + tempdiff2 + tempdiff3 + tempdiff4 + tempdiff5 + tempdiff6 + tempdiff7 + tempdiff8; 
                    temp_diff = 0;
                    temp_diff = Math.Max(tempdiff1, tempdiff2);
                    temp_diff = Math.Max(temp_diff, tempdiff3);
                    temp_diff = Math.Max(temp_diff, tempdiff4);
                    temp_diff = Math.Max(temp_diff, tempdiff5);
                    temp_diff = Math.Max(temp_diff, tempdiff6);
                    temp_diff = Math.Max(temp_diff, tempdiff7);
                    temp_diff = Math.Max(temp_diff, tempdiff8);

                    tempdiff = tempdiff + temp_diff;
                }
            }
            tempdiff = tempdiff / (TROIHeight * TROIWidth);
            meanTemp.Value = tempdiff;
            //Console.WriteLine(meanTemp.Value);

            // this.myHost.SendData(SharpnessPin, meanTemp, this);
            return meanTemp.Value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public FocusDevicePlugin(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                ConnectedPort = (int)info.GetInt64("port");
                serialized = true;
            }
            catch
            {
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public FocusDevicePlugin()
        ///////////////////////////////////////////////////////////////////////////////////////////
        {

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            info.AddValue("port", ConnectedPort);
            

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}
