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

namespace RiPRipper
{
    using System.Collections;
    using System.Threading;
    using RiPRipper.Objects;

    /// <summary>
    /// Summary description for CacheController.
    /// </summary>
	public class CacheController
	{
        // Don't ask me why I named it 'mEventTable', since there's no event happening...
		private Hashtable mEventTable;

		private static CacheController _mInstance;

        public static MainForm xform;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private CacheController()
        {
            mEventTable = new Hashtable();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
		public static CacheController GetInstance()
        {
            return _mInstance ?? (_mInstance = new CacheController());
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
		public void EraseEventTable()
		{
			mEventTable.Clear();
		}

        /// <summary>
        /// Universal string, Last pic Race conditions happen alot on this string, 
        /// but it's function is non-critical enough to ignore those races.
        /// </summary>
		public string uSLastPic = string.Empty; // 
		
        public CacheObject GetObj(string strURL)
		{
            if (mEventTable.ContainsKey(strURL))
            {
                return (CacheObject) mEventTable[strURL];
            }
            return null;
		}

        /// <summary>
        /// Generic method for downloading an image that calls appropriate site-specific downloader 
        /// in a newly created thread.
        /// </summary>
        /// <param name="aImageUrl"></param>
        /// <param name="aLocalPath"></param>
        /// 
		public void DownloadImage
        ( string aImageUrl, string aLocalPath )
		{

			ThreadStart lThreadStart;

            // ImageDownloader is the bridging class between this routine and the
			// ServiceTemplate base class (which is the parent to all hosting site's
			// fetch code).
            ImageDownloader lImageDownloader = new ImageDownloader( aLocalPath, aImageUrl, ref mEventTable );

			if (aImageUrl.IndexOf( @"/img.php?loc=loc") >= 0 )
				lThreadStart = new ThreadStart( lImageDownloader.GetImageVenue  );
            else if (aImageUrl.IndexOf(@"imagevenue.com/img.php?") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageVenueNew);
			/*else if (aImageUrl.IndexOf( "uploadimages.net/" ) >= 0 )
				lThreadStart = new ThreadStart( lImageDownloader.GetUploadImage  );*/
			else if (aImageUrl.IndexOf( "fapomatic.com/" ) >= 0 )
				lThreadStart = new ThreadStart( lImageDownloader.GetFapomatic );
			else if (aImageUrl.IndexOf("moast.com/html/") >= 0)
				lThreadStart = new ThreadStart(lImageDownloader.GetMoast);
			else if (aImageUrl.IndexOf("watermark-it.com/view/") >= 0)
				lThreadStart = new ThreadStart(lImageDownloader.GetwatermarkIt);
			else if (aImageUrl.IndexOf("picbux.com/image.php?id=") >= 0)
				lThreadStart = new ThreadStart(lImageDownloader.GetPicBux);
            else if (aImageUrl.IndexOf("picturesupload.com/show.php/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicturesUpload);
            else if (aImageUrl.IndexOf("imagehigh.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHigh);
            else if (aImageUrl.IndexOf("image2share.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImage2Share);
            else if (aImageUrl.IndexOf("paintedover.com/uploads/show.php") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPaintedOver);
            else if (aImageUrl.IndexOf("dumparump.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetDumbARump);
            else if (aImageUrl.IndexOf("imagecrack.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageCrack);
            else if (aImageUrl.IndexOf("10pix.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetTenPix);
            else if (aImageUrl.IndexOf("supload.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetSupload);
            else if (aImageUrl.IndexOf("imagethrust.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageThrust);
            else if (aImageUrl.IndexOf("shareapic.net/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetShareAPic);
            else if (aImageUrl.IndexOf("fileden.com/pview.php") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFileDen);
            else if (aImageUrl.IndexOf("server5.pictiger.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicTiger);
            else if (aImageUrl.IndexOf(@"pictiger.com/albums/") >= 0 || aImageUrl.IndexOf(@"pictiger.com/images/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicTiger2);
            else if (aImageUrl.IndexOf("celebs.myphotos.cc/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetMyPhotos);
            else if (aImageUrl.IndexOf(@"theimagehosting.com/image.php?") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetTheImageHosting);
            else if (aImageUrl.IndexOf("zshare.net/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetZShare);
            else if (aImageUrl.IndexOf("keepmyfile.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetKeepMyFile);
            else if (aImageUrl.IndexOf("imagebeaver.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageBeaver);
            else if (aImageUrl.IndexOf("shareavenue.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetShareAvenue);
            else if (aImageUrl.IndexOf("glowfoto.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetGlowFoto);
            else if (aImageUrl.IndexOf("jpghosting.com/images/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetJpgHosting);
            else if (aImageUrl.IndexOf("jpghosting.com/showpic.php") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetJpgHosting2);
            else if (aImageUrl.IndexOf("imagefling.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageFling);
            else if (aImageUrl.IndexOf("yourpix.org/view/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetYourPix);
            else if (aImageUrl.IndexOf("freeimagehost.eu/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFreeImageHost);
            else if (aImageUrl.IndexOf("freeshare.us/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFreeShare);
            else if (aImageUrl.IndexOf("suprfile.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetSuprFile);
            else if (aImageUrl.IndexOf("letmehost.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetLetMeHost);
            else if (aImageUrl.IndexOf("filehost.to/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFileHost);
            else if (aImageUrl.IndexOf("thefreeimagehosting.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetTheFreeImageHosting);
            else if (aImageUrl.IndexOf("yesalbum.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetYesAlbum);
            else if (aImageUrl.IndexOf("picsplace.to") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicsPlace);
            else if (aImageUrl.IndexOf("xs.to/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetXsHosting);
            else if (aImageUrl.IndexOf("celebs.sytes.net/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetCelebs);
            else if (aImageUrl.IndexOf("rip-hq.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetRipHq);
            else if (aImageUrl.IndexOf("benuri.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetBenuri);
            else if (aImageUrl.IndexOf(@"imagehaven.net/img.php?id=") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHaven);
            else if (aImageUrl.IndexOf("imagepundit.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImagePundit);
            else if (aImageUrl.IndexOf(@"uploadem.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetUploadEm);
            else if (aImageUrl.IndexOf("uppix.info/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetUpPix);
            else if (aImageUrl.IndexOf("pixhosting.info/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPixHosting);
            else if (aImageUrl.IndexOf("pussyupload.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPussyUpload);
            else if (aImageUrl.IndexOf(@"hotlinkimage.com/img.php?") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetHotLinkImage);
            else if (aImageUrl.IndexOf(@"imagebam.com/image/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageBam);
            else if (aImageUrl.IndexOf(@"imagehosting.gr/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHosting);
            else if (aImageUrl.IndexOf(@"imagefap.com/image.php") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageFap);
            else if (aImageUrl.IndexOf(@"allyoucanupload.webshots.com/v/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetAllYouCanUpload);
            else if (aImageUrl.IndexOf(@"largeimagehost.com/img/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetLargeImageHost);
            else if (aImageUrl.IndexOf(@"radikal.ru/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetRadikal);
            else if (aImageUrl.IndexOf(@"pixup.info/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPixUp);
            else if (aImageUrl.IndexOf(@"freeporndumpster.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFreePornDumpster);
            else if (aImageUrl.IndexOf(@"imagesocket.com/view/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageSocket);
            else if (aImageUrl.IndexOf(@"storm-factory.org/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetStormFactory);
            else if (aImageUrl.IndexOf(@"pichoarder.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicHoarder);
            else if (aImageUrl.IndexOf(@"multipics.net/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetMultiPics);
            else if (aImageUrl.IndexOf(@".imagefoco.com/") >= 0 ||
                aImageUrl.IndexOf(@".cocopic.com/") >= 0 ||
                aImageUrl.IndexOf(@".cocoimage.com/") >= 0 ||
                aImageUrl.IndexOf(@".picfoco.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageFoco);
            else if (aImageUrl.IndexOf(@".speedimg.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetSpeedImg);
            else if (aImageUrl.IndexOf(@".dollarlink.biz/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetDollarLink);
            else if (aImageUrl.IndexOf(@"picseasy.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicEasy);
            else if (aImageUrl.IndexOf(@"pictureshoster.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicturesHoster);
            else if (aImageUrl.IndexOf(@".picjackal.com/") >= 0 ||
                aImageUrl.IndexOf(@".picsharebunny.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicJackal);
            else if (aImageUrl.IndexOf(@"amazingdicksuckingsluts.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetAmazingDickSSl);
            else if (aImageUrl.IndexOf(@"imagesgal.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImagesGal);
            else if (aImageUrl.IndexOf(@"bigpics.ru/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetBigPics);
            else if (aImageUrl.IndexOf(@"xxxphotosharing.com/") >= 0 ||
                aImageUrl.IndexOf(@"myadultimage.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetXPhotoSharing);
            else if (aImageUrl.IndexOf(@"busyupload.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetBusyUpload);
            else if (aImageUrl.IndexOf(@"upmyphoto.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetUpMyPhoto);
            else if (aImageUrl.IndexOf(@"turboimagehost.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetTurboImageHost);
            else if (aImageUrl.IndexOf(@"abload.de/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetAbload);
            else if (aImageUrl.IndexOf(@"imagedoza.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageDoza);
            else if (aImageUrl.IndexOf(@"imagewam.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageWam);
            else if (aImageUrl.IndexOf(@"imageflea.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageFlea);
            else if (aImageUrl.IndexOf(@"imagecargo.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageCargo);
            else if (aImageUrl.IndexOf(@"pixslam.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPixSlam);
            else if (aImageUrl.IndexOf(@"imagehost.org/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHost);
            else if (aImageUrl.IndexOf(@"my-image-host.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetMyImageHost);
            else if (aImageUrl.IndexOf(@"sharenxs.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetShareNxs);
            else if (aImageUrl.IndexOf(@"kemipic.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetKemiPic);
            else if (aImageUrl.IndexOf(@"fototube.pl/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFotoTube);
            else if (aImageUrl.IndexOf(@"immage.de/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImmage);
            else if (aImageUrl.IndexOf(@"ipicture.ru/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetIpicture);
            else if (aImageUrl.IndexOf(@"pornimghost.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPornImgHost);
            else if (aImageUrl.IndexOf(@"imagetwist.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageTwist);
            else if (aImageUrl.IndexOf(@"imagewaste.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageWaste);
            else if (aImageUrl.IndexOf(@"pixhost.org/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPixHost);
            else if (aImageUrl.IndexOf(@"fastpic.ru/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFastPic);
            else if (aImageUrl.IndexOf(@"picdir.de/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetPicDir);
            else if (aImageUrl.IndexOf(@"fotosik.pl/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFotoSik);
            else if (aImageUrl.IndexOf(@"dailypoa.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetDailyPoa);
            else if (aImageUrl.IndexOf(@"imagehost.li/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHostLi);
            else if (aImageUrl.IndexOf(@"stooorage.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetStooorage);
            else if (aImageUrl.IndexOf(@"imageporter.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImagePorter);
            else if (aImageUrl.IndexOf(@"filemad.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetFileMad);
            else if (aImageUrl.IndexOf(@"mypixhost.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetMyPixHost);
            else if (aImageUrl.IndexOf(@"7bucket.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetSevenBucket);
            else if (aImageUrl.IndexOf(@"imagehyper.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageHyper);
            else if (aImageUrl.IndexOf(@"imgiga.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageGiga);
            else if (aImageUrl.IndexOf(@"imageswitch.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageSwitch);

            else if (aImageUrl.IndexOf(@"imageupper.com/") >= 0)
                lThreadStart = new ThreadStart(lImageDownloader.GetImageUpper);
                
            else if (aImageUrl.IndexOf("ayhja.com/") >= 0)
                return;
            else
				lThreadStart = new ThreadStart( lImageDownloader.GetImage  ); // hotlinked.

            ThreadManager lTdm = ThreadManager.GetInstance();
			lTdm.LaunchThread(aImageUrl, lThreadStart);
			
		}
	}
}
