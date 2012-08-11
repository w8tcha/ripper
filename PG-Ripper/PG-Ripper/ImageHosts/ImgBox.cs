// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgBox.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PGRipper.ImageHosts
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImgBox.com
    /// </summary>
    public class ImgBox : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgBox"/> class.
        /// </summary>
        /// <param name="savePath">
        /// The save Path.
        /// </param>
        /// <param name="imageUrl">
        /// The image Url.
        /// </param>
        /// <param name="hashtable">
        /// The hashtable.
        /// </param>
        public ImgBox(ref string savePath, ref string imageUrl, ref Hashtable hashtable)
            : base(savePath, imageUrl, ref hashtable)
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
            string strImgURL = mstrURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            var filePath = string.Empty;

            try
            {
                if (!Directory.Exists(this.mSavePath))
                {
                    Directory.CreateDirectory(this.mSavePath);
                }
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return false;
            }

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = strImgURL };

            try
            {
                EventTable.Add(strImgURL, ccObj);
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

                EventTable.Add(strImgURL, ccObj);
            }

            var page = GetImageHostPage(ref strImgURL);

            if (page.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(page, @"id=\""img\"".*?src=\""(?<inner>[^\""]*)\"" title=\""(?<title>[^\""]*)\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value.Replace("&amp;", "&");
                filePath = m.Groups["title"].Value;
            }
            else
            {
                return false;
            }

            filePath = Path.Combine(this.mSavePath, Utility.RemoveIllegalCharecters(filePath));

            if (!Directory.Exists(this.mSavePath))
            {
                Directory.CreateDirectory(this.mSavePath);
            }

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[mstrURL]).FilePath = filePath;
            }

            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add(string.Format("Referer: {0}", strImgURL));
                webClient.DownloadFile(strNewURL, filePath);
                webClient.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (WebException ex)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return false;
            }

            ((CacheObject)EventTable[mstrURL]).IsDownloaded = true;
            CacheController.GetInstance().uSLastPic = ((CacheObject)EventTable[mstrURL]).FilePath = filePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}