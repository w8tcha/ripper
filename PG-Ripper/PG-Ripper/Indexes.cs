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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
	/// Ripper ONLY handles Level one indices (Index -> post or Index -> Thread)
	/// and NOT (Index -> Index) !
	/// This class gets all the thread pages of the index and parses them to extract
	/// links to showthread.php or showpost.php
	/// </summary>
	public class Indexes
	{
	    /// <summary>
        /// Gets the Content of all Thread Pages
        /// </summary>
        /// <param name="sURL">Original URL of the Forum Thread</param>
        /// <returns>The Content of all Thread Pages</returns>
        public string GetThreadPages(string sURL)
        {
            string sPageContent = GetRipPage(sURL);

            int iOccurIdx = 0;
	        iOccurIdx = sPageContent.IndexOf("Page 1 of ", iOccurIdx);

            int iOpe1 = sPageContent.IndexOf("</td>", iOccurIdx);
            int iThreadPages = int.Parse(sPageContent.Substring(iOccurIdx + 10, iOpe1 - (iOccurIdx + 10)));

            string szThreadBaseURL = sURL;

	        for (int i = 1; i <= iThreadPages; i++)
            {
                string szComposed = string.Format("{0}&page={1}", szThreadBaseURL, i);
                sPageContent += GetRipPage(szComposed);
            }

	        return sPageContent;
        }
        /// <summary>
        /// Gets the Content of all Thread Pages (VB Forums 4.x)
        /// </summary>
        /// <param name="sURL">Original URL of the Forum Thread</param>
        /// <returns>The Content of all Thread Pages</returns>
        public string GetThreadPagesNew(string sURL)
        {
            string sPageContent = GetRipPage(sURL);

            int iStart;
            int iThreadPages;

            try
            {
                int iPagerStart = sPageContent.IndexOf("<span><a href=\"javascript://\" class=\"popupctrl\">Page ");
                iStart = sPageContent.IndexOf("of ", iPagerStart);

                iStart += 3;
            }
            catch (Exception)
            {
                return sPageContent;
            }

            if (iStart >= 0)
            {
                int iEnd = sPageContent.IndexOf("</a></span>", iStart);

                iThreadPages = int.Parse(sPageContent.Substring(iStart, iEnd - iStart));
            }
            else
            {
                return sPageContent;
            }

            string szThreadBaseURL = sURL;

            for (int i = 1; i <= iThreadPages; i++)
            {
                //-2.html
                string szComposed = szThreadBaseURL.Contains(".html") ? szThreadBaseURL.Replace(".html", string.Format("-{0}.html", i)) : string.Format("{0}/page{1}", szThreadBaseURL, i);

                sPageContent += GetRipPage(szComposed);
            }

            return sPageContent;
        }
		public string GetPostPages(string sURL)
		{		
			return GetRipPage(sURL);
		}

		private static string GetRipPage(string strURL)
		{
            HttpWebRequest lHttpWebRequest;
		    Stream lHttpWebResponseStream;
            string strPageRead = string.Empty;

            try
            {
                lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);
                lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
                lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                lHttpWebRequest.Headers.Add("Cookie: " + CookieManager.GetInstance().GetCookieString());
                lHttpWebRequest.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
                lHttpWebRequest.KeepAlive = true;
                //lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
                lHttpWebRequest.Referer = MainForm.userSettings.sForumUrl;

                lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");

                StreamReader readStream = new StreamReader(lHttpWebResponseStream, encode);

                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    strPageRead += str;
                    count = readStream.Read(read, 0, 256);
                }
                lHttpWebResponseStream.Close();
            }
			catch (Exception)
			{
			    strPageRead = string.Empty;
			}

		    return strPageRead;
		}
        public List<ImageInfo> ParseHtml(string sContent, string sUrl)
        {
            return Utility.ExtractIndexUrlsHtml(sContent, sUrl);
        }
	}
}
