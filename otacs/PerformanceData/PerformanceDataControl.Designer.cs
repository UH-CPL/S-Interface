namespace PerformanceData
{
    partial class PerformanceDataControl
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
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStartStopRecording = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.timerCAN = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Enabled = false;
            this.textBoxFileName.Location = new System.Drawing.Point(87, 11);
            this.textBoxFileName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBoxFileName.Size = new System.Drawing.Size(267, 22);
            this.textBoxFileName.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "Filename:";
            // 
            // btnStartStopRecording
            // 
            this.btnStartStopRecording.Location = new System.Drawing.Point(19, 170);
            this.btnStartStopRecording.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStopRecording.Name = "btnStartStopRecording";
            this.btnStartStopRecording.Size = new System.Drawing.Size(335, 33);
            this.btnStartStopRecording.TabIndex = 16;
            this.btnStartStopRecording.Text = "Start/Stop Recording";
            this.btnStartStopRecording.UseVisualStyleBackColor = true;
            this.btnStartStopRecording.Click += new System.EventHandler(this.btnStartStopRecording_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 17);
            this.label3.TabIndex = 15;
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Location = new System.Drawing.Point(19, 53);
            this.textBoxDisplay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxDisplay.Multiline = true;
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.Size = new System.Drawing.Size(335, 102);
            this.textBoxDisplay.TabIndex = 19;
            // 
            // timerCAN
            // 
            this.timerCAN.Interval = 1000;
            this.timerCAN.Tick += new System.EventHandler(this.timerCAN_Tick);
            // 
            // PerformanceDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnStartStopRecording);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PerformanceDataControl";
            this.Size = new System.Drawing.Size(371, 214);
            this.Leave += new System.EventHandler(this.PerformanceDataControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStartStopRecording;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.Timer timerCAN;
    }
}
