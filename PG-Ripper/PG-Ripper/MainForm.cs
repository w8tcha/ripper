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

namespace PGRipper
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

#if (!PGRIPPERX)
    using Microsoft.WindowsAPICodePack.Shell;
    using Microsoft.WindowsAPICodePack.Taskbar;
#endif
    using PGRipper.Objects;

    /// <summary>
    /// The Main Form
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// All Settings
        /// </summary>
        public SettingBase userSettings = new SettingBase();

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

        /// <summary>
        /// The Jump List
        /// </summary>
        private JumpList jumpList;
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
        /// Indicates that Ripper is Finishing the last Threads
        /// </summary>
        private bool endingRip;

        /// <summary>
        /// Indicates that users Stopping and Deleting Current Job
        /// </summary>
        private bool stopingJob;

        /// <summary>
        /// Indicates that Disc is full and Ripping stoped
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

            mJobsList = new List<JobInfo>();
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
        /// Gets or sets a value indicating whether if the Browse Dialog is Already Opten
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
            if (Clipboard.GetText().IndexOf(this.userSettings.CurrentForumUrl) < 0 ||
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
            Hide();

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem show = new MenuItem("Show PG-Ripper", this.ShowClick);
            trayMenu.MenuItems.Add(0, show);

            this.trayIcon.MouseDoubleClick -= this.HideClick;
            this.trayIcon.MouseDoubleClick += this.ShowClick;

            if (!this.userSettings.ShowPopUps)
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

            CacheController.Xform = new MainForm();
            ProcessArgs(args, CacheController.Xform);

            Application.Run(CacheController.Xform);
        }

        /// <summary>
        /// Processes the args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="aForm">A form.</param>
        private static void ProcessArgs(ICollection<string> args, Form aForm)
        {
            if (args.Count == 0)
            {
                return;
            }

            foreach (string curArg in args.Select(t => t.ToUpper()))
            {
                string aValue;
                if (CheckArg("MIN", curArg, out aValue))
                {
                    aForm.WindowState = FormWindowState.Minimized;
                }
            }
        }

        /// <summary>
        /// Checks the arg.
        /// </summary>
        /// <param name="aExpected">
        /// A expected.
        /// </param>
        /// <param name="aSupplied">
        /// A supplied.
        /// </param>
        /// <param name="aSuppliedValue">
        /// A supplied value.
        /// </param>
        /// <returns>
        /// The check arg.
        /// </returns>
        private static bool CheckArg(string aExpected, string aSupplied, out string aSuppliedValue)
        {
            int equalsPos = aSupplied.IndexOf("=");
            string lSupplied;
            if (equalsPos > -1)
            {
                aSuppliedValue = aSupplied.Substring(equalsPos + 1);
                lSupplied = aSupplied.Substring(0, equalsPos);
            }
            else
            {
                aSuppliedValue = string.Empty;
                lSupplied = aSupplied;
            }

            return lSupplied == aExpected || lSupplied == string.Format("/{0}", aExpected) || lSupplied == string.Format("\\{0}", aExpected);
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
            this.userSettings = Utility.LoadSettings();

            // Max. Threads
            this.userSettings.ThreadLimit = this.userSettings.ThreadLimit == -1 ? 3 : this.userSettings.ThreadLimit;
            ThreadManager.GetInstance().SetThreadThreshHold(this.userSettings.ThreadLimit);

            if (string.IsNullOrEmpty(this.userSettings.DownloadFolder))
            {
                this.userSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                this.textBox2.Text = this.userSettings.DownloadFolder;

                Utility.SaveSettings(this.userSettings);
            }

            this.textBox2.Text = this.userSettings.DownloadFolder;

            // Load "Download Options"
            try
            {
                this.comboBox1.SelectedIndex = Convert.ToInt32(this.userSettings.DownloadOptions);
            }
            catch (Exception)
            {
                this.userSettings.DownloadOptions = "0";
                this.comboBox1.SelectedIndex = 0;
            }

            this.TopMost = this.userSettings.TopMost;

            // Load Language Setting
            try
            {
                switch (this.userSettings.Language)
                {
                    case "de-DE":
                        this._ResourceManager = new ResourceManager(
                            "PGRipper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        this._ResourceManager = new ResourceManager(
                            "PGRipper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        this._ResourceManager = new ResourceManager(
                            "PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    default:
                        this._ResourceManager = new ResourceManager(
                            "PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                this.AdjustCulture();
            }
            catch (Exception)
            {
                this._ResourceManager = new ResourceManager(
                    "PGRipper.Languages.english", Assembly.GetExecutingAssembly());
            }

            // Load Show Last Download Image
            this.groupBox4.Visible = this.userSettings.ShowLastDownloaded;

            try
            {
                var accountNotFound =
                    this.userSettings.ForumsAccount.Where(
                        forumAccount => forumAccount.ForumURL.Equals(this.userSettings.CurrentForumUrl)).Any(
                            forumAccount =>
                            !string.IsNullOrEmpty(forumAccount.UserName)
                            && !string.IsNullOrEmpty(forumAccount.UserPassWord));

                if (accountNotFound)
                {
                    return;
                }

                var frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (this.bCameThroughCorrectLogin)
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

                if (this.bCameThroughCorrectLogin)
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
            this.settingsToolStripMenuItem1.Text = this._ResourceManager.GetString("MenuSettings");
            this.exitToolStripMenuItem.Text = this._ResourceManager.GetString("MenuExit");
            this.accountsToolStripMenuItem.Text = this._ResourceManager.GetString("MenuAccounts");

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

            this.mrefCC = CacheController.GetInstance();
            this.mrefTM = ThreadManager.GetInstance();

            this.LoadSettings();

            this.SetWindow();

#if (!PGRIPPERX)
            string mbUpdate = this._ResourceManager.GetString("mbUpdate"), mbUpdate2 = this._ResourceManager.GetString("mbUpdate2");

            if (VersionCheck.UpdateAvailable() && File.Exists(Path.Combine(Application.StartupPath, "ICSharpCode.SharpZipLib.dll")))
            {
                DialogResult result = TopMostMessageBox.Show(
                    mbUpdate, mbUpdate2, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Clipboard.Clear();

                    Enabled = false;
                    WindowState = FormWindowState.Minimized;

                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    trayIcon.BalloonTipTitle = "Update in Progress";
                    trayIcon.BalloonTipText = "Currently upgrading to newest version.";
                    trayIcon.ShowBalloonTip(10);

                    AutoUpdater.TryUpdate();
                }
            }
#endif

            this.AutoLogin();

            if (!bCameThroughCorrectLogin)
            {
                Application.Exit();
            }
            else
            {
                this.CheckAccountMenu();
            }

            if (this.userSettings.SavePids)
            {
                this.LoadHistory();
            }

            // Hide Index Thread Checkbox if not RiP Forums
            if (!this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net"))
            {
                mIsIndexChk.Visible = false;
            }

#if (!PGRIPPERX)
            trayIcon.Visible = true;
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
                Text = "Add New Forum Account",
                Checked = false
            };

            newAccountMenuItem.Click += this.AddNewAccount_Click;

            this.accountsToolStripMenuItem.DropDownItems.Add(newAccountMenuItem);

            foreach (var accounts in this.userSettings.ForumsAccount)
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
                        Checked = this.userSettings.CurrentForumUrl.Equals(accounts.ForumURL)
                    };

                forumMenuItem.Click += this.ForumMenuItem_Click;

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
        /// Checks the URL if it matches the current forum account.
        /// </summary>
        /// <param name="forumUrl">The forum URL.</param>
        private void CheckUrlForumAccount(string forumUrl)
        {
            if (forumUrl.StartsWith(this.userSettings.CurrentForumUrl))
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
                this.userSettings.ForumsAccount.Where(forumAccount => forumUrl.StartsWith(forumAccount.ForumURL)).Where(
                    forumAccount =>
                    !string.IsNullOrEmpty(forumAccount.UserName) && !string.IsNullOrEmpty(forumAccount.UserPassWord)))
            {
                currentAccount = forumAccount;
            }

            if (currentAccount != null)
            {
                this.userSettings.CurrentForumUrl = currentAccount.ForumURL;
                this.userSettings.CurrentUserName = currentAccount.UserName;

                LoginManager lgnMgr = new LoginManager(currentAccount.UserName, currentAccount.UserPassWord);

                if (lgnMgr.DoLogin())
                {
                    bCameThroughCorrectLogin = true;
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
                string.Format("{0}{1}\" @ {2} ]", header, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));
#elif (PGRIPPERX)
                Text = string.Format("PG-Ripper X {0}.{1}.{2}{3}{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format("{0}{1}\" @ {2} ]", header, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));
#else
            Text = string.Format(
                "PG-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format(
                    "{0}{1}\" @ {2} ]", header, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));
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

            foreach (var forumAccount in this.userSettings.ForumsAccount.Where(
                    forumAccount =>
                        forumAccount.ForumURL.Equals(this.userSettings.CurrentForumUrl)).Where(
                        forumAccount => !string.IsNullOrEmpty(forumAccount.UserName) && !string.IsNullOrEmpty(forumAccount.UserPassWord)))
            {
                currentForumAccount = forumAccount;
                accountExists = true;
            }

            if (accountExists)
            {
                LoginManager lgnMgr = new LoginManager(currentForumAccount.UserName, currentForumAccount.UserPassWord);

                if (lgnMgr.DoLogin())
                {
                    bCameThroughCorrectLogin = true;
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
            this.CheckUrlForumAccount(this.textBox1.Text);

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
            if (this.userSettings.SavePids)
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
                if (this.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                    this.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                    this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net") ||
                    this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com"))
                {
                    if (!sHtmlUrl.Contains(@"#post"))
                    {
                        this.EnqueueThreadToPost(sHtmlUrl);
                    }
                    else
                    {
                        this.EnqueueThreadOrPost(sHtmlUrl);
                    }
                }
                else
                {
                    if (sHtmlUrl.Contains(@"showthread.php"))
                    {
                        this.EnqueueThreadToPost(sHtmlUrl);
                    }
                    else
                    {
                        this.EnqueueThreadOrPost(sHtmlUrl);
                    }
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

            if (string.IsNullOrEmpty(this.userSettings.DownloadFolder))
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
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
            }
#endif

            try
            {
                if (
                    string.IsNullOrEmpty(
                        Maintainance.GetInstance().GetRipPageTitle(Maintainance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                        this.windowsTaskbar.SetProgressValue(10, 100);
                        this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
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
                    this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
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
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
            }
#endif

           try
            {
                if (
                    string.IsNullOrEmpty(
                        Maintainance.GetInstance().GetRipPageTitle(Maintainance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                        this.windowsTaskbar.SetProgressValue(10, 100);
                        this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
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
                    this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
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

            GetPostsWorker.RunWorkerAsync(sHtmlUrl);
        }

        /// <summary>
        /// Parse a Thread or Post as Single Job
        /// </summary>
        /// <param name="sHtmlUrl">The Thread/Post Url</param>
        private void EnqueueThreadOrPost(string sHtmlUrl)
        {
            if (mJobsList.Any(t => t.URL == sHtmlUrl))
            {
                TopMostMessageBox.Show(mAlreadyQueuedMsg, "Info");
                return;
            }

            JobInfo job = new JobInfo
                { URL = sHtmlUrl, HtmlPayLoad = Maintainance.GetInstance().GetPostPages(sHtmlUrl) };

            job.Title = Utility.ReplaceHexWithAscii(Maintainance.GetInstance().GetRipPageTitle(job.HtmlPayLoad));

            if (this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net/") ||
                this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"passesforthemasses.com/") ||
                this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com"))
            {
                job.SecurityToken = Maintainance.GetInstance().GetSecurityToken(job.HtmlPayLoad);
            }

            if (!sHtmlUrl.Contains(@"showthread") || sHtmlUrl.Contains(@"#post"))
            {
                job.PostTitle =
                    Utility.ReplaceHexWithAscii(
                        Maintainance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, sHtmlUrl));

                if (this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net"))
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.URL, true);
                }
                else
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.HtmlPayLoad, true);

                    job.ForumTitle =
                        job.ForumTitle.Substring(
                            job.ForumTitle.IndexOf(string.Format("{0} ", job.Title)) + job.Title.Length + 1);
                }
            }
            else
            {
                if (this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net"))
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.URL, false);
                }
                else
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.HtmlPayLoad, false);

                    job.ForumTitle =
                        job.ForumTitle.Substring(
                            job.ForumTitle.IndexOf(string.Format("{0} ", job.Title)) + job.Title.Length + 1);
                }
            }

            job.ImageList = Utility.ExtractImagesHtml(job.HtmlPayLoad, sHtmlUrl);
            job.ImageCount = job.ImageList.Count;

            if (job.ImageCount == 0)
            {
                // Unlock Controls
                this.UnlockControls();

                return;
            }

            if (string.IsNullOrEmpty(job.Title))
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
            if (!this.userSettings.AutoThank)
            {
                return;
            }

            if (iICount < this.userSettings.MinImageCount)
            {
                return;
            }

            SendThankYouDelegate lSendThankYouDel = this.SendThankYou;

            if (sPostId == null)
            {
                return;
            }

            string tyURL;
            if (this.userSettings.CurrentForumUrl.Contains(@"scanlover.com"))
            {
                tyURL = string.Format("{0}post_thanks.php?do=post_thanks_add&p={1}", this.userSettings.CurrentForumUrl, sPostId);
            }
            else if (this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net") ||
                     this.userSettings.CurrentForumUrl.Contains(@"passesforthemasses.com/") ||
                     this.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                     this.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                    this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com"))
            {
                tyURL = string.Format(
                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                    this.userSettings.CurrentForumUrl,
                    sPostId,
                    sSecurityToken);
            }
            else
            {
                return;
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
            string tyURLRef = this.userSettings.CurrentForumUrl;

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

            string sPagecontent = this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net")
                                  || this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net")
                                  || this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com")
                                      ? idxs.GetThreadPagesNew(sHtmlUrl)
                                      : idxs.GetThreadPages(sHtmlUrl);

            this.indexedTopicsList = idxs.ParseHtml(sPagecontent, sHtmlUrl);
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

            string sPagecontent;

            if (this.userSettings.CurrentForumUrl.Contains(@"http://rip-") || this.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net"))
            {
                sPagecontent = threads.GetThreadPagesNew(htmlUrl);
            }
            else
            {
                sPagecontent = threads.GetThreadPages(htmlUrl);
            }

            string sForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(htmlUrl, false);

            List<ImageInfo> arlst = threads.ParseHtml(sPagecontent);

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

                                StatusLabelInfo.ForeColor = Color.Green;
                            });
                }
                else
                {
                    this.StatusLabelInfo.Text = string.Format(
                       "{0}{1}/{2}", this._ResourceManager.GetString("gbParse"), po, arlst.Count);
                    StatusLabelInfo.ForeColor = Color.Green;
                }

                string sLpostId = arlst[po].ImageUrl;

                //////////////////////////////////////////////////////////////////////////

                if (this.userSettings.SavePids && this.IsPostAlreadyRipped(sLpostId))
                {
                    goto SKIPIT;
                }

                string sLComposedURL = this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net")
                                       || this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net")
                                       || this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com")
                                           ? string.Format("{0}#post{1}", htmlUrl, sLpostId)
                                           : string.Format(
                                               "{0}showpost.php?p={1}", this.userSettings.CurrentForumUrl, sLpostId);

                JobInfo jobInfo = mJobsList.Find(doubleJob => doubleJob.URL.Equals(sLComposedURL));

                if (jobInfo != null)
                {
                    goto SKIPIT;
                }

                if (mCurrentJob != null)
                {
                    if (mCurrentJob.URL.Equals(sLComposedURL))
                    {
                        goto SKIPIT;
                    }
                }

                JobInfo job = new JobInfo
                    { URL = sLComposedURL, HtmlPayLoad = Maintainance.GetInstance().GetPostPages(sLComposedURL) };

                if (string.IsNullOrEmpty(job.HtmlPayLoad))
                {
                    goto SKIPIT;
                }

                job.Title = Maintainance.GetInstance().GetRipPageTitle(job.HtmlPayLoad);

                if (this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net") ||
                    this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"passesforthemasses.com/") ||
                    this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                    this.userSettings.AutoThank & this.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                    this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com"))
                {
                    job.SecurityToken = Maintainance.GetInstance().GetSecurityToken(job.HtmlPayLoad);
                }

                job.ForumTitle = this.userSettings.CurrentForumUrl.Contains(@"rip-productions.net")
                                 || this.userSettings.CurrentForumUrl.Contains(@"kitty-kats.net")
                                 || this.userSettings.CurrentForumUrl.Contains(@"bignaturalsonly.com")
                                     ? sForumTitle
                                     : sForumTitle.Substring(
                                         sForumTitle.IndexOf(string.Format("{0} ", job.Title)) + job.Title.Length + 1);

                job.PostTitle = Maintainance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, sLComposedURL);
                job.Title = Utility.ReplaceHexWithAscii(job.Title);

                job.ImageList = Utility.ExtractImagesHtml(job.HtmlPayLoad, sLpostId);
                job.ImageCount = job.ImageList.Count;

                if (job.ImageCount == 0)
                {
                    goto SKIPIT;
                }

                job.StorePath = this.GenerateStorePath(job);

                JobListAddDelegate newJob = this.JobListAdd;

                Invoke(newJob, new object[] { job });

                //// JobListAdd(job);

                //////////////////////////////////////////////////////////////////////////
                SKIPIT:
                continue;
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
            if (this.working && !this.parseActive || this.mJobsList.Count > 0 && !this.parseActive)
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
                if (this.mCurrentJob == null && this.mJobsList.Count.Equals(0) && !this.parseActive)
                {
                    this.endingRip = true;

                    this.StatusLabelInfo.Text = this._ResourceManager.GetString("StatusLabelInfo");
                    StatusLabelInfo.ForeColor = Color.Red;

                    this.groupBox5.Text = string.Format("{0} (-):", this._ResourceManager.GetString("lblRippingQue"));
                }
                else
                {
                    this.endingRip = false;
                }
            }
            else
            {
                // Check if Last Downloadfolder is Empty
                if (!string.IsNullOrEmpty(this.lastDownFolder))
                {
                    CheckCurJobFolder(this.lastDownFolder);
                }

                if (!this.userSettings.CurrentlyPauseThreads)
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

                    if (mCurrentJob == null && mJobsList.Count > 0)
                    {
#if (!PGRIPPERX)
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                            Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
                        {
                            this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                            this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
                        }
#endif

                        // STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
                        this.ProcessNextJob();
                    }
                    else if (mCurrentJob == null && mJobsList.Count.Equals(0))
                    {
                        this.IdleRipper();
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

            if (mJobsList.Count == 0)
            {
                return;
            }

            mCurrentJob = mJobsList[0];

            this.JobListRemove(0);

            string bSystemExtr = this._ResourceManager.GetString("bSystemExtr");

            this.ParseJob();

            // NO IMAGES TO PROCESS SO ABANDON CURRENT THREAD
            if (mImagesList == null || mImagesList.Count <= 0)
            {
                mCurrentJob = null;
                deleteJob.Enabled = true;
                stopCurrentThreads.Enabled = true;
                return;
            }

            this.groupBox2.Text = string.Format("{0}...", this._ResourceManager.GetString("gbCurrentlyExtract"));

            if (mCurrentJob.Title.Equals(mCurrentJob.PostTitle))
            {
                Text = string.Format(
                    "{0}: {1} - x{2}", this._ResourceManager.GetString("gbCurrentlyExtract"), this.mCurrentJob.Title, this.mImagesList.Count);
            }
            else
            {
                Text = string.Format(
                    "{0}: {1} - {2} - x{3}",
                    this._ResourceManager.GetString("gbCurrentlyExtract"),
                    mCurrentJob.Title,
                    mCurrentJob.PostTitle,
                    mImagesList.Count);
            }

            lvCurJob.Columns[0].Text = string.Format("{0} - x{1}", mCurrentJob.PostTitle, mImagesList.Count);

#if (!PGRIPPERX)
            try
            {
                if (this.userSettings.ShowPopUps)
                {
                    this.trayIcon.Text = this._ResourceManager.GetString("gbCurrentlyExtract") + this.mCurrentJob.Title;
                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    this.trayIcon.BalloonTipTitle = this._ResourceManager.GetString("gbCurrentlyExtract");
                    trayIcon.BalloonTipText = mCurrentJob.Title;
                    trayIcon.ShowBalloonTip(10);
                }
            }
            catch (Exception)
            {
                if (this.userSettings.ShowPopUps)
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

            this.lastDownFolder = mCurrentJob.StorePath;

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
                        if (progressBar1 != null)
                        {
                            progressBar1.Value = i;
                        }

                        this.StatusLabelImageC.Text = string.Format(
                            tiImagesRemain, mImagesList.Count - i, i * 100 / mImagesList.Count);
                    }

                    try
                    {
                        // Attempts to download an image when the URL is longer than http://
                        if (mImagesList[i].ImageUrl.Length > 6)
                        {
                            if (!(i > lvCurJob.Items.Count))
                            {
                                lvCurJob.Items[i].Selected = true;
                                lvCurJob.EnsureVisible(i);
                            }

                            CacheController.GetInstance().DownloadImage(mImagesList[i].ImageUrl, mCurrentJob.StorePath);
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
                mCurrentJob = null;

                if (!string.IsNullOrEmpty(this.lastDownFolder))
                {
                    CheckCurJobFolder(this.lastDownFolder);
                }

                if (mJobsList.Count > 0)
                {
#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                        this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
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
            }
        }

        private Image imgLastPic;

        /// <summary>
        /// Shows the last Downloaded Image
        /// </summary>
        private void ShowLastPic()
        {
            if (!this.userSettings.ShowLastDownloaded)
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

            this.sLastPic = this.mrefCC.uSLastPic;

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

            mJobsList.Add(job);

            ListViewItem ijobJob = new ListViewItem { Text = job.Title };

            ijobJob.SubItems.Add(job.PostTitle);
            ijobJob.SubItems.Add(job.ImageCount.ToString());

            listViewJobList.Items.AddRange(new[] { ijobJob });

            this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), this.mJobsList.Count);
        }

        /// <summary>
        /// Removes a Job from the Joblist and ListView
        /// </summary>
        /// <param name="iJobIndex">The Index of the Job inside the Joblist</param>
        private void JobListRemove(int iJobIndex)
        {
            mJobsList.RemoveAt(iJobIndex);

            listViewJobList.Items.RemoveAt(iJobIndex);

            this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), this.mJobsList.Count);
        }

        /// <summary>
        /// Idles the ripper.
        /// </summary>
        private void IdleRipper()
        {
#if (!PGRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                this.windowsTaskbar.SetProgressValue(10, 100);
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
            }

            string btleExit = this._ResourceManager.GetString("btleExit"), btexExit = this._ResourceManager.GetString("btexExit");

            if (this.endingRip && this.userSettings.ShowCompletePopUp)
            {
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.BalloonTipTitle = btleExit;
                trayIcon.BalloonTipText = btexExit;
                trayIcon.ShowBalloonTip(10);
            }
#endif
            stopCurrentThreads.Enabled = true;
            this.stopingJob = false;
            this.endingRip = false;
            this.parseActive = false;

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

            textBox1.Text = string.Empty;

            progressBar1.Value = 0;

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
                string.Format("{0}{1}\" @ {2} ]", ttlHeader, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));

            trayIcon.Text = "Right click for context menu";
#elif (PGRIPPERX)
                this.Text = string.Format("PG-Ripper X {0}.{1}.{2}{3}{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format("{0}{1}\" @ {2} ]", ttlHeader, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));
#else
            this.Text = string.Format(
                "PG-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format("{0}{1}\" @ {2} ]", ttlHeader, this.userSettings.CurrentUserName, this.userSettings.CurrentForumUrl));

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

            Invoke(updateJob, new object[] { mCurrentJob });

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

            mCurrentJob = null;

            this.UpdateDownloadFolder();

            for (int i = 0; i != mJobsList.Count; i++)
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

                mJobsList[i].StorePath = this.userSettings.DownloadFolder;

                if (!this.userSettings.SubDirs)
                {
                    continue;
                }

                if (comboBox1.SelectedIndex == 0)
                {
                    mJobsList[i].StorePath = Path.Combine(this.userSettings.DownloadFolder, mJobsList[i].Title);
                }

                if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                {
                    mJobsList[i].StorePath = Path.Combine(
                        this.userSettings.DownloadFolder,
                        mJobsList[i].Title + Path.DirectorySeparatorChar + mJobsList[i].PostTitle);
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

            if (mCurrentJob.URL.Contains("showpost.php"))
            {
                sPostIdStart = "showpost.php?p=";
            }
            else if (mCurrentJob.URL.Contains("#post"))
            {
                sPostIdStart = "#post";
            }

            string postId = mCurrentJob.URL.Substring(mCurrentJob.URL.IndexOf(sPostIdStart) + sPostIdStart.Length);

            if (postId.Contains(@"postcount"))
            {
                postId = Regex.Replace(postId, postId.Substring(postId.IndexOf("postcount=") - 1), string.Empty);
            }

            mImagesList = mCurrentJob.ImageList;

            this.ProcessAutoThankYou(postId, this.mImagesList.Count, this.mCurrentJob.SecurityToken);
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
                if (listViewJobList.SelectedItems.Count > 0)
                {
                    mJobsList.RemoveRange(listViewJobList.SelectedItems[0].Index, listViewJobList.SelectedIndices.Count);

                    foreach (ListViewItem deleteItem in listViewJobList.SelectedItems)
                    {
                        listViewJobList.Items.Remove(deleteItem);
                    }
                }
            }
            catch (Exception)
            {
                listViewJobList.Items.Clear();

                for (int i = 0; i < mJobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.mJobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(mJobsList[i].Title, 0);
                    ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }
            }
            finally
            {
                this.groupBox5.Text = string.Format("{0} ({1}):", this._ResourceManager.GetString("lblRippingQue"), mJobsList.Count);
            }
        }

        /// <summary>
        /// Kill and Deletes the Current Job
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void StopCurrentThreadsClick(object sender, EventArgs e)
        {
            if (mCurrentJob == null)
            {
                return;
            }

            stopCurrentThreads.Enabled = false;
            this.stopingJob = true;

            ThreadManager.GetInstance().DismantleAllThreads();

            string sLastJobFolder = mCurrentJob.StorePath;

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

            CheckCurJobFolder(sLastJobFolder);
        }

        /// <summary>
        /// Pause/Resumes Downloading
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PauseCurrentThreadsClick(object sender, EventArgs e)
        {
            switch (pauseCurrentThreads.Text)
            {
                case "Pause Download(s)":
                    pauseCurrentThreads.Text = "Resume Download(s)";
                    this.userSettings.CurrentlyPauseThreads = true;
                    ThreadManager.GetInstance().HoldAllThreads();
                    pauseCurrentThreads.Image = Languages.english.play;
                    break;
                case "(Re)Start Download(s)":
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

                    this.userSettings.CurrentlyPauseThreads = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    pauseCurrentThreads.Text = "Pause Download(s)";
                    pauseCurrentThreads.Image = Languages.english.pause;
                    break;
                case "Resume Download(s)":
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

                    this.userSettings.CurrentlyPauseThreads = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    pauseCurrentThreads.Text = "Pause Download(s)";
                    ThreadManager.GetInstance().ResumeAllThreads();
                    pauseCurrentThreads.Image = Languages.english.pause;
                    break;
            }

            Utility.SaveSettings(this.userSettings);
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
                    this.userSettings.DownloadOptions = "0";
                    break;
                case 1:
                    this.userSettings.DownloadOptions = "1";
                    break;
                case 2:
                    this.userSettings.DownloadOptions = "2";
                    break;
            }

            Utility.SaveSettings(this.userSettings);
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
                Hide();
                this.trayIcon.MouseDoubleClick -= this.HideClick;
                this.trayIcon.MouseDoubleClick += this.ShowClick;

                trayMenu.MenuItems.RemoveAt(0);
                MenuItem show = new MenuItem("Show PG-Ripper", this.ShowClick);
                trayMenu.MenuItems.Add(0, show);

                if (this.userSettings.ShowPopUps)
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
            if (mCurrentJob == null && mJobsList.Count <= 0)
            {
                return;
            }

            // Save Current Job to quere List
            if (mCurrentJob != null)
            {
                this.ripperClosing = true;

                ThreadManager.GetInstance().DismantleAllThreads();

                mJobsList.Add(mCurrentJob);

                mCurrentJob = null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
            TextWriter tr = new StreamWriter(Path.Combine(Application.StartupPath, "jobs.xml"));
            serializer.Serialize(tr, mJobsList);
            tr.Close();

            // If Pause
            if (this.pauseCurrentThreads.Text != "Resume Download")
            {
                return;
            }

            this.userSettings.CurrentlyPauseThreads = true;
            Utility.SaveSettings(this.userSettings);
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
        /// Settingses the tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsToolStripMenuItem1Click(object sender, EventArgs e)
        {
            if (this.userSettings.SavePids)
            {
                this.SaveHistory();
            }

            Options oForm = new Options();
            oForm.ShowDialog();

            this.LoadSettings();

            if (this.userSettings.SavePids)
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

            this.textBox2.Text = this.dfolderBrowserDialog.SelectedPath;

            this.userSettings.DownloadFolder = this.textBox2.Text;

            Utility.SaveSettings(this.userSettings);

            this.IsBrowserOpen = false;
        }

        /// <summary>
        /// Set Window Position and Size
        /// </summary>
        private void SetWindow()
        {
            try
            {
                Left = this.userSettings.WindowLeft;
                Top = this.userSettings.WindowTop;
            }
            catch (Exception)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }

            Width = this.userSettings.WindowWidth;
            Height = this.userSettings.WindowHeight;
        }

        /// <summary>
        /// Mains the form form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveOnExit();

            if (this.userSettings.SavePids)
            {
                this.SaveHistory();
            }

            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            this.userSettings.WindowLeft = Left;
            this.userSettings.WindowTop = Top;
            this.userSettings.WindowWidth = Width;
            this.userSettings.WindowHeight = Height;

            Utility.SaveSettings(this.userSettings);
        }

        /// <summary>
        /// Checks the clipboard data.
        /// </summary>
        private void CheckClipboardData()
        {
            if (!this.userSettings.ClipBWatch)
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(Clipboard.GetText()))
                {
                    return;
                }

                var clipBoardURLTemp = Clipboard.GetText();

                var sClipBoardUrLs = clipBoardURLTemp.Split(new[] { '\n' });

                foreach (string sClipBoardURL in sClipBoardUrLs.Where(sClipBoardURL => this.userSettings.ForumsAccount.Any(
                    forumAccount => sClipBoardURL.StartsWith(forumAccount.ForumURL))))
                {
                    this.CheckUrlForumAccount(this.textBox1.Text);

                    string sClipBoardURLNew = sClipBoardURL;

                    if (sClipBoardURLNew.Contains("\r"))
                    {
                        sClipBoardURLNew = sClipBoardURLNew.Replace("\r", string.Empty);
                    }

                    if (sClipBoardURL.Contains(@"&postcount=") || sClipBoardURL.Contains(@"&page="))
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
                Clipboard.Clear();
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

                // create a new taskbar jump list for the main window
                this.jumpList = JumpList.CreateJumpList();

                if (!string.IsNullOrEmpty(this.userSettings.DownloadFolder))
                {
                    // Add our user tasks
                    this.jumpList.AddUserTasks(
                        new JumpListLink(this.userSettings.DownloadFolder, "Open Download Folder")
                            {
                                IconReference =
                                    new IconReference(
                                    Path.Combine(
                                        Application.StartupPath, string.Format("{0}.exe", Assembly.GetExecutingAssembly().GetName().Name)),
                                    0)
                            });

                    this.jumpList.Refresh();
                }
            }
