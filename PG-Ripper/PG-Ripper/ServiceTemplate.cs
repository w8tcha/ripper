// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTemplate.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PGRipper
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// Service Template Class
    /// </summary>
    public abstract class ServiceTemplate
    {
        /// <summary>
        /// Image Link Url
        /// </summary>
        protected string mstrURL = string.Empty;

        /// <summary>
        /// HashTable with Urls.
        /// </summary>
        protected Hashtable eventTable;

        /// <summary>
        /// Image Save Folder Path
        /// </summary>
        protected string mSavePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplate"/> class.
        /// </summary>
        /// <param name="savePath">
        /// The save path.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="hTbl">
        /// The h tbl.
        /// </param>
        protected ServiceTemplate(string savePath, string url, ref Hashtable hTbl)
        {
            this.mstrURL = url;
            this.eventTable = hTbl;
            this.mSavePath = savePath;
        }

        /// <summary>
        /// Start Download
        /// </summary>
        public void StartDownload()
        {
            this.DoDownload();

            if (this.eventTable[this.mstrURL] != null)
            {
                if (this.eventTable.Contains(this.mstrURL))
                {
                    this.eventTable.Remove(this.mstrURL);
                }
            }

            ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
        }

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected abstract bool DoDownload();

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL">
        /// The str URL.
        /// </param>
        /// <returns>
        /// Returns the Page as string.
        /// </returns>
        protected string GetImageHostPage(ref string strURL)
        {
            string strPageRead;

            try
            {
                HttpWebRequest lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);

                lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Referer = strURL;
                lHttpWebRequest.KeepAlive = true;
                lHttpWebRequest.Timeout = 20000;

                Stream lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

                StreamReader streamReader = new StreamReader(lHttpWebResponseStream);

                strPageRead = streamReader.ReadToEnd();

                lHttpWebResponseStream.Close();
            }
            catch (ThreadAbortException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return strPageRead;
        }
    }
}