//////////////////////////////////////////////////////////////////////////
// Code Named: RiP-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace RiPRipper
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public partial class Login : Form
	{
        public ResourceManager rm;

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
            label4.Text = rm.GetString("lblWarningLogin");
            label1.Text = rm.GetString("lblUser");
            label2.Text = rm.GetString("lblPass");
            checkBox1.Text = rm.GetString("chRememberCred");
            LoginBtn.Text = rm.GetString("logintext");
            label5.Text = rm.GetString("gbLanguage");
        }
        /// <summary>
        /// Loads the Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginLoad(object sender, EventArgs e)
		{
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
            // Encrypt Password
            textBox2.Text = Utility.EncodePassword(textBox2.Text).Replace("-", string.Empty).ToLower();

            LoginManager lgnMgr = new LoginManager(textBox1.Text, textBox2.Text);
            
            string lblWelcome = rm.GetString("lblWelcome"), lblFailed = rm.GetString("lblFailed");
			
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
        /// TODO : Remove Timer
        /// If Login sucessfully send user data to MainForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Timer1Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer1.Enabled = false;
			((MainForm)Owner).bCameThroughCorrectLogin = true;

            MainForm.userSettings.User = textBox1.Text;
            MainForm.userSettings.Pass = textBox2.Text;

			Close();
		}
        /// <summary>
        /// Changes the UI Language based on the selected Language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                rm = new ResourceManager("RiPRipper.Languages.german", Assembly.GetExecutingAssembly());
                Utility.SaveSetting("UserLanguage", "de-DE");
            }
            if (comboBox2.SelectedIndex == 1)
            {
                rm = new ResourceManager("RiPRipper.Languages.french", Assembly.GetExecutingAssembly());
                Utility.SaveSetting("UserLanguage", "fr-FR");
            }
            if (comboBox2.SelectedIndex == 2)
            {
                rm = new ResourceManager("RiPRipper.Languages.english", Assembly.GetExecutingAssembly());
                Utility.SaveSetting("UserLanguage", "en-EN");
            }

            AdjustCulture();
        }
	}
}
