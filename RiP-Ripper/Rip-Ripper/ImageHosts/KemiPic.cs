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
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace RiPRipper.ImageHosts
{
    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images from KemiPic.com
    /// </summary>
    public class KemiPic : ServiceTemplate
    {
        public KemiPic(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
        }


        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

            try
            {
                if (!Directory.Exists(mSavePath))
                    Directory.CreateDirectory(mSavePath);
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return false;
            }

            CacheObject ccObj = new CacheObject
            {
                IsDownloaded = false,
                FilePath = strFilePath,
                Url = strImgURL
            };

            try
            {
                EventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (EventTable.ContainsKey(strImgURL))
                {
                    return false;
                }

                EventTable.Add(strImgURL, ccObj);
            }

            const string sStartTitle = "<title>Kemipic - Earn money by sharing pictures, images - ";

            string sPage = GetImageHostPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            int iStartSrc = sPage.IndexOf(sStartTitle);

            if (iStartSrc < 0)
            {
                return false;
            }

            iStartSrc += sStartTitle.Length;

            int iEndSrc = sPage.IndexOf(" - </title>", iStartSrc);

            if (iEndSrc < 0)
            {
                return false;
            }

            strFilePath = sPage.Substring(iStartSrc, iEndSrc - iStartSrc);

            string strNewURL = strImgURL.Replace("share", "image").Replace(".html", strFilePath.Substring(strFilePath.LastIndexOf(".")));

            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////

            try
            {
                string sNewAlteredPath = Utility.GetSuitableName(strFilePath);
                if (strFilePath != sNewAlteredPath)
                {
                    strFilePath = sNewAlteredPath;
                    ((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;
                }

                WebClient client = new WebClient();
                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);

                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return false;
            }

            ((CacheObject)EventTable[mstrURL]).IsDownloaded = true;
            //CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.GetInstance().uSLastPic =((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}