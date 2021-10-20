namespace Display
{
    partial class DisplayControl
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
            this.picBoxDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxDisplay
            // 
            this.picBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxDisplay.Location = new System.Drawing.Point(3, 3);
            this.picBoxDisplay.Name = "picBoxDisplay";
            this.picBoxDisplay.Size = new System.Drawing.Size(394, 264);
            this.picBoxDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxDisplay.TabIndex = 0;
            this.picBoxDisplay.TabStop = false;
            this.picBoxDisplay.Click += new System.EventHandler(this.picBoxDisplay_Click);
            // 
            // DisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.picBoxDisplay);
            this.Name = "DisplayControl";
            this.Size = new System.Drawing.Size(400, 270);
            this.Load += new System.EventHandler(this.DisplayControl_Load);
            this.Resize += new System.EventHandler(this.picBoxDisplay_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DisplayControl_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxDisplay;
    }
}
