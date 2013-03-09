//////////////////////////////////////////////////////////////////////////
// Code Named: VG-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

using System;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace RiPRipper
{
    public partial class Options : Form
    {
        private ResourceManager rm;

        public Options()
        {
            InitializeComponent();

            LoadSettings();
        }
        /// <summary>
        /// Set Options
        /// </summary>
        public void LoadSettings()
        {

#if (RIPRIPPERX)
            checkBox9.Enabled = false;
            showTrayPopups.Enabled = false;
            checkBox10.Enabled = false;
#endif
            var cacheController = CacheController.GetInstance();

            // Load "Show Tray PopUps" Setting
            this.showTrayPopups.Checked = cacheController.UserSettings.ShowPopUps;

            // Load "Download Folder" Setting
            textBox2.Text = cacheController.UserSettings.DownloadFolder;

            // Load "Thread Limit" Setting
            numericUDThreads.Text = cacheController.UserSettings.ThreadLimit.ToString();
            ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(numericUDThreads.Text));

            // min. Image Count for Thanks
            numericUDThanks.Text =cacheController.UserSettings.MinImageCount.ToString();

            // Load "Create Subdirctories" Setting
            checkBox1.Checked =cacheController.UserSettings.SubDirs;

            // Load "Automaticly Thank You Button" Setting
            if (cacheController.UserSettings.AutoThank)
            {
                checkBox8.Checked = true;
            }
            else
            {
                checkBox8.Checked = false;
                numericUDThanks.Enabled = false;
            }

            // Load "Clipboard Watch" Setting
            checkBox10.Checked =cacheController.UserSettings.ClipBWatch;
           
            // Load "Always on Top" Setting
            checkBox5.Checked =cacheController.UserSettings.TopMost;
            TopMost =cacheController.UserSettings.TopMost;

            // Load "Download each post in its own folder" Setting
            mDownInSepFolderChk.Checked =cacheController.UserSettings.DownInSepFolder;   

            if (!checkBox1.Checked)
            {
                mDownInSepFolderChk.Checked = false;
                mDownInSepFolderChk.Enabled = false;
            }

            // Load "Save Ripped posts for checking" Setting
            saveHistoryChk.Checked =cacheController.UserSettings.SavePids;

            // Load "Show Downloads Complete PopUp" Setting
            checkBox9.Checked =cacheController.UserSettings.ShowCompletePopUp;

            // Load Language Setting
            try
            {
                switch (cacheController.UserSettings.Language)
                {
                    case "de-DE":
                        rm = new ResourceManager("RiPRipper.Languages.german", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 0;
                        pictureBox2.Image = Languages.english.de;
                        break;
                    case "fr-FR":
                        rm = new ResourceManager("RiPRipper.Languages.french", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 1;
                        pictureBox2.Image = Languages.english.fr;
                        break;
                    case "en-EN":
                        rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 2;
                        pictureBox2.Image = Languages.english.us;
                        break;
                    default:
                        rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 2;
                        pictureBox2.Image = Languages.english.us;
                        break;
                }

                AdjustCulture();
            }
            catch (Exception)
            {
                languageSelector.SelectedIndex = 2;
                pictureBox2.Image = Languages.english.us;
            }
        }
        private void AdjustCulture()
        {
            groupBox1.Text = rm.GetString("downloadOptions", culture);
            label2.Text = rm.GetString("lblDownloadFolder", culture);
            button4.Text = rm.GetString("btnBrowse", culture);
            checkBox1.Text = rm.GetString("chbSubdirectories", culture);
            checkBox8.Text = rm.GetString("chbAutoTKButton", culture);
            showTrayPopups.Text = rm.GetString("chbShowPopUps", culture);
            checkBox5.Text = rm.GetString("chbAlwaysOnTop", culture);
            checkBox9.Text = rm.GetString("chbShowDCPopUps", culture);
            mDownInSepFolderChk.Text = rm.GetString("chbSubThreadRip", culture);
            saveHistoryChk.Text = rm.GetString("chbSaveHistory", culture);
            label6.Text = rm.GetString("lblThreadLimit", culture);
            label1.Text = rm.GetString("lblminImageCount", culture);
            groupBox3.Text = rm.GetString("gbMainOptions", culture);
        }
        /// <summary>
        /// Set Language Strings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            if (languageSelector.SelectedIndex == 0)
            {
                pictureBox2.Image = Languages.english.de;
            }
            if (languageSelector.SelectedIndex == 1)
            {
                pictureBox2.Image = Languages.english.fr;
            }
            if (languageSelector.SelectedIndex == 2)
            {
                pictureBox2.Image = Languages.english.us;
            }
        }

        /// <summary>
        /// Open Browse Download Folder Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Button4Click(object sender, EventArgs e)
        {
            var cacheController = CacheController.GetInstance();

            this.FBD.ShowDialog(this);

            if (FBD.SelectedPath.Length <= 1)
            {
                return;
            }

            this.textBox2.Text = this.FBD.SelectedPath;

            Utility.SaveSetting("Download Folder", this.textBox2.Text);
                
           cacheController.UserSettings.DownloadFolder = this.textBox2.Text;
        }

        /// <summary>
        /// Close Dialog and Save Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            string mbThreadNum = rm.GetString("mbThreadNum"),
                mbThreadbetw = rm.GetString("mbThreadbetw"),
                mbNum = rm.GetString("mbNum");

            if (!Utility.IsNumeric(numericUDThreads.Text))
            {
                MessageBox.Show(this, mbThreadNum);
                return;
            }

            if (!Utility.IsNumeric(numericUDThanks.Text))
            {
                MessageBox.Show(this, mbNum);
                return;
            }

            if (Convert.ToInt32(numericUDThreads.Text) > 20 || Convert.ToInt32(numericUDThreads.Text) < 1)
            {
                MessageBox.Show(this, mbThreadbetw);
                return;
            }

            ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(numericUDThreads.Text));

            Utility.SaveSetting("Thread Limit", numericUDThreads.Text);
            Utility.SaveSetting("minImageCountThanks", numericUDThanks.Text);
            Utility.SaveSetting("SubDirs", checkBox1.Checked.ToString());
            Utility.SaveSetting("Auto TK Button", checkBox8.Checked.ToString());
            Utility.SaveSetting("clipBoardWatch", checkBox10.Checked.ToString());
            Utility.SaveSetting("Show Popups", showTrayPopups.Checked.ToString());
            Utility.SaveSetting("Always OnTop", checkBox5.Checked.ToString());
            Utility.SaveSetting("DownInSepFolder", mDownInSepFolderChk.Checked.ToString());
            Utility.SaveSetting("SaveRippedPosts", saveHistoryChk.Checked.ToString());
            Utility.SaveSetting("Show Downloads Complete PopUp", checkBox9.Checked.ToString());
            
            switch (languageSelector.SelectedIndex)
            {
                case 0:
                    Utility.SaveSetting("UserLanguage", "de-DE");
                    break;
                case 1:
                    Utility.SaveSetting("UserLanguage", "fr-FR");
                    break;
                case 2:
                    Utility.SaveSetting("UserLanguage", "en-EN");
                    break;
            }
              
            Close();
        }

        private void CheckBox8CheckedChanged(object sender, EventArgs e)
        {
            this.numericUDThanks.Enabled = this.checkBox8.Checked;
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                mDownInSepFolderChk.Enabled = true;
            }
            else
            {
                mDownInSepFolderChk.Enabled = false;
                mDownInSepFolderChk.Checked = false;
            }
        }
        /// <summary>
        /// Check if Input is a Number between 1-20
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumericUdThreadsValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(numericUDThreads.Text) <= 20 && Convert.ToInt32(numericUDThreads.Text) >= 1) return;

            MessageBox.Show(this, rm.GetString("mbThreadbetw"));
                
            numericUDThreads.Text = "3";
        }
    }
}