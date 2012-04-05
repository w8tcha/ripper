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
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImageHaven.net
    /// </summary>
    public class ImageHaven : ServiceTemplate
    {
        public ImageHaven(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
        }


        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (eventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

            if (strImgURL.Contains("_"))
            {
                strFilePath = strImgURL.Substring(strImgURL.IndexOf("_") + 1);
            }
            else
            {
                strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("?id=") + 13);
            }

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

            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

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
            catch (Exception)
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

            string strIVPage = string.Empty;

            try
            {

                strIVPage = GetRealLink(strImgURL);
            }
            catch (Exception)
            {
                return false;
            }

            string strNewURL = string.Empty;

           

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("<img src='.");


            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 11;

            iEndSRC = strIVPage.IndexOf("' id=\"image\"", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            string sAltUrl = strImgURL;
            
            sAltUrl = sAltUrl.Remove(strImgURL.IndexOf("/img.php"));

            strNewURL = string.Format("{0}{1}", 
                sAltUrl,
                strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC));

            //////////////////////////////////////////////////////////////////////////

            try
            {

                WebClient client = new WebClient();

                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
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
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

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
            CacheController.GetInstance().uSLastPic =((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }
        protected string GetRealLink(string strURL)
        {
            HttpWebRequest lHttpWebRequest;
            HttpWebResponse lHttpWebResponse;
            Stream lHttpWebResponseStream;


            lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);

            lHttpWebRequest.UserAgent = @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Q312461; .NET CLR 1.1.4322)";
            lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
            lHttpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
            lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            lHttpWebRequest.Headers.Add("Cookie: " + "imageshowcookie_1=yes");
            lHttpWebRequest.Referer = strURL;
            lHttpWebRequest.KeepAlive = true;

            lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();
            lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

            StreamReader streamReader = new StreamReader(lHttpWebResponseStream);

            string sPage = streamReader.ReadToEnd();

            return sPage;
        }


        //////////////////////////////////////////////////////////////////////////

    }
}