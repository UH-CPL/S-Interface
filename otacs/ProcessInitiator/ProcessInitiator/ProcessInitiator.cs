using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace ProcessInitiator
{
    [Serializable]
    public class ProcessInitiator:IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "Process Initiator";
        string myDescription = "To initiate a process";
        string myAuthor = "Avinash Wesley";
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
        #region ProcessInitiator Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //IPin inImage = null;
        IPin StartStopOutputPin = null;
        IPin ActivityInputPin;

        //protected BoolData processingState;
        // protected IntegerData processingState;
        public int processingState;
        public bool isPause = false; // buttons show play icon and stop icon
  

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Events
        ///////////////////////////////////////////////////////////////////////////////////////////
      //  public event BoolDelegate ProcessingStateChanged;
        public event IntDelegate ProcessingStateChanged;
        public event BoolDelegate ActivityChanged;

        ///////////////////////////////////////////////////////////////////////////////////////////
        //Properties
        ///////////////////////////////////////////////////////////////////////////////////////////
        //public int ProcessingState
        //{
        //    get { return processingState.Value; }
        //    set
        //    {
        //        if (processingState == null)
        //        { processingState = new IntegerData(); }
        //         processingState.Value = value;
        //        if (ProcessingStateChanged != null)
        //        { ProcessingStateChanged(processingState.Value); }

        //    }
        //}



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

            ActivityInputPin = Host.LoadOrCreatePin("Active", PinCategory.Optional, new Type[] { typeof(BoolData) });
            inPins.Add(ActivityInputPin);

            StartStopOutputPin = Host.LoadOrCreatePin("Process Initiator", PinCategory.Optional, new Type[] { typeof(IIntegerData) });
            outPins.Add(StartStopOutputPin);
            processingState = new int();
            processingState = 0; //0=stop;1=Play;2=Pause
            //processingState.Value =0;  

            myMainInterface = new ProcessControl(this);
            //ProcessingState = 0; //0=stop;1=Play;2=Pause
            //++++++++++++++++++
            if (ProcessingStateChanged != null)
                        { ProcessingStateChanged(processingState); }
            //++++++++++++++++++
            UpdateOutputPin();

        }
        public void UpdateOutputPin()
        {
          //  if (StartStopOutputPin.Connected == true)
              //  this.Host.SendData(StartStopOutputPin,(BoolData)processingState, this);
            this.Host.SendData(StartStopOutputPin, new IntegerData(processingState), this);
             //   SendData(StartStopOutputPin, processingState);
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            //Put process code here

            ////this.Host.SendData(StartStopOutputPin,processingState, this);


            //   this.Host.SignalCriticalProcessingIsFinished(inImage, this);
            if (pin == ActivityInputPin)
            {
                if (input != null)
                {
                    bool isActive = ((IBoolData)input).Data;
                    ActivityChanged(isActive);
                }

            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public ProcessInitiator()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public ProcessInitiator(SerializationInfo info, StreamingContext context)
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
