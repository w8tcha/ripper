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
    /// Worker class to get images hosted on ImageBeaver.com
    /// </summary>
    public class ImageBeaver : ServiceTemplate
    {
        public ImageBeaver(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
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

            var strImgURLNew = strImgURL;

            strImgURLNew = strImgURLNew.Replace("view.php?mode=gallery&g=", "photos/").Replace("&photo=", "_")
                           + "-.html";

            var strIVPage = this.GetImageHostPage(ref strImgURLNew);

            // Adult Warning
            var iStartConfirm = 0;
            var iEndConfirm = 0;

            iStartConfirm = strIVPage.IndexOf("<title>");

            iStartConfirm += 7;

            if (iStartConfirm < 0)
            {
                return false;
            }

            try
            {
                iEndConfirm = strIVPage.IndexOf("</title>", iStartConfirm);
            }
            catch (Exception)
            {
                return false;
            }

            if (iEndConfirm < 0)
            {
                return false;
            }

            var sConfirmURL = strIVPage.Substring(iStartConfirm, iEndConfirm - iStartConfirm);

            if (sConfirmURL == "ImageBeaver Warning")
            {
                iStartConfirm = 0;
                iEndConfirm = 0;

                iStartConfirm = strIVPage.IndexOf("<p class=\"enter\"><a href=\"");

                iStartConfirm += 26;

                iEndConfirm = strIVPage.IndexOf("\">", iStartConfirm);

                var strURL = "http://www.imagebeaver.com/"
                             + strIVPage.Substring(iStartConfirm, iEndConfirm - iStartConfirm);

                strIVPage = this.GetImageHostsPage(ref strURL);
            }

            if (strIVPage.Length < 10)
            {
                return false;
            }

            var strNewURL = string.Empty;

            var iStartSRC = 0;
            var iEndSRC = 0;

            iStartSRC = strIVPage.IndexOf("onLoad=\"scaleImg('thepic')\" src=\"");

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 33;

            iEndSRC = strIVPage.IndexOf("\">", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }

            strNewURL = strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            var strFilePath = string.Empty;

            if (strImgURL.Contains(@"view.php?"))
            {
                strFilePath = strNewURL.Substring(strNewURL.LastIndexOf("/") + 1);
            }
            else
            {
                strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("/") + 1);
                strFilePath = strFilePath.Replace(".html", ".jpg");
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

            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            var CCObj = new CacheObject();
            CCObj.IsDownloaded = false;
            CCObj.FilePath = strFilePath;
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

        /// <summary>
        /// a generic function to fetch urls including cookie
        /// </summary>
        /// <param name="strURL"></param>
        /// <returns></returns>
        protected string GetImageHostsPage(ref string strURL)
        {
            HttpWebRequest req;
            StreamReader reader;
            HttpWebResponse res;
            var strPageRead = string.Empty;

            try
            {
                var cookieContainer = new CookieContainer();

                req = (HttpWebRequest)WebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.CookieContainer = cookieContainer;


                res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                var stream = res.GetResponseStream();
                reader = new StreamReader(stream);

                strPageRead = reader.ReadToEnd();

                res.Close();
                reader.Close();
            }
            catch (ThreadAbortException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                // Debug only
                // MessageBox.Show(ex.Message);
            }

            return strPageRead;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}