// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="The Watcher">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Resources;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Ripper.Core.Objects;
    using Ripper.Core.Components;
    using Ripper.Services;

#if (!PGRIPPERX)
    using Microsoft.WindowsAPICodePack.Taskbar;
#endif

    /// <summary>
    /// The Main Form
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// String List of Urls to Rip
        /// </summary>
        private readonly List<string> ExtractUrls = new List<string>();

        /// <summary>
        /// Array List of all Stored (Ripped) Post Ids
        /// </summary>
        private readonly ArrayList sRippedPosts = new ArrayList();

#if (!PGRIPPERX)
        /// <summary>
        /// TaskBar Manager Instance
        /// </summary>
        private TaskbarManager windowsTaskbar;
#endif

        /// <summary>
        /// The Indexed Topic Lists
        /// </summary>
        private List<ImageInfo> indexedTopicsList;

        /// <summary>
        /// Indicates if Ripper Currently Parsing Jobs
        /// </summary>
        private bool parseActive;

        /// <summary>
        /// Indicates that users Stopping and Deleting Current Job
        /// </summary>
        private bool stopingJob;

        /// <summary>
        /// Indicates that Disc is full and Ripping stopped
        /// </summary>
        private bool fullDisc;

        /// <summary>
        /// Indicates if Ripper is Currently Ripping
        /// </summary>
        private bool working = true;

        /// <summary>
        /// Indicates if Ripper is Currently Closing the Program
        /// </summary>
        private bool ripperClosing;

        /// <summary>
        /// Indicator if the ripper is currently hidden in the tray
        /// </summary>
        private bool isHiddenInTray;

        /// <summary>
        /// The Last Download Folder
        /// </summary>
        private string lastDownFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
#if (!PGRIPPERX)
            // Tray Menue
            trayMenu = new ContextMenu();

            MenuItem hide = new MenuItem("Hide PG-Ripper", this.HideClick);
            MenuItem sDownload = new MenuItem("Start Download", this.SDownloadClick);
            MenuItem exit = new MenuItem("Exit Program", this.ExitClick);

            trayMenu.MenuItems.Add(0, hide);
            trayMenu.MenuItems.Add(1, sDownload);
            trayMenu.MenuItems.Add(2, exit);

            // Tray Icon
            trayIcon = new NotifyIcon
                {
                    Text = "Right click for context menu",
                    Visible = false,
                    Icon = new Icon(GetType(), "App.ico"),
                    ContextMenu = trayMenu
                };

            this.trayIcon.MouseDoubleClick += this.HideClick;
#endif
            InitializeComponent();

            jobsList = new List<JobInfo>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MainForm"/> is delete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if delete; otherwise, <c>false</c>.
        /// </value>
        public static bool Delete { get; set; }

        /// <summary>
        /// Gets or sets the Delete Message
        /// </summary>
        public static string DeleteMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the Browse Dialog is Already Open
        /// </summary>
        public bool IsBrowserOpen { get; set; }

        /// <summary>
        /// Gets or sets the Resource Manger that is used for Icons and Labels etc.
        /// </summary>
        public ResourceManager _ResourceManager { get; set; }

