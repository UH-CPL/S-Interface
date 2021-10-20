namespace VideoCameraPlugin
{
    partial class VideoCameraControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelCameraTitle = new System.Windows.Forms.Label();
            this.comboBoxCameraList = new System.Windows.Forms.ComboBox();
            this.timerRecord = new System.Windows.Forms.Timer(this.components);
            this.btnStartStopRecord = new System.Windows.Forms.Button();
            this.btnCameraSettings = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFrameRate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxAVINumber = new System.Windows.Forms.ComboBox();
            this.lblFramesLastSec = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblRecordTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCameraTitle
            // 
            this.labelCameraTitle.AutoSize = true;
            this.labelCameraTitle.Location = new System.Drawing.Point(13, 21);
            this.labelCameraTitle.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this.labelCameraTitle.Name = "labelCameraTitle";
            this.labelCameraTitle.Size = new System.Drawing.Size(69, 20);
            this.labelCameraTitle.TabIndex = 18;
            this.labelCameraTitle.Text = "Camera:";
            // 
            // comboBoxCameraList
            // 
            this.comboBoxCameraList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCameraList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCameraList.FormattingEnabled = true;
            this.comboBoxCameraList.Location = new System.Drawing.Point(82, 18);
            this.comboBoxCameraList.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxCameraList.Name = "comboBoxCameraList";
            this.comboBoxCameraList.Size = new System.Drawing.Size(197, 28);
            this.comboBoxCameraList.TabIndex = 17;
            this.comboBoxCameraList.SelectedIndexChanged += new System.EventHandler(this.comboBoxCameraList_SelectedIndexChanged);
            // 
            // timerRecord
            // 
            this.timerRecord.Interval = 1000;
            this.timerRecord.Tick += new System.EventHandler(this.timerRecord_Tick);
            // 
            // btnStartStopRecord
            // 
            this.btnStartStopRecord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStopRecord.Location = new System.Drawing.Point(16, 382);
            this.btnStartStopRecord.Name = "btnStartStopRecord";
            this.btnStartStopRecord.Size = new System.Drawing.Size(354, 28);
            this.btnStartStopRecord.TabIndex = 22;
            this.btnStartStopRecord.Text = "Start Recording";
            this.btnStartStopRecord.UseVisualStyleBackColor = true;
            this.btnStartStopRecord.Click += new System.EventHandler(this.btnStartStopRecord_Click);
            // 
            // btnCameraSettings
            // 
            this.btnCameraSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCameraSettings.Location = new System.Drawing.Point(286, 16);
            this.btnCameraSettings.Name = "btnCameraSettings";
            this.btnCameraSettings.Size = new System.Drawing.Size(84, 27);
            this.btnCameraSettings.TabIndex = 23;
            this.btnCameraSettings.Text = "Settings";
            this.btnCameraSettings.UseVisualStyleBackColor = true;
            this.btnCameraSettings.Click += new System.EventHandler(this.btnCameraSettings_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox1.Location = new System.Drawing.Point(16, 61);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(354, 284);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 26;
            this.label2.Text = "Frame Rate:";
            // 
            // lblFrameRate
            // 
            this.lblFrameRate.Location = new System.Drawing.Point(113, 354);
            this.lblFrameRate.Name = "lblFrameRate";
            this.lblFrameRate.Size = new System.Drawing.Size(75, 22);
            this.lblFrameRate.TabIndex = 27;
            this.lblFrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 356);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 20);
            this.label3.TabIndex = 29;
            this.label3.Text = "AVI #:";
            // 
            // comboBoxAVINumber
            // 
            this.comboBoxAVINumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAVINumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAVINumber.FormattingEnabled = true;
            this.comboBoxAVINumber.Items.AddRange(new object[] {
            "avi1",
            "avi2",
            "avi3",
            "avi4",
            "avi5"});
            this.comboBoxAVINumber.Location = new System.Drawing.Point(286, 353);
            this.comboBoxAVINumber.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxAVINumber.Name = "comboBoxAVINumber";
            this.comboBoxAVINumber.Size = new System.Drawing.Size(84, 28);
            this.comboBoxAVINumber.TabIndex = 28;
            this.comboBoxAVINumber.SelectedIndexChanged += new System.EventHandler(this.comboBoxAVINumber_SelectedIndexChanged);
            // 
            // lblFramesLastSec
            // 
            this.lblFramesLastSec.Location = new System.Drawing.Point(205, 417);
            this.lblFramesLastSec.Name = "lblFramesLastSec";
            this.lblFramesLastSec.Size = new System.Drawing.Size(165, 27);
            this.lblFramesLastSec.TabIndex = 31;
            this.lblFramesLastSec.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(14, 419);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(190, 20);
            this.label4.TabIndex = 30;
            this.label4.Text = "Frames acquired last sec:";
            // 
            // lblRecordTime
            // 
            this.lblRecordTime.Location = new System.Drawing.Point(181, 452);
            this.lblRecordTime.Name = "lblRecordTime";
            this.lblRecordTime.Size = new System.Drawing.Size(165, 27);
            this.lblRecordTime.TabIndex = 33;
            this.lblRecordTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(14, 456);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 21);
            this.label5.TabIndex = 32;
            this.label5.Text = "Record time (in secs):";
            // 
            // VideoCameraControl
            // 
            this.Controls.Add(this.lblRecordTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblFramesLastSec);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxAVINumber);
            this.Controls.Add(this.lblFrameRate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCameraSettings);
            this.Controls.Add(this.btnStartStopRecord);
            this.Controls.Add(this.labelCameraTitle);
            this.Controls.Add(this.comboBoxCameraList);
            this.Name = "VideoCameraControl";
            this.Size = new System.Drawing.Size(387, 493);
            this.Load += new System.EventHandler(this.VideoCameraControl_Load);
            this.Leave += new System.EventHandler(this.VideoCameraControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        #endregion

        private System.Windows.Forms.Label labelCameraTitle;
        private System.Windows.Forms.ComboBox comboBoxCameraList;
        private System.Windows.Forms.Timer timerRecord;
        private System.Windows.Forms.Button btnStartStopRecord;
        private System.Windows.Forms.Button btnCameraSettings;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFrameRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxAVINumber;
        private System.Windows.Forms.Label lblFramesLastSec;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblRecordTime;
        private System.Windows.Forms.Label label5;
    }
}
