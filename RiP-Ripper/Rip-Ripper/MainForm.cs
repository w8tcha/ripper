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
    /// Summary description for MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool bDelete;
        /// <summary>
        /// 
        /// </summary>
        public static string sDeleteMessage;
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
        private ResourceManager rm;

        /// <summary>
        /// All Settings
        /// </summary>
        public static SettingBase userSettings = new SettingBase();

#if (!RIPRIPPERX)

        private TaskbarManager windowsTaskbar;
        private JumpList jumpList;

        public IntPtr nextClipboardViewer;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
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
        protected void SDownloadClick(Object sender, EventArgs e)
        {
            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) ||
                Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().IndexOf(@"http://rip-productions.net/") <
                0 || this.bParseAct)
            {
                return;
            }

            comboBox1.SelectedIndex = 2;

            textBox1.Text = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();

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

            if (!userSettings.bShowPopUps) return;

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
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            Application.Run(new MainForm());

            //ProcessArgs(args);
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
                userSettings.bOfflMod = bool.Parse(Utility.LoadSetting("OfflineModus"));
            }
            catch (Exception)
            {
                userSettings.bOfflMod = false;
            }

            // Load "Firefox Exension Enabled" Setting
            try
            {
                userSettings.bExtension = bool.Parse(Utility.LoadSetting("FFExtension"));
            }
            catch (Exception)
            {
                userSettings.bExtension = false;
            }

            
            try
            {
                if (!string.IsNullOrEmpty(Utility.LoadSetting("TxtFolder")))
                {
                    userSettings.sTxtFolder = Utility.LoadSetting("TxtFolder");
                }
            }
            catch (Exception)
            {
                userSettings.sTxtFolder = null;
            }

            // Load "Clipboard Watch" Setting
            try
            {
                userSettings.bClipBWatch = bool.Parse(Utility.LoadSetting("clipBoardWatch"));
            }
            catch (Exception)
            {
                userSettings.bClipBWatch = true;
            }


            try
            {
                userSettings.bShowPopUps = bool.Parse(Utility.LoadSetting("Show Popups"));
            }
            catch (Exception)
            {
                userSettings.bShowPopUps = true;
            }

            try
            {
                userSettings.bSubDirs = bool.Parse(Utility.LoadSetting("SubDirs"));
            }
            catch (Exception)
            {
                userSettings.bSubDirs = true;
            }


            try
            {
                userSettings.bAutoThank = bool.Parse(Utility.LoadSetting("Auto TK Button"));
            }
            catch (Exception)
            {
                userSettings.bAutoThank = false;
            }

            try
            {
                userSettings.bDownInSepFolder = bool.Parse(Utility.LoadSetting("DownInSepFolder"));
            }
            catch (Exception)
            {
                userSettings.bDownInSepFolder = true;
            }

            try
            {
                userSettings.bSavePids = bool.Parse(Utility.LoadSetting("SaveRippedPosts"));
            }
            catch (Exception)
            {
                userSettings.bSavePids = true;
            }

            try
            {
                userSettings.bShowCompletePopUp = bool.Parse(Utility.LoadSetting("Show Downloads Complete PopUp"));
            }
            catch (Exception)
            {
                userSettings.bShowCompletePopUp = true;
            }

            // min. Image Count for Thanks
            try
            {
                userSettings.iMinImageCount = !string.IsNullOrEmpty(Utility.LoadSetting("minImageCountThanks")) ? int.Parse(Utility.LoadSetting("minImageCountThanks")) : 3;
            }
            catch (Exception)
            {
                userSettings.iMinImageCount = 3;
            }

            // Max. Threads
            try
            {
                userSettings.iThreadLimit = -1;

                userSettings.iThreadLimit = Convert.ToInt32(Utility.LoadSetting("Thread Limit"));

                ThreadManager.GetInstance().SetThreadThreshHold(userSettings.iThreadLimit == -1
                                                                    ? 3
                                                                    : userSettings.iThreadLimit);
            }
            catch (Exception)
            {
                userSettings.iThreadLimit = 3;
                ThreadManager.GetInstance().SetThreadThreshHold(3);
            }

            try
            {
                userSettings.sDownloadFolder = Utility.LoadSetting("Download Folder");
                textBox2.Text = userSettings.sDownloadFolder;

                if (string.IsNullOrEmpty(userSettings.sDownloadFolder))
                {
                    userSettings.sDownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    textBox2.Text = userSettings.sDownloadFolder;

                    Utility.SaveSetting("Download Folder", textBox2.Text);
                }
            }
            catch (Exception)
            {
                userSettings.sDownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                textBox2.Text = userSettings.sDownloadFolder;

                Utility.SaveSetting("Download Folder", textBox2.Text);
                userSettings.sDownloadFolder = textBox2.Text;
            }

            // Load "Download Options"
            try
            {
                userSettings.sDownloadOptions = Utility.LoadSetting("Download Options");

                switch (userSettings.sDownloadOptions)
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
                userSettings.sDownloadOptions = "0";
                comboBox1.SelectedIndex = 0;
            }

            // Load "Always on Top" Setting
            try
            {
                userSettings.bTopMost = bool.Parse(Utility.LoadSetting("Always OnTop"));
                TopMost = userSettings.bTopMost;
            }
            catch (Exception)
            {
                userSettings.bTopMost = false;
                TopMost = false;
            }


            // Load Language Setting
            try
            {
                userSettings.sLanguage = Utility.LoadSetting("UserLanguage");

                switch (userSettings.sLanguage)
                {
                    case "de-DE":
                        rm = new ResourceManager("RiPRipper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        rm = new ResourceManager("RiPRipper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    default:
                        rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                AdjustCulture();
            }
            catch (Exception)
            {
                rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
            }


            try
            {
                userSettings.sUser = Utility.LoadSetting("User");

                // Import old Password
                try
                {
                    string sOldPass = Utility.EncodePassword(Utility.LoadSetting("Pass")).Replace("-", string.Empty).ToLower();

                    Utility.SaveSetting("Password", sOldPass);
                    Utility.DeleteSetting("Pass");

                    userSettings.sPass = sOldPass;
                }
                catch (Exception)
                {
                    userSettings.sPass = null;
                }

                userSettings.sPass = Utility.LoadSetting("Password");
            }
            catch (Exception)
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (!bCameThroughCorrectLogin)
                {
                    DialogResult result = TopMostMessageBox.Show(this.rm.GetString("mbExit"), this.rm.GetString("mbExitTtl"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                }
            }
        }
        
        
        private void AdjustCulture()
        {
            mStartDownloadBtn.Text = rm.GetString("btnStartDownload");
            groupBox1.Text = rm.GetString("downloadOptions");
            groupBox5.Text = string.Format("{0} (-):", rm.GetString("lblRippingQue"));
            stopCurrentThreads.Text = rm.GetString("btnStop");
            groupBox4.Text = rm.GetString("lblLastPicture");
            groupBox2.Text = rm.GetString("gbCurrently");
            mInvalidDestinationMsg = rm.GetString("mbInvalidDes");
            mIncorrectUrlMsg = rm.GetString("mbIncorrectURL");
            mNoThreadMsg = rm.GetString("mbNoThread");
            mNoPostMsg = rm.GetString("mbNoPost");
            mAlreadyQueuedMsg = rm.GetString("mbAlreadyQueued");
            mTNumericMsg = rm.GetString("mbTNumeric");

            // Menue
            fileToolStripMenuItem.Text = rm.GetString("MenuFile");
            settingsToolStripMenuItem1.Text = rm.GetString("MenuSettings");
            exitToolStripMenuItem.Text = rm.GetString("MenuExit");

            settingsToolStripMenuItem.Text = rm.GetString("MenuHelp");
            helpToolStripMenuItem.Text = rm.GetString("MenuAbout");

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

            mrefCC = CacheController.GetInstance();
            mrefTM = ThreadManager.GetInstance();

#if (!RIPRIPPERX)


            if (VersionCheck.UpdateAvailable() && File.Exists(Application.StartupPath + "\\ICSharpCode.SharpZipLib.dll"))
            {
                string mbUpdate = rm.GetString("mbUpdate"), mbUpdate2 = rm.GetString("mbUpdate2");
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

            LoadSettings();

            if (!userSettings.bOfflMod)
            {
                AutoLogin();

                if (!bCameThroughCorrectLogin)
                    Application.Exit();
            }

            //LoadSettings();

            if (userSettings.bSavePids)
            {
                LoadHistory();
            }

#if (!RIPRIPPERX)
            trayIcon.Visible = true;
#endif

            if (!userSettings.bExtension)
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
                userSettings.sDownloadFolder = aJobPath;
                comboBox1.SelectedIndex = 2;

                if (aAutoFolder == "1")
                {
                    userSettings.bSubDirs = true;
                    userSettings.bDownInSepFolder = true;
                }
                else
                {
                    userSettings.bSubDirs = false;
                    userSettings.bDownInSepFolder = false;
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
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Arguments.
        /// </param>
        private void MStartDownloadBtnClick(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) && Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().IndexOf(@"http://rip-productions.net/") >= 0)
            {
                comboBox1.SelectedIndex = 2;

                textBox1.Text = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
            }

            if (textBox1.Text.StartsWith("http"))
            {
                comboBox1.SelectedIndex = 2;
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
            if (sXmlUrl.Contains("dpver=2&postid=") && userSettings.bSavePids)
            {
                if (this.IsPostAlreadyRipped(sXmlUrl.Substring(sXmlUrl.IndexOf("&postid=") + 8)))
                {
                    DialogResult result = TopMostMessageBox.Show(this.rm.GetString("mBAlready"), "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                if (userSettings.bDownInSepFolder && sXmlUrl.Contains("threadid"))
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

            if (string.IsNullOrEmpty(userSettings.sDownloadFolder))
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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().ExtractTitleFromXML(Maintainance.GetInstance().GetPostPages(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().ExtractTitleFromXML(Maintainance.GetInstance().GetPostPages(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().ExtractTitleFromXML(Maintainance.GetInstance().GetPostPages(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().ExtractTitleFromXML(Maintainance.GetInstance().GetPostPages(sXmlUrl))))
                {
                    TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

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

            JobInfo job = new JobInfo { URL = sXmlUrl, XMLPayLoad = Maintainance.GetInstance().GetPostPages(sXmlUrl) };

            //job.sStorePath = sDownloadFolder;
            job.PostTitle = Maintainance.GetInstance().ExtractPostTitleFromXML(job.XMLPayLoad);
            job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
            job.Title = Maintainance.GetInstance().ExtractTitleFromXML(job.XMLPayLoad);
            job.ImageCount = Maintainance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

            job.Title = Utility.ReplaceHexWithAscii(job.Title);
            job.PostTitle = Utility.ReplaceHexWithAscii(job.PostTitle);

            if (job.ImageCount.Equals(0))
            {
                this.UnlockControls();
            }

            job.StorePath = this.GenerateStorePath(job);

            if (string.IsNullOrEmpty(job.Title))
            {
                TopMostMessageBox.Show(sXmlUrl.IndexOf("threadid=") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

                this.UnlockControls();

                return;
            }

            this.ProcessAutoThankYou(null, job.ImageCount, job.URL, null);

            JobListAddDelegate newJob = this.JobListAdd;
           Invoke(newJob, new object[] { job });

            ///////////////////////////////////////////////
           this.UnlockControls();
            ///////////////////////////////////////////////
        }

        /// <summary>
        /// Pushes the "Thank You" Button for the user.
        /// </summary>
        void ProcessAutoThankYou(string sPostId, int iICount, string sUrl, string sToken)
        {
            if (!userSettings.bAutoThank) { return; }

            if (iICount < userSettings.iMinImageCount) { return; }

            SendThankYouDelegate lSendThankYouDel = (SendThankYou);

            string tyURL;

            sUrl = sUrl.Replace("getSTDpost-imgXML.php?dpver=2&postid=", "showpost.php?p=");

            // Complete Thread
            if (sPostId != null)
            {
                //string sToken = Utility.GetSToken("http://rip-productions.net/showpost.php?p=" + sPostId);

                if (string.IsNullOrEmpty(sToken))
                {
                    return;
                }

                tyURL = string.Format("http://rip-productions.net/post_thanks.php?do=post_thanks_add&p={0}&securitytoken={1}", sPostId, sToken);

                //SendThankYou(tyURL);
                Invoke(lSendThankYouDel, new object[] { tyURL });

            }
            else
            {
                // Single Post
                switch (comboBox1.SelectedIndex)
                {
                    case 1:
                        {
                            sToken = Utility.GetSToken(sUrl);

                            tyURL = string.Format("http://rip-productions.net/post_thanks.php?do=post_thanks_add&p={0}&securitytoken={1}",
                                                  sUrl.Substring(sUrl.IndexOf("?p=") + 3), sToken);

                            //SendThankYou(tyURL);
                            Invoke(lSendThankYouDel, new object[] { tyURL });
                        }
                        break;
                    case 2:
                        string sUrlNew = sUrl.Replace("showpost.php?p=", "post_thanks.php?do=post_thanks_add&p=");

                        tyURL = string.Format("{0}&securitytoken={1}", sUrlNew, Utility.GetSToken(sUrl));

                        Invoke(lSendThankYouDel, new object[] { tyURL });

                        break;
                }
            }
        }

        // This delegate enables asynchronous calls for automatically sending thank yous
        delegate void SendThankYouDelegate(string aUrl);

        static void SendThankYou
        (string aUrl)
        {
            const string tyURLRef = "http://rip-productions.net/";

            HttpWebResponse lHttpWebResponse = null;
            Stream lHttpWebResponseStream = null;


            HttpWebRequest lHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
            lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
            lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            lHttpWebRequest.Headers.Add("Cookie: " + CookieManager.GetInstance().GetCookieString());
            lHttpWebRequest.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            lHttpWebRequest.KeepAlive = true;
            //lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
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
        /// TODO : Change to Html Output Xml doesnt include ShowLinks anymore
        /// </summary>
        private void ThrdGetIndexes(string sXmlUrl)
        {
            Indexes idxs = new Indexes();

            string sPagecontent = idxs.GetPostPages(string.Format("{0}&showlinks=1", sXmlUrl));

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
            ThreadToPost threadPosts = new ThreadToPost();

            string sPagecontent = threadPosts.GetPostPages(sXmlUrl);

            List<ImageInfo> arlst = threadPosts.ParseXML(sPagecontent);

            string sToken = Utility.GetSToken("http://rip-productions.net/");

            for (int po = 0; po < arlst.Count; po++)
            {
                StatusLabelInfo.ForeColor = Color.Green;

                try
                {
                    this.StatusLabelInfo.Text = string.Format("{0}{1}/{2}", this.rm.GetString("gbParse"), po, arlst.Count);
                }
                catch (Exception)
                {
                    
                }

                string sLpostId = arlst[po].ImageUrl;

                //////////////////////////////////////////////////////////////////////////

                if (userSettings.bSavePids && IsPostAlreadyRipped(sLpostId))
                {
                    goto SKIPIT;
                }

                string sLComposedURL = string.Format("http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}", sLpostId);


                JobInfo jobInfo = mJobsList.Find(doubleJob => (doubleJob.URL.Equals(sLComposedURL)));

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
                                  {
                                      URL = sLComposedURL,
                                      XMLPayLoad = Maintainance.GetInstance().GetPostPages(sLComposedURL)
                                  };

                job.PostTitle = Maintainance.GetInstance().ExtractPostTitleFromXML(job.XMLPayLoad);
                job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromXML(job.XMLPayLoad);
                job.Title = Maintainance.GetInstance().ExtractTitleFromXML(job.XMLPayLoad);
                job.ImageCount = Maintainance.GetInstance().CountImagesFromXML(job.XMLPayLoad);

                job.Title = Utility.ReplaceHexWithAscii(job.Title);
                job.PostTitle = Utility.ReplaceHexWithAscii(job.PostTitle);

                job.StorePath = GenerateStorePath(job);

                if (job.ImageCount.Equals(0))
                {
                    goto SKIPIT;
                }

                ProcessAutoThankYou(sLpostId, job.ImageCount, job.URL, sToken);

                //JobListAdd(job);
                JobListAddDelegate newJob = JobListAdd;
                Invoke(newJob, new object[] { job });

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
            ////if (bWorking || mJobsList.Count > 0)
            if (this.bWorking && !this.bParseAct || this.mJobsList.Count > 0 && !this.bParseAct)
            {
                this.LogicCode();
            }

            if (this.StatusLabelInfo.Text.Equals(this.rm.GetString("StatusLabelInfo")) &&
                this.mJobsList.Count.Equals(0) &&
                this.mCurrentJob == null)
            {
                this.IdleRipper();
            }

            if (!this.bParseAct && this.ExtractUrls.Count > 0 &&
                !GetPostsWorker.IsBusy && !GetIdxsWorker.IsBusy)
            {
                this.GetExtractUrls();
            }
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
            if (bDelete && !bFullDisc)
            {
                FullDisc();
            }

            if (mrefTM.GetThreadCount() > 0)
            {
                // If Joblist empty and the last Threads of Current Job are parsed
                if (mCurrentJob == null && mJobsList.Count.Equals(0) && !bParseAct)
                {
                    bEndRip = true;

                    StatusLabelInfo.Text = rm.GetString("StatusLabelInfo");
                    StatusLabelInfo.ForeColor = Color.Red;

                    groupBox5.Text = string.Format("{0} (-):", rm.GetString("lblRippingQue"));
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
                    StatusLabelImageC.Text = string.Empty;

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
                        IdleRipper();
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

            string bSystemExtr = rm.GetString("bSystemExtr");

            ParseJobXml();

            groupBox2.Text = string.Format("{0}...", rm.GetString("gbCurrentlyExtract"));

            if (mCurrentJob.Title.Equals(mCurrentJob.PostTitle))
            {
                Text = string.Format("{0}: {1} - x{2}", rm.GetString("gbCurrentlyExtract"), mCurrentJob.Title,
                                                mImagesList.Count);
            }
            else
            {
                Text = string.Format("{0}: {1} - {2} - x{3}", rm.GetString("gbCurrentlyExtract"),
                                               mCurrentJob.Title, mCurrentJob.PostTitle, mImagesList.Count);
            }

            lvCurJob.Columns[0].Text = string.Format("{0} - x{1}", mCurrentJob.PostTitle, mImagesList.Count);

#if (!RIPRIPPERX)
            try
            {
                if (userSettings.bShowPopUps)
                {
                    trayIcon.Text = rm.GetString("gbCurrentlyExtract") + mCurrentJob.Title;
                    trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                    trayIcon.BalloonTipTitle = rm.GetString("gbCurrentlyExtract");
                    trayIcon.BalloonTipText = mCurrentJob.Title;
                    trayIcon.ShowBalloonTip(10);
                }
            }
            catch (Exception)
            {
                if (userSettings.bShowPopUps)
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
                string tiImagesRemain = this.rm.GetString("tiImagesRemain");

                ////////////////
                lvCurJob.Items.Clear();
                StatusLabelImageC.Text = string.Empty;

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
                        if (mImagesList[i].ImageUrl.Length > 6)
                        {
                            if (!(i > lvCurJob.Items.Count))
                            {
                                lvCurJob.Items[i].Selected = true;
                                lvCurJob.EnsureVisible(i);
                            }

                            CacheController.GetInstance().DownloadImage(mImagesList[i].ImageUrl, mCurrentJob.StorePath);
                        }

                        if (!(i > lvCurJob.Items.Count))
                        {
                            if (InvokeRequired)
                            {
                               this.Invoke((MethodInvoker)this.ShowLastPic);
                            }
                            else
                            {
                                this.ShowLastPic();
                            }

                            if (!this.bRipperClosing)
                            {
                                lvCurJob.Items[i].ForeColor = Color.Green;
                            }
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
                StatusLabelImageC.Text = string.Empty;
            }
        }

        private Image imgLastPic;
        /// <summary>
        /// Shows the last Downloaded Image
        /// </summary>
        public void ShowLastPic()
        {

            // Reclaim resources used by previous image in the picturebox
            /*if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }*/

            if (pictureBox1.Image != null)
            {
              //  imgLastPic.Dispose();
               // imgLastPic = null;

                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;

            }

            if (pictureBox1.BackgroundImage != null)
            {
                pictureBox1.BackgroundImage.Dispose();
                pictureBox1.BackgroundImage = null;
            }
            ///////////////////////////

            sLastPic = mrefCC.uSLastPic;

            if (File.Exists(sLastPic))
            {
                try
                {
                    pictureBox1.Visible = true;

                    if (sLastPic.EndsWith(".gif"))
                    {
                        pictureBox1.BackgroundImage = Image.FromFile(sLastPic);
                        pictureBox1.Update();
                    }
                    else
                    {
                        // This statement causes file locking until the
                        // process exits unless cleared when not visible

                        imgLastPic = Image.FromFile(sLastPic);

                        pictureBox1.Image = imgLastPic;

                        //pictureBox1.Image = Image.FromFile(sLastPic);
                        pictureBox1.Update();
                    }
                }
                catch (Exception)
                {
                    //imgLastPic.Dispose();
                    //imgLastPic = null;
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }
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

            string btleExit = rm.GetString("btleExit"),
                   btexExit = rm.GetString("btexExit");

            if (bEndRip && userSettings.bShowCompletePopUp)
            {
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.BalloonTipTitle = btleExit;
                trayIcon.BalloonTipText = btexExit;
                trayIcon.ShowBalloonTip(10);
            }
#endif
            lvCurJob.Items.Clear();
            StatusLabelImageC.Text = string.Empty;

            textBox1.Text = string.Empty;
            progressBar1.Value = 0;

            stopCurrentThreads.Enabled = true;
            bStopJob = false;
            bEndRip = false;

            string ttlHeader = rm.GetString("ttlHeader");

            groupBox2.Text = rm.GetString("gbCurrentlyIdle");
            StatusLabelInfo.Text = rm.GetString("gbCurrentlyIdle");
            StatusLabelInfo.ForeColor = Color.Gray;

            lvCurJob.Columns[0].Text = "  ";

#if (RIPRIPPER)
            Text = String.Format("RiP-Ripper {0}.{1}.{2}{3}{4}", 
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                ttlHeader + userSettings.sUser + "\"]");

            trayIcon.Text = "Right click for context menu";
#elif (RIPRIPPERX)
            Text = String.Format("RiP-Ripper X {0}.{1}.{2}{3}{4}", 
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                ttlHeader + userSettings.sUser + "\"]");
#else
            Text = String.Format("RiP-Ripper {0}.{1}.{2}{3}{4}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                ttlHeader + userSettings.sUser + "\"]");

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

            groupBox5.Text = string.Format("{0} ({1}):", rm.GetString("lblRippingQue"), mJobsList.Count);
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

            groupBox5.Text = string.Format("{0} ({1}):", rm.GetString("lblRippingQue"), mJobsList.Count);
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

            StatusLabelInfo.Text = sDeleteMessage;
            StatusLabelInfo.ForeColor = Color.Red;

            TopMostMessageBox.Show(string.Format("Please change your download location, then press \"Resume Download\", because {0}", sDeleteMessage), "Warning");

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

                if (userSettings.bSubDirs)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        updatedJob.StorePath = Path.Combine(userSettings.sDownloadFolder, mJobsList[i].Title);
                    }
                    if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                    {
                        updatedJob.StorePath = Path.Combine(userSettings.sDownloadFolder, mJobsList[i].Title + Path.DirectorySeparatorChar + mJobsList[i].PostTitle);
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
                    StatusLabelInfo.Text = string.Format("{0}{1}/{2}", rm.GetString("gbParse2"), i, mJobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(mJobsList[i].Title, 0);
                    ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }
            }
            finally
            {
                groupBox5.Text = string.Format("{0} ({1}):", rm.GetString("lblRippingQue"), mJobsList.Count);
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
            StatusLabelImageC.Text = string.Empty;

            CheckCurJobFolder(sLastJobFolder);
        }

        /// <summary>
        /// Pause/Resumes Downloading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseCurrentThreadsClick(object sender, EventArgs e)
        {
            switch (pauseCurrentThreads.Text)
            {
                case "Pause Download(s)":
                    pauseCurrentThreads.Text = "Resume Download(s)";
                    bCurPause = true;
                    ThreadManager.GetInstance().HoldAllThreads();
                    pauseCurrentThreads.Image = Languages.english.play;
                    break;
                case "(Re)Start Download(s)":
                    StatusLabelImageC.Text = string.Empty;
                    Utility.SaveSetting("CurrentlyPauseThreads", "false");
                    bCurPause = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    //LoadSettings();
                    //IdleRipper();
                    pauseCurrentThreads.Text = "Pause Download(s)";
                    pauseCurrentThreads.Image = Languages.english.pause;
                    break;
                case "Resume Download(s)":
                    StatusLabelImageC.Text = string.Empty;
                    bCurPause = false;
                    deleteJob.Enabled = true;
                    stopCurrentThreads.Enabled = true;
                    pauseCurrentThreads.Text = "Pause Download(s)";
                    ThreadManager.GetInstance().ResumeAllThreads();
                    pauseCurrentThreads.Image = Languages.english.pause;
                    break;
            }
        }
        private void StartNcProcessor()
        {
            mNCProcessor = new NetCommandProcessor();
            mNCProcessor.StartListening(this, NCProcessorPort);
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Utility.SaveSetting("Download Options", "0");
                    userSettings.sDownloadOptions = "0";
                    break;
                case 1:
                    Utility.SaveSetting("Download Options", "1");
                    userSettings.sDownloadOptions = "1";
                    break;
                case 2:
                    Utility.SaveSetting("Download Options", "2");
                    userSettings.sDownloadOptions = "2";
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

                if (userSettings.bShowPopUps)
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

            if (userSettings.bSavePids)
            {
                SaveHistory();
            }

            Options oForm = new Options();
            oForm.ShowDialog();

            LoadSettings();

            if (userSettings.bSavePids)
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

            userSettings.sDownloadFolder = this.textBox2.Text;

            this.bIsBrowserOpen = false;
        }
#if (!RIPRIPPERX)
        protected override void WndProc(ref Message m)
        {
            try
            {
                // defined in winuser.h
                const int wmDrawclipboard = 0x308;
                const int wmChangecbchain = 0x030D;

                switch (m.Msg)
                {
                    case wmDrawclipboard:
                        CheckClipboardData();
                        Win32.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;

                    case wmChangecbchain:
                        if (m.WParam == nextClipboardViewer)
                            nextClipboardViewer = m.LParam;
                        else
                            Win32.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Check Windows Clip board for Urls to Rip
        /// </summary>
        private void CheckClipboardData()
        {
            if (!userSettings.bClipBWatch)
            {
                return;
            }

            try
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string sClipBoardURLTemp = (string)iData.GetData(DataFormats.Text);

                    string[] sClipBoardUrLs = sClipBoardURLTemp.Split(new[] { '\n' });

                    foreach (string sClipBoardURL in
                        sClipBoardUrLs.Where(sClipBoardURL => sClipBoardURL.StartsWith(@"http://rip-productions.net/") || sClipBoardURL.StartsWith(@"http://www.rip-productions.net/")))
                    {
                        if (!this.bParseAct)
                        {
                            if (this.comboBox1.SelectedIndex != 2)
                            {
                                this.comboBox1.SelectedIndex = 2;
                            }

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

                        Clipboard.Clear();
                    }
                }
            }
            catch (Exception)
            {
                Clipboard.Clear();
            }
        }
#endif
        /// <summary>
        /// Auto login if User Credentials exists in Config file.
        /// if no Data show Login Form
        /// </summary>
        public void AutoLogin()
        {
            if (userSettings.sUser != null && userSettings.sPass != null)
            {
                LoginManager lgnMgr = new LoginManager(userSettings.sUser, userSettings.sPass);

                if (lgnMgr.DoLogin())
                {
                    bCameThroughCorrectLogin = true;
                }
                else
                {
                    Login frmLgn = new Login();
                    frmLgn.ShowDialog(this);

                    DialogResult result = TopMostMessageBox.Show(rm.GetString("mbExit"), rm.GetString("mbExitTtl"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

                bCameThroughCorrectLogin = false;
            }
        }
        /// <summary>
        /// Set Window Position and Size
        /// </summary>
        private void SetWindow()
        {
            try
            {
                userSettings.iWindowLeft = int.Parse(Utility.LoadSetting("Window left"));
                userSettings.iWindowTop = int.Parse(Utility.LoadSetting("Window top"));

                Left = userSettings.iWindowLeft;
                Top = userSettings.iWindowTop;
            }
            catch (Exception)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }

            try
            {
                userSettings.iWindowWidth = int.Parse(Utility.LoadSetting("Window width"));
            }
            catch (Exception)
            {
                userSettings.iWindowWidth = 863;
            }

            Width = userSettings.iWindowWidth;

            try
            {
                userSettings.iWindowHeight = int.Parse(Utility.LoadSetting("Window height"));
            }
            catch (Exception)
            {
                userSettings.iWindowHeight = 611;
            }

            Height = userSettings.iWindowHeight;

        }
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveOnExit();

            if (userSettings.bSavePids)
            {
                SaveHistory();
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
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            Application.Idle += (OnLoaded);
        }

        private void OnLoaded(object sender, EventArgs args)
        {
            Application.Idle -= OnLoaded;

            tmrPageUpdate.Enabled = true;

#if (!RIPRIPPERX)

            // TODO : Check if Windows 7 API dlls exists
            //if (File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll")) && 
             //   File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll")))
            //{
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    Environment.OSVersion.Version.Major >= 6 &&
                    Environment.OSVersion.Version.Minor >= 1)
                {
                    windowsTaskbar = TaskbarManager.Instance;

                    // create a new taskbar jump list for the main window
                    jumpList = JumpList.CreateJumpList();



                    if (!string.IsNullOrEmpty(userSettings.sDownloadFolder))
                    {

                        // Add our user tasks
                        jumpList.AddUserTasks(new JumpListLink(userSettings.sDownloadFolder, "Open Download Folder")
                                                  {
                                                      IconReference =
                                                          new IconReference(
                                                          Path.Combine(Application.StartupPath,
                                                                       Assembly.GetExecutingAssembly().GetName().Name +
                                                                       ".exe"), 0)
                                                  });

                        jumpList.Refresh();
                    }


                    // }
            }
#endif
            IdleRipper();

            try
            {
                // Reading Saved Jobs
                XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
                TextReader tr = new StreamReader(Path.Combine(Application.StartupPath, "jobs.xml"));
                mJobsList = (List<JobInfo>)serializer.Deserialize(tr);
                tr.Close();

                try
                {
                    bCurPause = bool.Parse(Utility.LoadSetting("CurrentlyPauseThreads"));
                }
                catch (Exception)
                {
                    bCurPause = false;
                }

                if (bCurPause)
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
                StatusLabelInfo.Text = rm.GetString("gbParse2");
                StatusLabelInfo.ForeColor = Color.Green;

                for (int i = 0; i < mJobsList.Count; i++)
                {
                    StatusLabelInfo.Text = string.Format("{0}{1}/{2}", rm.GetString("gbParse2"), i, mJobsList.Count);
                    StatusLabelInfo.ForeColor = Color.Green;

                    ListViewItem ijobJob = new ListViewItem(mJobsList[i].Title, 0);
                    ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                    ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                    listViewJobList.Items.AddRange(new[] { ijobJob });
                }

                bWorking = true;

                StatusLabelInfo.Text = string.Empty;
            }

#if (!RIPRIPPERX)
            nextClipboardViewer = (IntPtr)Win32.SetClipboardViewer((int)Handle);
#endif

            // Delete Backup Files From AutoUpdate
            if (File.Exists("RiPRipper.bak"))
            {
                File.Delete("RiPRipper.bak");
            }

            // Extract Urls from Text file for Ripping
            userSettings.sTxtFolder = string.IsNullOrEmpty(userSettings.sTxtFolder) ? "ExtractUrls.txt" : Path.Combine(userSettings.sTxtFolder, "ExtractUrls.txt");

            if (File.Exists(userSettings.sTxtFolder))
            {
                this.GetTxtUrls(userSettings.sTxtFolder);
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
                        sRipUrl.StartsWith(@"http://rip-productions.net/") ||
                        sRipUrl.StartsWith(@"http://www.rip-productions.net/")))
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

            if (userSettings.bSavePids)
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

            if (userSettings.bSavePids)
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
            string sStorePath = userSettings.sDownloadFolder;
            
            if (userSettings.bSubDirs)
            {
                try
                {
                    if (curJob.ForumTitle != null)
                    {
                        if (userSettings.bDownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                userSettings.sDownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                userSettings.sDownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.ForumTitle) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.Title));
                        }
                    }
                    else
                    {
                        if (userSettings.bDownInSepFolder)
                        {
                            sStorePath = Path.Combine(
                                userSettings.sDownloadFolder,
                                Utility.RemoveIllegalCharecters(curJob.Title) + Path.DirectorySeparatorChar +
                                Utility.RemoveIllegalCharecters(curJob.PostTitle));
                        }
                        else
                        {
                            sStorePath = Path.Combine(
                                userSettings.sDownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
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
                    sStorePath = Path.Combine(userSettings.sDownloadFolder, Utility.RemoveIllegalCharecters(curJob.Title));
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

            this.StatusLabelInfo.Text = this.rm.GetString("gbParse");
            StatusLabelInfo.ForeColor = Color.Green;
        }

    }
}