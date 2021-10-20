using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVIWriterCompressedMulti
{
    public partial class AVIWriterCompressMultiControl : UserControl
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        //Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        AVIWriterCompressMulti plugin;                     //The plugin that this control is providing a user interface for
        private WaitForm waitForm = null;

        public AVIWriterCompressMultiControl(AVIWriterCompressMulti setPlugin)
        {
            InitializeComponent();

            //set the plugin
            plugin = setPlugin;

            //listen to the plugin events
            plugin.saveFileNameChanged += new StringDelegate(OnSaveFileNameChanged);
            // plugin.saveFileNameChanged2 += new StringDelegate(OnSaveFileNameChanged2);
            //plugin.appendStringChanged += new StringDelegate(OnAppendStringChanged);
            //plugin.UpdateAVIWritingStatus += new VoidDelegate(OnUpdateAVIWritingStatus);
            plugin.ShowHideWaitForm += new BoolDelegate(OnShowHideWaitForm);
        }

        void OnShowHideWaitForm(bool show)
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new BoolDelegate(OnShowHideWaitForm), show);
                return;
            }

            if (show)
            {
                waitForm = new WaitForm();
                waitForm.lblMessage.Text = "Preparing thermal video...";
                waitForm.Show();
            }
            else
            {
                waitForm.Close();
                waitForm = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnUpdateAVIWritingStatus()
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnUpdateAVIWritingStatus));
                return;
            }

            btnStartStopAVI.Text = "Start Creating AVI File";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void OnSaveFileNameChanged(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnSaveFileNameChanged), value);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;
            fileNameText.Text = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /*void OnSaveFileNameChanged2(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnSaveFileNameChanged2), value);
                return;
            }

            //Add the new device to the listbox
            //comboBoxFormatsInput.SelectedIndex = val;
            fileNameText2.Text = value;
        }
        */
        ///////////////////////////////////////////////////////////////////////////////////////////
        /*void OnAppendStringChanged(string value)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnAppendStringChanged), value);
                return;
            }

            if (value == null)
            {
                chkBoxAppendFilename.Checked = false;
            }
            else
            {
                chkBoxAppendFilename.Checked = true;

                if (value == "_Perspiration")
                {
                    radBtnPerspiration.Checked = true;
                }

                if (value == "_Breathing")
                {
                    radBtnBreathing.Checked = true;
                }
            }
        }
        */
        protected override void OnBackColorChanged(EventArgs e)
        {
            BackColor = Color.FromArgb(245, 245, 245);
        }        

        private void btnStartStopAVI_Click(object sender, EventArgs e)
        {
          //  plugin.writeAVIFile = true;
            if (plugin.writeAVIFile) 
            {
                btnStartStopAVI.Text = "Start Creating AVI File";
                 
                //plugin.writeAVIFile = false;
                plugin.StartStopAVIWriting();
                 
            }
            else
            {
                btnStartStopAVI.Text = "Stop Creating AVI File...";               
                //plugin.writeAVIFile = true;
                plugin.StartStopAVIWriting();             
            } 
        }

        /*private void chkBoxAppendFilename_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxAppendFilename.Checked)
            {
                radBtnBreathing.Enabled = true;
                radBtnPerspiration.Enabled = true;
            }
            else
            {
                radBtnBreathing.Enabled = false;
                radBtnPerspiration.Enabled = false;
                plugin.appendString = null;
            }
            updateSaveFileName();
            
        }
        */
        private void radBtnPerspiration_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radBtnBreathing_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radBtnPerspiration_Click(object sender, EventArgs e)
        {
            updateSaveFileName();
        }

        private void radBtnBreathing_Click(object sender, EventArgs e)
        {
            updateSaveFileName();
        }

        private void updateSaveFileName()
        {
            if (plugin.saveFileName != null)
            {
                if (plugin.saveFileName.Contains("_Perspiration"))
                {
                    plugin.saveFileName = plugin.saveFileName.Substring(0, plugin.saveFileName.LastIndexOf("_Perspiration")) + ".avi";
                }

                if (plugin.saveFileName.Contains("_Breathing"))
                {
                    plugin.saveFileName = plugin.saveFileName.Substring(0, plugin.saveFileName.LastIndexOf("_Breathing")) + ".avi";
                }

                string tempStr = plugin.saveFileName;
                plugin.saveFileName = tempStr.Substring(0, tempStr.LastIndexOf('.'));
                /*if (chkBoxAppendFilename.Checked)
                {
                    if (radBtnPerspiration.Checked)
                    {
                        plugin.appendString = "_Perspiration";
                    }
                    else
                    {
                        plugin.appendString = "_Breathing";
                    }

                    plugin.saveFileName += plugin.appendString;
                }
                 */
                plugin.saveFileName += ".avi";
                fileNameText.Text = plugin.saveFileName;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
