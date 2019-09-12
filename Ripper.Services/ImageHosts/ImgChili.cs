// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgChili.cs" company="The Watcher">
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

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images from 
    /// ImgChili.com
    /// </summary>
    public class ImgChili : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgChili" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImgChili(
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
        /// Return if Downloaded or not
        /// </returns>
        protected override bool DoDownload()
        {
            var imageDownloadURL = this.ThumbImageURL;

            if (string.IsNullOrEmpty(imageDownloadURL))
            {
                ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = false;
                return false;
            }

            // Set the download Path
            imageDownloadURL = imageDownloadURL.Replace(@"http://t", @"http://i");

            // Set Image Name
            var filePath = this.ImageLinkURL.Substring(this.ImageLinkURL.IndexOf("_", StringComparison.Ordinal) + 1);

            var fileExtension = this.ImageLinkURL.Substring(this.ImageLinkURL.LastIndexOf(".", StringComparison.Ordinal));

            imageDownloadURL = imageDownloadURL.Remove(imageDownloadURL.LastIndexOf(".", StringComparison.Ordinal))
                               + fileExtension;

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}