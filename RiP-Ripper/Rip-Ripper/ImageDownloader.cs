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

using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System;
using RiPRipper.ImageHosts;

namespace RiPRipper
{
    using RiPRipper.Objects;

    public class ImageDownloader
    {
        private string mstrURL = string.Empty;
        private Hashtable eventTable;
        private string mSavePath = string.Empty;
        private ServiceTemplate xService;

        public ImageDownloader( string sSavePath, string strURL, ref Hashtable hTbl )
        {
            mstrURL = strURL;
            eventTable = hTbl;
            mSavePath = sSavePath;
        }

        public void GeneralDownloader()
        {
            xService = new uploadimages_net(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }

        public void GetUploadImage()
        {
            xService = new uploadimages_net( ref mSavePath, ref mstrURL, ref eventTable );
            xService.StartDownload();
        }
        public void GetFapomatic()
        {
            xService = new fapomatic( ref mSavePath, ref mstrURL, ref eventTable );
            xService.StartDownload();
        }

        public void GetImageVenue()
        {
            xService = new imagevenue( ref mSavePath, ref mstrURL, ref eventTable );
            xService.StartDownload();
        }

        public void GetImageVenueNew()
        {
            xService = new imagevenueNew(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }

        public void GetMoast()
        {
            xService = new Moast(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetwatermarkIt()
        {
            xService = new watermarkIt(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicBux()
        {
            xService = new PicBux(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicturesUpload()
        {
            xService = new PicturesUpload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHigh()
        {
            xService = new ImageHigh(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImage2Share()
        {
            xService = new Image2Share(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPaintedOver()
        {
            xService = new PaintedOver(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetDumbARump()
        {
            xService = new DumbARump(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageCrack()
        {
            xService = new ImageCrack(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetTenPix()
        {
            xService = new TenPix(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetSupload()
        {
            xService = new Supload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageThrust()
        {
            xService = new ImageThrust(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetShareAPic()
        {
            xService = new ShareAPic(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFileDen()
        {
            xService = new FileDen(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicTiger()
        {
            xService = new PicTiger(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicTiger2()
        {
            xService = new PicTiger2(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetMyPhotos()
        {
            xService = new MyPhotos(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetTheImageHosting()
        {
            xService = new TheImageHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetZShare()
        {
            xService = new ZShare(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetKeepMyFile()
        {
            xService = new KeepMyFile(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageBeaver()
        {
            xService = new ImageBeaver(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetShareAvenue()
        {
            xService = new ShareAvenue(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetGlowFoto()
        {
            xService = new GlowFoto(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetJpgHosting()
        {
            xService = new JpgHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetJpgHosting2()
        {
            xService = new JpgHosting2(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageFling()
        {
            xService = new ImageFling(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetYourPix()
        {
            xService = new YourPix(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFreeImageHost()
        {
            xService = new FreeImageHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFreeShare()
        {
            xService = new FreeShare(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetSuprFile()
        {
            xService = new SuprFile(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetLetMeHost()
        {
            xService = new LetMeHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFileHost()
        {
            xService = new FileHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetTheFreeImageHosting()
        {
            xService = new TheFreeImageHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetYesAlbum()
        {
            xService = new YesAlbum(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicsPlace()
        {
            xService = new PicsPlace(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetXsHosting()
        {
            xService = new XsHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetCelebs()
        {
            xService = new Celebs(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetRipHq()
        {
            xService = new RipHq(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetBenuri()
        {
            xService = new Benuri(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHaven()
        {
            xService = new ImageHaven(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImagePundit()
        {
            xService = new ImagePundit(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetUploadEm()
        {
            xService = new UploadEm(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetUpPix()
        {
            xService = new UpPix(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPixHosting()
        {
            xService = new PixHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPussyUpload()
        {
            xService = new PussyUpload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetHotLinkImage()
        {
            xService = new HotLinkImage(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageBam()
        {
            xService = new ImageBam(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHosting()
        {
            xService = new ImageHosting(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageFap()
        {
            xService = new ImageFap(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetAllYouCanUpload()
        {
            xService = new AllYouCanUpload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetLargeImageHost()
        {
            xService = new LargeImageHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetRadikal()
        {
            xService = new Radikal(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPixUp()
        {
            xService = new PixUp(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFreePornDumpster()
        {
            xService = new FreePornDumpster(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageSocket()
        {
            xService = new ImageSocket(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetStormFactory()
        {
            xService = new StormFactory(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicHoarder()
        {
            xService = new PicHoarder(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetMultiPics()
        {
            xService = new MultiPics(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageFoco()
        {
            xService = new ImageFoco(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetSpeedImg()
        {
            xService = new SpeedImg(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetDollarLink()
        {
            xService = new DollarLink(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicEasy()
        {
            xService = new PicEasy(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicturesHoster()
        {
            xService = new PicturesHoster(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicJackal()
        {
            xService = new PicJackal(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetAmazingDickSSl()
        {
            xService = new AmazingDickSSl(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImagesGal()
        {
            xService = new ImagesGal(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetBigPics()
        {
            xService = new BigPics(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetXPhotoSharing()
        {
            xService = new XPhotoSharing(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetBusyUpload()
        {
            xService = new BusyUpload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetUpMyPhoto()
        {
            xService = new UpMyPhoto(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetTurboImageHost()
        {
            xService = new TurboImageHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetAbload()
        {
            xService = new Abload(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageDoza()
        {
            xService = new ImageDoza(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageWam()
        {
            xService = new ImageWam(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageFlea()
        {
            xService = new ImageFlea(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageCargo()
        {
            xService = new ImageCargo(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPixSlam()
        {
            xService = new PixSlam(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHost()
        {
            xService = new ImageHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
            xService.StartDownload();
        }
        public void GetMyImageHost()
        {
            xService = new MyImageHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetShareNxs()
        {
            xService = new ShareNxs(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetKemiPic()
        {
            xService = new KemiPic(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFotoTube()
        {
            xService = new FotoTube(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImmage()
        {
            xService = new Immage(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetIpicture()
        {
            xService = new Ipicture(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPornImgHost()
        {
            xService = new PornImgHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageTwist()
        {
            xService = new ImageTwist(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageWaste()
        {
            xService = new ImageWaste(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPixHost()
        {
            xService = new PixHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFastPic()
        {
            xService = new FastPic(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetPicDir()
        {
            xService = new PicDir(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFotoSik()
        {
            xService = new FotoSik(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetDailyPoa()
        {
            xService = new DailyPoa(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHostLi()
        {
            xService = new ImageHostLi(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetStooorage()
        {
            xService = new Stooorage(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImagePorter()
        {
            xService = new ImagePorter(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetFileMad()
        {
            xService = new FileMad(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetMyPixHost()
        {
            xService = new MyPixHost(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetSevenBucket()
        {
            xService = new SevenBucket(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }
        public void GetImageHyper()
        {
            xService = new ImageHyper(ref mSavePath, ref mstrURL, ref eventTable);
            xService.StartDownload();
        }

        /// <summary>
        /// Get ImageGiga Download
        /// </summary>
        public void GetImageGiga()
        {
            this.xService = new ImageGiga(ref this.mSavePath, ref this.mstrURL, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageSwitch Download
        /// </summary>
        public void GetImageSwitch()
        {
            this.xService = new ImageSwitch(ref this.mSavePath, ref this.mstrURL, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgChili Download
        /// </summary>
        public void GetImgChili()
        {
            this.xService = new ImgChili(ref this.mSavePath, ref this.mstrURL, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgDepot Download
        /// </summary>
        public void GetImgDepot()
        {
            this.xService = new ImgDepot(ref this.mSavePath, ref this.mstrURL, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageUpper Download
        /// </summary>
        public void GetImageUpper()
        {
            this.xService = new ImageUpper(ref this.mSavePath, ref this.mstrURL, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Hotlinked image fether...
        /// </summary>
        public void GetImage()
        {
            string strImgURL = mstrURL;

            if (eventTable.ContainsKey(strImgURL))	
            {
                if (((CacheObject)eventTable[strImgURL]).IsDownloaded)
                    return;
            }

            string strFilePath = strImgURL.Substring(  strImgURL.LastIndexOf("/") + 1 );

            try
            {
                if (!Directory.Exists(mSavePath))
                    Directory.CreateDirectory(mSavePath);
            }
            catch (IOException ex)
            {
                MainForm.sDeleteMessage = ex.Message;
                MainForm.bDelete = true;

                return;
            }
            
            strFilePath = Path.Combine(mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            CacheObject ccObj = new CacheObject {IsDownloaded = false, FilePath = strFilePath, Url = strImgURL};
            try
            {
                eventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
                return;
            }
            catch(Exception)
            {
                if (eventTable.ContainsKey(strImgURL))	
                {
                    ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);

                    return;
                }
                eventTable.Add(strImgURL, ccObj);
            }


            //////////////////////////////////////////////////////////////////////////

            HttpWebRequest lHttpWebRequest;
            HttpWebResponse lHttpWebResponse;
            Stream lHttpWebResponseStream;
			
			
            try
            {
                lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strImgURL);

                lHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
                lHttpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
                lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                lHttpWebRequest.Accept = "image/png,*/*;q=0.5";
                //lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
                lHttpWebRequest.KeepAlive = true;
                lHttpWebRequest.Timeout = 20000;


                lHttpWebRequest.Referer = strImgURL.IndexOf("www.ripnetwork.net:82") >= 0 ? "http://rip-productions.net/showthread.php" : strImgURL;
				
                lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();

                lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();
				
                if (lHttpWebResponse.ContentType.IndexOf("image") < 0)
                {
                    lHttpWebResponse.Close();
                    lHttpWebResponseStream.Close();

                    ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;
                    ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
                    return;
                }
                string sNewAlteredPath = Utility.GetSuitableName(strFilePath);
                if (strFilePath != sNewAlteredPath)
                {
                    strFilePath = sNewAlteredPath;
                    ((CacheObject)eventTable[mstrURL]).FilePath = strFilePath;
                }

                lHttpWebResponse.Close();
                lHttpWebResponseStream.Close();

                WebClient client = new WebClient();
                client.Headers.Add("Accept-Language: en-us,en;q=0.5");
                client.Headers.Add("Accept-Encoding: gzip,deflate");
                client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                //client.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
                if (strImgURL.IndexOf("www.ripnetwork.net:82") >= 0)
                    client.Headers.Add("Referer: http://rip-productions.net/showthread.php");
                else
                    client.Headers.Add("Referer: " + strImgURL);
                client.Headers.Add("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strImgURL, strFilePath);

                client.Dispose();

            }
            catch (ThreadAbortException)
            {
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;

                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
                return;
            }
            catch (Exception)
            {
                ((CacheObject)eventTable[strImgURL]).IsDownloaded = false;

                ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
                return;
            }
			
            ((CacheObject)eventTable[strImgURL]).IsDownloaded = true;
            ThreadManager.GetInstance().RemoveThreadbyId(mstrURL);
            CacheController.GetInstance().uSLastPic =((CacheObject)eventTable[strImgURL]).FilePath;
            return;
        }
    }
}