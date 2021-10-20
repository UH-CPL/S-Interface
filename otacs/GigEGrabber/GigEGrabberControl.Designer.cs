namespace GigEGrabber
{
    partial class GigEGrabberControl
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
            this.btnUpdateDevice = new System.Windows.Forms.Button();
            this.lblCurrentInterface = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStartStopRecording = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowseFolderPath = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFrameNum = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnUpdateDevice
            // 
            this.btnUpdateDevice.Location = new System.Drawing.Point(29, 12);
            this.btnUpdateDevice.Name = "btnUpdateDevice";
            this.btnUpdateDevice.Size = new System.Drawing.Size(304, 33);
            this.btnUpdateDevice.TabIndex = 0;
            this.btnUpdateDevice.Text = "Select/Update Interface";
            this.btnUpdateDevice.UseVisualStyleBackColor = true;
            this.btnUpdateDevice.Click += new System.EventHandler(this.btnUpdateDevice_Click);
            // 
            // lblCurrentInterface
            // 
            this.lblCurrentInterface.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.lblCurrentInterface.Location = new System.Drawing.Point(83, 55);
            this.lblCurrentInterface.Name = "lblCurrentInterface";
            this.lblCurrentInterface.Size = new System.Drawing.Size(237, 40);
            this.lblCurrentInterface.TabIndex = 2;
            this.lblCurrentInterface.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Interface:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(86, 143);
            this.textBoxFileName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(267, 22);
            this.textBoxFileName.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 146);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Filename:";
            // 
            // btnStartStopRecording
            // 
            this.btnStartStopRecording.Location = new System.Drawing.Point(29, 178);
            this.btnStartStopRecording.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStopRecording.Name = "btnStartStopRecording";
            this.btnStartStopRecording.Size = new System.Drawing.Size(304, 33);
            this.btnStartStopRecording.TabIndex = 10;
            this.btnStartStopRecording.Text = "Start/Stop Recording";
            this.btnStartStopRecording.UseVisualStyleBackColor = true;
            this.btnStartStopRecording.Click += new System.EventHandler(this.btnStartStopRecording_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 109);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Path:";
            // 
            // btnBrowseFolderPath
            // 
            this.btnBrowseFolderPath.Location = new System.Drawing.Point(266, 103);
            this.btnBrowseFolderPath.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseFolderPath.Name = "btnBrowseFolderPath";
            this.btnBrowseFolderPath.Size = new System.Drawing.Size(87, 28);
            this.btnBrowseFolderPath.TabIndex = 8;
            this.btnBrowseFolderPath.Text = "Browse...";
            this.btnBrowseFolderPath.UseVisualStyleBackColor = true;
            this.btnBrowseFolderPath.Click += new System.EventHandler(this.btnBrowseFolderPath_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(58, 106);
            this.textBoxPath.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(200, 22);
            this.textBoxPath.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 232);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Frame#:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(142, 232);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Timestamp:";
            // 
            // lblFrameNum
            // 
            this.lblFrameNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFrameNum.Location = new System.Drawing.Point(74, 232);
            this.lblFrameNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrameNum.Name = "lblFrameNum";
            this.lblFrameNum.Size = new System.Drawing.Size(60, 22);
            this.lblFrameNum.TabIndex = 16;
            // 
            // lblTimestamp
            // 
            this.lblTimestamp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTimestamp.Location = new System.Drawing.Point(221, 232);
            this.lblTimestamp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(132, 22);
            this.lblTimestamp.TabIndex = 17;
            // 
            // GigEGrabberControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCurrentInterface);
            this.Controls.Add(this.lblTimestamp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFrameNum);
            this.Controls.Add(this.btnUpdateDevice);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnStartStopRecording);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBrowseFolderPath);
            this.Controls.Add(this.textBoxPath);
            this.Name = "GigEGrabberControl";
            this.Size = new System.Drawing.Size(365, 268);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdateDevice;
        private System.Windows.Forms.Label lblCurrentInterface;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStartStopRecording;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseFolderPath;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFrameNum;
        private System.Windows.Forms.Label lblTimestamp;
    }
}
