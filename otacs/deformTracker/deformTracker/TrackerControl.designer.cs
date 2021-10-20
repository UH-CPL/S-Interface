namespace deformTracker
{
    partial class TrackerControl
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
            this.orientationLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.minusButton = new System.Windows.Forms.Button();
            this.MRI = new System.Windows.Forms.Panel();
            this.Deformation = new System.Windows.Forms.Label();
            this.Inaccuracy = new System.Windows.Forms.Label();
            this.avgMotion = new System.Windows.Forms.TextBox();
            this.motionScore = new System.Windows.Forms.TextBox();
            this.setTrackerPanel = new System.Windows.Forms.Panel();
            this.visual = new System.Windows.Forms.GroupBox();
            this.visualTrue = new System.Windows.Forms.RadioButton();
            this.visualLabel = new System.Windows.Forms.Label();
            this.UpdateTemplateGroup = new System.Windows.Forms.GroupBox();
            this.updateTemplateTrue = new System.Windows.Forms.RadioButton();
            this.updateTemplateLabel = new System.Windows.Forms.Label();
            this.displayMeshGroup = new System.Windows.Forms.GroupBox();
            this.displayMesh = new System.Windows.Forms.Label();
            this.displayMeshTrue = new System.Windows.Forms.RadioButton();
            this.displayIndiTrackerGroup = new System.Windows.Forms.GroupBox();
            this.displayIndi = new System.Windows.Forms.Label();
            this.displayIndiTrue = new System.Windows.Forms.RadioButton();
            this.setTracker = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.setTrackerNumButton = new System.Windows.Forms.Button();
            this.rowNumber = new System.Windows.Forms.TextBox();
            this.rowNumLabel = new System.Windows.Forms.Label();
            this.colNumLabel = new System.Windows.Forms.Label();
            this.colNumber = new System.Windows.Forms.TextBox();
            this.MRI.SuspendLayout();
            this.setTrackerPanel.SuspendLayout();
            this.visual.SuspendLayout();
            this.UpdateTemplateGroup.SuspendLayout();
            this.displayMeshGroup.SuspendLayout();
            this.displayIndiTrackerGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // orientationLabel
            // 
            this.orientationLabel.AutoSize = true;
            this.orientationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orientationLabel.Location = new System.Drawing.Point(57, 3);
            this.orientationLabel.Name = "orientationLabel";
            this.orientationLabel.Size = new System.Drawing.Size(114, 15);
            this.orientationLabel.TabIndex = 0;
            this.orientationLabel.Text = "InitialOrientation";
            // 
            // addButton
            // 
            this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addButton.Location = new System.Drawing.Point(30, 19);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "+";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // minusButton
            // 
            this.minusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minusButton.Location = new System.Drawing.Point(117, 19);
            this.minusButton.Name = "minusButton";
            this.minusButton.Size = new System.Drawing.Size(75, 23);
            this.minusButton.TabIndex = 2;
            this.minusButton.Text = "-";
            this.minusButton.UseVisualStyleBackColor = true;
            this.minusButton.Click += new System.EventHandler(this.minusButton_Click);
            // 
            // MRI
            // 
            this.MRI.Controls.Add(this.Deformation);
            this.MRI.Controls.Add(this.Inaccuracy);
            this.MRI.Controls.Add(this.avgMotion);
            this.MRI.Controls.Add(this.motionScore);
            this.MRI.Location = new System.Drawing.Point(9, 43);
            this.MRI.Name = "MRI";
            this.MRI.Size = new System.Drawing.Size(204, 62);
            this.MRI.TabIndex = 3;
            // 
            // Deformation
            // 
            this.Deformation.AutoSize = true;
            this.Deformation.BackColor = System.Drawing.SystemColors.Control;
            this.Deformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Deformation.Location = new System.Drawing.Point(16, 33);
            this.Deformation.Name = "Deformation";
            this.Deformation.Size = new System.Drawing.Size(81, 16);
            this.Deformation.TabIndex = 3;
            this.Deformation.Text = "Deformation";
            // 
            // Inaccuracy
            // 
            this.Inaccuracy.AutoSize = true;
            this.Inaccuracy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Inaccuracy.Location = new System.Drawing.Point(114, 32);
            this.Inaccuracy.Name = "Inaccuracy";
            this.Inaccuracy.Size = new System.Drawing.Size(73, 16);
            this.Inaccuracy.TabIndex = 2;
            this.Inaccuracy.Text = "Inaccuracy";
            // 
            // avgMotion
            // 
            this.avgMotion.Location = new System.Drawing.Point(107, 5);
            this.avgMotion.Name = "avgMotion";
            this.avgMotion.Size = new System.Drawing.Size(89, 20);
            this.avgMotion.TabIndex = 1;
            // 
            // motionScore
            // 
            this.motionScore.Location = new System.Drawing.Point(12, 5);
            this.motionScore.Name = "motionScore";
            this.motionScore.Size = new System.Drawing.Size(89, 20);
            this.motionScore.TabIndex = 0;
            // 
            // setTrackerPanel
            // 
            this.setTrackerPanel.Controls.Add(this.visual);
            this.setTrackerPanel.Controls.Add(this.UpdateTemplateGroup);
            this.setTrackerPanel.Controls.Add(this.displayMeshGroup);
            this.setTrackerPanel.Controls.Add(this.displayIndiTrackerGroup);
            this.setTrackerPanel.Controls.Add(this.setTracker);
            this.setTrackerPanel.Location = new System.Drawing.Point(9, 106);
            this.setTrackerPanel.Name = "setTrackerPanel";
            this.setTrackerPanel.Size = new System.Drawing.Size(203, 190);
            this.setTrackerPanel.TabIndex = 4;
            // 
            // visual
            // 
            this.visual.Controls.Add(this.visualTrue);
            this.visual.Controls.Add(this.visualLabel);
            this.visual.Location = new System.Drawing.Point(3, 118);
            this.visual.Name = "visual";
            this.visual.Size = new System.Drawing.Size(193, 34);
            this.visual.TabIndex = 17;
            this.visual.TabStop = false;
            // 
            // visualTrue
            // 
            this.visualTrue.AutoSize = true;
            this.visualTrue.Location = new System.Drawing.Point(115, 11);
            this.visualTrue.Name = "visualTrue";
            this.visualTrue.Size = new System.Drawing.Size(43, 17);
            this.visualTrue.TabIndex = 1;
            this.visualTrue.TabStop = true;
            this.visualTrue.Text = "Yes";
            this.visualTrue.UseVisualStyleBackColor = true;
            // 
            // visualLabel
            // 
            this.visualLabel.AutoSize = true;
            this.visualLabel.Location = new System.Drawing.Point(5, 12);
            this.visualLabel.Name = "visualLabel";
            this.visualLabel.Size = new System.Drawing.Size(72, 13);
            this.visualLabel.TabIndex = 0;
            this.visualLabel.Text = "VisualImagery";
            // 
            // UpdateTemplateGroup
            // 
            this.UpdateTemplateGroup.Controls.Add(this.updateTemplateTrue);
            this.UpdateTemplateGroup.Controls.Add(this.updateTemplateLabel);
            this.UpdateTemplateGroup.Location = new System.Drawing.Point(3, 84);
            this.UpdateTemplateGroup.Name = "UpdateTemplateGroup";
            this.UpdateTemplateGroup.Size = new System.Drawing.Size(193, 32);
            this.UpdateTemplateGroup.TabIndex = 16;
            this.UpdateTemplateGroup.TabStop = false;
            // 
            // updateTemplateTrue
            // 
            this.updateTemplateTrue.AutoSize = true;
            this.updateTemplateTrue.Location = new System.Drawing.Point(115, 11);
            this.updateTemplateTrue.Name = "updateTemplateTrue";
            this.updateTemplateTrue.Size = new System.Drawing.Size(43, 17);
            this.updateTemplateTrue.TabIndex = 1;
            this.updateTemplateTrue.TabStop = true;
            this.updateTemplateTrue.Text = "Yes";
            this.updateTemplateTrue.UseVisualStyleBackColor = true;
            // 
            // updateTemplateLabel
            // 
            this.updateTemplateLabel.AutoSize = true;
            this.updateTemplateLabel.Location = new System.Drawing.Point(4, 13);
            this.updateTemplateLabel.Name = "updateTemplateLabel";
            this.updateTemplateLabel.Size = new System.Drawing.Size(91, 13);
            this.updateTemplateLabel.TabIndex = 0;
            this.updateTemplateLabel.Text = "UpdateTemplates";
            // 
            // displayMeshGroup
            // 
            this.displayMeshGroup.Controls.Add(this.displayMesh);
            this.displayMeshGroup.Controls.Add(this.displayMeshTrue);
            this.displayMeshGroup.Location = new System.Drawing.Point(3, 43);
            this.displayMeshGroup.Name = "displayMeshGroup";
            this.displayMeshGroup.Size = new System.Drawing.Size(193, 36);
            this.displayMeshGroup.TabIndex = 15;
            this.displayMeshGroup.TabStop = false;
            // 
            // displayMesh
            // 
            this.displayMesh.AutoSize = true;
            this.displayMesh.Location = new System.Drawing.Point(4, 13);
            this.displayMesh.Name = "displayMesh";
            this.displayMesh.Size = new System.Drawing.Size(67, 13);
            this.displayMesh.TabIndex = 11;
            this.displayMesh.Text = "DisplayMesh";
            // 
            // displayMeshTrue
            // 
            this.displayMeshTrue.AutoSize = true;
            this.displayMeshTrue.Location = new System.Drawing.Point(115, 11);
            this.displayMeshTrue.Name = "displayMeshTrue";
            this.displayMeshTrue.Size = new System.Drawing.Size(43, 17);
            this.displayMeshTrue.TabIndex = 12;
            this.displayMeshTrue.TabStop = true;
            this.displayMeshTrue.Text = "Yes";
            this.displayMeshTrue.UseVisualStyleBackColor = true;
            // 
            // displayIndiTrackerGroup
            // 
            this.displayIndiTrackerGroup.Controls.Add(this.displayIndi);
            this.displayIndiTrackerGroup.Controls.Add(this.displayIndiTrue);
            this.displayIndiTrackerGroup.Location = new System.Drawing.Point(3, 3);
            this.displayIndiTrackerGroup.Name = "displayIndiTrackerGroup";
            this.displayIndiTrackerGroup.Size = new System.Drawing.Size(193, 36);
            this.displayIndiTrackerGroup.TabIndex = 14;
            this.displayIndiTrackerGroup.TabStop = false;
            // 
            // displayIndi
            // 
            this.displayIndi.AutoSize = true;
            this.displayIndi.Location = new System.Drawing.Point(4, 14);
            this.displayIndi.Name = "displayIndi";
            this.displayIndi.Size = new System.Drawing.Size(100, 13);
            this.displayIndi.TabIndex = 8;
            this.displayIndi.Text = "DisplayIndiTrackers";
            // 
            // displayIndiTrue
            // 
            this.displayIndiTrue.AutoSize = true;
            this.displayIndiTrue.Location = new System.Drawing.Point(115, 13);
            this.displayIndiTrue.Name = "displayIndiTrue";
            this.displayIndiTrue.Size = new System.Drawing.Size(43, 17);
            this.displayIndiTrue.TabIndex = 9;
            this.displayIndiTrue.TabStop = true;
            this.displayIndiTrue.Text = "Yes";
            this.displayIndiTrue.UseVisualStyleBackColor = true;
            // 
            // setTracker
            // 
            this.setTracker.Location = new System.Drawing.Point(40, 158);
            this.setTracker.Name = "setTracker";
            this.setTracker.Size = new System.Drawing.Size(121, 28);
            this.setTracker.TabIndex = 7;
            this.setTracker.Text = "SetTrackerProperty";
            this.setTracker.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.setTrackerNumButton);
            this.panel1.Controls.Add(this.rowNumber);
            this.panel1.Controls.Add(this.rowNumLabel);
            this.panel1.Controls.Add(this.colNumLabel);
            this.panel1.Controls.Add(this.colNumber);
            this.panel1.Location = new System.Drawing.Point(9, 302);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 69);
            this.panel1.TabIndex = 5;
            // 
            // setTrackerNumButton
            // 
            this.setTrackerNumButton.Location = new System.Drawing.Point(60, 38);
            this.setTrackerNumButton.Name = "setTrackerNumButton";
            this.setTrackerNumButton.Size = new System.Drawing.Size(75, 23);
            this.setTrackerNumButton.TabIndex = 4;
            this.setTrackerNumButton.Text = "SetTracker#";
            this.setTrackerNumButton.UseVisualStyleBackColor = true;
            this.setTrackerNumButton.Click += new System.EventHandler(this.setTrackerNumButton_Click);
            // 
            // rowNumber
            // 
            this.rowNumber.Location = new System.Drawing.Point(141, 9);
            this.rowNumber.Name = "rowNumber";
            this.rowNumber.Size = new System.Drawing.Size(43, 20);
            this.rowNumber.TabIndex = 3;
            // 
            // rowNumLabel
            // 
            this.rowNumLabel.AutoSize = true;
            this.rowNumLabel.Location = new System.Drawing.Point(97, 13);
            this.rowNumLabel.Name = "rowNumLabel";
            this.rowNumLabel.Size = new System.Drawing.Size(39, 13);
            this.rowNumLabel.TabIndex = 2;
            this.rowNumLabel.Text = "Row #";
            // 
            // colNumLabel
            // 
            this.colNumLabel.AutoSize = true;
            this.colNumLabel.Location = new System.Drawing.Point(4, 13);
            this.colNumLabel.Name = "colNumLabel";
            this.colNumLabel.Size = new System.Drawing.Size(32, 13);
            this.colNumLabel.TabIndex = 1;
            this.colNumLabel.Text = "Col #";
            // 
            // colNumber
            // 
            this.colNumber.Location = new System.Drawing.Point(40, 9);
            this.colNumber.Name = "colNumber";
            this.colNumber.Size = new System.Drawing.Size(51, 20);
            this.colNumber.TabIndex = 0;
            // 
            // TrackerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.setTrackerPanel);
            this.Controls.Add(this.MRI);
            this.Controls.Add(this.minusButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.orientationLabel);
            this.Name = "TrackerControl";
            this.Size = new System.Drawing.Size(221, 374);
            this.MRI.ResumeLayout(false);
            this.MRI.PerformLayout();
            this.setTrackerPanel.ResumeLayout(false);
            this.visual.ResumeLayout(false);
            this.visual.PerformLayout();
            this.UpdateTemplateGroup.ResumeLayout(false);
            this.UpdateTemplateGroup.PerformLayout();
            this.displayMeshGroup.ResumeLayout(false);
            this.displayMeshGroup.PerformLayout();
            this.displayIndiTrackerGroup.ResumeLayout(false);
            this.displayIndiTrackerGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label orientationLabel;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button minusButton;
        private System.Windows.Forms.Panel MRI;
        private System.Windows.Forms.TextBox avgMotion;
        private System.Windows.Forms.TextBox motionScore;
        private System.Windows.Forms.Panel setTrackerPanel;
        private System.Windows.Forms.Button setTracker;
        private System.Windows.Forms.Label displayIndi;
        private System.Windows.Forms.RadioButton displayIndiTrue;
        private System.Windows.Forms.RadioButton displayMeshTrue;
        private System.Windows.Forms.Label displayMesh;
        private System.Windows.Forms.GroupBox displayMeshGroup;
        private System.Windows.Forms.GroupBox displayIndiTrackerGroup;
        private System.Windows.Forms.GroupBox UpdateTemplateGroup;
        private System.Windows.Forms.RadioButton updateTemplateTrue;
        private System.Windows.Forms.Label updateTemplateLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button setTrackerNumButton;
        private System.Windows.Forms.TextBox rowNumber;
        private System.Windows.Forms.Label rowNumLabel;
        private System.Windows.Forms.Label colNumLabel;
        private System.Windows.Forms.TextBox colNumber;
        private System.Windows.Forms.GroupBox visual;
        private System.Windows.Forms.Label visualLabel;
        private System.Windows.Forms.RadioButton visualTrue;
        private System.Windows.Forms.Label Deformation;
        private System.Windows.Forms.Label Inaccuracy;
    }
}
