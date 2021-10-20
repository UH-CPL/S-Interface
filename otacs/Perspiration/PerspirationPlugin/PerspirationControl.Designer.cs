namespace PerspirationPlugin
{
    partial class PerspirationControl
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
            this.checkBoxNormEnergy = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxNormEnergy
            // 
            this.checkBoxNormEnergy.AutoSize = true;
            this.checkBoxNormEnergy.Location = new System.Drawing.Point(20, 19);
            this.checkBoxNormEnergy.Name = "checkBoxNormEnergy";
            this.checkBoxNormEnergy.Size = new System.Drawing.Size(114, 17);
            this.checkBoxNormEnergy.TabIndex = 0;
            this.checkBoxNormEnergy.Text = "Normalized Energy";
            this.checkBoxNormEnergy.UseVisualStyleBackColor = true;
            this.checkBoxNormEnergy.CheckedChanged += new System.EventHandler(this.checkBoxNormEnergy_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxNormEnergy);
            this.groupBox1.Location = new System.Drawing.Point(56, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(155, 55);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // PerspirationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PerspirationControl";
            this.Size = new System.Drawing.Size(268, 99);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxNormEnergy;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}
