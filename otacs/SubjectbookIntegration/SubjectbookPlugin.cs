using System;
using PluginInterface;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections;

namespace SubjectbookIntegration
{
    [Serializable]
    public class SubjectbookPlugin : IPlugin, ISerializable
    {
        //Declarations of all our internal plugin variables
        string myName = "Subjectbook";
        string myDescription = "Subjectbook Plugin";
        string myAuthor = "Pradeep Buddharaju";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        int myID = -1;

        bool serialized = false;

        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }


        IPin UploadPPInputPin;
        IPin UploadThermalAVIInputPin;
        IPin UploadVisualAVIInputPin;
        IPin UploadVisualAVI2InputPin;
        IPin UploadAudioInputPin;
        IPin filenameInputPin;

        IPin FileNameOutputPin;
        IPin BiofeedbackOutputPin;
        IPin ActivityOutputPin;

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


        public event VoidDelegate UploadPerspirationFile;
        public event VoidDelegate UploadPerspirationAVI;
        public event VoidDelegate UploadVisualAVI;
        public event VoidDelegate UploadVisualAVI2;
        public event VoidDelegate UploadAudio;
        public event VoidDelegate MessageCreatingExcelSheet;

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first

            UploadPPInputPin = Host.LoadOrCreatePin("Upload pp", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(UploadPPInputPin);

            UploadThermalAVIInputPin = Host.LoadOrCreatePin("Upload AVI", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(UploadThermalAVIInputPin);

            UploadVisualAVIInputPin = Host.LoadOrCreatePin("Upload Visual", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(UploadVisualAVIInputPin);

            UploadVisualAVI2InputPin = Host.LoadOrCreatePin("Upload Visual 2", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(UploadVisualAVI2InputPin);

            filenameInputPin = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            inPins.Add(filenameInputPin);

            UploadAudioInputPin = Host.LoadOrCreatePin("Upload Audio", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            inPins.Add(UploadAudioInputPin);

            FileNameOutputPin = Host.LoadOrCreatePin("File Name", PinCategory.Optional, new Type[] { typeof(IStringData) });
            outPins.Add(FileNameOutputPin);

            BiofeedbackOutputPin = Host.LoadOrCreatePin("Biofeedback", PinCategory.Optional, new Type[] { typeof(IBoolData) });
            outPins.Add(BiofeedbackOutputPin);

            ActivityOutputPin = Host.LoadOrCreatePin("Active", PinCategory.Optional, new Type[] { typeof(IBoolData) });
            outPins.Add(ActivityOutputPin);


            myMainInterface = new SubjectbookControl(this);

            /*
            if (serialized && ConnectedPort != -1)
            {
                ConnectedPortChanged(ConnectedPort);
            }
            */
            
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            if (pin == filenameInputPin)
            {
                if (input != null)
                {
                    FileName = ((IStringData)input).Data;
                }

            }

            if (pin == UploadPPInputPin)
            {
                if (input != null)
                {
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        UploadPerspirationFile();
                    }
                    else if (trigger == 0)
                    {
                        MessageCreatingExcelSheet();
                    }
                }

            }

            if (pin == UploadThermalAVIInputPin)
            {
                if (input != null)
                {
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        UploadPerspirationAVI();
                    }
                    
                }

            }

            if (pin == UploadVisualAVIInputPin)
            {
                if (input != null)
                {
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        UploadVisualAVI();
                    }

                }

            }

            if (pin == UploadVisualAVI2InputPin)
            {
                if (input != null)
                {
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        UploadVisualAVI2();
                    }

                }

            }

            if (pin == UploadAudioInputPin)
            {
                if (input != null)
                {
                    int trigger = ((IIntegerData)input).Value;
                    if (trigger == 1)
                    {
                        UploadAudio();
                    }

                }

            }

        }

        public void SendFileName(string fileName)
        {
            this.Host.SendData(FileNameOutputPin, new StringData(fileName), this);
        }

        public void SendBiofeedback(bool isBiofeedback)
        {
            BoolData isBiofeedbackData = new BoolData();
            isBiofeedbackData.Data = isBiofeedback;

            this.Host.SendData(BiofeedbackOutputPin, isBiofeedbackData, this);
        }

        public void SendActivity(bool isActive)
        {
            BoolData isActiveData = new BoolData();
            isActiveData.Data = isActive;
            this.Host.SendData(ActivityOutputPin, isActiveData, this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SubjectbookPlugin(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                //ConnectedPort = (int)info.GetInt64("port");
                serialized = true;
            }
            catch
            {
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SubjectbookPlugin()
        ///////////////////////////////////////////////////////////////////////////////////////////
        {

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //info.AddValue("port", ConnectedPort);


        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}