#endif

            this.IdleRipper();

            try
            {
                // Reading Saved Jobs
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextReader tr = new StreamReader(Path.Combine(Application.StartupPath, "jobs.xml"));
                mJobsList = (List<JobInfo>)serializer.Deserialize(tr);
                tr.Close();

                if (this.userSettings.CurrentlyPauseThreads)
                {
                    pauseCurrentThreads.Text = "(Re)Start Download(s)";
                    pauseCurrentThreads.Image = Languages.english.play;
                }

                File.Delete(Path.Combine(Application.StartupPath, "jobs.xml"));
            }
            catch (Exception)
            {
                mJobsList = new List<JobInfo>();
            }

            if (mJobsList.Count != 0)
            {
                StatusLabelInfo.Text = this._ResourceManager.GetString("gbParse2");
                StatusLabelInfo.ForeColor = Color.Green;

                for (int i = 0; i < mJobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.mJobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem { Text = mJobsList[i].Title };

                    ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }

                this.working = true;

                StatusLabelInfo.Text = string.Empty;
            }

            // Delete Backup Files From AutoUpdate
            if (File.Exists("PG-Ripper.bak"))
            {
                File.Delete("PG-Ripper.bak");
            }

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
                    if (!sRipUrl.StartsWith(this.userSettings.CurrentForumUrl))
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

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, mCurrentJob);

            if (mJobsList.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextWriter tr = new StreamWriter(Path.Combine(Application.StartupPath, "jobs.xml"));
                serializer.Serialize(tr, mJobsList);
                tr.Close();

                // If Pause
                this.userSettings.CurrentlyPauseThreads = true;
                Utility.SaveSettings(this.userSettings);
            }

            if (this.userSettings.SavePids)
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

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, mCurrentJob);

            if (mJobsList.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextWriter tr = new StreamWriter(Path.Combine(Application.StartupPath, "jobs.xml"));
                serializer.Serialize(tr, mJobsList);
                tr.Close();

                // If Pause
                if (this.userSettings.CurrentlyPauseThreads)
                {
                    Utility.SaveSettings(this.userSettings);
                }
            }

            if (this.userSettings.SavePids)
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
            if (pictureBox1.Visible && pictureBox1.Image != null)
            {
                System.Diagnostics.Process.Start(sLastPic);
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
        /// Gets the idxs worker do work.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void GetIdxsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.ThrdGetIndexes(Convert.ToString(e.Argument));
        }

        /// <summary>
        /// Gets the idxs worker completed.
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
                StatusLabelInfo.ForeColor = Color.Green;
                this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", "Analyse Index(es)", po, this.indexedTopicsList.Count);

                this.textBox1.Text = this.indexedTopicsList[po].ImageUrl;

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)this.EnqueueJob);
                }
                else
                {
                    this.EnqueueJob();
                }
                //////////////////////////////////////////////////////////////////////////
            }

            this.indexedTopicsList = null;
        }

        /// <summary>
        /// Genarates the Storage Folder for the Current Job
        /// </summary>
        /// <param name="curJob">Curren Job</param>
        /// <returns>The Storage Folder</returns>
        private string GenerateStorePath(JobInfo curJob)
        {
            string sStorePath = this.userSettings.DownloadFolder;

            if (this.userSettings.SubDirs)
            {
                try
                {
                    if (curJob.ForumTitle != null)
                    {
                        if (this.userSettings.DownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                this.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                this.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title));
                        }
                    }
                    else
                    {
                        if (this.userSettings.DownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                this.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                this.userSettings.DownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
                        }
                    }

                    int iRenameCnt = 2;

                    string sbegining = sStorePath;

                    // Auto Rename if post titles are the same...
                    if (mJobsList.Count != 0)
                    {
                        string path = sStorePath;

                        foreach (JobInfo t in
                            this.mJobsList.Where(
                                t =>
                                t.PostTitle.Equals(curJob.PostTitle) ||
                                Directory.Exists(path) && t.Title.Equals(curJob.Title)))
                        {
                            while (t.StorePath.Equals(sStorePath) || Directory.Exists(sStorePath))
                            {
                                sStorePath = string.Format("{0} Set# {1}", sbegining, iRenameCnt);
                                iRenameCnt++;
                            }
                        }
                    }
                    else
                    {
                        while (Directory.Exists(sStorePath))
                        {
                            sStorePath = string.Format("{0} Set# {1}", sbegining, iRenameCnt);
                            iRenameCnt++;
                        }
                    }
                }
                catch (Exception)
                {
                    sStorePath = Path.Combine(
                        this.userSettings.DownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
                }
            }

            return sStorePath;
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
    }
}