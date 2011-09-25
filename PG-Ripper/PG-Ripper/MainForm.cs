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
    using Microsoft.WindowsAPICodePack.Shell;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using PGRipper.Objects;

    /// <summary>
	/// Summary description for MainForm.
	/// </summary>
    public partial class MainForm : Form
	{
        /// <summary>
        /// String List of Urls to Rip
        /// </summary>
        private readonly List<string> ExtractUrls = new List<string>(); 
#if (!PGRIPPERX)
        private TaskbarManager windowsTaskbar;
        private JumpList jumpList;

        public IntPtr nextClipboardViewer;
#endif

        public static bool bDelete;
        public static string sDeleteMessage;

        /// <summary>
        /// Indicates if the Browse Dialog is Already Opten
        /// </summary>
        public bool bIsBrowserOpen;

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
        public ResourceManager rm;

        /// <summary>
        /// Array List of all Stored (Ripped) Post Ids
        /// </summary>
        private readonly ArrayList sRippedPosts = new ArrayList();

        /// <summary>
        /// All Settings
        /// </summary>
        public static SettingBase userSettings = new SettingBase();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
#if (!PGRIPPERX)
            // Tray Menue
            trayMenu = new ContextMenu();

            MenuItem hide = new MenuItem("Hide PG-Ripper", HideClick);
            MenuItem sDownload = new MenuItem("Start Download", SDownloadClick);
            MenuItem exit = new MenuItem("Exit Program", ExitClick);
            
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

            trayIcon.MouseDoubleClick += HideClick;
#endif
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mJobsList = new List<JobInfo>();

            SetWindow();
        }

#if (!PGRIPPERX)
        protected void SDownloadClick(Object sender, EventArgs e)
        {
            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) ||
                Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().IndexOf(userSettings.sForumUrl) < 0 ||
                bParseAct)
            {
                return;
            }

            if (this.comboBox1.SelectedIndex != 2)
            {
                this.comboBox1.SelectedIndex = 2;
            }

            this.textBox1.Text = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)this.EnqueueJob);
            }
            else
            {
                this.EnqueueJob();
            }
        }
        protected void ExitClick(Object sender, EventArgs e)
        {
            Close();
        }
        protected void HideClick(Object sender, EventArgs e)
        {
            Hide();

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem show = new MenuItem("Show PG-Ripper", ShowClick); 
            trayMenu.MenuItems.Add(0, show);

            trayIcon.MouseDoubleClick -= HideClick;
            trayIcon.MouseDoubleClick += ShowClick;

            if (!userSettings.bShowPopUps) return;

            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.BalloonTipTitle = "Hidden in Tray";
            trayIcon.BalloonTipText = "PG-Ripper is hidden in the Tray";
            trayIcon.ShowBalloonTip(10);
        }

        protected void ShowClick(Object sender, EventArgs e)
        {
            Show();

            trayMenu.MenuItems.RemoveAt(0);
            MenuItem hide = new MenuItem("Hide PG-Ripper", HideClick);
            trayMenu.MenuItems.Add(0,hide);
            
            trayIcon.MouseDoubleClick -= ShowClick;
            trayIcon.MouseDoubleClick += HideClick;

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
        }
