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
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace RiPRipper
{
    using RiPRipper.Objects;

    /// <summary>
	/// Worker class to get images from fapomatic.com
	/// </summary>
	public class fapomatic : ServiceTemplate
	{
		public fapomatic(ref string sSavePath, ref string strURL, ref Hashtable hTbl)
			: base( sSavePath, strURL, ref hTbl )
		{
		}

		protected override bool DoDownload()
		{
		
			string strImgURL = mstrURL;

			if (eventTable.ContainsKey(strImgURL))	
			{
				return true;;
			}

			string strFilePath = string.Empty;

			
			strFilePath = strImgURL.Substring(  strImgURL.IndexOf( "f=" ) + 2 );

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

            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

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
			
			int iLocStart = strImgURL.IndexOf( "loc=" );
			if (iLocStart < 0)
			{
				return false;
			}
			iLocStart += 4;

            int iNameStart = strImgURL.IndexOf("f=");
            if (iNameStart < 0)
            {
                return false;
            }
            iNameStart += 2;

			string strNewURL = string.Format("http://fapomatic.com/{0}/{1}", 
                strImgURL.Substring(iLocStart, strImgURL.IndexOf( "&", iLocStart ) - iLocStart  ),
                strImgURL.Substring(iNameStart));
			
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
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strNewURL, strFilePath);
                client.Dispose();

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
            CacheController.GetInstance().uSLastPic =((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;

			return true;
		}

	}
}
