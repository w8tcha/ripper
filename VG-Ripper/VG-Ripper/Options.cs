//////////////////////////////////////////////////////////////////////////
// Code Named: VG-Ripper
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    /// <summary>
    /// The Options Window
    /// </summary>
    public partial class Options : Form
    {
        /// <summary>
        /// The resource manager
        /// </summary>
        private ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            this.InitializeComponent();

            this.LoadSettings();
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
            var cacheController = CacheController.Instance();

            // Load "Show Tray PopUps" Setting
            this.showTrayPopups.Checked = cacheController.UserSettings.ShowPopUps;

            // Load "Download Folder" Setting
            this.textBox2.Text = cacheController.UserSettings.DownloadFolder;

            // Load "Thread Limit" Setting
            this.numericUDThreads.Text = cacheController.UserSettings.ThreadLimit.ToString();
            ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(this.numericUDThreads.Text));

            // min. Image Count for Thanks
            this.numericUDThanks.Text = cacheController.UserSettings.MinImageCount.ToString();

            // Load "Create Subdirctories" Setting
            this.checkBox1.Checked = cacheController.UserSettings.SubDirs;

            // Load "Automaticly Thank You Button" Setting
            if (cacheController.UserSettings.AutoThank)
            {
                this.checkBox8.Checked = true;
            }
            else
            {
                this.checkBox8.Checked = false;
                this.numericUDThanks.Enabled = false;
            }

            // Load "Clipboard Watch" Setting
            this.checkBox10.Checked = cacheController.UserSettings.ClipBWatch;
           
            // Load "Always on Top" Setting
            this.checkBox5.Checked = cacheController.UserSettings.TopMost;
            this.TopMost = cacheController.UserSettings.TopMost;

            // Load "Download each post in its own folder" Setting
            this.mDownInSepFolderChk.Checked = cacheController.UserSettings.DownInSepFolder;

            if (!this.checkBox1.Checked)
            {
                this.mDownInSepFolderChk.Checked = false;
                this.mDownInSepFolderChk.Enabled = false;
            }

            // Load "Save Ripped posts for checking" Setting
            this.saveHistoryChk.Checked = cacheController.UserSettings.SavePids;

            // Load "Show Downloads Complete PopUp" Setting
            this.checkBox9.Checked = cacheController.UserSettings.ShowCompletePopUp;

            // Load Show Last Download Image Setting
            this.checkBox11.Checked = cacheController.UserSettings.ShowLastDownloaded;

            // Load Language Setting
            try
            {
                switch (cacheController.UserSettings.Language)
                {
                    case "de-DE":
                        resourceManager = new ResourceManager("RiPRipper.Languages.german", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 0;
                        pictureBox2.Image = Languages.english.de;
                        break;
                    case "fr-FR":
                        resourceManager = new ResourceManager("RiPRipper.Languages.french", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 1;
                        pictureBox2.Image = Languages.english.fr;
                        break;
                    case "en-EN":
                        resourceManager = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        languageSelector.SelectedIndex = 2;
                        pictureBox2.Image = Languages.english.us;
                        break;
                    default:
                        resourceManager = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
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
        /// Adjusts the culture.
        /// </summary>
        private void AdjustCulture()
        {
            this.groupBox1.Text = this.resourceManager.GetString("downloadOptions", this.culture);
            this.label2.Text = this.resourceManager.GetString("lblDownloadFolder", this.culture);
            this.button4.Text = this.resourceManager.GetString("btnBrowse", this.culture);
            this.checkBox1.Text = this.resourceManager.GetString("chbSubdirectories", this.culture);
            this.checkBox8.Text = this.resourceManager.GetString("chbAutoTKButton", this.culture);
            this.showTrayPopups.Text = this.resourceManager.GetString("chbShowPopUps", this.culture);
            this.checkBox5.Text = this.resourceManager.GetString("chbAlwaysOnTop", this.culture);
            this.checkBox9.Text = this.resourceManager.GetString("chbShowDCPopUps", this.culture);
            this.mDownInSepFolderChk.Text = this.resourceManager.GetString("chbSubThreadRip", this.culture);
            this.saveHistoryChk.Text = this.resourceManager.GetString("chbSaveHistory", this.culture);
            this.label6.Text = this.resourceManager.GetString("lblThreadLimit", this.culture);
            this.label1.Text = this.resourceManager.GetString("lblminImageCount", this.culture);
            this.groupBox3.Text = this.resourceManager.GetString("gbMainOptions", this.culture);
            this.checkBox11.Text = this.resourceManager.GetString("ShowLastDownloaded", this.culture);
            this.checkBox10.Text = this.resourceManager.GetString("clipboardWatch");
        }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LanguageSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.languageSelector.SelectedIndex)
            {
                case 0:
                    this.pictureBox2.Image = Languages.english.de;
                    break;
                case 1:
                    this.pictureBox2.Image = Languages.english.fr;
                    break;
                case 2:
                    this.pictureBox2.Image = Languages.english.us;
                    break;
            }
        }

        /// <summary>
        /// Open Browse Download Folder Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Button4Click(object sender, EventArgs e)
        {
            var cacheController = CacheController.Instance();

            this.FBD.ShowDialog(this);

            if (this.FBD.SelectedPath.Length <= 1)
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
            string mbThreadNum = resourceManager.GetString("mbThreadNum"),
                mbThreadbetw = resourceManager.GetString("mbThreadbetw"),
                mbNum = resourceManager.GetString("mbNum");

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
            Utility.SaveSetting("ShowLastDownloaded", checkBox11.Checked.ToString());
            
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

        /// <summary>
        /// Checks the box8 checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CheckBox8CheckedChanged(object sender, EventArgs e)
        {
            this.numericUDThanks.Enabled = this.checkBox8.Checked;
        }

        /// <summary>
        /// Checks the box1 checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NumericUdThreadsValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(numericUDThreads.Text) <= 20 && Convert.ToInt32(numericUDThreads.Text) >= 1)
            {
                return;
            }

            MessageBox.Show(this, this.resourceManager.GetString("mbThreadbetw"));

            this.numericUDThreads.Text = "3";
        }
    }
}