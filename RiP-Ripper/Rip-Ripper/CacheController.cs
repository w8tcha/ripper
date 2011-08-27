// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheController.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    #region

    using System.Collections;
    using System.Threading;

    using RiPRipper.Objects;

    #endregion

    /// <summary>
    ///   Cache Controller Class.
    /// </summary>
    public class CacheController
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        public static MainForm xform;

        /// <summary>
        ///   Universal string, Last pic Race conditions happen alot on this string, 
        ///   but it's function is non-critical enough to ignore those races.
        /// </summary>
        public string uSLastPic = string.Empty; // 

        /// <summary>
        /// </summary>
        private static CacheController _mInstance;

        /// <summary>
        /// </summary>
        private Hashtable mEventTable;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CacheController"/> class from being created.
        /// </summary>
        private CacheController()
        {
            this.mEventTable = new Hashtable();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static CacheController GetInstance()
        {
            return _mInstance ?? (_mInstance = new CacheController());
        }

        /// <summary>
        /// Generic method for downloading an image that calls appropriate site-specific downloader 
        ///   in a newly created thread.
        /// </summary>
        /// <param name="aImageUrl">
        /// The a Image Url.
        /// </param>
        /// <param name="aLocalPath">
        /// The a Local Path.
        /// </param>
        public void DownloadImage(string aImageUrl, string aLocalPath)
        {
            ThreadStart lThreadStart;

            // ImageDownloader is the bridging class between this routine and the
            // ServiceTemplate base class (which is the parent to all hosting site's
            // fetch code).
            ImageDownloader lImageDownloader = new ImageDownloader(aLocalPath, aImageUrl, ref this.mEventTable);

            if (aImageUrl.IndexOf(@"/img.php?loc=loc") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageVenue;
            }
            else if (aImageUrl.IndexOf(@"imagevenue.com/img.php?") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageVenueNew;
            }
            else if (aImageUrl.IndexOf("fapomatic.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFapomatic;
            }
            else if (aImageUrl.IndexOf("moast.com/html/") >= 0)
            {
                lThreadStart = lImageDownloader.GetMoast;
            }
            else if (aImageUrl.IndexOf("watermark-it.com/view/") >= 0)
            {
                lThreadStart = lImageDownloader.GetwatermarkIt;
            }
            else if (aImageUrl.IndexOf("picbux.com/image.php?id=") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicBux;
            }
            else if (aImageUrl.IndexOf("picturesupload.com/show.php/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicturesUpload;
            }
            else if (aImageUrl.IndexOf("imagehigh.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHigh;
            }
            else if (aImageUrl.IndexOf("image2share.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImage2Share;
            }
            else if (aImageUrl.IndexOf("paintedover.com/uploads/show.php") >= 0)
            {
                lThreadStart = lImageDownloader.GetPaintedOver;
            }
            else if (aImageUrl.IndexOf("dumparump.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetDumbARump;
            }
            else if (aImageUrl.IndexOf("imagecrack.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageCrack;
            }
            else if (aImageUrl.IndexOf("10pix.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetTenPix;
            }
            else if (aImageUrl.IndexOf("supload.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetSupload;
            }
            else if (aImageUrl.IndexOf("imagethrust.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageThrust;
            }
            else if (aImageUrl.IndexOf("shareapic.net/") >= 0)
            {
                lThreadStart = lImageDownloader.GetShareAPic;
            }
            else if (aImageUrl.IndexOf("fileden.com/pview.php") >= 0)
            {
                lThreadStart = lImageDownloader.GetFileDen;
            }
            else if (aImageUrl.IndexOf("server5.pictiger.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicTiger;
            }
            else if (aImageUrl.IndexOf(@"pictiger.com/albums/") >= 0 || aImageUrl.IndexOf(@"pictiger.com/images/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicTiger2;
            }
            else if (aImageUrl.IndexOf("celebs.myphotos.cc/") >= 0)
            {
                lThreadStart = lImageDownloader.GetMyPhotos;
            }
            else if (aImageUrl.IndexOf(@"theimagehosting.com/image.php?") >= 0)
            {
                lThreadStart = lImageDownloader.GetTheImageHosting;
            }
            else if (aImageUrl.IndexOf("zshare.net/") >= 0)
            {
                lThreadStart = lImageDownloader.GetZShare;
            }
            else if (aImageUrl.IndexOf("keepmyfile.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetKeepMyFile;
            }
            else if (aImageUrl.IndexOf("imagebeaver.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageBeaver;
            }
            else if (aImageUrl.IndexOf("shareavenue.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetShareAvenue;
            }
            else if (aImageUrl.IndexOf("glowfoto.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetGlowFoto;
            }
            else if (aImageUrl.IndexOf("jpghosting.com/images/") >= 0)
            {
                lThreadStart = lImageDownloader.GetJpgHosting;
            }
            else if (aImageUrl.IndexOf("jpghosting.com/showpic.php") >= 0)
            {
                lThreadStart = lImageDownloader.GetJpgHosting2;
            }
            else if (aImageUrl.IndexOf("imagefling.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageFling;
            }
            else if (aImageUrl.IndexOf("yourpix.org/view/") >= 0)
            {
                lThreadStart = lImageDownloader.GetYourPix;
            }
            else if (aImageUrl.IndexOf("freeimagehost.eu/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFreeImageHost;
            }
            else if (aImageUrl.IndexOf("freeshare.us/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFreeShare;
            }
            else if (aImageUrl.IndexOf("suprfile.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetSuprFile;
            }
            else if (aImageUrl.IndexOf("letmehost.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetLetMeHost;
            }
            else if (aImageUrl.IndexOf("filehost.to/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFileHost;
            }
            else if (aImageUrl.IndexOf("thefreeimagehosting.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetTheFreeImageHosting;
            }
            else if (aImageUrl.IndexOf("yesalbum.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetYesAlbum;
            }
            else if (aImageUrl.IndexOf("picsplace.to") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicsPlace;
            }
            else if (aImageUrl.IndexOf("xs.to/") >= 0)
            {
                lThreadStart = lImageDownloader.GetXsHosting;
            }
            else if (aImageUrl.IndexOf("celebs.sytes.net/") >= 0)
            {
                lThreadStart = lImageDownloader.GetCelebs;
            }
            else if (aImageUrl.IndexOf("rip-hq.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetRipHq;
            }
            else if (aImageUrl.IndexOf("benuri.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetBenuri;
            }
            else if (aImageUrl.IndexOf(@"imagehaven.net/img.php?id=") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHaven;
            }
            else if (aImageUrl.IndexOf("imagepundit.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImagePundit;
            }
            else if (aImageUrl.IndexOf(@"uploadem.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetUploadEm;
            }
            else if (aImageUrl.IndexOf("uppix.info/") >= 0)
            {
                lThreadStart = lImageDownloader.GetUpPix;
            }
            else if (aImageUrl.IndexOf("pixhosting.info/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPixHosting;
            }
            else if (aImageUrl.IndexOf("pussyupload.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPussyUpload;
            }
            else if (aImageUrl.IndexOf(@"hotlinkimage.com/img.php?") >= 0)
            {
                lThreadStart = lImageDownloader.GetHotLinkImage;
            }
            else if (aImageUrl.IndexOf(@"imagebam.com/image/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageBam;
            }
            else if (aImageUrl.IndexOf(@"imagehosting.gr/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHosting;
            }
            else if (aImageUrl.IndexOf(@"imagefap.com/image.php") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageFap;
            }
            else if (aImageUrl.IndexOf(@"allyoucanupload.webshots.com/v/") >= 0)
            {
                lThreadStart = lImageDownloader.GetAllYouCanUpload;
            }
            else if (aImageUrl.IndexOf(@"largeimagehost.com/img/") >= 0)
            {
                lThreadStart = lImageDownloader.GetLargeImageHost;
            }
            else if (aImageUrl.IndexOf(@"radikal.ru/") >= 0)
            {
                lThreadStart = lImageDownloader.GetRadikal;
            }
            else if (aImageUrl.IndexOf(@"pixup.info/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPixUp;
            }
            else if (aImageUrl.IndexOf(@"freeporndumpster.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFreePornDumpster;
            }
            else if (aImageUrl.IndexOf(@"imagesocket.com/view/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageSocket;
            }
            else if (aImageUrl.IndexOf(@"storm-factory.org/") >= 0)
            {
                lThreadStart = lImageDownloader.GetStormFactory;
            }
            else if (aImageUrl.IndexOf(@"pichoarder.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicHoarder;
            }
            else if (aImageUrl.IndexOf(@"multipics.net/") >= 0)
            {
                lThreadStart = lImageDownloader.GetMultiPics;
            }
            else if (aImageUrl.IndexOf(@".imagefoco.com/") >= 0 || aImageUrl.IndexOf(@".cocopic.com/") >= 0 ||
                     aImageUrl.IndexOf(@".cocoimage.com/") >= 0 || aImageUrl.IndexOf(@".picfoco.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageFoco;
            }
            else if (aImageUrl.IndexOf(@".speedimg.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetSpeedImg;
            }
            else if (aImageUrl.IndexOf(@".dollarlink.biz/") >= 0)
            {
                lThreadStart = lImageDownloader.GetDollarLink;
            }
            else if (aImageUrl.IndexOf(@"picseasy.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicEasy;
            }
            else if (aImageUrl.IndexOf(@"pictureshoster.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicturesHoster;
            }
            else if (aImageUrl.IndexOf(@".picjackal.com/") >= 0 || aImageUrl.IndexOf(@".picsharebunny.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicJackal;
            }
            else if (aImageUrl.IndexOf(@"amazingdicksuckingsluts.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetAmazingDickSSl;
            }
            else if (aImageUrl.IndexOf(@"imagesgal.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImagesGal;
            }
            else if (aImageUrl.IndexOf(@"bigpics.ru/") >= 0)
            {
                lThreadStart = lImageDownloader.GetBigPics;
            }
            else if (aImageUrl.IndexOf(@"xxxphotosharing.com/") >= 0 || aImageUrl.IndexOf(@"myadultimage.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetXPhotoSharing;
            }
            else if (aImageUrl.IndexOf(@"busyupload.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetBusyUpload;
            }
            else if (aImageUrl.IndexOf(@"upmyphoto.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetUpMyPhoto;
            }
            else if (aImageUrl.IndexOf(@"turboimagehost.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetTurboImageHost;
            }
            else if (aImageUrl.IndexOf(@"abload.de/") >= 0)
            {
                lThreadStart = lImageDownloader.GetAbload;
            }
            else if (aImageUrl.IndexOf(@"imagedoza.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageDoza;
            }
            else if (aImageUrl.IndexOf(@"imagewam.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageWam;
            }
            else if (aImageUrl.IndexOf(@"imageflea.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageFlea;
            }
            else if (aImageUrl.IndexOf(@"imagecargo.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageCargo;
            }
            else if (aImageUrl.IndexOf(@"pixslam.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPixSlam;
            }
            else if (aImageUrl.IndexOf(@"imagehost.org/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHost;
            }
            else if (aImageUrl.IndexOf(@"my-image-host.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetMyImageHost;
            }
            else if (aImageUrl.IndexOf(@"sharenxs.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetShareNxs;
            }
            else if (aImageUrl.IndexOf(@"kemipic.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetKemiPic;
            }
            else if (aImageUrl.IndexOf(@"fototube.pl/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFotoTube;
            }
            else if (aImageUrl.IndexOf(@"immage.de/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImmage;
            }
            else if (aImageUrl.IndexOf(@"ipicture.ru/") >= 0)
            {
                lThreadStart = lImageDownloader.GetIpicture;
            }
            else if (aImageUrl.IndexOf(@"pornimghost.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPornImgHost;
            }
            else if (aImageUrl.IndexOf(@"imagetwist.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageTwist;
            }
            else if (aImageUrl.IndexOf(@"imagewaste.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageWaste;
            }
            else if (aImageUrl.IndexOf(@"pixhost.org/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPixHost;
            }
            else if (aImageUrl.IndexOf(@"fastpic.ru/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFastPic;
            }
            else if (aImageUrl.IndexOf(@"picdir.de/") >= 0)
            {
                lThreadStart = lImageDownloader.GetPicDir;
            }
            else if (aImageUrl.IndexOf(@"fotosik.pl/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFotoSik;
            }
            else if (aImageUrl.IndexOf(@"dailypoa.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetDailyPoa;
            }
            else if (aImageUrl.IndexOf(@"imagehost.li/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHostLi;
            }
            else if (aImageUrl.IndexOf(@"stooorage.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetStooorage;
            }
            else if (aImageUrl.IndexOf(@"imageporter.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImagePorter;
            }
            else if (aImageUrl.IndexOf(@"filemad.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetFileMad;
            }
            else if (aImageUrl.IndexOf(@"mypixhost.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetMyPixHost;
            }
            else if (aImageUrl.IndexOf(@"7bucket.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetSevenBucket;
            }
            else if (aImageUrl.IndexOf(@"imagehyper.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageHyper;
            }
            else if (aImageUrl.IndexOf(@"imgiga.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageGiga;
            }
            else if (aImageUrl.IndexOf(@"imageswitch.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageSwitch;
            }
            else if (aImageUrl.IndexOf(@"imageupper.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImageUpper;
            }
            else if (aImageUrl.IndexOf(@"imgchili.com/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImgChili;
            }
            else if (aImageUrl.IndexOf(@"imgdepot.org/") >= 0)
            {
                lThreadStart = lImageDownloader.GetImgDepot;
            }
            else if (aImageUrl.IndexOf("ayhja.com/") >= 0)
            {
                return;
            }
            else
            {
                lThreadStart = lImageDownloader.GetImage; // hotlinked.
            }

            ThreadManager lTdm = ThreadManager.GetInstance();
            lTdm.LaunchThread(aImageUrl, lThreadStart);
        }

        /// <summary>
        /// Erase the Event Table
        /// </summary>
        public void EraseEventTable()
        {
            this.mEventTable.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="strURL">
        /// The str url.
        /// </param>
        /// <returns>
        /// </returns>
        public CacheObject GetObj(string strURL)
        {
            if (this.mEventTable.ContainsKey(strURL))
            {
                return (CacheObject)this.mEventTable[strURL];
            }

            return null;
        }

        #endregion
    }
}