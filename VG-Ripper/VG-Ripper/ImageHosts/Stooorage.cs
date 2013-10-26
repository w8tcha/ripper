// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stooorage.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //  This software is licensed under the MIT license. See license.txt for details.
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
    using System.Threading;

    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from Stooorage.com
    /// </summary>
    public class Stooorage : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stooorage"/> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageHostURL">The image host URL.</param>
        /// <param name="thumbURL">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashTable">The hash table.</param>
        public Stooorage(ref string savePath, ref string imageHostURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashTable)
            : base(savePath, imageHostURL, thumbURL, imageName, imageNumber, ref hashTable)
        {
        }

        /// <summary>
        /// Do the image Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected override bool DoDownload()
        {
            var imageHostURL = ImageLinkURL;
            var filePath = string.Empty;

            if (EventTable.ContainsKey(imageHostURL))
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

            var cacheObject = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = imageHostURL };

            try
            {
                EventTable.Add(imageHostURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (EventTable.ContainsKey(imageHostURL))
                {
                    return false;
                }

                EventTable.Add(imageHostURL, cacheObject);
            }

            var imageDownloadURL = this.ThumbImageURL.Replace("/thumbs/", "/images/").Replace("http://t", "http://img");

            filePath = imageDownloadURL.Substring(imageDownloadURL.IndexOf("_") + 1);

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            //////////////////////////////////////////////////////////////////////////

            var newAlteredPath = Utility.GetSuitableName(filePath);

            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                var client = new WebClient();

                client.Headers.Add(string.Format("Referer: {0}", imageHostURL));
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                
                client.DownloadFile(imageDownloadURL, filePath);

                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[imageHostURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[imageHostURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[imageHostURL]).IsDownloaded = false;
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