using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThermalVisualizerPlugin
{
    public partial class ThermalVisualizerControl : UserControl
    {
        ThermalVisualizer plugin;

        float lowerTemp = 18*100;
        float upperTemp = 50*100;

        //float lowerMaxTemp = 20;
        //float upperMaxTemp = 50;

        Point minTrackLoc;
        Point maxTrackLoc;

        int offset_X = 0;

        float trackWidthTempRatio = 1.0F; //Width per degree change of temp.


        public ThermalVisualizerControl()
        {
            InitializeComponent();
        }

        public ThermalVisualizerControl(ThermalVisualizer setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;

            // Listen to the delegate
            plugin.MaxTempChanged +=new FloatDelegate(plugin_MaxTempChanged);
            plugin.MinTempChanged += new FloatDelegate(plugin_MinTempChanged);
            plugin.OptimizedChanged += new BoolDelegate(plugin_OptimizedChanged);

            //trackBarMax.TickFrequency = 1000;
            trackBarMax.Minimum = (int)lowerTemp;
            trackBarMax.Maximum = (int)upperTemp;
            trackBarMax.SmallChange = 1;
            trackBarMax.Value = (int)upperTemp - 200;

            //trackBarMin.TickFrequency = 1000;
            trackBarMin.Minimum = (int)lowerTemp;
            trackBarMin.Maximum = (int)upperTemp;
            trackBarMin.SmallChange = 1;
            trackBarMin.Value = (int)lowerTemp + 200;

            minTrackLoc = new Point();
            maxTrackLoc = new Point();

            UpdateColorBarAsPerTrackWidthRatio();
        }

        void plugin_OptimizedChanged(bool value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(plugin_OptimizedChanged), value);
                return;
            }
            checkBoxOptimized.Checked = value;
        }


        
        void plugin_MaxTempChanged(float value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new FloatDelegate(plugin_MaxTempChanged), value);
                return;
            }
            //if (value >= trackBarMax.Minimum && value <= trackBarMax.Maximum)
            {
            
                upperTemp = (int)value * 100 + 200;
                trackBarMax.Maximum = (int)upperTemp;
                trackBarMin.Maximum = (int)upperTemp;
                trackBarMax.Value = (int)value * 100;
                textBoxMax.Text = (trackBarMax.Value/100).ToString();
                UpdateColorBarAsPerTrackWidthRatio();
            }
        }

        void plugin_MinTempChanged(float value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new FloatDelegate(plugin_MinTempChanged), value);
                return;
            }

            //if (value >= trackBarMin.Minimum && value <= trackBarMin.Maximum)
            {
                lowerTemp = (int)value*100 - 200;
                trackBarMax.Minimum = (int)lowerTemp;                
                trackBarMin.Minimum = (int)lowerTemp;
                trackBarMin.Value = (int)value * 100;

                textBoxMin.Text = (trackBarMin.Value/100).ToString();

                UpdateColorBarAsPerTrackWidthRatio();

            }

        }

        private void textBoxMax_TextChanged(object sender, EventArgs e)
        {Update_textBoxMax();}


        private void textBoxMin_TextChanged(object sender, EventArgs e)
        {Update_textBoxMin();}

        private void Update_textBoxMax()
        {
            float i = 0;
            float temp;
            if (float.TryParse(textBoxMax.Text, out i))
            {
                temp = (float)Convert.ToDouble(textBoxMax.Text)*100.0F;
                if (temp >= trackBarMax.Minimum && temp <= trackBarMax.Maximum)
                { plugin.MaxTempProperty = temp/100.0F; }
            }
        }

        private void Update_textBoxMin()
        {

            float i = 0;
            float temp;
            if (float.TryParse(textBoxMin.Text, out i))
            { 
                temp = (float)Convert.ToDouble(textBoxMin.Text)*100.0F;
                if (temp >= trackBarMin.Minimum && temp <= trackBarMin.Maximum)
                { plugin.MinTempProperty = temp/100.0F; }
            }
        }

        private void trackBarMin_Scroll(object sender, EventArgs e)
        {
            //Stop listening to the plug-in's "ThresholdChanged" event because the threshold value will be set by this control (otherwise an infinite loop would result)
            if (trackBarMin.Value < trackBarMax.Value)
            {
                plugin.MinTempChanged -= new FloatDelegate(plugin_MinTempChanged);
                plugin.MinTempProperty = trackBarMin.Value/100.0F;
                textBoxMin.Text = (trackBarMin.Value/100.0F).ToString();
                plugin.MinTempChanged += new FloatDelegate(plugin_MinTempChanged);//listen to the plug-in's "MaxTempChanged" event again
            }
            else
            {
                trackBarMin.Value = trackBarMax.Value;
                
            }
            UpdateColorBarAsPerTrackWidthRatio();
        }

        private void trackBarMax_Scroll(object sender, EventArgs e)
        {
            //Stop listening to the plug-in's "MaxTempChanged" event because the max value will be set by this control (otherwise an infinite loop would result)
            if (trackBarMax.Value > trackBarMin.Value)
            {
                plugin.MaxTempChanged -= new FloatDelegate(plugin_MaxTempChanged);
                plugin.MaxTempProperty = trackBarMax.Value/100.0F;
                textBoxMax.Text = (trackBarMax.Value/100.0F).ToString();
                plugin.MaxTempChanged += new FloatDelegate(plugin_MaxTempChanged);//listen to the plug-in's "MaxTempChanged" event again
            }
            else
            {
                trackBarMax.Value = trackBarMin.Value;
            }

            UpdateColorBarAsPerTrackWidthRatio();
        }

        private void checkBoxOptimized_CheckedChanged(object sender, EventArgs e)
        {
            plugin.IsOptimized = checkBoxOptimized.Checked;
        }

        private void trackBarMin_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void trackBarMin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && trackBarMax.Value > trackBarMin.Value && e.X >= 5 && e.X <= trackBarMin.Width-10)
            {
               // minTrackLoc.X = e.X;
               // minTrackLoc.Y = e.Y;

              
                //UpdateColorBar();
            }
        }

        private void trackBarMax_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left && trackBarMax.Value > trackBarMin.Value && e.X >= 5 && e.X <= trackBarMax.Width-10)
            //{
            //   // maxTrackLoc.X = e.X;
            //   // maxTrackLoc.Y = e.Y;
                
            //    //UpdateColorBar();
            //}
        }

        


        private void UpdateColorBar()
        {
            pictureBoxBlue.Width = minTrackLoc.X;
            pictureBoxBlue.Invalidate(Region);

            Point cBar = new Point();
            cBar.X = offset_X + minTrackLoc.X;
            cBar.Y = pictureBoxColorBar.Location.Y;
            pictureBoxColorBar.Location = cBar;
            pictureBoxColorBar.Width = maxTrackLoc.X-minTrackLoc.X;
            pictureBoxColorBar.Invalidate(Region);

            Point cRed = new Point();
            cRed.X = offset_X + maxTrackLoc.X;
            cRed.Y = pictureBoxRed.Location.Y;
            int diffX = pictureBoxRed.Location.X - cRed.X;
            pictureBoxRed.Location = cRed;
            pictureBoxRed.Width = pictureBoxRed.Width + diffX;
            pictureBoxRed.Invalidate(Region);


        }
        private void UpdateColorBarAsPerTrackWidthRatio()
        {
            offset_X = pictureBoxBlue.Location.X;

            trackWidthTempRatio = ((trackBarMax.Width-22) / (upperTemp - lowerTemp)); //Width per degree change of temp.
            minTrackLoc.X = (int)Math.Round((trackWidthTempRatio * (trackBarMin.Value - lowerTemp))) ;
            maxTrackLoc.X = (int)Math.Round((trackWidthTempRatio * (trackBarMax.Value - lowerTemp)));

            int offestLeft = 0;
            int offsetRight = 0;
            
            if (minTrackLoc.X < 5)
            {offestLeft = 5;}
            if (maxTrackLoc.X > trackBarMax.Width - 5)
            {
                offsetRight = 5;
            }
            pictureBoxBlue.Width = minTrackLoc.X + offestLeft;

            pictureBoxBlue.Invalidate(Region);

            Point cBar = new Point();
            cBar.X = offset_X + minTrackLoc.X + offestLeft;
            cBar.Y = pictureBoxColorBar.Location.Y;
            pictureBoxColorBar.Location = cBar;
            pictureBoxColorBar.Width = maxTrackLoc.X - minTrackLoc.X - offsetRight - offestLeft;
            pictureBoxColorBar.Invalidate(Region);

            Point cRed = new Point();
            cRed.X = offset_X + maxTrackLoc.X - offsetRight;
            cRed.Y = pictureBoxRed.Location.Y;
            int diffX = trackBarMax.Width - pictureBoxColorBar.Width - pictureBoxBlue.Width-15; ///*pictureBoxRed.Location.X*/ -cRed.X;
            pictureBoxRed.Location = cRed;
            pictureBoxRed.Width = /*pictureBoxRed.Width +*/ diffX;
            pictureBoxRed.Invalidate(Region);


        }

        protected override void OnPaint(PaintEventArgs e)
        {
           // UpdateColorBar();
        }

        private void ThermalVisualizerControl_SizeChanged(object sender, EventArgs e)
        {
            UpdateColorBarAsPerTrackWidthRatio();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            plugin.isFirstFrame = true;
        }

        

        
        
      
       
       
    }
}
