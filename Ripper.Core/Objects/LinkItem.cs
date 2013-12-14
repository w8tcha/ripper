// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkItem.cs" company="The Watcher">
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
    /// A Link Item Class
    /// </summary>
    public class LinkItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkItem" /> class.
        /// </summary>
        public LinkItem()
        {
            this.Href = string.Empty;
            this.Text = string.Empty;
        }

        #region Properties

        /// <summary>
        ///   Gets or sets Link.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        public string Text { get; set; }

        #endregion
    }
}