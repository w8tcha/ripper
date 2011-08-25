//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on PG forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher 
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG-Ripper project base.

using System;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace PGRipper
{
    public partial class Options : Form
    {
        private ResourceManager rm;

        public Options()
        {
            InitializeComponent();
            LoadSettings();
        }
        public void LoadSettings()
        {

#if (PGRIPPERX)
            checkBox9.Enabled = false;
            showTrayPopups.Enabled = false;
            checkBox10.Enabled = false;
#endif

            // Load "Show Tray PopUps" Setting
            showTrayPopups.Checked = MainForm.userSettings.bShowPopUps;

            // Load "Download Folder" Setting
            textBox2.Text = MainForm.userSettings.sDownloadFolder;

            // Load "Thread Limit" Setting
            numericUDThreads.Text = MainForm.userSettings.iThreadLimit.ToString();
            ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(numericUDThreads.Text));

            // min. Image Count for Thanks
            numericUDThanks.Text = MainForm.userSettings.iMinImageCount.ToString();

            // Load "Create Subdirctories" Setting
            checkBox1.Checked = MainForm.userSettings.bSubDirs;

            // Load "Automaticly Thank You Button" Setting
            if (MainForm.userSettings.bAutoThank)
            {
                checkBox8.Checked = true;
            }
            else
            {
                checkBox8.Checked = false;
                numericUDThanks.Enabled = false;
            }

            // Load "Clipboard Watch" Setting
            checkBox10.Checked = MainForm.userSettings.bClipBWatch;

            // Load "Always on Top" Setting
            checkBox5.Checked = MainForm.userSettings.bTopMost;
            TopMost = MainForm.userSettings.bTopMost;

            // Load "Download each post in its own folder" Setting
            mDownInSepFolderChk.Checked = MainForm.userSettings.bDownInSepFolder;

            if (!checkBox1.Checked)
            {
                mDownInSepFolderChk.Checked = false;
                mDownInSepFolderChk.Enabled = false;
            }

            // Load "Save Ripped posts for checking" Setting
            saveHistoryChk.Checked = MainForm.userSettings.bSavePids;

            // Load "Show Downloads Complete PopUp" Setting
            checkBox9.Checked = MainForm.userSettings.bShowCompletePopUp;

            // Load Language Setting
            try
            {
                switch (MainForm.userSettings.sLanguage)
                {
                    case "de-DE":
                        rm = new ResourceManager("PGRipper.Languages.german", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 0;
                        pictureBox2.Image = Languages.english.de;
                        break;
                    case "fr-FR":
                        rm = new ResourceManager("PGRipper.Languages.french", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 1;
                        pictureBox2.Image = Languages.english.fr;
                        break;
                    case "en-EN":
                        rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 2;
                        pictureBox2.Image = Languages.english.us;
                        break;
                    default:
                        rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
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
        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            groupBox1.Text = rm.GetString("downloadOptions");
            label2.Text = rm.GetString("lblDownloadFolder");
            button4.Text = rm.GetString("btnBrowse");
            checkBox1.Text = rm.GetString("chbSubdirectories");
            checkBox8.Text = rm.GetString("chbAutoTKButton");
            showTrayPopups.Text = rm.GetString("chbShowPopUps");
            checkBox5.Text = rm.GetString("chbAlwaysOnTop");
            checkBox9.Text = rm.GetString("chbShowDCPopUps");
            mDownInSepFolderChk.Text = rm.GetString("chbSubThreadRip");
            saveHistoryChk.Text = rm.GetString("chbSaveHistory");
            label6.Text = rm.GetString("lblThreadLimit");
            label1.Text = rm.GetString("lblminImageCount");
            groupBox3.Text = rm.GetString("gbMainOptions");

        }
        /// <summary>
        /// Switch Language
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4Click(object sender, EventArgs e)
        {
            FBD.ShowDialog(this);

            if (FBD.SelectedPath.Length <= 1) return;

            textBox2.Text = FBD.SelectedPath;

            Utility.SaveSetting("Download Folder", textBox2.Text);

            MainForm.userSettings.sDownloadFolder = textBox2.Text;
        }
        /// <summary>
        /// Close Dialog and Save Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            try
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
            }
            finally
            {
                Close();
            }
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