// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookiePair.cs" company="The Watcher">
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
    /// <summary>
    /// Originally intended to be a struct hence the _s suffix
    /// </summary>
    public class CookiePair
    {
        /// <summary>
        /// Gets or sets Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        public string Value { get; set; }
    }
}