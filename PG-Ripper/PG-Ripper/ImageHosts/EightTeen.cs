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

using System.Net;

namespace Ripper
{
    /// <summary>
    /// Worker class to get Frame Image Links from *.EightTeen.com
    /// </summary>
    public class EightTeen
    {
        /// <summary>
        /// Extracts Frame Links from the html
        /// </summary>
        /// <param name="strOrgLnk">Web Adress of EightTeen link</param>
        /// <returns>Real Image Web Adress</returns>
        public static string EightTeenLink(string strOrgLnk)
        {
            string strIVPage = getPage(strOrgLnk);

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("<title>");

            iStartSRC += 7;

            iEndSRC = strIVPage.IndexOf("</title>", iStartSRC);

            string strRelLnk = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            return strRelLnk;
        }
        private static string getPage(string strURL)
        {
            string strPageRead = string.Empty;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Referer: " + strURL);
                wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                strPageRead = wc.DownloadString(strURL);
            }
            catch (System.Exception)
            {
                //MessageBox.Show( e.Message );
            }

            return strPageRead;
        }
    }
}