#if (!PGRIPPERX)
        /// <summary>
        /// the download click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SDownloadClick(object sender, EventArgs e)
        {
            if (Clipboard.GetText().IndexOf(CacheController.Instance().UserSettings.CurrentForumUrl) < 0 ||
                this.parseActive)
            {
                return;
            }

            this.comboBox1.SelectedIndex = 2;

            this.textBox1.Text = Clipboard.GetText();

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)this.EnqueueJob);
            }
            else
            {
                this.EnqueueJob();
            }
        }

        /// <summary>
        /// Exits Ripper
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ExitClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Hides Ripper in tray
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void HideClick(object sender, EventArgs e)
        {
            this.Hide();
            this.isHiddenInTray = true;

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem show = new MenuItem("Show PG-Ripper", this.ShowClick);
            trayMenu.MenuItems.Add(0, show);

            this.trayIcon.MouseDoubleClick -= this.HideClick;
            this.trayIcon.MouseDoubleClick += this.ShowClick;

            if (!CacheController.Instance().UserSettings.ShowPopUps)
            {
                return;
            }

            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.BalloonTipTitle = "Hidden in Tray";
            trayIcon.BalloonTipText = "PG-Ripper is hidden in the Tray";
            trayIcon.ShowBalloonTip(10);
        }

        /// <summary>
        /// Shows the Ripper.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ShowClick(object sender, EventArgs e)
        {
            this.Show();
            this.isHiddenInTray = false;

            this.trayMenu.MenuItems.RemoveAt(0);

            var hide = new MenuItem("Hide PG-Ripper", this.HideClick);
            this.trayMenu.MenuItems.Add(0, hide);

            this.trayIcon.MouseDoubleClick -= this.ShowClick;
            this.trayIcon.MouseDoubleClick += this.HideClick;

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception)
            {
            }
        }
        
        /// <summary>
        /// Checks the Download Folder of the Current Finished Job, if Empty delete the folder.
        /// </summary>
        /// <param name="checkFolder">The check folder.</param>
        private static void CheckCurJobFolder(string checkFolder)
        {
            if (!Directory.Exists(checkFolder))
            {
                return;
            }

            if (Directory.GetFiles(checkFolder).Length == 0)
            {
                Directory.Delete(checkFolder);
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        private void LoadSettings()
        {
            CacheController.Instance().UserSettings = SettingsHelper.LoadSettings();

            // Max. Threads
            CacheController.Instance().UserSettings.ThreadLimit = CacheController.Instance().UserSettings.ThreadLimit == -1 ? 3 : CacheController.Instance().UserSettings.ThreadLimit;
            ThreadManager.GetInstance().SetThreadThreshHold(CacheController.Instance().UserSettings.ThreadLimit);

            if (string.IsNullOrEmpty(CacheController.Instance().UserSettings.DownloadFolder))
            {
                CacheController.Instance().UserSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                this.DownloadFolder.Text = CacheController.Instance().UserSettings.DownloadFolder;

                SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
            }

            this.DownloadFolder.Text = CacheController.Instance().UserSettings.DownloadFolder;

            // Load "Download Options"
            try
            {
                this.comboBox1.SelectedIndex = Convert.ToInt32(CacheController.Instance().UserSettings.DownloadOptions);
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.DownloadOptions = "0";
                this.comboBox1.SelectedIndex = 0;
            }

            this.TopMost = CacheController.Instance().UserSettings.TopMost;

            // Load Language Setting
            try
            {
                switch (CacheController.Instance().UserSettings.Language)
                {
                    case "de-DE":
                        this._ResourceManager = new ResourceManager(
                            "Ripper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        this._ResourceManager = new ResourceManager(
                            "Ripper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        this._ResourceManager = new ResourceManager(
                            "Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    default:
                        this._ResourceManager = new ResourceManager(
                            "Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                this.AdjustCulture();
            }
            catch (Exception)
            {
                this._ResourceManager = new ResourceManager(
                    "Ripper.Languages.english", Assembly.GetExecutingAssembly());
            }

            switch (CacheController.Instance().UserSettings.AfterDownloads)
            {
                case 0:
                    this.doNothingToolStripMenuItem.Checked = true;
                    this.closeRipperToolStripMenuItem.Checked = false;
                    break;
                case 1:
                    this.doNothingToolStripMenuItem.Checked = false;
                    this.closeRipperToolStripMenuItem.Checked = true;
                    break;
            }

            // Load Show Last Download Image
            this.groupBox4.Visible = CacheController.Instance().UserSettings.ShowLastDownloaded;

            this.showLastImageToolStripMenuItem.Checked = CacheController.Instance().UserSettings.ShowLastDownloaded;
            this.useCliboardMonitoringToolStripMenuItem.Checked = CacheController.Instance().UserSettings.ClipBWatch;

            try
            {
                var accountNotFound =
                    CacheController.Instance().UserSettings.ForumsAccount.Where(
                        forumAccount => forumAccount.ForumURL.Equals(CacheController.Instance().UserSettings.CurrentForumUrl)).Any(
                            forumAccount =>
                            !string.IsNullOrEmpty(forumAccount.UserName)
                            && !string.IsNullOrEmpty(forumAccount.UserPassWord));

                if (accountNotFound)
                {
                    return;
                }

                var frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (this.cameThroughCorrectLogin)
                {
                    return;
                }

                var result = TopMostMessageBox.Show(
                    this._ResourceManager.GetString("mbExit"),
                    this._ResourceManager.GetString("mbExitTtl"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            catch (Exception)
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (this.cameThroughCorrectLogin)
                {
                    this.CheckAccountMenu();

                    return;
                }

                var result = TopMostMessageBox.Show(
                    this._ResourceManager.GetString("mbExit"),
                    this._ResourceManager.GetString("mbExitTtl"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// Adjusts the culture.
        /// </summary>
        private void AdjustCulture()
        {
            this.mStartDownloadBtn.Text = this._ResourceManager.GetString("btnStartDownload");
            this.groupBox1.Text = this._ResourceManager.GetString("downloadOptions");
            this.groupBox5.Text = string.Format("{0} (-):", this._ResourceManager.GetString("lblRippingQue"));
            this.stopCurrentThreads.Text = this._ResourceManager.GetString("btnStop");
            this.groupBox4.Text = this._ResourceManager.GetString("lblLastPicture");
            this.groupBox2.Text = this._ResourceManager.GetString("gbCurrently");
            this.mInvalidDestinationMsg = this._ResourceManager.GetString("mbInvalidDes");
            this.mIncorrectUrlMsg = this._ResourceManager.GetString("mbIncorrectURL");
            this.mNoThreadMsg = this._ResourceManager.GetString("mbNoThread");
            this.mNoPostMsg = this._ResourceManager.GetString("mbNoPost");
            this.mAlreadyQueuedMsg = this._ResourceManager.GetString("mbAlreadyQueued");
            this.mTNumericMsg = this._ResourceManager.GetString("mbTNumeric");

            // Menue
            this.fileToolStripMenuItem.Text = this._ResourceManager.GetString("MenuFile");
            this.saveRippingQueueToolStripMenuItem.Text = this._ResourceManager.GetString("MenuExport");
            this.exitToolStripMenuItem.Text = this._ResourceManager.GetString("MenuExit");

            this.accountsToolStripMenuItem.Text = this._ResourceManager.GetString("MenuAccounts");

            this.optionsToolStripMenuItem.Text = this._ResourceManager.GetString("MenuOptions");
            this.settingsToolStripMenuItem2.Text = this._ResourceManager.GetString("MenuSettings");
            this.showLastImageToolStripMenuItem.Text = this._ResourceManager.GetString("ShowLastDownloaded");
            this.useCliboardMonitoringToolStripMenuItem.Text = this._ResourceManager.GetString("clipboardWatch");
            this.afterDownloadsFinishedToolStripMenuItem.Text = this._ResourceManager.GetString("AfterDownload");
            this.doNothingToolStripMenuItem.Text = this._ResourceManager.GetString("DoNothing");
            this.closeRipperToolStripMenuItem.Text = this._ResourceManager.GetString("CloseRipper"); 

            this.settingsToolStripMenuItem.Text = this._ResourceManager.GetString("MenuHelp");
            this.helpToolStripMenuItem.Text = this._ResourceManager.GetString("MenuAbout");
        }

        /// <summary>
        /// Mains the form load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainFormLoad(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += this.UnhandledExceptionFunction;
            Application.ThreadException += this.ThreadExceptionFunction;

            this.LastWorkingTime = DateTime.Now;

            this.mrefCC = CacheController.Instance();
            this.mrefTM = ThreadManager.GetInstance();

            this.LoadSettings();

            this.SetWindow();

#if (!PGRIPPERX)
            var updateNotes = string.Empty;

            if (VersionCheck.UpdateAvailable(Assembly.GetExecutingAssembly(), "PG-Ripper", out updateNotes) && File.Exists(Path.Combine(Application.StartupPath, "ICSharpCode.SharpZipLib.dll")))
            {
                var mbUpdate = this._ResourceManager.GetString("mbUpdate");
                var mbUpdate2 = this._ResourceManager.GetString("mbUpdate2");

                DialogResult result = TopMostMessageBox.Show(
                    string.Format("{0}{1}", mbUpdate, updateNotes),
                    mbUpdate2,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Clipboard.Clear();

                    this.Enabled = false;
                    this.WindowState = FormWindowState.Minimized;

                    this.trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    this.trayIcon.BalloonTipTitle = "Update in Progress";
                    this.trayIcon.BalloonTipText = "Currently upgrading to newest version.";
                    this.trayIcon.ShowBalloonTip(10);

                    AutoUpdater.TryUpdate("PG-Ripper", Assembly.GetExecutingAssembly());
                }
            }

            updateNotes = string.Empty;

#endif

#if (PGRIPPERX)
            var updateNotes = string.Empty;
#endif

            // Auto Check for Ripper.Services.dll Update
            if (VersionCheck.UpdateAvailable(typeof(Downloader).Assembly, "Ripper.Services", out updateNotes) | !File.Exists(Path.Combine(Application.StartupPath, "Ripper.Services.dll")))
            {
                TopMostMessageBox.Show(
                    string.Format(
                        "{0}{1}",
                        this._ResourceManager.GetString("ServicesUpdate"),
                        updateNotes));

                AutoUpdater.TryUpdate("Ripper.Services", Assembly.GetExecutingAssembly());
            }

            this.AutoLogin();

            if (!this.cameThroughCorrectLogin)
            {
                Application.Exit();
            }
            else
            {
                this.CheckAccountMenu();
            }

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.LoadHistory();
            }

            // Hide Index Thread Checkbox if not RiP Forums
            if (!CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"vipergirls.to"))
            {
                this.mIsIndexChk.Visible = false;
            }

#if (!PGRIPPERX)
            this.trayIcon.Visible = true;
#endif
        }

        /// <summary>
        /// Add Forum Accounts to the Menu to allow switching.
        /// </summary>
        private void CheckAccountMenu()
        {
            this.accountsToolStripMenuItem.DropDownItems.Clear();

            var newAccountMenuItem = new ToolStripMenuItem
            {
                Text = this._ResourceManager.GetString("AddNewAccount"),
                Checked = false
            };

            newAccountMenuItem.Click += this.AddNewAccount_Click;

            this.accountsToolStripMenuItem.DropDownItems.Add(newAccountMenuItem);

            this.accountsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            var deleteForumString = this._ResourceManager.GetString("DeleteAccount");

            foreach (var accounts in CacheController.Instance().UserSettings.ForumsAccount)
            {
                var forumName = accounts.ForumURL.Replace("http://", string.Empty);

                if (forumName.Contains("www."))
                {
                    forumName = forumName.Replace("www.", string.Empty);
                }

                if (forumName.EndsWith("/"))
                {
                    forumName = forumName.Remove(forumName.Length - 1);
                }

                var forumMenuItem = new ToolStripMenuItem
                    {
                        Tag = accounts.ForumURL,
                        Text = forumName, 
                        Checked = CacheController.Instance().UserSettings.CurrentForumUrl.Equals(accounts.ForumURL)
                    };

                forumMenuItem.Click += this.ForumMenuItem_Click;

                var forumDeleteMenuItem = new ToolStripMenuItem
                    {
                        Tag = accounts.ForumURL,
                        Text = deleteForumString
                    };

                forumDeleteMenuItem.Click += this.ForumMenuDeleteItem_Click;

                forumMenuItem.DropDownItems.Add(forumDeleteMenuItem);

                this.accountsToolStripMenuItem.DropDownItems.Add(forumMenuItem);
            }
        }

        /// <summary>
        /// Go To the Login Dialog to add a New Forum Account
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddNewAccount_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            Login frmLgn = new Login();
            frmLgn.ShowDialog(this);

            this.CheckAccountMenu();

            this.SwitchAccount(null);

            this.Visible = true;
        }

        /// <summary>
        /// Switch Login with Selected Account
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumMenuItem_Click(object sender, EventArgs e)
        {
            this.SwitchAccount(sender);
        }

        /// <summary>
        /// Delete the Forum Account
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumMenuDeleteItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;

            var deleteForumUrl = (string)menuItem.Tag;

            if (CacheController.Instance().UserSettings.ForumsAccount.Any(item => item.ForumURL.Equals(deleteForumUrl)))
            {
                CacheController.Instance().UserSettings.ForumsAccount.RemoveAll(item => item.ForumURL.Equals(deleteForumUrl));
            }

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);

            this.CheckAccountMenu();

            // if account is current account switch to the first available account
            if (CacheController.Instance().UserSettings.CurrentForumUrl.Equals(deleteForumUrl))
            {
                this.SwitchCurrentForumAccount(CacheController.Instance().UserSettings.ForumsAccount.First().ForumURL);
            }
            else
            {
                this.SwitchAccount(null);
            }
        }

        /// <summary>
        /// Checks the URL if it matches the current forum account.
        /// </summary>
        /// <param name="forumUrl">The forum URL.</param>
        private void CheckUrlForumAccount(string forumUrl)
        {
            if (forumUrl.StartsWith(CacheController.Instance().UserSettings.CurrentForumUrl))
            {
                return;
            }

            this.SwitchCurrentForumAccount(forumUrl);
        }

        /// <summary>
        /// Switches the forum account.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void SwitchAccount(object sender)
        {
            // Kill all current downloads 
            /*if (mCurrentJob != null)
            {
                stopCurrentThreads.Enabled = false;
                this.stopingJob = true;

                ThreadManager.GetInstance().DismantleAllThreads();

                lvCurJob.Items.Clear();
                mCurrentJob = null;
                if (this.InvokeRequired)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                        {
                            StatusLabelImageC.Text = string.Empty;
                        });
                }
                else
                {
                    StatusLabelImageC.Text = string.Empty;
                }

                this.IdleRipper();
            }*/
            ////

            if (sender == null)
            {
                return;
            }

            var menuItem = (ToolStripMenuItem)sender;

            this.SwitchCurrentForumAccount((string)menuItem.Tag);
        }

        /// <summary>
        /// Switches the current forum account to the forum Url
        /// </summary>
        /// <param name="forumUrl">The forum URL.</param>
        private void SwitchCurrentForumAccount(string forumUrl)
        {
            ForumAccount currentAccount = null;

            foreach (var forumAccount in
                CacheController.Instance().UserSettings.ForumsAccount.Where(forumAccount => forumUrl.StartsWith(forumAccount.ForumURL)).Where(
                    forumAccount =>
                    !string.IsNullOrEmpty(forumAccount.UserName) && !string.IsNullOrEmpty(forumAccount.UserPassWord)))
            {
                currentAccount = forumAccount;
            }

            if (currentAccount != null)
            {
                CacheController.Instance().UserSettings.CurrentForumUrl = currentAccount.ForumURL;
                CacheController.Instance().UserSettings.CurrentUserName = currentAccount.UserName;

                if (currentAccount.GuestAccount)
                {
                    this.cameThroughCorrectLogin = true;
                    this.CheckAccountMenu();
                }
                else
                {
                    var lgnMgr = new LoginManager(currentAccount.UserName, currentAccount.UserPassWord);

                    if (lgnMgr.DoLogin(CacheController.Instance().UserSettings.CurrentForumUrl))
                    {
                        this.cameThroughCorrectLogin = true;
                        this.CheckAccountMenu();
                    }
                    else
                    {
                        this.Visible = false;

                        Login frmLgn = new Login();
                        frmLgn.ShowDialog(this);

                        this.Visible = true;
                    }
                }
            }
            else
            {
                return;
            }

            var header = this._ResourceManager.GetString("ttlHeader");

#if (PGRIPPER)
            Text = string.Format(
                "PG-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format("{0}{1}\" @ {2} ]", header, CacheController.Instance().UserSettings.CurrentUserName, CacheController.Instance().UserSettings.CurrentForumUrl));
#elif (PGRIPPERX)
                Text = string.Format("PG-Ripper X {0}.{1}.{2}{3}{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format("{0}{1}\" @ {2} ]", header, CacheController.Instance().UserSettings.CurrentUserName, CacheController.Instance().UserSettings.CurrentForumUrl));
#else
            Text = string.Format(
                "PG-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format(
                    "{0}{1}\" @ {2} ]", header, CacheController.Instance().UserSettings.CurrentUserName, CacheController.Instance().UserSettings.CurrentForumUrl));
#endif
        }

        /// <summary>
        /// Auto login if User Credentials exists in Config file.
        /// if no Data show Login Form
        /// </summary>
        private void AutoLogin()
        {
            bool accountExists = false;
            var currentForumAccount = new ForumAccount();

            foreach (var forumAccount in CacheController.Instance().UserSettings.ForumsAccount.Where(
                    forumAccount =>
                        forumAccount.ForumURL.Equals(CacheController.Instance().UserSettings.CurrentForumUrl)).Where(
                        forumAccount => !string.IsNullOrEmpty(forumAccount.UserName) && !string.IsNullOrEmpty(forumAccount.UserPassWord)))
            {
                currentForumAccount = forumAccount;
                accountExists = true;
            }

            if (accountExists)
            {
                if (currentForumAccount.GuestAccount)
                {
                    this.cameThroughCorrectLogin = true;
                }
                else
                {
                    LoginManager lgnMgr = new LoginManager(currentForumAccount.UserName, currentForumAccount.UserPassWord);

                    if (lgnMgr.DoLogin(CacheController.Instance().UserSettings.CurrentForumUrl))
                    {
                        this.cameThroughCorrectLogin = true;
                    }
                    else
                    {
                        Login frmLgn = new Login();
                        frmLgn.ShowDialog(this);

                        DialogResult result = TopMostMessageBox.Show(
                            this._ResourceManager.GetString("mbExit"),
                            this._ResourceManager.GetString("mbExitTtl"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            else
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                Application.Exit();
            }
        }

        /// <summary>
        /// Starts Ripping
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Arguments.
        /// </param>
        private void MStartDownloadBtnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text) || this.parseActive)
            {
                return;
            }

            if (this.textBox1.Text.StartsWith("http"))
            {
                this.comboBox1.SelectedIndex = 2;
            }

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)this.EnqueueJob);
            }
            else
            {
                this.EnqueueJob();
            }
        }

        /// <summary>
        /// Starts the Download when Pressing Enter in Textbox URL
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Key Press Event Arguments.
        /// </param>
        private void TextBox1KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r' || this.parseActive)
            {
                return;
            }

            e.Handled = true;

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)this.EnqueueJob);
            }
            else
            {
                this.EnqueueJob();
            }
        }

        /// <summary>
        /// Enqueue Job
        /// </summary>
        private void EnqueueJob()
        {
            if (this.comboBox1.SelectedIndex.Equals(2))
            {
                // Encode URL
                this.textBox1.Text = Uri.EscapeUriString(this.textBox1.Text);

                this.CheckUrlForumAccount(this.textBox1.Text);
            }


            this.tmrPageUpdate.Enabled = true;

            if (!this.IsValidJob())
            {
                this.UnlockControls();
                return;
            }

            // Format Url
            var sHtmlUrl = UrlHandler.GetHtmlUrl(this.textBox1.Text, this.comboBox1.SelectedIndex);

            if (string.IsNullOrEmpty(sHtmlUrl))
            {
                this.UnlockControls();
                return;
            }

            // Check Post is Ripped?!
            if (CacheController.Instance().UserSettings.SavePids)
            {
                string sPostId = null;

                if (sHtmlUrl.Contains("showpost.php?p="))
                {
                    sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("showpost.php?p=") + 15);
                }
                else if (sHtmlUrl.Contains("#post"))
                {
                    sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("#post") + 5);
                }

                if (!string.IsNullOrEmpty(sPostId))
                {
                    if (sPostId.Contains("&postcount"))
                    {
                        sPostId.Remove(sHtmlUrl.IndexOf("&postcount"));
                    }

                    if (this.IsPostAlreadyRipped(sPostId))
                    {
                        DialogResult result = TopMostMessageBox.Show(
                            this._ResourceManager.GetString("mBAlready"), "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result != DialogResult.Yes)
                        {
                            this.UnlockControls();
                            return;
                        }
                    }
                }
            }

            ///////////////////////////////////////////////
            this.LockControls();
            ///////////////////////////////////////////////

            if (mIsIndexChk.Checked)
            {
                // Parse Job as Index Thread
                this.EnqueueIndexThread(sHtmlUrl);
            }
            else
            {
                if (!sHtmlUrl.Contains(@"#post") && !sHtmlUrl.Contains(@"showpost"))
                {
                    this.EnqueueThreadToPost(sHtmlUrl);
                }
                else
                {
                    this.EnqueueThreadOrPost(sHtmlUrl);
                }
            }
        }

        /// <summary>
        /// Check if Job is OK
        /// </summary>
        /// <returns>Returns true or false</returns>
        private bool IsValidJob()
        {
            if (!Utility.IsNumeric(textBox1.Text) && comboBox1.SelectedIndex != 2)
            {
                TopMostMessageBox.Show(mTNumericMsg, "Info");

                return false;
            }

            if (string.IsNullOrEmpty(CacheController.Instance().UserSettings.DownloadFolder))
            {
                DialogResult result = TopMostMessageBox.Show(
                    "Please Set Up Download Folder before starting download",
                    "Info",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    this.UpdateDownloadFolder();
                }
            }

            return true;
        }

        /// <summary>
        /// Parse all Threads/Posts in the Index
        /// </summary>
        /// <param name="sHtmlUrl">The Url to the Thread/Post</param>
        private void EnqueueIndexThread(string sHtmlUrl)
        {
#if (!PGRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
            }
