// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
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
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using Microsoft.WindowsAPICodePack.Taskbar;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;
    using Ripper.Services;
    using System.Threading.Tasks;

    /// <summary>
    /// The Main Form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Vars and Properties
        /// <summary>
        /// 
        /// </summary>
        public string sLastPic = string.Empty;

        /// <summary>
        /// Array List of all Stored (Ripped) Post Ids
        /// </summary>
        private readonly ArrayList sRippedPosts = new ArrayList();

        /// <summary>
        /// String List of Urls to Rip
        /// </summary>
        private readonly List<string> ExtractUrls = new List<string>();

        /// <summary>
        /// The indexed topics list
        /// </summary>
        private List<ImageInfo> indexedTopicsList;

        /// <summary>
        /// Gets or sets a value indicating whether if the Browse Dialog is Already Open
        /// </summary>
        private bool IsBrowserOpen { get; set; }

        /// <summary>
        /// Indicates if Ripper Currently Parsing Jobs
        /// </summary>
        private bool bParseAct;

        /// <summary>
        /// Indicates that users Stopping and Deleting Current Job
        /// </summary>
        private bool bStopJob;

        /// <summary>
        /// Indicates if all Functions currently pausing
        /// </summary>
        private bool bCurPause;

        /// <summary>
        /// Indicates that Disc is full and Ripping stoped
        /// </summary>
        private bool bFullDisc;

        /// <summary>
        /// Indicates if Ripper is Currently Ripping
        /// </summary>
        private bool bWorking = true;

        /// <summary>
        /// Indicates if Ripper is Currently Closing the Program
        /// </summary>
        private bool bRipperClosing;

        /// <summary>
        /// The Last Download Folder
        /// </summary>
        private string sLastDownFolder;

        /// <summary>
        /// The Resource Manger that is used for Icons and Labels etc.
        /// </summary>
        private ResourceManager _ResourceManager = new ResourceManager(
            "Ripper.Languages.english", Assembly.GetExecutingAssembly());

        /// <summary>
        /// Indicator if the ripper is currently hidden in the tray
        /// </summary>
        private bool isHiddenInTray;

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

#if (!RIPRIPPERX)

        private TaskbarManager windowsTaskbar;

#endif
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {

#if (!RIPRIPPERX)

            // Tray Menue
            trayMenu = new ContextMenu();

            MenuItem hide = new MenuItem("Hide VG-Ripper", this.HideClick);
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

            mIdleTimer = new System.Windows.Forms.Timer();
            ExitOnIdleTimeout = -1;

            this.SetWindow();
        }

