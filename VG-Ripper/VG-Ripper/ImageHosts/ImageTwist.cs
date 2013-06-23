// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageTwist.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper.ImageHosts
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImageTwist.com
    /// </summary>
    public class ImageTwist : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTwist" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageHostURL">The image Host URL</param>
        /// <param name="thumbURL">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImageTwist(ref string savePath, ref string imageHostURL, ref string thumbURL, ref string imageName, ref Hashtable hashTable)
            : base(savePath, imageHostURL, thumbURL, imageName, ref hashTable)
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

            if (EventTable.ContainsKey(imageURL))
            {
                return true;
            }

            var filePath = string.Empty;

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

            string sPage = GetImageHostPage(ref imageURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"src=\""(?<inner>[^\""]*)\"" class=""pic""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            filePath = imageURL.Substring(imageURL.LastIndexOf("/", StringComparison.Ordinal) + 1).Replace(".html", string.Empty);

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", imageURL));
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, filePath);
                client.Dispose();
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