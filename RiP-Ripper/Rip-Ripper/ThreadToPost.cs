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

using System;
using System.Collections.Generic;
using System.Net;

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
    /// This class gets all the thread pages of the Thread and parse every single post
    /// </summary>
    public class ThreadToPost
    {
        public string GetPostPages(string sURL)
        {
            return GetRipPage(sURL);
        }
        /// <summary>
        /// Get Current Thread XML Page
        /// </summary>
        /// <param name="strURL"></param>
        /// <returns></returns>
        private static string GetRipPage(string strURL)
        {
            string strPageRead;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                strPageRead = wc.DownloadString(strURL);

                wc.Dispose();
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return strPageRead;
        }

        /// <summary>
        /// Extract all Posts
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<ImageInfo> ParseXML(string s)
        {
            List<ImageInfo> mArrImgLst = Utility.ExtractThreadtoPosts(s);

            return mArrImgLst;
        }
    }
}