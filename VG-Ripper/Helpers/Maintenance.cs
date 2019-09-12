// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Maintenance.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Ripper.Core.Components;

    #endregion

    /// <summary>
    /// Maintenance Class.
    /// </summary>
    public class Maintenance
    {
        #region Constants and Fields

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static Maintenance Instance { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static Maintenance GetInstance()
        {
            return Instance ?? (Instance = new Maintenance());
        }

        /// <summary>
        /// Count The Images from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns How Many Images the Post contains
        /// </returns>
        public List<string> GetAllPostIds(string xmlPayload)
        {
            var postIds = new List<string>();

            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                postIds.AddRange(from DataRow row in dataSet.Tables["post"].Rows select row["id"].ToString());
            }
            catch (Exception)
            {
                return postIds;
            }

            return postIds;
        }

        /// <summary>
        /// Count The Images from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns How Many Images the Post contains
        /// </returns>
        public int CountImagesFromXML(string xmlPayload)
        {
            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in dataSet.Tables["post"].Rows)
                {
                    return Convert.ToInt32(row["imageCount"]);
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// The extract forum title from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns the Forum Title
        /// </returns>
        public string ExtractForumTitleFromXML(string xmlPayload)
        {
            var forumTitle = string.Empty;

            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in dataSet.Tables["forum"].Rows)
                {
                    forumTitle = row["title"].ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            Utility.RemoveIllegalCharecters(forumTitle);

            return forumTitle;
        }

        /// <summary>
        /// The extract post title from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns the Post Title
        /// </returns>
        public string ExtractPostTitleFromXML(string xmlPayload)
        {
            var postTitle = string.Empty;

            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in dataSet.Tables["post"].Rows)
                {
                    postTitle = row["title"].ToString();

                    if (postTitle == string.Empty)
                    {
                        postTitle = $"post# {row["id"]}";
                    }
                    else if (postTitle == $"Re: {this.ExtractTopicTitleFromXML(xmlPayload)}")
                    {
                        postTitle = $"post# {row["id"]}";
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            Utility.RemoveIllegalCharecters(postTitle);

            return postTitle;
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
            var postId = url.Substring(url.IndexOf("p=", StringComparison.Ordinal) + 2);

            var check =
                $@"<h2 class=\""title icon\"">\r\n\t\t\t\t\t(?<inner>[^\r]*)\r\n\t\t\t\t</h2>\r\n\t\t\t\t\r\n\r\n\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t<div class=\""content\"">\r\n\t\t\t\t\t<div id=\""post_message_{postId}\"">";

            var check2 =
                $@"<h2 class=\""title icon\"">\r\n\t\t\t\t\t(?<inner>[^\r]*)\r\n\t\t\t\t</h2>\r\n\t\t\t\t\r\n\r\n\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t<div class=\""content\"">\r\n\t\t\t\t\t<div id=\""post_message_{postId}\"">";

            var match = Regex.Match(content, check, RegexOptions.Compiled);

            var postTitle = string.Empty;

            if (!match.Success)
            {
                match = Regex.Match(content, check2, RegexOptions.Compiled);

                if (!match.Success)
                {
                    return postTitle;
                }
            }

            postTitle = match.Groups["inner"].Value.Trim();

            if (postTitle == string.Empty)
            {
                postTitle = $"post# {postId}";
            }
            else if (postTitle == $"Re: {this.ExtractTopicTitleFromHtml(content)}")
            {
                postTitle = $"post# {postId}";
            }

            // Remove Topic Icons if found
            if (postTitle.Contains("<img"))
            {
                postTitle = postTitle.Substring(postTitle.IndexOf(" /> ", StringComparison.Ordinal) + 4);
            }

            return Utility.ReplaceHexWithAscii(postTitle);
        }

        /// <summary>
        /// Extracts the topic title from XML.
        /// </summary>
        /// <param name="xmlPayload">The xml payload.</param>
        /// <returns>
        /// Returns the topic Title
        /// </returns>
        public string ExtractTopicTitleFromXML(string xmlPayload)
        {
            var title = string.Empty;

            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in dataSet.Tables["thread"].Rows)
                {
                    title = row["title"].ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return Utility.RemoveIllegalCharecters(title);
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

            if (title.Contains(" - Page"))
            {
                title = title.Remove(title.IndexOf(" - Page", StringComparison.Ordinal));
            }

            return title.Trim();
        }

        #endregion
    }
}