namespace ProcessInitiator
{
    partial class ProcessControl
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
            this.buttonstopInitiator = new System.Windows.Forms.Button();
            this.buttonStartStopInitiator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonstopInitiator
            // 
            this.buttonstopInitiator.Enabled = false;
            this.buttonstopInitiator.Image = global::ProcessInitiator.Properties.Resources.Stop;
            this.buttonstopInitiator.Location = new System.Drawing.Point(108, 4);
            this.buttonstopInitiator.Name = "buttonstopInitiator";
            this.buttonstopInitiator.Size = new System.Drawing.Size(31, 29);
            this.buttonstopInitiator.TabIndex = 1;
            this.buttonstopInitiator.UseVisualStyleBackColor = true;
            this.buttonstopInitiator.Click += new System.EventHandler(this.buttonstopInitiator_Click);
            // 
            // buttonStartStopInitiator
            // 
            this.buttonStartStopInitiator.Image = global::ProcessInitiator.Properties.Resources.play;
            this.buttonStartStopInitiator.Location = new System.Drawing.Point(60, 4);
            this.buttonStartStopInitiator.Name = "buttonStartStopInitiator";
            this.buttonStartStopInitiator.Size = new System.Drawing.Size(31, 29);
            this.buttonStartStopInitiator.TabIndex = 0;
            this.buttonStartStopInitiator.UseVisualStyleBackColor = true;
            this.buttonStartStopInitiator.Click += new System.EventHandler(this.buttonStartStopInitiator_Click);
            // 
            // ProcessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.buttonstopInitiator);
            this.Controls.Add(this.buttonStartStopInitiator);
            this.Name = "ProcessControl";
            this.Size = new System.Drawing.Size(198, 33);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartStopInitiator;
        private System.Windows.Forms.Button buttonstopInitiator;
    }
}
