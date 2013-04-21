// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgWoot.cs" company="The Watcher">
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
    using System.Threading;

    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImgWoot.com/ImgMoney.com/ImgProof.net
    /// </summary>
    public class ImgWoot : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgWoot" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImgWoot(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, ref hashtable)
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
            var imageURL = ImageLinkURL;
            var thumbURL = ThumbImageURL;

            if (EventTable.ContainsKey(imageURL))
            {
                return true;
            }

            var filePath = string.Empty;

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
            
            // Set the download Path
            var imageDownloadURL = thumbURL.Replace(@"/upload/small/", @"/upload/big/");

            // Set Image Name instead of using random name
            filePath = this.GetImageName(this.PostTitle, imageDownloadURL);

            filePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(filePath));

            //////////////////////////////////////////////////////////////////////////

            var newAlteredPath = Utility.GetSuitableName(filePath, true);

            if (filePath != newAlteredPath)
            {
                filePath = newAlteredPath;
                ((CacheObject)EventTable[imageURL]).FilePath = filePath;
            }

            try
            {
                var client = new WebClient();
                client.Headers.Add(string.Format("Referer: {0}", imageURL));
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(imageDownloadURL, filePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(imageURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(imageURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(imageURL);

                return false;
            }

            ((CacheObject)EventTable[imageURL]).IsDownloaded = true;
            CacheController.Instance().LastPic = ((CacheObject)EventTable[imageURL]).FilePath = filePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
    }
}