#endif

            try
            {
                if (
                    string.IsNullOrEmpty(
                        Maintenance.GetInstance().ExtractTopicTitleFromHtml(Maintenance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                        this.windowsTaskbar.SetProgressValue(10, 100);
                    }
#endif
                    // Unlock Controls
                    this.UnlockControls();

                    return;
                }
            }
            catch (Exception)
            {
                TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                    Environment.OSVersion.Version.Minor >= 1)
                {
                    this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                    this.windowsTaskbar.SetProgressValue(10, 100);
                }
#endif
                // Unlock Controls
                this.UnlockControls();

                return;
            }

            mIsIndexChk.Checked = false;

            GetIdxsWorker.RunWorkerAsync(sHtmlUrl);

            while (GetIdxsWorker.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Parse all Posts of a Thread
        /// </summary>
        /// <param name="sHtmlUrl">The Thread Url</param>
        private void EnqueueThreadToPost(string sHtmlUrl)
        {
#if (!PGRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
            }
#endif

           try
            {
                if (
                    string.IsNullOrEmpty(
                        Maintenance.GetInstance().ExtractTopicTitleFromHtml(Maintenance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                        this.windowsTaskbar.SetProgressValue(10, 100);
                    }
#endif
                    // Unlock Controls
                    this.UnlockControls();

                    return;
                }
           }
            catch (Exception)
            {
                TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                    Environment.OSVersion.Version.Minor >= 1)
                {
                    this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                    this.windowsTaskbar.SetProgressValue(10, 100);
                }
#endif
                // Unlock Controls
                this.UnlockControls();

                return;
            }

            ///////
            while (GetPostsWorker.IsBusy)
            {
                Application.DoEvents();
            }

            this.GetPostsWorker.RunWorkerAsync(sHtmlUrl);
        }

        /// <summary>
        /// Parse a Thread or Post as Single Job
        /// </summary>
        /// <param name="sHtmlUrl">The Thread/Post Url</param>
        private void EnqueueThreadOrPost(string sHtmlUrl)
        {
            if (jobsList.Any(t => t.HtmlUrl == sHtmlUrl))
            {
                TopMostMessageBox.Show(mAlreadyQueuedMsg, "Info");
                return;
            }

            JobInfo job = new JobInfo { HtmlUrl = sHtmlUrl, HtmlPayLoad = Maintenance.GetInstance().GetPostPages(sHtmlUrl) };

            job.TopicTitle = Utility.ReplaceHexWithAscii(Maintenance.GetInstance().ExtractTopicTitleFromHtml(job.HtmlPayLoad));

            if (CacheController.Instance().UserSettings.AutoThank)
            {
                job.SecurityToken = Utility.GetSecurityToken(job.HtmlPayLoad);
            }

            if (!sHtmlUrl.Contains(@"showthread") || sHtmlUrl.Contains(@"#post"))
            {
                job.PostTitle =
                    Utility.ReplaceHexWithAscii(
                        Maintenance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, sHtmlUrl));

                job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromHtml(job.HtmlUrl, true);
            }
            else
            {
                job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromHtml(job.HtmlUrl, false);
            }

            job.ImageList = ExtractHelper.ExtractImagesHtml(job.HtmlPayLoad, sHtmlUrl);
            job.ImageCount = job.ImageList.Count;

            if (job.ImageCount == 0)
            {
                // Unlock Controls
                this.UnlockControls();

                return;
            }

            if (string.IsNullOrEmpty(job.TopicTitle))
            {
                TopMostMessageBox.Show(sHtmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

                // Unlock Controls
                this.UnlockControls();

                return;
            }

            job.StorePath = this.GenerateStorePath(job);

            JobListAddDelegate newJob = this.JobListAdd;

            Invoke(newJob, new object[] { job });

            ///////////////////////////////////////////////
            this.UnlockControls();
            ///////////////////////////////////////////////
        }

        /// <summary>
        /// Pushes the "Thank You" Button for the user.
        /// </summary>
        /// <param name="sPostId">
        /// The s post id.
        /// </param>
        /// <param name="iICount">
        /// The i i count.
        /// </param>
        /// <param name="sSecurityToken">
        /// The s security token.
        /// </param>
        private void ProcessAutoThankYou(string sPostId, int iICount, string sSecurityToken)
        {
            if (!CacheController.Instance().UserSettings.AutoThank)
            {
                return;
            }

            if (iICount < CacheController.Instance().UserSettings.MinImageCount)
            {
                return;
            }

            SendThankYouDelegate lSendThankYouDel = this.SendThankYou;

            if (sPostId == null)
            {
                return;
            }

            string tyURL;
            if (CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"scanlover.com"))
            {
                tyURL = string.Format(
                    "{0}post_thanks.php?do=post_thanks_add&p={1}",
                    CacheController.Instance().UserSettings.CurrentForumUrl,
                    sPostId);
            }
            else
            {
                tyURL = string.Format(
                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                    CacheController.Instance().UserSettings.CurrentForumUrl,
                    sPostId,
                    sSecurityToken);
            }

            this.Invoke(lSendThankYouDel, new object[] { tyURL });
        }

        /// <summary>
        /// This delegate enables asynchronous calls for automatically sending thank yous
        /// </summary>
        /// <param name="aUrl">A URL.</param>
        private delegate void SendThankYouDelegate(string aUrl);

        /// <summary>
        /// Sends the thank you.
        /// </summary>
        /// <param name="aUrl">A URL.</param>
        private void SendThankYou(string aUrl)
        {
            string tyURLRef = CacheController.Instance().UserSettings.CurrentForumUrl;

            HttpWebResponse lHttpWebResponse = null;
            Stream lHttpWebResponseStream = null;

            HttpWebRequest lHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            lHttpWebRequest.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
            lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
            lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            lHttpWebRequest.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
            lHttpWebRequest.Accept =
                "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            lHttpWebRequest.KeepAlive = true;
            lHttpWebRequest.Referer = tyURLRef;
            lHttpWebRequest.AllowAutoRedirect = false;
            lHttpWebRequest.Timeout = 1500;
            ///////////////////////////////////

            try
            {
                lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();
                lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();
            }
            finally
            {
                lHttpWebResponse.Close();
                lHttpWebResponseStream.Close();
            }
        }

        /// <summary>
        /// Parses an index and places all linked images in mDownloadsList.
        /// </summary>
        /// <param name="sHtmlUrl">The s HTML URL.</param>
        private void ThrdGetIndexes(string sHtmlUrl)
        {
            Indexes idxs = new Indexes();

            var pagecontent = idxs.GetThreadPagesNew(sHtmlUrl);

            this.indexedTopicsList = idxs.ParseHtml(pagecontent, sHtmlUrl);
        }

        /// <summary>
        /// Get All Post of a Thread and Parse them as new Job
        /// </summary>
        /// <param name="htmlUrl">
        /// The html Url.
        /// </param>
        private void ThrdGetPosts(string htmlUrl)
        {
            ThreadToPost threads = new ThreadToPost();

            string pagecontent = threads.GetThreadPagesNew(htmlUrl);

            string forumTitle = Maintenance.GetInstance().ExtractForumTitleFromHtml(htmlUrl, false);
           
            List<ImageInfo> arlst = threads.ParseHtml(pagecontent);

            for (int po = 0; po < arlst.Count; po++)
            {
                var po1 = po;

                if (this.InvokeRequired)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                            {
                                this.StatusLabelInfo.Text = string.Format(
                                    "{0}{1}/{2}", this._ResourceManager.GetString("gbParse"), po1, arlst.Count);

                                this.StatusLabelInfo.ForeColor = Color.Green;
                            });
                }
                else
                {
                    this.StatusLabelInfo.Text = string.Format(
                       "{0}{1}/{2}", this._ResourceManager.GetString("gbParse"), po, arlst.Count);
                    this.StatusLabelInfo.ForeColor = Color.Green;
                }

                string postId = arlst[po].ImageUrl;

                //////////////////////////////////////////////////////////////////////////

                if (CacheController.Instance().UserSettings.SavePids && this.IsPostAlreadyRipped(postId))
                {
                    continue;
                }

                string newPostUrl = string.Format(
                    "{0}showpost.php?p={1}#post{1}", CacheController.Instance().UserSettings.CurrentForumUrl, postId);
                
                JobInfo jobInfoDouble = this.jobsList.Find(doubleJob => doubleJob.HtmlUrl.Equals(newPostUrl));

                if (jobInfoDouble != null)
                {
                    continue;
                }

                if (this.currentJob != null)
                {
                    if (this.currentJob.HtmlUrl.Equals(newPostUrl))
                    {
                        continue;
                    }
                }

                var newJob = new JobInfo
                                 {
                                     HtmlUrl = newPostUrl,
                                     HtmlPayLoad = Maintenance.GetInstance().GetPostPages(newPostUrl)
                                 };

                if (string.IsNullOrEmpty(newJob.HtmlPayLoad))
                {
                    continue;
                }

                newJob.TopicTitle = Maintenance.GetInstance().ExtractTopicTitleFromHtml(newJob.HtmlPayLoad);

                if (CacheController.Instance().UserSettings.AutoThank)
                {
                    newJob.SecurityToken = Utility.GetSecurityToken(newJob.HtmlPayLoad);
                }

                newJob.ForumTitle = forumTitle;

                newJob.PostTitle = Maintenance.GetInstance().ExtractPostTitleFromHtml(newJob.HtmlPayLoad, newPostUrl);
                newJob.TopicTitle = Utility.ReplaceHexWithAscii(newJob.TopicTitle);

                newJob.ImageList = ExtractHelper.ExtractImagesHtml(newJob.HtmlPayLoad, postId);
                newJob.ImageCount = newJob.ImageList.Count;

                if (newJob.ImageCount == 0)
                {
                    continue;
                }

                newJob.StorePath = this.GenerateStorePath(newJob);

                JobListAddDelegate newJobDelegate = this.JobListAdd;

                this.Invoke(newJobDelegate, new object[] { newJob });

                //// JobListAdd(job);

                //////////////////////////////////////////////////////////////////////////
            }
        }

        /// <summary>
        /// Timer thats fires up the LogicCode
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Elapsed Event Arguments.
        /// </param>
        private void TmrPageUpdateElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.working && !this.parseActive || this.jobsList.Count > 0 && !this.parseActive)
            {
                this.LogicCode();
            }

            if (!this.parseActive && this.ExtractUrls.Count > 0 && !this.GetPostsWorker.IsBusy && !this.GetIdxsWorker.IsBusy)
            {
                this.GetExtractUrls();
            }

            this.CheckClipboardData();
        }

        /// <summary>
        /// Over time, it turned into a nasty C-like code; where everything is
        /// in the same method...
        /// Its kind of an over kill and I need to rewrite this sorry excuse 
        /// for a 'scheduler'...
        /// </summary>
        private void LogicCode()
        {
            // Full HDD solution
            if (Delete && !this.fullDisc)
            {
                this.FullDisc();
            }

            if (mrefTM.GetThreadCount() > 0)
            {
                // If Joblist empty and the last Threads of Current Job are parsed
                if (this.currentJob == null && this.jobsList.Count.Equals(0) && !this.parseActive)
                {
                    this.StatusLabelInfo.Text = this._ResourceManager.GetString("StatusLabelInfo");
                    StatusLabelInfo.ForeColor = Color.Red;

                    this.groupBox5.Text = string.Format("{0} (-):", this._ResourceManager.GetString("lblRippingQue"));
                }
            }
            else
            {
                // Check if Last Downloadfolder is Empty
                if (!string.IsNullOrEmpty(this.lastDownFolder))
                {
                    CheckCurJobFolder(this.lastDownFolder);
                }

                if (!CacheController.Instance().UserSettings.CurrentlyPauseThreads)
                {
                    lvCurJob.Items.Clear();
                    if (this.InvokeRequired)
                    {
                        this.Invoke(
                            (MethodInvoker)delegate
                            {
                                StatusLabelImageC.Text = string.Empty;
                            });
                    }
                    else
                    {
                        StatusLabelImageC.Text = string.Empty;
                    }

                    if (this.currentJob == null)
                    {
                        if (this.jobsList.Count.Equals(0))
                        {
                            this.IdleRipper(true);
                        }
                        else
                        {
#if (!PGRIPPERX)
                            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                                && Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
                            {
                                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                            }
#endif

                            // STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
                            this.ProcessNextJob();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
        /// </summary>
        private void ProcessNextJob()
        {
            this.working = true;

            deleteJob.Enabled = true;
            stopCurrentThreads.Enabled = true;

            if (jobsList.Count == 0)
            {
                return;
            }

            currentJob = jobsList[0];

            this.JobListRemove(0);

            string bSystemExtr = this._ResourceManager.GetString("bSystemExtr");

            this.ParseJob();

            // NO IMAGES TO PROCESS SO ABANDON CURRENT THREAD
            if (mImagesList == null || mImagesList.Count <= 0)
            {
                currentJob = null;
                deleteJob.Enabled = true;
                stopCurrentThreads.Enabled = true;
                return;
            }

            this.groupBox2.Text = string.Format("{0}...", this._ResourceManager.GetString("gbCurrentlyExtract"));

            if (currentJob.TopicTitle.Equals(currentJob.PostTitle))
            {
                Text = string.Format(
                    "{0}: {1} - x{2}", this._ResourceManager.GetString("gbCurrentlyExtract"), this.currentJob.TopicTitle, this.mImagesList.Count);
            }
            else
            {
                Text = string.Format(
                    "{0}: {1} - {2} - x{3}",
                    this._ResourceManager.GetString("gbCurrentlyExtract"),
                    currentJob.TopicTitle,
                    currentJob.PostTitle,
                    mImagesList.Count);
            }

            lvCurJob.Columns[0].Text = string.Format("{0} - x{1}", currentJob.PostTitle, mImagesList.Count);

#if (!PGRIPPERX)
            try
            {
                if (CacheController.Instance().UserSettings.ShowPopUps)
                {
                    this.trayIcon.Text = this._ResourceManager.GetString("gbCurrentlyExtract") + this.currentJob.TopicTitle;
                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    this.trayIcon.BalloonTipTitle = this._ResourceManager.GetString("gbCurrentlyExtract");
                    trayIcon.BalloonTipText = currentJob.TopicTitle;
                    trayIcon.ShowBalloonTip(10);
                }
            }
            catch (Exception)
            {
                if (CacheController.Instance().UserSettings.ShowPopUps)
                {
                    trayIcon.Text = bSystemExtr;
                    trayIcon.BalloonTipTitle = bSystemExtr;
                    trayIcon.BalloonTipText = bSystemExtr;
                    trayIcon.ShowBalloonTip(10);
                }
            }
#endif

            try
            {
                progressBar1.Maximum = mImagesList.Count;
            }
            catch (Exception)
            {
                progressBar1.Maximum = 10000;
            }

            this.ProcessCurImgLst();
        }

        /// <summary>
        /// Processing the Images list of the Current Job
        /// </summary>
        private void ProcessCurImgLst()
        {
            stopCurrentThreads.Enabled = true;
            this.stopingJob = false;
            this.working = true;

            this.lastDownFolder = null;

            ThreadManager lTdm = ThreadManager.GetInstance();

            this.lastDownFolder = currentJob.StorePath;

            if (mImagesList.Count > 0)
            {
                string tiImagesRemain = this._ResourceManager.GetString("tiImagesRemain");

                ////////////////
                lvCurJob.Items.Clear();
                if (this.InvokeRequired)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                        {
                            StatusLabelImageC.Text = string.Empty;
                        });
                }
                else
                {
                    StatusLabelImageC.Text = string.Empty;
                }

                for (int i = 0; i < mImagesList.Count; i++)
                {
                    lvCurJob.Items.Add(
                        string.Format("{0}/{1} - {2}", i + 1, mImagesList.Count, mImagesList[i].ImageUrl),
                        mImagesList[i].ImageUrl);
                }
                ///////////////////

                progressBar1.Maximum = mImagesList.Count;

                for (int i = 0; i < mImagesList.Count; i++)
                {
                    if (this.stopingJob || this.ripperClosing)
                    {
                        break;
                    }

#if (!PGRIPPERX)
                    this.trayIcon.Text = string.Format(tiImagesRemain, this.mImagesList.Count - i, i * 100 / this.mImagesList.Count);

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressValue(i, mImagesList.Count);
                    }
#endif
                    while (!lTdm.IsSystemReadyForNewThread())
                    {
                        Application.DoEvents();
                    }

                    if (!this.ripperClosing)
                    {
                        if (this.progressBar1 != null)
                        {
                            this.progressBar1.Value = i;
                        }

                        this.StatusLabelImageC.Text = string.Format(
                            tiImagesRemain, mImagesList.Count - i, i * 100 / mImagesList.Count);
                    }

                    try
                    {
                        // Attempts to download an image when the URL is longer than http://
                        if (this.mImagesList[i].ImageUrl.Length > 6)
                        {
                            if (!(i > this.lvCurJob.Items.Count))
                            {
                                this.lvCurJob.Items[i].Selected = true;
                                this.lvCurJob.EnsureVisible(i);
                            }

                            Downloader.DownloadImage(
                                this.mImagesList[i].ImageUrl,
                                this.mImagesList[i].ThumbnailUrl,
                                this.currentJob.StorePath,
                                !string.IsNullOrEmpty(this.currentJob.PostTitle)
                                    ? this.currentJob.PostTitle
                                    : this.currentJob.TopicTitle,
                                i + 1);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (this.stopingJob || this.ripperClosing)
                        {
                            break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        if (this.stopingJob || this.ripperClosing)
                        {
                            break;
                        }
                    }

                    if (i > this.lvCurJob.Items.Count)
                    {
                        continue;
                    }

                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)this.ShowLastPic);
                    }
                    else
                    {
                        this.ShowLastPic();
                    }

                    if (!this.ripperClosing)
                    {
                        this.lvCurJob.Items[i].ForeColor = Color.Green;
                    }
                }

                // FINISED A THREAD/POST DOWNLOAD JOB
                currentJob = null;

                if (!string.IsNullOrEmpty(this.lastDownFolder))
                {
                    CheckCurJobFolder(this.lastDownFolder);
                }

                if (jobsList.Count > 0)
                {
#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                    }
#endif
                    // STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
                    this.ProcessNextJob();
                }
            }
            else
            {
                // NO IMAGES TO PROCESS SO ABANDON CURRENT THREAD
                lvCurJob.Items.Clear();
                currentJob = null;
                if (this.InvokeRequired)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                        {
                            StatusLabelImageC.Text = string.Empty;
                        });
                }
                else
                {
                    StatusLabelImageC.Text = string.Empty;
                }
            }
        }

        private Image imgLastPic;

        /// <summary>
        /// Shows the last Downloaded Image
        /// </summary>
        private void ShowLastPic()
        {
            if (!CacheController.Instance().UserSettings.ShowLastDownloaded)
            {
                return;
            }

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }

            if (this.pictureBox1.BackgroundImage != null)
            {
                this.pictureBox1.BackgroundImage.Dispose();
                this.pictureBox1.BackgroundImage = null;
            }
            ///////////////////////////

            this.sLastPic = this.mrefCC.LastPic;

            if (!File.Exists(this.sLastPic))
            {
                return;
            }

            try
            {
                var fileInfo = new FileInfo(this.sLastPic);

                // skip Images above 9 MB
                if (fileInfo.Length >= 9437184)
                {
                    return;
                }

                this.pictureBox1.Visible = true;

                if (this.sLastPic.EndsWith(".gif"))
                {
                    this.pictureBox1.BackgroundImage = Image.FromFile(this.sLastPic);

                    this.pictureBox1.Update();
                }
                else
                {
                    // This statement causes file locking until the
                    // process exits unless cleared when not visible
                    //this.imgLastPic = Image.FromFile(this.sLastPic);

                    using (FileStream stream = new FileStream(this.sLastPic, FileMode.Open, FileAccess.Read))
                    {
                        this.imgLastPic = Image.FromStream(stream);
                    }

                    this.pictureBox1.Image = this.imgLastPic;
                    this.pictureBox1.Update();
                }
            }
            catch (Exception)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }
        }

        /// <summary>
        /// Adds new Job to JobList and ListView
        /// </summary>
        /// <param name="job">The New Job</param>
        private delegate void JobListAddDelegate(JobInfo job);

        /// <summary>
        /// Adds new Job to JobList and ListView
        /// </summary>
        /// <param name="job">The new Job</param>
        private void JobListAdd(JobInfo job)
        {
            this.working = true;

            jobsList.Add(job);

            ListViewItem ijobJob = new ListViewItem { Text = job.TopicTitle };

            ijobJob.SubItems.Add(job.PostTitle);
            ijobJob.SubItems.Add(job.ImageCount.ToString());

            listViewJobList.Items.AddRange(new[] { ijobJob });

            this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), this.jobsList.Count);
        }

        /// <summary>
        /// Removes a Job from the Joblist and ListView
        /// </summary>
        /// <param name="iJobIndex">The Index of the Job inside the Joblist</param>
        private void JobListRemove(int iJobIndex)
        {
            jobsList.RemoveAt(iJobIndex);

            listViewJobList.Items.RemoveAt(iJobIndex);

            this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), this.jobsList.Count);
        }

        /// <summary>
        /// Idles the ripper.
        /// </summary>
        /// <param name="endingAllRip">Indicates that Ripper is Finishing the last Threads of all Jobs</param>
        private void IdleRipper(bool endingAllRip = false)
        {
#if (!PGRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                this.windowsTaskbar.SetProgressValue(10, 100);
            }

            string btleExit = this._ResourceManager.GetString("btleExit"), btexExit = this._ResourceManager.GetString("btexExit");

            if (endingAllRip && CacheController.Instance().UserSettings.ShowCompletePopUp)
            {
                this.trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                this.trayIcon.BalloonTipTitle = btleExit;
                this.trayIcon.BalloonTipText = btexExit;
                this.trayIcon.ShowBalloonTip(10);
            }
