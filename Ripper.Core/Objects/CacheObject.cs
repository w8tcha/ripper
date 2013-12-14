// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheObject.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Core.Objects
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