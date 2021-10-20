namespace Thermal_File
{
    partial class Thermal_File_Control
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
            this.lblLength = new System.Windows.Forms.Label();
            this.lblTotalFrames = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.trackBarThermalVideo = new System.Windows.Forms.TrackBar();
            this.lblCurTimeStamp = new System.Windows.Forms.Label();
            this.lblCurFrameNum = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFrame = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThermalVideo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLength.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLength.Location = new System.Drawing.Point(231, 33);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(104, 23);
            this.lblLength.TabIndex = 35;
            this.lblLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalFrames
            // 
            this.lblTotalFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalFrames.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTotalFrames.Location = new System.Drawing.Point(84, 33);
            this.lblTotalFrames.Name = "lblTotalFrames";
            this.lblTotalFrames.Size = new System.Drawing.Size(82, 23);
            this.lblTotalFrames.TabIndex = 34;
            this.lblTotalFrames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(182, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Length:";
            // 
            // btnFirst
            // 
            this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirst.Location = new System.Drawing.Point(16, 119);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(55, 23);
            this.btnFirst.TabIndex = 31;
            this.btnFirst.Text = "<<";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // lblSeconds
            // 
            this.lblSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(286, 65);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblSeconds.TabIndex = 36;
            this.lblSeconds.Text = "seconds";
            this.lblSeconds.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Total Frames:";
            // 
            // btnLast
            // 
            this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLast.Location = new System.Drawing.Point(275, 119);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(60, 23);
            this.btnLast.TabIndex = 30;
            this.btnLast.Text = ">>";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(209, 119);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(60, 23);
            this.btnNext.TabIndex = 29;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlayPause.Location = new System.Drawing.Point(143, 119);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(60, 23);
            this.btnPlayPause.TabIndex = 28;
            this.btnPlayPause.Text = "|> ||";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrev.Location = new System.Drawing.Point(77, 119);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(60, 23);
            this.btnPrev.TabIndex = 27;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // trackBarThermalVideo
            // 
            this.trackBarThermalVideo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarThermalVideo.AutoSize = false;
            this.trackBarThermalVideo.Location = new System.Drawing.Point(5, 89);
            this.trackBarThermalVideo.Name = "trackBarThermalVideo";
            this.trackBarThermalVideo.Size = new System.Drawing.Size(330, 45);
            this.trackBarThermalVideo.TabIndex = 26;
            this.trackBarThermalVideo.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarThermalVideo.Scroll += new System.EventHandler(this.trackBarThermalVideo_Scroll);
            // 
            // lblCurTimeStamp
            // 
            this.lblCurTimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurTimeStamp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurTimeStamp.Location = new System.Drawing.Point(231, 61);
            this.lblCurTimeStamp.Name = "lblCurTimeStamp";
            this.lblCurTimeStamp.Size = new System.Drawing.Size(104, 23);
            this.lblCurTimeStamp.TabIndex = 25;
            this.lblCurTimeStamp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurFrameNum
            // 
            this.lblCurFrameNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurFrameNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurFrameNum.Location = new System.Drawing.Point(84, 61);
            this.lblCurFrameNum.Name = "lblCurFrameNum";
            this.lblCurFrameNum.Size = new System.Drawing.Size(82, 23);
            this.lblCurFrameNum.TabIndex = 24;
            this.lblCurFrameNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Offset:";
            // 
            // lblFrame
            // 
            this.lblFrame.AutoSize = true;
            this.lblFrame.Location = new System.Drawing.Point(6, 65);
            this.lblFrame.Name = "lblFrame";
            this.lblFrame.Size = new System.Drawing.Size(79, 13);
            this.lblFrame.TabIndex = 22;
            this.lblFrame.Text = "Frame Number:";
            // 
            // lblFileName
            // 
            this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFileName.Location = new System.Drawing.Point(84, 4);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(202, 23);
            this.lblFileName.TabIndex = 21;
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Thermal File:";
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(292, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(43, 23);
            this.btnOpen.TabIndex = 19;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // Thermal_File_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLength);
            this.Controls.Add(this.lblTotalFrames);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.lblSeconds);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPlayPause);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.trackBarThermalVideo);
            this.Controls.Add(this.lblCurTimeStamp);
            this.Controls.Add(this.lblCurFrameNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFrame);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpen);
            this.Name = "Thermal_File_Control";
            this.Size = new System.Drawing.Size(346, 151);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThermalVideo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Label lblTotalFrames;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.TrackBar trackBarThermalVideo;
        private System.Windows.Forms.Label lblCurTimeStamp;
        private System.Windows.Forms.Label lblCurFrameNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFrame;
        public System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpen;

    }
}
