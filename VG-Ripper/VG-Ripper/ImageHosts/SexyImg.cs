// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SexyImg.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
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
    /// Worker class to get images from SexyImg.com
    /// </summary>
    public class SexyImg : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SexyImg" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public SexyImg(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, ref hashtable)
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
            var imageURL = ImageLinkURL;
            var filePath = string.Empty;

            if (EventTable.ContainsKey(imageURL))
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

            var cacheObject = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = imageURL };

            try
            {
                EventTable.Add(imageURL, cacheObject);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (EventTable.ContainsKey(imageURL))
                {
                    return false;
                }

                EventTable.Add(imageURL, cacheObject);
            }

            imageURL = imageURL.Replace(".com/s/", ".com/b/");

            var page = this.GetImageHostsPage(ref imageURL);

            if (page.Length < 10)
            {
                return false;
            }

            string imageDownloadURL = imageURL;

            var matchTitle = Regex.Match(page, @"\<h1\>(?<title>.*?)\</h1>", RegexOptions.Compiled);

            if (matchTitle.Success)
            {
                filePath = matchTitle.Groups["title"].Value;
            }
            else
            {
                return false;
            }

            var matchUrl = Regex.Match(page, @"class=\""bigimg\"" src=\""(?<inner>[^\""]*)\""", RegexOptions.Compiled);

            if (matchUrl.Success)
            {
                imageDownloadURL = matchUrl.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            if (!Directory.Exists(this.SavePath))
            {
                Directory.CreateDirectory(this.SavePath);
            }

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[this.ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add(string.Format("Referer: {0}", imageURL));
                webClient.DownloadFile(imageDownloadURL, filePath);
                webClient.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)EventTable[this.ImageLinkURL]).IsDownloaded = true;
            CacheController.Instance().LastPic = ((CacheObject)EventTable[this.ImageLinkURL]).FilePath = filePath;

            return true;
        }

        /// <summary>
        /// Generic function to fetch URLs.
        /// </summary>
        /// <param name="imageHostUrl">The image host URL.</param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostsPage(ref string imageHostUrl)
        {
            string strPageRead;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(imageHostUrl);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.Headers["Cookie"] = "PHPSESSID=123456789;";
                req.Referer = imageHostUrl;

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