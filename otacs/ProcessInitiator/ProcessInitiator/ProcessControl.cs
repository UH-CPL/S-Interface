using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace ProcessInitiator
{
    enum ProcessState
    {stopProcess = 0, startProcess = 1, pauseProcess = 2};
    public partial class ProcessControl : UserControl
    {
        //Members
        ProcessInitiator plugin;
        ToolTip toolTip;
        ProcessState buttonState;
        public ProcessControl(ProcessInitiator setPlugin)
        {
            InitializeComponent();
            toolTip = new ToolTip();
            toolTip.SetToolTip(buttonStartStopInitiator, "Press this button to start/stop the process");
            buttonState = ProcessState.stopProcess;

            //Set the plugin 
            plugin = setPlugin;
            plugin.ProcessingStateChanged += new IntDelegate(OnProcessingStateChanged);
            plugin.ActivityChanged += new BoolDelegate(OnActivityChanged);
        }

        void OnActivityChanged(bool value)
        {

            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(OnActivityChanged), value);
                return;
            }

            buttonStartStopInitiator.Enabled = value;
        }

        void OnProcessingStateChanged(int value)
        {

            if (InvokeRequired)
            {
                Invoke(new IntDelegate(OnProcessingStateChanged), value);
                return;
            }
            //===========================
            //Update the button label 
            ////if (buttonState == ProcessState.stopProcess)
            ////{ buttonstartInitiator.Text = "Start Processing"; }
            ////else
            ////{ buttonstartInitiator.Text = "Stop Processing"; }
            //============================
        }
        private void buttonStartStopInitiator_Click(object sender, EventArgs e)
        {
            //if (buttonState == ProcessState.stopProcess)
            //{
            //    buttonState = ProcessState.startProcess;
            //    plugin.ProcessingState = true;
            //}
            //else
            //{
            //    buttonState = ProcessState.stopProcess;
            //    plugin.ProcessingState = false;
            //}
            if (plugin.isPause == false)
            {
                plugin.isPause = true;
                buttonStartStopInitiator.Image = Image.FromStream(GetEmbeddedResource("pause.png"));
                plugin.processingState = 1;//0=stop;1=Play;2=Pause
                plugin.UpdateOutputPin();
            }
            else
            {
                plugin.isPause = false;
                buttonStartStopInitiator.Image = Image.FromStream(GetEmbeddedResource("play.png")); //Image.FromFile("play.png");
                plugin.processingState = 2;//0=stop;1=Play;2=Pause
                plugin.UpdateOutputPin();
            }
            buttonstopInitiator.Enabled = true;
            buttonStartStopInitiator.Enabled = false;


        }

        Stream GetEmbeddedResource(string fileName)
        {
            //get the embedded resources 
            string[] embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            //check to see if any of the resources contain the specified file name
            for (int r = 0; r < embeddedResources.Length; r++)
                if (embeddedResources[r].Contains(fileName))
                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResources[r]);

            //didn't find the file
            return null;

        }

        //private void buttonpauseInitiator_Click(object sender, EventArgs e)
        //{
        //    plugin.processingState = 2;//0=stop;1=Play;2=Pause
        //    plugin.UpdateOutputPin();
        //}

        private void buttonstopInitiator_Click(object sender, EventArgs e)
        {
            plugin.isPause = false;
            buttonStartStopInitiator.Enabled = true;
            buttonStartStopInitiator.Image = Image.FromStream(GetEmbeddedResource("play.png")); //Image.FromFile("play.png");//Whenever user click Stop draw play icon
            plugin.processingState = 0; //0=stop;1=Play;2=Pause
            plugin.UpdateOutputPin();
            buttonstopInitiator.Enabled = false;
        }
       
        
    }
}
