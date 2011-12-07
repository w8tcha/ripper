// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgChili.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
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
    /// Worker class to get images from ImgChili.com
    /// </summary>
    public class ImgChili : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgChili"/> class.
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
        public ImgChili(ref string savePath, ref string imageUrl, ref Hashtable hashtable)
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

            string sPage = this.GetImageHostsPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"onclick=\""scale\(this\);\""  src=\""http://i(?<inner>[^\""]*)\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = string.Format("http://i{0}", m.Groups["inner"].Value);
            }
            else
            {
                return false;
            }

            strFilePath = strNewURL.Substring(strNewURL.IndexOf("_") + 1);

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
                client.Headers["Cookie"] = "chililast=1318769375.861;";
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

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL">
        /// The str URL.
        /// </param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostsPage(ref string strURL)
        {
            string strPageRead;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.Headers["Cookie"] = "chililast=1318769375.861;";
                req.Referer = strURL;

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