// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkItem.cs" company="The Watcher">
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
    /// A Link Item Class
    /// </summary>
    public class LinkItem
    {
        #region Properties

        /// <summary>
        ///   Gets or sets Href.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        public string Text { get; set; }

        #endregion
    }
}