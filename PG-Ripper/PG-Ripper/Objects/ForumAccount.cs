// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForumAccount.cs" company="The Watcher">
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
    /// <summary>
    /// The Forum Account.
    /// </summary>
    public class ForumAccount : object
    {
        #region Properties

        /// <summary>
        /// Gets or sets the forum URL.
        /// </summary>
        /// <value>
        /// The forum URL.
        /// </value>
        public string ForumURL { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user pass word.
        /// </summary>
        /// <value>
        /// The user pass word.
        /// </value>
        public string UserPassWord { get; set; }

        #endregion
    }
}