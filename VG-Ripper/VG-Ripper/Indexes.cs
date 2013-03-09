// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Indexes.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System.Collections.Generic;
    using RiPRipper.Objects;

    /// <summary>
    /// Ripper ONLY handles Level one indices (Index -> post or Index -> Thread)
    /// and NOT (Index -> Index) !
    /// This class gets all the thread pages of the index and parses them to extract
    /// links to showthread.php or showpost.php
    /// </summary>
    public class Indexes
    {
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the Rip Urls</returns>
        public List<ImageInfo> ParseXML(string url)
        {
            return Utility.ExtractRiPUrls(url);
        }
    }
}
