namespace SubjectbookIntegration
{
    partial class SubjectbookControl
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.studyBox = new System.Windows.Forms.ComboBox();
            this.listFolders = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblStatusVideo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblStatusVisual2Video = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblStatusVisualVideo = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblStatusConnection = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblStatusSignal = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridViewSession = new System.Windows.Forms.DataGridView();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.btnRefreshSubject = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.txtBoxStudy = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblStatusAudio = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSession)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 182);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "Session:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 114);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "Subject:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Study:";
            // 
            // studyBox
            // 
            this.studyBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.studyBox.FormattingEnabled = true;
            this.studyBox.Location = new System.Drawing.Point(64, 66);
            this.studyBox.Margin = new System.Windows.Forms.Padding(4);
            this.studyBox.Name = "studyBox";
            this.studyBox.Size = new System.Drawing.Size(196, 33);
            this.studyBox.TabIndex = 10;
            this.studyBox.SelectedIndexChanged += new System.EventHandler(this.studyBox_SelectedIndexChanged);
            // 
            // listFolders
            // 
            this.listFolders.Location = new System.Drawing.Point(17, 14);
            this.listFolders.Margin = new System.Windows.Forms.Padding(4);
            this.listFolders.Name = "listFolders";
            this.listFolders.Size = new System.Drawing.Size(189, 28);
            this.listFolders.TabIndex = 9;
            this.listFolders.Text = "Login to Subjectbook";
            this.listFolders.UseVisualStyleBackColor = true;
            this.listFolders.Click += new System.EventHandler(this.listFolders_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(235, 14);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 28);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Logout";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblStatusVideo
            // 
            this.lblStatusVideo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusVideo.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusVideo.Location = new System.Drawing.Point(104, 77);
            this.lblStatusVideo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusVideo.Name = "lblStatusVideo";
            this.lblStatusVideo.Size = new System.Drawing.Size(220, 18);
            this.lblStatusVideo.TabIndex = 19;
            this.lblStatusVideo.Text = "Not connected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblStatusAudio);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lblStatusVisual2Video);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblStatusVisualVideo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblStatusConnection);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblStatusSignal);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblStatusVideo);
            this.groupBox1.Location = new System.Drawing.Point(8, 416);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(338, 184);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 133);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 17);
            this.label8.TabIndex = 29;
            this.label8.Text = "Visual Video 2:";
            // 
            // lblStatusVisual2Video
            // 
            this.lblStatusVisual2Video.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusVisual2Video.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusVisual2Video.Location = new System.Drawing.Point(103, 133);
            this.lblStatusVisual2Video.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusVisual2Video.Name = "lblStatusVisual2Video";
            this.lblStatusVisual2Video.Size = new System.Drawing.Size(220, 18);
            this.lblStatusVisual2Video.TabIndex = 28;
            this.lblStatusVisual2Video.Text = "Not connected";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 106);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "Visual Video:";
            // 
            // lblStatusVisualVideo
            // 
            this.lblStatusVisualVideo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusVisualVideo.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusVisualVideo.Location = new System.Drawing.Point(105, 106);
            this.lblStatusVisualVideo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusVisualVideo.Name = "lblStatusVisualVideo";
            this.lblStatusVisualVideo.Size = new System.Drawing.Size(220, 18);
            this.lblStatusVisualVideo.TabIndex = 26;
            this.lblStatusVisualVideo.Text = "Not connected";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 17);
            this.label7.TabIndex = 25;
            this.label7.Text = "Connection:";
            // 
            // lblStatusConnection
            // 
            this.lblStatusConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusConnection.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusConnection.Location = new System.Drawing.Point(105, 22);
            this.lblStatusConnection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusConnection.Name = "lblStatusConnection";
            this.lblStatusConnection.Size = new System.Drawing.Size(220, 18);
            this.lblStatusConnection.TabIndex = 24;
            this.lblStatusConnection.Text = "Not connected";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 51);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 23;
            this.label5.Text = "Signal:";
            // 
            // lblStatusSignal
            // 
            this.lblStatusSignal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusSignal.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusSignal.Location = new System.Drawing.Point(104, 51);
            this.lblStatusSignal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusSignal.Name = "lblStatusSignal";
            this.lblStatusSignal.Size = new System.Drawing.Size(220, 18);
            this.lblStatusSignal.TabIndex = 22;
            this.lblStatusSignal.Text = "Not connected";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 77);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 17);
            this.label4.TabIndex = 21;
            this.label4.Text = "Thermal Video:";
            // 
            // dataGridViewSession
            // 
            this.dataGridViewSession.AllowUserToAddRows = false;
            this.dataGridViewSession.AllowUserToDeleteRows = false;
            this.dataGridViewSession.AllowUserToResizeColumns = false;
            this.dataGridViewSession.AllowUserToResizeRows = false;
            this.dataGridViewSession.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridViewSession.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSession.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSession.Enabled = false;
            this.dataGridViewSession.Location = new System.Drawing.Point(68, 160);
            this.dataGridViewSession.Name = "dataGridViewSession";
            this.dataGridViewSession.ReadOnly = true;
            this.dataGridViewSession.RowTemplate.Height = 24;
            this.dataGridViewSession.Size = new System.Drawing.Size(251, 248);
            this.dataGridViewSession.TabIndex = 21;
            // 
            // textBoxSubject
            // 
            this.textBoxSubject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSubject.Location = new System.Drawing.Point(64, 113);
            this.textBoxSubject.Name = "textBoxSubject";
            this.textBoxSubject.ReadOnly = true;
            this.textBoxSubject.Size = new System.Drawing.Size(196, 30);
            this.textBoxSubject.TabIndex = 23;
            // 
            // btnRefreshSubject
            // 
            this.btnRefreshSubject.Location = new System.Drawing.Point(267, 114);
            this.btnRefreshSubject.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefreshSubject.Name = "btnRefreshSubject";
            this.btnRefreshSubject.Size = new System.Drawing.Size(68, 28);
            this.btnRefreshSubject.TabIndex = 24;
            this.btnRefreshSubject.Text = "Refresh";
            this.btnRefreshSubject.UseVisualStyleBackColor = true;
            this.btnRefreshSubject.Click += new System.EventHandler(this.btnRefreshSubject_Click);
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(268, 68);
            this.btnChange.Margin = new System.Windows.Forms.Padding(4);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(68, 28);
            this.btnChange.TabIndex = 25;
            this.btnChange.Text = "Change";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // txtBoxStudy
            // 
            this.txtBoxStudy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxStudy.Location = new System.Drawing.Point(65, 67);
            this.txtBoxStudy.Name = "txtBoxStudy";
            this.txtBoxStudy.ReadOnly = true;
            this.txtBoxStudy.Size = new System.Drawing.Size(196, 30);
            this.txtBoxStudy.TabIndex = 26;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(59, 157);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 17);
            this.label9.TabIndex = 31;
            this.label9.Text = "Audio:";
            // 
            // lblStatusAudio
            // 
            this.lblStatusAudio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusAudio.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatusAudio.Location = new System.Drawing.Point(104, 157);
            this.lblStatusAudio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusAudio.Name = "lblStatusAudio";
            this.lblStatusAudio.Size = new System.Drawing.Size(220, 18);
            this.lblStatusAudio.TabIndex = 30;
            this.lblStatusAudio.Text = "Not connected";
            // 
            // SubjectbookControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtBoxStudy);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.dataGridViewSession);
            this.Controls.Add(this.btnRefreshSubject);
            this.Controls.Add(this.textBoxSubject);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.studyBox);
            this.Controls.Add(this.listFolders);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SubjectbookControl";
            this.Size = new System.Drawing.Size(353, 611);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSession)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox studyBox;
        private System.Windows.Forms.Button listFolders;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblStatusVideo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblStatusConnection;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblStatusSignal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblStatusVisualVideo;
        private System.Windows.Forms.DataGridView dataGridViewSession;
        private System.Windows.Forms.TextBox textBoxSubject;
        private System.Windows.Forms.Button btnRefreshSubject;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.TextBox txtBoxStudy;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblStatusVisual2Video;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblStatusAudio;
    }
}
