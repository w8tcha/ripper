//////////////////////////////////////////////////////////////////////////
// Code Named: VG-Ripper
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

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images hosted on ShareAvenue.com
    /// </summary>
    public class ShareAvenue : ServiceTemplate
    {
        public ShareAvenue(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref Hashtable hTbl)
            : base(sSavePath, strURL, thumbURL, imageName, ref hTbl)
        {
            //
            // Add constructor logic here
            //
        }


        protected override bool DoDownload()
        {
            string strImgURL = ImageLinkURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strIVPage = GetImageHostPage(ref strImgURL);

            if (strIVPage.Length < 10)
            {
               
                return false;
            }

            int iStartSRC2 = 0;
            int iEndSRC2 = 0;

            iStartSRC2 = strIVPage.IndexOf("<tr><td colspan=2><p>Original filename: ");

            if (iStartSRC2 < 0)
            {
                return false;
            }

            iStartSRC2 += 40;

            iEndSRC2 = strIVPage.IndexOf("</p></td></tr><tr><td colspan=2><p>Size:", iStartSRC2);

            if (iEndSRC2 < 0)
            {
                return false;
            }

            string strFilePath = strIVPage.Substring(iStartSRC2, iEndSRC2 - iStartSRC2);

            try
            {
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return false;
            }

            strFilePath = Path.Combine(SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            CacheObject CCObj = new CacheObject();
            CCObj.IsDownloaded = false;
            CCObj.FilePath = strFilePath;
            CCObj.Url = strImgURL;
            try
            {
                EventTable.Add(strImgURL, CCObj);
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
                else
                {
                    EventTable.Add(strImgURL, CCObj);
                }
            }

            string strIVPage2 = GetImageHostPage(ref strImgURL);

            if (strIVPage2.Length < 10)
            {
               
                return false;
            }

            string strNewURL = strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1);

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage2.IndexOf("onClick=\"scale('img')\" onLoad=\"scale_load('img')\" src=\"");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 55;

            iEndSRC = strIVPage2.IndexOf("\"></td><td valign=\"top\">", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            strNewURL = "http://" + strImgURL.Substring(strImgURL.IndexOf("http://") + 7, 4) + ".shareavenue.com/" + strIVPage2.Substring(iStartSRC, iEndSRC - iStartSRC);

            //////////////////////////////////////////////////////////////////////////

            string NewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;
                ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(ImageLinkURL);

                return false;
            }

            ((CacheObject)EventTable[ImageLinkURL]).IsDownloaded = true;
            //CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.GetInstance().LastPic =((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}