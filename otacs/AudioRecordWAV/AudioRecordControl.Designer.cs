namespace AudioRecordWAV
{
    partial class AudioRecordControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboWasapiDevices = new System.Windows.Forms.ComboBox();
            this.btnRecordStartStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Audio Device:";
            this.label1.Visible = false;
            // 
            // comboWasapiDevices
            // 
            this.comboWasapiDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboWasapiDevices.FormattingEnabled = true;
            this.comboWasapiDevices.Location = new System.Drawing.Point(110, 59);
            this.comboWasapiDevices.Name = "comboWasapiDevices";
            this.comboWasapiDevices.Size = new System.Drawing.Size(195, 24);
            this.comboWasapiDevices.TabIndex = 1;
            this.comboWasapiDevices.Visible = false;
            this.comboWasapiDevices.SelectedIndexChanged += new System.EventHandler(this.comboWasapiDevices_SelectedIndexChanged);
            // 
            // btnRecordStartStop
            // 
            this.btnRecordStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecordStartStop.Location = new System.Drawing.Point(10, 12);
            this.btnRecordStartStop.Name = "btnRecordStartStop";
            this.btnRecordStartStop.Size = new System.Drawing.Size(288, 31);
            this.btnRecordStartStop.TabIndex = 2;
            this.btnRecordStartStop.Text = "Start Recording";
            this.btnRecordStartStop.UseVisualStyleBackColor = true;
            this.btnRecordStartStop.Click += new System.EventHandler(this.btnRecordStartStop_Click);
            // 
            // AudioRecordControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRecordStartStop);
            this.Controls.Add(this.comboWasapiDevices);
            this.Controls.Add(this.label1);
            this.Name = "AudioRecordControl";
            this.Size = new System.Drawing.Size(312, 53);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboWasapiDevices;
        private System.Windows.Forms.Button btnRecordStartStop;
    }
}