#endif
		

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args ) 
		{
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
			
            CacheController.xform = new MainForm();
			ProcessArgs( args, CacheController.xform );

            Application.Run( CacheController.xform );
		}
        public void LoadSettings()
        {
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

                if (string.IsNullOrEmpty(userSettings.sDownloadFolder))
                {
                    userSettings.sDownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    textBox2.Text = userSettings.sDownloadFolder;

                    Utility.SaveSetting("Download Folder", textBox2.Text);
                }

                textBox2.Text = userSettings.sDownloadFolder;
            }
            catch (Exception)
            {
                userSettings.sDownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                textBox2.Text = userSettings.sDownloadFolder;

                Utility.SaveSetting("Download Folder", textBox2.Text);
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
                        rm = new ResourceManager("PGRipper.Languages.german", Assembly.GetExecutingAssembly());
                        break;
                    case "fr-FR":
                        rm = new ResourceManager("PGRipper.Languages.french", Assembly.GetExecutingAssembly());
                        break;
                    case "en-EN":
                        rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                    default:
                        rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                        break;
                }

                AdjustCulture();
            }
            catch (Exception)
            {
                rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
            }

            try
            {
                userSettings.sForumUrl = Utility.LoadSetting("forumURL");
            }
            catch (Exception)
            {
                userSettings.sForumUrl = "http://www.kitty-kats.com/";
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

                if (string.IsNullOrEmpty(userSettings.sUser) || string.IsNullOrEmpty(userSettings.sPass))
                {
                    Login frmLgn = new Login();
                    frmLgn.ShowDialog(this);

                    if (!bCameThroughCorrectLogin)
                    {
                        DialogResult result = TopMostMessageBox.Show(
                            this.rm.GetString("mbExit"),
                            this.rm.GetString("mbExitTtl"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            catch (Exception)
            {
                Login frmLgn = new Login();
                frmLgn.ShowDialog(this);

                if (!bCameThroughCorrectLogin)
                {
                    DialogResult result = TopMostMessageBox.Show(
                        this.rm.GetString("mbExit"),
                        this.rm.GetString("mbExitTtl"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

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


		private static void ProcessArgs( string[] args, MainForm aForm )
		{
		    if( args.Length == 0 )
			{
				return;
			}

		    foreach (string curArg in args.Select(t => t.ToUpper()))
		    {
		        string aValue;
		        if( CheckArg( "MIN", curArg, out aValue ) )
		        {
		            aForm.WindowState = FormWindowState.Minimized;
		        }
		    }
		}

	    static bool CheckArg( string aExpected, string aSupplied, out string aSuppliedValue )
		{
			int equalsPos = aSupplied.IndexOf( "=" );
			string lSupplied;
			if( equalsPos > -1 )
			{
				aSuppliedValue = aSupplied.Substring( equalsPos + 1 );
				lSupplied = aSupplied.Substring( 0, equalsPos );
			}

			else
			{
				aSuppliedValue = string.Empty;
				lSupplied = aSupplied;
			}

			return( lSupplied == aExpected || lSupplied == "/" + aExpected || lSupplied == "\\" + aExpected );
		}


        private void MainFormLoad(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionFunction);
            Application.ThreadException += (ThreadExceptionFunction);

            LastWorkingTime = DateTime.Now;

            mrefCC = CacheController.GetInstance();
            mrefTM = ThreadManager.GetInstance();

            LoadSettings();

#if (!PGRIPPERX)
            string mbUpdate = rm.GetString("mbUpdate"), mbUpdate2 = rm.GetString("mbUpdate2");

            if (VersionCheck.UpdateAvailable() && File.Exists(Application.StartupPath + "\\ICSharpCode.SharpZipLib.dll"))
            {
                DialogResult result = TopMostMessageBox.Show(mbUpdate, mbUpdate2, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

            AutoLogin();

            if (!bCameThroughCorrectLogin)
            {
                Application.Exit();
            }

            if (userSettings.bSavePids)
            {
                LoadHistory();
            }

            // Hide Index Thread Checkbox if not RiP Forums
            if (!userSettings.sForumUrl.Contains(@"rip-productions.net"))
            {
                mIsIndexChk.Visible = false;
            }

#if (!PGRIPPERX)
            trayIcon.Visible = true;
#endif
        }
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
            if (string.IsNullOrEmpty(textBox1.Text) || this.bParseAct)
            {
                return;
            }

            if (textBox1.Text.StartsWith("http"))
            {
                comboBox1.SelectedIndex = 2;
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
            tmrPageUpdate.Enabled = true;

            if (!this.IsValidJob())
            {
                this.UnlockControls();
                return;
            }

            // Format Url
            var sHtmlUrl = UrlHandler.GetHtmlUrl(textBox1.Text, comboBox1.SelectedIndex);

            if (string.IsNullOrEmpty(sHtmlUrl))
            {
                this.UnlockControls();
                return;
            }

            // Check Post is Ripped?!
            if (userSettings.bSavePids)
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
                            this.rm.GetString("mBAlready"), "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                if (userSettings.sForumUrl.Contains(@"http://rip-") ||
                   userSettings.sForumUrl.Contains(@"http://www.rip-") ||
                userSettings.sForumUrl.Contains(@"kitty-kats.com"))
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
        /// <param name="sHtmlUrl">The Url to the Thread/Post</param>
        private void EnqueueIndexThread(string sHtmlUrl)
        {
#if (!PGRIPPERX)
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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().GetRipPageTitle(Maintainance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
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
                TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
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
                if (string.IsNullOrEmpty(Maintainance.GetInstance().GetRipPageTitle(Maintainance.GetInstance().GetPostPages(sHtmlUrl))))
                {
                    TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
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
                TopMostMessageBox.Show(sHtmlUrl.IndexOf("showthread.php") > 0 ? mNoThreadMsg : mNoPostMsg, "Info");

#if (!PGRIPPERX)
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
                {
                    URL = sHtmlUrl, 
                    HtmlPayLoad = Maintainance.GetInstance().GetPostPages(sHtmlUrl)
                };

            //job.sStorePath = sDownloadFolder;

            job.Title = Utility.ReplaceHexWithAscii(Maintainance.GetInstance().GetRipPageTitle(job.HtmlPayLoad));

            if (userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"kitty-kats.com/") ||
                userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"http://rip-") ||
                userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"http://www.rip-"))
            {
                job.SecurityToken = Maintainance.GetInstance().GetSecurityToken(job.HtmlPayLoad);
            }

            if (!sHtmlUrl.Contains(@"showthread") ||
                sHtmlUrl.Contains(@"#post"))
            {
                job.PostTitle = Utility.ReplaceHexWithAscii(Maintainance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, sHtmlUrl));

                if (userSettings.sForumUrl.Contains(@"rip-productions.net"))
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.URL, true);
                }
                else
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.HtmlPayLoad, true);

                    job.ForumTitle =
                        job.ForumTitle.Substring(job.ForumTitle.IndexOf(string.Format("{0} ", job.Title)) + job.Title.Length + 1);
                }
            }
            else
            {
                if (userSettings.sForumUrl.Contains(@"rip-productions.net"))
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.URL, false);
                }
                else
                {
                    job.ForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(job.HtmlPayLoad, false);

                    job.ForumTitle =
                        job.ForumTitle.Substring(job.ForumTitle.IndexOf(string.Format("{0} ", job.Title)) + job.Title.Length + 1);
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
        void ProcessAutoThankYou(string sPostId, int iICount, string sSecurityToken)
        {
            if (!userSettings.bAutoThank) { return; }

            if (iICount < userSettings.iMinImageCount) { return; }

            SendThankYouDelegate lSendThankYouDel = SendThankYou;

            if (sPostId == null)
            {
                return;
            }

            string tyURL;
            if (userSettings.sForumUrl.Contains(@"scanlover.com"))
            {
                tyURL = string.Format("{0}post_thanks.php?do=post_thanks_add&p={1}", userSettings.sForumUrl, sPostId);
            }
            else if (
                userSettings.sForumUrl.Contains(@"kitty-kats.com") ||
                userSettings.sForumUrl.Contains(@"http://rip-") ||
                userSettings.sForumUrl.Contains(@"http://www.rip-"))
            {
                tyURL = string.Format("{0}post_thanks.php?do=post_thanks_add&p={1}&securitytoken={2}", userSettings.sForumUrl, sPostId, sSecurityToken);
            }
            else
            {
                return;
            }

            this.Invoke(lSendThankYouDel, new object[] { tyURL });
        }

        // This delegate enables asynchronous calls for automatically sending thank yous
        delegate void SendThankYouDelegate(string aUrl);

	    static void SendThankYou
        (string aUrl)
        {
            string tyURLRef = userSettings.sForumUrl;

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
        /// </summary>
        /// <param name="sHtmlUrl">The s HTML URL.</param>
        private void ThrdGetIndexes(string sHtmlUrl)
        {
            Indexes idxs = new Indexes();

            string sPagecontent = userSettings.sForumUrl.Contains(@"rip-productions.net") ||
                                  userSettings.sForumUrl.Contains(@"kitty-kats.com")
                                      ? idxs.GetThreadPagesNew(sHtmlUrl)
                                      : idxs.GetThreadPages(sHtmlUrl);

            this.arlstIndxs = idxs.ParseHtml(sPagecontent, sHtmlUrl);
        }

        private List<ImageInfo> arlstIndxs;
        
        /// <summary>
        /// Get All Post of a Thread and Parse them as new Job
        /// </summary>
        private void ThrdGetPosts(string sHtmlUrl)
        {
            ThreadToPost threads = new ThreadToPost();

            string sPagecontent;

            if (userSettings.sForumUrl.Contains(@"http://rip-") || userSettings.sForumUrl.Contains(@"http://www.rip-") ||
                userSettings.sForumUrl.Contains(@"kitty-kats.com"))
            {
                sPagecontent = threads.GetThreadPagesNew(sHtmlUrl);
            }
            else
            {
                sPagecontent = threads.GetThreadPages(sHtmlUrl);
            }

            string sForumTitle = Maintainance.GetInstance().ExtractForumTitleFromHtml(sHtmlUrl, false);

            List<ImageInfo> arlst = threads.ParseHtml(sPagecontent);

            for (int po = 0; po < arlst.Count; po++)
            {
                StatusLabelInfo.ForeColor = Color.Green;
                StatusLabelInfo.Text = string.Format("{0}{1}/{2}", rm.GetString("gbParse"), po, arlst.Count);

                string sLpostId = arlst[po].ImageUrl;

                //////////////////////////////////////////////////////////////////////////

                if (userSettings.bSavePids && IsPostAlreadyRipped(sLpostId))
                {
                    goto SKIPIT;
                }
                //sHtmlUrl

                string sLComposedURL = userSettings.sForumUrl.Contains(@"rip-productions.net") ||
                userSettings.sForumUrl.Contains(@"kitty-kats.com")
                                           ? string.Format("{0}#post{1}", sHtmlUrl, sLpostId)
                                           : string.Format("{0}showpost.php?p={1}", userSettings.sForumUrl, sLpostId);

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
                    { URL = sLComposedURL, HtmlPayLoad = Maintainance.GetInstance().GetPostPages(sLComposedURL) };

                //job.sStorePath = sDownloadFolder;

                if (string.IsNullOrEmpty(job.HtmlPayLoad))
                {
                    goto SKIPIT;
                }

                job.Title = Maintainance.GetInstance().GetRipPageTitle(job.HtmlPayLoad);

                if (userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"kitty-kats.com") ||
                    userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"http://rip-") ||
                    userSettings.bAutoThank & userSettings.sForumUrl.Contains(@"http://www.rip-"))
                {
                    job.SecurityToken = Maintainance.GetInstance().GetSecurityToken(job.HtmlPayLoad);
                }

                job.ForumTitle = userSettings.sForumUrl.Contains(@"rip-productions.net") ||
                userSettings.sForumUrl.Contains(@"kitty-kats.com")
                                     ? sForumTitle
                                     : sForumTitle.Substring(
                                         sForumTitle.IndexOf(job.Title + " ") + job.Title.Length + 1);

                job.PostTitle = Maintainance.GetInstance().ExtractPostTitleFromHtml(job.HtmlPayLoad, sLComposedURL);
                job.Title = Utility.ReplaceHexWithAscii(job.Title);

                job.ImageList = Utility.ExtractImagesHtml(job.HtmlPayLoad, sLpostId);
                job.ImageCount = job.ImageList.Count;


                if (job.ImageCount == 0)
                {
                    goto SKIPIT;
                }

                job.StorePath = GenerateStorePath(job);

                JobListAddDelegate newJob = JobListAdd;

                Invoke(newJob, new object[] { job });

                //JobListAdd(job);

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
            if (this.bWorking && !this.bParseAct || this.mJobsList.Count > 0 && !this.bParseAct)
            {
                this.LogicCode();
            }

            if (!this.bParseAct && this.ExtractUrls.Count > 0 &&
                !GetPostsWorker.IsBusy && !GetIdxsWorker.IsBusy)
            {
                this.GetExtractUrls();
            }
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
                // Check if Last Downloadfolder is Empty
                if (!string.IsNullOrEmpty(sLastDownFolder))
                {
                    CheckCurJobFolder(sLastDownFolder);
                }

                if (!bCurPause)
                {
                    lvCurJob.Items.Clear();
                    StatusLabelImageC.Text = string.Empty;

                    if (mCurrentJob == null && mJobsList.Count > 0)
                    {
#if (!PGRIPPERX)
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                           Environment.OSVersion.Version.Major >= 6 &&
                           Environment.OSVersion.Version.Minor >= 1)
                        {
                            this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                            this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
                        }
#endif

                        // STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
                        ProcessNextJob();
                    }
                    else if (mCurrentJob == null && mJobsList.Count.Equals(0))
                    {
                        IdleRipper();
                    }
                }
            }
		}
        /// <summary>
        /// Checks the Download Folder of the Current Finished Job, if Empty delete the folder.
        /// </summary>
        /// <param name="sCheckFolder"></param>
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

            JobListRemove(0);

            string bSystemExtr = rm.GetString("bSystemExtr");

            ParseJob();

            // NO IMAGES TO PROCESS SO ABANDON CURRENT THREAD
            if (mImagesList == null || mImagesList.Count <= 0)
            {
                mCurrentJob = null;
                deleteJob.Enabled = true;
                stopCurrentThreads.Enabled = true;
                return;
            }

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

#if (!PGRIPPERX)
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

            try
            {
                progressBar1.Maximum = mImagesList.Count;
            }
            catch (Exception)
            {
                progressBar1.Maximum = 10000;
            }

            ProcessCurImgLst();
        }
        /// <summary>
        /// Processing the Images list of the Current Job
        /// </summary>
        private void ProcessCurImgLst()
        {
            stopCurrentThreads.Enabled = true;
            bStopJob = false;
            bWorking = true;

            sLastDownFolder = null;

            ThreadManager lTdm = ThreadManager.GetInstance();

            sLastDownFolder = mCurrentJob.StorePath;

            if (mImagesList.Count > 0)
            {
                string tiImagesRemain = rm.GetString("tiImagesRemain");

                ////////////////
                lvCurJob.Items.Clear();
                StatusLabelImageC.Text = string.Empty;

                for (int i = 0; i < mImagesList.Count; i++)
                {
                    lvCurJob.Items.Add(string.Format("{0}/{1} - {2}", i + 1, mImagesList.Count, mImagesList[i].ImageUrl), mImagesList[i].ImageUrl);
                }
                ///////////////////

                progressBar1.Maximum = mImagesList.Count;

                for (int i = 0; i < mImagesList.Count; i++)
                {
                    if (bStopJob || bRipperClosing)
                    {
                        break;
                    }

#if (!PGRIPPERX)
                    trayIcon.Text = String.Format(tiImagesRemain, mImagesList.Count - i, i * 100 / mImagesList.Count);

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

                    if (!bRipperClosing)
                    {
                        if (progressBar1 != null)
                        {
                            progressBar1.Value = i;
                        }

                        StatusLabelImageC.Text = String.Format(tiImagesRemain, mImagesList.Count - i,
                                                                  i * 100 / mImagesList.Count);
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
                        if (bStopJob || bRipperClosing)
                        {
                            break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        if (bStopJob || bRipperClosing)
                        {
                            break;
                        }
                    }

                    if ((i > lvCurJob.Items.Count))
                    {
                        continue;
                    }

                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(this.ShowLastPic));
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

                // FINISED A THREAD/POST DOWNLOAD JOB
                mCurrentJob = null;

                if (mJobsList.Count > 0)
                {
#if (!PGRIPPERX)
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
               Environment.OSVersion.Version.Major >= 6 &&
               Environment.OSVersion.Version.Minor >= 1)
                    {
                        this.windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
                        this.windowsTaskbar.SetOverlayIcon(Languages.english.Download, "Download");
                    }
#endif
                    // STARTING TO PROCESS NEXT THREAD IN DOWNLOAD JOBS LIST
                    ProcessNextJob();
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
        private void ShowLastPic()
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

            ListViewItem ijobJob = new ListViewItem {Text = job.Title};

            ijobJob.SubItems.Add(job.PostTitle);
            ijobJob.SubItems.Add(job.ImageCount.ToString());

            listViewJobList.Items.AddRange(new[] { ijobJob });

            groupBox5.Text = string.Format("{0} ({1}):", rm.GetString("lblRippingQue"),  mJobsList.Count);
        }
        /// <summary>
        /// Removes a Job from the Joblist and ListView
        /// </summary>
        /// <param name="iJobIndex">The Index of the Job inside the Joblist</param>
        private void JobListRemove(int iJobIndex)
        {
            mJobsList.RemoveAt(iJobIndex);

            listViewJobList.Items.RemoveAt(iJobIndex);

            groupBox5.Text = string.Format("{0} ({1}):", rm.GetString("lblRippingQue"), mJobsList.Count);
        }

        private void IdleRipper()
        {

#if (!PGRIPPERX)
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
            stopCurrentThreads.Enabled = true;
            bStopJob = false;
            bEndRip = false;
            bParseAct = false;

            lvCurJob.Items.Clear();
            StatusLabelImageC.Text = string.Empty;

            textBox1.Text = string.Empty;
            
            progressBar1.Value = 0;

            string ttlHeader = rm.GetString("ttlHeader");

            groupBox2.Text = rm.GetString("gbCurrentlyIdle");
            StatusLabelInfo.Text = rm.GetString("gbCurrentlyIdle");
            StatusLabelInfo.ForeColor = Color.Gray;

            lvCurJob.Columns[0].Text = "  ";

#if (PGRIPPER)
            Text = String.Format("PG-Ripper {0}.{1}.{2}{3}{4}", 
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"), 
                string.Format("{0}{1}\" @ {2} ]", ttlHeader, userSettings.sUser, userSettings.sForumUrl));

            trayIcon.Text = "Right click for context menu";
#elif (PGRIPPERX)
                Text = String.Format("PG-Ripper X {0}.{1}.{2}{3}{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                    string.Format("{0}{1}\" @ {2} ]", ttlHeader, userSettings.sUser, userSettings.sForumUrl));
#else
            Text = String.Format("PG-Ripper {0}.{1}.{2}{3}{4}", 
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"),
                string.Format("{0}{1}\" @ {2} ]", ttlHeader, userSettings.sUser, userSettings.sForumUrl));

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

            if (pictureBox1.BackgroundImage != null)
            {
                pictureBox1.BackgroundImage.Dispose();
                pictureBox1.BackgroundImage = null;
            }

            // Hide any image last displayed in the picturebox
            pictureBox1.Visible = false;

           

            deleteJob.Enabled = false;
            stopCurrentThreads.Enabled = false;

            bWorking = false;
        }
        /// <summary>
        /// Full HDD solution
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

            //JobListAdd(mCurrentJob);

            JobListAddDelegate updateJob = JobListAdd;

            Invoke(updateJob, new object[] { mCurrentJob });

            lvCurJob.Items.Clear();
            StatusLabelImageC.Text = string.Empty;
            mCurrentJob = null;

            UpdateDownloadFolder();

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

                mJobsList[i].StorePath = userSettings.sDownloadFolder;

                if (!userSettings.bSubDirs) continue;

                if (comboBox1.SelectedIndex == 0)
                {
                    mJobsList[i].StorePath = Path.Combine(userSettings.sDownloadFolder, mJobsList[i].Title);
                }
                if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                {
                    mJobsList[i].StorePath = Path.Combine(userSettings.sDownloadFolder, mJobsList[i].Title + Path.DirectorySeparatorChar + mJobsList[i].PostTitle);
                }
            }

            bDelete = false;
            bFullDisc = false;
        }


		//..........................................


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
                postId = Regex.Replace(postId, postId.Substring(postId.IndexOf("postcount=") -1), string.Empty);

            mImagesList = mCurrentJob.ImageList;

            ProcessAutoThankYou(postId, mImagesList.Count, mCurrentJob.SecurityToken);
		}
        /// <summary>
        /// Deletes the Selected Jobs
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopCurrentThreadsClick(object sender, EventArgs e)
        {
            if (mCurrentJob == null) return;

            stopCurrentThreads.Enabled = false;
            bStopJob = true;

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
                    Utility.SaveSetting("CurrentlyPauseThreads", "false");
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
#if (!PGRIPPERX)
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.MouseDoubleClick -= (HideClick);
                trayIcon.MouseDoubleClick += (ShowClick);

                trayMenu.MenuItems.RemoveAt(0);
                MenuItem show = new MenuItem("Show PG-Ripper", (ShowClick));
                trayMenu.MenuItems.Add(0, show);

                if (userSettings.bShowPopUps)
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
            if (pauseCurrentThreads.Text == "Resume Download")
            {
                Utility.SaveSetting("CurrentlyPauseThreads", "true");
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

            if (WindowState == FormWindowState.Minimized) return;

            Utility.SaveSetting("Window left", Left.ToString());
            Utility.SaveSetting("Window top", Top.ToString());
            Utility.SaveSetting("Window width", Width.ToString());
            Utility.SaveSetting("Window height", Height.ToString());
        }
#if (!PGRIPPERX)
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
        void CheckClipboardData()
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

                    foreach (string sClipBoardURL in sClipBoardUrLs)
                    {
                        if (!sClipBoardURL.StartsWith(userSettings.sForumUrl)) continue;

                        string sClipBoardURLNew = sClipBoardURL;

                        if (sClipBoardURLNew.Contains("\r"))
                        {
                            sClipBoardURLNew = sClipBoardURLNew.Replace("\r", "");
                        }

                        if (sClipBoardURL.Contains(@"&postcount=") || sClipBoardURL.Contains(@"&page="))
                        {
                            sClipBoardURLNew = Regex.Replace(sClipBoardURLNew, sClipBoardURLNew.Substring(sClipBoardURLNew.IndexOf("&")), "");
                        }

                        //Application.DoEvents();

                        if (this.comboBox1.SelectedIndex != 2)
                        {
                            this.comboBox1.SelectedIndex = 2;
                        }

                        if (!this.bParseAct)
                        {
                            if (this.comboBox1.SelectedIndex != 2)
                            {
                                this.comboBox1.SelectedIndex = 2;
                            }

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

            }
            catch (Exception)
            {
                Clipboard.Clear();
            }
        }
#endif
        protected override void OnLoad(EventArgs args)
      {
          base.OnLoad(args);

          Application.Idle += OnLoaded;
      }

      private void OnLoaded(object sender,
                            EventArgs args)
        {
            Application.Idle -= OnLoaded;

            tmrPageUpdate.Enabled = true;

#if (!PGRIPPERX)
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

                  ListViewItem ijobJob = new ListViewItem {Text = mJobsList[i].Title};

                  ijobJob.SubItems.Add(mJobsList[i].PostTitle);
                  ijobJob.SubItems.Add(mJobsList[i].ImageCount.ToString());
                  listViewJobList.Items.AddRange(new[] { ijobJob });
              }

              bWorking = true;

              StatusLabelInfo.Text = string.Empty;
          }

