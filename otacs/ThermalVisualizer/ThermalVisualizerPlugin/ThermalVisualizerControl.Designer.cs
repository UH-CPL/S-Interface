namespace ThermalVisualizerPlugin
{
    partial class ThermalVisualizerControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThermalVisualizerControl));
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.labelMin = new System.Windows.Forms.Label();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.labelMax = new System.Windows.Forms.Label();
            this.trackBarMin = new System.Windows.Forms.TrackBar();
            this.trackBarMax = new System.Windows.Forms.TrackBar();
            this.pictureBoxBlue = new System.Windows.Forms.PictureBox();
            this.checkBoxOptimized = new System.Windows.Forms.CheckBox();
            this.pictureBoxColorBar = new System.Windows.Forms.PictureBox();
            this.pictureBoxRed = new System.Windows.Forms.PictureBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(31, 37);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(48, 20);
            this.textBoxMin.TabIndex = 1;
            this.textBoxMin.TextChanged += new System.EventHandler(this.textBoxMin_TextChanged);
            // 
            // labelMin
            // 
            this.labelMin.AutoSize = true;
            this.labelMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMin.Location = new System.Drawing.Point(4, 41);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(24, 13);
            this.labelMin.TabIndex = 2;
            this.labelMin.Text = "Min";
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(31, 61);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(48, 20);
            this.textBoxMax.TabIndex = 4;
            this.textBoxMax.TextChanged += new System.EventHandler(this.textBoxMax_TextChanged);
            // 
            // labelMax
            // 
            this.labelMax.AutoSize = true;
            this.labelMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMax.Location = new System.Drawing.Point(4, 62);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(27, 13);
            this.labelMax.TabIndex = 5;
            this.labelMax.Text = "Max";
            // 
            // trackBarMin
            // 
            this.trackBarMin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMin.Location = new System.Drawing.Point(80, 37);
            this.trackBarMin.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarMin.Name = "trackBarMin";
            this.trackBarMin.Size = new System.Drawing.Size(224, 45);
            this.trackBarMin.TabIndex = 7;
            this.trackBarMin.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMin.Scroll += new System.EventHandler(this.trackBarMin_Scroll);
            this.trackBarMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarMin_MouseDown);
            this.trackBarMin.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trackBarMin_MouseMove);
            // 
            // trackBarMax
            // 
            this.trackBarMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMax.Location = new System.Drawing.Point(80, 60);
            this.trackBarMax.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarMax.Name = "trackBarMax";
            this.trackBarMax.Size = new System.Drawing.Size(224, 45);
            this.trackBarMax.TabIndex = 8;
            this.trackBarMax.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMax.Scroll += new System.EventHandler(this.trackBarMax_Scroll);
            this.trackBarMax.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trackBarMax_MouseMove);
            // 
            // pictureBoxBlue
            // 
            this.pictureBoxBlue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBlue.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxBlue.Image")));
            this.pictureBoxBlue.Location = new System.Drawing.Point(88, 6);
            this.pictureBoxBlue.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxBlue.Name = "pictureBoxBlue";
            this.pictureBoxBlue.Size = new System.Drawing.Size(41, 32);
            this.pictureBoxBlue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBlue.TabIndex = 9;
            this.pictureBoxBlue.TabStop = false;
            // 
            // checkBoxOptimized
            // 
            this.checkBoxOptimized.AutoSize = true;
            this.checkBoxOptimized.Location = new System.Drawing.Point(60, 88);
            this.checkBoxOptimized.Name = "checkBoxOptimized";
            this.checkBoxOptimized.Size = new System.Drawing.Size(175, 17);
            this.checkBoxOptimized.TabIndex = 10;
            this.checkBoxOptimized.Text = "Apply Performance Optimization";
            this.checkBoxOptimized.UseVisualStyleBackColor = true;
            this.checkBoxOptimized.CheckedChanged += new System.EventHandler(this.checkBoxOptimized_CheckedChanged);
            // 
            // pictureBoxColorBar
            // 
            this.pictureBoxColorBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxColorBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxColorBar.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxColorBar.Image")));
            this.pictureBoxColorBar.Location = new System.Drawing.Point(129, 6);
            this.pictureBoxColorBar.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxColorBar.Name = "pictureBoxColorBar";
            this.pictureBoxColorBar.Size = new System.Drawing.Size(131, 32);
            this.pictureBoxColorBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxColorBar.TabIndex = 11;
            this.pictureBoxColorBar.TabStop = false;
            // 
            // pictureBoxRed
            // 
            this.pictureBoxRed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxRed.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxRed.Image")));
            this.pictureBoxRed.Location = new System.Drawing.Point(260, 6);
            this.pictureBoxRed.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxRed.Name = "pictureBoxRed";
            this.pictureBoxRed.Size = new System.Drawing.Size(36, 32);
            this.pictureBoxRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRed.TabIndex = 12;
            this.pictureBoxRed.TabStop = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(7, 8);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 23);
            this.btnUpdate.TabIndex = 13;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // ThermalVisualizerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.pictureBoxRed);
            this.Controls.Add(this.pictureBoxColorBar);
            this.Controls.Add(this.checkBoxOptimized);
            this.Controls.Add(this.pictureBoxBlue);
            this.Controls.Add(this.trackBarMax);
            this.Controls.Add(this.trackBarMin);
            this.Controls.Add(this.labelMax);
            this.Controls.Add(this.textBoxMin);
            this.Controls.Add(this.textBoxMax);
            this.Controls.Add(this.labelMin);
            this.MaximumSize = new System.Drawing.Size(500, 188);
            this.MinimumSize = new System.Drawing.Size(300, 30);
            this.Name = "ThermalVisualizerControl";
            this.Size = new System.Drawing.Size(307, 88);
            this.SizeChanged += new System.EventHandler(this.ThermalVisualizerControl_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMin;
        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.TrackBar trackBarMin;
        private System.Windows.Forms.TrackBar trackBarMax;
        private System.Windows.Forms.PictureBox pictureBoxBlue;
        private System.Windows.Forms.CheckBox checkBoxOptimized;
        private System.Windows.Forms.PictureBox pictureBoxColorBar;
        private System.Windows.Forms.PictureBox pictureBoxRed;
        private System.Windows.Forms.Button btnUpdate;



    }
}
