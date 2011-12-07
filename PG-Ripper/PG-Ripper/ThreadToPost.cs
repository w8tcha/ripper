// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadToPost.cs" company="The Watcher">
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
    using System.Collections.Generic;
    using System.Net;
    using PGRipper.Objects;

    /// <summary>
    /// This class gets all the thread pages of the Thread and parse every single post
    /// </summary>
    public class ThreadToPost
    {
        /// <summary>
        /// Gets the post pages.
        /// </summary>
        /// <param name="uRL">
        /// The u RL.
        /// </param>
        /// <returns>
        /// The get post pages.
        /// </returns>
        public string GetPostPages(string uRL)
        {
            return GetRipPage(uRL);
        }

        /// <summary>
        /// Gets the Content of all Thread Pages
        /// </summary>
        /// <param name="sURL">Original URL of the Forum Thread</param>
        /// <returns>The Content of all Thread Pages</returns>
        public string GetThreadPages(string sURL)
        {
            string sPageContent = GetRipPage(sURL);

            int iStart = 0;
            int iThreadPages;
            try
            {
                iStart = sPageContent.IndexOf("Page 1 of ", iStart);
            }
            catch (Exception)
            {
                return sPageContent;
            }

            if (iStart >= 0)
            {
                int iEnd = sPageContent.IndexOf("</td>", iStart);
                iThreadPages = int.Parse(sPageContent.Substring(iStart + 10, iEnd - (iStart + 10)));
            }
            else
            {
                return sPageContent;
            }

            string szThreadBaseURL = sURL;

            for (int i = 1; i <= iThreadPages; i++)
            {
                string szComposed = string.Format("{0}&page={1}", szThreadBaseURL, i);

                sPageContent += GetRipPage(szComposed);
            }

            return sPageContent;
        }

        /// <summary>
        /// Gets the Content of all Thread Pages (VB Forums 4.x)
        /// </summary>
        /// <param name="sURL">Original URL of the Forum Thread</param>
        /// <returns>The Content of all Thread Pages</returns>
        public string GetThreadPagesNew(string sURL)
        {
            string sPageContent = GetRipPage(sURL);

            int iStart;
            int iThreadPages;

            try
            {
                int iPagerStart = sPageContent.IndexOf("<span><a href=\"javascript://\" class=\"popupctrl\">Page ");
                iStart = sPageContent.IndexOf("of ", iPagerStart);

                iStart += 3;
            }
            catch (Exception)
            {
                return sPageContent;
            }

            if (iStart >= 0)
            {
                int iEnd = sPageContent.IndexOf("</a></span>", iStart);

                iThreadPages = int.Parse(sPageContent.Substring(iStart, iEnd - iStart));
            }
            else
            {
                return sPageContent;
            }

            string szThreadBaseURL = sURL;

            for (int i = 1; i <= iThreadPages; i++)
            {
                // -2.html
                string szComposed = szThreadBaseURL.Contains(".html") ? szThreadBaseURL.Replace(".html", string.Format("-{0}.html", i)) : string.Format("{0}&page={1}", szThreadBaseURL, i);

                sPageContent += GetRipPage(szComposed);
            }

            return sPageContent;
        }

        /// <summary>
        /// Extracts all post Content
        /// </summary>
        /// <param name="s">Full HTML Content</param>
        /// <returns>Post Content</returns>
        public List<ImageInfo> ParseHtml(string s)
        {
            return Utility.ExtractThreadtoPostsHtml(s);
        }

        /// <summary>
        /// Gets the rip page.
        /// </summary>
        /// <param name="strURL">
        /// The STR URL.
        /// </param>
        /// <returns>
        /// The get rip page.
        /// </returns>
        private static string GetRipPage(string strURL)
        {
            Uri uURL = new Uri(strURL);

            string sResult;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                sResult = wc.DownloadString(uURL);

                while (wc.IsBusy)
                {
                    System.Threading.Thread.Sleep(10);
                    ////Application.DoEvents();
                }

                wc.Dispose();
            }
            catch (Exception)
            {
                return "error";
            }

            return sResult;
        }
    }
}