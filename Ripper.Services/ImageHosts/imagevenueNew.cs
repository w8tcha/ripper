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
using System.Text.RegularExpressions;
using System.Threading;

namespace Ripper
{
    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Worker class to get images hosted on ImageVenue
    /// </summary>
    public class imagevenueNew : ServiceTemplate
    {
        public imagevenueNew(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref int imageNumber, ref Hashtable hashtable)
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

            var strFilePath = string.Empty;

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
                var a = Regex.Match(strFilePath, @"(^\d{5})_");
                var b = Regex.Match(strFilePath, @"(^\d{3})_");
                var c = Regex.Match(strFilePath, @"(^\w{3})_");
                var d = Regex.Match(strFilePath, @"(^\w\d\w)_");
                var e = Regex.Match(strFilePath, @"(^\d\w\d)_");
                var f = Regex.Match(strFilePath, @"(^\d\d\w)_");
                var g = Regex.Match(strFilePath, @"(^\w\d\d)_");
                var h = Regex.Match(strFilePath, @"(^\d{3}\w\d)_");
                var i = Regex.Match(strFilePath, @"(^\d\w\d\w\d)_");
                var j = Regex.Match(strFilePath, @"(^\w\d\w\d\w)_");
                var k = Regex.Match(strFilePath, @"(^\w\d\d\d\w)_");
                var l = Regex.Match(strFilePath, @"(^\w\d\w\d\d)_");
                var m = Regex.Match(strFilePath, @"(^\d\w\d\d\w)_");
                var n = Regex.Match(strFilePath, @"(^\d\w\w\d\w)_");
                var o = Regex.Match(strFilePath, @"(^\d\w\w\d\d)_");
                var p = Regex.Match(strFilePath, @"(^\d\w\w\w\d)_");
                var q = Regex.Match(strFilePath, @"(^\d\w\d\d\w)_");
                var r = Regex.Match(strFilePath, @"(^\d\w\d\w\w)_");
                var s = Regex.Match(strFilePath, @"(^\d\d\w\w\w)_");
                var t = Regex.Match(strFilePath, @"(^\d\d\d\w\w)_");
                var u = Regex.Match(strFilePath, @"(^\d\w\w\w\w)_");
                var v = Regex.Match(strFilePath, @"(^\w\d\d\w\d)_");
                var w = Regex.Match(strFilePath, @"(^\w\d\d\w\w)_");
                var x = Regex.Match(strFilePath, @"(^\w\w\w\w\d)_");
                var y = Regex.Match(strFilePath, @"(^\w\w\d\d\d)_");
                var z = Regex.Match(strFilePath, @"(^\w\w\w\d\d)_");
                var aa = Regex.Match(strFilePath, @"(^\w\w\w\d\w)_");
                var bb = Regex.Match(strFilePath, @"(^\w\w\d\w\w)_");
                var cc = Regex.Match(strFilePath, @"(^\w\w\d\w\d)_");
                var dd = Regex.Match(strFilePath, @"(^\w\w\d\d\w)_");
                var ee = Regex.Match(strFilePath, @"(^\w\w\d\d\d)_");
                var ff = Regex.Match(strFilePath, @"(^\w\d\w\w\w)_");
                var gg = Regex.Match(strFilePath, @"(^\w\d\w\w\d)_");
                var hh = Regex.Match(strFilePath, @"(^\w{5})_");
                
                
                if (a.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{5})_", string.Empty);
                }
                else if (b.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{3})_", string.Empty);
                }
                else if (c.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w{3})_", string.Empty);
                }
                else if (d.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w)_", string.Empty);
                }
                else if (e.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d)_", string.Empty);
                }
                else if (f.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\w)_", string.Empty);
                }
                else if (g.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d)_", string.Empty);
                }
                else if (h.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d{3}\w\d)_", string.Empty);
                }
                else if (i.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\w\d)_", string.Empty);
                }
                else if (j.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\d\w)_", string.Empty);
                }
                else if (k.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\d\w)_", string.Empty);
                }
                else if (l.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\d\d)_", string.Empty);
                }
                else if (m.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\d\w)_", string.Empty);
                }
                else if (n.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\d\w)_", string.Empty);
                }
                else if (o.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\d\d)_", string.Empty);
                }
                else if (p.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\w\d)_", string.Empty);
                }
                else if (q.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\d\w)_", string.Empty);
                }
                else if (r.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\d\w\w)_", string.Empty);
                }
                else if (s.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\w\w\w)_", string.Empty);
                }
                else if (t.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\d\d\w\w)_", string.Empty);
                }
                else if (u.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\d\w\w\w\w)_", string.Empty);
                }
                else if (v.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\w\d)_", string.Empty);
                }
                else if (w.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\d\w\w)_", string.Empty);
                }
                else if (x.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\w\d)_", string.Empty);
                }
                else if (y.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\d)_", string.Empty);
                }
                else if (z.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\d\d)_", string.Empty);
                }
                else if (aa.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\w\d\w)_", string.Empty);
                }
                else if (bb.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\w\w)_", string.Empty);
                }
                else if (cc.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\w\d)_", string.Empty);
                }
                else if (dd.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\w)_", string.Empty);
                }
                else if (ee.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\w\d\d\d)_", string.Empty);
                }
                else if (ff.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\w\w)_", string.Empty);
                }
                else if (gg.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w\d\w\w\d)_", string.Empty);
                }
                else if (hh.Success)
                {
                    strFilePath = Regex.Replace(strFilePath, @"(^\w{5})_", string.Empty);
                }

                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{4})lo", string.Empty); 
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{3})lo", string.Empty);
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})_(\d{2})lo", string.Empty);
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})lo", string.Empty);
            }
            catch (Exception)
            {
            }


            strFilePath = Path.Combine(this.SavePath, Utility.RemoveIllegalCharecters(strFilePath));

            var ccObj = new CacheObject {IsDownloaded = false, FilePath = strFilePath, Url = strImgURL};

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

            // Bypass for Random "Continue to your Image " Button
            var sImageURL = strImgURL.Substring(strImgURL.LastIndexOf("_") + 1);

            if (strImgURL.EndsWith("lo.jpg") ||
                strImgURL.EndsWith("lo.JPG") ||
                strImgURL.EndsWith("lo.jpeg"))
            {
                sImageURL = sImageURL.Replace(strImgURL.Substring(strImgURL.LastIndexOf("lo")), string.Empty);
            }

            sImageURL = strImgURL + "&loc=loc" + sImageURL;

            var strIvPage = this.GetImageHostPage(ref sImageURL);

            if (strIvPage.Length < 10)
            {
               
                return false;
            }

            var strNewURL = string.Empty;

            var iStartIMG = 0;
            var iStartSRC = 0;
            var iEndSRC = 0;
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


            strNewURL =
                $"{strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1)}{strIvPage.Substring(iStartSRC, iEndSRC - iStartSRC)}";

            //////////////////////////////////////////////////////////////////////////
            var NewAlteredPath = Utility.GetSuitableName(strFilePath);

            NewAlteredPath = NewAlteredPath.Replace("lo.", ".");

            if (strFilePath != NewAlteredPath)
            {
                strFilePath = NewAlteredPath;

                ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;
            }

            strFilePath = Utility.CheckPathLength(strFilePath);

            ((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath = strFilePath;

            try
            {
                var client = new WebClient();
                client.Headers.Add("Referer: " + strImgURL);
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

            CacheController.Instance().LastPic =((CacheObject)this.EventTable[this.ImageLinkURL]).FilePath;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        
    }
}
