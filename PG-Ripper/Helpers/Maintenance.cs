// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Maintenance.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    using Ripper.Core.Components;

    /// <summary>
    /// Maintenance Class.
    /// </summary>
    public class Maintenance
    {
        /// <summary>
        /// Gets or sets the MainForm Instance
        /// </summary>
        public static System.Windows.Forms.Form Xform { get; set; }

        /// <summary>
        /// Gets or sets the Maintenance Instance
        /// </summary>
        public static Maintenance Instace { get; set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static Maintenance GetInstance()
        {
            return Instace ?? (Instace = new Maintenance());
        }

        /// <summary>
        /// Extract Current Page Title
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The get rip page title.
        /// </returns>
        public string GetRipPageTitle(string page)
        {
            var match = Regex.Match(
                page,
                @"<title>(?<inner>[^<]*)</title>",
                RegexOptions.Compiled);

            if (!match.Success)
            {
                return string.Empty;
            }

            var title = match.Groups["inner"].Value;

            if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
            {
                return title.Trim();
            }

            title = title.Contains(@"View Single Post")
                        ? title.Substring(title.IndexOf("View Single Post - ") + 19)
                        : Regex.Replace(title, title.Substring(title.IndexOf("- ")), string.Empty);

            return title.Trim();
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

            if (CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"vipergirls.to"))
            {
                sPage = GetForumPageAsString(content);

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

            if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
            {
                sPage = GetForumPageAsString(content);

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

                sPage = GetForumPageAsString(CacheController.Instance().UserSettings.CurrentForumUrl + sFTitleUrl);
            }
            else
            {
                sPage = GetForumPageAsString(content);
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

            if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
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
                if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
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
            return GetForumPageAsString(url);
        }

        /// <summary>
        /// Gets the forum page as string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The Page Content
        /// </returns>
        private static string GetForumPageAsString(string url)
        {
            string pageContent;

            try
            {
                var webClient = new WebClient();
               
                webClient.Headers.Add(string.Format("Referer: {0}", url));

                if (!CacheController.Instance().UserSettings.CurrentUserName.Equals("Guest"))
                {
                    webClient.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                }

                pageContent = webClient.DownloadString(url);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return pageContent;
        }
    }
}
