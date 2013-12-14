// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hoooster.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Services.ImageHosts
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images from Hooster.com
    /// </summary>
    public class Hoooster : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hoooster" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public Hoooster(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, imageNumber, ref hashtable)
        {
        }

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Return if Downloaded or not
        /// </returns>
        protected override bool DoDownload()
        {
            string strImgURL = ImageLinkURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

            try
            {
                if (!Directory.Exists(this.SavePath))
                {
                    Directory.CreateDirectory(this.SavePath);
                }
            }
            catch (IOException ex)
            {
                //MainForm.DeleteMessage = ex.Message;
                //MainForm.Delete = true;

                return false;
            }

            var cacheObject = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                EventTable.Add(strImgURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (EventTable.ContainsKey(strImgURL))
                {
                    return false;
                }

                EventTable.Add(strImgURL, cacheObject);
            }

            var page = GetImageHostPage(ref strImgURL, "hoosterads=1;");

            if (page.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(page, @"img src=\""(?<inner>[^\""]*)\"" alt=\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            if (strNewURL.StartsWith(" "))
            {
                strNewURL = strNewURL.Replace(" ", string.Empty);
            }

            strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("/", StringComparison.Ordinal) + 1);

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)EventTable[this.ImageLinkURL]).FilePath = strFilePath;
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", strImgURL));
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                //MainForm.DeleteMessage = ex.Message;
                //MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)EventTable[ImageLinkURL]).IsDownloaded = true;
            CacheController.Instance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}