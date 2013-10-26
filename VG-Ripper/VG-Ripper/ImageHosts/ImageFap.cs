//////////////////////////////////////////////////////////////////////////
// Code Named: PiP-Ripper
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
// This file is part of the RiP-Ripper project base.

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace Ripper
{
    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from ImageFap.com
    /// </summary>
    public class ImageFap : ServiceTemplate
    {
        public ImageFap(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashtable)
        {
        }

        protected override bool DoDownload()
        {
            string strImgURL = ImageLinkURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = strImgURL.Substring(strImgURL.IndexOf("id=") + 3) + ".jpg";

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

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("<a href=\"" + strImgURL.Substring(strImgURL.LastIndexOf("/") + 1) + "\"><img border=\"0\" src=\"");

            if (iStartSRC < 0)
            {
                return false;
            }


            iStartSRC += 32 + strImgURL.Substring(strImgURL.LastIndexOf("/") + 1).Length;

            iEndSRC = strIVPage.IndexOf("\"></a>", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            string strNewURL = string.Empty;
            strNewURL = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            strNewURL = strNewURL.Replace("images.imagefap", "cache.imagefap").Replace("/mini/", "/full/");

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
            CacheController.Instance().LastPic =((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

        
    }
}