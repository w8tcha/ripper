//////////////////////////////////////////////////////////////////////////
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
    using System.Text.RegularExpressions;
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
        public ImageBam(ref string sSavePath, ref string strURL, ref string imageName, ref Hashtable hTbl)
            : base(sSavePath, strURL, imageName, ref hTbl)
        {
            // Add constructor logic here
        }

        #endregion

        #region Methods

        /// <summary>
        /// The do download.
        /// </summary>
        /// <returns>
        /// Returns Value if the Image was downloaded.
        /// </returns>
        protected override bool DoDownload()
        {
            string strImgURL = this.ImageLinkURL;

            if (this.EventTable.ContainsKey(strImgURL))
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

            string strFilePath = string.Empty;

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                this.EventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (this.EventTable.ContainsKey(strImgURL))
                {
                    return false;
                }

                this.EventTable.Add(strImgURL, ccObj);
            }

            string pageContent = this.GetImageHostPage(ref strImgURL);

            if (pageContent.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(pageContent, @";\"" src=\""(?<inner>[^\""]*)\"" alt=\""loading\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("/", StringComparison.Ordinal) + 1);

            if (strNewURL.Contains("amazonaws.com/bambackup/"))
            {
                var fileMatch = Regex.Match(
                    strNewURL, @"bambackup/(?<inner>[^AWS]*)AWSAccessKeyId", RegexOptions.Singleline);

                if (fileMatch.Success)
                {
                    strFilePath = string.Format("{0}.jpg", fileMatch.Groups["inner"].Value);
                }
            }

            if (strNewURL.Contains("filename="))
            {
                strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("=", StringComparison.Ordinal) + 1);
                strNewURL = HttpUtility.HtmlDecode(strNewURL);
            }

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////
            string sNewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != sNewAlteredPath)
            {
                strFilePath = sNewAlteredPath;
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;
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
                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = true;
            CacheController.GetInstance().LastPic =
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
    }
}