// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.cs" company="The Watcher">
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
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    using PGRipper.Objects;

    /// <summary>
    /// The Login Dialog
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// The Resource Manger Instance
        /// </summary>
        private ResourceManager rm;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            this.groupBox1.Text = this.rm.GetString("gbLoginHead");
            this.label1.Text = this.rm.GetString("lblUser");
            this.label2.Text = this.rm.GetString("lblPass");
            this.LoginButton.Text = this.rm.GetString("logintext");
            this.label5.Text = this.rm.GetString("gbLanguage");
            this.label6.Text = this.rm.GetString("lblForums");
            this.GuestLogin.Text = this.rm.GetString("GuestLogin");
        }

        /// <summary>
        /// Loads the Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginLoad(object sender, EventArgs e)
        {
            // Set Default Forum
            this.ForumList.SelectedIndex = 0;

            // Load Language Setting
            try
            {
                string sLanguage = CacheController.Xform.userSettings.Language;

                switch (sLanguage)
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
        /// Tries to Login to the Forums
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginBtnClick(object sender, EventArgs e)
        {
            if (this.ForumUrl.Text.StartsWith("http://"))
            {
                if (!this.ForumUrl.Text.EndsWith("/"))
                {
                    this.ForumUrl.Text += "/";
                }

                CacheController.Xform.userSettings.CurrentForumUrl = this.ForumUrl.Text;
            }

            string welcomeString = this.rm.GetString("lblWelcome"), lblFailed = this.rm.GetString("lblFailed");

            if (this.GuestLogin.Checked)
            {
                this.UserNameField.Text = "Guest";
                this.PasswordField.Text = "Guest";

                this.label3.Text = string.Format("{0}{1}", welcomeString, this.UserNameField.Text);
                this.label3.ForeColor = Color.Green;
                this.LoginButton.Enabled = false;

                if (
                    CacheController.Xform.userSettings.ForumsAccount.Any(
                        item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl))
                {
                    CacheController.Xform.userSettings.ForumsAccount.RemoveAll(
                        item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl);
                }

                var forumsAccount = new ForumAccount
                {
                    ForumURL = CacheController.Xform.userSettings.CurrentForumUrl,
                    UserName = this.UserNameField.Text,
                    UserPassWord = this.PasswordField.Text,
                    GuestAccount = false
                };

                CacheController.Xform.userSettings.ForumsAccount.Add(forumsAccount);
                CacheController.Xform.userSettings.CurrentUserName = this.UserNameField.Text;

                this.timer1.Enabled = true;
            }
            else
            {
                // Encrypt Password
                this.PasswordField.Text =
                    Utility.EncodePassword(this.PasswordField.Text).Replace("-", string.Empty).ToLower();

                var loginManager = new LoginManager(this.UserNameField.Text, this.PasswordField.Text);

                if (loginManager.DoLogin())
                {
                    this.label3.Text = string.Format("{0}{1}", welcomeString, this.UserNameField.Text);
                    this.label3.ForeColor = Color.Green;
                    this.LoginButton.Enabled = false;

                    if (
                        CacheController.Xform.userSettings.ForumsAccount.Any(
                            item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl))
                    {
                        CacheController.Xform.userSettings.ForumsAccount.RemoveAll(
                            item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl);
                    }

                    var forumsAccount = new ForumAccount
                                            {
                                                ForumURL = CacheController.Xform.userSettings.CurrentForumUrl,
                                                UserName = this.UserNameField.Text,
                                                UserPassWord = this.PasswordField.Text,
                                                GuestAccount = false
                                            };

                    CacheController.Xform.userSettings.ForumsAccount.Add(forumsAccount);
                    CacheController.Xform.userSettings.CurrentUserName = this.UserNameField.Text;

                    this.timer1.Enabled = true;
                }
                else
                {
                    this.label3.Text = lblFailed;
                    this.label3.ForeColor = Color.Red;
                }
            }
        }

        /// <summary>
        /// If Login successfully send user data to MainForm
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer1Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.timer1.Enabled = false;

            CacheController.Xform.bCameThroughCorrectLogin = true;

            if (CacheController.Xform.userSettings.ForumsAccount.Any(item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl))
            {
                CacheController.Xform.userSettings.ForumsAccount.RemoveAll(
                   item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl);
            }

            var forumsAccount = new ForumAccount
            {
                ForumURL = CacheController.Xform.userSettings.CurrentForumUrl,
                UserName = this.UserNameField.Text,
                UserPassWord = this.PasswordField.Text,
                GuestAccount = this.GuestLogin.Checked
            };

            CacheController.Xform.userSettings.ForumsAccount.Add(forumsAccount);
            CacheController.Xform.userSettings.CurrentUserName = this.UserNameField.Text;

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
                    this.rm = new ResourceManager("PGRipper.Languages.german", Assembly.GetExecutingAssembly());
                    CacheController.Xform.userSettings.Language = "de-DE";
                    break;
                case 1:
                    this.rm = new ResourceManager("PGRipper.Languages.french", Assembly.GetExecutingAssembly());
                    CacheController.Xform.userSettings.Language = "fr-FR";
                    break;
                case 2:
                    this.rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                    CacheController.Xform.userSettings.Language = "en-EN";
                    break;
                default:
                    this.rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                    CacheController.Xform.userSettings.Language = "en-EN";
                    break;
            }

            this.AdjustCulture();
        }

        /// <summary>
        /// Forum Chooser
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CBForumSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.ForumList.SelectedIndex)
            {
                case 1:
                    this.ForumUrl.Text = "http://www.kitty-kats.net/";
                    break;
                case 2:
                    this.ForumUrl.Text = "http://vipergirls.to/";
                    break;
                case 3:
                    this.ForumUrl.Text = "http://forums.sexyandfunny.com/";
                    break;
                case 4:
                    this.ForumUrl.Text = "http://forum.scanlover.com/";
                    break;
                case 5:
                    this.ForumUrl.Text = "http://bignaturalsonly.com/";
                    break;
                case 6:
                    this.ForumUrl.Text = "http://forum.phun.org/";
                    break;
                case 7:
                    this.ForumUrl.Text = "http://...";
                    break;
                default:
                    this.ForumUrl.Text = "http://...";
                    break;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the GuestLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GuestLogin_CheckedChanged(object sender, EventArgs e)
        {
            this.UserNameField.Enabled = !this.GuestLogin.Checked;
            this.PasswordField.Enabled = !this.GuestLogin.Checked;
        }
    }
}