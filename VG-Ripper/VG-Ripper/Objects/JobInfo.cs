// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobInfo.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// The job info.
    /// </summary>
    public class JobInfo : object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobInfo" /> class.
        /// </summary>
        public JobInfo()
        {
            this.PostIds = new List<string>();
        }

        #region Properties

        /// <summary>
        /// Gets or sets Forum Title.
        /// </summary>
        public string ForumTitle { get; set; }

        /// <summary>
        /// Gets or sets Image Count.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// Gets or sets Post Title.
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// Gets or sets Store Path.
        /// </summary>
        public string StorePath { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets XML Pay Load.
        /// </summary>
        public string XMLPayLoad { get; set; }

        /// <summary>
        /// Gets or sets the post ids.
        /// </summary>
        /// <value>
        /// The post ids.
        /// </value>
        public List<string> PostIds { get; set; }

        #endregion
    }
}