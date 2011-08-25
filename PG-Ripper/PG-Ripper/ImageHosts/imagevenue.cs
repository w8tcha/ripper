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
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
	/// Worker class to get images hosted on ImageVenue
	/// </summary>
	public class imagevenue : ServiceTemplate
	{
		public imagevenue(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
			: base( sSavePath, strURL, ref hTbl )
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

			string strFilePath = string.Empty;

			strFilePath = strImgURL.Substring(  strImgURL.IndexOf( "image=" ) + 6 );

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
                strFilePath = Regex.Replace(strFilePath, @"_(\d{3})lo", "");
            }
            catch (System.Exception)
            {
                //
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

            if (mSavePath.Contains("/"))
                strFilePath = mSavePath + "/" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;
            else
                strFilePath = mSavePath + "\\" + Utility.RemoveIllegalCharecters(strFilePath); //strFilePath;

			CacheObject CCObj = new CacheObject();
			CCObj.IsDownloaded = false;
			CCObj.FilePath = strFilePath ;
			CCObj.Url = strImgURL;
			try
			{
				eventTable.Add(strImgURL, CCObj);
			}
			catch (ThreadAbortException)
			{
				return true;
			}
			catch(System.Exception)
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
			
			string strIVPage = GetImageHostPage(ref strImgURL);

			if (strIVPage.Length < 10)
			{
				
				return false;
			}

			string strNewURL = string.Empty; 

			int iStartIMG = 0;
			int iStartSRC = 0;
			int iEndSRC = 0;
			iStartIMG = strIVPage.IndexOf("<img id=\"thepic\"");

			if (iStartIMG < 0)
			{
				return false;
			}

			iStartSRC = strIVPage.IndexOf("SRC=\"", iStartIMG);

			if (iStartSRC < 0)
			{
				return false;
			}
			
			iStartSRC += 5;

            iEndSRC = strIVPage.IndexOf("\" alt=\"", iStartSRC);

			if (iEndSRC < 0)
			{
				return false;
			}

            strNewURL = string.Format("{0}{1}",
                strImgURL.Substring(0, strImgURL.IndexOf("/", 8) + 1),
                strIVPage.Substring(iStartSRC, iEndSRC - iStartSRC));
			
			//////////////////////////////////////////////////////////////////////////
			HttpWebRequest lHttpWebRequest;
			HttpWebResponse lHttpWebResponse;
			Stream lHttpWebResponseStream;

			string NewAlteredPath = Utility.GetSuitableName(strFilePath);

            if (strFilePath != NewAlteredPath)
			{
				strFilePath = NewAlteredPath;
				((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
			}

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
					//lFileStream.Close();
					return false;
				}
				//lFileStream.Close();
				lHttpWebResponseStream.Close();

				System.Net.WebClient client = new WebClient();
				client.Headers.Add("Accept-Language: en-us,en;q=0.5");
				client.Headers.Add("Accept-Encoding: gzip,deflate");
				client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				client.Headers.Add("Referer: " + strImgURL );
				client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
				client.DownloadFile( strNewURL, strFilePath );
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
			CacheController.GetInstance().uSLastPic = ((CacheObject)eventTable[mstrURL]).FilePath;

			return true;
		}

		//////////////////////////////////////////////////////////////////////////
		
	}
}
