//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on PG forums and attempts to fetch
//             them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// <copyright file="ImageUpper.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG-Ripper project base.

namespace PGRipper.ImageHosts
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Threading;
    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images from DailyPoa.com
    /// </summary>
    public class ImageUpper : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUpper"/> class.
        /// </summary>
        /// <param name="sSavePath">
        /// The s save path.
        /// </param>
        /// <param name="strURL">
        /// The str url.
        /// </param>
        /// <param name="hTbl">
        /// The h tbl.
        /// </param>
        public ImageUpper(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
        }

        /// <summary>
        ///  Do the Download
        /// </summary>
        /// <returns>
        /// Returns if download was successfull
        /// </returns>
        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (eventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

            try
            {
                if (!Directory.Exists(mSavePath))
                {
                    Directory.CreateDirectory(mSavePath);
                }
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                return false;
            }

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                eventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (eventTable.ContainsKey(strImgURL))
                {
                    return false;
                }

                eventTable.Add(strImgURL, ccObj);
            }

            string startSrc = string.Format(
                "SRC=\"http://{0}", strImgURL.Substring(strImgURL.IndexOf(@"/?") + 2, 3).ToLower());

            string sPage = GetImageHostPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            int iStartSrc = sPage.IndexOf(startSrc);

            if (iStartSrc < 0)
            {
                return false;
            }

            iStartSrc += startSrc.Length;

            int iEndSrc = sPage.IndexOf("\"", iStartSrc);

            if (iEndSrc < 0)
            {
                return false;
            }

            string strNewURL = string.Format(
                "http://{0}{1}",
                strImgURL.Substring(strImgURL.IndexOf(@"/?") + 2, 3).ToLower(),
                sPage.Substring(iStartSrc, iEndSrc - iStartSrc));

            strFilePath = strNewURL.Substring(strImgURL.LastIndexOf("/") + 1);

            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
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
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return false;
            }

            ((CacheObject)eventTable[mstrURL]).IsDownloaded = true;
            CacheController.GetInstance().uSLastPic = ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}