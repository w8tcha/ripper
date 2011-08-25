//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on PG forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher 
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG-Ripper project base.

namespace PGRipper.Objects
{
    /// <summary>
    /// Provides the Image URL 
    /// </summary>
    public class ImageInfo : object
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
