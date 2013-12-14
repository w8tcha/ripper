// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Downloader.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Services
{
    #region

    using System.Threading;

    using Ripper.Core.Components;

    #endregion

    /// <summary>
    ///  Download Class 
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// Generic method for downloading an image that calls appropriate site-specific downloader
        /// in a newly created thread.
        /// </summary>
        /// <param name="imageUrl">The Image Url.</param>
        /// <param name="thumbImageUrl">The thumb image URL.</param>
        /// <param name="localPath">The Local Path.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        public static void DownloadImage(string imageUrl, string thumbImageUrl, string localPath, string imageName, int imageNumber)
        {
            // TODO : Check which image host needs that check
            ////imageUrl = imageUrl.ToLower();

            if (!imageUrl.Contains("imgserve.net") && !imageUrl.Contains("imagesion.com")
                && !imageUrl.Contains("picturesion.com") && !imageUrl.Contains("picsious.com")
                && !imageUrl.Contains("imagetwist.com") && !imageUrl.Contains("premiumpics.net")
                && !imageUrl.Contains("imgmaster.net") && !imageUrl.Contains("perverzia.com"))
            {
                thumbImageUrl = thumbImageUrl.ToLower();
            }

            ThreadStart threadStart;

            var imageDownloader = new ImageDownloader(
                localPath,
                imageUrl,
                thumbImageUrl,
                imageName,
                imageNumber,
                CacheController.Instance().EventTable);

            if (imageUrl.Contains(@"/img.php?loc=loc"))
            {
                threadStart = imageDownloader.GetImageVenue;
            }
            else if (imageUrl.Contains(@"imagevenue.com/img.php?"))
            {
                threadStart = imageDownloader.GetImageVenueNew;
            }
            else if (imageUrl.IndexOf("fapomatic.com/") >= 0)
            {
                threadStart = imageDownloader.GetFapomatic;
            }
            else if (imageUrl.IndexOf("moast.com/html/") >= 0)
            {
                threadStart = imageDownloader.GetMoast;
            }
            else if (imageUrl.IndexOf("watermark-it.com/view/") >= 0)
            {
                threadStart = imageDownloader.GetwatermarkIt;
            }
            else if (imageUrl.IndexOf("picbux.com/image.php?id=") >= 0)
            {
                threadStart = imageDownloader.GetPicBux;
            }
            else if (imageUrl.IndexOf("picturesupload.com/show.php/") >= 0)
            {
                threadStart = imageDownloader.GetPicturesUpload;
            }
            else if (imageUrl.IndexOf("imagehigh.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageHigh;
            }
            else if (imageUrl.IndexOf("image2share.com/") >= 0)
            {
                threadStart = imageDownloader.GetImage2Share;
            }
            else if (imageUrl.IndexOf("paintedover.com/uploads/show.php") >= 0)
            {
                threadStart = imageDownloader.GetPaintedOver;
            }
            else if (imageUrl.IndexOf("dumparump.com/") >= 0)
            {
                threadStart = imageDownloader.GetDumbARump;
            }
            else if (imageUrl.IndexOf("imagecrack.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageCrack;
            }
            else if (imageUrl.IndexOf("10pix.com/") >= 0)
            {
                threadStart = imageDownloader.GetTenPix;
            }
            else if (imageUrl.IndexOf("supload.com/") >= 0)
            {
                threadStart = imageDownloader.GetSupload;
            }
            else if (imageUrl.IndexOf("imagethrust.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageThrust;
            }
            else if (imageUrl.IndexOf("shareapic.net/") >= 0)
            {
                threadStart = imageDownloader.GetShareAPic;
            }
            else if (imageUrl.IndexOf("fileden.com/pview.php") >= 0)
            {
                threadStart = imageDownloader.GetFileDen;
            }
            else if (imageUrl.IndexOf("server5.pictiger.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicTiger;
            }
            else if (imageUrl.IndexOf(@"pictiger.com/albums/") >= 0 || imageUrl.IndexOf(@"pictiger.com/images/") >= 0)
            {
                threadStart = imageDownloader.GetPicTiger2;
            }
            else if (imageUrl.IndexOf("celebs.myphotos.cc/") >= 0)
            {
                threadStart = imageDownloader.GetMyPhotos;
            }
            else if (imageUrl.IndexOf(@"theimagehosting.com/image.php?") >= 0)
            {
                threadStart = imageDownloader.GetTheImageHosting;
            }
            else if (imageUrl.IndexOf("zshare.net/") >= 0)
            {
                threadStart = imageDownloader.GetZShare;
            }
            else if (imageUrl.IndexOf("keepmyfile.com/") >= 0)
            {
                threadStart = imageDownloader.GetKeepMyFile;
            }
            else if (imageUrl.IndexOf("imagebeaver.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageBeaver;
            }
            else if (imageUrl.IndexOf("shareavenue.com/") >= 0)
            {
                threadStart = imageDownloader.GetShareAvenue;
            }
            else if (imageUrl.IndexOf("glowfoto.com/") >= 0)
            {
                threadStart = imageDownloader.GetGlowFoto;
            }
            else if (imageUrl.IndexOf("jpghosting.com/images/") >= 0)
            {
                threadStart = imageDownloader.GetJpgHosting;
            }
            else if (imageUrl.IndexOf("jpghosting.com/showpic.php") >= 0)
            {
                threadStart = imageDownloader.GetJpgHosting2;
            }
            else if (imageUrl.IndexOf("imagefling.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageFling;
            }
            else if (imageUrl.IndexOf("yourpix.org/view/") >= 0)
            {
                threadStart = imageDownloader.GetYourPix;
            }
            else if (imageUrl.IndexOf("freeimagehost.eu/") >= 0)
            {
                threadStart = imageDownloader.GetFreeImageHost;
            }
            else if (imageUrl.IndexOf("freeshare.us/") >= 0)
            {
                threadStart = imageDownloader.GetFreeShare;
            }
            else if (imageUrl.IndexOf("suprfile.com/") >= 0)
            {
                threadStart = imageDownloader.GetSuprFile;
            }
            else if (imageUrl.IndexOf("letmehost.com/") >= 0)
            {
                threadStart = imageDownloader.GetLetMeHost;
            }
            else if (imageUrl.IndexOf("filehost.to/") >= 0)
            {
                threadStart = imageDownloader.GetFileHost;
            }
            else if (imageUrl.IndexOf("thefreeimagehosting.com/") >= 0)
            {
                threadStart = imageDownloader.GetTheFreeImageHosting;
            }
            else if (imageUrl.IndexOf("yesalbum.com/") >= 0)
            {
                threadStart = imageDownloader.GetYesAlbum;
            }
            else if (imageUrl.IndexOf("picsplace.to") >= 0)
            {
                threadStart = imageDownloader.GetPicsPlace;
            }
            else if (imageUrl.IndexOf("xs.to/") >= 0)
            {
                threadStart = imageDownloader.GetXsHosting;
            }
            else if (imageUrl.IndexOf("celebs.sytes.net/") >= 0)
            {
                threadStart = imageDownloader.GetCelebs;
            }
            else if (imageUrl.IndexOf("rip-hq.com/") >= 0)
            {
                threadStart = imageDownloader.GetRipHq;
            }
            else if (imageUrl.IndexOf("benuri.com/") >= 0)
            {
                threadStart = imageDownloader.GetBenuri;
            }
            else if (imageUrl.IndexOf(@"imagehaven.net/img.php?id=") >= 0)
            {
                threadStart = imageDownloader.GetImageHaven;
            }
            else if (imageUrl.IndexOf("imagepundit.com/") >= 0)
            {
                threadStart = imageDownloader.GetImagePundit;
            }
            else if (imageUrl.IndexOf(@"uploadem.com/") >= 0)
            {
                threadStart = imageDownloader.GetUploadEm;
            }
            else if (imageUrl.IndexOf("uppix.info/") >= 0)
            {
                threadStart = imageDownloader.GetUpPix;
            }
            else if (imageUrl.IndexOf("pixhosting.info/") >= 0)
            {
                threadStart = imageDownloader.GetPixHosting;
            }
            else if (imageUrl.IndexOf("pussyupload.com/") >= 0)
            {
                threadStart = imageDownloader.GetPussyUpload;
            }
            else if (imageUrl.IndexOf(@"hotlinkimage.com/img.php?") >= 0)
            {
                threadStart = imageDownloader.GetHotLinkImage;
            }
            else if (imageUrl.IndexOf(@"imagebam.com/image/") >= 0)
            {
                threadStart = imageDownloader.GetImageBam;
            }
            else if (imageUrl.IndexOf(@"imagehosting.gr/") >= 0)
            {
                threadStart = imageDownloader.GetImageHosting;
            }
            else if (imageUrl.IndexOf(@"imagefap.com/image.php") >= 0)
            {
                threadStart = imageDownloader.GetImageFap;
            }
            else if (imageUrl.IndexOf(@"allyoucanupload.webshots.com/v/") >= 0)
            {
                threadStart = imageDownloader.GetAllYouCanUpload;
            }
            else if (imageUrl.IndexOf(@"largeimagehost.com/img/") >= 0)
            {
                threadStart = imageDownloader.GetLargeImageHost;
            }
            else if (imageUrl.IndexOf(@"radikal.ru/") >= 0)
            {
                threadStart = imageDownloader.GetRadikal;
            }
            else if (imageUrl.IndexOf(@"pixup.info/") >= 0)
            {
                threadStart = imageDownloader.GetPixUp;
            }
            else if (imageUrl.IndexOf(@"freeporndumpster.com/") >= 0)
            {
                threadStart = imageDownloader.GetFreePornDumpster;
            }
            else if (imageUrl.IndexOf(@"imagesocket.com/view/") >= 0)
            {
                threadStart = imageDownloader.GetImageSocket;
            }
            else if (imageUrl.IndexOf(@"storm-factory.org/") >= 0)
            {
                threadStart = imageDownloader.GetStormFactory;
            }
            else if (imageUrl.IndexOf(@"pichoarder.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicHoarder;
            }
            else if (imageUrl.IndexOf(@"multipics.net/") >= 0)
            {
                threadStart = imageDownloader.GetMultiPics;
            }
            else if (imageUrl.IndexOf(@".imagefoco.com/") >= 0 || imageUrl.IndexOf(@".cocopic.com/") >= 0 ||
                     imageUrl.IndexOf(@".cocoimage.com/") >= 0 || imageUrl.IndexOf(@".picfoco.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageFoco;
            }
            else if (imageUrl.IndexOf(@".speedimg.com/") >= 0)
            {
                threadStart = imageDownloader.GetSpeedImg;
            }
            else if (imageUrl.IndexOf(@".dollarlink.biz/") >= 0)
            {
                threadStart = imageDownloader.GetDollarLink;
            }
            else if (imageUrl.IndexOf(@"picseasy.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicEasy;
            }
            else if (imageUrl.IndexOf(@"pictureshoster.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicturesHoster;
            }
            else if (imageUrl.IndexOf(@".picjackal.com/") >= 0 || imageUrl.IndexOf(@".picsharebunny.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicJackal;
            }
            else if (imageUrl.IndexOf(@"amazingdicksuckingsluts.com/") >= 0)
            {
                threadStart = imageDownloader.GetAmazingDickSSl;
            }
            else if (imageUrl.IndexOf(@"imagesgal.com/") >= 0)
            {
                threadStart = imageDownloader.GetImagesGal;
            }
            else if (imageUrl.IndexOf(@"bigpics.ru/") >= 0)
            {
                threadStart = imageDownloader.GetBigPics;
            }
            else if (imageUrl.IndexOf(@"xxxphotosharing.com/") >= 0 || imageUrl.IndexOf(@"myadultimage.com/") >= 0)
            {
                threadStart = imageDownloader.GetXPhotoSharing;
            }
            else if (imageUrl.IndexOf(@"busyupload.com/") >= 0)
            {
                threadStart = imageDownloader.GetBusyUpload;
            }
            else if (imageUrl.IndexOf(@"upmyphoto.com/") >= 0)
            {
                threadStart = imageDownloader.GetUpMyPhoto;
            }
            else if (imageUrl.IndexOf(@"turboimagehost.com/") >= 0)
            {
                threadStart = imageDownloader.GetTurboImageHost;
            }
            else if (imageUrl.IndexOf(@"abload.de/") >= 0)
            {
                threadStart = imageDownloader.GetAbload;
            }
            else if (imageUrl.IndexOf(@"imagedoza.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageDoza;
            }
            else if (imageUrl.IndexOf(@"imagewam.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageWam;
            }
            else if (imageUrl.IndexOf(@"imageflea.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageFlea;
            }
            else if (imageUrl.IndexOf(@"imagecargo.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageCargo;
            }
            else if (imageUrl.IndexOf(@"pixslam.com/") >= 0)
            {
                threadStart = imageDownloader.GetPixSlam;
            }
            else if (imageUrl.IndexOf(@"imagehost.org/") >= 0)
            {
                threadStart = imageDownloader.GetImageHost;
            }
            else if (imageUrl.IndexOf(@"my-image-host.com/") >= 0)
            {
                threadStart = imageDownloader.GetMyImageHost;
            }
            else if (imageUrl.IndexOf(@"sharenxs.com/") >= 0)
            {
                threadStart = imageDownloader.GetShareNxs;
            }
            else if (imageUrl.IndexOf(@"kemipic.com/") >= 0)
            {
                threadStart = imageDownloader.GetKemiPic;
            }
            else if (imageUrl.IndexOf(@"fototube.pl/") >= 0)
            {
                threadStart = imageDownloader.GetFotoTube;
            }
            else if (imageUrl.IndexOf(@"immage.de/") >= 0)
            {
                threadStart = imageDownloader.GetImmage;
            }
            else if (imageUrl.IndexOf(@"ipicture.ru/") >= 0)
            {
                threadStart = imageDownloader.GetIpicture;
            }
            else if (imageUrl.IndexOf(@"pornimghost.com/") >= 0)
            {
                threadStart = imageDownloader.GetPornImgHost;
            }
            else if (imageUrl.IndexOf(@"imagetwist.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageTwist;
            }
            else if (imageUrl.IndexOf(@"imagewaste.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageWaste;
            }
            else if (imageUrl.IndexOf(@"pixhost.org/") >= 0)
            {
                threadStart = imageDownloader.GetPixHost;
            }
            else if (imageUrl.IndexOf(@"fastpic.ru/") >= 0)
            {
                threadStart = imageDownloader.GetFastPic;
            }
            else if (imageUrl.IndexOf(@"picdir.de/") >= 0)
            {
                threadStart = imageDownloader.GetPicDir;
            }
            else if (imageUrl.IndexOf(@"fotosik.pl/") >= 0)
            {
                threadStart = imageDownloader.GetFotoSik;
            }
            else if (imageUrl.IndexOf(@"dailypoa.com/") >= 0)
            {
                threadStart = imageDownloader.GetDailyPoa;
            }
            else if (imageUrl.IndexOf(@"imagehost.li/") >= 0)
            {
                threadStart = imageDownloader.GetImageHostLi;
            }
            else if (imageUrl.IndexOf(@"stooorage.com/") >= 0)
            {
                threadStart = imageDownloader.GetStooorage;
            }
            else if (imageUrl.IndexOf(@"imageporter.com/") >= 0)
            {
                threadStart = imageDownloader.GetImagePorter;
            }
            else if (imageUrl.IndexOf(@"filemad.com/") >= 0)
            {
                threadStart = imageDownloader.GetFileMad;
            }
            else if (imageUrl.IndexOf(@"mypixhost.com/") >= 0)
            {
                threadStart = imageDownloader.GetMyPixHost;
            }
            else if (imageUrl.IndexOf(@"7bucket.com/") >= 0)
            {
                threadStart = imageDownloader.GetSevenBucket;
            }
            else if (imageUrl.IndexOf(@"imagehyper.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageHyper;
            }
            else if (imageUrl.IndexOf(@"imgiga.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageGiga;
            }
            else if (imageUrl.IndexOf(@"imageswitch.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageSwitch;
            }
            else if (imageUrl.IndexOf(@"imageupper.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageUpper;
            }
            else if (imageUrl.IndexOf(@"imgchili.") >= 0)
            {
                threadStart = imageDownloader.GetImgChili;
            }
            else if (imageUrl.IndexOf(@"imgdepot.org/") >= 0)
            {
                threadStart = imageDownloader.GetImgDepot;
            }
            else if (imageUrl.IndexOf(@"imagepad.us/") >= 0)
            {
                threadStart = imageDownloader.GetImagePad;
            }
            else if (imageUrl.IndexOf(@"imagebunk.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageBunk;
            }
            else if (imageUrl.IndexOf(@"pimpandhost.com/") >= 0)
            {
                threadStart = imageDownloader.GetPimpAndHost;
            }
            else if (imageUrl.IndexOf(@"dumppix.com/") >= 0)
            {
                threadStart = imageDownloader.GetDumpPix;
            }
            else if (imageUrl.IndexOf(@"hoooster.com/") >= 0)
            {
                threadStart = imageDownloader.GetHoooster;
            }
            else if (imageUrl.IndexOf(@"pixhub.eu/") >= 0)
            {
                threadStart = imageDownloader.GetPixHub;
            }
            else if (imageUrl.IndexOf(@"pixroute.com/") >= 0)
            {
                threadStart = imageDownloader.GetPixRoute;
            }
            else if (imageUrl.IndexOf(@"imagepicsa.com/") >= 0)
            {
                threadStart = imageDownloader.GetImagePicasa;
            }
            else if (imageUrl.IndexOf(@"directupload.net/") >= 0)
            {
                threadStart = imageDownloader.GetDirectUpload;
            }
            else if (imageUrl.IndexOf(@"imgbox.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgBox;
            }
            else if (imageUrl.IndexOf(@"imgdino.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgDino;
            }
            else if (imageUrl.IndexOf(@"imgtiger.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgDino;
            }
            else if (imageUrl.IndexOf(@"imgwoot.com/") >= 0 || imageUrl.IndexOf(@"Imgwoot.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgmoney.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgmoney.net/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgproof.net/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"pixup.us/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgonion.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgcloud.co/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgirl.info/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"gatasexycity.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"hosterbin.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"picslite.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imageteam.org/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgnext.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"hosturimage.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"3xvintage.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgsavvy.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imggoo.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imgmaster.net/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"gogoimage.org/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"jovoimage.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;
            }
            else if (imageUrl.IndexOf(@"imagefolks.com/") >= 0 || imageUrl.IndexOf(@"ImageFolks.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgWoot;

                ////
            }
            else if (imageUrl.IndexOf(@"imgpo.st/") >= 0)
            {
                threadStart = imageDownloader.GetImgPo;
            }
            else if (imageUrl.IndexOf(@"imgah.com/") >= 0)
            {
                threadStart = imageDownloader.GetImGah;
            }
            else if (imageUrl.IndexOf(@"imgur.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgUr;
            }
            else if (imageUrl.IndexOf(@"tuspics.net/") >= 0)
            {
                threadStart = imageDownloader.GetTusPics;
            }
            else if (imageUrl.IndexOf(@"freeimagepic.com/") >= 0)
            {
                threadStart = imageDownloader.GetFreeImagePic;
            }
            else if (imageUrl.IndexOf(@"imgserve.net/") >= 0)
            {
                threadStart = imageDownloader.GetImgServe;
            }
            else if (imageUrl.IndexOf(@"celebsweet.6te.net/") >= 0)
            {
                threadStart = imageDownloader.GetCelebSweet;
            }
            else if (imageUrl.IndexOf(@"sexyimg.com/") >= 0)
            {
                threadStart = imageDownloader.GetSexyImg;
            }
            else if (imageUrl.IndexOf(@"imagejumbo.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageJumbo;
            }
            else if (imageUrl.IndexOf(@"imagedax.net/") >= 0)
            {
                threadStart = imageDownloader.GetImageDax;
            }
            else if (imageUrl.IndexOf(@"hosterbin.com/") >= 0)
            {
                threadStart = imageDownloader.GetHosterBin;
            }
            else if (imageUrl.IndexOf(@"imgbabes.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgBabes;
            }
            else if (imageUrl.IndexOf(@"imagesion.com/") >= 0)
            {
                threadStart = imageDownloader.GetImagesIon;
            }
            else if (imageUrl.IndexOf(@"picturedip.com/") >= 0)
            {
                threadStart = imageDownloader.GetPictureDip;
            }
            else if (imageUrl.IndexOf(@"imagezilla.net/") >= 0)
            {
                threadStart = imageDownloader.GetImageZilla;
            }
            else if (imageUrl.IndexOf(@"picturesion.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicturesIon;
            }
            else if (imageUrl.IndexOf(@"picsious.com/") >= 0)
            {
                threadStart = imageDownloader.GetPicturesIon;
            }
            else if (imageUrl.IndexOf(@"imagehosthq.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageHostHq;
            }
            else if (imageUrl.IndexOf(@"pixtreat.com/") >= 0)
            {
                threadStart = imageDownloader.GetPixTreat;
            }
            else if (imageUrl.IndexOf(@"premiumpics.net/") >= 0)
            {
                threadStart = imageDownloader.GetPremiumPics;
            }
            else if (imageUrl.IndexOf(@"imgdollar.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgDollar;
            }
            else if (imageUrl.IndexOf(@"imgflare.com/") >= 0)
            {
                threadStart = imageDownloader.GetImgFlare;
            }
            else if (imageUrl.IndexOf(@"xlocker.net/") >= 0)
            {
                threadStart = imageDownloader.GetXLocker;
            }
            else if (imageUrl.IndexOf(@"imagedunk.com/") >= 0)
            {
                threadStart = imageDownloader.GetImageDunk;
            }
            else if (imageUrl.IndexOf(@"perverzia.com/") >= 0)
            {
                threadStart = imageDownloader.GetPerverzia;
            }
            else if (imageUrl.IndexOf("ayhja.com/") >= 0)
            {
                return;
            }
            else
            {
                threadStart = imageDownloader.GetImageHotLinked;
            }

            var threadManager = ThreadManager.GetInstance();
            threadManager.LaunchThread(imageUrl, threadStart);
        }
    }
}