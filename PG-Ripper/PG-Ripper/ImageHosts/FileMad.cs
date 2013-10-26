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
using System.IO;
using System.Net;
using System.Threading;
using System;

namespace Ripper.ImageHosts
{
    using Ripper.Objects;

    /// <summary>
    /// Worker class to get images from FileMad.com
    /// </summary>
    public class FileMad : ServiceTemplate
    {
        public FileMad(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashTable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashTable)
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

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

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

            const string sStartSrc = "<p><img src=\"";

            string sPage = GetImageHostPage(ref strImgURL);

            if (sPage.Length < 10)
            {
                return false;
            }

            int iStartSrc = sPage.IndexOf(sStartSrc);

            if (iStartSrc < 0)
            {
                return false;
            }

            iStartSrc += sStartSrc.Length;

            int iEndSrc = sPage.IndexOf(" onLoad=\"scaleImg(this)\"", iStartSrc);

            if (iEndSrc < 0)
            {
                return false;
            }

            string strNewURL = sPage.Substring(iStartSrc, iEndSrc - iStartSrc);

            strFilePath = strNewURL.Substring(strNewURL.IndexOf("alt=") + 5);

            strFilePath = Path.Combine(SavePath, Utility.RemoveIllegalCharecters(strFilePath.Remove(strFilePath.IndexOf("\""))));

            strNewURL = strNewURL.Remove(strNewURL.IndexOf("\""));

            //////////////////////////////////////////////////////////////////////////

            string newAlteredPath = Utility.GetSuitableName(strFilePath);

            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
            }

            strFilePath = Utility.CheckPathLength(strFilePath);
            ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;


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
            CacheController.Instance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}