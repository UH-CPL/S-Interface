using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using Google.Apis.Download;
using System.Threading;

namespace SubjectbookIntegration
{
    public partial class SubjectbookControl : UserControl
    {
        public class TableData
        { 
            private string name;
            private Image statusImg;

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public Image StatusImg
            {
                get
                {
                    return statusImg;
                }
                set
                {
                    statusImg = value;
                }
            }

        }

        //Members
        SubjectbookPlugin plugin;
        private DriveService _service;
        private Dictionary<string, string> _studyFolders;
        private Dictionary<string, string> _sessionFolders;

        private string selectedSessionName;
        private string selectedSessionFolderID;

        delegate void UpdateStatusDelegate(string statusMsg);

        private WaitForm waitForm = new WaitForm();
        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker bw2 = new BackgroundWorker();

        bool isPPUploaded;
        bool isThermalAVIUploaded;
        bool isVisualAVIUploaded;
        bool isVisualAVI2Uploaded;
        bool isSTMUploaded;
        bool shouldNotifyApp = false;

        string pendingSubjectName;
        string pendingSubjectFolderID;
        const string waitingMessage = "GO TO QUESTIONNAIRE...";

        public SubjectbookControl(SubjectbookPlugin setPlugin)
        {
            InitializeComponent();
           
            //Set the plugin 
            plugin = setPlugin;
            plugin.UploadPerspirationFile += new VoidDelegate(OnUploadPerspirationFile);
            plugin.UploadPerspirationAVI += new VoidDelegate(OnUploadPerspirationAVI);
            plugin.UploadVisualAVI += new VoidDelegate(OnUploadVisualAVI);
            plugin.UploadVisualAVI2 += new VoidDelegate(OnUploadVisualAVI2);
            plugin.UploadAudio += new VoidDelegate(OnUploadAudio);
            plugin.MessageCreatingExcelSheet += new VoidDelegate(OnMessageCreatingExcelSheet);


            ResetStatusLabels();

            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_WorkComplete;

            bw2.DoWork += bw_DoWork2;
            bw2.RunWorkerCompleted += bw_WorkComplete2;


            dataGridViewSession.RowHeadersVisible = false;
            dataGridViewSession.ColumnHeadersVisible = false;
            dataGridViewSession.RowTemplate.Height = 35;
            dataGridViewSession.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        private void ResetStatusLabels()
        {
            lblStatusConnection.Text = "Not connected";
            lblStatusSignal.Text = "Pending";
            lblStatusVideo.Text = "Pending";
            lblStatusVisualVideo.Text = "Pending";
            lblStatusVisual2Video.Text = "Pending";
            lblStatusAudio.Text = "Pending";
        }

        private void listFolders_Click(object sender, EventArgs e)
        {
            
            bw.RunWorkerAsync();
            ShowWaitForm("Connecting to Subjectbook...");

        }


        private void RefreshSessionBox(string folderID)
        {

            if (folderID.Length > 0 && !folderID.Contains('['))
            {
                _sessionFolders = GoogleDriveConnector.GetFolders(_service, folderID);
                if (_sessionFolders.Count > 0)
                {
                    TableData[] arr = new TableData[_sessionFolders.Count];
                    bool markedForProcessing = false;
                    int processIndex = 0;

                    for (int i = 0; i < _sessionFolders.Count; i++)
                    {
                        KeyValuePair<string, string> session = _sessionFolders.ElementAt(i);
                        TableData td = new TableData();
                        td.Name = session.Value;
                        if(markedForProcessing)
                        {
                            td.StatusImg = Properties.Resources.missing;
                        }
                        else
                        {       
                            if (session.Value.ToUpper().Contains(selectedSessionName.ToUpper()))
                            {
                                selectedSessionFolderID = session.Key;
                                plugin.SendFileName(pendingSubjectName + "_" + selectedSessionName);

                                td.StatusImg = Properties.Resources.process;
                                processIndex = i;
                                markedForProcessing = true;

                                
                            }
                            else
                            {
                                td.StatusImg = Properties.Resources.done;
                            }
                        }

                        arr[i] = td;
                        
                    }

                    //dataGridViewSession.DataSource = null;
                    //dataGridViewSession.Update();
                    //dataGridViewSession.Refresh();
                    dataGridViewSession.DataSource = null;

                    BindingSource bs = new BindingSource();
                    bs.DataSource = arr;
                    dataGridViewSession.DataSource = bs;
                    //dataGridViewSession.DataSource = arr;
                    
                    dataGridViewSession.AutoResizeColumns();
                    dataGridViewSession.Columns[0].Width = 179;
                    dataGridViewSession.Columns[1].Width = 70;

                    

                    if (markedForProcessing)
                    {
                        dataGridViewSession.CurrentCell = dataGridViewSession.Rows[processIndex].Cells[0];
                        lblStatusConnection.Text = "Processing...";
                        lblStatusSignal.Text = "Pending";
                        lblStatusVideo.Text = "Pending";
                        lblStatusVisualVideo.Text = "Pending";
                        lblStatusVisual2Video.Text = "Pending";
                        lblStatusAudio.Text = "Pending";

                        isPPUploaded = false;
                        isSTMUploaded = false;
                        isThermalAVIUploaded = false;
                        isVisualAVIUploaded = false;
                        isVisualAVI2Uploaded = false;
                        shouldNotifyApp = false;
                    }
                    else
                    {
                        dataGridViewSession.ClearSelection();
                        lblStatusConnection.Text = "Finished all sessions!";
                        lblStatusSignal.Text = "";
                        lblStatusVideo.Text = "";
                        lblStatusVisualVideo.Text = "";
                        lblStatusVisual2Video.Text = "";
                        lblStatusAudio.Text = "";
                    }

                    dataGridViewSession.Update();
                    dataGridViewSession.Refresh();
                }
            }
        }

        private void createFileButton_Click(object sender, EventArgs e)
        {
            var filePath = "\\\\vmware-host\\Shared Folders\\Desktop\\test.csv";
            var sampleFolderId = "0BxudC9RDhVZoNWRzVFBBbmZMUms"; // this is for a folder in Ashik's google drive only! Will not work for others probably
            // if you don't pass folderId it will upload to root folder. 
            UploadToDrive(filePath, sampleFolderId);
        }

        private void studyBox_SelectedIndexChanged(object sender, EventArgs e)
        {   
            if(studyBox.SelectedIndex != -1)
            {
                KeyValuePair<string, string> selectedStudy = (KeyValuePair < string, string>)  studyBox.SelectedItem;

                Properties.Settings.Default.StudyName  = selectedStudy.Value;
                Properties.Settings.Default.StudyFolderID = selectedStudy.Key;
                Properties.Settings.Default.Save();

                txtBoxStudy.Text = Properties.Settings.Default.StudyName;
                txtBoxStudy.Visible = true;
                studyBox.Visible = false;

                //FindPendingSubject();
            }
            else
            {
                Properties.Settings.Default.Reset();
                txtBoxStudy.Visible = false;
                studyBox.Visible = true;
            }
        }


        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public void UploadToDrive(string uploadFile, string _parent = "root")
        {
            //if (!String.IsNullOrEmpty(uploadFile))
            //{
            //    File fileMetadata = new File();
            //    fileMetadata.Name = System.IO.Path.GetFileName(uploadFile);
            //    fileMetadata.MimeType = GetMimeType(uploadFile);
            //    fileMetadata.Parents = new List<string> { _parent };
            //    try
            //    {
            //        byte[] byteArray = System.IO.File.ReadAllBytes(uploadFile);
            //        System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            //        FilesResource.CreateMediaUpload request = _service.Files.Create(fileMetadata, stream, GetMimeType(uploadFile));
            //        request.ResponseReceived += RequestOnResponseReceived;
            //        request.ProgressChanged += RequestOnProgressChanged;
            //        request.UploadAsync();

            //    }
            //    catch (System.IO.IOException iox)
            //    {
            //        // Log
            //        Console.WriteLine(iox.Message);
            //    }
            //    catch (Exception e) // any special google drive exceptions??
            //    {
            //        //Log
            //        Console.WriteLine(e.Message);
            //    }
            //}
            //else
            //{
            //    //Log file does not exist
            //}
        }

        private void RequestOnProgressChanged(IUploadProgress uploadProgress)
        {
            //lblStatus.Text = uploadProgress.Status.ToString();
            Console.WriteLine(@"upload status: " + uploadProgress.Status.ToString());
        }

        private void RequestOnResponseReceived(File file)
        {
            Console.WriteLine(@"File uploaded with ID: " + file.Id);

            if(file.Name.Contains(".pp"))
            {
                isPPUploaded = true;
            }

            if (file.Name.Contains(".stm"))
            {
                isSTMUploaded = true;
            }

            if (file.Name.Contains(".avi") && !file.Name.Contains(".avi1.avi") && !file.Name.Contains(".avi2.avi"))
            {
                isThermalAVIUploaded = true;
                if (this.lblStatusVideo.InvokeRequired)
                {
                    this.lblStatusVideo.BeginInvoke((MethodInvoker)delegate () { this.lblStatusVideo.Text = "Uploaded successfully"; });
                }
                else
                {
                    this.lblStatusVideo.Text = "Uploaded successfully!";
                }
            }

            if (file.Name.Contains(".avi1.avi"))
            {
                isVisualAVIUploaded = true;
                if (this.lblStatusVisualVideo.InvokeRequired)
                {
                    this.lblStatusVisualVideo.BeginInvoke((MethodInvoker)delegate () { this.lblStatusVisualVideo.Text = "Uploaded successfully"; });
                }
                else
                {
                    this.lblStatusVisualVideo.Text = "Uploaded successfully!";
                }
            }

            if (file.Name.Contains(".avi2.avi"))
            {
                isVisualAVI2Uploaded = true;
                if (this.lblStatusVisual2Video.InvokeRequired)
                {
                    this.lblStatusVisual2Video.BeginInvoke((MethodInvoker)delegate () { this.lblStatusVisual2Video.Text = "Uploaded successfully"; });
                }
                else
                {
                    this.lblStatusVisual2Video.Text = "Uploaded successfully!";
                }
            }

            if (isPPUploaded && isSTMUploaded)
            {
                if (this.lblStatusSignal.InvokeRequired)
                {
                    this.lblStatusSignal.BeginInvoke((MethodInvoker)delegate () { this.lblStatusSignal.Text = "Uploaded successfully"; });
                }
                else
                {
                    this.lblStatusSignal.Text = "Uploaded successfully!";
                }

            }

            if (file.Name.Contains(".wav"))
            {
                
                if (this.lblStatusAudio.InvokeRequired)
                {
                    this.lblStatusAudio.BeginInvoke((MethodInvoker)delegate () { this.lblStatusAudio.Text = "Uploaded successfully"; });
                }
                else
                {
                    this.lblStatusAudio.Text = "Uploaded successfully!";
                }
            }

            /*
            if(isPPUploaded && isSTMUploaded && isVisualAVIUploaded && isThermalAVIUploaded && !shouldNotifyApp)
            {
                shouldNotifyApp = true;
                
            }
            */
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            GoogleDriveConnector.ClearCredentials();
            studyBox.DataSource = null;
            studyBox.DataBindings.Clear();

            dataGridViewSession.DataSource = null;
            dataGridViewSession.DataBindings.Clear();

            Properties.Settings.Default.Reset();
            ResetStatusLabels();

            txtBoxStudy.Text = "";
            textBoxSubject.Text = "";
        }

        
        void OnUploadPerspirationFile()
        {

            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUploadPerspirationFile));
                return;
            }

