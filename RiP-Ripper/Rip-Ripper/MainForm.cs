// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
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
    using Microsoft.WindowsAPICodePack.Shell;
    using Microsoft.WindowsAPICodePack.Taskbar;

    using RiPRipper.Objects;

    /// <summary>
    /// The Main Form.
    /// </summary>
    public partial class MainForm : Form
    {
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
        /// Indicates if the Browse Dialog is Already Opten
        /// </summary>
        private bool bIsBrowserOpen;

        /// <summary>
        /// Indicates if Ripper Currently Parsing Jobs
        /// </summary>
        private bool bParseAct;

        /// <summary>
        /// Indicates that Ripper is Finishing the last Threads
        /// </summary>
        private bool bEndRip;

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
        private ResourceManager _ResourceManager;

       

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
        private JumpList jumpList;

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.cacheController = CacheController.GetInstance();

#if (!RIPRIPPERX)

            // Tray Menue
            trayMenu = new ContextMenu();

            MenuItem hide = new MenuItem("Hide RiP-Ripper", this.HideClick);
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
            if (Clipboard.GetText().IndexOf(this.cacheController.userSettings.ForumURL) < 0 || this.bParseAct)
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
        /// Exits the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ExitClick(Object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Hides the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void HideClick(Object sender, EventArgs e)
        {
            Hide();

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem show = new MenuItem("Show RiP-Ripper", this.ShowClick);
            trayMenu.MenuItems.Add(0, show);

            this.trayIcon.MouseDoubleClick -= this.HideClick;
            this.trayIcon.MouseDoubleClick += this.ShowClick;

            if (!this.cacheController.userSettings.ShowPopUps) return;

            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.BalloonTipTitle = "Hidden in Tray";
            trayIcon.BalloonTipText = "RiP-Ripper is hidden in the Tray";
            trayIcon.ShowBalloonTip(10);
        }

        /// <summary>
        /// Shows the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ShowClick(Object sender, EventArgs e)
        {
            Show();

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem hide = new MenuItem("Hide RiP-Ripper", HideClick);
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

            controller.Run(args);
#else

            Application.Run(new MainForm());
#endif
        }

        /// <summary>
        /// Loads All Settings
        /// </summary>
        public void LoadSettings()
        {
            // Reading Settings
            
            // Load "Offline Modus" Setting
            try
            {
                this.cacheController.userSettings.OfflMode = bool.Parse(Utility.LoadSetting("OfflineModus"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.OfflMode = false;
            }

            // Load "Firefox Exension Enabled" Setting
            try
            {
                this.cacheController.userSettings.Extension = bool.Parse(Utility.LoadSetting("FFExtension"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.Extension = false;
            }

            try
            {
                if (!string.IsNullOrEmpty(Utility.LoadSetting("TxtFolder")))
                {
                    this.cacheController.userSettings.TxtFolder = Utility.LoadSetting("TxtFolder");
                }
            }
            catch (Exception)
            {
                this.cacheController.userSettings.TxtFolder = null;
            }

            // Load "Clipboard Watch" Setting
            try
            {
                this.cacheController.userSettings.ClipBWatch = bool.Parse(Utility.LoadSetting("clipBoardWatch"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.ClipBWatch = true;
            }


            try
            {
                this.cacheController.userSettings.ShowPopUps = bool.Parse(Utility.LoadSetting("Show Popups"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.ShowPopUps = true;
            }

            try
            {
                this.cacheController.userSettings.SubDirs = bool.Parse(Utility.LoadSetting("SubDirs"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.SubDirs = true;
            }

            try
            {
                this.cacheController.userSettings.AutoThank = bool.Parse(Utility.LoadSetting("Auto TK Button"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.AutoThank = false;
            }

            try
            {
                this.cacheController.userSettings.DownInSepFolder = bool.Parse(Utility.LoadSetting("DownInSepFolder"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.DownInSepFolder = true;
            }

            try
            {
                this.cacheController.userSettings.SavePids = bool.Parse(Utility.LoadSetting("SaveRippedPosts"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.SavePids = true;
            }

            try
            {
                this.cacheController.userSettings.ShowCompletePopUp = bool.Parse(Utility.LoadSetting("Show Downloads Complete PopUp"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.ShowCompletePopUp = true;
            }

            // min. Image Count for Thanks
            try
            {
                this.cacheController.userSettings.MinImageCount = !string.IsNullOrEmpty(Utility.LoadSetting("minImageCountThanks")) ? int.Parse(Utility.LoadSetting("minImageCountThanks")) : 3;
            }
            catch (Exception)
            {
                this.cacheController.userSettings.MinImageCount = 3;
            }

            // Max. Threads
            try
            {
                this.cacheController.userSettings.ThreadLimit = -1;

                this.cacheController.userSettings.ThreadLimit = Convert.ToInt32(Utility.LoadSetting("Thread Limit"));

                ThreadManager.GetInstance().SetThreadThreshHold(this.cacheController.userSettings.ThreadLimit == -1
                                                                    ? 3
                                                                    : this.cacheController.userSettings.ThreadLimit);
            }
            catch (Exception)
            {
                this.cacheController.userSettings.ThreadLimit = 3;
                ThreadManager.GetInstance().SetThreadThreshHold(3);
            }

            try
            {
                this.cacheController.userSettings.DownloadFolder = Utility.LoadSetting("Download Folder");
                this.textBox2.Text = this.cacheController.userSettings.DownloadFolder;

                if (string.IsNullOrEmpty(this.cacheController.userSettings.DownloadFolder))
                {
                    this.cacheController.userSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    this.textBox2.Text = this.cacheController.userSettings.DownloadFolder;

                    Utility.SaveSetting("Download Folder", textBox2.Text);
                }
            }
            catch (Exception)
            {
                this.cacheController.userSettings.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                textBox2.Text = this.cacheController.userSettings.DownloadFolder;

                Utility.SaveSetting("Download Folder", textBox2.Text);
                this.cacheController.userSettings.DownloadFolder = textBox2.Text;
            }

            // Load "Download Options"
            try
            {
                this.cacheController.userSettings.DownloadOptions = Utility.LoadSetting("Download Options");

                switch (this.cacheController.userSettings.DownloadOptions)
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
                this.cacheController.userSettings.DownloadOptions = "0";
                comboBox1.SelectedIndex = 0;
            }

            // Load "Always on Top" Setting
            try
            {
                this.cacheController.userSettings.TopMost = bool.Parse(Utility.LoadSetting("Always OnTop"));
                TopMost = this.cacheController.userSettings.TopMost;
            }
            catch (Exception)
            {
                this.cacheController.userSettings.TopMost = false;
                TopMost = false;
            }


            // Load Language Setting
            try
            {
                this.cacheController.userSettings.Language = Utility.LoadSetting("UserLanguage");

                switch (this.cacheController.userSettings.Language)
                {
                    case "de-DE":
                        this._ResourceManager = new ResourceManager("RiPRipper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        this._ResourceManager = new ResourceManager("RiPRipper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        this._ResourceManager = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    default:
                        this._ResourceManager = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                this.AdjustCulture();
            }
            catch (Exception)
            {
                this._ResourceManager = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
            }


            // Load "Guest Mode" Setting
            try
            {
                this.cacheController.userSettings.GuestMode = bool.Parse(Utility.LoadSetting("GuestMode"));

                if (this.cacheController.userSettings.GuestMode)
                {
                    this.cacheController.userSettings.AutoThank = false;
                }
            }
            catch (Exception)
            {
                this.cacheController.userSettings.GuestMode = false;
            }

            if (this.cacheController.userSettings.GuestMode)
            {
                return;
            }

            try
            {
                this.cacheController.userSettings.User = Utility.LoadSetting("User");

                // Import old Password
                try
                {
                    string sOldPass =
                        Utility.EncodePassword(Utility.LoadSetting("Pass")).Replace("-", string.Empty).ToLower();

                    Utility.SaveSetting("Password", sOldPass);
                    Utility.DeleteSetting("Pass");

                    this.cacheController.userSettings.Pass = sOldPass;
                }
                catch (Exception)
                {
                    this.cacheController.userSettings.Pass = null;
                }

                this.cacheController.userSettings.Pass = Utility.LoadSetting("Password");
            }
            catch (Exception)
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (!this.bCameThroughCorrectLogin)
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
            this.settingsToolStripMenuItem1.Text = this._ResourceManager.GetString("MenuSettings");
            this.exitToolStripMenuItem.Text = this._ResourceManager.GetString("MenuExit");

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

        private void MainFormLoad(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionFunction);
            Application.ThreadException += (ThreadExceptionFunction);

            LastWorkingTime = DateTime.Now;

            // Create the delegate that invokes methods for the timer.
            //TimerCallback timerDelegate = new TimerCallback( OnIdleTimeout );

            //AutoResetEvent autoEvent = new AutoResetEvent( false );

            mrefTM = ThreadManager.GetInstance();

#if (!RIPRIPPERX)


            if (VersionCheck.UpdateAvailable() && File.Exists(Application.StartupPath + "\\ICSharpCode.SharpZipLib.dll"))
            {
                string mbUpdate = _ResourceManager.GetString("mbUpdate"), mbUpdate2 = _ResourceManager.GetString("mbUpdate2");
                DialogResult result = TopMostMessageBox.Show(mbUpdate, mbUpdate2, MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Question);

                if (result.Equals(DialogResult.Yes))
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

            this.LoadSettings();

            if (!this.cacheController.userSettings.OfflMode && !this.cacheController.userSettings.GuestMode)
            {
                this.AutoLogin();

                if (!this.bCameThroughCorrectLogin)
                {
                    Application.Exit();
                }
            }

            //LoadSettings();

            if (this.cacheController.userSettings.SavePids)
            {
                this.LoadHistory();
            }

#if (!RIPRIPPERX)
            this.trayIcon.Visible = true;
#endif

#if (!RIPRIPPERX)
            Utility.RegisterRipUrlProtocol();
#endif
            if (!this.cacheController.userSettings.Extension)
            {
                return;
            }


            this.NCProcessorPort = Convert.ToInt32(36969);
            this.StartNcProcessor();
        }

        // Getting Input From FireFox Extension
        public void ProcessCommand(string aCommand)
        {
            int lSplitIndex = aCommand.IndexOf("<**>");

            int lSplitIndex2 = aCommand.IndexOf("<***>");

            if (lSplitIndex == -1 || aCommand.Length < lSplitIndex + 4)
            {
                Console.WriteLine("UNRECOGNIZED COMMAND: " + aCommand);
                return;
            }

            string lUrl = aCommand.Substring(0, lSplitIndex);
            string lJobPath = aCommand.Substring(lSplitIndex + 4);
            string lAutoFolder = aCommand.Substring(lSplitIndex2 + 5);

            if (lAutoFolder == "1")
            {
                lJobPath = lJobPath.Remove(lJobPath.Length - 6, 6);
            }

            SetFormControls(lUrl, lJobPath, lAutoFolder);
        }

        // This delegate enables asynchronous calls for changing properties of controls on this form
        delegate void SetFormControlsCallback(string aUrl, string aJobPath, string aAutoFolder);

        void SetFormControls
        (string aUrl, string aJobPath, string aAutoFolder)
        {
            if (textBox1.InvokeRequired)
            {
                SetFormControlsCallback d = this.SetFormControls;
                Invoke(d, new object[] { aUrl, aJobPath, aAutoFolder });
            }
            else
            {
                textBox1.Text = aUrl;
                this.cacheController.userSettings.DownloadFolder = aJobPath;
                comboBox1.SelectedIndex = 2;

                if (aAutoFolder == "1")
                {
                    this.cacheController.userSettings.SubDirs = true;
                    this.cacheController.userSettings.DownInSepFolder = true;
                }
                else
                {
                    this.cacheController.userSettings.SubDirs = false;
                    this.cacheController.userSettings.DownInSepFolder = false;
                }

                if (!this.bParseAct)
                {
                    if (InvokeRequired)
                    {
                        Invoke((MethodInvoker)this.EnqueueJob);
                    }
                    else
                    {
                        this.EnqueueJob();
                    } 
                }
            }
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
            var sXmlUrl = UrlHandler.GetXmlUrl(textBox1.Text, comboBox1.SelectedIndex);

            if (string.IsNullOrEmpty(sXmlUrl))
            {
                this.UnlockControls();
                return;
            }

            // Check Post is Ripped?!
            if (sXmlUrl.Contains("dpver=2&postid=") && this.cacheController.userSettings.SavePids)
            {
                if (this.IsPostAlreadyRipped(sXmlUrl.Substring(sXmlUrl.IndexOf("&postid=") + 8)))
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
                this.EnqueueIndexThread(sXmlUrl);
            }
            else
            {
                if (this.cacheController.userSettings.DownInSepFolder && sXmlUrl.Contains("threadid"))
                {
                    this.EnqueueThreadToPost(sXmlUrl);
                }
                else
                {
                    this.EnqueueThreadOrPost(sXmlUrl);
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

            if (string.IsNullOrEmpty(this.cacheController.userSettings.DownloadFolder))
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
        /// <param name="sXmlUrl">The Url to the Thread/Post</param>
        private void EnqueueIndexThread(string sXmlUrl)
        {
#if (!RIPRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
       Environment.OSVersion.Version.Major >= 6 &&
       Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
            }
#endif



            try
            {
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTitleFromXML(Utility.DownloadRipPage(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
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
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTitleFromXML(Utility.DownloadRipPage(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
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
          
            //////////////////////////////////////////////////////////////////////////

            mIsIndexChk.Checked = false;

            GetIdxsWorker.RunWorkerAsync(sXmlUrl);

            while (GetIdxsWorker.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Parse all Posts of a Thread
        /// </summary>
        /// <param name="sXmlUrl">The Thread Url</param>
        private void EnqueueThreadToPost(string sXmlUrl)
        {
#if (!RIPRIPPERX)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
       Environment.OSVersion.Version.Major >= 6 &&
       Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
            }
#endif

            try
            {
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTitleFromXML(Utility.DownloadRipPage(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
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
                if (string.IsNullOrEmpty(Maintenance.GetInstance().ExtractTitleFromXML(Utility.DownloadRipPage(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
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

            while (GetPostsWorker.IsBusy)
            {
                Application.DoEvents();
            }

            GetPostsWorker.RunWorkerAsync(sXmlUrl);
        }

        /// <summary>
        /// Parse a Thread or Post as Single Job
        /// </summary>
        /// <param name="sXmlUrl">The Thread/Post Url</param>
        private void EnqueueThreadOrPost(string sXmlUrl)
        {
            if (mJobsList.Any(t => t.URL == sXmlUrl))
            {
                TopMostMessageBox.Show(mAlreadyQueuedMsg, "Info");

                this.UnlockControls();

                return;
            }

            JobInfo job = new JobInfo { URL = sXmlUrl, XMLPayLoad = Utility.DownloadRipPage(sXmlUrl) };

            job.ImageCount = Maintenance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

            if (job.ImageCount.Equals(0))
            {
                this.UnlockControls();
            }

            job.PostTitle = Maintenance.GetInstance().ExtractPostTitleFromXML(job.XMLPayLoad);
            job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
            job.Title = Maintenance.GetInstance().ExtractTitleFromXML(job.XMLPayLoad);
            job.Title = Utility.ReplaceHexWithAscii(job.Title);
            job.PostTitle = Utility.ReplaceHexWithAscii(job.PostTitle);

            job.PostIds = Maintenance.GetInstance().GetAllPostIds(job.XMLPayLoad);

            job.StorePath = this.GenerateStorePath(job);

            if (string.IsNullOrEmpty(job.Title))
            {
                TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? this.mNoThreadMsg : this.mNoPostMsg, "Info");

                this.UnlockControls();

                return;
            }

            if (this.cacheController.userSettings.AutoThank)
            {
                var token = Utility.GetSToken(this.cacheController.userSettings.ForumURL);

                foreach (string postId in job.PostIds)
                {
                    this.ProcessAutoThankYou(postId, job.ImageCount, job.URL, token);
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
            if (!this.cacheController.userSettings.AutoThank)
            {
                return;
            }

            if (imageCount < this.cacheController.userSettings.MinImageCount)
            {
                return;
            }

            SendThankYouDelegate lSendThankYouDel = SendThankYou;

            string tyURL;

            url = url.Replace("getSTDpost-imgXML.php?dpver=2&postid=", "showpost.php?p=");

            // Complete Thread
            if (postId != null)
            {
                //string sToken = Utility.GetSToken(this.cacheController.userSettings.ForumURL + "showpost.php?p=" + sPostId);

                if (string.IsNullOrEmpty(token))
                {
                    return;
                }

                tyURL = string.Format(
                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                    this.cacheController.userSettings.ForumURL,
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
                            token = Utility.GetSToken(url);

                            tyURL =
                                string.Format(
                                    "{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}",
                                    this.cacheController.userSettings.ForumURL,
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

                        tyURL = string.Format("{0}&securitytoken={1}", sUrlNew, Utility.GetSToken(url));
                        
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
            lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
            lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
            lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            lHttpWebRequest.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
            lHttpWebRequest.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            lHttpWebRequest.KeepAlive = true;
            //lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
            lHttpWebRequest.Referer = CacheController.GetInstance().userSettings.ForumURL;
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
        /// TODO : Change to Html Output Xml doesnt include ShowLinks anymore
        /// </summary>
        private void ThrdGetIndexes(string sXmlUrl)
        {
            Indexes idxs = new Indexes();

            string sPagecontent = Utility.DownloadRipPage(string.Format("{0}&showlinks=1", sXmlUrl));

            arlstIndxs = idxs.ParseXML(sPagecontent);
        }

        private List<ImageInfo> arlstIndxs;

        /// <summary>
        /// Get All Post of a Thread and Parse them as new Job
        /// </summary>
        /// <param name="sXmlUrl">
        /// The s Xml Url.
        /// </param>
        private void ThrdGetPosts(string sXmlUrl)
        {
            var threadPosts = new ThreadToPost();

            string sPagecontent = Utility.DownloadRipPage(sXmlUrl);

            List<ImageInfo> arlst = threadPosts.ParseXML(sPagecontent);

            string sToken = Utility.GetSToken(this.cacheController.userSettings.ForumURL);

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

                if (this.cacheController.userSettings.SavePids && this.IsPostAlreadyRipped(sLpostId))
                {
                    continue;
                }

                var composedURL = string.Format("{0}getSTDpost-imgXML.php?dpver=2&postid={1}", this.cacheController.userSettings.ForumURL, sLpostId);


                var jobInfo = this.mJobsList.Find(doubleJob => doubleJob.URL.Equals(composedURL));

                if (jobInfo != null)
                {
                    continue;
                }

                if (mCurrentJob != null)
                {
                    if (mCurrentJob.URL.Equals(composedURL))
                    {
                        continue;
                    }
                }

                JobInfo job = new JobInfo
                                  {
                                      URL = composedURL,
                                      XMLPayLoad = Utility.DownloadRipPage(composedURL)
                                  };

                job.ImageCount = Maintenance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

                if (job.ImageCount.Equals(0))
                {
                    continue;
                }

                job.PostTitle = Maintenance.GetInstance().ExtractPostTitleFromXML(job.XMLPayLoad);
                job.ForumTitle = Maintenance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
                job.Title = Maintenance.GetInstance().ExtractTitleFromXML(job.XMLPayLoad);
                

                job.Title = Utility.ReplaceHexWithAscii(job.Title);
                job.PostTitle = Utility.ReplaceHexWithAscii(job.PostTitle);

                job.StorePath = this.GenerateStorePath(job);

                ProcessAutoThankYou(sLpostId, job.ImageCount, job.URL, sToken);

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
            if (this.bWorking && !this.bParseAct || this.mJobsList.Count > 0 && !this.bParseAct)
            {
                this.LogicCode();
            }

            if (this.StatusLabelInfo.Text.Equals(this._ResourceManager.GetString("StatusLabelInfo")) &&
                this.mJobsList.Count.Equals(0) &&
                this.mCurrentJob == null)
            {
                this.IdleRipper();
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
            if (Delete && !bFullDisc)
            {
                FullDisc();
            }

            if (mrefTM.GetThreadCount() > 0)
            {
                // If Joblist empty and the last Threads of Current Job are parsed
                if (mCurrentJob == null && mJobsList.Count.Equals(0) && !bParseAct)
                {
                    bEndRip = true;

                    StatusLabelInfo.Text = _ResourceManager.GetString("StatusLabelInfo");
                    StatusLabelInfo.ForeColor = Color.Red;

                    groupBox5.Text = string.Format("{0} (-):", _ResourceManager.GetString("lblRippingQue"));
                }
                else
                {
                    bEndRip = false;
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

                    if (mCurrentJob == null && mJobsList.Count > 0)
                    {
#if (!RIPRIPPERX)
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                   Environment.OSVersion.Version.Major >= 6 &&
                   Environment.OSVersion.Version.Minor >= 1)
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

            if (mJobsList.Count == 0)
            {
                return;
            }

            mCurrentJob = mJobsList[0];

            JobListRemove(mCurrentJob, 0);

            string bSystemExtr = _ResourceManager.GetString("bSystemExtr");

            ParseJobXml();

            groupBox2.Text = string.Format("{0}...", _ResourceManager.GetString("gbCurrentlyExtract"));

            if (mCurrentJob.Title.Equals(mCurrentJob.PostTitle))
            {
                Text = string.Format("{0}: {1} - x{2}", _ResourceManager.GetString("gbCurrentlyExtract"), mCurrentJob.Title,
                                                mImagesList.Count);
            }
            else
            {
                Text = string.Format("{0}: {1} - {2} - x{3}", _ResourceManager.GetString("gbCurrentlyExtract"),
                                               mCurrentJob.Title, mCurrentJob.PostTitle, mImagesList.Count);
            }

            lvCurJob.Columns[0].Text = string.Format("{0} - x{1}", mCurrentJob.PostTitle, mImagesList.Count);

#if (!RIPRIPPERX)
            try
            {
                if (this.cacheController.userSettings.ShowPopUps)
                {
                    trayIcon.Text = _ResourceManager.GetString("gbCurrentlyExtract") + mCurrentJob.Title;
                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    trayIcon.BalloonTipTitle = _ResourceManager.GetString("gbCurrentlyExtract");
                    trayIcon.BalloonTipText = mCurrentJob.Title;
                    trayIcon.ShowBalloonTip(10);
                }
            }
            catch (Exception)
            {
                if (this.cacheController.userSettings.ShowPopUps)
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

        private void ParseJobXml()
        {
            mImagesList = Utility.ExtractImages(mCurrentJob.XMLPayLoad);
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

            this.sLastDownFolder = this.mCurrentJob.StorePath;

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

                            CacheController.GetInstance()
                                           .DownloadImage(
                                               this.mImagesList[i].ImageUrl,
                                               this.mImagesList[i].ThumbnailUrl,
                                               this.mCurrentJob.StorePath,
                                               !string.IsNullOrEmpty(this.mCurrentJob.PostTitle)
                                                   ? this.mCurrentJob.PostTitle
                                                   : this.mCurrentJob.Title);
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
                mCurrentJob = null;

                if (!string.IsNullOrEmpty(this.sLastDownFolder))
                {
                    CheckCurJobFolder(this.sLastDownFolder);
                }

                if (mJobsList.Count > 0)
                {
#if (!RIPRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
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
        public void ShowLastPic()
        {
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

            this.sLastPic = this.cacheController.LastPic;

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
        private void IdleRipper()
        {
#if (!RIPRIPPERX)

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
               Environment.OSVersion.Version.Minor >= 1)
            {
                this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal);
                this.windowsTaskbar.SetProgressValue(10, 100);
                this.windowsTaskbar.SetOverlayIcon(Languages.english.Sleep, "Sleep");
            }

            string btleExit = this._ResourceManager.GetString("btleExit"),
                   btexExit = this._ResourceManager.GetString("btexExit");

            if (this.bEndRip && this.cacheController.userSettings.ShowCompletePopUp)
            {
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.BalloonTipTitle = btleExit;
                trayIcon.BalloonTipText = btexExit;
                trayIcon.ShowBalloonTip(10);
            }
#endif
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

            stopCurrentThreads.Enabled = true;
            this.bStopJob = false;
            this.bEndRip = false;

            var ttlHeader = this._ResourceManager.GetString("ttlHeader");
            var guestModeText = this._ResourceManager.GetString("guestModeText");

            this.groupBox2.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
            this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbCurrentlyIdle");
            StatusLabelInfo.ForeColor = Color.Gray;

            lvCurJob.Columns[0].Text = "  ";

#if (RIPRIPPER)
            this.Text = string.Format(
                "RiP-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                this.cacheController.userSettings.GuestMode
                    ? guestModeText
                    : string.Format("{0}{1}\"]", ttlHeader, this.cacheController.userSettings.User));

            trayIcon.Text = "Right click for context menu";
#elif (RIPRIPPERX)
            this.Text = string.Format(
                "RiP-Ripper X {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                this.cacheController.userSettings.GuestMode
                    ? guestModeText
                    : string.Format("{0}{1}\"]", ttlHeader, this.cacheController.userSettings.User));
#else
            this.Text = string.Format(
                "RiP-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                this.cacheController.userSettings.GuestMode
                    ? guestModeText
                    : string.Format("{0}{1}\"]", ttlHeader, this.cacheController.userSettings.User));

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

            mJobsList.Add(job);

            ListViewItem ijobJob = new ListViewItem(job.Title, 0);

            ijobJob.SubItems.Add(job.PostTitle);
            ijobJob.SubItems.Add(job.ImageCount.ToString());

            listViewJobList.Items.AddRange(new[] { ijobJob });

            groupBox5.Text = string.Format("{0} ({1}):", _ResourceManager.GetString("lblRippingQue"), mJobsList.Count);
        }
        /// <summary>
        /// Removes a Job from the Joblist and ListView
        /// </summary>
        /// <param name="jiDelete"></param>
        /// <param name="iJobIndex">The Index of the Job inside the Joblist</param>
        private void JobListRemove(JobInfo jiDelete, int iJobIndex)
        {
            //mJobsList.RemoveAt(iJobIndex);
            mJobsList.Remove(jiDelete);

            listViewJobList.Items.RemoveAt(iJobIndex);

            groupBox5.Text = string.Format("{0} ({1}):", _ResourceManager.GetString("lblRippingQue"), mJobsList.Count);
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
            Invoke(newJob, new object[] { mCurrentJob });

            mCurrentJob = null;
            StatusLabelInfo.Text = string.Empty;
            lvCurJob.Items.Clear();

            for (int i = 0; i != mJobsList.Count; i++)
            {
                JobInfo updatedJob = new JobInfo
                                         {
                                             ImageCount = mJobsList[i].ImageCount,
                                             PostTitle = mJobsList[i].PostTitle
                                         };

                //updatedJob.sStorePath = sDownloadFolder;

                if (this.cacheController.userSettings.SubDirs)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        updatedJob.StorePath = Path.Combine(this.cacheController.userSettings.DownloadFolder, mJobsList[i].Title);
                    }
                    if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                    {
                        updatedJob.StorePath = Path.Combine(this.cacheController.userSettings.DownloadFolder, mJobsList[i].Title + Path.DirectorySeparatorChar + mJobsList[i].PostTitle);
                    }
                }


                updatedJob.Title = mJobsList[i].Title;
                updatedJob.URL = mJobsList[i].URL;
                updatedJob.XMLPayLoad = mJobsList[i].XMLPayLoad;

                JobListRemove(mJobsList[i], i);

                mJobsList.Insert(i, updatedJob);
            }

            bFullDisc = false;
        }
        /// <summary>
        /// Delete Selected Jobs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    StatusLabelInfo.Text = string.Format("{0}{1}/{2}", _ResourceManager.GetString("gbParse2"), i, mJobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(mJobsList[i].Title, 0);
                    ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }
            }
            finally
            {
                groupBox5.Text = string.Format("{0} ({1}):", _ResourceManager.GetString("lblRippingQue"), mJobsList.Count);
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

            this.bStopJob = true;
            stopCurrentThreads.Enabled = false;

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
                    Utility.SaveSetting("CurrentlyPauseThreads", "false");
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

        /// <summary>
        /// Starts the net command processor.
        /// </summary>
        private void StartNcProcessor()
        {
            this.mNCProcessor = new NetCommandProcessor();
            this.mNCProcessor.StartListening(this, this.NCProcessorPort);
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Utility.SaveSetting("Download Options", "0");
                    this.cacheController.userSettings.DownloadOptions = "0";
                    break;
                case 1:
                    Utility.SaveSetting("Download Options", "1");
                    this.cacheController.userSettings.DownloadOptions = "1";
                    break;
                case 2:
                    Utility.SaveSetting("Download Options", "2");
                    this.cacheController.userSettings.DownloadOptions = "2";
                    break;
            }
        }

        private void MainFormResize(object sender, EventArgs e)
        {
#if (!RIPRIPPERX)
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.MouseDoubleClick -= (HideClick);
                trayIcon.MouseDoubleClick += (ShowClick);

                trayMenu.MenuItems.RemoveAt(0);
                MenuItem show = new MenuItem("Show RiP-Ripper", (ShowClick));
                trayMenu.MenuItems.Add(0, show);

                if (this.cacheController.userSettings.ShowPopUps)
                {
                    trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
                    trayIcon.BalloonTipTitle = "Hidden in Tray";
                    trayIcon.BalloonTipText = "RiP-Ripper is hidden in the Tray";
                    trayIcon.ShowBalloonTip(10);
                }
            }
#endif
            lvCurJob.Columns[0].Width = lvCurJob.Width - 22;
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Save all Jobs
        /// </summary>
        public void SaveOnExit()
        {
            if (mCurrentJob == null && mJobsList.Count <= 0) return;
            try
            {
                // Save Current Job to quere List
                if (mCurrentJob != null)
                {
                    bRipperClosing = true;

                    ThreadManager.GetInstance().DismantleAllThreads();
                    mJobsList.Add(mCurrentJob);

                    mCurrentJob = null;
                }

                XmlSerializer serializer = new XmlSerializer(typeof (List<JobInfo>));
                TextWriter tr = new StreamWriter(Path.Combine(Application.StartupPath, "jobs.xml"));
                serializer.Serialize(tr, mJobsList);
                tr.Close();

                // If Pause
                if (bCurPause)
                {
                    Utility.SaveSetting("CurrentlyPauseThreads", "true");
                }
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, mCurrentJob);
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
                if (sRippedPosts.Contains(sPostId))
                {
                    bCheck = true;
                }
                else
                {
                    sRippedPosts.Add(sPostId);
                }
            }
            catch (Exception)
            {
                bCheck = false;
            }

            return bCheck;
        }

        private void SettingsToolStripMenuItem1Click(object sender, EventArgs e)
        {

            if (this.cacheController.userSettings.SavePids)
            {
                SaveHistory();
            }

            Options oForm = new Options();
            oForm.ShowDialog();

            LoadSettings();

            if (this.cacheController.userSettings.SavePids)
            {
                LoadHistory();
            }
        }

        private static void HelpToolStripMenuItemClick(object sender, EventArgs e)
        {
            About aForm = new About();
            aForm.ShowDialog();
        }

        private void BrowsFolderBtnClick(object sender, EventArgs e)
        {
            UpdateDownloadFolder();
        }
        /// <summary>
        /// Changes the download Folder.
        /// </summary>
        public void UpdateDownloadFolder()
        {
            //dfolderBrowserDialog.ShowDialog(this);
            if (bIsBrowserOpen) return;

            bIsBrowserOpen = true;

            if (this.dfolderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBox2.Text = this.dfolderBrowserDialog.SelectedPath;

            Utility.SaveSetting("Download Folder", this.textBox2.Text);

            this.cacheController.userSettings.DownloadFolder = this.textBox2.Text;

            this.bIsBrowserOpen = false;
        }

        /// <summary>
        /// Check Windows Clip board for Urls to Rip
        /// </summary>
        private void CheckClipboardData()
        {
            if (!cacheController.userSettings.ClipBWatch)
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(Clipboard.GetText()))
                {
                    return;
                }

                // Ignore other input
                if (!Clipboard.GetText().StartsWith(this.cacheController.userSettings.ForumURL))
                {
                    return;
                }

                var clipBoardURLTemp = Clipboard.GetText();

                Clipboard.Clear();

                string[] sClipBoardUrLs = clipBoardURLTemp.Split(new[] { '\n' });

                foreach (string sClipBoardURL in
                    sClipBoardUrLs.Where(
                        sClipBoardURL =>
                        sClipBoardURL.StartsWith(this.cacheController.userSettings.ForumURL)
                        || sClipBoardURL.StartsWith(this.cacheController.userSettings.ForumURL)))
                {
                    if (!this.bParseAct)
                    {
                        this.comboBox1.SelectedIndex = 2;

                        this.textBox1.Text = sClipBoardURL;

                        if (InvokeRequired)
                        {
                            Invoke((MethodInvoker)this.EnqueueJob);
                        }
                        else
                        {
                            this.EnqueueJob();
                        }
                    }
                    else
                    {
                        this.ExtractUrls.Add(sClipBoardURL);
                    }
                }
            }
            catch (Exception)
            {
                Clipboard.Clear();
            }
        }

        /// <summary>
        /// Auto login if User Credentials exists in Config file.
        /// if no Data show Login Form
        /// </summary>
        public void AutoLogin()
        {
            if (this.cacheController.userSettings.User != null && this.cacheController.userSettings.Pass != null)
            {
                LoginManager lgnMgr = new LoginManager(this.cacheController.userSettings.User, this.cacheController.userSettings.Pass);

                if (lgnMgr.DoLogin())
                {
                    this.bCameThroughCorrectLogin = true;
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

                this.bCameThroughCorrectLogin = false;
            }
        }

        /// <summary>
        /// Set Window Position and Size
        /// </summary>
        private void SetWindow()
        {
            try
            {
                this.cacheController.userSettings.WindowLeft = int.Parse(Utility.LoadSetting("Window left"));
                this.cacheController.userSettings.WindowTop = int.Parse(Utility.LoadSetting("Window top"));

                this.Left = this.cacheController.userSettings.WindowLeft;
                this.Top = this.cacheController.userSettings.WindowTop;
            }
            catch (Exception)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            try
            {
                this.cacheController.userSettings.WindowWidth = int.Parse(Utility.LoadSetting("Window width"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.WindowWidth = 863;
            }

            this.Width = this.cacheController.userSettings.WindowWidth;

            try
            {
                this.cacheController.userSettings.WindowHeight = int.Parse(Utility.LoadSetting("Window height"));
            }
            catch (Exception)
            {
                this.cacheController.userSettings.WindowHeight = 611;
            }

            this.Height = this.cacheController.userSettings.WindowHeight;
        }

        /// <summary>
        /// Save the History and Window Size & Positioning on Program Closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs" /> instance containing the event data.</param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveOnExit();

            if (this.cacheController.userSettings.SavePids)
            {
                this.SaveHistory();
            }

            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            Utility.SaveSetting("Window left", this.Left.ToString());
            Utility.SaveSetting("Window top", this.Top.ToString());
            Utility.SaveSetting("Window width", this.Width.ToString());
            Utility.SaveSetting("Window height", this.Height.ToString());
        }

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

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

                    // create a new taskbar jump list for the main window
                    this.jumpList = JumpList.CreateJumpList();

                    if (!string.IsNullOrEmpty(this.cacheController.userSettings.DownloadFolder))
                    {
                        // Add our user tasks
                        this.jumpList.AddUserTasks(
                            new JumpListLink(this.cacheController.userSettings.DownloadFolder, "Open Download Folder")
                                {
                                    IconReference =
                                        new IconReference(
                                        Path.Combine(
                                            Application.StartupPath,
                                            string.Format("{0}.exe", Assembly.GetExecutingAssembly().GetName().Name)),
                                        0)
                                });

                        this.jumpList.Refresh();
                    }


                    // }
            }
#endif
            this.IdleRipper();

            try
            {
                // Reading Saved Jobs
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextReader tr = new StreamReader(Path.Combine(Application.StartupPath, "jobs.xml"));
                this.mJobsList = (List<JobInfo>)serializer.Deserialize(tr);
                tr.Close();

                try
                {
                    this.bCurPause = bool.Parse(Utility.LoadSetting("CurrentlyPauseThreads"));
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
                this.mJobsList = new List<JobInfo>();
            }

            if (this.mJobsList.Count != 0)
            {
                this.StatusLabelInfo.Text = this._ResourceManager.GetString("gbParse2");
                this.StatusLabelInfo.ForeColor = Color.Green;

                for (int i = 0; i < this.mJobsList.Count; i++)
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this._ResourceManager.GetString("gbParse2"), i, this.mJobsList.Count);
                    this.StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(this.mJobsList[i].Title, 0);
                    ijobJob.SubItems.Add(this.mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(this.mJobsList[i].ImageCount.ToString());
                    this.listViewJobList.Items.AddRange(new[] { ijobJob });
                }

                this.bWorking = true;

                this.StatusLabelInfo.Text = string.Empty;
            }

            // Delete Backup Files From AutoUpdate
            if (File.Exists("RiPRipper.bak"))
            {
                File.Delete("RiPRipper.bak");
            }

            // Extract Urls from Text file for Ripping
            this.cacheController.userSettings.TxtFolder = string.IsNullOrEmpty(this.cacheController.userSettings.TxtFolder) ? "ExtractUrls.txt" : Path.Combine(this.cacheController.userSettings.TxtFolder, "ExtractUrls.txt");

            if (File.Exists(this.cacheController.userSettings.TxtFolder))
            {
                this.GetTxtUrls(this.cacheController.userSettings.TxtFolder);
            }
            /////////////////
        }

        /// <summary>
        /// Extract The Cached Urls and Rip them.
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

            this.ExtractUrls.RemoveAt(0);

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
                        sRipUrl.StartsWith(this.cacheController.userSettings.ForumURL) ||
                        sRipUrl.StartsWith(this.cacheController.userSettings.ForumURL)))
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

            Utility.SaveOnCrash(ex.Message, ex.StackTrace, mCurrentJob);

            if (mJobsList.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextWriter tr = new StreamWriter(Path.Combine(Application.StartupPath, "jobs.xml"));
                serializer.Serialize(tr, mJobsList);
                tr.Close();

                // If Pause
                if (this.bCurPause)
                {
                    Utility.SaveSetting("CurrentlyPauseThreads", "true");
                }
            }

            if (this.cacheController.userSettings.SavePids)
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ThreadExceptionFunction(object sender, ThreadExceptionEventArgs e)
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
                if (bCurPause)
                {
                    Utility.SaveSetting("CurrentlyPauseThreads", "true");
                }
            }

            if (this.cacheController.userSettings.SavePids)
            {
                SaveHistory();
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

        private void GetIdxsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ThrdGetIndexes(Convert.ToString(e.Argument)); 
        }

        private void GetIdxsWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.UnlockControls();

            Application.DoEvents();

            if (arlstIndxs == null) return;

            for (int po = 0; po < arlstIndxs.Count; po++)
            {
                StatusLabelInfo.ForeColor = Color.Green;
                StatusLabelInfo.Text = string.Format("{0}{1}/{2}", "Analyse Index(es)", po, arlstIndxs.Count);

                textBox1.Text = arlstIndxs[po].ImageUrl;

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

            arlstIndxs = null;
        }

        /// <summary>
        /// Genarates the Storage Folder for the Current Job
        /// </summary>
        /// <param name="curJob">Curren Job</param>
        /// <returns>The Storage Folder</returns>
        private string GenerateStorePath(JobInfo curJob)
        {
            string sStorePath = this.cacheController.userSettings.DownloadFolder;
            
            if (this.cacheController.userSettings.SubDirs)
            {
                try
                {
                    if (curJob.ForumTitle != null)
                    {
                        if (this.cacheController.userSettings.DownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                this.cacheController.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                this.cacheController.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title));
                        }
                    }
                    else
                    {
                        if (this.cacheController.userSettings.DownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                this.cacheController.userSettings.DownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                this.cacheController.userSettings.DownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
                        }
                    }

                    int iRenameCnt = 2;

                    string sbegining = sStorePath;

                    // Auto Rename if post titles are the same...
                    if (mJobsList.Count != 0)
                    {
                        string path = sStorePath;

                        foreach (JobInfo t in
                            this.mJobsList.Where(t => t.PostTitle.Equals(curJob.PostTitle) || (Directory.Exists(path) && t.Title.Equals(curJob.Title))))
                        {
                            while (t.StorePath.Equals(sStorePath) ||
                                   Directory.Exists(sStorePath))
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
                    sStorePath = Path.Combine(this.cacheController.userSettings.DownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
                }
            }

            return sStorePath;
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
    }
}