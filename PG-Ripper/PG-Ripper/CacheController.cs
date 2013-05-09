// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheController.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PGRipper
{
    #region

    using System.Collections;
    using System.Threading;

    using PGRipper.Objects;

    #endregion

    /// <summary>
    ///   Cache Controller Class.
    /// </summary>
    public class CacheController
    {
        #region Constants and Fields

        /// <summary>
        ///   Universal string, Last pic Race conditions happen a lot on this string, 
        ///   but it's function is non-critical enough to ignore those races.
        /// </summary>
        public string LastPic = string.Empty;

        /// <summary>
        /// The CacheController Instance
        /// </summary>
        private static CacheController _mInstance;

        /// <summary>
        /// The Table that contains the image urls
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

        /// <summary>
        /// Gets or sets the Main Form
        /// </summary>
        public static MainForm Xform { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static CacheController Instance()
        {
            return _mInstance ?? (_mInstance = new CacheController());
        }

        /// <summary>
        /// Generic method for downloading an image that calls appropriate site-specific downloader
        /// in a newly created thread.
        /// </summary>
        /// <param name="imageUrl">The Image Url.</param>
        /// <param name="thumbImageUrl">The thumb image URL.</param>
        /// <param name="localPath">The Local Path.</param>
        /// <param name="imageName">Name of the image.</param>
        public void DownloadImage(string imageUrl, string thumbImageUrl, string localPath, string imageName)
        {
            ThreadStart lThreadStart;

            //imageUrl = imageUrl.ToLower();

            thumbImageUrl = thumbImageUrl.ToLower();

            // ImageDownloader is the bridging class between this routine and the
            // ServiceTemplate base class (which is the parent to all hosting site's
            // fetch code).
            var imageDownloader = new ImageDownloader(localPath, imageUrl, thumbImageUrl, imageName, ref this.mEventTable);

            if (imageUrl.IndexOf(@"/img.php?loc=loc") >= 0)
            {
                lThreadStart = imageDownloader.GetImageVenue;
            }
            else if (imageUrl.IndexOf(@"imagevenue.com/img.php?") >= 0)
            {
                lThreadStart = imageDownloader.GetImageVenueNew;
            }
            else if (imageUrl.IndexOf("fapomatic.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetFapomatic;
            }
            else if (imageUrl.IndexOf("moast.com/html/") >= 0)
            {
                lThreadStart = imageDownloader.GetMoast;
            }
            else if (imageUrl.IndexOf("watermark-it.com/view/") >= 0)
            {
                lThreadStart = imageDownloader.GetwatermarkIt;
            }
            else if (imageUrl.IndexOf("picbux.com/image.php?id=") >= 0)
            {
                lThreadStart = imageDownloader.GetPicBux;
            }
            else if (imageUrl.IndexOf("picturesupload.com/show.php/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicturesUpload;
            }
            else if (imageUrl.IndexOf("imagehigh.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHigh;
            }
            else if (imageUrl.IndexOf("image2share.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImage2Share;
            }
            else if (imageUrl.IndexOf("paintedover.com/uploads/show.php") >= 0)
            {
                lThreadStart = imageDownloader.GetPaintedOver;
            }
            else if (imageUrl.IndexOf("dumparump.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetDumbARump;
            }
            else if (imageUrl.IndexOf("imagecrack.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageCrack;
            }
            else if (imageUrl.IndexOf("10pix.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetTenPix;
            }
            else if (imageUrl.IndexOf("supload.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetSupload;
            }
            else if (imageUrl.IndexOf("imagethrust.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageThrust;
            }
            else if (imageUrl.IndexOf("shareapic.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetShareAPic;
            }
            else if (imageUrl.IndexOf("fileden.com/pview.php") >= 0)
            {
                lThreadStart = imageDownloader.GetFileDen;
            }
            else if (imageUrl.IndexOf("server5.pictiger.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicTiger;
            }
            else if (imageUrl.IndexOf(@"pictiger.com/albums/") >= 0 || imageUrl.IndexOf(@"pictiger.com/images/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicTiger2;
            }
            else if (imageUrl.IndexOf("celebs.myphotos.cc/") >= 0)
            {
                lThreadStart = imageDownloader.GetMyPhotos;
            }
            else if (imageUrl.IndexOf(@"theimagehosting.com/image.php?") >= 0)
            {
                lThreadStart = imageDownloader.GetTheImageHosting;
            }
            else if (imageUrl.IndexOf("zshare.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetZShare;
            }
            else if (imageUrl.IndexOf("keepmyfile.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetKeepMyFile;
            }
            else if (imageUrl.IndexOf("imagebeaver.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageBeaver;
            }
            else if (imageUrl.IndexOf("shareavenue.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetShareAvenue;
            }
            else if (imageUrl.IndexOf("glowfoto.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetGlowFoto;
            }
            else if (imageUrl.IndexOf("jpghosting.com/images/") >= 0)
            {
                lThreadStart = imageDownloader.GetJpgHosting;
            }
            else if (imageUrl.IndexOf("jpghosting.com/showpic.php") >= 0)
            {
                lThreadStart = imageDownloader.GetJpgHosting2;
            }
            else if (imageUrl.IndexOf("imagefling.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageFling;
            }
            else if (imageUrl.IndexOf("yourpix.org/view/") >= 0)
            {
                lThreadStart = imageDownloader.GetYourPix;
            }
            else if (imageUrl.IndexOf("freeimagehost.eu/") >= 0)
            {
                lThreadStart = imageDownloader.GetFreeImageHost;
            }
            else if (imageUrl.IndexOf("freeshare.us/") >= 0)
            {
                lThreadStart = imageDownloader.GetFreeShare;
            }
            else if (imageUrl.IndexOf("suprfile.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetSuprFile;
            }
            else if (imageUrl.IndexOf("letmehost.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetLetMeHost;
            }
            else if (imageUrl.IndexOf("filehost.to/") >= 0)
            {
                lThreadStart = imageDownloader.GetFileHost;
            }
            else if (imageUrl.IndexOf("thefreeimagehosting.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetTheFreeImageHosting;
            }
            else if (imageUrl.IndexOf("yesalbum.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetYesAlbum;
            }
            else if (imageUrl.IndexOf("picsplace.to") >= 0)
            {
                lThreadStart = imageDownloader.GetPicsPlace;
            }
            else if (imageUrl.IndexOf("xs.to/") >= 0)
            {
                lThreadStart = imageDownloader.GetXsHosting;
            }
            else if (imageUrl.IndexOf("celebs.sytes.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetCelebs;
            }
            else if (imageUrl.IndexOf("rip-hq.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetRipHq;
            }
            else if (imageUrl.IndexOf("benuri.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetBenuri;
            }
            else if (imageUrl.IndexOf(@"imagehaven.net/img.php?id=") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHaven;
            }
            else if (imageUrl.IndexOf("imagepundit.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImagePundit;
            }
            else if (imageUrl.IndexOf(@"uploadem.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetUploadEm;
            }
            else if (imageUrl.IndexOf("uppix.info/") >= 0)
            {
                lThreadStart = imageDownloader.GetUpPix;
            }
            else if (imageUrl.IndexOf("pixhosting.info/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixHosting;
            }
            else if (imageUrl.IndexOf("pussyupload.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPussyUpload;
            }
            else if (imageUrl.IndexOf(@"hotlinkimage.com/img.php?") >= 0)
            {
                lThreadStart = imageDownloader.GetHotLinkImage;
            }
            else if (imageUrl.IndexOf(@"imagebam.com/image/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageBam;
            }
            else if (imageUrl.IndexOf(@"imagehosting.gr/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHosting;
            }
            else if (imageUrl.IndexOf(@"imagefap.com/image.php") >= 0)
            {
                lThreadStart = imageDownloader.GetImageFap;
            }
            else if (imageUrl.IndexOf(@"allyoucanupload.webshots.com/v/") >= 0)
            {
                lThreadStart = imageDownloader.GetAllYouCanUpload;
            }
            else if (imageUrl.IndexOf(@"largeimagehost.com/img/") >= 0)
            {
                lThreadStart = imageDownloader.GetLargeImageHost;
            }
            else if (imageUrl.IndexOf(@"radikal.ru/") >= 0)
            {
                lThreadStart = imageDownloader.GetRadikal;
            }
            else if (imageUrl.IndexOf(@"pixup.info/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixUp;
            }
            else if (imageUrl.IndexOf(@"freeporndumpster.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetFreePornDumpster;
            }
            else if (imageUrl.IndexOf(@"imagesocket.com/view/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageSocket;
            }
            else if (imageUrl.IndexOf(@"storm-factory.org/") >= 0)
            {
                lThreadStart = imageDownloader.GetStormFactory;
            }
            else if (imageUrl.IndexOf(@"pichoarder.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicHoarder;
            }
            else if (imageUrl.IndexOf(@"multipics.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetMultiPics;
            }
            else if (imageUrl.IndexOf(@"attachment.php") >= 0)
            {
                lThreadStart = imageDownloader.GetForumAttachment;
            }
            else if (imageUrl.IndexOf(@".imagefoco.com/") >= 0 || imageUrl.IndexOf(@".cocopic.com/") >= 0 ||
                     imageUrl.IndexOf(@".cocoimage.com/") >= 0 || imageUrl.IndexOf(@".picfoco.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageFoco;
            }
            else if (imageUrl.IndexOf(@".speedimg.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetSpeedImg;
            }
            else if (imageUrl.IndexOf(@".dollarlink.biz/") >= 0)
            {
                lThreadStart = imageDownloader.GetDollarLink;
            }
            else if (imageUrl.IndexOf(@"picseasy.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicEasy;
            }
            else if (imageUrl.IndexOf(@"pictureshoster.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicturesHoster;
            }
            else if (imageUrl.IndexOf(@".picjackal.com/") >= 0 || imageUrl.IndexOf(@".picsharebunny.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicJackal;
            }
            else if (imageUrl.IndexOf(@"amazingdicksuckingsluts.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetAmazingDickSSl;
            }
            else if (imageUrl.IndexOf(@"imagesgal.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImagesGal;
            }
            else if (imageUrl.IndexOf(@"bigpics.ru/") >= 0)
            {
                lThreadStart = imageDownloader.GetBigPics;
            }
            else if (imageUrl.IndexOf(@"xxxphotosharing.com/") >= 0 || imageUrl.IndexOf(@"myadultimage.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetXPhotoSharing;
            }
            else if (imageUrl.IndexOf(@"busyupload.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetBusyUpload;
            }
            else if (imageUrl.IndexOf(@"upmyphoto.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetUpMyPhoto;
            }
            else if (imageUrl.IndexOf(@"picslibraries.blogspot.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicsLibraries;
            }
            else if (imageUrl.IndexOf(@"turboimagehost.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetTurboImageHost;
            }
            else if (imageUrl.IndexOf(@"abload.de/") >= 0)
            {
                lThreadStart = imageDownloader.GetAbload;
            }
            else if (imageUrl.IndexOf(@"imagedoza.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageDoza;
            }
            else if (imageUrl.IndexOf(@"imagewam.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageWam;
            }
            else if (imageUrl.IndexOf(@"imageflea.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageFlea;
            }
            else if (imageUrl.IndexOf(@"imagecargo.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageCargo;
            }
            else if (imageUrl.IndexOf(@"pixslam.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixSlam;
            }
            else if (imageUrl.IndexOf(@"imagehost.org/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHost;
            }
            else if (imageUrl.IndexOf(@"my-image-host.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetMyImageHost;
            }
            else if (imageUrl.IndexOf(@"sharenxs.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetShareNxs;
            }
            else if (imageUrl.IndexOf(@"kemipic.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetKemiPic;
            }
            else if (imageUrl.IndexOf(@"fototube.pl/") >= 0)
            {
                lThreadStart = imageDownloader.GetFotoTube;
            }
            else if (imageUrl.IndexOf(@"immage.de/") >= 0)
            {
                lThreadStart = imageDownloader.GetImmage;
            }
            else if (imageUrl.IndexOf(@"ipicture.ru/") >= 0)
            {
                lThreadStart = imageDownloader.GetIpicture;
            }
            else if (imageUrl.IndexOf(@"pornimghost.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPornImgHost;
            }
            else if (imageUrl.IndexOf(@"imagetwist.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageTwist;
            }
            else if (imageUrl.IndexOf(@"imagewaste.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageWaste;
            }
            else if (imageUrl.IndexOf(@"pixhost.org/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixHost;
            }
            else if (imageUrl.IndexOf(@"fastpic.ru/") >= 0)
            {
                lThreadStart = imageDownloader.GetFastPic;
            }
            else if (imageUrl.IndexOf(@"picdir.de/") >= 0)
            {
                lThreadStart = imageDownloader.GetPicDir;
            }
            else if (imageUrl.IndexOf(@"fotosik.pl/") >= 0)
            {
                lThreadStart = imageDownloader.GetFotoSik;
            }
            else if (imageUrl.IndexOf(@"dailypoa.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetDailyPoa;
            }
            else if (imageUrl.IndexOf(@"imagehost.li/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHostLi;
            }
            else if (imageUrl.IndexOf(@"stooorage.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetStooorage;
            }
            else if (imageUrl.IndexOf(@"imageporter.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImagePorter;
            }
            else if (imageUrl.IndexOf(@"filemad.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetFileMad;
            }
            else if (imageUrl.IndexOf(@"mypixhost.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetMyPixHost;
            }
            else if (imageUrl.IndexOf(@"7bucket.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetSevenBucket;
            }
            else if (imageUrl.IndexOf(@"imagehyper.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageHyper;
            }
            else if (imageUrl.IndexOf(@"imgiga.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageGiga;
            }
            else if (imageUrl.IndexOf(@"imageswitch.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageSwitch;
            }
            else if (imageUrl.IndexOf(@"imageupper.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageUpper;
            }
            else if (imageUrl.IndexOf(@"imgchili.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgChili;
            }
            else if (imageUrl.IndexOf(@"imgdepot.org/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgDepot;
            }
            else if (imageUrl.IndexOf(@"imagepad.us/") >= 0)
            {
                lThreadStart = imageDownloader.GetImagePad;
            }
            else if (imageUrl.IndexOf(@"imagebunk.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageBunk;
            }
            else if (imageUrl.IndexOf(@"pimpandhost.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPimpAndHost;
            }
            else if (imageUrl.IndexOf(@"dumppix.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetDumpPix;
            }
            else if (imageUrl.IndexOf(@"hoooster.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetHoooster;
            }
            else if (imageUrl.IndexOf(@"pixhub.eu/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixHub;
            }
            else if (imageUrl.IndexOf(@"pixroute.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetPixRoute;
            }
            else if (imageUrl.IndexOf(@"imagepicsa.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImagePicasa;
            }
            else if (imageUrl.IndexOf(@"directupload.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetDirectUpload;
            }
            else if (imageUrl.IndexOf(@"imgbox.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgBox;
            }
            else if (imageUrl.IndexOf(@"imgdino.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgDino;
            }
            else if (imageUrl.IndexOf(@"imgwoot.com/") >= 0 || imageUrl.IndexOf(@"Imgwoot.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgmoney.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgproof.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"pixup.us/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgonion.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imagefolks.com/") >= 0 || imageUrl.IndexOf(@"ImageFolks.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImageFolks;
            }
            else if (imageUrl.IndexOf(@"imgpo.st/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgPo;
            }
            else if (imageUrl.IndexOf(@"imgah.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImGah;
            }
            else if (imageUrl.IndexOf(@"imgur.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgUr;
            }
            else if (imageUrl.IndexOf(@"tuspics.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetTusPics;
            }
            else if (imageUrl.IndexOf(@"freeimagepic.com/") >= 0)
            {
                lThreadStart = imageDownloader.GetFreeImagePic;
            }
            else if (imageUrl.IndexOf(@"imgserve.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetImgServe;
            }
            else if (imageUrl.IndexOf(@"celebsweet.6te.net/") >= 0)
            {
                lThreadStart = imageDownloader.GetCelebSweet;
            }
            else if (imageUrl.IndexOf("ayhja.com/") >= 0)
            {
                return;
            }
            else
            {
                lThreadStart = imageDownloader.GetImage; // hotlinked.
            }

            ThreadManager lTdm = ThreadManager.GetInstance();
            lTdm.LaunchThread(imageUrl, lThreadStart);
        }

        /// <summary>
        /// Erases the event table.
        /// </summary>
        public void EraseEventTable()
        {
            this.mEventTable.Clear();
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the object</returns>
        public CacheObject GetObject(string url)
        {
            if (this.mEventTable.ContainsKey(url))
            {
                return (CacheObject)this.mEventTable[url];
            }

            return null;
        }

        #endregion
    }
}