#endif
            if (endingAllRip && CacheController.Instance().UserSettings.AfterDownloads.Equals(1))
            {
                this.Close();
            }
            else
            {
                this.lvCurJob.Items.Clear();

                this.stopCurrentThreads.Enabled = true;
                this.stopingJob = false;

                if (this.InvokeRequired)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                            {
                                StatusLabelImageC.Text = string.Empty;
                            });
                }
                else
                {
                    StatusLabelImageC.Text = string.Empty;
                }

                textBox1.Text = string.Empty;

                progressBar1.Value = 0;

                this.parseActive = false;

                string ttlHeader = this._ResourceManager.GetString("ttlHeader");

                this.groupBox2.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
                this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
                StatusLabelInfo.ForeColor = Color.Gray;

                lvCurJob.Columns[0].Text = "  ";

#if (PGRIPPER)
            this.Text = string.Format(
                "PG-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format("{0}{1}\" @ {2} ]", ttlHeader, CacheController.Instance().UserSettings.CurrentUserName, CacheController.Instance().UserSettings.CurrentForumUrl));

            trayIcon.Text = "Right click for context menu";
#elif (PGRIPPERX)
                this.Text = string.Format("PG-Ripper X {0}.{1}.{2}{3}{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format("{0}{1}\" @ {2} ]", ttlHeader, CacheController.Instance().UserSettings.CurrentUserName, CacheController.Instance().UserSettings.CurrentForumUrl));
