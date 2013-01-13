﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgDino.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //  This software is licensed under the MIT license. See license.txt for details.
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
    using System.Threading;

    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImgDino.com
    /// </summary>
    public class ImgDino : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgDino"/> class.
        /// </summary>
        /// <param name="savePath">
        /// The save Path.
        /// </param>
        /// <param name="imageUrl">
        /// The image Url.
        /// </param>
        /// <param name="hashtable">
        /// The hash table.
        /// </param>
        public ImgDino(ref string savePath, ref string imageUrl, ref Hashtable hashtable)
            : base(savePath, imageUrl, ref hashtable)
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
            var strImgURL = mstrURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

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

            var ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

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

            var strNewURL = strImgURL.Replace(@"viewer.php", @"download.php");

            strFilePath = strNewURL.Substring(strNewURL.IndexOf("=") + 1);

            strFilePath = Path.Combine(this.mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////

            var newAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;
            }

            try
            {
                var client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", strImgURL));
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return false;
            }

            ((CacheObject)EventTable[mstrURL]).IsDownloaded = true;
            CacheController.GetInstance().uSLastPic = ((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}