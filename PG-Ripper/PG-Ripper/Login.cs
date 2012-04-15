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
            InitializeComponent();
        }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            this.groupBox1.Text = this.rm.GetString("gbLoginHead");
            this.label1.Text = this.rm.GetString("lblUser");
            this.label2.Text = this.rm.GetString("lblPass");
            this.checkBox1.Text = this.rm.GetString("chRememberCred");
            this.LoginBtn.Text = this.rm.GetString("logintext");
            this.label5.Text = this.rm.GetString("gbLanguage");
            this.label6.Text = this.rm.GetString("lblForums");
        }

        /// <summary>
        /// Loads the Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginLoad(object sender, EventArgs e)
        {
            // Set Default Forum
            cBForum.SelectedIndex = 0;

            // Load Language Setting
            try
            {
                string sLanguage = CacheController.Xform.userSettings.Language;

                switch (sLanguage)
                {
                    case "de-DE":
                        comboBox2.SelectedIndex = 0;
                        break;
                    case "fr-FR":
                        comboBox2.SelectedIndex = 1;
                        break;
                    case "en-EN":
                        comboBox2.SelectedIndex = 2;
                        break;
                    default:
                        comboBox2.SelectedIndex = 2;
                        break;
                }
            }
            catch (Exception)
            {
                comboBox2.SelectedIndex = 2;
            }
        }

        /// <summary>
        /// Trys to Login to the Forums
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginBtnClick(object sender, EventArgs e)
        {
            if (textBox3.Text.StartsWith("http://"))
            {
                if (!textBox3.Text.EndsWith("/"))
                {
                    textBox3.Text += "/";
                }

                CacheController.Xform.userSettings.CurrentForumUrl = textBox3.Text;
            }

            // Encrypt Password
            textBox2.Text = Utility.EncodePassword(textBox2.Text).Replace("-", string.Empty).ToLower();

            LoginManager lgnMgr = new LoginManager(textBox1.Text, textBox2.Text);

            string lblWelcome = this.rm.GetString("lblWelcome"), lblFailed = this.rm.GetString("lblFailed");

            if (lgnMgr.DoLogin())
            {
                label3.Text = string.Format("{0}{1}", lblWelcome, this.textBox1.Text);
                label3.ForeColor = Color.Green;
                LoginBtn.Enabled = false;

                if (CacheController.Xform.userSettings.ForumsAccount.Any(item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl))
                {
                    CacheController.Xform.userSettings.ForumsAccount.RemoveAll(
                       item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl);
                }

                var forumsAccount = new ForumAccount
                {
                    ForumURL = CacheController.Xform.userSettings.CurrentForumUrl,
                    UserName = this.textBox1.Text,
                    UserPassWord = this.textBox2.Text
                };

                CacheController.Xform.userSettings.ForumsAccount.Add(forumsAccount);
                CacheController.Xform.userSettings.CurrentUserName = this.textBox1.Text;

                timer1.Enabled = true;
            }
            else
            {
                label3.Text = lblFailed;
                label3.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// If Login sucessfully send user data to MainForm
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer1Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Enabled = false;

            CacheController.Xform.bCameThroughCorrectLogin = true;

            if (CacheController.Xform.userSettings.ForumsAccount.Any(item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl))
            {
                CacheController.Xform.userSettings.ForumsAccount.RemoveAll(
                   item => item.ForumURL == CacheController.Xform.userSettings.CurrentForumUrl);
            }

            var forumsAccount = new ForumAccount
            {
                ForumURL = CacheController.Xform.userSettings.CurrentForumUrl,
                UserName = this.textBox1.Text,
                UserPassWord = this.textBox2.Text
            };

            CacheController.Xform.userSettings.ForumsAccount.Add(forumsAccount);
            CacheController.Xform.userSettings.CurrentUserName = this.textBox1.Text;
            
            Close();
        }

        /// <summary>
        /// Changes the UI Language based on the selected Language
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
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
            switch (cBForum.SelectedIndex)
            {
                case 1:
                    textBox3.Text = "http://www.kitty-kats.com/";
                    break;
                case 2:
                    textBox3.Text = "http://rip-productions.net/";
                    break;
                case 3:
                    textBox3.Text = "http://forums.sexyandfunny.com/";
                    break;
                case 4:
                    textBox3.Text = "http://forum.scanlover.com/";
                    break;
                case 5:
                    textBox3.Text = "http://...";
                    break;
                default:
                    textBox3.Text = "http://...";
                    break;
            }
        }
    }
}