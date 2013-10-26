// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CelebSweet.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.ImageHosts
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from CelebSweet.6te.net
    /// </summary>
    public class CelebSweet : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CelebSweet" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public CelebSweet(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, imageNumber, ref hashtable)
        {
        }

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected override bool DoDownload()
        {
            var imageURL = ImageLinkURL;
            var filePath = string.Empty;

            if (EventTable.ContainsKey(imageURL))
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

            var cacheObject = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = imageURL };

            try
            {
                EventTable.Add(imageURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (EventTable.ContainsKey(imageURL))
                {
                    return false;
                }

                EventTable.Add(imageURL, cacheObject);
            }

            var page = GetImageHostPage(ref imageURL);

            if (page.Length < 10)
            {
                return false;
            }

            string imageDownloadURL = imageURL.Replace("share.php", "image.php");

            var match = Regex.Match(page, @"title=(?<title>.*?) - ", RegexOptions.Compiled);

            if (match.Success)
            {
                filePath = match.Groups["title"].Value;
            }
            else
            {
                return false;
            }

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            if (!Directory.Exists(this.SavePath))
            {
                Directory.CreateDirectory(this.SavePath);
            }

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[this.ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add(string.Format("Referer: {0}", imageURL));
                webClient.DownloadFile(imageDownloadURL, filePath);
                webClient.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)EventTable[this.ImageLinkURL]).IsDownloaded = true;
            CacheController.Instance().LastPic = ((CacheObject)EventTable[this.ImageLinkURL]).FilePath = filePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}