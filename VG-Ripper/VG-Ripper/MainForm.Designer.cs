using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;

namespace RiPRipper
{
    using RiPRipper.Objects;

    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

#if (!RIPRIPPERX)
                this.trayIcon.Dispose();
#endif

                if (components != null)
                {
                    components.Dispose();
                }
            }
            
            tmrPageUpdate.Enabled = false;
            ThreadManager.GetInstance().DismantleAllThreads();
            mJobsList.Clear();


            base.Dispose(disposing);

        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.browsFolderBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.mIsIndexChk = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.mStartDownloadBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listViewJobList = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.deleteJob = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvCurJob = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pauseCurrentThreads = new System.Windows.Forms.Button();
            this.stopCurrentThreads = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tmrPageUpdate = new System.Timers.Timer();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelImageC = new System.Windows.Forms.ToolStripStatusLabel();
            this.dfolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.GetPostsWorker = new System.ComponentModel.BackgroundWorker();
            this.GetIdxsWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmrPageUpdate)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.browsFolderBtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.mIsIndexChk);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.mStartDownloadBtn);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 144);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download Options";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // browsFolderBtn
            // 
            this.browsFolderBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browsFolderBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browsFolderBtn.Location = new System.Drawing.Point(291, 50);
            this.browsFolderBtn.Name = "browsFolderBtn";
            this.browsFolderBtn.Size = new System.Drawing.Size(56, 20);
            this.browsFolderBtn.TabIndex = 3;
            this.browsFolderBtn.Text = "Browse";
            this.browsFolderBtn.UseCompatibleTextRendering = true;
            this.browsFolderBtn.Click += new System.EventHandler(this.BrowsFolderBtnClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 45;
            this.label2.Text = "Download Folder :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.UseCompatibleTextRendering = true;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(120, 50);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(162, 20);
            this.textBox2.TabIndex = 2;
            // 
            // mIsIndexChk
            // 
            this.mIsIndexChk.Enabled = false;
            this.mIsIndexChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mIsIndexChk.Location = new System.Drawing.Point(120, 76);
            this.mIsIndexChk.MaximumSize = new System.Drawing.Size(200, 16);
            this.mIsIndexChk.Name = "mIsIndexChk";
            this.mIsIndexChk.Size = new System.Drawing.Size(200, 16);
            this.mIsIndexChk.TabIndex = 4;
            this.mIsIndexChk.Text = "Babe Index or Website Index?";
            this.mIsIndexChk.UseCompatibleTextRendering = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.ItemHeight = 13;
            this.comboBox1.Items.AddRange(new object[] {
            "Thread ID:",
            "Post ID:",
            "URL:"});
            this.comboBox1.Location = new System.Drawing.Point(13, 23);
            this.comboBox1.MaximumSize = new System.Drawing.Size(88, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(88, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // mStartDownloadBtn
            // 
            this.mStartDownloadBtn.Image = ((System.Drawing.Image)(resources.GetObject("mStartDownloadBtn.Image")));
            this.mStartDownloadBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mStartDownloadBtn.Location = new System.Drawing.Point(123, 98);
            this.mStartDownloadBtn.MaximumSize = new System.Drawing.Size(224, 34);
            this.mStartDownloadBtn.Name = "mStartDownloadBtn";
            this.mStartDownloadBtn.Size = new System.Drawing.Size(224, 34);
            this.mStartDownloadBtn.TabIndex = 5;
            this.mStartDownloadBtn.Text = "Start Download";
            this.mStartDownloadBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mStartDownloadBtn.UseCompatibleTextRendering = true;
            this.mStartDownloadBtn.Click += new System.EventHandler(this.MStartDownloadBtnClick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(120, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(226, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1KeyPress);
            // 
            // listViewJobList
            // 
            this.listViewJobList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewJobList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader1});
            this.listViewJobList.FullRowSelect = true;
            this.listViewJobList.Location = new System.Drawing.Point(6, 23);
            this.listViewJobList.Name = "listViewJobList";
            this.listViewJobList.Size = new System.Drawing.Size(428, 507);
            this.listViewJobList.TabIndex = 6;
            this.listViewJobList.UseCompatibleStateImageBehavior = false;
            this.listViewJobList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Thread";
            this.columnHeader8.Width = 180;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Post";
            this.columnHeader9.Width = 175;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Pics";
            // 
            // deleteJob
            // 
            this.deleteJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteJob.Image = ((System.Drawing.Image)(resources.GetObject("deleteJob.Image")));
            this.deleteJob.Location = new System.Drawing.Point(440, 23);
            this.deleteJob.MaximumSize = new System.Drawing.Size(24, 24);
            this.deleteJob.MinimumSize = new System.Drawing.Size(24, 24);
            this.deleteJob.Name = "deleteJob";
            this.deleteJob.Size = new System.Drawing.Size(24, 24);
            this.deleteJob.TabIndex = 7;
            this.deleteJob.Click += new System.EventHandler(this.DeleteJobClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lvCurJob);
            this.groupBox2.Controls.Add(this.pauseCurrentThreads);
            this.groupBox2.Controls.Add(this.stopCurrentThreads);
            this.groupBox2.Location = new System.Drawing.Point(13, 179);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 158);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Currently,";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // lvCurJob
            // 
            this.lvCurJob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCurJob.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvCurJob.FullRowSelect = true;
            this.lvCurJob.HideSelection = false;
            this.lvCurJob.Location = new System.Drawing.Point(12, 20);
            this.lvCurJob.MultiSelect = false;
            this.lvCurJob.Name = "lvCurJob";
            this.lvCurJob.ShowGroups = false;
            this.lvCurJob.Size = new System.Drawing.Size(333, 97);
            this.lvCurJob.TabIndex = 10;
            this.lvCurJob.UseCompatibleStateImageBehavior = false;
            this.lvCurJob.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "  ";
            this.columnHeader2.Width = 322;
            // 
            // pauseCurrentThreads
            // 
            this.pauseCurrentThreads.Image = ((System.Drawing.Image)(resources.GetObject("pauseCurrentThreads.Image")));
            this.pauseCurrentThreads.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pauseCurrentThreads.Location = new System.Drawing.Point(61, 128);
            this.pauseCurrentThreads.MaximumSize = new System.Drawing.Size(129, 24);
            this.pauseCurrentThreads.Name = "pauseCurrentThreads";
            this.pauseCurrentThreads.Size = new System.Drawing.Size(129, 24);
            this.pauseCurrentThreads.TabIndex = 8;
            this.pauseCurrentThreads.Text = "Pause Download(s)";
            this.pauseCurrentThreads.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pauseCurrentThreads.UseCompatibleTextRendering = true;
            this.pauseCurrentThreads.UseVisualStyleBackColor = true;
            this.pauseCurrentThreads.Click += new System.EventHandler(this.PauseCurrentThreadsClick);
            // 
            // stopCurrentThreads
            // 
            this.stopCurrentThreads.Image = ((System.Drawing.Image)(resources.GetObject("stopCurrentThreads.Image")));
            this.stopCurrentThreads.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stopCurrentThreads.Location = new System.Drawing.Point(197, 128);
            this.stopCurrentThreads.MaximumSize = new System.Drawing.Size(149, 24);
            this.stopCurrentThreads.Name = "stopCurrentThreads";
            this.stopCurrentThreads.Size = new System.Drawing.Size(149, 24);
            this.stopCurrentThreads.TabIndex = 9;
            this.stopCurrentThreads.Text = "Stop/Delete Job";
            this.stopCurrentThreads.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.stopCurrentThreads.UseCompatibleTextRendering = true;
            this.stopCurrentThreads.Click += new System.EventHandler(this.StopCurrentThreadsClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(6, 19);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(345, 189);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(348, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.PictureBox1DoubleClick);
            // 
            // tmrPageUpdate
            // 
            this.tmrPageUpdate.Interval = 200D;
            this.tmrPageUpdate.SynchronizingObject = this;
            this.tmrPageUpdate.Elapsed += new System.Timers.ElapsedEventHandler(this.TmrPageUpdateElapsed);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(857, 24);
            this.menuStrip.TabIndex = 7;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem1.Text = "&Settings";
            this.settingsToolStripMenuItem1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsToolStripMenuItem1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.SettingsToolStripMenuItem1Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exitToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.settingsToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem.Text = "&About";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar1,
            this.StatusLabelInfo,
            this.StatusLabelImageC});
            this.statusStrip1.Location = new System.Drawing.Point(0, 571);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(857, 23);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(364, 17);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // StatusLabelInfo
            // 
            this.StatusLabelInfo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.StatusLabelInfo.ForeColor = System.Drawing.Color.Red;
            this.StatusLabelInfo.Name = "StatusLabelInfo";
            this.StatusLabelInfo.Size = new System.Drawing.Size(13, 18);
            this.StatusLabelInfo.Text = "  ";
            // 
            // StatusLabelImageC
            // 
            this.StatusLabelImageC.Name = "StatusLabelImageC";
            this.StatusLabelImageC.Size = new System.Drawing.Size(13, 18);
            this.StatusLabelImageC.Text = "  ";
            this.StatusLabelImageC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.pictureBox1);
            this.groupBox4.Location = new System.Drawing.Point(13, 343);
            this.groupBox4.MinimumSize = new System.Drawing.Size(358, 214);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(360, 225);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Last Picture Downloaded...";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.listViewJobList);
            this.groupBox5.Controls.Add(this.deleteJob);
            this.groupBox5.Location = new System.Drawing.Point(378, 32);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(471, 536);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ripping Queue:";
            // 
            // GetPostsWorker
            // 
            this.GetPostsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.GetPostsWorkerDoWork);
            this.GetPostsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.GetPostsWorkerCompleted);
            // 
            // GetIdxsWorker
            // 
            this.GetIdxsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.GetIdxsWorkerDoWork);
            this.GetIdxsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.GetIdxsWorkerCompleted);
            // 
            // MainForm
            // 
            this.AccessibleDescription = "i";
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(857, 594);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(865, 622);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ViperGirls Ripper 2.0.0  [Logged in as: \"\"]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.Resize += new System.EventHandler(this.MainFormResize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmrPageUpdate)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private string mInvalidDestinationMsg;
        private string mIncorrectUrlMsg;
        private string mNoThreadMsg;
        private string mNoPostMsg;
        private string mAlreadyQueuedMsg;
        private string mTNumericMsg;

#if (RIPRIPPER)
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
#elif (RIPRIPPERX)
            
#else
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
#endif


        private int NCProcessorPort;
        private NetCommandProcessor mNCProcessor;

        public int ExitOnIdleTimeout;
        private DateTime LastWorkingTime;

        private Timer mIdleTimer;

        private GroupBox groupBox1;
        private TextBox textBox1;
        private Button deleteJob;
        private GroupBox groupBox2;
        private Button mStartDownloadBtn;
        private PictureBox pictureBox1;

        private List<JobInfo> mJobsList = null;
        private System.Timers.Timer tmrPageUpdate;
        private JobInfo mCurrentJob = null;
        private List<ImageInfo> mImagesList = null;
        private CacheController cacheController = null;
        private ThreadManager mrefTM = null;
        public bool bCameThroughCorrectLogin = false;
        public bool bKeepPicHidden = false;
        private ComboBox comboBox1;
        private Button stopCurrentThreads;

        //private string g_sComposedURI = string.Empty;
        private CheckBox mIsIndexChk;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ListView listViewJobList;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar progressBar1;
        private Button pauseCurrentThreads;
        private ToolStripStatusLabel StatusLabelInfo;
        private ColumnHeader columnHeader1;
        private Button browsFolderBtn;
        private Label label2;
        private TextBox textBox2;
        private FolderBrowserDialog dfolderBrowserDialog;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private ToolStripStatusLabel StatusLabelImageC;
        private ListView lvCurJob;
        private ColumnHeader columnHeader2;
        private System.ComponentModel.BackgroundWorker GetPostsWorker;
        private System.ComponentModel.BackgroundWorker GetIdxsWorker;
    }
}
