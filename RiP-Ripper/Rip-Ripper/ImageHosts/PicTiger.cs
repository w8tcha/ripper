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

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images from PicTiger.com
    /// </summary>
    public class PicTiger : ServiceTemplate
    {
        public PicTiger(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref Hashtable hTbl)
            : base(sSavePath, strURL, thumbURL, imageName, ref hTbl)
        {
        }


        protected override bool DoDownload()
        {
            string strImgURL = ImageLinkURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = strImgURL.Substring(strImgURL.IndexOf("picture-hosting/") + 16);

            strFilePath = strFilePath.Remove(strFilePath.Length - 4, 4);

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

            string strIVPage = GetImageHostPage(ref strImgURL);

            if (strIVPage.Length < 10)
            {
               
                return false;
            }

            string strNewURL = string.Empty;
            strNewURL = strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1);

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("<img id=\"mainImg\" src='");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 23;

            iEndSRC = strIVPage.IndexOf("' alt='", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            strNewURL = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            //////////////////////////////////////////////////////////////////////////

            HttpWebRequest lHttpWebRequest;
            HttpWebResponse lHttpWebResponse;
            Stream lHttpWebResponseStream;

            try
            {
                lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strNewURL);

                lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
                lHttpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
                lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                lHttpWebRequest.Referer = strImgURL;
                lHttpWebRequest.Accept = "image/png,*/*;q=0.5";
                lHttpWebRequest.KeepAlive = true;

                lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();
                lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

                if (lHttpWebResponse.ContentType.IndexOf("image") < 0)
                {
                    //if (lFileStream != null)
                    //	lFileStream.Close();
                    return false;
                }
                if (lHttpWebResponse.ContentType.ToLower() == "image/jpeg")
                    strFilePath += ".jpg";
                else if (lHttpWebResponse.ContentType.ToLower() == "image/gif")
                    strFilePath += ".gif";
                else if (lHttpWebResponse.ContentType.ToLower() == "image/png")
                    strFilePath += ".png";

                lHttpWebResponseStream.Close();

                string NewAlteredPath = Utility.GetSuitableName(strFilePath);
                if (strFilePath != NewAlteredPath)
                {
                    strFilePath = NewAlteredPath;
                    ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
                }

                WebClient client = new WebClient();
                client.Headers.Add("Accept-Language: en-us,en;q=0.5");
                client.Headers.Add("Accept-Encoding: gzip,deflate");
                client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
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