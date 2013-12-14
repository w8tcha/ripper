// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtractHelper.cs" company="The Watcher">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Helper Class to Extract Content from Forum Pages
    /// </summary>
    public class ExtractHelper
    {
        /// <summary>
        /// Attempts to extract hot linked and thumb-&gt;FullScale images.
        /// </summary>
        /// <param name="htmlDump">
        /// The html Dump.
        /// </param>
        /// <returns>
        /// The extract attachment images html.
        /// </returns>
        public static List<ImageInfo> ExtractAttachmentImagesHtml(string htmlDump)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();

            htmlDump = htmlDump.Replace("&amp;", "&");

            // use only message content
            var iStart = htmlDump.IndexOf("<!-- attachments -->");

            if (iStart < 0)
            {
                // Return Empty List
                return rtnList;
            }

            iStart += 21;

            var iEnd = htmlDump.IndexOf("<!-- / attachments -->");

            if (iEnd > 0)
            {
                htmlDump = htmlDump.Substring(iStart, iEnd - iStart);
            }

            ///////////////////////////////////////////////
            rtnList.AddRange(
                LinkFinder.ListAllLinks(htmlDump).Select(
                    link =>
                    new ImageInfo
                    {
                        ImageUrl = CacheController.Instance().UserSettings.CurrentForumUrl + Utility.ReplaceHexWithAscii(link.Href),
                        ThumbnailUrl = string.Empty
                    }).Where(
                            newPicPoolItem => !Utility.IsImageNoneSense(newPicPoolItem.ImageUrl)));

            return rtnList;
        }

        /// <summary>
        /// The extract images html.
        /// </summary>
        /// <param name="htmlDump">
        /// The html dump.
        /// </param>
        /// <param name="postId">
        /// The post id.
        /// </param>
        /// <returns>
        /// Extracted Images List
        /// </returns>
        public static List<ImageInfo> ExtractImagesHtml(string htmlDump, string postId)
        {
            List<ImageInfo> rtnList;

            if (CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"scanlover.com"))
            {
                rtnList = ExtractAttachmentImagesHtml(htmlDump);
            }
            else if (CacheController.Instance().UserSettings.CurrentForumUrl.Contains(@"sexyandfunny.com"))
            {
                rtnList = ExtractImagesLinksHtml(htmlDump, null);

                if (rtnList.Count.Equals(0))
                {
                    rtnList = ExtractAttachmentImagesHtml(htmlDump);
                }
            }
            else if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
            {
                rtnList = ExtractImagesLinksHtml(htmlDump, postId);
            }
            else
            {
                rtnList = ExtractImagesLinksHtml(htmlDump, null);
            }

            return rtnList;
        }

        /// <summary>
        /// Attempts to extract hot linked and thumb-&gt;FullScale images.
        /// </summary>
        /// <param name="htmlDump">
        /// The html Dump.
        /// </param>
        /// <param name="postId">
        /// The Post Id.
        /// </param>
        /// <returns>
        /// The extract images links html.
        /// </returns>
        public static List<ImageInfo> ExtractImagesLinksHtml(string htmlDump, string postId)
        {
            if (!string.IsNullOrEmpty(postId) && postId.StartsWith("http://"))
            {
                postId = postId.Substring(postId.IndexOf("#post") + 5);
            }

            htmlDump = htmlDump.Replace("&amp;", "&");

            // use only message content
            var sMessageStart = "<!-- message -->";
            var sMessageEnd = "<!-- / message -->";

            // If Forum uses VB 4.x or higher
            if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
            {
                sMessageStart = string.Format("<div id=\"post_message_{0}\">", postId);
                sMessageEnd = "</blockquote>";
            }

            var iStart = htmlDump.IndexOf(sMessageStart);

            iStart += sMessageStart.Length;

            var iEnd = htmlDump.IndexOf(sMessageEnd, iStart);

            htmlDump = htmlDump.Substring(iStart, iEnd - iStart);

            ///////////////////////////////////////////////

            // Parse all Links <a>
            var rtnList =
                LinkFinder.ListAllLinks(htmlDump).Select(
                    link =>
                    new ImageInfo
                    {
                        ImageUrl = RemoveRedirectLink(Utility.ReplaceHexWithAscii(link.Href)),
                        ThumbnailUrl = Utility.ReplaceHexWithAscii(link.Text)
                    }).Where(newPicPoolItem => !Utility.IsImageNoneSense(newPicPoolItem.ImageUrl)).ToList();

            // Parse all Image <a>
            rtnList.AddRange(
                LinkFinder.ListAllImages(htmlDump).Select(
                    link =>
                    new ImageInfo
                    {
                        ImageUrl = RemoveRedirectLink(Utility.ReplaceHexWithAscii(link.Href)),
                        ThumbnailUrl = Utility.ReplaceHexWithAscii(link.Text)
                    }));

            return rtnList;
        }

        /// <summary>
        /// Removes the redirect link.
        /// </summary>
        /// <param name="linkToCheck">
        /// The link to check.
        /// </param>
        /// <returns>
        /// The remove redirect link.
        /// </returns>
        public static string RemoveRedirectLink(string linkToCheck)
        {
            if (linkToCheck.Contains(@"http://vipergirls.to/redirect-to/?redirect="))
            {
                linkToCheck =
                    linkToCheck.Replace(@"http://vipergirls.to/redirect-to/?redirect=", string.Empty);

                linkToCheck = HttpUtility.UrlDecode(linkToCheck);
            }

            // Remove anonym.to from Link if exists
            if (linkToCheck.Contains("www.anonym.to"))
            {
                linkToCheck = linkToCheck.Replace(@"http://www.anonym.to/?", string.Empty);
            }

            // Remove anonym.to from Link if exists
            if (linkToCheck.Contains("anonym.to"))
            {
                linkToCheck = linkToCheck.Replace(@"http://anonym.to/?", string.Empty);
            }

            return linkToCheck;
        }

        /// <summary>
        /// TODO : Change to regex
        /// Extracts links leading to other threads and posts for indices crawling.
        /// </summary>
        /// <param name="htmlDump">
        /// The HTML Dump.
        /// </param>
        /// <param name="url">
        /// The Url.
        /// </param>
        /// <returns>
        /// The extract index URL's html.
        /// </returns>
        public static List<ImageInfo> ExtractIndexUrlsHtml(string htmlDump, string url)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();

            const string StartHref = "<a ";
            const string Href = "href=\"";
            const string EndHref = "</a>";

            // use only message content
            if (!string.IsNullOrEmpty(url) && url.StartsWith("http://") && url.Contains("#post"))
            {
                url = url.Substring(url.IndexOf("#post") + 5);

                string sMessageStart = string.Format("<div id=\"post_message_{0}\">", url);
                const string MessageEnd = "</blockquote>";

                int iStart = htmlDump.IndexOf(sMessageStart);

                iStart += sMessageStart.Length;

                int iEnd = htmlDump.IndexOf(MessageEnd, iStart);

                htmlDump = htmlDump.Substring(iStart, iEnd - iStart);
            }

            string sCopy = htmlDump;

            ///////////////////////////////////////////////
            int iStartHref = sCopy.IndexOf(StartHref);

            if (iStartHref < 0)
            {
                return rtnList;
            }

            //////////////////////////////////////////////////////////////////////////

            while (iStartHref >= 0)
            {
                // Thread.Sleep(1);
                int iHref = sCopy.IndexOf(Href, iStartHref);

                if (!(iHref >= 0))
                {
                    iStartHref = sCopy.IndexOf(StartHref, iStartHref + EndHref.Length);
                    continue;
                }

                int iEndHref = sCopy.IndexOf(EndHref, iHref);

                if (iEndHref >= 0)
                {
                    string substring = sCopy.Substring(iHref + Href.Length, iEndHref - (iHref + Href.Length));
                    sCopy = sCopy.Remove(iStartHref, iEndHref + EndHref.Length - iStartHref);

                    iStartHref = substring.IndexOf("\" target=\"_blank\">");

                    if (iStartHref >= 0)
                    {
                        ImageInfo imgInfoIndexLink = new ImageInfo { ThumbnailUrl = string.Empty, ImageUrl = substring.Substring(0, iStartHref) };

                        if (imgInfoIndexLink.ImageUrl.Contains(@"showthread.php") ||
                            imgInfoIndexLink.ImageUrl.Contains(@"showpost.php"))
                        {
                            if (imgInfoIndexLink.ImageUrl.Contains("&amp;"))
                            {
                                imgInfoIndexLink.ImageUrl =
                                    imgInfoIndexLink.ImageUrl.Remove(imgInfoIndexLink.ImageUrl.IndexOf("&amp;"));
                            }

                            rtnList.Add(imgInfoIndexLink);
                        }
                    }
                }

                iStartHref = 0;
                iStartHref = sCopy.IndexOf(StartHref, iStartHref);
            }

            //////////////////////////////////////////////////////////////////////////

            return rtnList;
        }

        /// <summary>
        /// TODO : Change to Regex
        /// Get Post ids of all Posts
        /// </summary>
        /// <param name="htmlDump">
        /// The html Dump.
        /// </param>
        /// <returns>
        /// The extract thread to posts html.
        /// </returns>
        public static List<ImageInfo> ExtractThreadtoPostsHtml(string htmlDump)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();

            const string Start = "<a name=\"post";

            string sEnd = "\">";

            if (Utility.IsV4Forum(CacheController.Instance().UserSettings))
            {
                sEnd = "\" href";
            }

            string sCopy = htmlDump;

            int iStart = 0;

            iStart = sCopy.IndexOf(Start, iStart);

            while (iStart >= 0)
            {
                int iEnd = sCopy.IndexOf(sEnd, iStart);

                string sPostId = sCopy.Substring(iStart + Start.Length, iEnd - (iStart + Start.Length));

                ImageInfo newThumbPicPool = new ImageInfo { ImageUrl = sPostId };

                // iEnd = 0;
                if (Utility.IsNumeric(sPostId) && !string.IsNullOrEmpty(sPostId))
                {
                    rtnList.Add(newThumbPicPool);
                }

                iStart = sCopy.IndexOf(Start, iStart + sEnd.Length);
            }

            return rtnList;
        }
    }
}