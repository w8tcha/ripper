// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgBabes.cs" company="The Watcher">
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
    using System.Text.RegularExpressions;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images from IMGBabes.com
    /// </summary>
    public class ImgBabes : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgBabes" /> class.
        /// </summary>
        /// <param name="savePath">The save Path.</param>
        /// <param name="imageUrl">The image Url.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashtable">The hash table.</param>
        public ImgBabes(
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

            // Get Image Link
            var page = this.GetImageHostPage(ref imageURL);

            if (page.Length < 10)
            {
                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            string imageDownloadURL;

            var match = Regex.Match(page, @"img src=\""(?<inner>[^\""]*)\"" class=\""pic\""", RegexOptions.Compiled);

            if (match.Success)
            {
                imageDownloadURL = match.Groups["inner"].Value;
            }
            else
            {
                ((CacheObject)this.EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            // Set Image Name instead of using random name
            var filePath = this.GetImageName(this.PostTitle, imageDownloadURL, this.ImageNumber);

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}