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
using System.IO;
using System.Net;
using System.Threading;

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
    /// Worker class to get images hosted on ImageBeaver.com
    /// </summary>
    public class ImageBeaver : ServiceTemplate
    {
        public ImageBeaver(ref string sSavePath, ref string strURL, ref string imageName, ref Hashtable hTbl)
            : base(sSavePath, strURL, imageName, ref hTbl)
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

            string strImgURLNew = strImgURL;

            strImgURLNew = strImgURLNew.Replace("view.php?mode=gallery&g=", "photos/").Replace("&photo=", "_") + "-.html";

            string strIVPage = GetImageHostPage(ref strImgURLNew);

            // Adult Warning
            int iStartConfirm = 0;
            int iEndConfirm = 0;

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

            string sConfirmURL = strIVPage.Substring(iStartConfirm, iEndConfirm - iStartConfirm);

            if (sConfirmURL == "ImageBeaver Warning")
            {
                iStartConfirm = 0;
                iEndConfirm = 0;

                iStartConfirm = strIVPage.IndexOf("<p class=\"enter\"><a href=\"");

                iStartConfirm += 26;

                iEndConfirm = strIVPage.IndexOf("\">", iStartConfirm);

                string strURL = "http://www.imagebeaver.com/" + strIVPage.Substring(iStartConfirm, iEndConfirm - iStartConfirm);

                strIVPage = GetImageHostsPage(ref strURL);
            }


            if (strIVPage.Length < 10)
            {
                
                return false;
            }

            string strNewURL = string.Empty;

            int iStartSRC = 0;
            int iEndSRC = 0;

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

            string strFilePath = string.Empty;

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
            //CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.GetInstance().LastPic = ((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

            return true;
        }

        /// <summary>
        /// a generic function to fetch urls.
        /// </summary>
        /// <param name="strURL"></param>
        /// <returns></returns>
        protected string GetImageHostsPage(ref string strURL)
        {
            HttpWebRequest req;
            StreamReader reader;
            HttpWebResponse res;
            string strPageRead = string.Empty;

            try
            {
                CookieContainer cookieContainer = new CookieContainer();

                req = (HttpWebRequest)HttpWebRequest.Create(strURL);

                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
                req.CookieContainer = cookieContainer;


                res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                Stream stream = res.GetResponseStream();
                reader = new StreamReader(stream);

                strPageRead = reader.ReadToEnd();

                res.Close();
                reader.Close();
            }
            catch (ThreadAbortException)
            {
                return "";
            }
            catch (Exception)
            {
                // Debug only
                //MessageBox.Show(ex.Message);
            }
            return strPageRead;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}