// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareNxs.cs" company="The Watcher">
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
    /// Worker class to get images from ShareNxs.com
    /// </summary>
    public class ShareNxs : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShareNxs"/> class.
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
        public ShareNxs(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashtable)
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
            var strImgURL = this.ImageLinkURL;

            if (this.EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            var strFilePath = string.Empty;

            try
            {
                if (!Directory.Exists(this.SavePath))
                {
                    Directory.CreateDirectory(this.SavePath);
                }
            }
            catch (IOException ex)
            {
                // MainForm.DeleteMessage = ex.Message;
                // MainForm.Delete = true;
                return false;
            }

            var ccObj = new CacheObject
            {
                IsDownloaded = false,
                FilePath = strFilePath,
                Url = strImgURL
            };

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

            var sLargeUrl = $"{strImgURL}&pjk=l".Replace("view/?id", "view/index.php?id");

            var sPage = this.GetImageHostsPage(ref sLargeUrl, strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"src=\""(?<inner>[^\""]*)\"" id=img1", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("/") + 1);

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////
            try
            {
                var sNewAlteredPath = Utility.GetSuitableName(strFilePath);
                if (strFilePath != sNewAlteredPath)
                {
                    strFilePath = sNewAlteredPath;
                    ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;
                }

                var client = new WebClient();
                client.Headers.Add($"Referer: {sLargeUrl}");
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
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
                // MainForm.DeleteMessage = ex.Message;
                // MainForm.Delete = true;
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
            CacheController.Instance().LastPic = ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL">
        /// The str URL.
        /// </param>
        /// <param name="referrerUrl">
        /// The referrer Url.
        /// </param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostsPage(ref string strURL, string referrerUrl)
        {
            string strPageRead;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.Headers["Cookie"] = "ad_redirect_st_nxs";
                req.Referer = referrerUrl;

                var res = (HttpWebResponse)req.GetResponse();

                var stream = res.GetResponseStream();
                var reader = new StreamReader(stream);

                strPageRead = reader.ReadToEnd();

                res.Close();
                reader.Close();
            }
            catch (ThreadAbortException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return strPageRead;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}