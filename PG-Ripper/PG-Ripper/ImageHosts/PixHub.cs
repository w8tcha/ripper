// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PixHub.cs" company="The Watcher">
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
    /// Worker class to get images from PixHub.eu
    /// </summary>
    public class PixHub : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixHub"/> class.
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
        public PixHub(ref string savePath, ref string imageUrl, ref Hashtable hashtable)
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
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

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

            var cookieValue = this.GetCookieValue(strImgURL);

            string sPage = this.GetImageHostsPage(ref strImgURL, cookieValue);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"img src=\""(?<inner>[^\""]*)\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;

                if (strNewURL.Contains("thumbnail"))
                {
                    strNewURL = strNewURL.Replace("thumbnail/", string.Empty);
                }
            }
            else
            {
                return false;
            }

            strFilePath = strNewURL.Substring(strNewURL.IndexOf("_", StringComparison.Ordinal) + 1);

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
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

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

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL">
        /// The str URL.
        /// </param>
        /// <param name="cookieValue">
        /// The cookie Value.
        /// </param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostsPage(ref string strURL, string cookieValue)
        {
            string strPageRead;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.Headers["Cookie"] = string.Format("ads_pixhub={0};", cookieValue);
                req.Referer = strURL;

                var res = (HttpWebResponse)req.GetResponse();

                var stream = res.GetResponseStream();

                if (stream != null)
                {
                    var reader = new StreamReader(stream);

                    strPageRead = reader.ReadToEnd();

                    res.Close();
                    reader.Close();
                }
                else
                {
                    res.Close();
                    return string.Empty;
                }
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

        /// <summary>
        /// Gets the cookie value.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the Cookie Value</returns>
        private string GetCookieValue(string url)
        {
            var webClient = new WebClient();

            var page = webClient.DownloadString(url);

            var m = Regex.Match(page, @"writeCookie\('ads_pixhub', '(?<inner>[^']*)', '1'\)", RegexOptions.Singleline);

            return m.Success ? m.Groups["inner"].Value : string.Empty;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}