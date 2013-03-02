// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageInfo.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper.Objects
{
    /// <summary>
    /// Provides the Image URL 
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// Gets or sets Thumbnail Url.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets Image Url.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
