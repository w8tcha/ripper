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

namespace Ripper
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    using Ripper.Objects;

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
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashTable">The url list.</param>
        protected ServiceTemplate(string savePath, string imageUrl, string thumbImageUrl, string postTitle, int imageNumber, ref Hashtable hashTable)
        {
            this.WebClient = new WebClient();
            this.WebClient.DownloadFileCompleted += this.DownloadImageCompleted;

            this.ImageLinkURL = imageUrl;
            this.ThumbImageURL = thumbImageUrl;
            this.EventTable = hashTable;
            this.SavePath = savePath;
            this.PostTitle = postTitle;
            this.ImageNumber = imageNumber;
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
        /// Gets or sets the image number.
        /// </summary>
        /// <value>
        /// The image number.
        /// </value>
        protected int ImageNumber { get; set; }

        /// <summary>
        /// Gets or sets the web client.
        /// </summary>
        /// <value>
        /// The web client.
        /// </value>
        protected WebClient WebClient { get; set; }

        /// <summary>
        /// Start Download
        /// </summary>
        [Obsolete("Please use StartDownloadAsync instead.")]
        public void StartDownload()
        {
            this.DoDownload();

            this.RemoveThread();
        }

        /// <summary>
        /// Start Download Async.
        /// </summary>
        public void StartDownloadAsync()
        {
            if (this.EventTable.ContainsKey(this.ImageLinkURL))
            {
                return;
            }

            var cacheObject = new CacheObject { IsDownloaded = false, FilePath = string.Empty, Url = this.ImageLinkURL };

            try
            {
                this.EventTable.Add(this.ImageLinkURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception)
            {
                if (this.EventTable.ContainsKey(this.ImageLinkURL))
                {
                    return;
                }

                this.EventTable.Add(this.ImageLinkURL, cacheObject);
            }

            try
            {
                if (!Directory.Exists(this.SavePath))
                {
                    Directory.CreateDirectory(this.SavePath);
                }
            }
            catch (IOException ex)
            {
                return;
            }

            if (!this.DoDownload())
            {
                this.RemoveThread();
            }
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
        /// <param name="imageNumber">The image number.</param>
        /// <returns>
        /// Returns the Image Name.
        /// </returns>
        protected string GetImageName(string postTitle, string imageUrl, int imageNumber)
        {
            postTitle = Utility.RemoveIllegalCharecters(postTitle).Replace(" ", "_");

            var imageName = string.Format(
                "{0}_{1}{2}",
                postTitle,
                imageNumber,
                imageUrl.Substring(imageUrl.LastIndexOf(".", StringComparison.Ordinal)));

            // Check if folder path is too long
            var savePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(imageName));

            if (savePath.Length > 250)
            {
                return string.Format(
                "{0}{1}",
                imageNumber,
                imageUrl.Substring(imageUrl.LastIndexOf(".", StringComparison.Ordinal)));
            }

            return imageName;
        }

        /// <summary>
        /// Downloads the image.
        /// </summary>
        /// <param name="downloadPath">The download path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="addReferer">if set to <c>true</c> [add Referer].</param>
        /// <returns>
        /// Returns if the Image was downloaded or not
        /// </returns>
        protected bool DownloadImageAsync(string downloadPath, string savePath, bool addReferer = false)
        {
            savePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(savePath));

            if (!Directory.Exists(this.SavePath))
            {
                Directory.CreateDirectory(this.SavePath);
            }

            savePath = Utility.GetSuitableName(savePath);

            ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = savePath;

            if (addReferer)
            {
                this.WebClient.Headers.Add(string.Format("Referer: {0}", this.ThumbImageURL));
            }

            this.WebClient.DownloadFileAsync(new Uri(downloadPath), savePath);

            return true;
        }

        /// <summary>
        /// Removes the thread.
        /// </summary>
        protected void RemoveThread()
        {
            if (!this.EventTable.Contains(this.ImageLinkURL))
            {
                return;
            }

            this.EventTable.Remove(this.ImageLinkURL);
            ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);
        }

        /// <summary>
        /// Downloads the image completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs" /> instance containing the event data.</param>
        private void DownloadImageCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = true;

                var cacheObject = (CacheObject)this.EventTable[this.ImageLinkURL];

                Application.DoEvents();

                CacheController.Instance().LastPic = cacheObject.FilePath;
            }
            else
            {
                var exception = e.Error;

                //Utility.SaveOnCrash(exception.Message, exception.StackTrace, null);

                ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = false;
            }

            this.RemoveThread();

            Application.DoEvents();
        } 
    }
}