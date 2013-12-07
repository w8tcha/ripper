// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgFlare.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
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
    /// Worker class to get images from ImgFlare.com
    /// </summary>
    public class ImgFlare : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgFlare" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageHostURL">The image Host URL</param>
        /// <param name="thumbURL">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImgFlare(ref string savePath, ref string imageHostURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashTable)
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

            // Get Image Link
            var page = GetImageHostPage(ref imageURL);

            if (page.Length < 10)
            {
                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            string imageDownloadURL;

            var match = Regex.Match(
                page,
                @"src=\""(?<inner>[^\""]*)\"" class=""pic""",
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