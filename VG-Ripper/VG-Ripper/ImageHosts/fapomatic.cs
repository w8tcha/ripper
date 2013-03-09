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
		public fapomatic(ref string sSavePath, ref string strURL, ref string thumbURL, ref string imageName, ref Hashtable hTbl)
			: base(sSavePath, strURL, thumbURL, imageName, ref hTbl)
		{
		}

		protected override bool DoDownload()
		{
		
			string strImgURL = ImageLinkURL;

			if (EventTable.ContainsKey(strImgURL))	
			{
				return true;;
			}

			string strFilePath = string.Empty;

			
			strFilePath = strImgURL.Substring(  strImgURL.IndexOf( "f=" ) + 2 );

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

            strFilePath = Path.Combine(SavePath, Utility.RemoveIllegalCharecters(strFilePath));

			CacheObject CCObj = new CacheObject();
			CCObj.IsDownloaded = false;
			CCObj.FilePath = strFilePath ;
			CCObj.Url = strImgURL;
			try
			{
				EventTable.Add(strImgURL, CCObj);
			}
			catch (ThreadAbortException)
			{
				return true;
			}
			catch(System.Exception)
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
				((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;
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
            CacheController.GetInstance().LastPic =((CacheObject)EventTable[ImageLinkURL]).FilePath = strFilePath;

			return true;
		}

	}
}
