using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZedGraph;
using PluginInterface;

namespace RealTimeGraph
{
    public partial class RealTimeGraphControl : UserControl
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        //Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        //The plugin that this control is providing a user interface for
        RealTimeGraph plugin;
        ZedGraphControl plotter;
        private bool hide = false;
        private bool autoscaleYAxis = true;
        private Color graph1Color, graph2Color = Color.Red;

        private WaitForm waitForm = null;

        public ZedGraphControl Plotter
        {
            get
            {
                return plotter;
            }
            set
            {
                plotter = value;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setPlugin">The plugin that the control will provide a user interface for</param>
        public RealTimeGraphControl(RealTimeGraph setPlugin)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {

            //initialize the GUI controls
            InitializeComponent();

            //set the plugin
            plugin = setPlugin;
            //listen to plugin events
            plugin.OneMeasuredValueChanged += new OneValueFloatDelegate(OnPluginOneMeasuredValueChanged);
            plugin.TwoMeasuredValueChanged += new TwoValuesFloatDelegate(OnPluginTwoMeasuredValueChanged);
            plugin.ChannelSelectionChanged += new IntegerDelegate(OnPluginChannelSelectionChanged);
            plugin.StartStopChanged += new BooleanDelegate(OnPluginStartStopChanged);
            plugin.ShowHideWaitForm += new BooleanDelegate(OnShowHideWaitForm);
            plugin.EnableDisableStressors += new BooleanDelegate(OnEnableDisableStressors);
            plugin.BiofeedbackAlgorithmChanged += new BiofeedbackDelegate(OnBiofeedbackAlgorithmChanged);
            plugin.FileNameChanged += new StringDelegate(OnFileNameChanged);
        }

        void OnBiofeedbackAlgorithmChanged(Biofeedback value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BiofeedbackDelegate(OnBiofeedbackAlgorithmChanged), value);
                return;
            }

            if (value == Biofeedback.Basic)
            {
                radioBiofeedbackBasic.Checked = true;
            }
            else if (value == Biofeedback.Advanced1)
            {
                radioBiofeedbackAdv1.Checked = true;
            }
            else if (value == Biofeedback.SelfStart1)
            {
                radioBiofeedbackSelfStart1.Checked = true;
            }
            else if (value == Biofeedback.SelfStart2)
            {
                radioBiofeedbackSelfStart2.Checked = true;
            }
        }

