// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobInfo.cs" company="The Watcher">
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
    /// The job info.
    /// </summary>
    public class JobInfo : object
    {
        #region Properties

        /// <summary>
        /// Gets or sets Forum Title.
        /// </summary>
        public string ForumTitle { get; set; }

        /// <summary>
        /// Gets or sets sImageCount.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// Gets or sets sPostTitle.
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// Gets or sets sStorePath.
        /// </summary>
        public string StorePath { get; set; }

        /// <summary>
        /// Gets or sets sTitle.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets sURL.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets sXMLPayLoad.
        /// </summary>
        public string XMLPayLoad { get; set; }

        #endregion
    }
}