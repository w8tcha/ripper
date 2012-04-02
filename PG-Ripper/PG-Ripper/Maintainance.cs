// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Maintainance.cs" company="The Watcher">
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
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Maintainance Class.
    /// </summary>
    public class Maintainance
    {
        /// <summary>
        /// Gets or sets the MainForm Instance
        /// </summary>
        public static System.Windows.Forms.Form Xform { get; set; }

        /// <summary>
        /// Gets or sets the Maintainance Instance
        /// </summary>
        public static Maintainance Instace { get; set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static Maintainance GetInstance()
        {
            return Instace ?? (Instace = new Maintainance());
        }

        /// <summary>
        /// Extract Current Page Title
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The get rip page title.
        /// </returns>
        public string GetRipPageTitle(string url)
        {
            string sPage = url;

            int iTitleStart = sPage.IndexOf("<title>");

            if (iTitleStart < 0)
            {
                return string.Empty;
            }

            iTitleStart += 7;

            if (MainForm.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                MainForm.userSettings.CurrentForumUrl.Contains(@"http://www.rip-"))
            {
                iTitleStart += 1;
            }

            int iTitleEnd = sPage.IndexOf(@"</title>");

            if (iTitleEnd < 0)
            {
                return string.Empty;
            }

            string sTitle = sPage.Substring(iTitleStart, iTitleEnd - iTitleStart);

            if (MainForm.userSettings.CurrentForumUrl.Contains("rip-productions.net") ||
                MainForm.userSettings.CurrentForumUrl.Contains(@"kitty-kats.com"))
            {
                return sTitle.Trim();
            }

            sTitle = sTitle.Contains(@"View Single Post")
                         ? sTitle.Substring(sTitle.IndexOf("View Single Post - ") + 19)
                         : Regex.Replace(sTitle, sTitle.Substring(sTitle.IndexOf("- ")), string.Empty);

            return sTitle.Trim();
        }

        /// <summary>
        /// Get Current Security Token for the Thank You Button Function
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The get security token.
        /// </returns>
        public string GetSecurityToken(string content)
        {
            string sPage = content;

            int iSecurityTokenStart = sPage.IndexOf("securitytoken=");

            if (iSecurityTokenStart < 0)
            {
                return string.Empty;
            }

            iSecurityTokenStart += 14;

            int iSecurityTokenEnd = sPage.IndexOf("\" id=\"post_thanks_button_");

            if (iSecurityTokenEnd < 0)
            {
                return string.Empty;
            }

            string sSecurityToken = sPage.Substring(iSecurityTokenStart, iSecurityTokenEnd - iSecurityTokenStart);

            return sSecurityToken;
        }

        /// <summary>
        /// Extract Current Forum Title of the Current Sub Forum
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="isPost">
        /// if set to <c>true</c> [is post].
        /// </param>
        /// <returns>
        /// The extract forum title from html.
        /// </returns>
        public string ExtractForumTitleFromHtml(string content, bool isPost)
        {
            string sPage;

            if (MainForm.userSettings.CurrentForumUrl.Contains(@"rip-productions.net"))
            {
                sPage = GetRipPage(content);

                const string Start = "<span class=\"ctrlcontainer\">";

                int iPageStart = sPage.IndexOf(Start);

                if (iPageStart < 0)
                {
                    return string.Empty;
                }

                iPageStart += Start.Length;

                int iPageEnd = sPage.IndexOf("</span></a>", iPageStart);

                return iPageEnd < 0 ? string.Empty : sPage.Substring(iPageStart, iPageEnd - iPageStart);
            }

            if (MainForm.userSettings.CurrentForumUrl.Contains(@"kitty-kats.com"))
            {
                sPage = GetRipPage(content);

                const string Start = "<li class=\"navbit\">";

                int iPageStart = sPage.LastIndexOf(Start, StringComparison.Ordinal);

                if (iPageStart < 0)
                {
                    return string.Empty;
                }

                iPageStart += Start.Length;

                iPageStart = sPage.IndexOf(">", iPageStart);

                iPageStart += 1;

                int iPageEnd = sPage.IndexOf("</a></li>", iPageStart);

                return iPageEnd < 0 ? string.Empty : sPage.Substring(iPageStart, iPageEnd - iPageStart);
            }

            if (isPost)
            {
                // First get Thread Url of Current Post
                int iPageStart = content.IndexOf("href=\"showthread.php");

                if (iPageStart < 0)
                {
                    return string.Empty;
                }

                iPageStart += 6;

                int iPageEnd = content.IndexOf("\">", iPageStart);

                if (iPageEnd < 0)
                {
                    return string.Empty;
                }

                string sFTitleUrl = content.Substring(iPageStart, iPageEnd - iPageStart);

                sPage = GetRipPage(MainForm.userSettings.CurrentForumUrl + sFTitleUrl);
            }
            else
            {
                sPage = GetRipPage(content);
            }

            // Now Parse Sub Forum Title
            const string MetaStart = "<meta name=\"description\" content=\"";

            int iTitleStart = sPage.IndexOf(MetaStart);

            if (iTitleStart < 0)
            {
                return string.Empty;
            }

            iTitleStart += MetaStart.Length;

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
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The extract post title from html.
        /// </returns>
        public string ExtractPostTitleFromHtml(string content, string url)
        {
            string sPage = content;

            string sPostTitle;

            if (MainForm.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                MainForm.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                MainForm.userSettings.CurrentForumUrl.Contains(@"kitty-kats.com"))
            {
                ////////////////////////////////////
                // Extract Current Post first
                string sPostId = url.Substring(url.IndexOf("#post") + 5);

                // use only message content
                string sMessageStart =
                    string.Format("<li class=\"postbitlegacy postbitim postcontainer\" id=\"post_{0}\">", sPostId);
                const string MessageEnd = "</blockquote>";

                int iStart = content.IndexOf(sMessageStart);

                iStart += sMessageStart.Length;

                int iEnd = content.IndexOf(MessageEnd, iStart);

                sPage = content.Substring(iStart, iEnd - iStart);

                /////////////////////////////////

                const string TitleStart = "<h2 class=\"title icon\">";

                int iTitleStart = sPage.IndexOf(TitleStart);

                iTitleStart += TitleStart.Length;

                int iTitleEnd = sPage.IndexOf("</h2>", iTitleStart);

                try
                {
                    sPostTitle =
                        sPage.Substring(iTitleStart, iTitleEnd - iTitleStart).Replace("\r", string.Empty).Replace(
                            "\t", string.Empty).Replace("\n", string.Empty);

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
                    int iTitleStart =
                        sPage.IndexOf(
                            "<td width=\"100%\" valign=\"middle\"><div class=\"smallfont\" align=\"center\"> <strong>");

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
                if (MainForm.userSettings.CurrentForumUrl.Contains(@"http://rip-") ||
                    MainForm.userSettings.CurrentForumUrl.Contains(@"http://www.rip-") ||
                    MainForm.userSettings.CurrentForumUrl.Contains(@"kitty-kats.com"))
                {
                    sPostTitle = string.Format("post# {0}", url.Substring(url.IndexOf(@"#post") + 5));
                }
                else
                {
                    if (url.Contains(@"&postcount="))
                    {
                        int iTitleStart = url.IndexOf("showpost.php?p=");

                        if (iTitleStart < 0)
                        {
                            return string.Empty;
                        }

                        iTitleStart += 16;

                        int iTitleEnd = url.IndexOf("&postcount=");

                        if (iTitleEnd < 0)
                        {
                            return string.Empty;
                        }

                        sPostTitle = string.Format("post# {0}", url.Substring(iTitleStart, iTitleEnd - iTitleStart));
                    }
                    else
                    {
                        sPostTitle = string.Format("post# {0}", url.Substring(url.IndexOf(@"showpost.php?p=") + 15));
                    }
                }
            }
            else
            {
                return Utility.ReplaceHexWithAscii(sPostTitle);
            }

            return Utility.ReplaceHexWithAscii(sPostTitle);
        }

        /// <summary>
        /// Gets the post pages.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <returns>
        /// The get post pages.
        /// </returns>
        public string GetPostPages(string url)
        {
            return GetRipPage(url);
        }

        /// <summary>
        /// Gets the rip page.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <returns>
        /// The get rip page.
        /// </returns>
        private static string GetRipPage(string url)
        {
            string sPageRead;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(string.Format("Referer: {0}", url));
                wc.Headers.Add(
                    "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                wc.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                sPageRead = wc.DownloadString(url);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return sPageRead;
        }
    }
}
