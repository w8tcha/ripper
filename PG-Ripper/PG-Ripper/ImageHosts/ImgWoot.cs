﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgWoot.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.ImageHosts
{
    using System.Collections;

    /// <summary>
    /// Worker class to get images from 
    /// ImgWoot.com/ImgMoney.com/ImgProof.net/PixUp.us/ImgCloud.co/ImGirl.info/GatASexyCity.com/
    /// HosterBin.com/PicsLite.com/ImageTeam.org/ImgNext.com/HostUrImage.com/3XVintage.com
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
        public ImgWoot(ref string savePath, ref string imageUrl, ref string thumbUrl, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
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
            var imageDownloadURL = ThumbImageURL;

            // Set the download Path
            if (ThumbImageURL.Contains("/upload/small/"))
            {
                imageDownloadURL = ThumbImageURL.Replace(@"/upload/small/", @"/upload/big/");
            }
            else if (ThumbImageURL.Contains("/img/small/"))
            {
                imageDownloadURL = ThumbImageURL.Replace(@"/img/small/", @"/img/big/");
            }

            // Set Image Name instead of using random name
            var filePath = this.GetImageName(this.PostTitle, imageDownloadURL, this.ImageNumber);

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}