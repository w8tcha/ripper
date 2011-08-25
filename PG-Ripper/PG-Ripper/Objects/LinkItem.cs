//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on PG forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher 
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG-Ripper project base.

namespace PGRipper
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