// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlowFoto.cs" company="The Watcher">
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
    /// Worker class to get images from GlowFoto.com
    /// </summary>
    public class GlowFoto : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlowFoto"/> class.
        /// </summary>
        /// <param name="sSavePath">The s save path.</param>
        /// <param name="strURL">The STR URL.</param>
        /// <param name="hTbl">The h TBL.</param>
        public GlowFoto(
            ref string sSavePath,
            ref string strURL,
            ref string thumbURL,
            ref string imageName,
            ref int imageNumber,
            ref Hashtable hashtable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashtable)
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
            var strImgURL = this.ImageLinkURL;

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
                // MainForm.DeleteMessage = ex.Message;
                // MainForm.Delete = true;
                return false;
            }

            var filePath = string.Empty;

            var cCObj = new CacheObject { IsDownloaded = false, FilePath = filePath, Url = strImgURL };

            try
            {
                this.EventTable.Add(strImgURL, cCObj);
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

                this.EventTable.Add(strImgURL, cCObj);
            }

            string newURL;

            var m = Regex.Match(
                strImgURL,
                @"img=(?<img>[^&]*)&y=(?<year>[0-9]*)&m=(?<month>[0-9]*)&t=(?<type>[^&]*)&rand=(?<rand>([0-9]*))",
                RegexOptions.Singleline);

            if (m.Success)
            {
                var img = m.Groups["img"].Value.Remove(m.Groups["img"].Value.Length - 1);
                filePath = $"{img}{m.Groups["rand"].Value}L.{m.Groups["type"].Value}";

                if (strImgURL.Contains("&srv="))
                {
                    newURL = string.Format(
                        "http://{5}.glowfoto.com/images/{0}/{1}/{2}{3}L.{4}",
                        m.Groups["year"].Value,
                        m.Groups["month"].Value,
                        img,
                        m.Groups["rand"].Value,
                        m.Groups["type"].Value,
                        strImgURL.Substring(strImgURL.IndexOf("&srv=") + 5));
                }
                else
                {
                    newURL =
                        $"http://www.glowfoto.com/images/{m.Groups["year"].Value}/{m.Groups["month"].Value}/{img}{m.Groups["rand"].Value}L.{m.Groups["type"].Value}";
                }
            }
            else
            {
                return false;
            }

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            //////////////////////////////////////////////////////////////////////////
            var newAlteredPath = Utility.GetSuitableName(filePath);
            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = filePath;
            }

            try
            {
                var client = new WebClient();
                client.Headers.Add($"Referer: {strImgURL}");
                client.Headers.Add(
                    "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(newURL, filePath);
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
            CacheController.Instance().LastPic = ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = filePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}
