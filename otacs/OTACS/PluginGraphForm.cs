using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace OTACS
{
    public partial class PluginGraphForm : Form
    {
        private string title;
        private int id;
        private string version;
        private string[] inPins;
        private string[] outPins;
        private PictureBox[] picBoxInPins;
        private PictureBox[] picBoxOutPins;
        private const int startY = 70;
        private const int picBoxSize = 10;
        private int curSelOutputPin = -1;
        private int curSelInputPin = -1;
        private bool isUIVisible = true;

        public event PluginGraphDelegate OutputPinClicked;
        public event PluginGraphDelegate InputPinClicked;
        public event PluginGraphDelegate ShowHideUIButtonClicked;
        public event PluginGraphDelegate PluginGraphMoved;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string[] InPins
        {
            get { return inPins; }
            set { inPins = value; }
        }

        public string[] OutPins
        {
            get { return outPins; }
            set { outPins = value; }
        }

        public PictureBox[] PicBoxInPins
        {
            get { return picBoxInPins; }
            set { picBoxInPins = value; }
        }

        public PictureBox[] PicBoxOutPins
        {
            get { return picBoxOutPins; }
            set { picBoxOutPins = value; }
        }

        public int CurSelOutputPin
        {
            get { return curSelOutputPin; }
            set { curSelOutputPin = value; }
        }

        public int CurSelInputPin
        {
            get { return curSelInputPin; }
            set { curSelInputPin = value; }
        }

        public bool IsUIVisible
        {
            get { return isUIVisible; }
            set 
            { 
                isUIVisible = value;
                if (isUIVisible)
                {
                    btnShowHideUI.Text = "Hide UI";
                }
                else
                {
                    btnShowHideUI.Text = "Show UI";
                }
            }
        }

        public PluginGraphForm()
        {
            InitializeComponent();
        }

        public PluginGraphForm(AvailablePlugin myPlugin)
        {
            InitializeComponent();

            Title = myPlugin.Instance.Name;
            ID = myPlugin.Instance.MyID;
            Version = myPlugin.Instance.Version;
            //Author = myPlugin.Instance.Author;
            ArrayList inP = myPlugin.Instance.InputPins;
            ArrayList outP = myPlugin.Instance.OutputPins;
            InPins = new string[inP.Count];
            OutPins = new string[outP.Count];
            PicBoxInPins = new PictureBox[inP.Count];
            PicBoxOutPins = new PictureBox[outP.Count];

            int yOffset = 13;

            if (isUIVisible)
            {
                btnShowHideUI.Text = "Hide UI";
            }
            else
            {
                btnShowHideUI.Text = "Show UI";
            }

            for (int i = 0; i < inP.Count; i++)
            {
                InPins[i] = ((BasePin)inP[i]).Name;
            }

            for (int i = 0; i < outP.Count; i++)
            {
                OutPins[i] = ((BasePin)outP[i]).Name;
            }

            lblTitle.Text = Title;
            this.Text = "Ver " + Version + " by " + myPlugin.Instance.Author;
            Graphics graphics = this.CreateGraphics();
            int MaxTextWidth = 0;
            for (int i = 0; i < InPins.Length; i++)
            {
                PicBoxInPins[i] = new PictureBox();
                PicBoxInPins[i].Location = new Point(0, startY + (i * (picBoxSize + yOffset)));
                PicBoxInPins[i].Height = picBoxSize;
                PicBoxInPins[i].Width = picBoxSize;
                PicBoxInPins[i].BackColor = Color.Black;
                
                Type[] here = ((BasePin)inP[i]).GetRequiredInterfaces();
                string test = here[0].Name;
                for (int j = 1; j < here.Length; j++)
                    test = test + "\n" +here[j].Name;                
                pinToolTop.SetToolTip(PicBoxInPins[i], test);

                PicBoxInPins[i].MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_InPinsMouseClick);
                this.Controls.Add(PicBoxInPins[i]);

                Label lbl = new Label();
                lbl.Text = InPins[i];
                lbl.AutoSize = true;
                SizeF textSize = graphics.MeasureString(InPins[i], this.Font);
                if ((int)textSize.Width > MaxTextWidth)
                {
                    MaxTextWidth = (int)textSize.Width;
                }
                lbl.Location = new Point(picBoxSize + 2, startY + (i * (picBoxSize + yOffset)) - 2);
                lbl.BackColor = Color.Snow;
                lbl.ForeColor = Color.Black;
                this.Controls.Add(lbl);
            }

            for (int i = 0; i < OutPins.Length; i++)
            {
                PicBoxOutPins[i] = new PictureBox();
                PicBoxOutPins[i].Location = new Point((this.Width - 15) - picBoxSize, startY + (i * (picBoxSize + yOffset)));
                PicBoxOutPins[i].Height = picBoxSize;
                PicBoxOutPins[i].Width = picBoxSize;
                PicBoxOutPins[i].BackColor = Color.SkyBlue;
                PicBoxOutPins[i].Anchor = AnchorStyles.Right|AnchorStyles.Top;

                Type[] here = ((BasePin)outP[i]).GetRequiredInterfaces();
                string test = here[0].Name;
                for (int j = 1; j < here.Length; j++)
                    test = test + "\n" + here[j].Name;
                pinToolTop.SetToolTip(PicBoxOutPins[i], test);

                PicBoxOutPins[i].MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_OutPinsMouseClick);
                this.Controls.Add(PicBoxOutPins[i]);

                
                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = OutPins[i];
                SizeF textSize = graphics.MeasureString(OutPins[i], this.Font);
                if ((int)textSize.Width > MaxTextWidth)
                {
                    MaxTextWidth = (int)textSize.Width;
                }
                lbl.Location = new Point((this.Width - 20) - picBoxSize - (int)textSize.Width, startY + (i * (picBoxSize + yOffset)) - 2);
                lbl.BackColor = Color.Black;
                lbl.ForeColor = Color.SkyBlue;
                lbl.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                this.Controls.Add(lbl);
                
            }

            if (PicBoxInPins.Length >= PicBoxOutPins.Length)
            {
                this.Height = PicBoxInPins[PicBoxInPins.Length - 1].Location.Y + PicBoxInPins[PicBoxInPins.Length - 1].Height + 60;
            }
            else
            {
                this.Height = PicBoxOutPins[PicBoxOutPins.Length - 1].Location.Y + PicBoxOutPins[PicBoxOutPins.Length - 1].Height + 60;
            }
            this.Width = 2 * MaxTextWidth + 2 * picBoxSize + 30;

            //this.TopMost = true;
        }

        private void btnShowHideUI_Click(object sender, EventArgs e)
        {
            //picBox.BackColor = Color.Red;
            /*
            if (IsUIVisible)
            {
                IsUIVisible = false;
                btnShowHideUI.Text = "Show UI";
            }
            else
            {
                IsUIVisible = true;
                btnShowHideUI.Text = "Hide UI";
            }
             */ 
            IsUIVisible = !IsUIVisible;
            ShowHideUIButtonClicked(this);
        }

        private void pictureBox_InPinsMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox pb = (PictureBox)sender;
                for (int i = 0; i < PicBoxInPins.Length; i++)
                {
                    if (PicBoxInPins[i] == pb)
                    {
                        //PicBoxOutPins[i].BackColor = Color.Red;
                        CurSelInputPin = i;
                        InputPinClicked(this);
                        break;
                    }
                }
            }
           
        }

        private void pictureBox_OutPinsMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pb = (PictureBox)sender;
                for (int i = 0; i < PicBoxOutPins.Length; i++)
                {
                    if (PicBoxOutPins[i] == pb)
                    {
                        //PicBoxOutPins[i].BackColor = Color.Red;
                        CurSelOutputPin = i;
                        OutputPinClicked(this);
                        break;
                    }
                }
            }

        }

        private void PluginGraphForm_LocationChanged(object sender, EventArgs e)
        {
            if (PluginGraphMoved != null)
            {
                PluginGraphMoved(this);
            }
        } 

    }
}