            string ppFilePath = null;
            string stmFilePath = null;
            
            if(plugin.FileName == null)
            {
                ppFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                ppFilePath += "\\PerspirationSig.pp";
                stmFilePath += "\\PerspirationSig.stm";
            }
            else
            {
                ppFilePath = plugin.FileName + ".pp";
                stmFilePath = plugin.FileName + ".stm";
            }
            
            if (System.IO.File.Exists(ppFilePath))
            {
                isPPUploaded = false;
                isSTMUploaded = false;
                lblStatusSignal.Text = "Uploading signal...";
                UploadToDrive(ppFilePath, selectedSessionFolderID);
                UploadToDrive(stmFilePath, selectedSessionFolderID);
            }

        }

        void OnUploadPerspirationAVI()
        {

            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUploadPerspirationAVI));
                return;
            }

            string aviFilePath = null;

            if (plugin.FileName == null)
            {
                aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                aviFilePath += "\\PerspirationVideo.avi";
            }
            else
            {
                aviFilePath = plugin.FileName + ".avi";
            }
            /*
            var aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            aviFilePath += "\\PerspirationVideo.avi";
            */

            if (System.IO.File.Exists(aviFilePath))
            {
                isThermalAVIUploaded = false;
                lblStatusVideo.Text = "Uploading video...";
                UploadToDrive(aviFilePath, selectedSessionFolderID);
            }

        }

        void OnUploadVisualAVI()
        {
            
            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUploadVisualAVI));
                return;
            }

            string aviFilePath = null;

            if (plugin.FileName == null)
            {
                aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                aviFilePath += "\\VisualVideo.avi1.avi";
            }
            else
            {
                aviFilePath = plugin.FileName + ".avi1.avi";
            }
            /*
            var aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            aviFilePath += "\\VisualVideo.avi1.avi";
            */

            if (System.IO.File.Exists(aviFilePath))
            {
                isVisualAVIUploaded = false;
                lblStatusVisualVideo.Text = "Uploading video...";
                UploadToDrive(aviFilePath, selectedSessionFolderID);
            }

            if (ChangeOTACStoAPP())
            {
                lblStatusConnection.Text = waitingMessage;
                plugin.SendActivity(false);
            }
            else
            {
                lblStatusConnection.Text = "Error during handshake...";
                plugin.SendActivity(true);
            }

        }

        void OnUploadVisualAVI2()
        {

            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUploadVisualAVI2));
                return;
            }

            string avi2FilePath = null;
            

            if (plugin.FileName == null)
            {
                avi2FilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                avi2FilePath += "\\VisualVideo.avi2.avi";
            }
            else
            {
                avi2FilePath = plugin.FileName + ".avi2.avi";
                
            }
            /*
            var aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            aviFilePath += "\\VisualVideo.avi1.avi";
            */

            if (System.IO.File.Exists(avi2FilePath))
            {
                isVisualAVI2Uploaded = false;
                lblStatusVisual2Video.Text = "Uploading video 2...";
                UploadToDrive(avi2FilePath, selectedSessionFolderID);
            }


        }

        

        void OnUploadAudio()
        {

            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUploadAudio));
                return;
            }

            string wavFilePath = null;

            if (plugin.FileName == null)
            {
                wavFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                wavFilePath += "\\Audio.wav";
            }
            else
            {
                wavFilePath = plugin.FileName + ".wav";
            }
            /*
            var aviFilePath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            aviFilePath += "\\VisualVideo.avi1.avi";
            */

            if (wavFilePath != null)
            {
                if (System.IO.File.Exists(wavFilePath))
                {
                    lblStatusAudio.Text = "Uploading audio...";
                    UploadToDrive(wavFilePath, selectedSessionFolderID);
                }
            }

        }

        void OnMessageCreatingExcelSheet()
        {

            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnMessageCreatingExcelSheet));
                return;
            }

            lblStatusSignal.Text = "Creating Signal file...";
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _service = GoogleDriveConnector.Connect();
        }

        private void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            if (_service != null)
            {
                _studyFolders = GoogleDriveConnector.GetFolders(_service, "root");
            }
        }

        private void bw_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWaitForm();

            if (_service != null)
            {
                lblStatusConnection.Text = "Connected";

                /*
                _studyFolders = GoogleDriveConnector.GetFolders(_service, "root");
                if (_studyFolders.Count > 0)
                {
                    studyBox.DataSource = new BindingSource(_studyFolders, null);
                    studyBox.DisplayMember = "Value";
                    studyBox.ValueMember = "Key";

                    studyBox.SelectedIndex = -1;
                }
                */

                if(Properties.Settings.Default.StudyName !=null && Properties.Settings.Default.StudyFolderID != null &&
                    Properties.Settings.Default.StudyName != "" && Properties.Settings.Default.StudyFolderID != "")
                {
                    txtBoxStudy.Text = Properties.Settings.Default.StudyName;

                    txtBoxStudy.Visible = true;
                    studyBox.Visible = false;

                    FindPendingSubject();
                }
                else
                {
                    LoadStudy();
                }
            }

            
        }

        private void bw_WorkComplete2(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWaitForm();

            if (_studyFolders.Count > 0)
            {
                studyBox.DataSource = new BindingSource(_studyFolders, null);
                studyBox.DisplayMember = "Value";
                studyBox.ValueMember = "Key";

                studyBox.SelectedIndex = -1;

                txtBoxStudy.Visible = false;
                studyBox.Visible = true;
            }
        }

        public void ShowWaitForm(string message)
        {
            if(waitForm == null)
            {
                waitForm = new WaitForm();
            }
            waitForm.lblMessage.Text = message;
            waitForm.Show();
        }

        public void HideWaitForm()
        {
            waitForm.Close();
            waitForm = null;
        }

        private void btnRefreshSubject_Click(object sender, EventArgs e)
        {
            /*
            if(shouldNotifyApp)
            {
                if (ChangeOTACStoAPP())
                {
                    lblStatusConnection.Text = waitingMessage;
                    shouldNotifyApp = false;
                }
                else
                {
                    lblStatusConnection.Text = "ERROR updating on Google Drive";
                }
                lblStatusSignal.Text = "";
                lblStatusVideo.Text = "";
                lblStatusVisualVideo.Text = "";
            }
            else
            {
                FindPendingSubject();
            }
            */
            FindPendingSubject();
        }

        private void FindPendingSubject()
        {
            pendingSubjectName = null;
            pendingSubjectFolderID = null;
            selectedSessionName = null;

            string studyFolderID = Properties.Settings.Default.StudyFolderID; //studyBox.SelectedValue.ToString();
            Dictionary<string, string> filesList = GoogleDriveConnector.GetFiles(_service, studyFolderID);
            string csvFolerID = null;

            foreach (KeyValuePair<string, string> file in filesList)
            {
                string fileName = file.Value;
                if (fileName.ToUpper().Contains("COMPLETION_PROGRESS"))
                {
                    csvFolerID = file.Key;
                    break;
                }
            }

            if (csvFolerID != null)
            {
                var request = _service.Files.Get(csvFolerID);
                var stream = new System.IO.MemoryStream();
                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        Console.WriteLine(progress.BytesDownloaded);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        Console.WriteLine("Download complete.");
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        Console.WriteLine("Download failed.");
                                        break;
                                    }
                            }
                        };
                request.Download(stream);

                stream.Position = 0; // Rewind!
                List<string> rows = new List<string>();
                // Are you *sure* you want ASCII?
                using (var reader = new System.IO.StreamReader(stream, Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        rows.Add(line);

                        if (line.ToUpper().Contains("PENDINGSUBJECT"))
                        {
                            string[] words = line.Split('\t');
                            if(words.Count() >= 4)
                            {
                                if(words[3].ToUpper().Contains("OTACS"))
                                {
                                    pendingSubjectName = words[1];
                                    textBoxSubject.Text = pendingSubjectName;
                                    selectedSessionName = words[2];

                                    foreach (string row in rows)
                                    {
                                        if (row.ToUpper().Contains(pendingSubjectName.ToUpper()))
                                        {
                                            string[] words2 = row.Split('\t');
                                            pendingSubjectFolderID = words2[1];
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    lblStatusConnection.Text = waitingMessage;
                                    lblStatusSignal.Text = "";
                                    lblStatusVideo.Text = "";
                                    lblStatusVisualVideo.Text = "";
                                    lblStatusVisual2Video.Text = "";
                                    lblStatusAudio.Text = "";
                                }
                                
                            }
                            else
                            {
                                lblStatusConnection.Text = waitingMessage;
                                lblStatusSignal.Text = "";
                                lblStatusVideo.Text = "";
                                lblStatusVisualVideo.Text = "";
                                lblStatusVisual2Video.Text = "";
                                lblStatusAudio.Text = "";
                            }
                            break;
                        }
                    }
                }

                //textBoxSubject.ReadOnly = true;
            }
            else
            {
                //textBoxSubject.ReadOnly = false;
            }

            if(pendingSubjectFolderID != null && selectedSessionName != null)
            {
                RefreshSessionBox(pendingSubjectFolderID);

                plugin.SendActivity(true);

                bool isBiofeedback = false;
                if(selectedSessionName.ToUpper().Contains("COGNITIVE") || selectedSessionName.ToUpper().Contains("MOTORIC") 
                    || selectedSessionName.ToUpper().Contains("FINAL") || selectedSessionName.ToUpper().Contains("FAILURE"))
                {
                    isBiofeedback = CheckIfBiofeedbackRequired();
                }
                plugin.SendBiofeedback(isBiofeedback);
            }
            else
            {
                plugin.SendActivity(false);
            }
            
        }

        private bool CheckIfBiofeedbackRequired()
        {
            Dictionary<string, string> filesList = GoogleDriveConnector.GetFiles(_service, pendingSubjectFolderID);
            string csvFolerID = null;
            foreach (KeyValuePair<string, string> file in filesList)
            {
                string fileName = file.Value;
                if (fileName.ToUpper().Contains(pendingSubjectName.ToUpper()))
                {
                    csvFolerID = file.Key;
                    break;
                }
            }

            if (csvFolerID != null)
            {
                var request = _service.Files.Get(csvFolerID);
                var stream = new System.IO.MemoryStream();
                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        Console.WriteLine(progress.BytesDownloaded);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        Console.WriteLine("Download complete.");
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        Console.WriteLine("Download failed.");
                                        break;
                                    }
                            }
                        };
                request.Download(stream);

                stream.Position = 0; // Rewind!
                // Are you *sure* you want ASCII?
                using (var reader = new System.IO.StreamReader(stream, Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.ToUpper().Contains("CONTROL"))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                //textBoxSubject.ReadOnly = false;
            }

            return true;
        }

        public System.IO.MemoryStream GenerateStreamFromString(string s)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private bool ChangeOTACStoAPP()
        {
            string studyFolderID = Properties.Settings.Default.StudyFolderID; //studyBox.SelectedValue.ToString();
            Dictionary<string, string> filesList = GoogleDriveConnector.GetFiles(_service, studyFolderID);
            string csvFolerID = null;

            foreach (KeyValuePair<string, string> file in filesList)
            {
                string fileName = file.Value;
                if (fileName.ToUpper().Contains("COMPLETION_PROGRESS"))
                {
                    csvFolerID = file.Key;
                    break;
                }
            }

            if (csvFolerID != null)
            {
                var request = _service.Files.Get(csvFolerID);
                var stream = new System.IO.MemoryStream();
                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        Console.WriteLine(progress.BytesDownloaded);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        Console.WriteLine("Download complete.");
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        Console.WriteLine("Download failed.");
                                        break;
                                    }
                            }
                        };
                request.Download(stream);

                stream.Position = 0; // Rewind!
                List<string> rows = new List<string>();
                // Are you *sure* you want ASCII?
                using (var reader = new System.IO.StreamReader(stream, Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("OTACS"))
                        {
                           string newLine = line.Replace("OTACS", "APP");
                           rows.Add(newLine);
                        }
                        else
                        {
                            rows.Add(line);
                        }
                    }
                }

                string newString = "";
                foreach (string row in rows)
                {
                    newString += row;
                    newString += "\r\n";
                }

                try
                {
                    using (System.IO.MemoryStream s = GenerateStreamFromString(newString))
                    {
                        // First retrieve the file from the API.
                        File file = _service.Files.Get(csvFolerID).Execute();

                        // File's new content.
                        //GoogleDriveConnector.UpdateFile(_service, @"C:\Users\CPLDemo\Desktop\completion_progress.csv", csvFolerID, file.MimeType);
                        GoogleDriveConnector.UpdateFileWithStream(_service, s, csvFolerID, file.MimeType);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return false;
                }

            }
            else
            {
                return false;
            }

            return true;

        }

        private void LoadStudy()
        {
            bw2.RunWorkerAsync();
            ShowWaitForm("Loading studies...");
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            dataGridViewSession.DataSource = null;
            dataGridViewSession.DataBindings.Clear();

            textBoxSubject.Text = "";

            LoadStudy();
        }
    }
}
