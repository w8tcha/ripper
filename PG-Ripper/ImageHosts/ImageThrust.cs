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

using System.Collections;
using System.Net;
using System.IO;
using System.Threading;

namespace Ripper
{
    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from ImageThrust.com
    /// </summary>
    public class ImageThrust : ServiceTemplate
    {
        public ImageThrust(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashTable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashTable)
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

            string strFilePath = string.Empty;

            strFilePath = strImgURL.Substring(strImgURL.IndexOf("view-image/") + 11);

            strFilePath = strFilePath.Remove(strFilePath.Length - 5, 5);

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

            string strIVPage = GetImageHostPage(ref strImgURL);

            if (strIVPage.Length < 10)
            {
                
                return false;
            }

            string strNewURL = string.Empty;

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("src=\"/i/");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 8;

            iEndSRC = strIVPage.IndexOf(".jpg\" id=\"picture\"", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }


            strNewURL = string.Format("{0}i/{1}",
               strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1),
               strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC) + ".jpg");
            
            //////////////////////////////////////////////////////////////////////////
            HttpWebRequest lHttpWebRequest;
            HttpWebResponse lHttpWebResponse;
            Stream lHttpWebResponseStream;

            //FileStream lFileStream = new FileStream(strFilePath, FileMode.Create);

            //int bytesRead;

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

                string NewAlteredPath = Utility.GetSuitableName(strFilePath);
                if (strFilePath != NewAlteredPath)
                {
                    strFilePath = NewAlteredPath;
                    ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
                }

                lHttpWebResponseStream.Close();

                System.Net.WebClient client = new WebClient();
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
            CacheController.Instance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}