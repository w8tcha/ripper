// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImagePad.cs" company="The Watcher">
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
    /// Worker class to get images from ImageTwist.com
    /// </summary>
    public class ImagePad : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePad"/> class.
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
        public ImagePad(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref Hashtable hTbl)
            : base(sSavePath, strURL, thumbURL, imageName, ref hTbl)
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

            string sPage = GetImageHostPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            string strNewURL;

            var m = Regex.Match(sPage, @"src=\""(?<inner>[^\""]*)\"" class=""pic""", RegexOptions.Singleline);

            if (m.Success)
            {
                strNewURL = m.Groups["inner"].Value;
            }
            else
            {
                return false;
            }

            strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("/") + 1).Replace(".html", string.Empty);

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

        //////////////////////////////////////////////////////////////////////////
    }
}