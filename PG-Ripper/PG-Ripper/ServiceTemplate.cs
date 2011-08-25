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
using System.Collections;
using System.Net;
using System.Threading;
using System.IO;

namespace PGRipper
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ServiceTemplate
    {
        protected string mstrURL = string.Empty;
        protected Hashtable eventTable;
        protected string mSavePath = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSavePath"></param>
        /// <param name="strURL"></param>
        /// <param name="hTbl"></param>
        protected ServiceTemplate(string sSavePath, string strURL, ref Hashtable hTbl)
        {
            mstrURL = strURL;
            eventTable = hTbl;
            mSavePath = sSavePath;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartDownload()
        {
            /*DoDownload();

            for (int i = 0; i < 3; i++)
            {
                DoDownload();

                if (eventTable[mstrURL] == null)
                {
                    ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
                    break;
                }

                if (((CacheObject)eventTable[mstrURL]).IsDownloaded)
                {
                    break;
                }
            }

            ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
            if (eventTable.Contains(mstrURL))
            {
                eventTable.Remove(mstrURL);
            }*/

            DoDownload();

            if (eventTable[mstrURL] != null)
            {
                if (eventTable.Contains(mstrURL))
                {
                    eventTable.Remove(mstrURL);
                }
            }

            ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
        }

        protected abstract bool DoDownload();

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL"></param>
        /// <returns></returns>
        protected string GetImageHostPage(ref string strURL)
        {
            string strPageRead;

            /*try
            {
                WebClient wc = new WebClient();
                strPageRead = wc.DownloadString(strURL);
                wc.T
                wc.Dispose();
            }
            catch (ThreadAbortException)
            {
                return "";
            }
            catch (Exception)
            {
                return "";
            }*/


            HttpWebRequest lHttpWebRequest;
            Stream lHttpWebResponseStream;

            try
            {
                lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);

                lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Referer = strURL;
                lHttpWebRequest.KeepAlive = true;
                lHttpWebRequest.Timeout = 20000;

                lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

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