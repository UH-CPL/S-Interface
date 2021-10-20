using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Host;

//using MoveGraphLibrary;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Reflection;
using System.IO;

namespace OTACS
{
    public partial class frmOTACS : Form
    {
        const int GAP = 15;
        const int LINE_WIDTH = 10;
        const int INPUTTEXT_INDENT = 15;
        const int OUTPUTTEXT_INDENT = 200;

        Connection curConnection;
        bool connOpen = false;
        bool isPlaying = false;
        bool isDockPinsShown = true;
        long prevCycleCount = 0;
        long curCycleCount = 0;
        bool hideCycleID = true;
        bool ispanelPluginGraphScrolling = false;
        //public delegate void LongDelegate(long value);

       // TwoNodeSquare tns;
       // Mover Movers = new Mover();
        int CurMovingJunction = -1;
        PictureBox CurMovingPB = null;

        private const int yOffsetPinLocation = 35;

        public frmOTACS()
        {
            InitializeComponent();
            CreateDirectory(Application.StartupPath + @"\Plugins");
            CreateDirectory(Application.StartupPath + @"\Configurations");
            CreateDirectory(Application.StartupPath + @"\OtherDLLs");
            LoadPlugins();
            LoadConfigs();
            
            this.WindowState = FormWindowState.Maximized;

        }

        private void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine("That path exists already.");
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory;
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
        }

        private void LoadConfigs()
        {
            cmbBoxConfigs.Items.Clear();
            cmbBoxConfigs.Items.Add("Load Other Configuration..");

            Global.Plugins.PopulateConfigs(Application.StartupPath + @"\Configurations");
            foreach (string config in Global.Plugins.ConfigList)
            {
                String configName = Path.GetFileNameWithoutExtension(config);
                cmbBoxConfigs.Items.Add(configName);
            }
        }

