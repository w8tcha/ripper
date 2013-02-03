// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobInfo.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PGRipper.Objects
{
    #region

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The job info.
    /// </summary>
    public class JobInfo : object
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image Count.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// Gets or sets Image List.
        /// </summary>
        public List<ImageInfo> ImageList { get; set; }

        /// <summary>
        /// Gets or sets Forum Title.
        /// </summary>
        public string ForumTitle { get; set; }

        /// <summary>
        /// Gets or sets Html PayLoad.
        /// </summary>
        public string HtmlPayLoad { get; set; }

        /// <summary>
        /// Gets or sets Post Title.
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// Gets or sets Security Token.
        /// </summary>
        public string SecurityToken { get; set; }

        /// <summary>
        /// Gets or sets Store Path.
        /// </summary>
        public string StorePath { get; set; }

        /// <summary>
        /// Gets or sets Title (Thread Name).
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }

        #endregion
    }
}