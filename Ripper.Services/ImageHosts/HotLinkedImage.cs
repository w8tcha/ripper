// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotLinkedImage.cs" company="The Watcher">
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
    using System.Collections;

    using Ripper.Core.Components;

    /// <summary>
    /// Worker class to get images from a HotLinked Image
    /// </summary>
    public class HotLinkedImage : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotLinkedImage" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashtable">The hash table.</param>
        public HotLinkedImage(
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
            var imageDownloadURL = ImageLinkURL;

            // Set Image Name instead of using random name
            var filePath = this.GetImageName(this.PostTitle, imageDownloadURL, this.ImageNumber);

            // Finally Download the Image
            return this.DownloadImageAsync(
                imageDownloadURL,
                filePath,
                false,
                imageDownloadURL.Contains(CacheController.Instance().UserSettings.CurrentForumUrl));
        }
    }
}