// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageEer.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
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
    using System.Net;
    using System.Text.RegularExpressions;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images from ImageEer.com
    /// </summary>
    public class ImageEer : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEer" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImageEer(
            ref string savePath,
            ref string imageUrl,
            ref string thumbUrl,
            ref string imageName,
            ref int imageNumber,
            ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, imageNumber, ref hashtable)
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
            var imageURL = this.ImageLinkURL;

            var cookieValue =
                imageURL.Remove(imageURL.LastIndexOf("/", StringComparison.Ordinal))
                    .Replace("http://imageeer.com/", string.Empty);

            // Get Image Link
            var page = this.GetImageHostPage(
                ref imageURL,
                WebRequestMethods.Http.Post,
                $"op=view&id={cookieValue}&pre=1&btn=next");

            if (page.Length < 10)
            {
                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            string imageDownloadURL, filePath;

            var match = Regex.Match(page, @"src=\""(?<inner>[^\""]*)\"" class=""pic""", RegexOptions.Compiled);

            if (match.Success)
            {
                imageDownloadURL = match.Groups["inner"].Value.Replace("&amp;", "&");

                // Set Image Name 
                filePath = imageURL.Substring(imageURL.LastIndexOf("/", StringComparison.Ordinal) + 1)
                    .Replace(".htm", string.Empty);
            }
            else
            {
                // Set the download Path
                imageDownloadURL = this.ThumbImageURL.Replace("_t.", ".");

                // Set Image Name instead of using random name
                filePath = this.GetImageName(this.PostTitle, imageDownloadURL, this.ImageNumber);
            }

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}