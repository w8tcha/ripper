//////////////////////////////////////////////////////////////////////////
// Code Named: VG-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
// them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// <copyright company="The Watcher" file="ImageBam.cs">
//  Copyright (c) The Watcher. Partial Rights Reserved.
// </copyright>
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

namespace RiPRipper.ImageHosts
{
    #region

    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;

    using RiPRipper.Objects;

    #endregion

    /// <summary>
    /// Worker class to get images hosted on ImageBam.com
    /// </summary>
    public class ImageBam : ServiceTemplate
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBam" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImageBam(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, ref hashtable)
        {
            // Add constructor logic here
        }

        #endregion

        #region Methods

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected override bool DoDownload()
        {
            string imageURL = this.ImageLinkURL;

            if (this.EventTable.ContainsKey(imageURL))
            {
                return true;
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
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return false;
            }

            var filePath = string.Empty;

            CacheObject cacheObject = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = imageURL };

            try
            {
                this.EventTable.Add(imageURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (this.EventTable.ContainsKey(imageURL))
                {
                    return false;
                }

                this.EventTable.Add(imageURL, cacheObject);
            }

            string pageContent = this.GetImageHostPage(ref imageURL);

            if (pageContent.Length < 10)
            {
                return false;
            }

            string imageDownloadURL;

            var m = Regex.Match(pageContent, @";\"" src=\""(?<inner>[^\""]*)\"" alt=\""loading\""", RegexOptions.Singleline);

            if (m.Success)
            {
                imageDownloadURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            if (imageDownloadURL.Contains("filename="))
            {
                imageDownloadURL = HttpUtility.HtmlDecode(imageDownloadURL);
            }

            // Set Image Name instead of using random name
            filePath = this.GetImageName(this.PostTitle, imageDownloadURL);

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            //////////////////////////////////////////////////////////////////////////
            string sNewAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != sNewAlteredPath)
            {
                filePath = sNewAlteredPath;
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", imageURL));
                client.DownloadFile(imageDownloadURL, filePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = true;
            CacheController.Instance().LastPic =
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = filePath;

            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
    }
}