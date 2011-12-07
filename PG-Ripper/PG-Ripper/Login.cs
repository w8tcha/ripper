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
    using System.Drawing;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    /// <summary>
    /// Summary description for Login.
    /// </summary>
    public partial class Login : Form
    {
        private ResourceManager rm;

        public Login()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Set Language Strings
        /// </summary>
        private void AdjustCulture()
        {
            groupBox1.Text = rm.GetString("gbLoginHead");
            label1.Text = rm.GetString("lblUser");
            label2.Text = rm.GetString("lblPass");
            checkBox1.Text = rm.GetString("chRememberCred");
            LoginBtn.Text = rm.GetString("logintext");
            label5.Text = rm.GetString("gbLanguage");
            label6.Text = rm.GetString("lblForums");
        }

        /// <summary>
        /// Loads the Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginLoad(object sender, EventArgs e)
        {
            // Set Default Forum
            cBForum.SelectedIndex = 0;

            // Load Language Setting
            try
            {
                string sLanguage = Utility.LoadSetting("UserLanguage");

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtnClick(object sender, EventArgs e)
        {
            if (textBox3.Text.StartsWith("http://"))
            {
                if (!textBox3.Text.EndsWith("/"))
                {
                    textBox3.Text += "/";
                }

                Utility.SaveSetting("forumURL", textBox3.Text);
            }

            // Encrypt Password
            textBox2.Text = Utility.EncodePassword(textBox2.Text).Replace("-", string.Empty).ToLower();

            LoginManager lgnMgr = new LoginManager(textBox1.Text, textBox2.Text);

            string lblWelcome = this.rm.GetString("lblWelcome"), lblFailed = this.rm.GetString("lblFailed");

            if (lgnMgr.DoLogin())
            {
                label3.Text = lblWelcome + textBox1.Text;
                label3.ForeColor = Color.Green;
                LoginBtn.Enabled = false;

                Utility.SaveSetting("User", textBox1.Text);
                Utility.SaveSetting("Password", textBox2.Text);

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Enabled = false;

            ((MainForm)Owner).bCameThroughCorrectLogin = true;

            MainForm.userSettings.sUser = textBox1.Text;
            MainForm.userSettings.sPass = textBox2.Text;


            Close();
        }

        /// <summary>
        /// Changes the UI Language based on the selected Language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    this.rm = new ResourceManager("PGRipper.Languages.german", Assembly.GetExecutingAssembly());
                    Utility.SaveSetting("UserLanguage", "de-DE");
                    break;
                case 1:
                    this.rm = new ResourceManager("PGRipper.Languages.french", Assembly.GetExecutingAssembly());
                    Utility.SaveSetting("UserLanguage", "fr-FR");
                    break;
                case 2:
                    this.rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                    Utility.SaveSetting("UserLanguage", "en-EN");
                    break;
                default:
                    this.rm = new ResourceManager("PGRipper.Languages.english", Assembly.GetExecutingAssembly());
                    Utility.SaveSetting("UserLanguage", "en-EN");
                    break;
            }

            this.AdjustCulture();
        }

        /// <summary>
        /// Forum Chooser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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