﻿//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
// them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// <copyright company="The Watcher" file="ImageBam.cs">
//  Copyright (c) The Watcher. Partial Rights Reserved.
// </copyright>
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG Ripper project base.

namespace PGRipper.ImageHosts
{
    #region

    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Web;

    using PGRipper.Objects;

    #endregion

    /// <summary>
    /// Worker class to get images hosted on ImageBam.com
    /// </summary>
    public class ImageBam : ServiceTemplate
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBam"/> class.
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
        public ImageBam(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
            // Add constructor logic here
        }

        #endregion

        #region Methods

        /// <summary>
        /// The do download.
        /// </summary>
        /// <returns>
        /// The do download.
        /// </returns>
        protected override bool DoDownload()
        {
            string strImgURL = this.mstrURL;

            if (this.eventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            try
            {
                if (!Directory.Exists(this.mSavePath))
                {
                    Directory.CreateDirectory(this.mSavePath);
                }
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                return false;
            }

            string strFilePath = string.Empty;

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                this.eventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (this.eventTable.ContainsKey(strImgURL))
                {
                    return false;
                }

                this.eventTable.Add(strImgURL, ccObj);
            }

            string strIvPage = this.GetImageHostPage(ref strImgURL);

            if (strIvPage.Length < 10)
            {
                return false;
            }

            const string Start = ";\" src=\"";

            int iStartSrc = strIvPage.IndexOf(Start);

            if (iStartSrc < 0)
            {
                return false;
            }

            iStartSrc += Start.Length;

            int iEndSrc = strIvPage.IndexOf("\"", iStartSrc);

            if (iEndSrc < 0)
            {
                return false;
            }

            string strNewURL = HttpUtility.HtmlDecode(strIvPage.Substring(iStartSrc, iEndSrc - iStartSrc));

            strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("/") + 1);

            if (strFilePath.Contains("filename="))
            {
                strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("=") + 1);
            }

            strFilePath = Path.Combine(this.mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////
            string sNewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != sNewAlteredPath)
            {
                strFilePath = sNewAlteredPath;
                ((CacheObject)this.eventTable[this.mstrURL]).FilePath = strFilePath;
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", strImgURL));
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);

                return false;
            }

            ((CacheObject)this.eventTable[this.mstrURL]).IsDownloaded = true;
            CacheController.GetInstance().uSLastPic =
                ((CacheObject)this.eventTable[this.mstrURL]).FilePath = strFilePath;

            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
    }
}