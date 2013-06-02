// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTemplate.cs" company="The Watcher">
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
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Service Template Class
    /// </summary>
    public abstract class ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplate" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageUrl">The image url.</param>
        /// <param name="thumbImageUrl">The thumb image URL.</param>
        /// <param name="postTitle">The post title</param>
        /// <param name="hashTable">The url list.</param>
        protected ServiceTemplate(string savePath, string imageUrl, string thumbImageUrl, string postTitle, ref Hashtable hashTable)
        {
            this.ImageLinkURL = imageUrl;
            this.ThumbImageURL = thumbImageUrl;
            this.EventTable = hashTable;
            this.SavePath = savePath;
            this.PostTitle = postTitle;
        }

        /// <summary>
        /// Gets or sets the HashTable with URLs.
        /// </summary>
        public Hashtable EventTable { get; set; }

        /// <summary>
        /// Gets or sets the Thumb Image Url
        /// </summary>
        protected string ThumbImageURL { get; set; }

        /// <summary>
        /// Gets or sets the Image Link Url
        /// </summary>
        protected string ImageLinkURL { get; set; }

        /// <summary>
        /// Gets or sets the Image Save Folder Path
        /// </summary>
        protected string SavePath { get; set; }

        /// <summary>
        /// Gets or sets the post title.
        /// </summary>
        /// <value>
        /// The post title.
        /// </value>
        protected string PostTitle { get; set; }

        /// <summary>
        /// Start Download
        /// </summary>
        public void StartDownload()
        {
            this.DoDownload();

            if (this.EventTable[this.ImageLinkURL] != null)
            {
                if (this.EventTable.Contains(this.ImageLinkURL))
                {
                    this.EventTable.Remove(this.ImageLinkURL);
                }
            }

            ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);
        }

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected abstract bool DoDownload();

        /// <summary>
        /// a generic function to fetch URLs.
        /// </summary>
        /// <param name="imageHostURL">The image host URL.</param>
        /// <param name="cookieValue">The cookie value.</param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostPage(
            ref string imageHostURL, string cookieValue = null)
        {
            string pageContent;
           
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(imageHostURL);

                webRequest.UserAgent =
                    "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                webRequest.Referer = imageHostURL;
                webRequest.KeepAlive = true;
                webRequest.Timeout = 2000;

                if (!string.IsNullOrEmpty(cookieValue))
                {
                    webRequest.Headers["Cookie"] = cookieValue;
                }

                var responseStream = webRequest.GetResponse().GetResponseStream();

                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);

                    pageContent = reader.ReadToEnd();

                    responseStream.Close();
                    reader.Close();
                }
                else
                {
                    responseStream.Close();
                    return string.Empty;
                }
            }
            catch (ThreadAbortException)
            {
                pageContent = string.Empty;
            }
            catch (Exception)
            {
                pageContent = string.Empty;
            }

            return pageContent;
        }

        /// <summary>
        /// Gets the cookie value.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="matchString">The match.</param>
        /// <returns>
        /// Returns the Cookie Value
        /// </returns>
        protected string GetCookieValue(string url, string matchString)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.Referer = url;
                req.Timeout = 20000;

                var res = (HttpWebResponse)req.GetResponse();

                var stream = res.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);

                    string page = reader.ReadToEnd();

                    res.Close();
                    reader.Close();

                    var match = Regex.Match(page, matchString, RegexOptions.Singleline);

                    return match.Success ? match.Groups["inner"].Value : string.Empty;
                }
            }
            catch (ThreadAbortException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <param name="postTitle">The post title.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns>Returns the Image Name.</returns>
        protected string GetImageName(string postTitle, string imageUrl)
        {
            postTitle = Utility.RemoveIllegalCharecters(postTitle).Replace(" ", "_");

            return string.Format(
                "{0}{1}", postTitle, imageUrl.Substring(imageUrl.LastIndexOf(".", StringComparison.Ordinal)));
        }
    }
}