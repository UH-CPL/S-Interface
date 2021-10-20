namespace AVIWriterCompressedMulti
{
    partial class AVIWriterCompressMultiControl
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
            this.fileNameText = new System.Windows.Forms.TextBox();
            this.btnStartStopAVI = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FileName:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // fileNameText
            // 
            this.fileNameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameText.Location = new System.Drawing.Point(69, 8);
            this.fileNameText.Name = "fileNameText";
            this.fileNameText.Size = new System.Drawing.Size(119, 20);
            this.fileNameText.TabIndex = 1;
            // 
            // btnStartStopAVI
            // 
            this.btnStartStopAVI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStopAVI.Location = new System.Drawing.Point(69, 47);
            this.btnStartStopAVI.Name = "btnStartStopAVI";
            this.btnStartStopAVI.Size = new System.Drawing.Size(119, 23);
            this.btnStartStopAVI.TabIndex = 3;
            this.btnStartStopAVI.Text = "Start Creating AVI File";
            this.btnStartStopAVI.UseVisualStyleBackColor = true;
            this.btnStartStopAVI.Click += new System.EventHandler(this.btnStartStopAVI_Click);
            // 
            // AVIWriterCompressMultiControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStartStopAVI);
            this.Controls.Add(this.fileNameText);
            this.Controls.Add(this.label1);
            this.Name = "AVIWriterCompressMultiControl";
            this.Size = new System.Drawing.Size(202, 78);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fileNameText;
        private System.Windows.Forms.Button btnStartStopAVI;

    }
}