#else
                this.Text = string.Format(
                    "PG-Ripper {0}.{1}.{2}{3}{4}",
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format(
                        "{0}{1}\" @ {2} ]",
                        ttlHeader,
                        CacheController.Instance().UserSettings.CurrentUserName,
                        CacheController.Instance().UserSettings.CurrentForumUrl));

                trayIcon.Text = "Right click for context menu";
#endif
                // Since no picbox image will be visible until another job is queued,
                // reclaim resources used by any previous image in the picturebox
                // This prevents file locking of downloaded images until the process exits
                /*if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }*/

                if (this.imgLastPic != null)
                {
                    this.imgLastPic.Dispose();
                    this.imgLastPic = null;
                }

                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }

                if (pictureBox1.BackgroundImage != null)
                {
                    pictureBox1.BackgroundImage.Dispose();
                    pictureBox1.BackgroundImage = null;
                }

                // Hide any image last displayed in the picturebox
                pictureBox1.Visible = false;

                deleteJob.Enabled = false;
                stopCurrentThreads.Enabled = false;

                this.working = false;
            }
        }

        /// <summary>
        /// Full HDD solution
        /// </summary>
        private void FullDisc()
        {
            this.working = false;
            this.fullDisc = true;

            pauseCurrentThreads.Text = "Resume Download(s)";
            ThreadManager.GetInstance().HoldAllThreads();

            StatusLabelInfo.Text = DeleteMessage;
            StatusLabelInfo.ForeColor = Color.Red;

            TopMostMessageBox.Show(
                string.Format(
                    "Please change your download location, then press \"Resume Download\", because {0}", DeleteMessage),
                "Warning");

            JobListAddDelegate updateJob = this.JobListAdd;

            Invoke(updateJob, new object[] { currentJob });

            lvCurJob.Items.Clear();
            if (this.InvokeRequired)
            {
                this.Invoke(
                    (MethodInvoker)delegate
                    {
                        StatusLabelImageC.Text = string.Empty;
                    });
            }
            else
            {
                StatusLabelImageC.Text = string.Empty;
            }

            currentJob = null;

            this.UpdateDownloadFolder();

            for (int i = 0; i != jobsList.Count; i++)
            {
                /* JobInfo updatedJob = new JobInfo
                                          {
                                              iImageCount = mJobsList[i].iImageCount,
                                              sPostTitle = mJobsList[i].sPostTitle,
                                              sStorePath = sDownloadFolder
                                          };

                 if (SubDirs)
                 {
                     if (comboBox1.SelectedIndex == 0)
                     {
                         updatedJob.sStorePath = Path.Combine(sDownloadFolder, mJobsList[i].sTitle);
                     }
                     if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                     {
                         updatedJob.sStorePath = Path.Combine(sDownloadFolder, mJobsList[i].sTitle + Path.DirectorySeparatorChar + mJobsList[i].sPostTitle);
                     }
                 }

                 updatedJob.sTitle = mJobsList[i].sTitle;
                 updatedJob.sURL = mJobsList[i].sURL;
                 updatedJob.sXMLPayLoad = mJobsList[i].sXMLPayLoad;

                 JobListRemove(i);

                 mJobsList.Insert(i, updatedJob);*/

                jobsList[i].StorePath = CacheController.Instance().UserSettings.DownloadFolder;

                if (!CacheController.Instance().UserSettings.SubDirs)
                {
                    continue;
                }

                if (comboBox1.SelectedIndex == 0)
                {
                    jobsList[i].StorePath = Path.Combine(CacheController.Instance().UserSettings.DownloadFolder, jobsList[i].TopicTitle);
                }

                if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                {
                    jobsList[i].StorePath = Path.Combine(
                        CacheController.Instance().UserSettings.DownloadFolder,
                        jobsList[i].TopicTitle + Path.DirectorySeparatorChar + jobsList[i].PostTitle);
                }
            }

            Delete = false;
            this.fullDisc = false;
        }

        /// <summary>
        /// Parses the job.
        /// </summary>
        private void ParseJob()
        {
            string sPostIdStart = null;

            if (currentJob.HtmlUrl.Contains("showpost.php"))
            {
                sPostIdStart = "showpost.php?p=";
            }
            else if (currentJob.HtmlUrl.Contains("#post"))
            {
                sPostIdStart = "#post";
            }

            string postId = currentJob.HtmlUrl.Substring(currentJob.HtmlUrl.IndexOf(sPostIdStart) + sPostIdStart.Length);

            if (postId.Contains(@"postcount"))
            {
                postId = Regex.Replace(postId, postId.Substring(postId.IndexOf("postcount=") - 1), string.Empty);
            }

            mImagesList = currentJob.ImageList;

            this.ProcessAutoThankYou(postId, this.mImagesList.Count, this.currentJob.SecurityToken);
        }

        /// <summary>
        /// Deletes the Selected Jobs
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeleteJobClick(object sender, EventArgs e)
        {
            try
            {
                if (this.listViewJobList.SelectedItems.Count <= 0)
                {
                    return;
                }

                this.jobsList.RemoveRange(this.listViewJobList.SelectedItems[0].Index, this.listViewJobList.SelectedIndices.Count);

                foreach (ListViewItem deleteItem in this.listViewJobList.SelectedItems)
                {
                    this.listViewJobList.Items.Remove(deleteItem);
                }
            }
            catch (Exception)
            {
                this.listViewJobList.Items.Clear();

                for (int i = 0; i < this.jobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.jobsList.Count);
                    this.StatusLabelInfo.ForeColor = Color.Green;

                    var jobItem = new ListViewItem(this.jobsList[i].TopicTitle, 0);

                    jobItem.SubItems.Add(this.jobsList[i].PostTitle);
                    jobItem.SubItems.Add(this.jobsList[i].ImageCount.ToString());

                    this.listViewJobList.Items.AddRange(new[] { jobItem });
                }
            }
            finally
            {
                this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), this.jobsList.Count);
            }
        }

        /// <summary>
        /// Kill and Deletes the Current Job
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void StopCurrentThreadsClick(object sender, EventArgs e)
        {
            if (currentJob == null)
            {
                return;
            }

            stopCurrentThreads.Enabled = false;
            this.stopingJob = true;

            ThreadManager.GetInstance().DismantleAllThreads();

            string sLastJobFolder = currentJob.StorePath;

            lvCurJob.Items.Clear();
            currentJob = null;
            if (this.InvokeRequired)
            {
                this.Invoke(
                    (MethodInvoker)delegate
                    {
                        StatusLabelImageC.Text = string.Empty;
                    });
            }
            else
            {
                StatusLabelImageC.Text = string.Empty;
            }

            CheckCurJobFolder(sLastJobFolder);
        }

        /// <summary>
        /// Pause/Resumes Downloading
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PauseCurrentThreadsClick(object sender, EventArgs e)
        {
            switch (this.pauseCurrentThreads.Text)
            {
                case "Pause Download(s)":
                    this.pauseCurrentThreads.Text = "Resume Download(s)";
                    CacheController.Instance().UserSettings.CurrentlyPauseThreads = true;
                    ThreadManager.GetInstance().HoldAllThreads();
                    this.pauseCurrentThreads.Image = Languages.english.play;
                    break;
                case "(Re)Start Download(s)":
                    if (this.InvokeRequired)
                    {
                        this.Invoke(
                            (MethodInvoker)delegate
                            {
                                this.StatusLabelImageC.Text = string.Empty;
                            });
                    }
                    else
                    {
                        StatusLabelImageC.Text = string.Empty;
                    }

                    CacheController.Instance().UserSettings.CurrentlyPauseThreads = false;
                    this.deleteJob.Enabled = true;
                    this.stopCurrentThreads.Enabled = true;
                    this.pauseCurrentThreads.Text = "Pause Download(s)";
                    this.pauseCurrentThreads.Image = Languages.english.pause;
                    break;
                case "Resume Download(s)":
                    if (this.InvokeRequired)
                    {
                        this.Invoke(
                            (MethodInvoker)delegate
                            {
                                this.StatusLabelImageC.Text = string.Empty;
                            });
                    }
                    else
                    {
                        StatusLabelImageC.Text = string.Empty;
                    }

                    CacheController.Instance().UserSettings.CurrentlyPauseThreads = false;
                    this.deleteJob.Enabled = true;
                    this.stopCurrentThreads.Enabled = true;
                    this.pauseCurrentThreads.Text = "Pause Download(s)";
                    
                    if (this.InvokeRequired)
                    {
                        this.Invoke(
                            (MethodInvoker)(() => ThreadManager.GetInstance().ResumeAllThreads()));
                    }
                    else
                    {
                        ThreadManager.GetInstance().ResumeAllThreads();
                    }
                    
                    this.pauseCurrentThreads.Image = Languages.english.pause;
                    break;
            }

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Comboes the box1 selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    CacheController.Instance().UserSettings.DownloadOptions = "0";
                    break;
                case 1:
                    CacheController.Instance().UserSettings.DownloadOptions = "1";
                    break;
                case 2:
                    CacheController.Instance().UserSettings.DownloadOptions = "2";
                    break;
            }

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Mains the form resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainFormResize(object sender, EventArgs e)
        {
#if (!PGRIPPERX)
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.isHiddenInTray = true;

                this.trayIcon.MouseDoubleClick -= this.HideClick;
                this.trayIcon.MouseDoubleClick += this.ShowClick;

                trayMenu.MenuItems.RemoveAt(0);
                MenuItem show = new MenuItem("Show PG-Ripper", this.ShowClick);
                trayMenu.MenuItems.Add(0, show);

                if (CacheController.Instance().UserSettings.ShowPopUps)
                {
                    trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
                    trayIcon.BalloonTipTitle = "Hidden in Tray";
                    trayIcon.BalloonTipText = "PG-Ripper is hidden in the Tray";
                    trayIcon.ShowBalloonTip(10);
                }
            }
