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
        /// Gets or sets iImageCount.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// Gets or sets lImageList.
        /// </summary>
        public List<ImageInfo> ImageList { get; set; }

        /// <summary>
        /// Gets or sets sForumTitle.
        /// </summary>
        public string ForumTitle { get; set; }

        /// <summary>
        /// Gets or sets sHtmlPayLoad.
        /// </summary>
        public string HtmlPayLoad { get; set; }

        /// <summary>
        /// Gets or sets sPostTitle.
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// Gets or sets sSecurityToken.
        /// </summary>
        public string SecurityToken { get; set; }

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

        #endregion
    }
}