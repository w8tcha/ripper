// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginManager.cs" company="The Watcher">
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
    using System.IO;
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
            const string LoginURL = "http://rip-productions.net/login.php";
            string sPostData =
                "do=login&forceredirect=1&url=%2Fforum%2F&vb_login_md5password=%md5pass%&vb_login_md5password_utf=%md5pass%&s=&vb_login_username=%md5user%&vb_login_password=&cookieuser=1";

            ////string hashedPassword = Utility.EncodePassword(sPass).Replace("-", "").ToLower();

            // sPostData = sPostData.Replace("%md5pass%", hashedPassword);
            sPostData = sPostData.Replace("%md5pass%", this.sPass);
            sPostData = sPostData.Replace("%md5user%", this.sUsr);

            bool bFinalRet = false;

            try
            {
                var cookieContainer = new CookieContainer();

                var req = (HttpWebRequest)WebRequest.Create(LoginURL);

                req.UserAgent =
                    "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.CookieContainer = cookieContainer;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.KeepAlive = true;

                var encoding = new ASCIIEncoding();
                byte[] loginDataBytes = encoding.GetBytes(sPostData);

                req.ContentLength = loginDataBytes.Length;

                Stream stream = req.GetRequestStream();
                stream.Write(loginDataBytes, 0, loginDataBytes.Length);
                stream.Close();

                var res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                CookieManager cookieMgr = CookieManager.GetInstance();

                ////cookieMgr.DeleteCookie("bb_userid");
                ////cookieMgr.DeleteCookie("bb_password");

                foreach (Cookie cook in res.Cookies)
                {
                    cookieMgr.SetCookie(cook.Name, cook.Value);

                    if (cook.Name.Contains("userid") && !string.IsNullOrEmpty(cook.Value))
                    {
                        bFinalRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }

            return bFinalRet;
        }
    }
}