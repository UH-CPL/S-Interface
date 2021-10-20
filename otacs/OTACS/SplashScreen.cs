using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Host;

namespace OTACS
{
    public partial class SplashScreen : Form
    {
        
        public SplashScreen()
        {
            InitializeComponent();
            timerSplash.Start();
        }

        private void timerSplash_Tick(object sender, EventArgs e)
        {
            if(Global.mainOTACSForm != null)
            {
                timerSplash.Stop();

                Global.mainOTACSForm.Show();

                this.Hide();
            }
            
        }

        public void StartTimer()
        {
            timerSplash.Start();
        }

    }
}
