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
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images from ImageHaven.net
    /// </summary>
    public class ImageHaven : ServiceTemplate
    {
        public ImageHaven(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref Hashtable hTbl)
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
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                 MainForm.Delete = true;

                return false;
            }

            if (SavePath.Contains("/"))
                strFilePath = SavePath + "/" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;
            else
                strFilePath = SavePath + "\\" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;

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
            catch (System.Exception)
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
            CacheController.GetInstance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

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

