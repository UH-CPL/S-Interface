using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace deformTracker
{
    public partial class TrackerControl : UserControl
    {
        MainProgram mainProgram;

        public TrackerControl(MainProgram setMainProgram)
        {

            InitializeComponent();
            mainProgram = setMainProgram;

            mainProgram.motionSimilarityChanged += new FloatDelegate(OnPluginMotionSimilarityChanged);
            mainProgram.motionMeanChanged += new FloatDelegate(OnPluginMotionMeanChanged);
            //motionScore.Text = mainProgram.motionSimilarityScore.ToString();
            //avgMatchScore.Text = mainProgram.avgMatchingScore.ToString();

        }

        void OnPluginMotionSimilarityChanged(float motionSimilarityScore)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new FloatDelegate(OnPluginMotionSimilarityChanged), motionSimilarityScore);
                return;
            }

            //set the text value
            //TrackerPositionText.Text = TrackCenter_x.ToString();
            motionScore.Text = motionSimilarityScore.ToString();
            if (motionSimilarityScore > 1.0)
            {
                Deformation.BackColor = Color.Crimson;
            }
            else
            {
                Deformation.BackColor = Color.GreenYellow;
            }
        }

        void OnPluginMotionMeanChanged(float motionMean)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new FloatDelegate(OnPluginMotionMeanChanged), motionMean);
                return;
            }

            //set the text value
            //TrackerPositionText.Text = TrackCenter_x.ToString();
            avgMotion.Text = motionMean.ToString();
            if (motionMean > 1.0)
            {
                Inaccuracy.BackColor = Color.Crimson;
            }
            else
            {
                Inaccuracy.BackColor = Color.GreenYellow;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (mainProgram.artificialIndex < 12)
            {
                mainProgram.artificialIndex = mainProgram.artificialIndex + 1;
            }  
        }

        private void minusButton_Click(object sender, EventArgs e)
        {
            if (mainProgram.artificialIndex > 0)
            {
                mainProgram.artificialIndex = mainProgram.artificialIndex - 1;
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            if (motionScore.Text != null && avgMotion.Text != null)
            {
                int tempx = Int32.Parse(motionScore.Text);
                int tempy = Int32.Parse(avgMotion.Text);
             

                mainProgram.interestPoint.x = tempx;
                mainProgram.interestPoint.y = tempy;
                mainProgram.interestPoint.distX = (int)(tempx - mainProgram.stateInfo.CenterX * mainProgram.downSampleRate);
                mainProgram.interestPoint.distY = (int)(tempy - mainProgram.stateInfo.CenterY * mainProgram.downSampleRate);
            

            }
        }

        private void setTracker_Click(object sender, EventArgs e)
        {
            if (displayIndiTrue.Checked == true)
            {
                mainProgram.displayIndiState = true;
            }

            if (displayMeshTrue.Checked == true)
            {
                mainProgram.displayMeshOverlays = true;
            }

            if (updateTemplateTrue.Checked == true)
            {
                mainProgram.updateTemplate = true;
            }

            if (visualTrue.Checked == true)
            {
                mainProgram.max = (float)deformTracker.MainProgram.MAX.Visual;
                mainProgram.min = (float)deformTracker.MainProgram.MIN.Visual;
            }

        }

        private void setTrackerNumButton_Click(object sender, EventArgs e)
        {
            if (colNumber.Text != null && rowNumber.Text != null)
            {
                mainProgram.setTrackersColNum = Int32.Parse(colNumber.Text);
                mainProgram.setTrackersRowNum = Int32.Parse(rowNumber.Text);
                
            }
        }

 

   

   

       


    }
}
