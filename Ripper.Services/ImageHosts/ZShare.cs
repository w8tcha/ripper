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

namespace Ripper
{
    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images hosted on ZShare.net
    /// </summary>
    public class ZShare : ServiceTemplate
    {
        public ZShare(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashtable)
        {
            // Add constructor logic here
        }


        protected override bool DoDownload()
        {
            var strImgURL = this.ImageLinkURL;

            if (this.EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            try
            {
                if (!Directory.Exists(this.SavePath))
                    Directory.CreateDirectory(this.SavePath);
            }
            catch (IOException ex)
            {
                // MainForm.DeleteMessage = ex.Message;
                // MainForm.Delete = true;
                return false;
            }

            var CCObj = new CacheObject();
            CCObj.IsDownloaded = false;

            // CCObj.FilePath = strFilePath;
            CCObj.Url = strImgURL;
            try
            {
                this.EventTable.Add(strImgURL, CCObj);
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                if (this.EventTable.ContainsKey(strImgURL))
                {
                    return false;
                }
                else
                {
                    this.EventTable.Add(strImgURL, CCObj);
                }
            }

            var strIVPage = this.GetImageHostPage(ref strImgURL);

            if (strIVPage.Length < 10)
            {
                return false;
            }

            var iStartSRC = 0;
            var iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("<img id=\"image\" onClick=\"scaleImg()\" src=\"");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 42;

            iEndSRC = strIVPage.IndexOf("\" border=\"0\" style=\"cursor: pointer\">", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            var strNewURL = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            var iStartFN = 0;
            var iEndFN = 0;

            iStartFN = strIVPage.IndexOf("class=\"text1\">File Name:");

            if (iStartFN < 0)
            {
                return false;
            }

            iStartFN += 48;

            iEndFN = strIVPage.IndexOf("</font></td", iStartFN);

            if (iEndFN < 0)
            {
                return false;
            }

            var strFilePath = strIVPage.Substring(iStartFN, iEndFN - iStartFN);

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            CCObj.FilePath = strFilePath;

            //////////////////////////////////////////////////////////////////////////
            var NewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;
            }

            try
            {
                var client = new WebClient();
                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add(
                    "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();
            }
            catch (ThreadAbortException)
            {
                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (IOException ex)
            {
                // MainForm.DeleteMessage = ex.Message;
                // MainForm.Delete = true;
                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)this.EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.ImageLinkURL);

                return false;
            }

            ((CacheObject)this.EventTable[this.ImageLinkURL]).IsDownloaded = true;

            // CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.Instance().LastPic =
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}