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

namespace PGRipper
{
    using PGRipper.Objects;

    /// <summary>
	/// Worker class for UploadImages.net
	/// </summary>
	public class uploadimages_net : ServiceTemplate
	{
		public uploadimages_net(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
			: base( sSavePath, strURL, ref hTbl )
		{
		}

		protected override bool DoDownload()
		{
			string strImgURL = mstrURL;

			if (eventTable.ContainsKey(strImgURL))	
			{
				return true;
			}

			string strFilePath = string.Empty;

			
			strFilePath = strImgURL.Substring(  strImgURL.IndexOf( "img=" ) + 4 );

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
			int iEndSRC = 0;
			iStartIMG = strIVPage.IndexOf("<img src=\"" + strNewURL);

			if (iStartIMG < 0)
			{
				return false;
			}
			iStartIMG += 10;

			iEndSRC = strIVPage.IndexOf("\"/>", iStartIMG);

			if (iEndSRC < 0)
			{
				return false;
			}
			
			strNewURL = strIVPage.Substring(iStartIMG, iEndSRC - iStartIMG);
			
			//////////////////////////////////////////////////////////////////////////
			HttpWebRequest lHttpWebRequest;
			HttpWebResponse lHttpWebResponse;
			Stream lHttpWebResponseStream;

			//	
			string NewAlteredPath = Utility.GetSuitableName(strFilePath);
			if (strFilePath != NewAlteredPath)
			{
				strFilePath = NewAlteredPath;
				((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
			}

			FileStream lFileStream = new FileStream(strFilePath, FileMode.Create);

			
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
					lFileStream.Close();
					return false;
				}

				lHttpWebResponseStream.Close();

				System.Net.WebClient client = new WebClient();
				client.Headers.Add("Accept-Language: en-us,en;q=0.5");
				client.Headers.Add("Accept-Encoding: gzip,deflate");
				client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				client.Headers.Add("Referer: " + strImgURL );
				client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
				client.DownloadFile(strNewURL, strFilePath);

				/*do
				{
					// Read up to 1000 bytes into the bytesRead array.
					bytesRead = lHttpWebResponseStream.Read(byteBuffer, 0, 999);
					lFileStream.Write(byteBuffer, 0, bytesRead);
				}while(bytesRead > 0);

				lHttpWebResponseStream.Close();*/

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

            //((CacheObject)eventTable[strImgURL]).IsDownloaded = true;
            ((CacheObject)eventTable[mstrURL]).IsDownloaded = true;
			CacheController.GetInstance().uSLastPic = ((CacheObject)eventTable[strImgURL]).FilePath;

			return true;
		}

	}
}