#if (!RIPRIPPERX)
        /// <summary>
        /// The download click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SDownloadClick(object sender, EventArgs e)
        {
            if (Clipboard.GetText().IndexOf(CacheController.Instance().UserSettings.ForumURL) < 0 || this.bParseAct)
            {
                return;
            }

            this.comboBox1.SelectedIndex = 2;

            this.textBox1.Text = Clipboard.GetText();

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
        /// Closes the Ripper
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void ExitClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Hides the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void HideClick(Object sender, EventArgs e)
        {
            this.Hide();
            this.isHiddenInTray = true;

            this.trayMenu.MenuItems.RemoveAt(0);
            MenuItem show = new MenuItem("Show VG-Ripper", this.ShowClick);
            this.trayMenu.MenuItems.Add(0, show);

            this.trayIcon.MouseDoubleClick -= this.HideClick;
            this.trayIcon.MouseDoubleClick += this.ShowClick;

            if (!CacheController.Instance().UserSettings.ShowPopUps)
            {
                return;
            }

            this.trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            this.trayIcon.BalloonTipTitle = "Hidden in Tray";
            this.trayIcon.BalloonTipText = "VG-Ripper is hidden in the Tray";
            this.trayIcon.ShowBalloonTip(10);
        }

        /// <summary>
        /// Shows the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ShowClick(Object sender, EventArgs e)
        {
            this.Show();
            this.isHiddenInTray = false;

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem hide = new MenuItem("Hide VG-Ripper", HideClick);
            trayMenu.MenuItems.Add(0, hide);

            trayIcon.MouseDoubleClick -= ShowClick;
            trayIcon.MouseDoubleClick += HideClick;

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

#if (!RIPRIPPERX)
            var controller = new SingleInstanceController();

            try
            {
                controller.Run(args);
            }
            catch (Exception)
            {

            }

#else

            Application.Run(new MainForm());
#endif
        }

        /// <summary>
        /// Loads All Settings
        /// </summary>
        public void LoadSettings()
        {
            // Load Forum Url
            try
            {
                if (!string.IsNullOrEmpty(SettingsHelper.LoadSetting("ForumURL")))
                {
                    CacheController.Instance().UserSettings.ForumURL = SettingsHelper.LoadSetting("ForumURL");
                }
                else
                {
                    CacheController.Instance().UserSettings.ForumURL = "http://vipergirls.to/";

                    SettingsHelper.SaveSetting("ForumURL", CacheController.Instance().UserSettings.ForumURL);
                }
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ForumURL = "http://vipergirls.to/";

                SettingsHelper.SaveSetting("ForumURL", CacheController.Instance().UserSettings.ForumURL);
            }


            // Load "Offline Modus" Setting
            try
            {
                CacheController.Instance().UserSettings.OfflMode = bool.Parse(SettingsHelper.LoadSetting("OfflineModus"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.OfflMode = false;
            }

            // Load "Firefox Exension Enabled" Setting
            try
            {
                CacheController.Instance().UserSettings.Extension = bool.Parse(SettingsHelper.LoadSetting("FFExtension"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.Extension = false;
            }

            try
            {
                if (!string.IsNullOrEmpty(SettingsHelper.LoadSetting("TxtFolder")))
                {
                    CacheController.Instance().UserSettings.TxtFolder = SettingsHelper.LoadSetting("TxtFolder");
                }
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.TxtFolder = null;
            }

            // Load "Clipboard Watch" Setting
            try
            {
                CacheController.Instance().UserSettings.ClipBWatch = bool.Parse(SettingsHelper.LoadSetting("clipBoardWatch"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ClipBWatch = true;
            }


            try
            {
                CacheController.Instance().UserSettings.ShowPopUps = bool.Parse(SettingsHelper.LoadSetting("Show Popups"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ShowPopUps = true;
            }

            try
            {
                CacheController.Instance().UserSettings.SubDirs = bool.Parse(SettingsHelper.LoadSetting("SubDirs"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.SubDirs = true;
            }

            try
            {
                CacheController.Instance().UserSettings.AutoThank = bool.Parse(SettingsHelper.LoadSetting("Auto TK Button"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.AutoThank = false;
            }

            try
            {
                CacheController.Instance().UserSettings.DownInSepFolder = bool.Parse(SettingsHelper.LoadSetting("DownInSepFolder"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.DownInSepFolder = true;
            }

            try
            {
                CacheController.Instance().UserSettings.SavePids = bool.Parse(SettingsHelper.LoadSetting("SaveRippedPosts"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.SavePids = true;
            }

            try
            {
                CacheController.Instance().UserSettings.ShowCompletePopUp = bool.Parse(SettingsHelper.LoadSetting("Show Downloads Complete PopUp"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ShowCompletePopUp = true;
            }

            // min. Image Count for Thanks
            try
            {
                CacheController.Instance().UserSettings.MinImageCount = !string.IsNullOrEmpty(SettingsHelper.LoadSetting("minImageCountThanks")) ? int.Parse(SettingsHelper.LoadSetting("minImageCountThanks")) : 3;
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.MinImageCount = 3;
            }

            // Max. Threads
            try
            {
                CacheController.Instance().UserSettings.ThreadLimit = -1;

                CacheController.Instance().UserSettings.ThreadLimit = Convert.ToInt32(SettingsHelper.LoadSetting("Thread Limit"));

                ThreadManager.GetInstance().SetThreadThreshHold(CacheController.Instance().UserSettings.ThreadLimit == -1
                                                                    ? 3
                                                                    : CacheController.Instance().UserSettings.ThreadLimit);
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ThreadLimit = 3;
                ThreadManager.GetInstance().SetThreadThreshHold(3);
            }

            try
            {
                CacheController.Instance().UserSettings.DownloadFolder = SettingsHelper.LoadSetting("Download Folder");
                this.DownloadFolder.Text = CacheController.Instance().UserSettings.DownloadFolder;

                if (string.IsNullOrEmpty(CacheController.Instance().UserSettings.DownloadFolder))
                {
                    CacheController.Instance().UserSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    this.DownloadFolder.Text = CacheController.Instance().UserSettings.DownloadFolder;

                    SettingsHelper.SaveSetting("Download Folder", DownloadFolder.Text);
                }
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                DownloadFolder.Text = CacheController.Instance().UserSettings.DownloadFolder;

                SettingsHelper.SaveSetting("Download Folder", DownloadFolder.Text);
                CacheController.Instance().UserSettings.DownloadFolder = DownloadFolder.Text;
            }

            // Load "Download Options"
            try
            {
                CacheController.Instance().UserSettings.DownloadOptions = SettingsHelper.LoadSetting("Download Options");

                switch (CacheController.Instance().UserSettings.DownloadOptions)
                {
                    case "0":
                        comboBox1.SelectedIndex = 0;
                        break;
                    case "1":
                        comboBox1.SelectedIndex = 1;
                        break;
                    case "2":
                        comboBox1.SelectedIndex = 2;
                        break;
                    default:
                        comboBox1.SelectedIndex = 0;
                        break;
                }
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.DownloadOptions = "0";
                comboBox1.SelectedIndex = 0;
            }

            // Load "Always on Top" Setting
            try
            {
                CacheController.Instance().UserSettings.TopMost = bool.Parse(SettingsHelper.LoadSetting("Always OnTop"));
                TopMost = CacheController.Instance().UserSettings.TopMost;
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.TopMost = false;
                TopMost = false;
            }


            // Load Language Setting
            try
            {
                CacheController.Instance().UserSettings.Language = SettingsHelper.LoadSetting("UserLanguage");

                switch (CacheController.Instance().UserSettings.Language)
                {
                    case "de-DE":
                        this._ResourceManager = new ResourceManager("Ripper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        this._ResourceManager = new ResourceManager("Ripper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        this._ResourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    /*case "zh-CN":
                        this._ResourceManager = new ResourceManager("Ripper.Languages.chinese-cn", Assembly.GetExecutingAssembly());
                        break;*/
                    default:
                        this._ResourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                this.AdjustCulture();
            }
            catch (Exception)
            {
                this._ResourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
            }


            // Load "Guest Mode" Setting
            try
            {
                CacheController.Instance().UserSettings.GuestMode = bool.Parse(SettingsHelper.LoadSetting("GuestMode"));

                if (CacheController.Instance().UserSettings.GuestMode)
                {
                    CacheController.Instance().UserSettings.AutoThank = false;
                }
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.GuestMode = false;
            }

            if (CacheController.Instance().UserSettings.GuestMode)
            {
                return;
            }

            // Load "ShowLastDownloaded" Setting
            try
            {
                CacheController.Instance().UserSettings.ShowLastDownloaded = bool.Parse(SettingsHelper.LoadSetting("ShowLastDownloaded"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.ShowLastDownloaded = false;
            }

            // Load "ShowLastDownloaded" Setting
            try
            {
                CacheController.Instance().UserSettings.AfterDownloads = Convert.ToInt32(SettingsHelper.LoadSetting("AfterDownloads"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.AfterDownloads = 0;
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
                CacheController.Instance().UserSettings.User = SettingsHelper.LoadSetting("User");

                // Import old Password
                try
                {
                    string sOldPass =
                        Utility.EncodePassword(SettingsHelper.LoadSetting("Pass")).Replace("-", string.Empty).ToLower();

                    SettingsHelper.SaveSetting("Password", sOldPass);
                    SettingsHelper.DeleteSetting("Pass");

                    CacheController.Instance().UserSettings.Pass = sOldPass;
                }
                catch (Exception)
                {
                    CacheController.Instance().UserSettings.Pass = null;
                }

                CacheController.Instance().UserSettings.Pass = SettingsHelper.LoadSetting("Password");
            }
            catch (Exception)
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (!this.cameThroughCorrectLogin)
                {
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

        /*
        private void ProcessArgs(ICollection<string> args)
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
                    WindowState = FormWindowState.Minimized;
                }

                else if (CheckArg("EXITONIDLE", curArg, out aValue))
                {
                    try
                    {
                        ExitOnIdleTimeout = Convert.ToInt32(aValue) * 60000;
                        mIdleTimer.Interval = ExitOnIdleTimeout;
                        mIdleTimer.Start();
                        mIdleTimer.Tick += OnIdleTimeout;
                        //MessageBox.Show( "TIMEOUT: " + aForm.ExitOnIdleTimeout );
                    }
                    catch (Exception exception)
                    {
                        Utility.SaveOnCrash(exception.Message, exception.StackTrace, null);
                    }
                }
            }
        }

        static bool CheckArg(string aExpected, string aSupplied, out string aSuppliedValue)
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

            return (lSupplied == aExpected || lSupplied == "/" + aExpected || lSupplied == "\\" + aExpected);
        }
        */

        /// <summary>
        /// Mains the form load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MainFormLoad(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionFunction;
            Application.ThreadException += ThreadExceptionFunction;

            LastWorkingTime = DateTime.Now;

            // Create the delegate that invokes methods for the timer.
            //TimerCallback timerDelegate = new TimerCallback( OnIdleTimeout );

            //AutoResetEvent autoEvent = new AutoResetEvent( false );

            mrefTM = ThreadManager.GetInstance();


#if (!RIPRIPPERX)

            string updateNotes;

            if (VersionCheck.UpdateAvailable(Assembly.GetExecutingAssembly(), "VG-Ripper", out updateNotes)
                && File.Exists(Path.Combine(Application.StartupPath, "ICSharpCode.SharpZipLib.dll")))
            {
                var mbUpdate = this._ResourceManager.GetString("mbUpdate");
                var mbUpdate2 = this._ResourceManager.GetString("mbUpdate2");

                DialogResult result = TopMostMessageBox.Show(
                    string.Format("{0}{1}", mbUpdate, updateNotes),
                    mbUpdate2,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result.Equals(DialogResult.Yes))
                {
                    Clipboard.Clear();

                    this.Enabled = false;
                    this.WindowState = FormWindowState.Minimized;

                    this.trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    this.trayIcon.BalloonTipTitle = "Update in Progress";
                    this.trayIcon.BalloonTipText = "Currently upgrading to newest version.";
                    this.trayIcon.ShowBalloonTip(10);

                    AutoUpdater.TryUpdate("VG-Ripper", Assembly.GetExecutingAssembly());
                }
            }

            // Auto Check for Ripper.Services.dll Update
            if (VersionCheck.UpdateAvailable(typeof(Downloader).Assembly, "Ripper.Services", out updateNotes)
                | !File.Exists(Path.Combine(Application.StartupPath, "Ripper.Services.dll")))
            {
                TopMostMessageBox.Show(
                    string.Format(
                        "{0}{1}",
                        this._ResourceManager.GetString("ServicesUpdate"),
                        updateNotes));

                AutoUpdater.TryUpdate("Ripper.Services", Assembly.GetExecutingAssembly());
            }

#endif
            this.LoadSettings();

            if (!CacheController.Instance().UserSettings.OfflMode && !CacheController.Instance().UserSettings.GuestMode)
            {
                this.AutoLogin();

                if (!this.cameThroughCorrectLogin)
                {
                    Application.Exit();
                }
            }

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.LoadHistory();
            }

#if (!RIPRIPPERX)
            if (this.trayIcon.Icon != null)
            {
                this.trayIcon.Visible = true;
            }
#endif

#if (!RIPRIPPERX)
            ProtocolHelper.RegisterRipUrlProtocol();
#endif
        }

        /// <summary>
        /// Starts Ripping
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MStartDownloadBtnClick(object sender, EventArgs e)
        {
            if (this.textBox1.Text.StartsWith("http"))
            {
                this.comboBox1.SelectedIndex = 2;
            }

            if (string.IsNullOrEmpty(this.textBox1.Text) || this.bParseAct)
            {
                return;
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
            if (e.KeyChar != '\r' || this.bParseAct)
            {
                return;
            }

            e.Handled = true;

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
        /// Enqueue Job
        /// </summary>
        private void EnqueueJob()
        {
            if (!this.IsValidJob())
            {
                this.UnlockControls();
                return;
            }

            // Format Url
            var xmlUrl = UrlHandler.GetXmlUrl(textBox1.Text, comboBox1.SelectedIndex);
            var htmlUrl = UrlHandler.GetHtmlUrl(xmlUrl);


            if (string.IsNullOrEmpty(xmlUrl))
            {
                this.UnlockControls();
                return;
            }

            // Check Post is Ripped?!
            if (xmlUrl.Contains("dpver=2&postid=") && CacheController.Instance().UserSettings.SavePids)
            {
                if (this.IsPostAlreadyRipped(xmlUrl.Substring(xmlUrl.IndexOf("&postid=", StringComparison.Ordinal) + 8)))
                {
                    DialogResult result = TopMostMessageBox.Show(this._ResourceManager.GetString("mBAlready"), "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        this.UnlockControls();
                        return;
                    }
                }
            }

            ///////////////////////////////////////////////
            this.LockControls();
            ///////////////////////////////////////////////
            
            if (mIsIndexChk.Checked)
            {
                // Parse Job as Index Thread
                this.EnqueueIndexThread(xmlUrl, htmlUrl);
            }
            else
            {
                if (CacheController.Instance().UserSettings.DownInSepFolder && xmlUrl.Contains("threadid"))
                {
                    this.EnqueueThreadToPost(xmlUrl, htmlUrl);
                }
                else
                {
                    this.EnqueueThreadOrPost(xmlUrl, htmlUrl);
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
                DialogResult result = TopMostMessageBox.Show("Please Set Up Download Folder before starting download", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
        /// <param name="xmlURL">The Url to the Thread/Post</param>
        /// <param name="htmlURL">The HTML URL.</param>
        private void EnqueueIndexThread(string xmlURL, string htmlURL)
        {
#if (!RIPRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
       Environment.OSVersion.Version.Major >= 6 &&
       Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
            }
#endif



            try
            {
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTopicTitleFromHtml(Utility.GetForumPageAsString(htmlURL))))
                {
                    TopMostMessageBox.Show(xmlURL.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
               Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                        this.windowsTaskbar.SetProgressValue(10, 100); ;
                    }
#endif

                    // Unlock Controls
                    this.UnlockControls();

                    return;
                }
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTopicTitleFromHtml(Utility.GetForumPageAsString(htmlURL))))
                {
                    TopMostMessageBox.Show(xmlURL.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
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

            //////////////////////////////////////////////////////////////////////////

            mIsIndexChk.Checked = false;

            GetIdxsWorker.RunWorkerAsync(xmlURL);

            while (GetIdxsWorker.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Parse all Posts of a Thread
        /// </summary>
        /// <param name="xmlURL">The Thread Url</param>
        /// <param name="htmlURL">The HTML URL.</param>
        private void EnqueueThreadToPost(string xmlURL, string htmlURL)
        {
#if (!RIPRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
       Environment.OSVersion.Version.Major >= 6 &&
       Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
            }
#endif

            try
            {
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTopicTitleFromHtml(Utility.GetForumPageAsString(htmlURL))))
                {
                    TopMostMessageBox.Show(xmlURL.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
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
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTopicTitleFromHtml(Utility.GetForumPageAsString(htmlURL))))
                {
                    TopMostMessageBox.Show(xmlURL.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
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

            while (GetPostsWorker.IsBusy)
            {
                Application.DoEvents();
            }

            GetPostsWorker.RunWorkerAsync(xmlURL);
        }

        /// <summary>
        /// Parse a Thread or Post as Single Job
        /// </summary>
        /// <param name="xmlURL">The Thread/Post Url</param>
        private void EnqueueThreadOrPost(string xmlURL, string htmlURL)
        {
            if (this.jobsList.Any(t => t.XMLUrl == xmlURL))
            {
                TopMostMessageBox.Show(this.mAlreadyQueuedMsg, "Info");

                this.UnlockControls();

                return;
            }

            JobInfo job = new JobInfo
                              {
                                  XMLUrl = xmlURL,
                                  HtmlUrl = htmlURL,
                                  XMLPayLoad = Utility.GetForumPageAsString(xmlURL),
                                  HtmlPayLoad = Utility.GetForumPageAsString(htmlURL)
                              };

            job.ImageCount = Maintenance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

            if (job.ImageCount.Equals(0))
            {
                this.UnlockControls();
            }

            job.PostTitle = Maintenance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, job.HtmlUrl);
            job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
            job.TopicTitle = Maintenance.GetInstance().ExtractTopicTitleFromHtml(job.HtmlPayLoad);
            job.TopicTitle = Utility.ReplaceHexWithAscii(job.TopicTitle);
            job.PostTitle = Utility.ReplaceHexWithAscii(job.PostTitle);

            job.PostIds = Maintenance.GetInstance().GetAllPostIds(job.XMLPayLoad);

            job.StorePath = this.GenerateStorePath(job);

            if (string.IsNullOrEmpty(job.TopicTitle))
            {
                TopMostMessageBox.Show(xmlURL.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

                this.UnlockControls();

                return;
            }

            if (CacheController.Instance().UserSettings.AutoThank)
            {
                var token = Utility.GetSecurityToken(new Uri(CacheController.Instance().UserSettings.ForumURL));

                foreach (string postId in job.PostIds)
                {
                    this.ProcessAutoThankYou(postId, job.ImageCount, job.XMLUrl, token);
                }
            }

            JobListAddDelegate newJob = this.JobListAdd;
            this.Invoke(newJob, new object[] { job });

            ///////////////////////////////////////////////
            this.UnlockControls();
            ///////////////////////////////////////////////
        }

        /// <summary>
        /// Pushes the "Thank You" Button for the user.
        /// </summary>
        /// <param name="postId">The post id.</param>
        /// <param name="imageCount">The image count.</param>
        /// <param name="url">The URL.</param>
        /// <param name="token">The token.</param>
        void ProcessAutoThankYou(string postId, int imageCount, string url, string token)
        {
            if (!CacheController.Instance().UserSettings.AutoThank)
            {
                return;
            }

            if (imageCount < CacheController.Instance().UserSettings.MinImageCount)
            {
                return;
            }

            SendThankYouDelegate lSendThankYouDel = SendThankYou;

            string tyURL;

            url = url.Replace("getSTDpost-imgXML.php?dpver=2&postid=", "showpost.php?p=");

            // Complete Thread
            if (postId != null)
            {
                //string sToken = Utility.GetSecurityToken(new Uri(CacheController.Instance().UserSettings.ForumURL + "showpost.php?p=" + sPostId));

                if (string.IsNullOrEmpty(token))
                {
                    return;
                }

                tyURL = string.Format(
                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                    CacheController.Instance().UserSettings.ForumURL,
                    postId,
                    token);

                // SendThankYou(tyURL);
                this.Invoke(lSendThankYouDel, new object[] { tyURL });
            }
            /*else
            {
                // Single Post
                switch (this.comboBox1.SelectedIndex)
                {
                    case 1:
                        {
                            token = Utility.GetSecurityToken(new Uri(url));

                            tyURL =
                                string.Format(
                                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                                    CacheController.Instance().UserSettings.ForumURL,
                                    url.Substring(url.IndexOf("?p=") + 3),
                                    token);

                            //SendThankYou(tyURL);
                            this.Invoke(lSendThankYouDel, new object[] { tyURL });
                        }

                        break;
                    default:
                        var sUrlNew = string.Empty;
                        if (url.Contains("showpost.php?p="))
                        {
                            sUrlNew = url.Replace("showpost.php?p=", "post_thanks.php?do=post_thanks_add&p=");
                        }
                        else if (url.Contains("showthread.php?t="))
                        {
                            sUrlNew = url.Replace("showpost.php?p=", "post_thanks.php?do=post_thanks_add&p=");
                        }

                        tyURL = string.Format("{0}&securitytoken={1}", sUrlNew, Utility.GetSecurityToken(new Uri(url));
                        
                        this.Invoke(lSendThankYouDel, new object[] { tyURL });

                        break;
                }
            }*/
        }

        /// <summary>
        /// This delegate enables asynchronous calls for automatically sending thank yous
        /// </summary>
        /// <param name="aUrl">A URL.</param>
        delegate void SendThankYouDelegate(string aUrl);

        /// <summary>
        /// Sends the thank you.
        /// </summary>
        /// <param name="aUrl">A URL.</param>
        private static void SendThankYou(string aUrl)
        {
            HttpWebResponse lHttpWebResponse = null;
            Stream lHttpWebResponseStream = null;


            HttpWebRequest lHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
            lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            lHttpWebRequest.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
            lHttpWebRequest.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            lHttpWebRequest.KeepAlive = true;
            //lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
            lHttpWebRequest.Referer = CacheController.Instance().UserSettings.ForumURL;
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
        /// TODO : Change to Html Output Xml doesn't include ShowLinks anymore
        /// </summary>
        /// <param name="XMLUrl">The XML URL.</param>
        private void ThreadGetIndexes(string XMLUrl)
        {
            var pagecontent = Utility.GetForumPageAsString(string.Format("{0}&showlinks=1", XMLUrl));

            this.indexedTopicsList = ExtractHelper.ExtractRiPUrls(pagecontent);
        }

        /// <summary>
        /// Get All Post of a Thread and Parse them as new Job
        /// </summary>
        /// <param name="sXMLUrl">
        /// The s Xml Url.
        /// </param>
        private void ThrdGetPosts(string sXMLUrl)
        {
            string sPagecontent = Utility.GetForumPageAsString(sXMLUrl);

            var arlst = ExtractHelper.ExtractThreadtoPosts(sPagecontent);

            string sToken = Utility.GetSecurityToken(new Uri(CacheController.Instance().UserSettings.ForumURL));

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

                string currentPostId = arlst[po].ImageUrl;

                //////////////////////////////////////////////////////////////////////////

                if (CacheController.Instance().UserSettings.SavePids && this.IsPostAlreadyRipped(currentPostId))
                {
                    continue;
                }

                var composedXMLUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                    CacheController.Instance().UserSettings.ForumURL,
                    currentPostId);
                var composedHtmlUrl = string.Format("http://vipergirls.to/showpost.php?p={0}", currentPostId);

                var jobInfo = this.jobsList.Find(doubleJob => doubleJob.XMLUrl.Equals(composedXMLUrl));

                if (jobInfo != null)
                {
                    continue;
                }

                if (this.currentJob != null)
                {
                    if (this.currentJob.XMLUrl.Equals(composedXMLUrl))
                    {
                        continue;
                    }
                }

                JobInfo job = new JobInfo
                                  {
                                      XMLUrl = composedXMLUrl,
                                      HtmlUrl = composedHtmlUrl,
                                      XMLPayLoad = Utility.GetForumPageAsString(composedXMLUrl),
                                      HtmlPayLoad = Utility.GetForumPageAsString(composedHtmlUrl)
                                  };

                job.ImageCount = Maintenance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

                if (job.ImageCount.Equals(0))
                {
                    continue;
                }

                job.PostTitle =
                    Utility.ReplaceHexWithAscii(Maintenance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, job.HtmlUrl));
                job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
                job.TopicTitle =
                    Utility.ReplaceHexWithAscii(Maintenance.GetInstance().ExtractTopicTitleFromHtml(job.HtmlPayLoad));

                job.StorePath = this.GenerateStorePath(job);

                ProcessAutoThankYou(currentPostId, job.ImageCount, job.XMLUrl, sToken);

                //JobListAdd(job);
                JobListAddDelegate newJob = JobListAdd;
                Invoke(newJob, new object[] { job });

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
            ////if (bWorking || mJobsList.Count > 0)
            if (this.bWorking && !this.bParseAct || this.jobsList.Count > 0 && !this.bParseAct)
            {
                this.LogicCode();
            }

            if (this.StatusLabelInfo.Text.Equals(this._ResourceManager.GetString("StatusLabelInfo")) &&
                this.jobsList.Count.Equals(0) &&
                this.currentJob == null &&
                ThreadManager.GetInstance().GetThreadCount().Equals(0))
            {
                this.IdleRipper(true);
            }

            if (!this.bParseAct && this.ExtractUrls.Count > 0 &&
                !this.GetPostsWorker.IsBusy && !this.GetIdxsWorker.IsBusy)
            {
                this.GetExtractUrls();
            }

            this.CheckClipboardData();
        }

        /*
        /// <summary>
        /// Close Ripper on Timer Idle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        private void OnIdleTimeout(object sender, EventArgs eArgs)
        {
            mIdleTimer.Stop();
            Close();
            Application.Exit();
        }
        /// <summary>
        /// Resets The IdleTimer
        /// </summary>
        private void ResetTimer()
        {
            if (ExitOnIdleTimeout > 0)
            {
                mIdleTimer.Stop();
                mIdleTimer.Start();
            }
        }*/

        /// <summary>
        /// Over time, it turned into a nasty C-like code; where everything is
        /// in the same method...
        /// Its kind of an over kill and I need to rewrite this sorry excuse 
        /// for a 'scheduler'...
        /// </summary>
        private void LogicCode()
        {
            // Full HDD solution
            if (Delete && !this.bFullDisc)
            {
                this.FullDisc();
            }

            if (mrefTM.GetThreadCount() > 0)
            {
                // If Joblist empty and the last Threads of Current Job are parsed
                if (currentJob == null && jobsList.Count.Equals(0) && !bParseAct)
                {
                    StatusLabelInfo.Text = _ResourceManager.GetString("StatusLabelInfo");
                    StatusLabelInfo.ForeColor = Color.Red;

                    groupBox5.Text = string.Format("{0} (-):", _ResourceManager.GetString("lblRippingQue"));
                }
            }
            else
            {
                // Check if Last sDownloadFolder is Empty
                if (!string.IsNullOrEmpty(this.sLastDownFolder))
                {
                    CheckCurJobFolder(this.sLastDownFolder);
                }

                if (!bCurPause)
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
#if (!RIPRIPPERX)
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
        ///  Checks the Download Folder of the Current Finished Job, if Empty delete the folder.
        /// </summary>
        /// <param name="sCheckFolder">The Folder to Check</param>
        private static void CheckCurJobFolder(string sCheckFolder)
        {
            if (!Directory.Exists(sCheckFolder))
            {
                return;
            }

            if (Directory.GetFiles(sCheckFolder).Length == 0)
            {
                Directory.Delete(sCheckFolder);
            }
        }

        //delegate void ProcessNextJobDelegate();
        /// <summary>
        /// STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
        /// </summary>
        private void ProcessNextJob()
        {
            bWorking = true;

            deleteJob.Enabled = true;
            stopCurrentThreads.Enabled = true;

            if (jobsList.Count == 0)
            {
                return;
            }

            currentJob = jobsList[0];

            JobListRemove(currentJob, 0);

            string bSystemExtr = _ResourceManager.GetString("bSystemExtr");

            ParseJobXml();

            groupBox2.Text = string.Format("{0}...", _ResourceManager.GetString("gbCurrentlyExtract"));

            if (currentJob.TopicTitle.Equals(currentJob.PostTitle))
            {
                Text = string.Format("{0}: {1} - x{2}", _ResourceManager.GetString("gbCurrentlyExtract"), currentJob.TopicTitle,
                                                mImagesList.Count);
            }
            else
            {
                Text = string.Format("{0}: {1} - {2} - x{3}", _ResourceManager.GetString("gbCurrentlyExtract"),
                                               currentJob.TopicTitle, currentJob.PostTitle, mImagesList.Count);
            }

            lvCurJob.Columns[0].Text = string.Format("{0} - x{1}", currentJob.PostTitle, mImagesList.Count);

#if (!RIPRIPPERX)
            try
            {
                if (CacheController.Instance().UserSettings.ShowPopUps)
                {
                    trayIcon.Text = _ResourceManager.GetString("gbCurrentlyExtract") + currentJob.TopicTitle;
                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    trayIcon.BalloonTipTitle = _ResourceManager.GetString("gbCurrentlyExtract");
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

            //ResetTimer();

            ProcessCurImgLst();
        }

        /// <summary>
        /// Parses the job XML.
        /// </summary>
        private void ParseJobXml()
        {
            this.mImagesList = ExtractHelper.ExtractImages(this.currentJob.XMLPayLoad);
        }

        /// <summary>
        /// Processing the Images list of the Current Job
        /// </summary>
        private void ProcessCurImgLst()
        {
            stopCurrentThreads.Enabled = true;
            this.bStopJob = false;
            this.bWorking = true;

            this.sLastDownFolder = null;

            ThreadManager lTdm = ThreadManager.GetInstance();

            this.sLastDownFolder = this.currentJob.StorePath;

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
                    if (this.bStopJob || this.bRipperClosing)
                    {
                        break;
                    }

#if (!RIPRIPPERX)
                    trayIcon.Text = string.Format(tiImagesRemain, mImagesList.Count - i, i * 100 / mImagesList.Count);

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressValue(i, mImagesList.Count);
                    }
#endif

                    while (!lTdm.IsSystemReadyForNewThread())
                    {
                        Application.DoEvents();
                        Thread.Sleep(100);
                    }

                    if (!this.bRipperClosing)
                    {
                        if (progressBar1 != null)
                        {
                            progressBar1.Value = i;
                        }

                        StatusLabelImageC.Text = string.Format(
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

                        if (!this.bRipperClosing)
                        {
                            this.lvCurJob.Items[i].ForeColor = Color.Green;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (this.bStopJob || this.bRipperClosing)
                        {
                            break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        if (this.bStopJob || this.bRipperClosing)
                        {
                            break;
                        }
                    }

                    //ResetTimer();
                }

                // FINISED A THREAD/POST DOWNLOAD JOB
                currentJob = null;

                if (!string.IsNullOrEmpty(this.sLastDownFolder))
                {
                    CheckCurJobFolder(this.sLastDownFolder);
                }

                if (jobsList.Count > 0)
                {
#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
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
        public void ShowLastPic()
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

            this.sLastPic = CacheController.Instance().LastPic;

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
        /// Idle Mode
        /// </summary>
        /// <param name="endingAllRip">Indicates that Ripper is Finishing the last Threads of all Jobs</param>
        private void IdleRipper(bool endingAllRip = false)
        {
#if (!RIPRIPPERX)

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
               Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                this.windowsTaskbar.SetProgressValue(10, 100);
            }

            string btleExit = this._ResourceManager.GetString("btleExit"),
                   btexExit = this._ResourceManager.GetString("btexExit");

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

                stopCurrentThreads.Enabled = true;

                this.bStopJob = false;

                var ttlHeader = this._ResourceManager.GetString("ttlHeader");
                var guestModeText = this._ResourceManager.GetString("guestModeText");

                this.groupBox2.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
                this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
                StatusLabelInfo.ForeColor = Color.Gray;

                lvCurJob.Columns[0].Text = "  ";

#if (RIPRIPPER)
            this.Text = string.Format(
                "Viper Girls Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                CacheController.Instance().UserSettings.GuestMode
                    ? guestModeText
                    : string.Format("{0}{1}\"]", ttlHeader, CacheController.Instance().UserSettings.User));

            trayIcon.Text = "Right click for context menu";
#elif (RIPRIPPERX)
            this.Text = string.Format(
                "Viper Girls Ripper X {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                CacheController.Instance().UserSettings.GuestMode
                    ? guestModeText
                    : string.Format("{0}{1}\"]", ttlHeader, CacheController.Instance().UserSettings.User));
#else
                this.Text = string.Format(
                    "Viper Girls Ripper {0}.{1}.{2}{3}{4}",
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    CacheController.Instance().UserSettings.GuestMode
                        ? guestModeText
                        : string.Format("{0}{1}\"]", ttlHeader, CacheController.Instance().UserSettings.User));

                this.trayIcon.Text = "Right click for context menu";
#endif

                // Since no picbox image will be visible until another job is queued,
                // reclaim resources used by any previous image in the picturebox
                // This prevents file locking of downloaded images until the process exits
                /*if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }*/

                if (pictureBox1.BackgroundImage != null)
                {
                    pictureBox1.BackgroundImage.Dispose();
                    pictureBox1.BackgroundImage = null;
                }

                if (imgLastPic != null)
                {
                    imgLastPic.Dispose();
                    imgLastPic = null;
                }

                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }

                // Hide any image last displayed in the picturebox
                pictureBox1.Visible = false;

                deleteJob.Enabled = false;
                stopCurrentThreads.Enabled = false;

                //ResetTimer();

                bWorking = false;
            }
        }
        /// <summary>
        /// Adds new Job to JobList and ListView
        /// </summary>
        /// <param name="job">The New Job</param>
        delegate void JobListAddDelegate(JobInfo job);

        /// <summary>
        /// Adds new Job to JobList and ListView
        /// </summary>
        /// <param name="job">The new Job</param>
        private void JobListAdd(JobInfo job)
        {
            bWorking = true;

            jobsList.Add(job);

            ListViewItem ijobJob = new ListViewItem(job.TopicTitle, 0);

            ijobJob.SubItems.Add(job.PostTitle);
            ijobJob.SubItems.Add(job.ImageCount.ToString());

            listViewJobList.Items.AddRange(new[] { ijobJob });

            groupBox5.Text = string.Format("{0} ({1}):", _ResourceManager.GetString("lblRippingQue"), jobsList.Count);
            
            Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);
        }
        /// <summary>
        /// Removes a Job from the Joblist and ListView
        /// </summary>
        /// <param name="jiDelete"></param>
        /// <param name="iJobIndex">The Index of the Job inside the Joblist</param>
        private void JobListRemove(JobInfo jiDelete, int iJobIndex)
        {
            //mJobsList.RemoveAt(iJobIndex);
            jobsList.Remove(jiDelete);

            listViewJobList.Items.RemoveAt(iJobIndex);

            groupBox5.Text = string.Format("{0} ({1}):", _ResourceManager.GetString("lblRippingQue"), jobsList.Count);

            // Save Current Job to queue list -> in case of exception jobs get loaded on startup
            Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);
        }

        /// <summary>
        /// Full Disc Handling
        /// </summary>
        private void FullDisc()
        {
            bWorking = false;
            bFullDisc = true;

            pauseCurrentThreads.Text = "Resume Download(s)";
            ThreadManager.GetInstance().HoldAllThreads();

            StatusLabelInfo.Text = DeleteMessage;
            StatusLabelInfo.ForeColor = Color.Red;

            TopMostMessageBox.Show(string.Format("Please change your download location, then press \"Resume Download\", because {0}", DeleteMessage), "Warning");

            UpdateDownloadFolder();

            //JobListAdd(mCurrentJob);

            JobListAddDelegate newJob = JobListAdd;
            Invoke(newJob, new object[] { currentJob });

            currentJob = null;
            StatusLabelInfo.Text = string.Empty;
            lvCurJob.Items.Clear();

            for (int i = 0; i != jobsList.Count; i++)
            {
                JobInfo updatedJob = new JobInfo
                                         {
                                             ImageCount = jobsList[i].ImageCount,
                                             PostTitle = jobsList[i].PostTitle
                                         };

                //updatedJob.sStorePath = sDownloadFolder;

                if (CacheController.Instance().UserSettings.SubDirs)
                {
                    switch (this.comboBox1.SelectedIndex)
                    {
                        case 0:
                            updatedJob.StorePath = Path.Combine(CacheController.Instance().UserSettings.DownloadFolder, this.jobsList[i].TopicTitle);
                            break;
                        case 2:
                        case 1:
                            updatedJob.StorePath = Path.Combine(CacheController.Instance().UserSettings.DownloadFolder, this.jobsList[i].TopicTitle + Path.DirectorySeparatorChar + this.jobsList[i].PostTitle);
                            break;
                    }
                }


                updatedJob.TopicTitle = jobsList[i].TopicTitle;
                updatedJob.XMLUrl = jobsList[i].XMLUrl;
                updatedJob.XMLPayLoad = jobsList[i].XMLPayLoad;

                JobListRemove(jobsList[i], i);

                jobsList.Insert(i, updatedJob);
            }

            bFullDisc = false;
        }

        /// <summary>
        /// Delete Selected Jobs
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
                // Save Current Job to queue list -> in case of exception jobs get loaded on startup
                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);
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

            this.bStopJob = true;
            stopCurrentThreads.Enabled = false;

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
            // Save Current Job to queue list -> in case of exception jobs get loaded on startup
            Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);
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
                    this.bCurPause = true;
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
                    SettingsHelper.SaveSetting("CurrentlyPauseThreads", "false");
                    this.bCurPause = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    //LoadSettings();
                    //IdleRipper();
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
                    this.bCurPause = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    pauseCurrentThreads.Text = "Pause Download(s)";
                    ThreadManager.GetInstance().ResumeAllThreads();
                    pauseCurrentThreads.Image = Languages.english.pause;
                    break;
            }
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    SettingsHelper.SaveSetting("Download Options", "0");
                    CacheController.Instance().UserSettings.DownloadOptions = "0";
                    break;
                case 1:
                    SettingsHelper.SaveSetting("Download Options", "1");
                    CacheController.Instance().UserSettings.DownloadOptions = "1";
                    break;
                case 2:
                    SettingsHelper.SaveSetting("Download Options", "2");
                    CacheController.Instance().UserSettings.DownloadOptions = "2";
                    break;
            }
        }

        private void MainFormResize(object sender, EventArgs e)
        {
#if (!RIPRIPPERX)
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.isHiddenInTray = true;

                trayIcon.MouseDoubleClick -= (HideClick);
                trayIcon.MouseDoubleClick += (ShowClick);

                trayMenu.MenuItems.RemoveAt(0);
                MenuItem show = new MenuItem("Show VG-Ripper", (ShowClick));
                trayMenu.MenuItems.Add(0, show);

                if (CacheController.Instance().UserSettings.ShowPopUps)
                {
                    trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
                    trayIcon.BalloonTipTitle = "Hidden in Tray";
                    trayIcon.BalloonTipText = "VG-Ripper is hidden in the Tray";
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
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
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

            try
            {
                // Save Current Job to quere List
                if (this.currentJob != null)
                {
                    this.bRipperClosing = true;

                    ThreadManager.GetInstance().DismantleAllThreads();
                    this.jobsList.Add(this.currentJob);

                    this.currentJob = null;
                }

                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);

                // If Pause
                if (this.bCurPause)
                {
                    SettingsHelper.SaveSetting("CurrentlyPauseThreads", "true");
                }
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, this.currentJob);
            }
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

            sRippedPosts.Clear();

            foreach (string sSavedId in sPostIDs.Where(sSavedId => !string.IsNullOrEmpty(sSavedId)))
            {
                sRippedPosts.Add(sSavedId.Contains("&postcount")
                                     ? sSavedId.Remove(sSavedId.IndexOf("&postcount"))
                                     : sSavedId);
            }

            /*foreach (string sSavedId in sPostIDs)
            {
                if (sSavedId != "" && !sRippedPosts.Contains(sSavedId))
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

            foreach (string sSavedId in sRippedPosts)
            {
                sw.Write(sSavedId + ";");
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
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
        /// Helps the tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HelpToolStripMenuItemClick(object sender, EventArgs e)
        {
            About aForm = new About();
            aForm.ShowDialog();
        }

        /// <summary>
        /// Browses the folder BTN click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

            SettingsHelper.SaveSetting("Download Folder", this.DownloadFolder.Text);

            CacheController.Instance().UserSettings.DownloadFolder = this.DownloadFolder.Text;

            this.IsBrowserOpen = false;
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

                // Ignore other input
                if (!clipboardText.StartsWith(CacheController.Instance().UserSettings.ForumURL))
                {
                    return;
                }

                var clipBoardURLTemp = clipboardText;

                Clipboard.Clear();

                var clipBoardUrLs = clipBoardURLTemp.Split(new[] { '\n' });

                foreach (var clipBoardURL in
                    clipBoardUrLs.Where(
                        sClipBoardURL =>
                        sClipBoardURL.StartsWith(CacheController.Instance().UserSettings.ForumURL)
                        || sClipBoardURL.StartsWith(CacheController.Instance().UserSettings.ForumURL)))
                {
                    if (!this.bParseAct)
                    {
                        this.comboBox1.SelectedIndex = 2;

                        this.textBox1.Text = clipBoardURL;

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
                        this.ExtractUrls.Add(clipBoardURL);
                    }
                }
            }
            catch (Exception)
            {
                //Clipboard.Clear();
            }
        }

        /// <summary>
        /// Auto login if User Credentials exists in Config file.
        /// if no Data show Login Form
        /// </summary>
        public void AutoLogin()
        {
            if (CacheController.Instance().UserSettings.User != null && CacheController.Instance().UserSettings.Pass != null)
            {
                LoginManager lgnMgr = new LoginManager(CacheController.Instance().UserSettings.User, CacheController.Instance().UserSettings.Pass);

                if (lgnMgr.DoLogin(CacheController.Instance().UserSettings.ForumURL))
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
            else
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                this.cameThroughCorrectLogin = false;
            }
        }

        /// <summary>
        /// Set Window Position and Size
        /// </summary>
        private void SetWindow()
        {
            try
            {
                CacheController.Instance().UserSettings.WindowLeft = int.Parse(SettingsHelper.LoadSetting("Window left"));
                CacheController.Instance().UserSettings.WindowTop = int.Parse(SettingsHelper.LoadSetting("Window top"));

                if (!CacheController.Instance().UserSettings.WindowLeft.Equals(-32000)
                    && !CacheController.Instance().UserSettings.WindowTop.Equals(-32000))
                {
                    this.Left = CacheController.Instance().UserSettings.WindowLeft;
                    this.Top = CacheController.Instance().UserSettings.WindowTop;
                }
            }
            catch (Exception)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }



            try
            {
                CacheController.Instance().UserSettings.WindowWidth = int.Parse(SettingsHelper.LoadSetting("Window width"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.WindowWidth = 863;
            }

            this.Width = CacheController.Instance().UserSettings.WindowWidth;

            try
            {
                CacheController.Instance().UserSettings.WindowHeight = int.Parse(SettingsHelper.LoadSetting("Window height"));
            }
            catch (Exception)
            {
                CacheController.Instance().UserSettings.WindowHeight = 611;
            }

            this.Height = CacheController.Instance().UserSettings.WindowHeight;
        }

        /// <summary>
        /// Save the History and Window Size & Positioning on Program Closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs" /> instance containing the event data.</param>
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

            if (!CacheController.Instance().UserSettings.WindowLeft.Equals(-32000)
                && !CacheController.Instance().UserSettings.WindowTop.Equals(-32000))
            {
                SettingsHelper.SaveSetting("Window left", this.Left.ToString());
                SettingsHelper.SaveSetting("Window top", this.Top.ToString());
            }

            SettingsHelper.SaveSetting("Window width", this.Width.ToString());
            SettingsHelper.SaveSetting("Window height", this.Height.ToString());
        }

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(EventArgs args)
        {
            try
            {
                base.OnLoad(args);
            }
            catch (Exception)
            {
                // Throws when exiting
            }


            Application.Idle += this.OnLoaded;
        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, EventArgs args)
        {
            Application.Idle -= this.OnLoaded;

            this.tmrPageUpdate.Enabled = true;

#if (!RIPRIPPERX)

            // TODO : Check if Windows 7 API dlls exists
            //if (File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll")) && 
            //   File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll")))
            //{
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.Major >= 6 &&
                Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar = TaskbarManager.Instance;
                // }
            }
#endif
            this.IdleRipper();

            try
            {
                // Reading Saved Jobs
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextReader tr = new StreamReader(Path.Combine(Application.StartupPath, "jobs.xml"));
                this.jobsList = (List<JobInfo>)serializer.Deserialize(tr);
                tr.Close();

                try
                {
                    this.bCurPause = bool.Parse(SettingsHelper.LoadSetting("CurrentlyPauseThreads"));
                }
                catch (Exception)
                {
                    this.bCurPause = false;
                }

                if (this.bCurPause)
                {
                    this.pauseCurrentThreads.Text = "(Re)Start Download(s)";
                    this.pauseCurrentThreads.Image = Languages.english.play;
                }

                File.Delete(Path.Combine(Application.StartupPath, "jobs.xml"));
            }
            catch (Exception)
            {
                this.jobsList = new List<JobInfo>();
            }

            if (this.jobsList.Count != 0)
            {
                this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbParse2");
                this.StatusLabelInfo.ForeColor = Color.Green;

                for (int i = 0; i < this.jobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.jobsList.Count);
                    this.StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(this.jobsList[i].TopicTitle, 0);
                    ijobJob.SubItems.Add(this.jobsList[i].PostTitle);
                    ijobJob.SubItems.Add(this.jobsList[i].ImageCount.ToString());
                    this.listViewJobList.Items.AddRange(new[] { ijobJob });
                }

                this.bWorking = true;

                this.StatusLabelInfo.Text = string.Empty;
            }

            AutoUpdater.CleanupAfterUpdate("RiPRipper");

            // Extract Urls from Text file for Ripping
            CacheController.Instance().UserSettings.TxtFolder =
                string.IsNullOrEmpty(CacheController.Instance().UserSettings.TxtFolder)
                    ? "ExtractUrls.txt"
                    : Path.Combine(CacheController.Instance().UserSettings.TxtFolder, "ExtractUrls.txt");

            if (File.Exists(CacheController.Instance().UserSettings.TxtFolder))
            {
                this.GetTxtUrls(CacheController.Instance().UserSettings.TxtFolder);
            }
            /////////////////
        }

        /// <summary>
        /// Extract The Cached URL's and Rip them.
        /// </summary>
        private void GetExtractUrls()
        {
            if (this.ExtractUrls.Count <= 0 && this.bParseAct || this.bParseAct)
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

            if (this.ExtractUrls.Count > 0)
            {
                this.ExtractUrls.RemoveAt(0);
            }

            if (this.ExtractUrls.Count > 0)
            {
                this.GetExtractUrls();
            }
        }

        /// <summary>
        /// Extract Urls from Text file for Ripping
        /// </summary>
        /// <param name="sTextFolder">Path to the Text File</param>
        private void GetTxtUrls(string sTextFolder)
        {
            if (!File.Exists(sTextFolder))
            {
                return;
            }

            try
            {
                FileStream file = new FileStream(sTextFolder, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(file);
                string srRead = sr.ReadToEnd();
                sr.Close();
                file.Close();

                File.Delete(sTextFolder);

                string[] sRipUrls = srRead.Split(new[] { '\n' });

                foreach (string sRipUrl in
                    sRipUrls.Where(
                        sRipUrl =>
                        sRipUrl.StartsWith(CacheController.Instance().UserSettings.ForumURL) ||
                        sRipUrl.StartsWith(CacheController.Instance().UserSettings.ForumURL)))
                {
                    this.ExtractUrls.Add(sRipUrl);
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
                if (this.bCurPause)
                {
                    SettingsHelper.SaveSetting("CurrentlyPauseThreads", "true");
                }
            }

            if (CacheController.Instance().UserSettings.SavePids)
            {
                this.SaveHistory();
            }

            // Eventlog
            /*if (!EventLog.SourceExists("Ripper"))
            {
                EventLog.CreateEventSource("Ripper", "Application");
            }

            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = "Ripper";
            myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);*/
            /////////////////////
        }

        /// <summary>
        /// Catches All Unhandled Thread Exceptions and creates a Crash Log
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ThreadExceptionEventArgs"/> instance containing the event data.</param>
        void ThreadExceptionFunction(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, this.currentJob);

            if (this.jobsList.Count > 0)
            {
                Utility.ExportCurrentJobsQueue(Path.Combine(Application.StartupPath, "jobs.xml"), this.jobsList);

                // If Pause
                if (this.bCurPause)
                {
                    SettingsHelper.SaveSetting("CurrentlyPauseThreads", "true");
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1DoubleClick(object sender, EventArgs e)
        {
            if (pictureBox1.Visible && pictureBox1.Image != null)
            {
                System.Diagnostics.Process.Start(sLastPic);
            }
        }
        private void GetPostsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.ThrdGetPosts(Convert.ToString(e.Argument));
        }

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
            this.ThreadGetIndexes(Convert.ToString(e.Argument));
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
                this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", "Analyse Index(es)", po, indexedTopicsList.Count);

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
                            t.PostTitle.Equals(job.PostTitle) || (Directory.Exists(path) && t.TopicTitle.Equals(job.TopicTitle))))
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
            this.bParseAct = false;

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)this.UnlockControlsElements);
            }
            else
            {
                this.UnlockControlsElements();
            }
        }

        /// <summary>
        /// Unlock (Enable) Elements after Parsing
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
            this.bParseAct = true;

            textBox1.Enabled = false;
            mStartDownloadBtn.Enabled = true;

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
            SettingsHelper.SaveSetting("ShowLastDownloaded", this.showLastImageToolStripMenuItem.Checked.ToString());

            this.groupBox4.Visible = this.showLastImageToolStripMenuItem.Checked;

            CacheController.Instance().UserSettings.ShowLastDownloaded = this.showLastImageToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Enable/Disable Clipboard Monitoring
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UseCliboardMonitoringToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SettingsHelper.SaveSetting("clipBoardWatch", this.useCliboardMonitoringToolStripMenuItem.Checked.ToString());

            CacheController.Instance().UserSettings.ClipBWatch = this.useCliboardMonitoringToolStripMenuItem.Checked;
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

            SettingsHelper.SaveSetting("AfterDownloads", CacheController.Instance().UserSettings.AfterDownloads.ToString());
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

            SettingsHelper.SaveSetting("AfterDownloads", CacheController.Instance().UserSettings.AfterDownloads.ToString());
        }

        /// <summary>
        /// Handles the event when the download folder is changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DownloadFolder_TextChanged(object sender, EventArgs e)
        {
            /* SettingsHelper.SaveSetting("Download Folder", this.DownloadFolder.Text);

             CacheController.Instance().UserSettings.DownloadFolder = this.DownloadFolder.Text;*/
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