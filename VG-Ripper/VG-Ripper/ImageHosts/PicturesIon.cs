﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PicturesIon.cs" company="The Watcher">
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
    using System.Collections;

    /// <summary>
    /// Worker class to get images from PicturesIon.com
    /// </summary>
    public class PicturesIon : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesIon" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashtable">The hash table.</param>
        public PicturesIon(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref Hashtable hashtable)
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
            // Set the download Path
            var imageDownloadURL = ThumbImageURL.Replace("/thumbs/", "/");

            // Set Image Name instead of using random name
            var filePath = this.GetImageName(this.PostTitle, imageDownloadURL);

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}