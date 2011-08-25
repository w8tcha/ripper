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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace PGRipper
{
	/// <summary>
	/// The hot sauce for Login management onto the RiP Forums.
	/// 
	/// Sends basic login credentials and if successfull, commits 
	/// cookies to hashtable.
	/// 
	/// If operation successfull, true is returned, otherwise false.
	/// </summary>
	public class LoginManager
	{
	    readonly string sUsr;
	    readonly string sPass;

	    public LoginManager(string sUserName, string sPassword)
		{
			sUsr = sUserName;
			sPass = sPassword;
		}

		public bool DoLogin()
		{
            string strURL = Utility.LoadSetting("forumURL") + "login.php";
			string sPostData = "do=login&forceredirect=1&url=%2Fforum%2F&vb_login_md5password=%md5pass%&vb_login_md5password_utf=%md5pass%&s=&vb_login_username=%md5user%&vb_login_password=&cookieuser=1";

            //string hashedPassword = Utility.EncodePassword(sPass).Replace("-", "").ToLower();

            // sPostData = sPostData.Replace("%md5pass%", hashedPassword);
            sPostData = sPostData.Replace("%md5pass%", sPass); 
			sPostData = sPostData.Replace("%md5user%", sUsr );

			bool bFinalRet = false;

			try
			{
                CookieContainer cookieContainer = new CookieContainer();

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.CookieContainer = cookieContainer;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ProtocolVersion = HttpVersion.Version10;
                req.KeepAlive = true;

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] loginDataBytes = encoding.GetBytes(sPostData);
                
                req.ContentLength = loginDataBytes.Length;
                
                Stream stream = req.GetRequestStream();
                stream.Write(loginDataBytes, 0, loginDataBytes.Length);
                stream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                CookieManager cookieMgr = CookieManager.GetInstance();

                cookieMgr.DeleteCookie("bbuserid");
                cookieMgr.DeleteCookie("bbpassword");

                foreach (Cookie cook in res.Cookies)
                {
                    cookieMgr.SetCookie(cook.Name, cook.Value);
                    if (cook.Name.Contains(@"userid") && !string.IsNullOrEmpty(cook.Value))
                        bFinalRet = true;
                }
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

		    return bFinalRet;
		}
	}
}