#endif
            lvCurJob.Columns[0].Width = lvCurJob.Width - 22;
        }

        /// <summary>
        /// Exits the tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Save all Jobs
        /// </summary>
        public void SaveOnExit()
        {
            if (this.currentJob == null && this.jobsList.Count <= 0)
            {
                return;
            }

            // Save Current Job to quere List
            if (this.currentJob != null)
            {
                this.ripperClosing = true;

                ThreadManager.GetInstance().DismantleAllThreads();

                this.jobsList.Add(this.currentJob);

                this.currentJob = null;
            }

            Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);

            // If Pause
            if (this.pauseCurrentThreads.Text != "Resume Download")
            {
                return;
            }

            CacheController.Instance().UserSettings.CurrentlyPauseThreads = true;
            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Loads the Saved PostsIDs into Array
        /// </summary>
        private void LoadHistory()
        {
            string sFile = Path.Combine(Application.StartupPath, "rippedPostHistory.txt");

            FileStream file = new FileStream(sFile, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string srRead = sr.ReadToEnd();

            if (srRead.Contains("\r\n"))
            {
                srRead = srRead.Replace("\r\n", string.Empty);
            }

            sr.Close();
            file.Close();

            string[] sPostIDs = srRead.Split(';');

            this.sRippedPosts.Clear();

            foreach (string sSavedId in sPostIDs.Where(sSavedId => !string.IsNullOrEmpty(sSavedId)))
            {
                this.sRippedPosts.Add(
                    sSavedId.Contains("&postcount") ? sSavedId.Remove(sSavedId.IndexOf("&postcount")) : sSavedId);
            }

            /*foreach (string sSavedId in sPostIDs)
            {
                if (sSavedId != "")
                {
                    if (sSavedId.Contains("&postcount"))
                    {
                        sRippedPosts.Add(sSavedId.Remove(sSavedId.IndexOf("&postcount")));
                    }
                    else
                    {
                        sRippedPosts.Add(sSavedId);
                    }
                }
            }*/
        }

        /// <summary>
        /// Save the Saved PostsIDs from Array to Textfile
        /// </summary>
        private void SaveHistory()
        {
            string sFile = Path.Combine(Application.StartupPath, "rippedPostHistory.txt");

            FileStream file = new FileStream(sFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(file);

            foreach (string sSavedId in this.sRippedPosts)
            {
                sw.Write("{0};", sSavedId);
            }

            sw.Close();
            file.Close();
        }

        /// <summary>
        /// Check if Post is already Ripped
        /// </summary>
        /// <param name="sPostId">The PostID to Check</param>
        /// <returns>Returns true or false</returns>
        private bool IsPostAlreadyRipped(string sPostId)
        {
            bool bCheck = false;

            if (!Utility.IsNumeric(sPostId))
            {
                return false;
            }

            try
            {
                if (this.sRippedPosts.Contains(sPostId))
                {
                    bCheck = true;
                }
                else
                {
                    this.sRippedPosts.Add(sPostId);
                }
            }
            catch (Exception)
            {
                bCheck = false;
            }

            return bCheck;
        }

        /// <summary>
        /// Open the Options Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsToolStripMenuItem1Click(object sender, EventArgs e)
        {
            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.SaveHistory();
            }

            var oForm = new Options();
            oForm.ShowDialog();

            this.LoadSettings();

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.LoadHistory();
            }
        }

        /// <summary>
        /// Shows the About Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HelpToolStripMenuItemClick(object sender, EventArgs e)
        {
            About aForm = new About();
            aForm.ShowDialog();
        }

        /// <summary>
        /// Update the Download Folder
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BrowsFolderBtnClick(object sender, EventArgs e)
        {
            this.UpdateDownloadFolder();
        }

        /// <summary>
        /// Changes the download Folder.
        /// </summary>
        public void UpdateDownloadFolder()
        {
            if (this.IsBrowserOpen)
            {
                return;
            }

            this.IsBrowserOpen = true;

            if (this.dfolderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.DownloadFolder.Text = this.dfolderBrowserDialog.SelectedPath;

            CacheController.Instance().UserSettings.DownloadFolder = this.DownloadFolder.Text;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);

            this.IsBrowserOpen = false;
        }

        /// <summary>
        /// Set Window Position and Size
        /// </summary>
        private void SetWindow()
        {
            try
            {
                Left = CacheController.Instance().UserSettings.WindowLeft;
                Top = CacheController.Instance().UserSettings.WindowTop;
            }
            catch (Exception)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }

            Width = CacheController.Instance().UserSettings.WindowWidth;
            Height = CacheController.Instance().UserSettings.WindowHeight;
        }

        /// <summary>
        /// Mains the form form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveOnExit();

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.SaveHistory();
            }

            if (this.WindowState == FormWindowState.Minimized || this.isHiddenInTray)
            {
                return;
            }

            CacheController.Instance().UserSettings.WindowLeft = this.Left;
            CacheController.Instance().UserSettings.WindowTop = this.Top;
            CacheController.Instance().UserSettings.WindowWidth = this.Width;
            CacheController.Instance().UserSettings.WindowHeight = this.Height;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Check Windows Clip board for URL's to Rip
        /// </summary>
        private void CheckClipboardData()
        {
            if (!CacheController.Instance().UserSettings.ClipBWatch)
            {
                return;
            }

            try
            {
                var clipboardText = Clipboard.GetText();

                if (string.IsNullOrEmpty(clipboardText))
                {
                    return;
                }

                var clipBoardURLTemp = clipboardText;

                var clipBoardUrLs = clipBoardURLTemp.Split(new[] { '\n' });

                foreach (var clipBoardURL in clipBoardUrLs.Where(clipBoardURL => CacheController.Instance().UserSettings.ForumsAccount.Any(
                    forumAccount => clipBoardURL.StartsWith(forumAccount.ForumURL))))
                {
                    this.CheckUrlForumAccount(this.textBox1.Text);

                    string sClipBoardURLNew = clipBoardURL;

                    if (sClipBoardURLNew.Contains("\r"))
                    {
                        sClipBoardURLNew = sClipBoardURLNew.Replace("\r", string.Empty);
                    }

                    if (clipBoardURL.Contains(@"&postcount=") || clipBoardURL.Contains(@"&page="))
                    {
                        sClipBoardURLNew = Regex.Replace(
                            sClipBoardURLNew, sClipBoardURLNew.Substring(sClipBoardURLNew.IndexOf("&")), string.Empty);
                    }

                    this.comboBox1.SelectedIndex = 2;

                    if (!this.parseActive)
                    {
                        this.comboBox1.SelectedIndex = 2;

                        this.textBox1.Text = sClipBoardURLNew;

                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)this.EnqueueJob);
                        }
                        else
                        {
                            this.EnqueueJob();
                        }
                    }
                    else
                    {
                        this.ExtractUrls.Add(sClipBoardURLNew);
                    }

                    Clipboard.Clear();
                }
            }
            catch (Exception)
            {
                //Clipboard.Clear();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Load"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            Application.Idle += this.OnLoaded;
        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, EventArgs args)
        {
            Application.Idle -= this.OnLoaded;

            this.tmrPageUpdate.Enabled = true;

#if (!PGRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar = TaskbarManager.Instance;
            }
