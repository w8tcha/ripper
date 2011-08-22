//////////////////////////////////////////////////////////////////////////
// Code Named: RiP-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

using System.Collections.Generic;
using System.Net;

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
	/// Ripper ONLY handles Level one indices (Index -> post or Index -> Thread)
	/// and NOT (Index -> Index) !
	/// This class gets all the thread pages of the index and parses them to extract
	/// links to showthread.php or showpost.php
	/// </summary>
	public class Indexes
	{
	    public string GetPostPages(string sURL)
		{		
			return GetRipPage(sURL);
		}

		private static string GetRipPage(string sURL)
		{
			string sPageRead;

            WebClient wc = new WebClient();

			try
			{
                wc.Headers.Add("Cookie: " + CookieManager.GetInstance().GetCookieString());

                sPageRead = wc.DownloadString(sURL);
			}
			finally
			{
                wc.Dispose();
			}
			
			return sPageRead;
		}
        public List< ImageInfo > ParseXML( string s )
        {
            return Utility.ExtractRiPUrls(s);
        }
	}
}