        void OnEnableDisableStressors(bool isBiofeedback)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnEnableDisableStressors), isBiofeedback);
                return;
            }

            checkBoxBiofeedback.Checked = isBiofeedback;
        }

        void OnFileNameChanged(string fName)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnFileNameChanged), fName);
                return;
            }

            if (fName != null)
            {
                if (fName.ToUpper().Contains("COGNITIVE") || fName.ToUpper().Contains("MOTORIC") || fName.ToUpper().Contains("FINAL"))
                {
                    btnStartStopQuestioning.Enabled = true;
                }
                else
                {
                    btnStartStopQuestioning.Enabled = false;
                }

                if (fName.ToUpper().Contains("FINAL"))
                {
                    btnFailureEvent.Visible = true;
                }
                else
                {
                    btnFailureEvent.Visible = false;
                }
            }
        }

        void OnShowHideWaitForm(bool show)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnShowHideWaitForm), show);
                return;
            }
            
            if(show)
            {
                waitForm = new WaitForm();
                waitForm.lblMessage.Text = plugin.WaitFormMessage;
                waitForm.Show();
            }
            else
            {
                waitForm.Close();
                waitForm = null;
            }
        }

        void OnPluginOneMeasuredValueChanged(float value1, float xAxisValue)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new OneValueFloatDelegate(OnPluginOneMeasuredValueChanged), value1, xAxisValue);
                return;
            }

            // Make sure that the graph control exist
            if (Plotter == null)
                return;

            // Make sure that the curvelist has at least one curve
            if (Plotter.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve = Plotter.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the PointPairList
            IPointListEdit list = curve.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;

            

            //add the value
            list.Add(xAxisValue, value1);

            //Keep the X scale
            Scale xScale = Plotter.GraphPane.XAxis.Scale;
            PointPair data = list[0];
            xScale.Min = data.X;
            if (xAxisValue > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = xAxisValue + xScale.MajorStep;
            }

            // Autoscale the Y axis
            if (autoscaleYAxis)
            {
                Plotter.GraphPane.YAxis.Scale.MaxAuto = true;
                Plotter.GraphPane.YAxis.Scale.MinAuto = true;
            }

            // Make sure the Y axis is rescaled to accommodate actual data
            Plotter.AxisChange();
            // Force a redraw
            Plotter.Invalidate();
            
        }
        void OnPluginTwoMeasuredValueChanged(float value1, float value2, float xAxisValue)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new TwoValuesFloatDelegate(OnPluginTwoMeasuredValueChanged), value1, value2, xAxisValue);
                return;
            }

            // Make sure that the graph control exist
            if (Plotter == null)
                return;

            // Make sure that the curvelist has at least one curve
            if (Plotter.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve1 = Plotter.GraphPane.CurveList[0] as LineItem;
            LineItem curve2 = Plotter.GraphPane.CurveList[1] as LineItem;
            if (curve1 == null || curve2 == null)
                return;

            // Get the PointPairList
            IPointListEdit list1 = curve1.Points as IPointListEdit;
            IPointListEdit list2 = curve2.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list1 == null)
                return;

            //add the value
            list1.Add(xAxisValue, value1);
            list2.Add(xAxisValue, value2);

            //Keep the X scale
            Scale xScale = Plotter.GraphPane.XAxis.Scale;
            PointPair data = list1[0];
            xScale.Min = data.X;
            if (xAxisValue > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = xAxisValue + xScale.MajorStep;
            }

            // Autoscale the Y axis
            if (autoscaleYAxis)
            {
                Plotter.GraphPane.YAxis.Scale.MaxAuto = true;
                Plotter.GraphPane.YAxis.Scale.MinAuto = true;
            }

            // Make sure the Y axis is rescaled to accommodate actual data
            Plotter.AxisChange();
            // Force a redraw
            Plotter.Invalidate();
        }
        void OnPluginChannelSelectionChanged(int value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new IntegerDelegate(OnPluginChannelSelectionChanged), value);
                return;
            }

            switch (value)
            {
                case 1:
                    //plugin.Title = "Breathing";
                    plugin.YAxisLabel = plugin.YAxisLabelBreathing;
                    plugin.RollingGraph = true;
                    plugin.Graph2Name = "";

                    Plotter.Show();
                    btHide.Show();
                    settingsPanel.Hide();
                    break;
                case 2:
                    //plugin.Title = "Perspiration";
                    plugin.YAxisLabel = "Energy";
                    plugin.RollingGraph = false;
                    plugin.Graph2Name = "";
                    Plotter.Show();
                    btHide.Show();
                    settingsPanel.Hide();
                    break;
                case 3:
                    //plugin.Title = "Pulse";
                    Plotter.Hide();
                    btHide.Hide();
                    settingsPanel.Hide();
                    break;
                default:
                    //plugin.Title = "Graph";
                    plugin.YAxisLabel = "Value";
                    Plotter.Show();
                    btHide.Show();
                    settingsPanel.Hide();
                    break;
            }

            //update the graph title
            if (plotter.GraphPane.Title == null)
            {
                plugin.TitleChanged += new StringDelegate(OnPluginTitleChanged);
                return;
            }
            Plotter.GraphPane.Title.Text = plugin.Title;
            tbTitle.Text = plugin.Title;

            //update the y-axis label
            if (Plotter.GraphPane.YAxis == null)
                return;
            Plotter.GraphPane.YAxis.Title.Text = plugin.YAxisLabel;
            tbyaxislabel.Text = plugin.YAxisLabel;

            //update the rolling graph
            checkBoxRollingGraph.Checked = plugin.RollingGraph;

            this.Clean();
        }

        void OnPluginStartStopChanged(bool value)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnPluginStartStopChanged), value);
                return;
            }
            
            //if (value)
            //{
                this.Clean();
            //}
            
            if(!value)
            {
                if (btnStartStopQuestioning.Text.ToUpper().Contains("STOP"))
                {
                    plugin.StartStopStressorAction(ActionCode.Stimulus, false);
                    btnStartStopQuestioning.Text = "Start Stimulus";
                }
            }

        }

        private void OnPluginYAxisLabelChanged(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnPluginYAxisLabelChanged), value);
                return;
            }
        }

        private void OnPluginTitleChanged(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnPluginTitleChanged), value);
                return;
            }
        }


        private void OnPluginGraph1NameChanged(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnPluginGraph1NameChanged), value);
                return;
            }
        }

        private void OnPluginGraph2NameChanged(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnPluginGraph2NameChanged), value);
                return;
            }
        }

        private void OnPluginGraph1ColorChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnPluginGraph1ColorChanged), value);
                return;
            }
        }

        private void OnPluginGraph2ColorChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnPluginGraph2ColorChanged), value);
                return;
            }
        }

        private void OnPluginRollingGraphChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnPluginRollingGraphChanged), value);
                return;
            }
        }

        private void OnPluginUseTimeForXAxisChanged(bool value)
        {
            if (InvokeRequired)
            {
                Invoke(new BooleanDelegate(OnPluginUseTimeForXAxisChanged), value);
                return;
            }
        }

        private void tbyaxislabel_TextChanged(object sender, EventArgs e)
        {
            plugin.YAxisLabelChanged -= new StringDelegate(OnPluginYAxisLabelChanged);

            //set the y-axis label
            if (plugin.ChannelSelector == 1)
            {
                plugin.YAxisLabelBreathing = tbyaxislabel.Text;
                plugin.YAxisLabel = tbyaxislabel.Text;
            }
            else
                plugin.YAxisLabel = tbyaxislabel.Text;

            //update the y-axis label
            if (Plotter.GraphPane.YAxis == null)
                return;

            Plotter.GraphPane.YAxis.Title.Text = tbyaxislabel.Text;
            Plotter.Invalidate();

            plugin.YAxisLabelChanged += new StringDelegate(OnPluginYAxisLabelChanged);
        }

        private void tbTitle_TextChanged(object sender, EventArgs e)
        {
            plugin.TitleChanged -= new StringDelegate(OnPluginTitleChanged);

            //set the graph title 
            plugin.Title = tbTitle.Text;

            //update the graph title
            if (plotter.GraphPane.Title == null)
            {
                plugin.TitleChanged += new StringDelegate(OnPluginTitleChanged);
                return;
            }

            Plotter.GraphPane.Title.Text = tbTitle.Text;
            Plotter.Invalidate();

            plugin.TitleChanged += new StringDelegate(OnPluginTitleChanged);
        }

        private void tbGraph1_TextChanged(object sender, EventArgs e)
        {
            plugin.Graph1NameChanged -= new StringDelegate(OnPluginGraph1NameChanged);

            plugin.Graph1Name = tbGraph1.Text;

            LineItem curve1 = Plotter.GraphPane.CurveList[0] as LineItem;
            curve1.Label.Text = tbGraph1.Text;

            Plotter.Invalidate();

            plugin.Graph1NameChanged += new StringDelegate(OnPluginGraph1NameChanged);
        }

        private void tbGraph2_TextChanged(object sender, EventArgs e)
        {
            plugin.Graph2NameChanged -= new StringDelegate(OnPluginGraph2NameChanged);

            plugin.Graph2Name = tbGraph2.Text;

            LineItem curve2 = Plotter.GraphPane.CurveList[1] as LineItem;

            curve2.Label.Text = tbGraph2.Text;
            Plotter.Invalidate();

            plugin.Graph2NameChanged += new StringDelegate(OnPluginGraph2NameChanged);
        }



        private void Graph1Color_Changed(object sender, EventArgs e)
        {
            plugin.Graph1ColorChanged -= new BooleanDelegate(OnPluginGraph1ColorChanged);

            plugin.Graph1Color = rbRedGraph1.Checked;
            if (Plotter.GraphPane.CurveList.Count > 0)
            {
                LineItem curve1 = Plotter.GraphPane.CurveList[0] as LineItem;
                if (rbRedGraph1.Checked)
                {
                    curve1.Color = Color.Red;
                }
                else
                {
                    curve1.Color = Color.Blue;
                }
            }

            plugin.Graph1ColorChanged += new BooleanDelegate(OnPluginGraph1ColorChanged);
        }

        private void Graph2Color_Changed(object sender, EventArgs e)
        {
            plugin.Graph2ColorChanged -= new BooleanDelegate(OnPluginGraph2ColorChanged);

            plugin.Graph2Color = rbRedGraph2.Checked;
            if (Plotter.GraphPane.CurveList.Count > 1)
            {
                LineItem curve2 = Plotter.GraphPane.CurveList[1] as LineItem;
                if (rbRedGraph2.Checked)
                {
                    curve2.Color = Color.Red;
                }
                else
                {
                    curve2.Color = Color.Blue;
                }
            }

            plugin.Graph2ColorChanged += new BooleanDelegate(OnPluginGraph2ColorChanged);
        }

        private void checkBoxRollingGraph_CheckedChanged(object sender, EventArgs e)
        {
            plugin.RollingGraphChanged -= new BooleanDelegate(OnPluginRollingGraphChanged);

            plugin.RollingGraph = checkBoxRollingGraph.Checked;
            this.Clean();

            plugin.RollingGraphChanged += new BooleanDelegate(OnPluginRollingGraphChanged);

        }

        private void xAxisTime_CheckedChanged(object sender, EventArgs e)
        {
            plugin.UseTimeForXAxisChanged -= new BooleanDelegate(OnPluginUseTimeForXAxisChanged);

            plugin.UseTimeForXAxis = checkBoxUseTimeForXAxis.Checked;
            if (checkBoxUseTimeForXAxis.Checked)
                Plotter.GraphPane.XAxis.Title.Text = "Time [s]";
            else
                Plotter.GraphPane.XAxis.Title.Text = "Frames";

            this.Clean();

            plugin.UseTimeForXAxisChanged += new BooleanDelegate(OnPluginUseTimeForXAxisChanged);
        }

        //Create the plotter and add it to the control
        private void RealTimeGraphControl_Load(object sender, EventArgs e)
        {

            settingsPanel.Hide();

            Plotter = new ZedGraphControl();
            this.panel2.Controls.Add(Plotter);
            Plotter.Dock = DockStyle.Fill;
            GraphPane myPane1 = Plotter.GraphPane;
            // Turn off the axis background fill
            myPane1.Chart.Fill.IsVisible = false;
            myPane1.Legend.IsVisible = true;

            // Set values from plugin
            myPane1.Title.Text = plugin.Title;
            tbTitle.Text = plugin.Title;
            myPane1.YAxis.Title.Text = plugin.YAxisLabel;
            tbyaxislabel.Text = plugin.YAxisLabel;

            if (plugin.UseTimeForXAxis)
                myPane1.XAxis.Title.Text = "Time [s]";
            else
                myPane1.XAxis.Title.Text = "Frames";


            if (plugin.Graph1Color)
            {
                graph1Color = Color.Red;
                rbRedGraph1.Checked = true;
            }
            else
            {
                graph1Color = Color.Blue;
                rbBlueGraph1.Checked = true;
            }

            if (plugin.Graph2Color)
            {
                graph2Color = Color.Red;
                rbRedGraph2.Checked = true;
            }
            else
            {
                graph2Color = Color.Blue;
                rbBlueGraph2.Checked = true;
            }

            LineItem curve1, curve2 = null;

            //the list to store our data points
            if (plugin.RollingGraph)
            {
                RollingPointPairList list1 = new RollingPointPairList(250);
                RollingPointPairList list2 = new RollingPointPairList(250);

                // Initially, a curve is added with no data points (list is empty)
                curve1 = myPane1.AddCurve(plugin.Graph1Name, list1, graph1Color, SymbolType.None);
                curve2 = myPane1.AddCurve(plugin.Graph2Name, list2, graph2Color, SymbolType.None);
            }
            else
            {
                PointPairList list1 = new PointPairList();
                PointPairList list2 = new PointPairList();

                // Initially, a curve is added with no data points (list is empty)
                curve1 = myPane1.AddCurve(plugin.Graph1Name, list1, graph1Color, SymbolType.None);
                curve2 = myPane1.AddCurve(plugin.Graph2Name, list2, graph2Color, SymbolType.None);
            }
            curve1.Line.Width = 2.0F;
            curve2.Line.Width = 2.0F;


            tbGraph1.Text = plugin.Graph1Name;
            tbGraph2.Text = plugin.Graph2Name;
            checkBoxRollingGraph.Checked = plugin.RollingGraph;

            checkBoxUseTimeForXAxis.Checked = plugin.UseTimeForXAxis;

            // Just manually control the X axis range
            myPane1.XAxis.Scale.Min = 0;
            if (checkBoxUseTimeForXAxis.Checked)
            {
                myPane1.XAxis.Scale.Max = 12;
                tbMaxScaleX.Text = "12";
                myPane1.XAxis.Scale.MinorStep = 0.4;
                tbTickMinX.Text = "0.4";
                myPane1.XAxis.Scale.MajorStep = 2;
                tbTickMaxX.Text = "2";
            }
            else
            {
                myPane1.XAxis.Scale.Max = 300;
                tbMaxScaleX.Text = "300";
                myPane1.XAxis.Scale.MinorStep = 10;
                tbTickMinX.Text = "10";
                myPane1.XAxis.Scale.MajorStep = 50;
                tbTickMaxX.Text = "50";
            }

            //Scale the axes
            Plotter.AxisChange();
        }

        private void btClean_Click(object sender, EventArgs e)
        {
            this.Clean();
        }

        private void Clean()
        {


            // Get the curves and remove them
            LineItem curve1 = Plotter.GraphPane.CurveList[0] as LineItem;
            LineItem curve2 = Plotter.GraphPane.CurveList[1] as LineItem;
            if (curve1 == null || curve2 == null)
                return;

            Plotter.GraphPane.CurveList.Remove(curve1);
            Plotter.GraphPane.CurveList.Remove(curve2);

            //the list to store our data points
            if (plugin.RollingGraph)
            {
                RollingPointPairList list1 = new RollingPointPairList(250);
                RollingPointPairList list2 = new RollingPointPairList(250);

                // Initially, a curve is added with no data points (list is empty)
                curve1 = Plotter.GraphPane.AddCurve(plugin.Graph1Name, list1, graph1Color, SymbolType.None);
                curve2 = Plotter.GraphPane.AddCurve(plugin.Graph2Name, list2, graph2Color, SymbolType.None);
            }
            else
            {
                PointPairList list1 = new PointPairList();
                PointPairList list2 = new PointPairList();

                // Initially, a curve is added with no data points (list is empty)
                curve1 = Plotter.GraphPane.AddCurve(plugin.Graph1Name, list1, graph1Color, SymbolType.None);
                curve2 = Plotter.GraphPane.AddCurve(plugin.Graph2Name, list2, graph2Color, SymbolType.None);

            }
            curve1.Line.Width = 2.0F;
            curve2.Line.Width = 2.0F;

            if (checkBoxUseTimeForXAxis.Checked)
            {
                Plotter.GraphPane.XAxis.Scale.Max = 12;
                Plotter.GraphPane.XAxis.Scale.MinorStep = 0.4;
                Plotter.GraphPane.XAxis.Scale.MajorStep = 2;
            }
            else
            {
                Plotter.GraphPane.XAxis.Scale.Max = 300;
                Plotter.GraphPane.XAxis.Scale.MinorStep = 10;
                Plotter.GraphPane.XAxis.Scale.MajorStep = 50;
            }

            Plotter.AxisChange();
            Plotter.Invalidate();
        }

        private void btHide_Click(object sender, EventArgs e)
        {
            if (hide)
            {
                hide = false;
                settingsPanel.Show();
            }
            else
            {
                hide = true;
                settingsPanel.Hide();
            }
        }

        private void tbMaxScaleX_TextChanged(object sender, EventArgs e)
        {
            bool scale = false;

            try
            {
                Convert.ToDouble(tbMaxScaleX.Text);
                scale = true;
            }
            catch { }

            if (scale)
            {
                ///update the x-axis max scale
                if (Plotter.GraphPane.XAxis == null)
                    return;

                Plotter.GraphPane.XAxis.Scale.Max = Convert.ToDouble(tbMaxScaleX.Text);
                Plotter.Invalidate();
            }
        }

        private void tbTickMin_TextChanged(object sender, EventArgs e)
        {
            bool tick = false;

            try
            {
                Convert.ToDouble(tbTickMinX.Text);
                tick = true;
            }
            catch { }

            if (tick)
            {
                //update the x-axis min tick
                if (Plotter.GraphPane.XAxis == null)
                    return;

                Plotter.GraphPane.XAxis.Scale.MinorStep = Convert.ToDouble(tbTickMinX.Text);
                Plotter.Invalidate();
            }
        }

        private void tbTickMax_TextChanged(object sender, EventArgs e)
        {
            bool tick = false;

            try
            {
                Convert.ToDouble(tbTickMaxX.Text);
                tick = true;
            }
            catch { }

            if (tick)
            {
                //update the x-axis max tick
                if (Plotter.GraphPane.XAxis == null)
                    return;

                Plotter.GraphPane.XAxis.Scale.MajorStep = Convert.ToDouble(tbTickMaxX.Text);
                Plotter.Invalidate();
            }
        }

        private void textBox_ymin_TextChanged(object sender, EventArgs e)
        {
            bool flag = false;
            try
            {
                Convert.ToDouble(textBox_ymin.Text);
                flag = true;
            }
            catch { }

            if (flag)
            {
                //update the y-axis min value
                if (Plotter.GraphPane.YAxis == null)
                    return;

                Plotter.GraphPane.YAxis.Scale.Min = Convert.ToDouble(textBox_ymin.Text);
                Plotter.Invalidate();
            }
        }

        private void textBox_ymax_TextChanged(object sender, EventArgs e)
        {
            bool flag = false;
            try
            {
                Convert.ToDouble(textBox_ymax.Text);
                flag = true;
            }
            catch { }

            if (flag)
            {
                //update the y-axis max value
                if (Plotter.GraphPane.YAxis == null)
                    return;

                Plotter.GraphPane.YAxis.Scale.Max = Convert.ToDouble(textBox_ymax.Text);
                Plotter.Invalidate();
            }
        }

        private void btn_yminDecrease_Click(object sender, EventArgs e)
        {
            textBox_ymin.TextChanged -= textBox_ymin_TextChanged;

            if (Plotter.GraphPane.YAxis == null)
                return;

            double newYMin = Plotter.GraphPane.YAxis.Scale.Min - Plotter.GraphPane.YAxis.Scale.MinorStep;
            textBox_ymin.Text = newYMin.ToString();

            Plotter.GraphPane.YAxis.Scale.Min = newYMin;
            Plotter.Invalidate();

            textBox_ymin.TextChanged += textBox_ymin_TextChanged;

        }

        private void btn_yminIncrease_Click(object sender, EventArgs e)
        {
            textBox_ymin.TextChanged -= textBox_ymin_TextChanged;

            if (Plotter.GraphPane.YAxis == null)
                return;

            double newYMin = Plotter.GraphPane.YAxis.Scale.Min + Plotter.GraphPane.YAxis.Scale.MinorStep;
            textBox_ymin.Text = newYMin.ToString();

            Plotter.GraphPane.YAxis.Scale.Min = newYMin;
            Plotter.Invalidate();

            textBox_ymin.TextChanged += textBox_ymin_TextChanged;

        }

        private void btn_ymaxDecrease_Click(object sender, EventArgs e)
        {
            textBox_ymax.TextChanged -= textBox_ymax_TextChanged;

            if (Plotter.GraphPane.YAxis == null)
                return;

            double newYMax = Plotter.GraphPane.YAxis.Scale.Max - Plotter.GraphPane.YAxis.Scale.MinorStep;
            textBox_ymax.Text = newYMax.ToString();

            Plotter.GraphPane.YAxis.Scale.Max = newYMax;
            Plotter.Invalidate();

            textBox_ymax.TextChanged += textBox_ymax_TextChanged;
        }

        private void btn_ymaxIncrease_Click(object sender, EventArgs e)
        {
            textBox_ymax.TextChanged -= textBox_ymax_TextChanged;

            if (Plotter.GraphPane.YAxis == null)
                return;

            double newYMax = Plotter.GraphPane.YAxis.Scale.Max + Plotter.GraphPane.YAxis.Scale.MinorStep;
            textBox_ymax.Text = newYMax.ToString();

            Plotter.GraphPane.YAxis.Scale.Max = newYMax;
            Plotter.Invalidate();

            textBox_ymax.TextChanged += textBox_ymax_TextChanged;
        }

        private void checkBoxAutoScale_CheckedChanged(object sender, EventArgs e)
        {
            autoscaleYAxis = checkBoxAutoScale.Checked;
            textBox_ymin.Enabled = !autoscaleYAxis;
            textBox_ymax.Enabled = !autoscaleYAxis;
            btn_yminDecrease.Enabled = !autoscaleYAxis;
            btn_yminIncrease.Enabled = !autoscaleYAxis;
            btn_ymaxDecrease.Enabled = !autoscaleYAxis;
            btn_ymaxIncrease.Enabled = !autoscaleYAxis;

            textBox_ymin.TextChanged -= textBox_ymin_TextChanged;
            textBox_ymax.TextChanged -= textBox_ymax_TextChanged;
            textBox_ymin.Text = Plotter.GraphPane.YAxis.Scale.Min.ToString();
            textBox_ymax.Text = Plotter.GraphPane.YAxis.Scale.Max.ToString();
            textBox_ymin.TextChanged += textBox_ymin_TextChanged;
            textBox_ymax.TextChanged += textBox_ymax_TextChanged;

        }

        private void settingsPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnStartStopQuestioning_Click(object sender, EventArgs e)
        {
            if(btnStartStopQuestioning.Text == "Start Stimulus")
            {
                plugin.StartStopStressorAction(ActionCode.Stimulus, true);
                btnStartStopQuestioning.Text = "Stop Stimulus...";
            }
            else
            {
                plugin.StartStopStressorAction(ActionCode.Stimulus, false);
                btnStartStopQuestioning.Text = "Start Stimulus";
            }

            
        }

        private void radioBiofeedbackBasic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBiofeedbackBasic.Checked)
            {
                plugin.BiofeedbackAlgorithm = Biofeedback.Basic;
            }
        }

        private void radioBiofeedbackSelfStart1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBiofeedbackSelfStart1.Checked)
            {
                plugin.BiofeedbackAlgorithm = Biofeedback.SelfStart1;
            }
        }

        private void radioBiofeedbackSelfStart2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBiofeedbackSelfStart2.Checked)
            {
                plugin.BiofeedbackAlgorithm = Biofeedback.SelfStart2;
            }
        }

        private void btnFailureEvent_Click(object sender, EventArgs e)
        {
            plugin.StartStopStressorAction(ActionCode.Failure, true);
            btnFailureEvent.Enabled = false;
        }

        private void checkBoxBiofeedback_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioBiofeedbackAdv1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBiofeedbackAdv1.Checked)
            {
                plugin.BiofeedbackAlgorithm = Biofeedback.Advanced1;
            }
            
        }








    }
}
