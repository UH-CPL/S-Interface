using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Drawing;


namespace VideoCameraPlugin
{
    [Serializable]
    public delegate void BoolDelegate(bool value);
    public delegate void StringDelegate(string value);
    
    [Serializable]
    public class VideoCamera : IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        

        //Declarations of all our internal plugin variables
        string myName = "Video Camera";
        string myDescription = "Records from any connected video camera input";
        string myAuthor = "Malcolm Dcosta";
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

        string deviceName;
        public string DeviceName
        {
            get
            {
                return deviceName;
            }
            set
            {
                deviceName = value;
            }
        }

        string aviNum;
        public string AviNum
        {
            get
            {
                return aviNum;
            }
            set
            {
                aviNum = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region VideoCamera Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        IPin processInitiator = null;
        IPin filenamepin = null;
        IPin UploadAVIOutputPin;
        bool processingState = false;
        string fileName = null;


        public event BoolDelegate ProcessingStateChanged;
        public event StringDelegate FileNameChanged;

        public bool ProcessingState
        {
            get { return processingState; }
            set
            {
                processingState = value;
                if (ProcessingStateChanged != null)
                { ProcessingStateChanged(processingState); }

            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                if (FileNameChanged != null)
                { FileNameChanged(fileName); }

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

            processInitiator = Host.LoadOrCreatePin("Process Initiator", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(processInitiator);
            filenamepin = Host.LoadOrCreatePin("Filename", PinCategory.Optional, new Type[] { typeof(IStringData) });
            inPins.Add(filenamepin);

            UploadAVIOutputPin = Host.LoadOrCreatePin("Upload AVI", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(UploadAVIOutputPin);

            myMainInterface = new VideoCameraControl(this);

        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            if (pin == processInitiator)
            {
                int state = ((IntegerData)input).Value;
                if (state == 0)
                {
                    ProcessingState = false;
                    FileName = null;
                }
                else
                {
                    ProcessingState = true;
                }
            }

            if (pin == filenamepin)
            {
                string filename = ((IStringData)input).Data;
                FileName = filename;
            }


        }

        public void NotifyOutputPin()
        {
            this.Host.SendData(UploadAVIOutputPin, new IntegerData(1), this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public VideoCamera()
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public VideoCamera(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                DeviceName = (string)info.GetString("deviceName");
            }
            catch
            {
                DeviceName = null;
            }

            try
            {
                AviNum = (string)info.GetString("aviNum");
            }
            catch
            {
                AviNum = null;
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            info.AddValue("deviceName", DeviceName);
            info.AddValue("aviNum", AviNum);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

    }
}
