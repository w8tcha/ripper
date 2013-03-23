// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginManager.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The hot sauce for Login management onto the RiP Forums.
    /// Sends basic login credentials and if successfull, commits 
    /// cookies to hashtable.
    /// If operation successfull, true is returned, otherwise false.
    /// </summary>
    public class LoginManager
    {
        /// <summary>
        /// The user name
        /// </summary>
        private readonly string sUsr;

        /// <summary>
        /// The user password
        /// </summary>
        private readonly string sPass;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginManager"/> class.
        /// </summary>
        /// <param name="sUserName">Name of the s user.</param>
        /// <param name="sPassword">The s password.</param>
        public LoginManager(string sUserName, string sPassword)
        {
            this.sUsr = sUserName;
            this.sPass = sPassword;
        }

        /// <summary>
        /// Does the login.
        /// </summary>
        /// <returns>
        /// Returns value if the Login was successfully
        /// </returns>
        public bool DoLogin()
        {
            var loginURL = string.Format("{0}login.php", CacheController.Instance().UserSettings.ForumURL);

            var postData =
                "do=login&forceredirect=1&url=%2Fforum%2F&vb_login_md5password=%md5pass%&vb_login_md5password_utf=%md5pass%&s=&vb_login_username=%md5user%&vb_login_password=&cookieuser=1";

            ////string hashedPassword = Utility.EncodePassword(sPass).Replace("-", "").ToLower();

            // sPostData = sPostData.Replace("%md5pass%", hashedPassword);
            postData = postData.Replace("%md5pass%", this.sPass);
            postData = postData.Replace("%md5user%", this.sUsr);

            bool loginSuccessfully = false;

            try
            {
                var cookieContainer = new CookieContainer();

                var req = (HttpWebRequest)WebRequest.Create(loginURL);

                req.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0";
                req.CookieContainer = cookieContainer;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.KeepAlive = true;

                var encoding = new ASCIIEncoding();
                byte[] loginDataBytes = encoding.GetBytes(postData);

                req.ContentLength = loginDataBytes.Length;

                var stream = req.GetRequestStream();
                stream.Write(loginDataBytes, 0, loginDataBytes.Length);
                stream.Close();

                var res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                var cookieMgr = CookieManager.GetInstance();

                ////cookieMgr.DeleteCookie("bb_userid");
                ////cookieMgr.DeleteCookie("bb_password");

                foreach (Cookie cook in res.Cookies)
                {
                    cookieMgr.SetCookie(cook.Name, cook.Value);

                    if (cook.Name.Contains("userid") && !string.IsNullOrEmpty(cook.Value))
                    {
                        loginSuccessfully = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }

            return loginSuccessfully;
        }
    }
}