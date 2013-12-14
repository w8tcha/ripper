// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForumAccount.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Core.Objects
{
    /// <summary>
    /// The Forum Account.
    /// </summary>
    public class ForumAccount : object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForumAccount"/> class.
        /// </summary>
        public ForumAccount()
        {
            this.GuestAccount = false;
        }

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

        /// <summary>
        /// Gets or sets a value indicating whether [guest account].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [guest account]; otherwise, <c>false</c>.
        /// </value>
        public bool GuestAccount { get; set; }

        #endregion
    }
}