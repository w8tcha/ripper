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

namespace Ripper.Core.Components
{
    using System;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The hot sauce for Login management onto the Forums.
    /// Sends basic login credentials and if succesfully, commits 
    /// cookies to hash table.
    /// If operation succesfully, true is returned, otherwise false.
    /// </summary>
    public class LoginManager
    {
        /// <summary>
        /// The user name
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// The user password
        /// </summary>
        private readonly string _userPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginManager"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public LoginManager(string userName, string password)
        {
            this._userName = userName;
            this._userPassword = password;
        }

        /// <summary>
        /// Does the login.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// Returns value if the Login was successfully
        /// </returns>
        public bool DoLogin(string url)
        {
            var loginURL = string.Format("{0}login.php", url);

            var postData =
                "do=login&forceredirect=1&url=%2Fforum%2F&vb_login_md5password=%md5pass%&vb_login_md5password_utf=%md5pass%&s=&vb_login_username=%md5user%&vb_login_password=&cookieuser=1";

            postData = postData.Replace("%md5pass%", this._userPassword);
            postData = postData.Replace("%md5user%", this._userName);

            var loginSuccessfully = false;

            try
            {
                var cookieContainer = new CookieContainer();

                var req = (HttpWebRequest)WebRequest.Create(loginURL);

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