// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageBam.cs" company="The Watcher">
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
    #region

    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Web;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    #endregion

    /// <summary>
    /// Worker class to get images hosted on ImageBam.com
    /// </summary>
    public class ImageBam : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBam" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageHostURL">The image Host URL</param>
        /// <param name="thumbURL">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImageBam(
            ref string savePath,
            ref string imageHostURL,
            ref string thumbURL,
            ref string imageName,
            ref int imageNumber,
            ref Hashtable hashTable)
            : base(savePath, imageHostURL, thumbURL, imageName, imageNumber, ref hashTable)
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

            //var cookieValue = this.GetCookieValue(imageURL, @"setCookie\(\""ibpuc\"", \""(?<inner>[^\""]*)\"", 1\)");

            //if (string.IsNullOrEmpty(cookieValue))
            //{
            //    return false;
            //}

            //// Get Image Link
            //var page = GetImageHostPage(ref imageURL, string.Format("ibpuc={0};", cookieValue));

            //if (page.Length < 10)
            //{
            //    ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
            //    return false;
            //}

            var page = this.GetImageHostPage(ref imageURL);
            string imageDownloadURL;

            var match = Regex.Match(page, @"src=\""(?<inner>[^\""]*)\"" alt=\""loading\""", RegexOptions.Compiled);

            if (match.Success)
            {
                imageDownloadURL = match.Groups["inner"].Value;
            }
            else
            {

                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            if (imageDownloadURL.Contains("filename="))
            {
                imageDownloadURL = HttpUtility.HtmlDecode(imageDownloadURL);
            }

            var filePath =
                imageDownloadURL.Substring(imageDownloadURL.LastIndexOf("/", System.StringComparison.Ordinal) + 1);

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}