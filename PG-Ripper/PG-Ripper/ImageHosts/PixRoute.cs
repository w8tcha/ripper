// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PixRoute.cs" company="The Watcher">
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
    /// Worker class to get images from PixRoute.com
    /// </summary>
    public class PixRoute : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixRoute"/> class.
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
        public PixRoute(ref string savePath, ref string imageUrl, ref string imageName, ref Hashtable hashtable)
            : base(savePath, imageUrl, imageName, ref hashtable)
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
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
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

            string sPage = this.GetImageHostsPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"<A HREF=\""javascript:loodfic\(\)\"" ><img src=\""(?<inner>[^\""]*)\""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("/", StringComparison.Ordinal) + 1).Replace(".html", string.Empty);

            strFilePath = Path.Combine(SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
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
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return false;
            }

            ((CacheObject)EventTable[ImageLinkURL]).IsDownloaded = true;
            CacheController.GetInstance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

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
                req.Headers["Cookie"] = "pixr8263_s=loaded;";
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

        //////////////////////////////////////////////////////////////////////////
    }
}