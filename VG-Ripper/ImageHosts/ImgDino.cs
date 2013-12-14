﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgDino.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.ImageHosts
{
    using System.Collections;
    using System.Text.RegularExpressions;

    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from ImgDino.com/ImgTiger.com
    /// </summary>
    public class ImgDino : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgDino" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImgDino(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(savePath, imageUrl, thumbUrl, imageName, imageNumber, ref hashtable)
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
            string imageDownloadURL;

            var imageURL = ImageLinkURL;

            // Get Image Link
            var page = GetImageHostPage(ref imageURL);

            if (page.Length < 10)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            var match = Regex.Match(
                page,
                @"onclick=\""scale\(this\);\"" src=\""(?<inner>[^\""]*)\""",
                RegexOptions.Compiled);

            if (match.Success)
            {
                imageDownloadURL = match.Groups["inner"].Value.Replace("&amp;", "&");
            }
            else
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            // Set Image Name instead of using random name
            var filePath = this.GetImageName(this.PostTitle, imageDownloadURL, this.ImageNumber);

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}