#if (!PGRIPPERX)
          nextClipboardViewer = (IntPtr)Win32.SetClipboardViewer((int)Handle);
#endif

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
                  if (!sRipUrl.StartsWith(userSettings.sForumUrl))
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
              if (this.bCurPause)
              {
                  Utility.SaveSetting("CurrentlyPauseThreads", "true");
              }
          }

          if (userSettings.bSavePids)
          {
              this.SaveHistory();
          }
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
          //bParseAct = true;
          ThrdGetPosts(Convert.ToString(e.Argument));
      }

      private void GetPostsWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
      {
          UnlockControls();
      }

      private void GetIdxsWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
      {
          //bParseAct = true;
          ThrdGetIndexes(Convert.ToString(e.Argument));
      }

      private void GetIdxsWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
      {
          UnlockControls();

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
                          this.mJobsList.Where(t => t.PostTitle.Equals(curJob.PostTitle) || Directory.Exists(path) && t.Title.Equals(curJob.Title)))
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
          bParseAct = false;

          if (InvokeRequired)
          {
              Invoke((MethodInvoker)(UnlockControlsElements));
          }
          else
          {
              UnlockControlsElements();
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
          bParseAct = true;

          textBox1.Enabled = false;
          mStartDownloadBtn.Enabled = false;

          StatusLabelInfo.Text = rm.GetString("gbParse");
          StatusLabelInfo.ForeColor = Color.Green;
      }
    }
}
