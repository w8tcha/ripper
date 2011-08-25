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

namespace PGRipper
{
    #region

    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// Finds, and extract all Items from a Page
    /// </summary>
    public static class ItemFinder
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

            var m1 = Regex.Matches(page, @"(<a.*?>.*?)", RegexOptions.Multiline);

            foreach (Match m in m1)
            {
                var value = m.Groups[1].Value;
                var item = new LinkItem();

                var m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.IgnoreCase);

                if (m2.Success)
                {
                    item.Href = m2.Groups[1].Value;
                }

                //item.Text = Regex.Replace(value, @"\s*<.*?>\s*", string.Empty, RegexOptions.IgnoreCase);

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

            var m1 = Regex.Matches(page, @"(<img.*?>)", RegexOptions.IgnoreCase);

            foreach (Match m in m1)
            {
                var value = m.Groups[1].Value;
                var item = new LinkItem();

                var m2 = Regex.Match(value, @"src=\""(.*?)\""", RegexOptions.IgnoreCase);

                if (m2.Success)
                {
                    item.Href = m2.Groups[1].Value;
                }

                anchorList.Add(item);
            }

            return anchorList;
        }

        #endregion
    }
}