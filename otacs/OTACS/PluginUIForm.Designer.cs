namespace OTACS
{
    partial class PluginUIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginUIForm));
            this.btnLockUnlock = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLockUnlock
            // 
            this.btnLockUnlock.Image = ((System.Drawing.Image)(resources.GetObject("btnLockUnlock.Image")));
            this.btnLockUnlock.Location = new System.Drawing.Point(2, 2);
            this.btnLockUnlock.Name = "btnLockUnlock";
            this.btnLockUnlock.Size = new System.Drawing.Size(38, 29);
            this.btnLockUnlock.TabIndex = 0;
            this.btnLockUnlock.UseVisualStyleBackColor = true;
            this.btnLockUnlock.Click += new System.EventHandler(this.btnLockUnlock_Click);
            // 
            // PluginUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 300);
            this.Controls.Add(this.btnLockUnlock);
            this.MaximizeBox = false;
            this.Name = "PluginUIForm";
            this.Text = "PluginUIForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLockUnlock;
    }
}