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
using System.Text.RegularExpressions;
using System.Threading;

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
    /// Worker class to get images hosted on ImageVenue
    /// </summary>
    public class imagevenueNew : ServiceTemplate
    {
        public imagevenueNew(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
            : base(sSavePath, strURL, ref hTbl)
        {
            //
            // Add constructor logic here
            //
        }


        protected override bool DoDownload()
        {
            string strImgURL = mstrURL;

            if (EventTable.ContainsKey(strImgURL))
            {
                return true;
            }

            string strFilePath = string.Empty;

            strFilePath = strImgURL.Substring(strImgURL.IndexOf("image=") + 6);

            // Get rid of the ImageVenue randomized tag

            // Old Remover
            /*int iTagStart = 0;
            iTagStart = strFilePath.IndexOf("_");
            if (iTagStart >= 0)
                strFilePath = strFilePath.Substring(iTagStart + 1);
             */ 

            

            try
            {
                Match a = Regex.Match(strFilePath, @"(^\d{5})_");
                Match b = Regex.Match(strFilePath, @"(^\d{3})_");
                Match c = Regex.Match(strFilePath, @"(^\w{3})_");
                Match d = Regex.Match(strFilePath, @"(^\w\d\w)_");
                Match e = Regex.Match(strFilePath, @"(^\d\w\d)_");
                Match f = Regex.Match(strFilePath, @"(^\d\d\w)_");
                Match g = Regex.Match(strFilePath, @"(^\w\d\d)_");
                Match h = Regex.Match(strFilePath, @"(^\d{3}\w\d)_");
                Match i = Regex.Match(strFilePath, @"(^\d\w\d\w\d)_");
                Match j = Regex.Match(strFilePath, @"(^\w\d\w\d\w)_");
                Match k = Regex.Match(strFilePath, @"(^\w\d\d\d\w)_");
                Match l = Regex.Match(strFilePath, @"(^\w\d\w\d\d)_");
                Match m = Regex.Match(strFilePath, @"(^\d\w\d\d\w)_");
                Match n = Regex.Match(strFilePath, @"(^\d\w\w\d\w)_");
                Match o = Regex.Match(strFilePath, @"(^\d\w\w\d\d)_");
                Match p = Regex.Match(strFilePath, @"(^\d\w\w\w\d)_");
                Match q = Regex.Match(strFilePath, @"(^\d\w\d\d\w)_");
                Match r = Regex.Match(strFilePath, @"(^\d\w\d\w\w)_");
                Match s = Regex.Match(strFilePath, @"(^\d\d\w\w\w)_");
                Match t = Regex.Match(strFilePath, @"(^\d\d\d\w\w)_");
                Match u = Regex.Match(strFilePath, @"(^\d\w\w\w\w)_");
                Match v = Regex.Match(strFilePath, @"(^\w\d\d\w\d)_");
                Match w = Regex.Match(strFilePath, @"(^\w\d\d\w\w)_");
                Match x = Regex.Match(strFilePath, @"(^\w\w\w\w\d)_");
                Match y = Regex.Match(strFilePath, @"(^\w\w\d\d\d)_");
                Match z = Regex.Match(strFilePath, @"(^\w\w\w\d\d)_");
                Match aa = Regex.Match(strFilePath, @"(^\w\w\w\d\w)_");
                Match bb = Regex.Match(strFilePath, @"(^\w\w\d\w\w)_");
                Match cc = Regex.Match(strFilePath, @"(^\w\w\d\w\d)_");
                Match dd = Regex.Match(strFilePath, @"(^\w\w\d\d\w)_");
                Match ee = Regex.Match(strFilePath, @"(^\w\w\d\d\d)_");
                Match ff = Regex.Match(strFilePath, @"(^\w\d\w\w\w)_");
                Match gg = Regex.Match(strFilePath, @"(^\w\d\w\w\d)_");
                Match hh = Regex.Match(strFilePath, @"(^\w{5})_");
                
                
                if (a.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{5})_", "");
                }
                else if (b.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{3})_", "");
                }
                else if (c.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w{3})_", "");
                }
                else if (d.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w)_", "");
                }
                else if (e.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d)_", "");
                }
                else if (f.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\w)_", "");
                }
                else if (g.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d)_", "");
                }
                else if (h.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{3}\w\d)_", "");
                }
                else if (i.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\w\d)_", "");
                }
                else if (j.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\d\w)_", "");
                }
                else if (k.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\d\w)_", "");
                }
                else if (l.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\d\d)_", "");
                }
                else if (m.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\d\w)_", "");
                }
                else if (n.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\d\w)_", "");
                }
                else if (o.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\d\d)_", "");
                }
                else if (p.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\w\d)_", "");
                }
                else if (q.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\d\w)_", "");
                }
                else if (r.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\w\w)_", "");
                }
                else if (s.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\w\w\w)_", "");
                }
                else if (t.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\d\w\w)_", "");
                }
                else if (u.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\w\w)_", "");
                }
                else if (v.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\w\d)_", "");
                }
                else if (w.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\w\w)_", "");
                }
                else if (x.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\w\d)_", "");
                }
                else if (y.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\d)_", "");
                }
                else if (z.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\d\d)_", "");
                }
                else if (aa.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\d\w)_", "");
                }
                else if (bb.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\w\w)_", "");
                }
                else if (cc.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\w\d)_", "");
                }
                else if (dd.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\w)_", "");
                }
                else if (ee.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\d)_", "");
                }
                else if (ff.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\w\w)_", "");
                }
                else if (gg.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\w\d)_", "");
                }
                else if (hh.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w{5})_", "");
                }
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{4})lo", ""); 
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{3})lo", "");
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{2})lo", "");
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})lo", "");
            }
            catch (Exception)
            {
                //
            }


            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            CacheObject ccObj = new CacheObject {IsDownloaded = false, FilePath = strFilePath, Url = strImgURL};

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

            try
            {
                if (!Directory.Exists(mSavePath))
                    Directory.CreateDirectory(mSavePath);
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return false;
            }

            // Bypass for Random "Continue to your Image " Button
            string sImageURL = strImgURL.Substring(strImgURL.LastIndexOf("_") + 1);

            if (strImgURL.EndsWith("lo.jpg") ||
                strImgURL.EndsWith("lo.JPG") ||
                strImgURL.EndsWith("lo.jpeg"))
            {
                sImageURL = sImageURL.Replace(strImgURL.Substring(strImgURL.LastIndexOf("lo")), "");
            }

            sImageURL = strImgURL + "&loc=loc" + sImageURL;

            string strIvPage = GetImageHostPage(ref sImageURL);

            if (strIvPage.Length < 10)
            {
               
                return false;
            }

            string strNewURL = string.Empty;

            int iStartIMG = 0;
            int iStartSRC = 0;
            int iEndSRC = 0;
            iStartIMG = strIvPage.IndexOf("<img id=\"thepic\"");

            if (iStartIMG < 0)
            {
                return false;
            }

            iStartSRC = strIvPage.IndexOf("SRC=\"", iStartIMG);

            if (iStartSRC < 0)
            {
                return false;
            }

            iStartSRC += 5;

            iEndSRC = strIvPage.IndexOf("\" alt=\"", iStartSRC);

            if (iEndSRC < 0)
            {
                return false;
            }


            strNewURL = string.Format("{0}{1}", 
                strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1), 
                strIvPage.Substring(iStartSRC, iEndSRC - iStartSRC));

            //////////////////////////////////////////////////////////////////////////
            string NewAlteredPath = Utility.GetSuitableName(strFilePath);

            NewAlteredPath = NewAlteredPath.Replace("lo.", ".");

            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;

                ((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;
            }

            strFilePath = Utility.CheckPathLength(strFilePath);

            ((CacheObject)EventTable[mstrURL]).FilePath = strFilePath;

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
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return true;
            }
            catch (WebException)
            {
                ((CacheObject)EventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                return false;
            }

            ((CacheObject)EventTable[mstrURL]).IsDownloaded = true;

            CacheController.GetInstance().uSLastPic =((CacheObject)EventTable[mstrURL]).FilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}
