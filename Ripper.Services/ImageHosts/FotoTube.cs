﻿//////////////////////////////////////////////////////////////////////////
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

namespace Ripper.Services.ImageHosts
{
    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images from FotoTube.pl
    /// </summary>
    public class FotoTube : ServiceTemplate
    {
        public FotoTube(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
            : base(sSavePath, strURL, thumbURL, imageName, imageNumber, ref hashtable)
        {
        }

        protected override bool DoDownload()
        {
            var strImgURL = this.ImageLinkURL;

            if (this.EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            var strFilePath = string.Empty;

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

            var ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                this.EventTable.Add(strImgURL, ccObj);
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

                this.EventTable.Add(strImgURL, ccObj);
            }

            var strNewURL = strImgURL.Replace("/obrazek,", "/pictures/").Replace(".html", string.Empty);

            strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("/") + 1);

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            //////////////////////////////////////////////////////////////////////////
            var newAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != newAlteredPath)
            {
                strFilePath = newAlteredPath;
                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;
            }

            try
            {
                var client = new WebClient();
                client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
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
            CacheController.Instance().LastPic =((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}