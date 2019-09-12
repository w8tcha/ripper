// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    using Ripper.Core.Components;

    /// <summary>
    /// Options Dialog
    /// </summary>
    public partial class Options : Form
    {
        /// <summary>
        /// The Resource Manager Instance
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
        /// Loads the settings.
        /// </summary>
        public void LoadSettings()
        {
#if (PGRIPPERX)
            checkBox9.Enabled = false;
            showTrayPopups.Enabled = false;
            checkBox10.Enabled = false;
#endif

            // Load "Show Tray PopUps" Setting
            this.showTrayPopups.Checked = CacheController.Instance().UserSettings.ShowPopUps;

            // Load "Download Folder" Setting
            this.textBox2.Text = CacheController.Instance().UserSettings.DownloadFolder;

            // Load "Thread Limit" Setting
            this.numericUDThreads.Text = CacheController.Instance().UserSettings.ThreadLimit.ToString();
            ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(this.numericUDThreads.Text));

            // min. Image Count for Thanks
            this.numericUDThanks.Text = CacheController.Instance().UserSettings.MinImageCount.ToString();

            // Load "Create Subdirctories" Setting
            this.checkBox1.Checked = CacheController.Instance().UserSettings.SubDirs;

            // Load Show Last Download Image Setting
            this.checkBox11.Checked = CacheController.Instance().UserSettings.ShowLastDownloaded;

            // Load "Automaticly Thank You Button" Setting
            if (CacheController.Instance().UserSettings.AutoThank)
            {
                this.checkBox8.Checked = true;
            }
            else
            {
                this.checkBox8.Checked = false;
                this.numericUDThanks.Enabled = false;
            }

            // Load "Clipboard Watch" Setting
            this.checkBox10.Checked = CacheController.Instance().UserSettings.ClipBWatch;

            // Load "Always on Top" Setting
            this.checkBox5.Checked = CacheController.Instance().UserSettings.TopMost;
            this.TopMost = CacheController.Instance().UserSettings.TopMost;

            // Load "Download each post in its own folder" Setting
            this.mDownInSepFolderChk.Checked = CacheController.Instance().UserSettings.DownInSepFolder;

            if (!this.checkBox1.Checked)
            {
                this.mDownInSepFolderChk.Checked = false;
                this.mDownInSepFolderChk.Enabled = false;
            }

            // Load "Save Ripped posts for checking" Setting
            this.saveHistoryChk.Checked = CacheController.Instance().UserSettings.SavePids;

            // Load "Show Downloads Complete PopUp" Setting
            this.checkBox9.Checked = CacheController.Instance().UserSettings.ShowCompletePopUp;

            // Load Language Setting
            try
            {
                switch (CacheController.Instance().UserSettings.Language)
                {
                    case "de-DE":
                        this.resourceManager = new ResourceManager("Ripper.Languages.german", Assembly.GetExecutingAssembly());
                        this.languageSelector.SelectedIndex = 0;
                        this.pictureBox2.Image = Languages.english.de;
                        break;
                    case "fr-FR":
                        this.resourceManager = new ResourceManager("Ripper.Languages.french", Assembly.GetExecutingAssembly());
                        this.languageSelector.SelectedIndex = 1;
                        this.pictureBox2.Image = Languages.english.fr;
                        break;
                    case "en-EN":
                        this.resourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        this.languageSelector.SelectedIndex = 2;
                        this.pictureBox2.Image = Languages.english.us;
                        break;
                    default:
                        this.resourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        this.languageSelector.SelectedIndex = 2;
                        this.pictureBox2.Image = Languages.english.us;
                        break;
                }

                this.AdjustCulture();
            }
            catch (Exception)
            {
                this.languageSelector.SelectedIndex = 2;
                this.pictureBox2.Image = Languages.english.us;
            }
        }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            this.groupBox1.Text = this.resourceManager.GetString("downloadOptions");
            this.label2.Text = this.resourceManager.GetString("lblDownloadFolder");
            this.button4.Text = this.resourceManager.GetString("btnBrowse");
            this.checkBox1.Text = this.resourceManager.GetString("chbSubdirectories");
            this.checkBox8.Text = this.resourceManager.GetString("chbAutoTKButton");
            this.showTrayPopups.Text = this.resourceManager.GetString("chbShowPopUps");
            this.checkBox5.Text = this.resourceManager.GetString("chbAlwaysOnTop");
            this.checkBox9.Text = this.resourceManager.GetString("chbShowDCPopUps");
            this.mDownInSepFolderChk.Text = this.resourceManager.GetString("chbSubThreadRip");
            this.saveHistoryChk.Text = this.resourceManager.GetString("chbSaveHistory");
            this.label6.Text = this.resourceManager.GetString("lblThreadLimit");
            this.label1.Text = this.resourceManager.GetString("lblminImageCount");
            this.groupBox3.Text = this.resourceManager.GetString("gbMainOptions");
            this.checkBox11.Text = this.resourceManager.GetString("ShowLastDownloaded");
            this.checkBox10.Text = this.resourceManager.GetString("clipboardWatch");
        }

        /// <summary>
        /// Switch Language
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
            this.FBD.ShowDialog(this);

            if (this.FBD.SelectedPath.Length <= 1)
            {
                return;
            }

            this.textBox2.Text = this.FBD.SelectedPath;

            CacheController.Instance().UserSettings.DownloadFolder = this.textBox2.Text;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Close Dialog and Save Changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            try
            {
                string mbThreadNum = this.resourceManager.GetString("mbThreadNum"),
                    mbThreadbetw = this.resourceManager.GetString("mbThreadbetw"),
                    mbNum = this.resourceManager.GetString("mbNum");

                if (!Utility.IsNumeric(this.numericUDThreads.Text))
                {
                    MessageBox.Show(this, mbThreadNum);
                    return;
                }

                if (!Utility.IsNumeric(this.numericUDThanks.Text))
                {
                    MessageBox.Show(this, mbNum);
                    return;
                }

                if (Convert.ToInt32(this.numericUDThreads.Text) > 20 || Convert.ToInt32(this.numericUDThreads.Text) < 1)
                {
                    MessageBox.Show(this, mbThreadbetw);
                    return;
                }

                ThreadManager.GetInstance().SetThreadThreshHold(Convert.ToInt32(this.numericUDThreads.Text));

                CacheController.Instance().UserSettings.ThreadLimit = Convert.ToInt32(this.numericUDThreads.Text);
                CacheController.Instance().UserSettings.MinImageCount = Convert.ToInt32(this.numericUDThanks.Text);
                CacheController.Instance().UserSettings.SubDirs = this.checkBox1.Checked;
                CacheController.Instance().UserSettings.AutoThank = this.checkBox8.Checked;
                CacheController.Instance().UserSettings.ClipBWatch = this.checkBox10.Checked;
                CacheController.Instance().UserSettings.ShowPopUps = this.showTrayPopups.Checked;
                CacheController.Instance().UserSettings.TopMost = this.checkBox5.Checked;
                CacheController.Instance().UserSettings.DownInSepFolder = this.mDownInSepFolderChk.Checked;
                CacheController.Instance().UserSettings.SavePids = this.saveHistoryChk.Checked;
                CacheController.Instance().UserSettings.ShowCompletePopUp = this.checkBox9.Checked;
                CacheController.Instance().UserSettings.ShowLastDownloaded = this.checkBox11.Checked;

                switch (this.languageSelector.SelectedIndex)
                {
                    case 0:
                        CacheController.Instance().UserSettings.Language = "de-DE";
                        break;
                    case 1:
                        CacheController.Instance().UserSettings.Language = "fr-FR";
                        break;
                    case 2:
                        CacheController.Instance().UserSettings.Language = "en-EN";
                        break;
                }
            }
            finally
            {
                SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
                this.Close();
            }
        }

        /// <summary>
        /// Checks the box8 checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CheckBox8CheckedChanged(object sender, EventArgs e)
        {
            this.numericUDThanks.Enabled = this.checkBox8.Checked;
        }

        /// <summary>
        /// Checks the box1 checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.mDownInSepFolderChk.Enabled = true;
            }
            else
            {
                this.mDownInSepFolderChk.Enabled = false;
                this.mDownInSepFolderChk.Checked = false;
            }
        }

        /// <summary>
        /// Check if Input is a Number between 1-20
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void NumericUdThreadsValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(this.numericUDThreads.Text) <= 20 && Convert.ToInt32(this.numericUDThreads.Text) >= 1)
            {
                return;
            }

            MessageBox.Show(this, this.resourceManager.GetString("mbThreadbetw"));

            this.numericUDThreads.Text = "3";
        }
    }
}