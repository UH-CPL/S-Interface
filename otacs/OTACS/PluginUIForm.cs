using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTACS
{
    public partial class PluginUIForm : Form
    {
        private bool isUIVisible = true;

        public bool IsUIVisible
        {
            get { return isUIVisible; }
            set { isUIVisible = value; }
        }

        private bool isDocked = true;

        public bool IsDocked
        {
            get { return isDocked; }
            set 
            { 
                isDocked = value;
                ShowHideDockPin(isDocked);
            }
        }

        public PluginUIForm()
        {
            InitializeComponent();

            if (!IsUIVisible)
            {
                this.Hide();
            
            }

            
        }

        private void btnLockUnlock_Click(object sender, EventArgs e)
        {
            ShowHideDockPin(true);
        }

        public void ShowHideDockPin(bool IsDocked)
        {
            btnLockUnlock.Visible = !IsDocked;
            if (!IsDocked)
            {
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
        }
    }
}