#endif

            this.IdleRipper();

            try
            {
                // Reading Saved Jobs
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextReader tr = new StreamReader(Path.Combine(Application.StartupPath, "jobs.xml"));
                jobsList = (List<JobInfo>)serializer.Deserialize(tr);
                tr.Close();

                if (CacheController.Instance().UserSettings.CurrentlyPauseThreads)
                {
                    pauseCurrentThreads.Text = "(Re)Start Download(s)";
                    pauseCurrentThreads.Image = Languages.english.play;
                }

                File.Delete(Path.Combine(Application.StartupPath, "jobs.xml"));
            }
            catch (Exception)
            {
                jobsList = new List<JobInfo>();
            }

            if (jobsList.Count != 0)
            {
                StatusLabelInfo.Text = this._ResourceManager.GetString("gbParse2");
                StatusLabelInfo.ForeColor = Color.Green;

                for (int i = 0; i < jobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.jobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem { Text = jobsList[i].TopicTitle };

                    ijobJob.SubItems.Add(jobsList[i].PostTitle);
                    ijobJob.SubItems.Add(jobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }

                this.working = true;

                StatusLabelInfo.Text = string.Empty;
            }

            AutoUpdater.CleanupAfterUpdate("PG-Ripper");

            // Extract Urls from Text file for Ripping
            if (File.Exists("ExtractUrls.txt"))
            {
                this.GetTxtUrls("ExtractUrls.txt");
            }
        }

        /// <summary>
        /// Extract The Cached Urls and Rip them.
        /// </summary>
        private void GetExtractUrls()
        {
            if (this.ExtractUrls.Count <= 0 && this.parseActive || this.parseActive)
            {
                return;
            }

            this.comboBox1.SelectedIndex = 2;

            this.textBox1.Text = this.ExtractUrls[0];

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)this.EnqueueJob);
            }
            else
            {
                this.EnqueueJob();
            }

            this.ExtractUrls.RemoveAt(0);

            if (this.ExtractUrls.Count > 0)
            {
                this.GetExtractUrls();
            }
        }

        /// <summary>
        /// Extract Urls from Text file for Ripping
        /// </summary>
        /// <param name="sTextFolder">
        /// Path to the Text File
        /// </param>
        private void GetTxtUrls(string sTextFolder)
        {
            if (!File.Exists(sTextFolder))
            {
                return;
            }

            try
            {
                FileStream file = new FileStream(sTextFolder, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                string srRead = sr.ReadToEnd();
                sr.Close();
                file.Close();

                File.Delete(sTextFolder);

                string[] sRipUrls = srRead.Split(new[] { '\n' });

                foreach (string sRipUrl in sRipUrls)
                {
                    if (!sRipUrl.StartsWith(CacheController.Instance().UserSettings.CurrentForumUrl))
                    {
                        continue;
                    }

                    string sClipBoardURLNew = sRipUrl;

                    if (sClipBoardURLNew.Contains("\r"))
                    {
                        sClipBoardURLNew = sClipBoardURLNew.Replace("\r", string.Empty);
                    }

                    if (sRipUrl.Contains(@"&postcount=") || sRipUrl.Contains(@"&page="))
                    {
                        sClipBoardURLNew = Regex.Replace(
                            sClipBoardURLNew, sClipBoardURLNew.Substring(sClipBoardURLNew.IndexOf("&")), string.Empty);
                    }

                    this.ExtractUrls.Add(sClipBoardURLNew);
                }
            }
            finally
            {
                this.GetExtractUrls();
            }
        }

        /// <summary>
        /// Catches All Unhandled Exceptions and creates a Crash Log
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Unhandled Exception Event Arguments.
        /// </param>
        private void UnhandledExceptionFunction(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, this.currentJob);

            if (this.jobsList.Count > 0)
            {
                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);

                // If Pause
                CacheController.Instance().UserSettings.CurrentlyPauseThreads = true;
                SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
            }

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.SaveHistory();
            }
        }

        /// <summary>
        /// Catches All Unhandled Thread Exceptions and creates a Crash Log
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
        private void ThreadExceptionFunction(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, this.currentJob);

            if (this.jobsList.Count > 0)
            {
                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);

                // If Pause
                if (CacheController.Instance().UserSettings.CurrentlyPauseThreads)
                {
                    SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
                }
            }

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.SaveHistory();
            }
        }

        /// <summary>
        /// Opens last downloaded Image
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PictureBox1DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.Visible && this.pictureBox1.Image != null)
            {
                System.Diagnostics.Process.Start(this.sLastPic);
            }
        }

        /// <summary>
        /// Gets the posts worker do work.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void GetPostsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.ThrdGetPosts(Convert.ToString(e.Argument));
        }

        /// <summary>
        /// Gets the posts worker completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void GetPostsWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.UnlockControls();
        }

        /// <summary>
        /// Gets the index worker do work.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void GetIdxsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.ThrdGetIndexes(Convert.ToString(e.Argument));
        }

        /// <summary>
        /// Gets the index worker completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void GetIdxsWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.UnlockControls();

            Application.DoEvents();

            if (this.indexedTopicsList == null)
            {
                return;
            }

            for (int po = 0; po < this.indexedTopicsList.Count; po++)
            {
                this.StatusLabelInfo.ForeColor = Color.Green;
                this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", "Analyse Index(es)", po, this.indexedTopicsList.Count);

                this.textBox1.Text = this.indexedTopicsList[po].ImageUrl;

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)this.EnqueueJob);
                }
                else
                {
                    this.EnqueueJob();
                }
            }

            this.indexedTopicsList = null;
        }

        /// <summary>
        /// Generates the Storage Folder for the Current Job
        /// </summary>
        /// <param name="job">The Job</param>
        /// <returns>The Storage Folder</returns>
        private string GenerateStorePath(JobInfo job)
        {
            var storePath = CacheController.Instance().UserSettings.DownloadFolder;

            if (!CacheController.Instance().UserSettings.SubDirs)
            {
                return storePath;
            }

            try
            {
                if (job.ForumTitle != null)
                {
                    if (CacheController.Instance().UserSettings.DownInSepFolder)
                    {
                        storePath = this.CheckAndShortenPath(
                            job,
                            Path.Combine(
                                CacheController.Instance().UserSettings.DownloadFolder,
                                string.Format(
                                    "{0}{1}{2}{1}{3}",
                                    Utility.RemoveIllegalCharecters(job.ForumTitle),
                                    Path.DirectorySeparatorChar,
                                    Utility.RemoveIllegalCharecters(job.TopicTitle),
                                    Utility.RemoveIllegalCharecters(job.PostTitle))),
                            true,
                            false);
                    }
                    else
                    {
                        storePath = this.CheckAndShortenPath(
                            job,
                            Path.Combine(
                                CacheController.Instance().UserSettings.DownloadFolder,
                                string.Format(
                                    "{0}{1}{2}",
                                    Utility.RemoveIllegalCharecters(job.ForumTitle),
                                    Path.DirectorySeparatorChar,
                                    Utility.RemoveIllegalCharecters(job.TopicTitle))),
                            false,
                            false);
                    }
                }
                else
                {
                    storePath = this.CheckAndShortenPath(
                        job,
                        Path.Combine(
                            CacheController.Instance().UserSettings.DownloadFolder,
                            CacheController.Instance().UserSettings.DownInSepFolder
                                ? string.Format(
                                    "{0}{1}{2}",
                                    Utility.RemoveIllegalCharecters(job.TopicTitle),
                                    Path.DirectorySeparatorChar,
                                    Utility.RemoveIllegalCharecters(job.PostTitle))
                                : Utility.RemoveIllegalCharecters(job.TopicTitle)),
                        false,
                        false);
                }

                var renameCount = 2;

                var begining = storePath;

                // Auto Rename if post titles are the same...
                if (this.jobsList.Count != 0)
                {
                    var path = storePath;

                    foreach (JobInfo t in
                        this.jobsList.Where(
                            t =>
                            t.PostTitle.Equals(job.PostTitle) ||
                            Directory.Exists(path) && t.TopicTitle.Equals(job.TopicTitle)))
                    {
                        while (t.StorePath.Equals(storePath) || Directory.Exists(storePath))
                        {
                            storePath = string.Format("{0} Set# {1}", begining, renameCount);
                            renameCount++;
                        }
                    }
                }
                else
                {
                    while (Directory.Exists(storePath))
                    {
                        storePath = string.Format("{0} Set# {1}", begining, renameCount);
                        renameCount++;
                    }
                }
            }
            catch (Exception)
            {
                storePath = Path.Combine(
                    CacheController.Instance().UserSettings.DownloadFolder,
                    Utility.RemoveIllegalCharecters(job.TopicTitle));
            }

            return storePath;
        }

        /// <summary>
        /// Checks the Path if its too long and shorten path.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="path">The path.</param>
        /// <param name="forumTitle">if set to <c>true</c> [forum title].</param>
        /// <param name="postTitle">if set to <c>true</c> [post title].</param>
        /// <returns>Returns the Shortened Path</returns>
        private string CheckAndShortenPath(JobInfo job, string path, bool forumTitle, bool postTitle)
        {
            // check path length
            if (path.Length > 248 && forumTitle && !postTitle)
            {
                path = Path.Combine(
                            CacheController.Instance().UserSettings.DownloadFolder,
                            string.Format(
                                "{0}{1}{2}",
                                Utility.RemoveIllegalCharecters(job.ForumTitle),
                                Path.DirectorySeparatorChar,
                                Utility.RemoveIllegalCharecters(job.TopicTitle)));

                path = this.CheckAndShortenPath(job, path, false, false);
            }

            if (path.Length > 248 && !forumTitle && !postTitle)
            {
                path = Path.Combine(
                        CacheController.Instance().UserSettings.DownloadFolder,
                        CacheController.Instance().UserSettings.DownInSepFolder
                            ? string.Format(
                                "{0}{1}{2}",
                                Utility.RemoveIllegalCharecters(job.TopicTitle),
                                Path.DirectorySeparatorChar,
                                Utility.RemoveIllegalCharecters(job.PostTitle))
                            : Utility.RemoveIllegalCharecters(job.TopicTitle));

                path = this.CheckAndShortenPath(job, path, false, false);
            }

            // if Path is still to long simply put in download folder
            if (path.Length > 248)
            {
                path = CacheController.Instance().UserSettings.DownloadFolder;
            }


            return path;
        }

        /// <summary>
        /// Unlock (Enable) Elements after Parsing
        /// </summary>
        private void UnlockControls()
        {
            this.parseActive = false;

            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)this.UnlockControlsElements);
            }
            else
            {
                this.UnlockControlsElements();
            }
        }

        /// <summary>
        /// Unlock the Controls
        /// </summary>
        private void UnlockControlsElements()
        {
            textBox1.Text = string.Empty;
            textBox1.Enabled = true;
            mStartDownloadBtn.Enabled = true;

            StatusLabelInfo.Text = string.Empty;
        }

        /// <summary>
        /// Unlock (Enable) Elements while Parsing
        /// </summary>
        private void LockControls()
        {
            this.parseActive = true;

            textBox1.Enabled = false;
            mStartDownloadBtn.Enabled = false;

            this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbParse");
            StatusLabelInfo.ForeColor = Color.Green;
        }

        /// <summary>
        /// Enable/Disable Show last image
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ShowLastImageToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            CacheController.Instance().UserSettings.ShowLastDownloaded = this.showLastImageToolStripMenuItem.Checked;

            this.groupBox4.Visible = this.showLastImageToolStripMenuItem.Checked;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Enable/Disable Clipboard Monitoring
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UseCliboardMonitoringToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            CacheController.Instance().UserSettings.ClipBWatch = this.useCliboardMonitoringToolStripMenuItem.Checked;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the DoNothingToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DoNothingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.closeRipperToolStripMenuItem.Checked = !this.doNothingToolStripMenuItem.Checked;

            CacheController.Instance().UserSettings.AfterDownloads = 0;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the CloseRipperToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CloseRipperToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.doNothingToolStripMenuItem.Checked = !this.closeRipperToolStripMenuItem.Checked;

            CacheController.Instance().UserSettings.AfterDownloads = 1;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);
        }

        /// <summary>
        /// Handles the event when the download folder is changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DownloadFolder_TextChanged(object sender, EventArgs e)
        {
            /*CacheController.Instance().UserSettings.DownloadFolder = this.DownloadFolder.Text;

            SettingsHelper.SaveSettings(CacheController.Instance().UserSettings);*/
        }

        /// <summary>
        /// Exports the Current Jobs Queue to the jobs.xml file
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveRippingQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.jobsList.Count > 0)
            {
                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);
            }
        }
    }
}