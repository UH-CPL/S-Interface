namespace OTACS
{
    partial class frmOTACS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnPlayPauseCycle = new System.Windows.Forms.Button();
            this.panelPlugin1 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnNewConfig = new System.Windows.Forms.Button();
            this.pluginComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControlOTACS = new System.Windows.Forms.TabControl();
            this.PluginGraph = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbBoxConfigs = new System.Windows.Forms.ComboBox();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.panelPluginGraph = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GUI = new System.Windows.Forms.TabPage();
            this.btnShowHideDockPins = new System.Windows.Forms.Button();
            this.timerCyclesPerSec = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControlOTACS.SuspendLayout();
            this.PluginGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.GUI.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlayPauseCycle
            // 
            this.btnPlayPauseCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayPauseCycle.Location = new System.Drawing.Point(9, 766);
            this.btnPlayPauseCycle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPlayPauseCycle.Name = "btnPlayPauseCycle";
            this.btnPlayPauseCycle.Size = new System.Drawing.Size(74, 35);
            this.btnPlayPauseCycle.TabIndex = 0;
            this.btnPlayPauseCycle.Text = "Start";
            this.btnPlayPauseCycle.UseVisualStyleBackColor = true;
            this.btnPlayPauseCycle.Click += new System.EventHandler(this.btnPlayPauseCycle_Click);
            // 
            // panelPlugin1
            // 
            this.panelPlugin1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlugin1.AutoScroll = true;
            this.panelPlugin1.Location = new System.Drawing.Point(14, 29);
            this.panelPlugin1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelPlugin1.Name = "panelPlugin1";
            this.panelPlugin1.Size = new System.Drawing.Size(1287, 708);
            this.panelPlugin1.TabIndex = 3;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(1163, 6);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(174, 35);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnNewConfig);
            this.groupBox1.Controls.Add(this.pluginComboBox);
            this.groupBox1.Location = new System.Drawing.Point(9, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(362, 80);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create Configuration";
            // 
            // btnNewConfig
            // 
            this.btnNewConfig.Location = new System.Drawing.Point(15, 28);
            this.btnNewConfig.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNewConfig.Name = "btnNewConfig";
            this.btnNewConfig.Size = new System.Drawing.Size(112, 35);
            this.btnNewConfig.TabIndex = 2;
            this.btnNewConfig.Text = "New";
            this.btnNewConfig.UseVisualStyleBackColor = true;
            this.btnNewConfig.Click += new System.EventHandler(this.btnNewConfig_Click);
            // 
            // pluginComboBox
            // 
            this.pluginComboBox.FormattingEnabled = true;
            this.pluginComboBox.Location = new System.Drawing.Point(136, 29);
            this.pluginComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pluginComboBox.Name = "pluginComboBox";
            this.pluginComboBox.Size = new System.Drawing.Size(214, 28);
            this.pluginComboBox.TabIndex = 10;
            this.pluginComboBox.SelectedValueChanged += new System.EventHandler(this.pluginComboBox_SelectedValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.panelPlugin1);
            this.groupBox3.Location = new System.Drawing.Point(9, 9);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(1309, 746);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            // 
            // tabControlOTACS
            // 
            this.tabControlOTACS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlOTACS.Controls.Add(this.PluginGraph);
            this.tabControlOTACS.Controls.Add(this.GUI);
            this.tabControlOTACS.Location = new System.Drawing.Point(18, 18);
            this.tabControlOTACS.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControlOTACS.Name = "tabControlOTACS";
            this.tabControlOTACS.SelectedIndex = 0;
            this.tabControlOTACS.Size = new System.Drawing.Size(1334, 848);
            this.tabControlOTACS.TabIndex = 0;
            // 
            // PluginGraph
            // 
            this.PluginGraph.Controls.Add(this.pictureBox1);
            this.PluginGraph.Controls.Add(this.groupBox4);
            this.PluginGraph.Controls.Add(this.panelPluginGraph);
            this.PluginGraph.Controls.Add(this.groupBox2);
            this.PluginGraph.Controls.Add(this.groupBox1);
            this.PluginGraph.Location = new System.Drawing.Point(4, 29);
            this.PluginGraph.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PluginGraph.Name = "PluginGraph";
            this.PluginGraph.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PluginGraph.Size = new System.Drawing.Size(1326, 815);
            this.PluginGraph.TabIndex = 1;
            this.PluginGraph.Text = "Plugin Graph";
            this.PluginGraph.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::OTACS.Properties.Resources.About1;
            this.pictureBox1.Location = new System.Drawing.Point(1094, 9);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(216, 85);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbBoxConfigs);
            this.groupBox4.Controls.Add(this.btnSaveConfig);
            this.groupBox4.Location = new System.Drawing.Point(380, 11);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(492, 80);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Load/Save Configuration";
            // 
            // cmbBoxConfigs
            // 
            this.cmbBoxConfigs.FormattingEnabled = true;
            this.cmbBoxConfigs.Location = new System.Drawing.Point(9, 29);
            this.cmbBoxConfigs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbBoxConfigs.Name = "cmbBoxConfigs";
            this.cmbBoxConfigs.Size = new System.Drawing.Size(314, 28);
            this.cmbBoxConfigs.TabIndex = 12;
            this.cmbBoxConfigs.SelectedIndexChanged += new System.EventHandler(this.cmbBoxConfigs_SelectedIndexChanged);
            this.cmbBoxConfigs.Click += new System.EventHandler(this.cmbBoxConfigs_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(334, 26);
            this.btnSaveConfig.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(148, 35);
            this.btnSaveConfig.TabIndex = 1;
            this.btnSaveConfig.Text = "Save";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // panelPluginGraph
            // 
            this.panelPluginGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPluginGraph.AutoScroll = true;
            this.panelPluginGraph.Location = new System.Drawing.Point(18, 125);
            this.panelPluginGraph.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelPluginGraph.Name = "panelPluginGraph";
            this.panelPluginGraph.Size = new System.Drawing.Size(1289, 665);
            this.panelPluginGraph.TabIndex = 8;
            this.panelPluginGraph.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panelPluginGraph_Scroll);
            this.panelPluginGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPluginGraph_Paint);
            this.panelPluginGraph.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelPluginGraph_MouseClick);
            this.panelPluginGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelPluginGraph_MouseDown);
            this.panelPluginGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelPluginGraph_MouseMove);
            this.panelPluginGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelPluginGraph_MouseUp);
            this.panelPluginGraph.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panelPluginGraph_PreviewKeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(9, 100);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(1306, 698);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Graph";
            // 
            // GUI
            // 
            this.GUI.Controls.Add(this.btnShowHideDockPins);
            this.GUI.Controls.Add(this.btnPlayPauseCycle);
            this.GUI.Controls.Add(this.groupBox3);
            this.GUI.Location = new System.Drawing.Point(4, 29);
            this.GUI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GUI.Name = "GUI";
            this.GUI.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GUI.Size = new System.Drawing.Size(1326, 815);
            this.GUI.TabIndex = 0;
            this.GUI.Text = "User Interface";
            this.GUI.UseVisualStyleBackColor = true;
            // 
            // btnShowHideDockPins
            // 
            this.btnShowHideDockPins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowHideDockPins.Location = new System.Drawing.Point(92, 766);
            this.btnShowHideDockPins.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowHideDockPins.Name = "btnShowHideDockPins";
            this.btnShowHideDockPins.Size = new System.Drawing.Size(207, 35);
            this.btnShowHideDockPins.TabIndex = 0;
            this.btnShowHideDockPins.Text = "Hide All Dock Pins";
            this.btnShowHideDockPins.UseVisualStyleBackColor = true;
            this.btnShowHideDockPins.Click += new System.EventHandler(this.btnShowHideDockPins_Click);
            // 
            // timerCyclesPerSec
            // 
            this.timerCyclesPerSec.Interval = 1000;
            this.timerCyclesPerSec.Tick += new System.EventHandler(this.timerCyclesPerSec_Tick);
            // 
            // frmOTACS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 918);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tabControlOTACS);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmOTACS";
            this.Text = "S-Interface (previously OTACS)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOTACS_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmOTACS_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabControlOTACS.ResumeLayout(false);
            this.PluginGraph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.GUI.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnPlayPauseCycle;
        private System.Windows.Forms.Panel panelPlugin1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabControlOTACS;
        private System.Windows.Forms.TabPage GUI;
        private System.Windows.Forms.TabPage PluginGraph;
        private System.Windows.Forms.Panel panelPluginGraph;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox pluginComboBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnShowHideDockPins;
        private System.Windows.Forms.Button btnNewConfig;
        private System.Windows.Forms.Timer timerCyclesPerSec;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbBoxConfigs;
    }
}

