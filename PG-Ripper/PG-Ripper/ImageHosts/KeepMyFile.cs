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
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images hosted on KeepMyFile.com
    /// </summary>
    public class KeepMyFile : ServiceTemplate
    {
        public KeepMyFile(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
            //
            // Add constructor logic here
            //
        }


        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (eventTable.ContainsKey(strImgURL))
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
                MainForm.sDeleteMessage = ex.Message;
                 MainForm.bDelete = true;

                return false;
            }

            if (mSavePath.Contains("/"))
                strFilePath = mSavePath + "/" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;
            else
                strFilePath = mSavePath + "\\" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;

            CacheObject CCObj = new CacheObject();
            CCObj.IsDownloaded = false;
            CCObj.FilePath = strFilePath;
            CCObj.Url = strImgURL;
            try
            {
                eventTable.Add(strImgURL, CCObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (System.Exception)
            {
                if (eventTable.ContainsKey(strImgURL))
                {
                    return false;
                }
                else
                {
                    eventTable.Add(strImgURL, CCObj);
                }
            }

            string strIVPage = GetImageHostPage(ref strImgURL);

            if (strIVPage.Length < 10)
            {
                
                return false;
            }

            string strNewURL = strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1);

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("onClick=\"scale();\" src=\"");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 24;

            iEndSRC = strIVPage.IndexOf("\" alt=\"image not loading?", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            strNewURL = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            strFilePath += strNewURL.Substring(strNewURL.LastIndexOf("/") + 1);

            //////////////////////////////////////////////////////////////////////////

            string NewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;
                ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
            }

            try
            {
                WebClient client = new WebClient();
                //client.Headers.Add("Accept-Language: en-us,en;q=0.5");
                //client.Headers.Add("Accept-Encoding: gzip,deflate");
                //client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                //client.Headers.Add("Referer: " + strImgURL);
                //client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);

            }
            catch (ThreadAbortException)
            {
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return false;
            }

            ((CacheObject)eventTable[mstrURL]).IsDownloaded = true;
            //CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.GetInstance().uSLastPic = ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}