        private void LoadPlugins()
        {
            
            //Call the find plugins routine, to search in our Plugins Folder
            Global.Plugins.PopulatePlugins(Application.StartupPath + @"\Plugins");
            Global.Plugins.CycleIDChanged += new LongDelegate(OnCycleIDChanged);
            Global.Plugins.ThermalPluginErrorOccured += new VoidDelegate(OnThermalPluginErrorOccured);
            foreach (PluginPathName pluginOn in Global.Plugins.PluginList)
            {
                pluginComboBox.Items.Add(pluginOn.Name);    
            }

            Directory.SetCurrentDirectory(Application.StartupPath + @"\OtherDLLs");

            /*
            //Add each plugin to the treeview
            foreach (AvailablePlugin pluginOn in Global.Plugins.AvailablePlugins)
            {
                string foundPlugin = pluginOn.Instance.Name;
                //pluginListBox.Items.Add(foundPlugin);
                pluginComboBox.Items.Add(foundPlugin);

                // Add the plugin user interface
                pluginOn.MyForm.TopLevel = false;
                pluginOn.MyForm.FormBorderStyle = FormBorderStyle.Sizable;
                pluginOn.MyForm.ControlBox = false;
                pluginOn.MyForm.Text = foundPlugin;
                pluginOn.MyForm.Controls.Add(pluginOn.Instance.MainInterface);

                // Add the plugin graph user control
                if (pluginOn.MyGraph == null)
                {
                    pluginOn.MyGraph = new PluginGraphForm(pluginOn);
                    pluginOn.MyGraph.TopLevel = false;
                    //pluginOn.MyGraph.FormBorderStyle = FormBorderStyle.Sizable;
                    pluginOn.MyGraph.FormBorderStyle = FormBorderStyle.FixedDialog;
                    pluginOn.MyGraph.ControlBox = false;
                    pluginOn.MyGraph.Text = "  ";
                    pluginOn.MyGraph.OutputPinClicked += new PluginGraphDelegate(OnOutputPinClicked);
                    pluginOn.MyGraph.InputPinClicked += new PluginGraphDelegate(OnInputPinClicked);
                    pluginOn.MyGraph.ShowHideUIButtonClicked += new PluginGraphDelegate(OnShowHideUIButtonClicked);
                    pluginOn.MyGraph.PluginGraphMoved += new PluginGraphDelegate(OnPluginGraphMoved);
                }
            }
             */
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            //check whether the cycle is running and stop it
             if (isPlaying)
            {
                isPlaying = false;
                Global.Plugins.PlayPauseCycle(false);
                btnPlayPauseCycle.Text = "Start";
                timerCyclesPerSec.Stop();
                //lblCyclesPerSec.Text = "0";

            }
            //Lets call the close for all the plugins before we truly exit!
            Global.Plugins.ClosePlugins();
            this.Close();
            Application.Exit();
        }

        
        private void btnPlayPauseCycle_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                isPlaying = false;
                Global.Plugins.PlayPauseCycle(false);
                btnPlayPauseCycle.Text = "Start";
                timerCyclesPerSec.Stop();
                //lblCyclesPerSec.Text = "0";
                
            }
            else
            {
                isPlaying = true;
                Global.Plugins.PlayPauseCycle(true);
                btnPlayPauseCycle.Text = "Stop";
                timerCyclesPerSec.Start();
            }
        }

        public void OnCycleIDChanged(long cycleID)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new LongDelegate(OnCycleIDChanged), cycleID);
                return;
            }

            //display the value
            curCycleCount = cycleID;
            if (!hideCycleID)
            {
                //lblCycleID.Text = curCycleCount.ToString();
            }
        }

        public void OnThermalPluginErrorOccured()
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnThermalPluginErrorOccured));
                return;
            }

            MessageBox.Show("No Thermal Camera/Thermal File plugin detected as source plugin!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void OnOutputPinClicked(PluginGraphForm graph)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new PluginGraphDelegate(OnOutputPinClicked), graph);
                return;
            }

            AvailablePlugin clickedPlugin = Global.Plugins.AvailablePlugins.Find(graph.Title, graph.ID);
            BasePin clickedPin = (BasePin)clickedPlugin.Instance.OutputPins[graph.CurSelOutputPin];
            if (!clickedPin.Connected)
            {
                if (connOpen)
                {

                    bool singleConnection = true;
                    for (int i = 0; i < curConnection.Junctions.Count; i++)
                    {
                        PictureBox connPB = (PictureBox)curConnection.Junctions[i];
                        if (CanRemove(connPB, -1))
                        {
                            panelPluginGraph.Controls.Remove(connPB);
                        }
                        else
                        {
                            singleConnection = false;
                        }
                    }

                    if (singleConnection)
                    {
                        curConnection.outPin.Connected = false;
                    } 
                }

                curConnection = new Connection();
                curConnection.outPlugin = clickedPlugin;
                curConnection.outPin = clickedPin;
                curConnection.outputPinIndex = graph.CurSelOutputPin;
                curConnection.outPin.Connected = true;
                connOpen = true;

                curConnection.Junctions = new System.Collections.ArrayList();

                PictureBox picBox = new PictureBox();
                Point picBoxLoc1 = new Point(graph.PicBoxOutPins[graph.CurSelOutputPin].Location.X + graph.Location.X, graph.PicBoxOutPins[graph.CurSelOutputPin].Location.Y + graph.Location.Y + yOffsetPinLocation);
                picBox.Location = picBoxLoc1;
                picBox.Height = 10;
                picBox.Width = 10;
                picBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                picBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                picBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
                curConnection.Junctions.Add(picBox);

                Point picBoxLoc2 = new Point(picBoxLoc1.X + 30, picBoxLoc1.Y);
                PictureBox picBox2 = new PictureBox();
                picBox2.Location = picBoxLoc2;
                picBox2.Height = 10;
                picBox2.Width = 10;
                curConnection.Junctions.Add(picBox2);
                picBox2.BackColor = Color.Black;
                picBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                picBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                picBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
                panelPluginGraph.Controls.Add(picBox2);

                CurMovingJunction = -1;
                CurMovingPB = null;

                panelPluginGraph.Refresh();
            }

        }


        public void OnInputPinClicked(PluginGraphForm graph)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new PluginGraphDelegate(OnInputPinClicked), graph);
                return;
            }

            if (connOpen && CurMovingJunction != -1)
            {
                AvailablePlugin clickedPlugin = Global.Plugins.AvailablePlugins.Find(graph.Title, graph.ID);
                BasePin clickedPin = (BasePin)clickedPlugin.Instance.InputPins[graph.CurSelInputPin];
                if (!clickedPin.Connected)
                {
                    curConnection.inPlugin = Global.Plugins.AvailablePlugins.Find(graph.Title, graph.ID);
                    curConnection.inPin = (BasePin)curConnection.inPlugin.Instance.InputPins[graph.CurSelInputPin];
                    curConnection.inputPinIndex = graph.CurSelInputPin;
                    curConnection.inPin.Connected = true;


                    Global.Plugins.CurConfiguration.PluginConnections.Add(curConnection);

                    if (Global.Plugins.ThermalPlugin == null)
                    {
                        if ((curConnection.outPlugin.Instance.Name == "Thermal File") || (curConnection.outPlugin.Instance.Name == "Thermal Camera") || (curConnection.outPlugin.Instance.Name == "SFMOV File"))
                        {
                            Global.Plugins.ThermalPlugin = curConnection.outPlugin;
                        }

                        if ((curConnection.inPlugin.Instance.Name == "Thermal File") || (curConnection.inPlugin.Instance.Name == "Thermal Camera") || (curConnection.inPlugin.Instance.Name == "SFMOV File"))
                        {
                            Global.Plugins.ThermalPlugin = curConnection.inPlugin;
                        }
                    }

                    Point picBoxLoc1 = new Point(graph.PicBoxInPins[graph.CurSelInputPin].Location.X + graph.Location.X, graph.PicBoxInPins[graph.CurSelInputPin].Location.Y + graph.Location.Y + yOffsetPinLocation);
                    PictureBox connPB = (PictureBox)curConnection.Junctions[CurMovingJunction];
                    connPB.Location = picBoxLoc1;
                    connPB.BackColor = Color.Black;

                    CurMovingJunction = -1;
                    CurMovingPB = null;
                    connOpen = false;

                    panelPluginGraph.Refresh();
                }
                else
                {
                    MessageBox.Show("Input pin is already connected!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }

        public void OnShowHideUIButtonClicked(PluginGraphForm graph)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new PluginGraphDelegate(OnShowHideUIButtonClicked), graph);
                return;
            }

            AvailablePlugin plugin = Global.Plugins.AvailablePlugins.Find(graph.Title, graph.ID);
            plugin.MyForm.Visible = graph.IsUIVisible;          

        }

        public void OnPluginGraphMoved(PluginGraphForm graph)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new PluginGraphDelegate(OnPluginGraphMoved), graph);
                return;
            }

 
            //for(int i=0; i<Global.Plugins.C
            AvailablePlugin curPlugin = Global.Plugins.AvailablePlugins.Find(graph.Title, graph.ID);
            for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
            {
                Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                if (conn.outPlugin == curPlugin)
                {
                    Point picBoxLoc = new Point(graph.PicBoxOutPins[conn.outputPinIndex].Location.X + graph.Location.X, graph.PicBoxOutPins[conn.outputPinIndex].Location.Y + graph.Location.Y + yOffsetPinLocation);
                    PictureBox picBox = (PictureBox)conn.Junctions[0];
                    picBox.Location = picBoxLoc; 
                }
                if (conn.inPlugin == curPlugin)
                {
                    Point picBoxLoc = new Point(graph.PicBoxInPins[conn.inputPinIndex].Location.X + graph.Location.X, graph.PicBoxInPins[conn.inputPinIndex].Location.Y + graph.Location.Y + yOffsetPinLocation);
                    PictureBox picBox = (PictureBox)conn.Junctions[conn.Junctions.Count-1];
                    picBox.Location = picBoxLoc;
                }
            }

            if (connOpen)
            {
                if (curConnection.outPlugin == curPlugin)
                {
                    Point picBoxLoc = new Point(graph.PicBoxOutPins[curConnection.outputPinIndex].Location.X + graph.Location.X, graph.PicBoxOutPins[curConnection.outputPinIndex].Location.Y + graph.Location.Y + yOffsetPinLocation);
                    PictureBox picBox = (PictureBox)curConnection.Junctions[0];
                    picBox.Location = picBoxLoc;
                }
            }

            panelPluginGraph.Refresh();      
        }



        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            if (connOpen && e.Button == MouseButtons.Left)
            {
                PictureBox pb = (PictureBox)sender;
                for (int i = 0; i < curConnection.Junctions.Count; i++)
                {
                    PictureBox connPB = (PictureBox)curConnection.Junctions[i];
                    if (connPB == pb)
                    {
                        CurMovingJunction = i;
                        break;
                    }
                }
            }
            */
        }

        

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            /*
            if (connOpen && CurMovingJunction != -1 && e.Button == MouseButtons.Left)
            {
                PictureBox pb = (PictureBox)sender;
                PictureBox connPB = (PictureBox)curConnection.Junctions[CurMovingJunction];
                if (connPB == pb)
                {
                    //CurMovingJunction = -1;
                }
            }
            */

        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurMovingPB != null)
            {
                CurMovingPB.BackColor = Color.Black;

                CurMovingPB = null;

                panelPluginGraph.Refresh();
            }

            if (e.Button == MouseButtons.Right)
            {
                PictureBox pb = (PictureBox)sender;
                if (connOpen)
                {  
                    if (pb == (PictureBox)curConnection.Junctions[curConnection.Junctions.Count - 1])
                    {
                        Point picBoxLoc2 = new Point(pb.Location.X + 30, pb.Location.Y);
                        PictureBox picBox2 = new PictureBox();
                        picBox2.Location = picBoxLoc2;
                        picBox2.Height = 10;
                        picBox2.Width = 10;
                        curConnection.Junctions.Add(picBox2);
                        picBox2.BackColor = Color.Black;
                        picBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                        picBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                        picBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
                        panelPluginGraph.Controls.Add(picBox2);
                    }
                }
                else
                {
                    
                    for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
                    {
                        bool found = false;
                        int index = 0;
                        Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                        for (int j = 0; j < conn.Junctions.Count - 1; j++)
                        {
                            PictureBox connPB = (PictureBox)conn.Junctions[j];
                            if (connPB == pb)
                            {
                                index = j;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            curConnection = new Connection();

                            curConnection.outPlugin = conn.outPlugin;
                            curConnection.outPin = conn.outPin;
                            curConnection.outputPinIndex = conn.outputPinIndex;
                            curConnection.outPin.Connected = true;
                            
                            curConnection.Junctions = new System.Collections.ArrayList();
                            for (int j = 0; j <= index; j++)
                            {
                                PictureBox connPB = (PictureBox)conn.Junctions[j];
                                curConnection.Junctions.Add(connPB);
                                
                            }

                            Point picBoxLoc2 = new Point(pb.Location.X + 30, pb.Location.Y);
                            PictureBox picBox2 = new PictureBox();
                            picBox2.Location = picBoxLoc2;
                            picBox2.Height = 10;
                            picBox2.Width = 10;
                            curConnection.Junctions.Add(picBox2);
                            picBox2.BackColor = Color.Black;
                            picBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                            picBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                            picBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
                            panelPluginGraph.Controls.Add(picBox2);

                            CurMovingJunction = -1;
                            CurMovingPB = null;
                            connOpen = true;
                            break;
                        }
                    }
                }


                panelPluginGraph.Refresh();
            }
            else if (e.Button == MouseButtons.Left)
            {
                bool found = false;
                PictureBox pb = (PictureBox)sender;
                if (connOpen)
                {
                    for (int i = 0; i < curConnection.Junctions.Count; i++)
                    {
                        PictureBox connPB = (PictureBox)curConnection.Junctions[i];
                        if (connPB == pb)
                        {
                            pb.BackColor = Color.Red;
                            CurMovingJunction = i;
                            CurMovingPB = connPB;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
                    {                        
                        Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                        for (int j = 0; j < conn.Junctions.Count - 1; j++)
                        {
                            PictureBox connPB = (PictureBox)conn.Junctions[j];
                            if (connPB == pb)
                            {
                                pb.BackColor = Color.Red;
                                CurMovingJunction = i;
                                CurMovingPB = connPB;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                }
                

            }

        }

        
        private void panelPluginGraph_Paint(object sender, PaintEventArgs e)
        {
            if (!ispanelPluginGraphScrolling)
            {
                Pen thickBlackPen = new Pen(Color.Black, 1);
                Graphics g = panelPluginGraph.CreateGraphics();
                for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
                {
                    Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                    for (int j = 0; j < conn.Junctions.Count - 1; j++)
                    {
                        PictureBox picBox1 = (PictureBox)conn.Junctions[j];
                        PictureBox picBox2 = (PictureBox)conn.Junctions[j + 1];
                        g.DrawLine(thickBlackPen, picBox1.Location.X + 5, picBox1.Location.Y + 5, picBox2.Location.X + 5, picBox2.Location.Y + 5);
                    }
                }
                if (connOpen)
                {
                    for (int j = 0; j < curConnection.Junctions.Count - 1; j++)
                    {
                        PictureBox picBox1 = (PictureBox)curConnection.Junctions[j];
                        PictureBox picBox2 = (PictureBox)curConnection.Junctions[j + 1];
                        g.DrawLine(thickBlackPen, picBox1.Location.X + 5, picBox1.Location.Y + 5, picBox2.Location.X + 5, picBox2.Location.Y + 5);
                    }
                }
                g.Dispose();
            }
             
        }

        private void panelPluginGraph_MouseDown(object sender, MouseEventArgs e)
        {
            //if (connOpen)
            //{
            //    Movers.CatchMover(e.Location);
            //}
        }

        private void panelPluginGraph_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (CurMovingPB != null)
            {
                CurMovingPB.Location = new Point(e.Location.X - 5, e.Location.Y - 5);
                CurMovingPB.BackColor = Color.Black;

                panelPluginGraph.Refresh();
            }
             * */
        }

        private void panelPluginGraph_MouseUp(object sender, MouseEventArgs e)
        {
            //if (connOpen)
            //{
            //    Movers.ReleaseMover();
            //}

            
        }

        private void panelPluginGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /*
                if (CurMovingJunction != -1)
                {
                    PictureBox connPB = (PictureBox)curConnection.Junctions[CurMovingJunction];
                    connPB.Location = new Point(e.Location.X - 5, e.Location.Y - 5);
                    connPB.BackColor = Color.Black;

                    panelPluginGraph.Refresh();
                }
                 */

                if (CurMovingPB != null)
                {
                    CurMovingPB.Location = new Point(e.Location.X - 5, e.Location.Y - 5);
                    CurMovingPB.BackColor = Color.Black;

                    CurMovingPB = null;

                    panelPluginGraph.Refresh();
                }
            }
            else
            {
                ispanelPluginGraphScrolling = false;

                panelPluginGraph.Refresh();
            }
        }


        private void pluginComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string assemblyPath = Global.Plugins.GetAssemblyPath(pluginComboBox.SelectedItem.ToString());
            if (assemblyPath != null)
            {
                Global.Plugins.AddPlugin(assemblyPath, Global.Plugins.UniquePluginID, null);
                Global.Plugins.UniquePluginID++;

                //Get the selected Plugin
                AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.PluginAt(Global.Plugins.AvailablePlugins.Count - 1);
                
                // Add the plugin user interface
                selectedPlugin.MyForm.TopLevel = false;
                selectedPlugin.MyForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                selectedPlugin.MyForm.ControlBox = false;
                selectedPlugin.MyForm.Text = selectedPlugin.Instance.Name;
                selectedPlugin.MyForm.Size = new Size(selectedPlugin.Instance.MainInterface.Size.Width + 20, selectedPlugin.Instance.MainInterface.Size.Height + 40);
                selectedPlugin.Instance.MainInterface.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
                selectedPlugin.MyForm.Controls.Add(selectedPlugin.Instance.MainInterface);

                // Add the plugin graph user control
                if (selectedPlugin.MyGraph == null)
                {
                    selectedPlugin.MyGraph = new PluginGraphForm(selectedPlugin);
                    selectedPlugin.MyGraph.TopLevel = false;
                    //pluginOn.MyGraph.FormBorderStyle = FormBorderStyle.Sizable;
                    selectedPlugin.MyGraph.FormBorderStyle = FormBorderStyle.FixedDialog;
                    selectedPlugin.MyGraph.ControlBox = false;
                    selectedPlugin.MyGraph.OutputPinClicked += new PluginGraphDelegate(OnOutputPinClicked);
                    selectedPlugin.MyGraph.InputPinClicked += new PluginGraphDelegate(OnInputPinClicked);
                    selectedPlugin.MyGraph.ShowHideUIButtonClicked += new PluginGraphDelegate(OnShowHideUIButtonClicked);
                    selectedPlugin.MyGraph.PluginGraphMoved += new PluginGraphDelegate(OnPluginGraphMoved);
                }

                if (selectedPlugin != null)
                {
                    //Again, if the plugin is found, do some work...

                    if (!selectedPlugin.MyForm.IsDisposed)
                    {
                        this.panelPlugin1.Controls.Add(selectedPlugin.MyForm);
                        selectedPlugin.MyForm.Show();
                    }

                    selectedPlugin.MyForm.BringToFront();
                    this.panelPluginGraph.Controls.Add(selectedPlugin.MyGraph);
                    selectedPlugin.MyGraph.Show();
                    selectedPlugin.MyGraph.BringToFront();

                }
            }
        }

        private void LoadConfiguration(string configFileName)
        {
            if (Global.Plugins.LoadConfiguration(configFileName))
            {
                panelPlugin1.Controls.Clear();
                panelPluginGraph.Controls.Clear();
                Global.Plugins.UniquePluginID = Global.Plugins.CurConfiguration.UniqueID;
                for (int i = 0; i < Global.Plugins.CurConfiguration.PluginParams.Count; i++)
                {
                    PluginParameters ppm = (PluginParameters)Global.Plugins.CurConfiguration.PluginParams[i];
                    //AvailablePlugin curPlugin = Global.Plugins.AvailablePlugins.Find(ppm.pluginName);
                    string assemblyPath = Global.Plugins.GetAssemblyPath(ppm.pluginName);
                    if (assemblyPath != null)
                    {
                        //String serialFilePath = Path.GetDirectoryName(dialog.FileName);
                        //String configName = Path.GetFileNameWithoutExtension(dialog.FileName);
                        //String serialFilename = serialFilePath + "\\" + configName + "_" + ppm.pluginName + "-" + ppm.pluginID + ".serial";

                        String serialFilePath = Path.GetDirectoryName(configFileName);
                        String configName = Path.GetFileNameWithoutExtension(configFileName);
                        String newFolder = serialFilePath + "\\" + configName;
                        String serialFilename = newFolder + "\\" + ppm.pluginName + "_" + ppm.pluginID + ".serial";

                        Global.Plugins.AddPlugin(assemblyPath, ppm.pluginID, serialFilename);

                        //Get the selected Plugin
                        AvailablePlugin curPlugin = Global.Plugins.AvailablePlugins.PluginAt(Global.Plugins.AvailablePlugins.Count - 1);

                        // Add the plugin user interface
                        curPlugin.MyForm.TopLevel = false;
                        curPlugin.MyForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                        curPlugin.MyForm.ControlBox = false;
                        curPlugin.MyForm.Text = curPlugin.Instance.Name;
                        //curPlugin.MyForm.Size = new Size(curPlugin.Instance.MainInterface.Size.Width + 20, curPlugin.Instance.MainInterface.Size.Height + 40);
                        curPlugin.MyForm.Size = new Size(curPlugin.Instance.MainInterface.Size.Width + 20, curPlugin.Instance.MainInterface.Size.Height + 40);
                        curPlugin.Instance.MainInterface.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
                        curPlugin.MyForm.Controls.Add(curPlugin.Instance.MainInterface);

                        // Add the plugin graph user control
                        if (curPlugin.MyGraph == null)
                        {
                            curPlugin.MyGraph = new PluginGraphForm(curPlugin);
                            curPlugin.MyGraph.TopLevel = false;
                            //pluginOn.MyGraph.FormBorderStyle = FormBorderStyle.Sizable;
                            curPlugin.MyGraph.FormBorderStyle = FormBorderStyle.FixedDialog;
                            curPlugin.MyGraph.ControlBox = false;
                            //curPlugin.MyGraph.Text = "  ";
                            curPlugin.MyGraph.OutputPinClicked += new PluginGraphDelegate(OnOutputPinClicked);
                            curPlugin.MyGraph.InputPinClicked += new PluginGraphDelegate(OnInputPinClicked);
                            curPlugin.MyGraph.ShowHideUIButtonClicked += new PluginGraphDelegate(OnShowHideUIButtonClicked);
                            curPlugin.MyGraph.PluginGraphMoved += new PluginGraphDelegate(OnPluginGraphMoved);
                        }

                        curPlugin.MyForm.Size = ppm.UISize;
                        curPlugin.MyForm.Location = ppm.UILocation;
                        panelPlugin1.Controls.Add(curPlugin.MyForm);
                        curPlugin.MyForm.IsUIVisible = ppm.isUIVisible;
                        if (curPlugin.MyForm.IsUIVisible)
                        {
                            curPlugin.MyForm.Show();
                        }
                        curPlugin.MyForm.IsDocked = true;

                        curPlugin.MyGraph.Size = ppm.GraphSize;
                        curPlugin.MyGraph.Location = ppm.GraphLocation;
                        curPlugin.MyGraph.IsUIVisible = ppm.isUIVisible;
                        panelPluginGraph.Controls.Add(curPlugin.MyGraph);
                        curPlugin.MyGraph.Show();

                        isDockPinsShown = false;
                        btnShowHideDockPins.Text = "Show All Dock Pins";

                        panelPluginGraph.Refresh();
                    }


                }

                for (int i = 0; i < Global.Plugins.CurConfiguration.ConnParams.Count; i++)
                {
                    ConnectionParameters ccm = (ConnectionParameters)Global.Plugins.CurConfiguration.ConnParams[i];
                    Connection curConn = new Connection();

                    AvailablePlugin inPlugin = Global.Plugins.AvailablePlugins.Find(ccm.inPluginName, ccm.inPluginID);
                    curConn.inPlugin = inPlugin;
                    curConn.inputPinIndex = ccm.inPinIndex;
                    curConn.inPin = (BasePin)inPlugin.Instance.InputPins[ccm.inPinIndex];
                    curConn.inPin.Connected = true;

                    AvailablePlugin outPlugin = Global.Plugins.AvailablePlugins.Find(ccm.outPluginName, ccm.outPluginID);
                    curConn.outPlugin = outPlugin;
                    curConn.outputPinIndex = ccm.outPinIndex;
                    curConn.outPin = (BasePin)outPlugin.Instance.OutputPins[ccm.outPinIndex];
                    curConn.outPin.Connected = true;

                    curConn.Junctions = new System.Collections.ArrayList();
                    for (int j = 0; j < ccm.ConnPicBoxLocations.Length; j++)
                    {
                        PictureBox picBox = new PictureBox();
                        picBox.Location = ccm.ConnPicBoxLocations[j];
                        picBox.Height = 10;
                        picBox.Width = 10;
                        picBox.BackColor = Color.Black;
                        picBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                        picBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                        picBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
                        curConn.Junctions.Add(picBox);
                        if ((j != 0) && (j != ccm.ConnPicBoxLocations.Length - 1))
                        {
                            panelPluginGraph.Controls.Add(picBox);
                        }
                    }

                    Global.Plugins.CurConfiguration.PluginConnections.Add(curConn);

                    if (Global.Plugins.ThermalPlugin == null)
                    {
                        if ((curConn.outPlugin.Instance.Name == "Thermal File") || (curConn.outPlugin.Instance.Name == "Thermal Camera") || (curConn.outPlugin.Instance.Name == "SFMOV File") || (curConn.outPlugin.Instance.Name == "GiGE Thermal Camera"))
                        {
                            Global.Plugins.ThermalPlugin = curConn.outPlugin;
                        }

                        if ((curConn.inPlugin.Instance.Name == "Thermal File") || (curConn.inPlugin.Instance.Name == "Thermal Camera") || (curConn.inPlugin.Instance.Name == "SFMOV File") || (curConn.inPlugin.Instance.Name == "GiGE Thermal Camera"))
                        {
                            Global.Plugins.ThermalPlugin = curConn.inPlugin;
                        }
                    }

                    panelPluginGraph.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Unable to load configuration file '" + configFileName + "'!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Configuration files (*.XML)|*.XML|All files (*.*)|*.*";
            dialog.Title = "Select a configuration file";
            
           if(dialog.ShowDialog() == DialogResult.OK)
           {
               LoadConfiguration(dialog.FileName);
                
           }
           

        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            // Create new SaveFileDialog object
            SaveFileDialog DialogSave = new SaveFileDialog();

            // Default file extension
            DialogSave.DefaultExt = "XML";

            // Available file extensions
            DialogSave.Filter = "XML files (*.XML)|*.XML|All files (*.*)|*.*";

            // Adds a extension if the user does not
            DialogSave.AddExtension = true;

            // Restores the selected directory, next time
            //DialogSave.RestoreDirectory = true;

            // Dialog title
            DialogSave.Title = "Where do you want to save the configuration file?";

            // Startup directory
            DialogSave.InitialDirectory = Application.StartupPath + @"\Configurations";

            // Show the dialog and process the result
            if (DialogSave.ShowDialog() == DialogResult.OK)
            {
                panelPluginGraph.AutoScrollPosition = new Point(0, 0);
                //panelPluginGraph.Refresh();
                panelPlugin1.AutoScrollPosition = new Point(0, 0);
                //panelPlugin1.Refresh();
                if (Global.Plugins.SaveConfiguration(DialogSave.FileName))
                {
                    MessageBox.Show("Configuration saved to XML file '" + DialogSave.FileName + "'!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Unable to save configuration!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("You hit cancel or closed the dialog.");
            }

            DialogSave.Dispose();
            DialogSave = null;
        }

        private void btnShowHideDockPins_Click(object sender, EventArgs e)
        {
            if (isDockPinsShown)
            {
                isDockPinsShown = false;
                btnShowHideDockPins.Text = "Show All DockPins";
            }
            else
            {
                isDockPinsShown = true;
                btnShowHideDockPins.Text = "Hide All DockPins";
            }

            foreach (AvailablePlugin pluginOn in Global.Plugins.AvailablePlugins)
            {

                // Add the plugin user interface
                pluginOn.MyForm.ShowHideDockPin(!isDockPinsShown);
            }
        }

        private void btnNewConfig_Click(object sender, EventArgs e)
        {
            panelPlugin1.Controls.Clear();
            panelPluginGraph.Controls.Clear();
            Global.Plugins.CurConfiguration = new Configuration();
        }

        private void panelPluginGraph_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void frmOTACS_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurMovingPB != null)
            {
                bool found = false;
                if (connOpen)
                {
                    for (int i = 0; i < curConnection.Junctions.Count; i++)
                    {
                        PictureBox connPB = (PictureBox)curConnection.Junctions[i];
                        if (connPB == CurMovingPB)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        bool singleConnection = true;

                        for (int i = 0; i < curConnection.Junctions.Count; i++)
                        {
                            PictureBox connPB = (PictureBox)curConnection.Junctions[i];
                            if (CanRemove(connPB, -1))
                            {
                                panelPluginGraph.Controls.Remove(connPB);
                            }
                            else
                            {
                                singleConnection = false;
                            }
                        }

                        if (singleConnection)
                        {
                            curConnection.outPin.Connected = false;
                        } 
                        curConnection = new Connection();
                        connOpen = false;
                    }
                }
                ArrayList foundArray = new ArrayList();
                for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
                {
                    found = false;
                    Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                    for (int j = 0; j < conn.Junctions.Count - 1; j++)
                    {
                        PictureBox connPB = (PictureBox)conn.Junctions[j];
                        if (connPB == CurMovingPB)
                        {
                            foundArray.Add(conn);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        bool singleConnection = true;
                        for (int j = 0; j < conn.Junctions.Count; j++)
                        {
                            PictureBox connPB = (PictureBox)conn.Junctions[j];
                            if (CanRemove(connPB, i))
                            {
                                panelPluginGraph.Controls.Remove(connPB);
                            }
                            else
                            {
                                singleConnection = false;
                            }
                        }
                        if (singleConnection)
                        {
                            conn.outPin.Connected = false;
                        }
                        conn.inPin.Connected = false;
                    }

                }

                
                for (int i = 0; i < foundArray.Count; i++)
                {
                    Connection conn = (Connection)foundArray[i];
                    Global.Plugins.CurConfiguration.PluginConnections.Remove(conn);
                }

                panelPluginGraph.Refresh();
            }
        }

        private bool CanRemove(PictureBox pb, int index)
        {
            for (int i = 0; i < Global.Plugins.CurConfiguration.PluginConnections.Count; i++)
            {
                if (i != index)
                {
                    Connection conn = (Connection)Global.Plugins.CurConfiguration.PluginConnections[i];
                    for (int j = 0; j < conn.Junctions.Count; j++)
                    {
                        PictureBox connPB = (PictureBox)conn.Junctions[j];
                        if (connPB == pb)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void timerCyclesPerSec_Tick(object sender, EventArgs e)
        {
            long cyclesPerSec = curCycleCount - prevCycleCount;
            prevCycleCount = curCycleCount;
            //lblCyclesPerSec.Text = cyclesPerSec.ToString();
        }

        /*
        private void lblCycleID_Click(object sender, EventArgs e)
        {
            hideCycleID = !hideCycleID;
            if (hideCycleID)
            {
                lblCycleID.Text = "";
            }
            else
            {
                lblCycleID.Text = curCycleCount.ToString();
            }
        }
        */

        private void panelPluginGraph_Scroll(object sender, ScrollEventArgs e)
        {
            ispanelPluginGraphScrolling = true;
            
        }

        private void cmbBoxConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = cmbBoxConfigs.SelectedIndex;

            if (selection >= 0)
            {
                if (selection == 0)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter =
                       "Configuration files (*.XML)|*.XML|All files (*.*)|*.*";
                    dialog.Title = "Select a configuration file";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadConfiguration(dialog.FileName);

                    }
                }
                else
                {
                    LoadConfiguration((string)Global.Plugins.ConfigList[selection - 1]);
                }
            }
        }

        private void cmbBoxConfigs_Click(object sender, EventArgs e)
        {
            
        }

        private void frmOTACS_FormClosing(object sender, FormClosingEventArgs e)
        {
            //check whether the cycle is running and stop it
            if (isPlaying)
            {
                isPlaying = false;
                Global.Plugins.PlayPauseCycle(false);
                btnPlayPauseCycle.Text = "Start";
                timerCyclesPerSec.Stop();
                //lblCyclesPerSec.Text = "0";

            }
            //Lets call the close for all the plugins before we truly exit!
            Global.Plugins.ClosePlugins();
            Application.Exit();
        }

        
    }
}
