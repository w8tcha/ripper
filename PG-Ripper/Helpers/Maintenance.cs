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
    using System.Text;
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
        public string ExtractTopicTitleFromHtml(string page)
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
            string pageContent;

            if (CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"vipergirls.to"))
            {
                pageContent = GetForumPageAsString(content);

                const string Start = "<span class=\"ctrlcontainer\">";

                int iPageStart = pageContent.IndexOf(Start, StringComparison.Ordinal);

                if (iPageStart < 0)
                {
                    return string.Empty;
                }

                iPageStart += Start.Length;

                int iPageEnd = pageContent.IndexOf("</span></a>", iPageStart, StringComparison.Ordinal);

                return iPageEnd < 0 ? string.Empty : pageContent.Substring(iPageStart, iPageEnd - iPageStart);
            }
            else
            {
                pageContent = GetForumPageAsString(content);

                const string Start = "<li class=\"navbit\">";

                int iPageStart = pageContent.LastIndexOf(Start, StringComparison.Ordinal);

                if (iPageStart < 0)
                {
                    return string.Empty;
                }

                iPageStart += Start.Length;

                iPageStart = pageContent.IndexOf(">", iPageStart, StringComparison.Ordinal);

                iPageStart += 1;

                int iPageEnd = pageContent.IndexOf("</a></li>", iPageStart, StringComparison.Ordinal);

                return iPageEnd < 0 ? string.Empty : pageContent.Substring(iPageStart, iPageEnd - iPageStart);
            }
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
            string postTitle;

            ////////////////////////////////////
            // Extract Current Post first
            string sPostId = url.Substring(url.IndexOf("#post") + 5);

            // use only message content
            string sMessageStart = string.Format(
                "<li class=\"postbitlegacy postbitim postcontainer\" id=\"post_{0}\">",
                sPostId);
            const string MessageEnd = "</blockquote>";

            int iStart = content.IndexOf(sMessageStart);

            iStart += sMessageStart.Length;

            int iEnd = content.IndexOf(MessageEnd, iStart);

            var pageContent = content.Substring(iStart, iEnd - iStart);

            /////////////////////////////////

            const string TitleStart = "<h2 class=\"title icon\">";

            int iTitleStart = pageContent.IndexOf(TitleStart);

            iTitleStart += TitleStart.Length;

            int iTitleEnd = pageContent.IndexOf("</h2>", iTitleStart);

            try
            {
                postTitle =
                    pageContent.Substring(iTitleStart, iTitleEnd - iTitleStart)
                        .Replace("\r", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("\n", string.Empty);

                // Remove Post Icon
                if (postTitle.StartsWith("<img src="))
                {
                    postTitle = postTitle.Substring(postTitle.IndexOf("/>") + 3);
                }
            }
            catch (Exception)
            {
                postTitle = string.Empty;
            }

            if (string.IsNullOrEmpty(postTitle))
            {
                postTitle = string.Format("post# {0}", url.Substring(url.IndexOf(@"#post") + 5));
            }
            else
            {
                return Utility.ReplaceHexWithAscii(postTitle);
            }

            return Utility.ReplaceHexWithAscii(postTitle);
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
                webClient.Encoding = Encoding.UTF8;

                if (!CacheController.Instance().UserSettings.CurrentUserName.Equals("Guest"))
                {
                    webClient.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                }

                pageContent = webClient.DownloadString(url);

                webClient.Dispose();
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return pageContent;
        }
    }
}
