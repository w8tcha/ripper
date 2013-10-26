// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadToPost.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System.Collections.Generic;
    using Ripper.Objects;

    /// <summary>
    /// This class gets all the thread pages of the Thread and parse every single post
    /// </summary>
    public class ThreadToPost
    {
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the Rip Urls</returns>
        public List<ImageInfo> ParseXML(string url)
        {
            List<ImageInfo> mArrImgLst = Utility.ExtractThreadtoPosts(url);

            return mArrImgLst;
        }
    }
}