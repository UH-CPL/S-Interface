using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Collections;
using System.Drawing;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Average
{
    [Serializable]
    public class Average : IPlugin, ISerializable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////



        //Declarations of all our internal plugin variables
        string myName = "Average";
        string myDescription = "Calculates the Mean temperature of the ROI";
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

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Average Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        IPin inImage = null;
        IPin avgTemp = null;




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

            inImage = Host.LoadOrCreatePin("ROI", PinCategory.Critical, new Type[] { typeof(IThermalData) });
            inPins.Add(inImage);

            avgTemp = Host.LoadOrCreatePin("Avg Temp", PinCategory.Optional, new Type[] { typeof(IFloatData) });
            outPins.Add(avgTemp);


            myMainInterface = new AverageControl();



        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            //Put process code here

            if (pin == inImage)
            {

                if (input != null)
                {

                    int Height = ((ThermalData)input).Height;
                    int Width = ((ThermalData)input).Width;
                    float avgfloat = ((ThermalData)input).Data.Average();

                    IFloatData avg = new FloatData(avgfloat);
                    this.Host.SendData(avgTemp, avg, this);

                }
                else
                {
                    this.Host.SendData(avgTemp, null, this);
                }


                

            }

            this.Host.SignalCriticalProcessingIsFinished(inImage, this);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public Average()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Average(SerializationInfo info, StreamingContext context)
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
