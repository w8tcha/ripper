//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on VB forums and attempts to fetch
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
    /// Worker class to get images from ImageFoco.com, PicFoco.com, CocoImage.com or CocoPic.com
    /// </summary>
    public class ImageFoco : ServiceTemplate
    {
        public ImageFoco(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
            //
            // Add constructor logic here
            //
        }
        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (eventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            try
            {
                if (!Directory.Exists(mSavePath))
                    Directory.CreateDirectory(mSavePath);
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                 MainForm.bDelete = true;

                return false;
            }

            string strFilePath = string.Empty;

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
            catch (System.Exception)
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

            string sNewURL;

            try
            {
                sNewURL = GetRealLink(strImgURL);
            }
            catch (Exception)
            {
                try
                {
                    sNewURL = GetRealLink(strImgURL);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            Thread.Sleep(900);
            
            string sNewPage = GetImageHostPage(sNewURL);

            if (sNewPage.Length < 10)
            {
                
                return false;
            }

            int iStartSRC2 = 0;
            int iEndSRC2 = 0;

            iStartSRC2 = sNewPage.IndexOf(")\" src=\"");

            if (iStartSRC2 < 0)
            {
                return false;
            }

            iStartSRC2 += 8;

            iEndSRC2 = sNewPage.IndexOf("\" >", iStartSRC2);

            if (iEndSRC2 < 0)
            {
                return false;
            }

            string strNewURL = sNewPage.Substring(iStartSRC2, iEndSRC2 - iStartSRC2);

            if (strNewURL.Contains("img_trans.php?"))
            {
                strFilePath = strNewURL.Substring(strNewURL.IndexOf("&file=") + 6).Remove(strNewURL.IndexOf("&server="));
            }
            else
            {
                strFilePath = strNewURL.Substring(strNewURL.IndexOf("&v=") + 3).Replace("&ext=", ".");

                if (strFilePath.Contains("&dt="))
                {
                    strFilePath = strFilePath.Remove(strFilePath.IndexOf("&dt="));
                }
            }

            if (mSavePath.Contains("/"))
                strFilePath = mSavePath + "/" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;
            else
                strFilePath = mSavePath + "\\" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;

            //////////////////////////////////////////////////////////////////////////

            string NewAlteredPath = Utility.GetSuitableName(strFilePath);
            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;
                ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
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
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

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
            //CacheController.GetInstance().u_s_LastPic = ((CacheObject)eventTable[mstrURL]).FilePath;
            CacheController.GetInstance().uSLastPic = ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;

            return true;
        }
        protected string GetImageHostPage(string strURL)
        {
            string strPageRead = string.Empty;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Referer: " + strURL);
                wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                strPageRead = wc.DownloadString(strURL);
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
            lHttpWebRequest.Referer = strURL;
            lHttpWebRequest.KeepAlive = true;

            lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();
            lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

            StreamReader streamReader = new StreamReader(lHttpWebResponseStream);

            string sPage = streamReader.ReadToEnd();

            lHttpWebResponseStream.Close();

            int iStartSRC = 0;
            int iEndSRC = 0;

            iStartSRC = sPage.IndexOf("window.location=\"");

            iStartSRC += 17;

            iEndSRC = sPage.IndexOf("\";", iStartSRC);

            return sPage.Substring(iStartSRC, iEndSRC - iStartSRC);

            
        }


        //////////////////////////////////////////////////////////////////////////

    }
}