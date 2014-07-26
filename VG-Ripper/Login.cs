// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    using Ripper.Core.Components;

    /// <summary>
    /// The Login Dialog
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Resource Manager
        /// </summary>
        private ResourceManager _ResourceManager { get; set; }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            this.groupBox1.Text = this._ResourceManager.GetString("gbLoginHead");
            this.groupBox2.Text = this._ResourceManager.GetString("gbGuestLoginHead");

            this.label1.Text = this._ResourceManager.GetString("lblUser");
            this.label2.Text = this._ResourceManager.GetString("lblPass");
            this.label4.Text = this._ResourceManager.GetString("lblWarningLogin");
            this.label5.Text = this._ResourceManager.GetString("gbLanguage");

            this.checkBox1.Text = this._ResourceManager.GetString("chRememberCred");

            this.LoginBtn.Text = this._ResourceManager.GetString("logintext");
            this.GuestLoginButton.Text = this._ResourceManager.GetString("guestLoginButton");
        }

        /// <summary>
        /// Load Language Settings
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginLoad(object sender, EventArgs e)
        {
            try
            {
                var language = SettingsHelper.LoadSetting("UserLanguage");

                switch (language)
                {
                    case "de-DE":
                        this.comboBox2.SelectedIndex = 0;
                        break;
                    case "fr-FR":
                        this.comboBox2.SelectedIndex = 1;
                        break;
                    case "en-EN":
                        this.comboBox2.SelectedIndex = 2;
                        break;
                    /*case "zh-CN":
                        this.comboBox2.SelectedIndex = 3;
                        break;*/
                    default:
                        this.comboBox2.SelectedIndex = 2;
                        break;
                }
            }
            catch (Exception)
            {
                this.comboBox2.SelectedIndex = 2;
            }
        }

        /// <summary>
        /// Try to Login to the Forums
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginBtnClick(object sender, EventArgs e)
        {
            // Encrypt Password
            this.textBox2.Text = Utility.EncodePassword(this.textBox2.Text).Replace("-", string.Empty).ToLower();

            var loginManager = new LoginManager(this.textBox1.Text, this.textBox2.Text);

            var welcomeMessage = this._ResourceManager.GetString("lblWelcome");
            var failedMessage = this._ResourceManager.GetString("lblFailed");

            if (loginManager.DoLogin(CacheController.Instance().UserSettings.ForumURL))
            {
                this.label3.Text = string.Format("{0}{1}", welcomeMessage, this.textBox1.Text);
                this.label3.ForeColor = Color.Green;
                this.LoginBtn.Enabled = false;

                SettingsHelper.SaveSetting("User", this.textBox1.Text);
                SettingsHelper.SaveSetting("Password", this.textBox2.Text);

                this.timer1.Enabled = true;
            }
            else
            {
                this.label3.Text = failedMessage;
                this.label3.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// TODO : Remove Timer
        /// If Login successfully send user data to MainForm
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer1Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.timer1.Enabled = false;
            ((MainForm)Owner).cameThroughCorrectLogin = true;

            var cacheController = CacheController.Instance();

            cacheController.UserSettings.User = this.textBox1.Text;
            cacheController.UserSettings.Pass = this.textBox2.Text;

            this.Close();
        }

        /// <summary>
        /// Changes the UI Language based on the selected Language
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    this._ResourceManager = new ResourceManager("Ripper.Languages.german", Assembly.GetExecutingAssembly());
                    SettingsHelper.SaveSetting("UserLanguage", "de-DE");
                    break;
                case 1:
                    this._ResourceManager = new ResourceManager("Ripper.Languages.french", Assembly.GetExecutingAssembly());
                    SettingsHelper.SaveSetting("UserLanguage", "fr-FR");
                    break;
                case 2:
                    this._ResourceManager = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                    SettingsHelper.SaveSetting("UserLanguage", "en-EN");
                    break;
                /*case 3:
                    this._ResourceManager = new ResourceManager("Ripper.Languages.chinese-cn", Assembly.GetExecutingAssembly());
                    SettingsHelper.SaveSetting("UserLanguage", "zh-CN");
                    break;*/
            }

            this.AdjustCulture();
        }

        /// <summary>
        /// Login as Guest
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GuestLoginButton_Click(object sender, EventArgs e)
        {
            ((MainForm)Owner).cameThroughCorrectLogin = true;

            var cacheController = CacheController.Instance();

            cacheController.UserSettings.GuestMode = true;

            SettingsHelper.SaveSetting("GuestMode", cacheController.UserSettings.GuestMode.ToString());

            this.Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the RegisterLink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void RegisterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(string.Format("{0}register.php", CacheController.Instance().UserSettings.ForumURL));
        }
    }
}