//////////////////////////////////////////////////////////////////////////
// Code Named: RiP-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

namespace RiPRipper.Objects
{
    /// <summary>
    /// The Cache Object Item.
    /// </summary>
    public class CacheObject
    {
        /// <summary>
        /// Gets or sets FilePath.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDownloaded.
        /// </summary>
        public bool IsDownloaded { get; set; }
    }
}