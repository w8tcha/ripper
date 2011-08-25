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
using System.Net;
using System.Text.RegularExpressions;


namespace PGRipper
{
	/// <summary>
	/// Summary description for Maintainance.
	/// </summary>
	public class Maintainance
	{
		public static System.Windows.Forms.Form xform;
		public static Maintainance mInstace;

		public static Maintainance GetInstance()
		{
		    return mInstace ?? (mInstace = new Maintainance());
		}
        /// <summary>
        /// Extract Current Page Title
        /// </summary>
        /// <param name="sURL"></param>
        /// <returns></returns>
	    public string GetRipPageTitle(string sURL)
        {
            //string sPage = getRIPPage(sURL);

            string sPage = sURL;


            int iTitleStart = sPage.IndexOf("<title>");

            if (iTitleStart < 0)
                return string.Empty;

            iTitleStart += 7;

            if (MainForm.userSettings.sForumUrl.Contains(@"http://rip-") ||
                    MainForm.userSettings.sForumUrl.Contains(@"http://www.rip-"))
            {
                iTitleStart += 1;
            }

	        int iTitleEnd = sPage.IndexOf(@"</title>");

            if (iTitleEnd < 0)
                return string.Empty;

            string sTitle = sPage.Substring(iTitleStart, iTitleEnd - iTitleStart);

            if (MainForm.userSettings.sForumUrl.Contains("rip-productions.net"))
            {
                return sTitle.Trim();
            }


	        sTitle = sTitle.Contains(@"View Single Post") ? sTitle.Substring(sTitle.IndexOf("View Single Post - ") + 19) : Regex.Replace(sTitle, sTitle.Substring(sTitle.IndexOf("- ")), string.Empty);


	        return sTitle.Trim();
        }
        /// <summary>
        /// Get Current Security Token for the Thank You Button Function
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public string GetSecurityToken(string sContent)
        {
            string sPage = sContent;

            

            int iSecurityTokenStart = sPage.IndexOf("securitytoken=");

            if (iSecurityTokenStart < 0)
                return string.Empty;

            iSecurityTokenStart += 14;


            int iSecurityTokenEnd = sPage.IndexOf("\" id=\"post_thanks_button_");

            if (iSecurityTokenEnd < 0)
                return string.Empty;

            string sSecurityToken = sPage.Substring(iSecurityTokenStart, iSecurityTokenEnd - iSecurityTokenStart);

            return sSecurityToken;
        }
        /// <summary>
        /// Extract Current Forum Title of the Current Sub Forum
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="bIsPost"></param>
        /// <returns></returns>
        public string ExtractForumTitleFromHtml(string sContent, bool bIsPost)
        {
            string sPage;

            /*if (MainForm.userSettings.sForumUrl.Contains(@"proving-grounds.com"))
            {
                sPage = sContent;

                if (sPage.Contains(@"proving-grounds.com"))
                {

                    string sForumTitleFromUrl = sPage.Substring(sPage.IndexOf("com/") + 4);

                    sForumTitleFromUrl = sForumTitleFromUrl.Remove(sForumTitleFromUrl.IndexOf("/"));

                    return sForumTitleFromUrl;
                }
            }
            else*/ if (MainForm.userSettings.sForumUrl.Contains(@"rip-productions.net"))
            {
                sPage = GetRipPage(sContent);

                const string sStart = "<span class=\"ctrlcontainer\">";

                int iPageStart = sPage.IndexOf(sStart);

                if (iPageStart < 0)
                    return string.Empty;

                iPageStart += sStart.Length;

                int iPageEnd = sPage.IndexOf("</span></a>", iPageStart);

                if (iPageEnd < 0)
                {
                    return string.Empty;
                }

                return sPage.Substring(iPageStart, iPageEnd - iPageStart);
            }

            if (bIsPost)
            {
                // First get Thread Url of Current Post
                int iPageStart = sContent.IndexOf("href=\"showthread.php");

                if (iPageStart < 0)
                    return string.Empty;

                iPageStart += 6;

                int iPageEnd = sContent.IndexOf("\">", iPageStart);

                if (iPageEnd < 0)
                {
                    return string.Empty;
                }

                string sFTitleUrl = sContent.Substring(iPageStart, iPageEnd - iPageStart);

                sPage = GetRipPage(MainForm.userSettings.sForumUrl + sFTitleUrl);
            }
            else
            {
                sPage = GetRipPage(sContent);
            }

            // Now Parse Sub Forum Title

            const string sMetaStart = "<meta name=\"description\" content=\"";

            int iTitleStart = sPage.IndexOf(sMetaStart);

            if (iTitleStart < 0)
                return string.Empty;

            iTitleStart += sMetaStart.Length;


            int iTitleEnd = sPage.IndexOf("\" />", iTitleStart);

            if (iTitleEnd < 0)
            {
                return string.Empty;
            }

            string sForumTitle = sPage.Substring(iTitleStart, iTitleEnd - iTitleStart);

            return Utility.ReplaceHexWithAscii(sForumTitle);
        }
        /// <summary>
        /// Extract the Current Post Title if there is any 
        /// if not use PostId As Title
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="sURL"></param>
        /// <returns></returns>
        public string ExtractPostTitleFromHtml(string sContent, string sURL)
        {
            string sPage = sContent;
            //string sPage = getRIPPage(sURL);

            string sPostTitle;

            if (MainForm.userSettings.sForumUrl.Contains(@"http://rip-") ||
                    MainForm.userSettings.sForumUrl.Contains(@"http://www.rip-"))
            {
                ////////////////////////////////////
                // Extract Current Post first
                string sPostId = sURL.Substring(sURL.IndexOf("#post") + 5);

                // use only message content
                string sMessageStart = string.Format("<li class=\"postbitlegacy postbitim postcontainer\" id=\"post_{0}\">", sPostId);
                const string sMessageEnd = "</blockquote>";

                int iStart = sContent.IndexOf(sMessageStart);

                iStart += sMessageStart.Length;

                int iEnd = sContent.IndexOf(sMessageEnd, iStart);

                sPage = sContent.Substring(iStart, iEnd - iStart);

                /////////////////////////////////

                const string sTitleStart = "<h2 class=\"title icon\">";

                int iTitleStart = sPage.IndexOf(sTitleStart);

                iTitleStart += sTitleStart.Length;

                int iTitleEnd = sPage.IndexOf("</h2>", iTitleStart);

                try
                {
                    sPostTitle = sPage.Substring(iTitleStart, iTitleEnd - iTitleStart).Replace("\r", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty);

                    // Remove Post Icon
                    if (sPostTitle.StartsWith("<img src="))
                    {
                        sPostTitle = sPostTitle.Substring(sPostTitle.IndexOf("/>") + 3);
                    }
                }
                catch (Exception)
                {
                    sPostTitle = string.Empty;
                }
               
            }
            else
            {
                try
                {
                    int iTitleStart = sPage.IndexOf("<td width=\"100%\" valign=\"middle\"><div class=\"smallfont\" align=\"center\"> <strong>");

                    iTitleStart += 80;

                    int iTitleEnd = sPage.IndexOf("</strong></div></td>");

                    sPostTitle = sPage.Substring(iTitleStart, iTitleEnd - iTitleStart);
                }
                catch (Exception)
                {
                    sPostTitle = string.Empty;
                }
            }

            

            if (string.IsNullOrEmpty(sPostTitle))
            {
                if (MainForm.userSettings.sForumUrl.Contains(@"http://rip-") ||
                    MainForm.userSettings.sForumUrl.Contains(@"http://www.rip-"))
                {
                    sPostTitle = "post# " + sURL.Substring(sURL.IndexOf(@"#post") + 5);
                }
                else
                {
                    if (sURL.Contains(@"&postcount="))
                    {
                        int iTitleStart = sURL.IndexOf("showpost.php?p=");

                        if (iTitleStart < 0)
                            return string.Empty;

                        iTitleStart += 16;


                        int iTitleEnd = sURL.IndexOf("&postcount=");

                        if (iTitleEnd < 0)
                            return string.Empty;

                        sPostTitle = "post# " + sURL.Substring(iTitleStart, iTitleEnd - iTitleStart);
                    }
                    else
                    {
                        sPostTitle = "post# " + sURL.Substring(sURL.IndexOf(@"showpost.php?p=") + 15);
                    }
                }

            }
            else
            {
                return Utility.ReplaceHexWithAscii(sPostTitle);
            }


            return Utility.ReplaceHexWithAscii(sPostTitle);
        }
          
        public string GetPostPages(string sURL)
        {
            return GetRipPage(sURL);
        }

        private static string GetRipPage(string strURL)
        {
            string sPageRead;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Referer: " + strURL);
                wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                wc.Headers.Add("Cookie: " + CookieManager.GetInstance().GetCookieString());
                sPageRead = wc.DownloadString(strURL);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return sPageRead;
        }
	}
}
