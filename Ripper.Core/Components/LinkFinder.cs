// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkFinder.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Core.Components
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Ripper.Core.Objects;

    /// <summary>
    /// Finds, and extract all Items from a Page
    /// </summary>
    public static class LinkFinder
    {
        #region Public Methods

        /// <summary>
        /// Extract the Links
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// All Anchors on the Page
        /// </returns>
        public static List<LinkItem> ListAllLinks(string page)
        {
            var linkList = new List<LinkItem>();

            var linkMatch = Regex.Matches(page, @"(<a.*?>.*?</a>)", RegexOptions.Multiline);

            foreach (Match match in linkMatch)
            {
                var value = match.Groups[1].Value;
                var item = new LinkItem();

                var hrefMatch = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.IgnoreCase);

                if (hrefMatch.Success)
                {
                    item.Href = hrefMatch.Groups[1].Value;
                }

                var thumbNailMatch = Regex.Match(value, @"src=\""(.*?)\""", RegexOptions.IgnoreCase);

                if (!thumbNailMatch.Success)
                {
                    continue;
                }

                item.Text = thumbNailMatch.Groups[1].Value;

                linkList.Add(item);
            }

            return linkList;
        }

        /// <summary>
        /// Extract the Image
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// All Anchors on the Page
        /// </returns>
        public static List<LinkItem> ListAllImages(string page)
        {
            var anchorList = new List<LinkItem>();

            var linkImageMatch = Regex.Matches(page, @"^(?!<a.*?>)(<img.*?>)", RegexOptions.IgnoreCase);

            foreach (Match match in linkImageMatch)
            {
                var value = match.Groups[1].Value;
                var item = new LinkItem();

                var hrefMatch = Regex.Match(value, @"src=\""(.*?)\""", RegexOptions.IgnoreCase);

                if (hrefMatch.Success)
                {
                    item.Href = hrefMatch.Groups[1].Value;
                }

                anchorList.Add(item);
            }

            return anchorList;
        }

        #endregion
    }
}