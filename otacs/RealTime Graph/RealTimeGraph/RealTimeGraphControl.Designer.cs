namespace RealTimeGraph
{
    partial class RealTimeGraphControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
      //  protected override void Dispose()
        //void Dispose(bool disposing)
        //{
        //    if (disposing && components != null)
        //    {
        //        components.Dispose();
        //    }
        //    //base.Dispose();
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioBiofeedbackSelfStart2 = new System.Windows.Forms.RadioButton();
            this.radioBiofeedbackSelfStart1 = new System.Windows.Forms.RadioButton();
            this.radioBiofeedbackAdv1 = new System.Windows.Forms.RadioButton();
            this.radioBiofeedbackBasic = new System.Windows.Forms.RadioButton();
            this.graph2colorPanel = new System.Windows.Forms.Panel();
            this.rbBlueGraph2 = new System.Windows.Forms.RadioButton();
            this.rbRedGraph2 = new System.Windows.Forms.RadioButton();
            this.graph1colorPanel = new System.Windows.Forms.Panel();
            this.rbBlueGraph1 = new System.Windows.Forms.RadioButton();
            this.rbRedGraph1 = new System.Windows.Forms.RadioButton();
            this.tbMaxScaleX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUseTimeForXAxis = new System.Windows.Forms.CheckBox();
            this.checkBoxRollingGraph = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoScale = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_ymaxIncrease = new System.Windows.Forms.Button();
            this.btn_ymaxDecrease = new System.Windows.Forms.Button();
            this.textBox_ymax = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_yminIncrease = new System.Windows.Forms.Button();
            this.btn_yminDecrease = new System.Windows.Forms.Button();
            this.textBox_ymin = new System.Windows.Forms.TextBox();
            this.tbGraph2 = new System.Windows.Forms.TextBox();
            this.tbGraph1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTickMaxX = new System.Windows.Forms.TextBox();
            this.btClean = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbTickMinX = new System.Windows.Forms.TextBox();
            this.tbyaxislabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnFailureEvent = new System.Windows.Forms.Button();
            this.btnStartStopQuestioning = new System.Windows.Forms.Button();
            this.checkBoxBiofeedback = new System.Windows.Forms.CheckBox();
            this.btHide = new System.Windows.Forms.Button();
            this.settingsPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.graph2colorPanel.SuspendLayout();
            this.graph1colorPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.groupBox3);
            this.settingsPanel.Controls.Add(this.graph2colorPanel);
            this.settingsPanel.Controls.Add(this.graph1colorPanel);
            this.settingsPanel.Controls.Add(this.tbMaxScaleX);
            this.settingsPanel.Controls.Add(this.label1);
            this.settingsPanel.Controls.Add(this.checkBoxUseTimeForXAxis);
            this.settingsPanel.Controls.Add(this.checkBoxRollingGraph);
            this.settingsPanel.Controls.Add(this.checkBoxAutoScale);
            this.settingsPanel.Controls.Add(this.groupBox2);
            this.settingsPanel.Controls.Add(this.groupBox1);
            this.settingsPanel.Controls.Add(this.tbGraph2);
            this.settingsPanel.Controls.Add(this.tbGraph1);
            this.settingsPanel.Controls.Add(this.label7);
            this.settingsPanel.Controls.Add(this.label6);
            this.settingsPanel.Controls.Add(this.tbTickMaxX);
            this.settingsPanel.Controls.Add(this.btClean);
            this.settingsPanel.Controls.Add(this.label5);
            this.settingsPanel.Controls.Add(this.tbTickMinX);
            this.settingsPanel.Controls.Add(this.tbyaxislabel);
            this.settingsPanel.Controls.Add(this.label3);
            this.settingsPanel.Controls.Add(this.label4);
            this.settingsPanel.Controls.Add(this.label2);
            this.settingsPanel.Controls.Add(this.tbTitle);
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.settingsPanel.Location = new System.Drawing.Point(0, 229);
            this.settingsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.settingsPanel.MinimumSize = new System.Drawing.Size(900, 69);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(900, 183);
            this.settingsPanel.TabIndex = 0;
            this.settingsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.settingsPanel_Paint);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.radioBiofeedbackSelfStart2);
            this.groupBox3.Controls.Add(this.radioBiofeedbackSelfStart1);
            this.groupBox3.Controls.Add(this.radioBiofeedbackAdv1);
            this.groupBox3.Controls.Add(this.radioBiofeedbackBasic);
            this.groupBox3.Location = new System.Drawing.Point(441, 116);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(438, 55);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Biofeedback Algorithm";
            // 
            // radioBiofeedbackSelfStart2
            // 
            this.radioBiofeedbackSelfStart2.AutoSize = true;
            this.radioBiofeedbackSelfStart2.Location = new System.Drawing.Point(322, 23);
            this.radioBiofeedbackSelfStart2.Name = "radioBiofeedbackSelfStart2";
            this.radioBiofeedbackSelfStart2.Size = new System.Drawing.Size(91, 21);
            this.radioBiofeedbackSelfStart2.TabIndex = 3;
            this.radioBiofeedbackSelfStart2.TabStop = true;
            this.radioBiofeedbackSelfStart2.Text = "SelfStart2";
            this.radioBiofeedbackSelfStart2.UseVisualStyleBackColor = true;
            this.radioBiofeedbackSelfStart2.CheckedChanged += new System.EventHandler(this.radioBiofeedbackSelfStart2_CheckedChanged);
            // 
            // radioBiofeedbackSelfStart1
            // 
            this.radioBiofeedbackSelfStart1.AutoSize = true;
            this.radioBiofeedbackSelfStart1.Location = new System.Drawing.Point(213, 23);
            this.radioBiofeedbackSelfStart1.Name = "radioBiofeedbackSelfStart1";
            this.radioBiofeedbackSelfStart1.Size = new System.Drawing.Size(91, 21);
            this.radioBiofeedbackSelfStart1.TabIndex = 2;
            this.radioBiofeedbackSelfStart1.TabStop = true;
            this.radioBiofeedbackSelfStart1.Text = "SelfStart1";
            this.radioBiofeedbackSelfStart1.UseVisualStyleBackColor = true;
            this.radioBiofeedbackSelfStart1.CheckedChanged += new System.EventHandler(this.radioBiofeedbackSelfStart1_CheckedChanged);
            // 
            // radioBiofeedbackAdv1
            // 
            this.radioBiofeedbackAdv1.AutoSize = true;
            this.radioBiofeedbackAdv1.Location = new System.Drawing.Point(85, 23);
            this.radioBiofeedbackAdv1.Name = "radioBiofeedbackAdv1";
            this.radioBiofeedbackAdv1.Size = new System.Drawing.Size(107, 21);
            this.radioBiofeedbackAdv1.TabIndex = 1;
            this.radioBiofeedbackAdv1.TabStop = true;
            this.radioBiofeedbackAdv1.Text = "ND Baseline";
            this.radioBiofeedbackAdv1.UseVisualStyleBackColor = true;
            this.radioBiofeedbackAdv1.CheckedChanged += new System.EventHandler(this.radioBiofeedbackAdv1_CheckedChanged);
            // 
            // radioBiofeedbackBasic
            // 
            this.radioBiofeedbackBasic.AutoSize = true;
            this.radioBiofeedbackBasic.Location = new System.Drawing.Point(12, 23);
            this.radioBiofeedbackBasic.Name = "radioBiofeedbackBasic";
            this.radioBiofeedbackBasic.Size = new System.Drawing.Size(63, 21);
            this.radioBiofeedbackBasic.TabIndex = 0;
            this.radioBiofeedbackBasic.TabStop = true;
            this.radioBiofeedbackBasic.Text = "Basic";
            this.radioBiofeedbackBasic.UseVisualStyleBackColor = true;
            this.radioBiofeedbackBasic.CheckedChanged += new System.EventHandler(this.radioBiofeedbackBasic_CheckedChanged);
            // 
            // graph2colorPanel
            // 
            this.graph2colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.graph2colorPanel.Controls.Add(this.rbBlueGraph2);
            this.graph2colorPanel.Controls.Add(this.rbRedGraph2);
            this.graph2colorPanel.Location = new System.Drawing.Point(243, 79);
            this.graph2colorPanel.Margin = new System.Windows.Forms.Padding(4);
            this.graph2colorPanel.Name = "graph2colorPanel";
            this.graph2colorPanel.Size = new System.Drawing.Size(129, 28);
            this.graph2colorPanel.TabIndex = 23;
            // 
            // rbBlueGraph2
            // 
            this.rbBlueGraph2.AutoSize = true;
            this.rbBlueGraph2.Location = new System.Drawing.Point(63, 4);
            this.rbBlueGraph2.Margin = new System.Windows.Forms.Padding(4);
            this.rbBlueGraph2.Name = "rbBlueGraph2";
            this.rbBlueGraph2.Size = new System.Drawing.Size(57, 21);
            this.rbBlueGraph2.TabIndex = 1;
            this.rbBlueGraph2.TabStop = true;
            this.rbBlueGraph2.Text = "Blue";
            this.rbBlueGraph2.UseVisualStyleBackColor = true;
            this.rbBlueGraph2.CheckedChanged += new System.EventHandler(this.Graph2Color_Changed);
            // 
            // rbRedGraph2
            // 
            this.rbRedGraph2.AutoSize = true;
            this.rbRedGraph2.Location = new System.Drawing.Point(5, 2);
            this.rbRedGraph2.Margin = new System.Windows.Forms.Padding(4);
            this.rbRedGraph2.Name = "rbRedGraph2";
            this.rbRedGraph2.Size = new System.Drawing.Size(55, 21);
            this.rbRedGraph2.TabIndex = 0;
            this.rbRedGraph2.TabStop = true;
            this.rbRedGraph2.Text = "Red";
            this.rbRedGraph2.UseVisualStyleBackColor = true;
            this.rbRedGraph2.CheckedChanged += new System.EventHandler(this.Graph2Color_Changed);
            // 
            // graph1colorPanel
            // 
            this.graph1colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.graph1colorPanel.Controls.Add(this.rbBlueGraph1);
            this.graph1colorPanel.Controls.Add(this.rbRedGraph1);
            this.graph1colorPanel.Location = new System.Drawing.Point(243, 46);
            this.graph1colorPanel.Margin = new System.Windows.Forms.Padding(4);
            this.graph1colorPanel.Name = "graph1colorPanel";
            this.graph1colorPanel.Size = new System.Drawing.Size(129, 28);
            this.graph1colorPanel.TabIndex = 5;
            // 
            // rbBlueGraph1
            // 
            this.rbBlueGraph1.AutoSize = true;
            this.rbBlueGraph1.Location = new System.Drawing.Point(63, 4);
            this.rbBlueGraph1.Margin = new System.Windows.Forms.Padding(4);
            this.rbBlueGraph1.Name = "rbBlueGraph1";
            this.rbBlueGraph1.Size = new System.Drawing.Size(57, 21);
            this.rbBlueGraph1.TabIndex = 24;
            this.rbBlueGraph1.TabStop = true;
            this.rbBlueGraph1.Text = "Blue";
            this.rbBlueGraph1.UseVisualStyleBackColor = true;
            this.rbBlueGraph1.CheckedChanged += new System.EventHandler(this.Graph1Color_Changed);
            // 
            // rbRedGraph1
            // 
            this.rbRedGraph1.AutoSize = true;
            this.rbRedGraph1.Location = new System.Drawing.Point(5, 4);
            this.rbRedGraph1.Margin = new System.Windows.Forms.Padding(4);
            this.rbRedGraph1.Name = "rbRedGraph1";
            this.rbRedGraph1.Size = new System.Drawing.Size(55, 21);
            this.rbRedGraph1.TabIndex = 23;
            this.rbRedGraph1.TabStop = true;
            this.rbRedGraph1.Text = "Red";
            this.rbRedGraph1.UseVisualStyleBackColor = true;
            this.rbRedGraph1.CheckedChanged += new System.EventHandler(this.Graph1Color_Changed);
            // 
            // tbMaxScaleX
            // 
            this.tbMaxScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMaxScaleX.Location = new System.Drawing.Point(366, 114);
            this.tbMaxScaleX.Margin = new System.Windows.Forms.Padding(4);
            this.tbMaxScaleX.Name = "tbMaxScaleX";
            this.tbMaxScaleX.Size = new System.Drawing.Size(56, 22);
            this.tbMaxScaleX.TabIndex = 22;
            this.tbMaxScaleX.TextChanged += new System.EventHandler(this.tbMaxScaleX_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 119);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 21;
            this.label1.Text = "x-Axis Max Scale:";
            // 
            // checkBoxUseTimeForXAxis
            // 
            this.checkBoxUseTimeForXAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseTimeForXAxis.AutoSize = true;
            this.checkBoxUseTimeForXAxis.Checked = true;
            this.checkBoxUseTimeForXAxis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseTimeForXAxis.Location = new System.Drawing.Point(720, 57);
            this.checkBoxUseTimeForXAxis.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxUseTimeForXAxis.Name = "checkBoxUseTimeForXAxis";
            this.checkBoxUseTimeForXAxis.Size = new System.Drawing.Size(141, 21);
            this.checkBoxUseTimeForXAxis.TabIndex = 20;
            this.checkBoxUseTimeForXAxis.Text = "Use Time/Frames";
            this.checkBoxUseTimeForXAxis.UseVisualStyleBackColor = true;
            this.checkBoxUseTimeForXAxis.CheckedChanged += new System.EventHandler(this.xAxisTime_CheckedChanged);
            // 
            // checkBoxRollingGraph
            // 
            this.checkBoxRollingGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRollingGraph.AutoSize = true;
            this.checkBoxRollingGraph.Checked = true;
            this.checkBoxRollingGraph.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRollingGraph.Location = new System.Drawing.Point(717, 32);
            this.checkBoxRollingGraph.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxRollingGraph.Name = "checkBoxRollingGraph";
            this.checkBoxRollingGraph.Size = new System.Drawing.Size(117, 21);
            this.checkBoxRollingGraph.TabIndex = 18;
            this.checkBoxRollingGraph.Text = "Rolling Graph";
            this.checkBoxRollingGraph.UseVisualStyleBackColor = true;
            this.checkBoxRollingGraph.CheckedChanged += new System.EventHandler(this.checkBoxRollingGraph_CheckedChanged);
            // 
            // checkBoxAutoScale
            // 
            this.checkBoxAutoScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScale.AutoSize = true;
            this.checkBoxAutoScale.Checked = true;
            this.checkBoxAutoScale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoScale.Location = new System.Drawing.Point(477, 4);
            this.checkBoxAutoScale.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxAutoScale.Name = "checkBoxAutoScale";
            this.checkBoxAutoScale.Size = new System.Drawing.Size(139, 21);
            this.checkBoxAutoScale.TabIndex = 17;
            this.checkBoxAutoScale.Text = "Auto Scale y-Axis";
            this.checkBoxAutoScale.UseVisualStyleBackColor = true;
            this.checkBoxAutoScale.CheckedChanged += new System.EventHandler(this.checkBoxAutoScale_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btn_ymaxIncrease);
            this.groupBox2.Controls.Add(this.btn_ymaxDecrease);
            this.groupBox2.Controls.Add(this.textBox_ymax);
            this.groupBox2.Location = new System.Drawing.Point(557, 26);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(140, 82);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "y-Axis Max";
            // 
            // btn_ymaxIncrease
            // 
            this.btn_ymaxIncrease.Enabled = false;
            this.btn_ymaxIncrease.Location = new System.Drawing.Point(73, 52);
            this.btn_ymaxIncrease.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ymaxIncrease.Name = "btn_ymaxIncrease";
            this.btn_ymaxIncrease.Size = new System.Drawing.Size(49, 28);
            this.btn_ymaxIncrease.TabIndex = 2;
            this.btn_ymaxIncrease.Text = ">>";
            this.btn_ymaxIncrease.UseVisualStyleBackColor = true;
            this.btn_ymaxIncrease.Click += new System.EventHandler(this.btn_ymaxIncrease_Click);
            // 
            // btn_ymaxDecrease
            // 
            this.btn_ymaxDecrease.Enabled = false;
            this.btn_ymaxDecrease.Location = new System.Drawing.Point(16, 52);
            this.btn_ymaxDecrease.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ymaxDecrease.Name = "btn_ymaxDecrease";
            this.btn_ymaxDecrease.Size = new System.Drawing.Size(49, 28);
            this.btn_ymaxDecrease.TabIndex = 1;
            this.btn_ymaxDecrease.Text = "<<";
            this.btn_ymaxDecrease.UseVisualStyleBackColor = true;
            this.btn_ymaxDecrease.Click += new System.EventHandler(this.btn_ymaxDecrease_Click);
            // 
            // textBox_ymax
            // 
            this.textBox_ymax.Enabled = false;
            this.textBox_ymax.Location = new System.Drawing.Point(16, 20);
            this.textBox_ymax.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ymax.Name = "textBox_ymax";
            this.textBox_ymax.Size = new System.Drawing.Size(105, 22);
            this.textBox_ymax.TabIndex = 0;
            this.textBox_ymax.TextChanged += new System.EventHandler(this.textBox_ymax_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btn_yminIncrease);
            this.groupBox1.Controls.Add(this.btn_yminDecrease);
            this.groupBox1.Controls.Add(this.textBox_ymin);
            this.groupBox1.Location = new System.Drawing.Point(394, 26);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(149, 82);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "y-Axis Min";
            // 
            // btn_yminIncrease
            // 
            this.btn_yminIncrease.Enabled = false;
            this.btn_yminIncrease.Location = new System.Drawing.Point(73, 49);
            this.btn_yminIncrease.Margin = new System.Windows.Forms.Padding(4);
            this.btn_yminIncrease.Name = "btn_yminIncrease";
            this.btn_yminIncrease.Size = new System.Drawing.Size(49, 28);
            this.btn_yminIncrease.TabIndex = 2;
            this.btn_yminIncrease.Text = ">>";
            this.btn_yminIncrease.UseVisualStyleBackColor = true;
            this.btn_yminIncrease.Click += new System.EventHandler(this.btn_yminIncrease_Click);
            // 
            // btn_yminDecrease
            // 
            this.btn_yminDecrease.Enabled = false;
            this.btn_yminDecrease.Location = new System.Drawing.Point(16, 49);
            this.btn_yminDecrease.Margin = new System.Windows.Forms.Padding(4);
            this.btn_yminDecrease.Name = "btn_yminDecrease";
            this.btn_yminDecrease.Size = new System.Drawing.Size(49, 28);
            this.btn_yminDecrease.TabIndex = 1;
            this.btn_yminDecrease.Text = "<<";
            this.btn_yminDecrease.UseVisualStyleBackColor = true;
            this.btn_yminDecrease.Click += new System.EventHandler(this.btn_yminDecrease_Click);
            // 
            // textBox_ymin
            // 
            this.textBox_ymin.Enabled = false;
            this.textBox_ymin.Location = new System.Drawing.Point(16, 18);
            this.textBox_ymin.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ymin.Name = "textBox_ymin";
            this.textBox_ymin.Size = new System.Drawing.Size(105, 22);
            this.textBox_ymin.TabIndex = 0;
            this.textBox_ymin.TextChanged += new System.EventHandler(this.textBox_ymin_TextChanged);
            // 
            // tbGraph2
            // 
            this.tbGraph2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGraph2.Location = new System.Drawing.Point(83, 81);
            this.tbGraph2.Margin = new System.Windows.Forms.Padding(4);
            this.tbGraph2.Name = "tbGraph2";
            this.tbGraph2.Size = new System.Drawing.Size(151, 22);
            this.tbGraph2.TabIndex = 14;
            this.tbGraph2.TextChanged += new System.EventHandler(this.tbGraph2_TextChanged);
            // 
            // tbGraph1
            // 
            this.tbGraph1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGraph1.Location = new System.Drawing.Point(83, 46);
            this.tbGraph1.Margin = new System.Windows.Forms.Padding(4);
            this.tbGraph1.Name = "tbGraph1";
            this.tbGraph1.Size = new System.Drawing.Size(151, 22);
            this.tbGraph1.TabIndex = 13;
            this.tbGraph1.TextChanged += new System.EventHandler(this.tbGraph1_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 86);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Graph 2:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 49);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Graph 1:";
            // 
            // tbTickMaxX
            // 
            this.tbTickMaxX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTickMaxX.Location = new System.Drawing.Point(368, 146);
            this.tbTickMaxX.Margin = new System.Windows.Forms.Padding(4);
            this.tbTickMaxX.Name = "tbTickMaxX";
            this.tbTickMaxX.Size = new System.Drawing.Size(53, 22);
            this.tbTickMaxX.TabIndex = 10;
            this.tbTickMaxX.TextChanged += new System.EventHandler(this.tbTickMax_TextChanged);
            // 
            // btClean
            // 
            this.btClean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClean.Location = new System.Drawing.Point(715, 80);
            this.btClean.Margin = new System.Windows.Forms.Padding(4);
            this.btClean.Name = "btClean";
            this.btClean.Size = new System.Drawing.Size(164, 28);
            this.btClean.TabIndex = 3;
            this.btClean.Text = "Clean";
            this.btClean.UseVisualStyleBackColor = true;
            this.btClean.Click += new System.EventHandler(this.btClean_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 151);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "x-Axis Max Tick:";
            // 
            // tbTickMinX
            // 
            this.tbTickMinX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTickMinX.Location = new System.Drawing.Point(183, 146);
            this.tbTickMinX.Margin = new System.Windows.Forms.Padding(4);
            this.tbTickMinX.Name = "tbTickMinX";
            this.tbTickMinX.Size = new System.Drawing.Size(51, 22);
            this.tbTickMinX.TabIndex = 8;
            this.tbTickMinX.TextChanged += new System.EventHandler(this.tbTickMin_TextChanged);
            // 
            // tbyaxislabel
            // 
            this.tbyaxislabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbyaxislabel.Location = new System.Drawing.Point(115, 116);
            this.tbyaxislabel.Margin = new System.Windows.Forms.Padding(4);
            this.tbyaxislabel.Name = "tbyaxislabel";
            this.tbyaxislabel.Size = new System.Drawing.Size(119, 22);
            this.tbyaxislabel.TabIndex = 4;
            this.tbyaxislabel.TextChanged += new System.EventHandler(this.tbyaxislabel_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Title:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 151);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "x-Axis Min Tick:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 119);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "y-Axis Label:";
            // 
            // tbTitle
            // 
            this.tbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTitle.Location = new System.Drawing.Point(51, 9);
            this.tbTitle.Margin = new System.Windows.Forms.Padding(4);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(321, 22);
            this.tbTitle.TabIndex = 6;
            this.tbTitle.TextChanged += new System.EventHandler(this.tbTitle_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnFailureEvent);
            this.panel2.Controls.Add(this.btnStartStopQuestioning);
            this.panel2.Controls.Add(this.checkBoxBiofeedback);
            this.panel2.Controls.Add(this.btHide);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 229);
            this.panel2.TabIndex = 2;
            // 
            // btnFailureEvent
            // 
            this.btnFailureEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFailureEvent.Location = new System.Drawing.Point(674, 197);
            this.btnFailureEvent.Name = "btnFailureEvent";
            this.btnFailureEvent.Size = new System.Drawing.Size(160, 27);
            this.btnFailureEvent.TabIndex = 6;
            this.btnFailureEvent.Text = "Failure Event";
            this.btnFailureEvent.UseVisualStyleBackColor = true;
            this.btnFailureEvent.Visible = false;
            this.btnFailureEvent.Click += new System.EventHandler(this.btnFailureEvent_Click);
            // 
            // btnStartStopQuestioning
            // 
            this.btnStartStopQuestioning.Enabled = false;
            this.btnStartStopQuestioning.Location = new System.Drawing.Point(129, 195);
            this.btnStartStopQuestioning.Name = "btnStartStopQuestioning";
            this.btnStartStopQuestioning.Size = new System.Drawing.Size(160, 27);
            this.btnStartStopQuestioning.TabIndex = 3;
            this.btnStartStopQuestioning.Text = "Start Stimulus";
            this.btnStartStopQuestioning.UseVisualStyleBackColor = true;
            this.btnStartStopQuestioning.Click += new System.EventHandler(this.btnStartStopQuestioning_Click);
            // 
            // checkBoxBiofeedback
            // 
            this.checkBoxBiofeedback.AutoSize = true;
            this.checkBoxBiofeedback.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.checkBoxBiofeedback.Enabled = false;
            this.checkBoxBiofeedback.Location = new System.Drawing.Point(15, 199);
            this.checkBoxBiofeedback.Name = "checkBoxBiofeedback";
            this.checkBoxBiofeedback.Size = new System.Drawing.Size(108, 21);
            this.checkBoxBiofeedback.TabIndex = 5;
            this.checkBoxBiofeedback.Text = "Biofeedback";
            this.checkBoxBiofeedback.UseVisualStyleBackColor = false;
            this.checkBoxBiofeedback.CheckedChanged += new System.EventHandler(this.checkBoxBiofeedback_CheckedChanged);
            // 
            // btHide
            // 
            this.btHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btHide.BackColor = System.Drawing.Color.White;
            this.btHide.FlatAppearance.BorderSize = 0;
            this.btHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btHide.Image = global::RealTimeGraph.Properties.Resources.togglebutton;
            this.btHide.Location = new System.Drawing.Point(852, 206);
            this.btHide.Margin = new System.Windows.Forms.Padding(4);
            this.btHide.Name = "btHide";
            this.btHide.Size = new System.Drawing.Size(36, 18);
            this.btHide.TabIndex = 4;
            this.btHide.UseVisualStyleBackColor = false;
            this.btHide.Click += new System.EventHandler(this.btHide_Click);
            // 
            // RealTimeGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.settingsPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RealTimeGraphControl";
            this.Size = new System.Drawing.Size(900, 412);
            this.Load += new System.EventHandler(this.RealTimeGraphControl_Load);
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.graph2colorPanel.ResumeLayout(false);
            this.graph2colorPanel.PerformLayout();
            this.graph1colorPanel.ResumeLayout(false);
            this.graph1colorPanel.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.TextBox tbTickMinX;
        private System.Windows.Forms.TextBox tbyaxislabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbTickMaxX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btClean;
        private System.Windows.Forms.Button btHide;
        private System.Windows.Forms.TextBox tbGraph2;
        private System.Windows.Forms.TextBox tbGraph1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_yminIncrease;
        private System.Windows.Forms.Button btn_yminDecrease;
        private System.Windows.Forms.TextBox textBox_ymin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_ymaxIncrease;
        private System.Windows.Forms.Button btn_ymaxDecrease;
        private System.Windows.Forms.TextBox textBox_ymax;
        private System.Windows.Forms.CheckBox checkBoxAutoScale;
        private System.Windows.Forms.CheckBox checkBoxRollingGraph;
        private System.Windows.Forms.CheckBox checkBoxUseTimeForXAxis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMaxScaleX;
        private System.Windows.Forms.RadioButton rbRedGraph1;
        private System.Windows.Forms.Panel graph1colorPanel;
        private System.Windows.Forms.RadioButton rbBlueGraph1;
        private System.Windows.Forms.Panel graph2colorPanel;
        private System.Windows.Forms.RadioButton rbBlueGraph2;
        private System.Windows.Forms.RadioButton rbRedGraph2;
        private System.Windows.Forms.Button btnStartStopQuestioning;
        private System.Windows.Forms.CheckBox checkBoxBiofeedback;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioBiofeedbackAdv1;
        private System.Windows.Forms.RadioButton radioBiofeedbackBasic;
        private System.Windows.Forms.RadioButton radioBiofeedbackSelfStart2;
        private System.Windows.Forms.RadioButton radioBiofeedbackSelfStart1;
        private System.Windows.Forms.Button btnFailureEvent;
    }
}
