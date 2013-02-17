// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageDownloader.cs" company="The Watcher">
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
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Threading;
    using PGRipper.ImageHosts;
    using PGRipper.Objects;

    /// <summary>
    /// Image Downloader Class
    /// </summary>
    public class ImageDownloader
    {
        private string mstrURL = string.Empty;

        private string ThumbImageURL = string.Empty;

        private Hashtable eventTable;

        private string mSavePath = string.Empty;

        private ServiceTemplate xService;

        private string sImageName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDownloader" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="url">The URL.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImageDownloader(string savePath, string url, string thumbUrl, string imageName, ref Hashtable hashTable)
        {
            this.mstrURL = url;
            this.ThumbImageURL = thumbUrl;
            this.eventTable = hashTable;
            this.mSavePath = savePath;
            this.sImageName = imageName;
        }

        /// <summary>
        /// Generals the downloader.
        /// </summary>
        public void GeneralDownloader()
        {
            this.xService = new uploadimages_net(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the upload image.
        /// </summary>
        public void GetUploadImage()
        {
            this.xService = new uploadimages_net(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the fapomatic.
        /// </summary>
        public void GetFapomatic()
        {
            this.xService = new fapomatic(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image venue.
        /// </summary>
        public void GetImageVenue()
        {
            this.xService = new imagevenue(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image venue new.
        /// </summary>
        public void GetImageVenueNew()
        {
            this.xService = new imagevenueNew(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the moast.
        /// </summary>
        public void GetMoast()
        {
            this.xService = new Moast(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetwatermarkIt()
        {
            this.xService = new watermarkIt(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicBux()
        {
            this.xService = new PicBux(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicturesUpload()
        {
            this.xService = new PicturesUpload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHigh()
        {
            this.xService = new ImageHigh(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImage2Share()
        {
            this.xService = new Image2Share(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPaintedOver()
        {
            this.xService = new PaintedOver(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetDumbARump()
        {
            this.xService = new DumbARump(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageCrack()
        {
            this.xService = new ImageCrack(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTenPix()
        {
            this.xService = new TenPix(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSupload()
        {
            this.xService = new Supload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageThrust()
        {
            this.xService = new ImageThrust(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareAPic()
        {
            this.xService = new ShareAPic(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFileDen()
        {
            this.xService = new FileDen(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicTiger()
        {
            this.xService = new PicTiger(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicTiger2()
        {
            this.xService = new PicTiger2(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMyPhotos()
        {
            this.xService = new MyPhotos(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTheImageHosting()
        {
            this.xService = new TheImageHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetZShare()
        {
            this.xService = new ZShare(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetKeepMyFile()
        {
            this.xService = new KeepMyFile(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageBeaver()
        {
            this.xService = new ImageBeaver(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareAvenue()
        {
            this.xService = new ShareAvenue(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetGlowFoto()
        {
            this.xService = new GlowFoto(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetJpgHosting()
        {
            this.xService = new JpgHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetJpgHosting2()
        {
            this.xService = new JpgHosting2(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFling()
        {
            this.xService = new ImageFling(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetYourPix()
        {
            this.xService = new YourPix(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreeImageHost()
        {
            this.xService = new FreeImageHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreeShare()
        {
            this.xService = new FreeShare(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSuprFile()
        {
            this.xService = new SuprFile(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetLetMeHost()
        {
            this.xService = new LetMeHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFileHost()
        {
            this.xService = new FileHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTheFreeImageHosting()
        {
            this.xService = new TheFreeImageHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetYesAlbum()
        {
            this.xService = new YesAlbum(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicsPlace()
        {
            this.xService = new PicsPlace(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetXsHosting()
        {
            this.xService = new XsHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetCelebs()
        {
            this.xService = new Celebs(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetRipHq()
        {
            this.xService = new RipHq(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBenuri()
        {
            this.xService = new Benuri(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHaven()
        {
            this.xService = new ImageHaven(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImagePundit()
        {
            this.xService = new ImagePundit(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUploadEm()
        {
            this.xService = new UploadEm(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUpPix()
        {
            this.xService = new UpPix(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixHosting()
        {
            this.xService = new PixHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPussyUpload()
        {
            this.xService = new PussyUpload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetHotLinkImage()
        {
            this.xService = new HotLinkImage(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageBam()
        {
            this.xService = new ImageBam(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHosting()
        {
            this.xService = new ImageHosting(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFap()
        {
            this.xService = new ImageFap(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAllYouCanUpload()
        {
            this.xService = new AllYouCanUpload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetLargeImageHost()
        {
            this.xService = new LargeImageHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetRadikal()
        {
            this.xService = new Radikal(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixUp()
        {
            this.xService = new PixUp(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreePornDumpster()
        {
            this.xService = new FreePornDumpster(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageSocket()
        {
            this.xService = new ImageSocket(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetStormFactory()
        {
            this.xService = new StormFactory(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicHoarder()
        {
            this.xService = new PicHoarder(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMultiPics()
        {
            this.xService = new MultiPics(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetForumAttachment()
        {
            this.xService = new ForumAttachment(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFoco()
        {
            this.xService = new ImageFoco(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSpeedImg()
        {
            this.xService = new SpeedImg(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetDollarLink()
        {
            this.xService = new DollarLink(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicEasy()
        {
            this.xService = new PicEasy(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicturesHoster()
        {
            this.xService = new PicturesHoster(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicJackal()
        {
            this.xService = new PicJackal(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAmazingDickSSl()
        {
            this.xService = new AmazingDickSSl(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImagesGal()
        {
            this.xService = new ImagesGal(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBigPics()
        {
            this.xService = new BigPics(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetXPhotoSharing()
        {
            this.xService = new XPhotoSharing(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBusyUpload()
        {
            this.xService = new BusyUpload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUpMyPhoto()
        {
            this.xService = new UpMyPhoto(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicsLibraries()
        {
            this.xService = new PicsLibraries(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTurboImageHost()
        {
            this.xService = new TurboImageHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAbload()
        {
            this.xService = new Abload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageDoza()
        {
            this.xService = new ImageDoza(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageWam()
        {
            this.xService = new ImageWam(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFlea()
        {
            this.xService = new ImageFlea(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageCargo()
        {
            this.xService = new ImageCargo(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixSlam()
        {
            this.xService = new PixSlam(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHost()
        {
            this.xService = new ImageHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMyImageHost()
        {
            this.xService = new MyImageHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareNxs()
        {
            this.xService = new ShareNxs(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetKemiPic()
        {
            this.xService = new KemiPic(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFotoTube()
        {
            this.xService = new FotoTube(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImmage()
        {
            this.xService = new Immage(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetIpicture()
        {
            this.xService = new Ipicture(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPornImgHost()
        {
            this.xService = new PornImgHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageTwist()
        {
            this.xService = new ImageTwist(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageWaste()
        {
            this.xService = new ImageWaste(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the pix host.
        /// </summary>
        public void GetPixHost()
        {
            this.xService = new PixHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the fast pic.
        /// </summary>
        public void GetFastPic()
        {
            this.xService = new FastPic(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the pic dir.
        /// </summary>
        public void GetPicDir()
        {
            this.xService = new PicDir(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the foto sik.
        /// </summary>
        public void GetFotoSik()
        {
            this.xService = new FotoSik(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the daily poa.
        /// </summary>
        public void GetDailyPoa()
        {
            this.xService = new DailyPoa(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image host li.
        /// </summary>
        public void GetImageHostLi()
        {
            this.xService = new ImageHostLi(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the stooorage.
        /// </summary>
        public void GetStooorage()
        {
            this.xService = new Stooorage(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image porter.
        /// </summary>
        public void GetImagePorter()
        {
            this.xService = new ImagePorter(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the file mad.
        /// </summary>
        public void GetFileMad()
        {
            this.xService = new FileMad(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets my pix host.
        /// </summary>
        public void GetMyPixHost()
        {
            this.xService = new MyPixHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the seven bucket.
        /// </summary>
        public void GetSevenBucket()
        {
            this.xService = new SevenBucket(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image hyper.
        /// </summary>
        public void GetImageHyper()
        {
            this.xService = new ImageHyper(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageGiga Download
        /// </summary>
        public void GetImageGiga()
        {
            this.xService = new ImageGiga(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageSwitch Download
        /// </summary>
        public void GetImageSwitch()
        {
            this.xService = new ImageSwitch(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageUpper Download
        /// </summary>
        public void GetImageUpper()
        {
            this.xService = new ImageUpper(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgChili Download
        /// </summary>
        public void GetImgChili()
        {
            this.xService = new ImgChili(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgDepot Download
        /// </summary>
        public void GetImgDepot()
        {
            this.xService = new ImgDepot(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImagePad Download
        /// </summary>
        public void GetImagePad()
        {
            this.xService = new ImagePad(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageBunk Download
        /// </summary>
        public void GetImageBunk()
        {
            this.xService = new ImageBunk(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PimpAndHost Download
        /// </summary>
        public void GetPimpAndHost()
        {
            this.xService = new PimpAndHost(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get DumpPix Download
        /// </summary>
        public void GetDumpPix()
        {
            this.xService = new DumpPix(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get Hoooster Download
        /// </summary>
        public void GetHoooster()
        {
            this.xService = new Hoooster(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PixHub Download
        /// </summary>
        public void GetPixHub()
        {
            this.xService = new PixHub(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PixRoute Download
        /// </summary>
        public void GetPixRoute()
        {
            this.xService = new PixRoute(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImagePicasa Download
        /// </summary>
        public void GetImagePicasa()
        {
            this.xService = new ImagePicasa(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get DirectUpload Download
        /// </summary>
        public void GetDirectUpload()
        {
            this.xService = new DirectUpload(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgBox Download
        /// </summary>
        public void GetImgBox()
        {
            this.xService = new ImgBox(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgDino Download
        /// </summary>
        public void GetImgDino()
        {
            this.xService = new ImgDino(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgWoot Download
        /// </summary>
        public void GetImgWoot()
        {
            this.xService = new ImgWoot(ref this.mSavePath, ref this.mstrURL, ref this.ThumbImageURL, ref this.sImageName, ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Hot linked image fetcher...
        /// </summary>
        public void GetImage()
        {
            string strImgURL = this.mstrURL;

            if (this.eventTable.ContainsKey(strImgURL))
            {
                if (((CacheObject)this.eventTable[strImgURL]).IsDownloaded)
                {
                    return;
                }
            }

            string strFilePath = strImgURL.Substring(strImgURL.LastIndexOf("/") + 1);

            try
            {
                if (!Directory.Exists(this.mSavePath))
                {
                    Directory.CreateDirectory(this.mSavePath);
                }
            }
            catch (IOException ex)
            {
                MainForm.DeleteMessage = ex.Message;
                MainForm.Delete = true;

                return;
            }

            strFilePath = Path.Combine(this.mSavePath, Utility.RemoveIllegalCharecters(strFilePath));

            CacheObject ccObj = new CacheObject { IsDownloaded = false, FilePath = strFilePath, Url = strImgURL };

            try
            {
                this.eventTable.Add(strImgURL, ccObj);
            }
            catch (ThreadAbortException)
            {
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
                return;
            }
            catch (System.Exception)
            {
                if (this.eventTable.ContainsKey(strImgURL))
                {
                    ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
                    return;
                }

                this.eventTable.Add(strImgURL, ccObj);
            }


            //////////////////////////////////////////////////////////////////////////

            try
            {
                var lHttpWebRequest = (HttpWebRequest)WebRequest.Create(strImgURL);

                lHttpWebRequest.UserAgent =
                    "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6";
                lHttpWebRequest.Headers.Add("Accept-Language: en-us,en;q=0.5");
                lHttpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
                lHttpWebRequest.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                lHttpWebRequest.Accept = "image/png,*/*;q=0.5";
                ////lHttpWebRequest.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
                lHttpWebRequest.KeepAlive = true;
                lHttpWebRequest.Referer = strImgURL.IndexOf("www.ripnetwork.net:82") >= 0
                                              ? string.Format(
                                                  "{0}showthread.php", CacheController.Xform.userSettings.CurrentForumUrl)
                                              : strImgURL;

                var lHttpWebResponse = (HttpWebResponse)lHttpWebRequest.GetResponse();
                var lHttpWebResponseStream = lHttpWebRequest.GetResponse().GetResponseStream();

                if (lHttpWebResponse.ContentType.IndexOf("image") < 0)
                {
                    lHttpWebResponse.Close();
                    lHttpWebResponseStream.Close();

                    ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                    ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
                    return;
                }

                string sNewAlteredPath = Utility.GetSuitableName(strFilePath);

                if (strFilePath != sNewAlteredPath)
                {
                    strFilePath = sNewAlteredPath;
                    ((CacheObject)this.eventTable[this.mstrURL]).FilePath = strFilePath;
                }

                lHttpWebResponse.Close();
                lHttpWebResponseStream.Close();

                var client = new WebClient();
                client.Headers.Add("Accept-Language: en-us,en;q=0.5");
                client.Headers.Add("Accept-Encoding: gzip,deflate");
                client.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                ////client.Credentials = new NetworkCredential(Utility.Username, Utility.Password);
                client.Headers.Add(
                    strImgURL.IndexOf("www.ripnetwork.net:82") >= 0
                        ? string.Format("Referer: {0}showthread.php", CacheController.Xform.userSettings.CurrentForumUrl)
                        : string.Format("Referer: {0}", strImgURL));
                client.Headers.Add(
                    "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.7.10) Gecko/20050716 Firefox/1.0.6");
                client.DownloadFile(strImgURL, strFilePath);
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
                ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
                return;
            }
            catch (System.Exception)
            {
                ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = false;
                ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
                return;
            }

            ((CacheObject)this.eventTable[strImgURL]).IsDownloaded = true;
            ThreadManager.GetInstance().RemoveThreadbyId(this.mstrURL);
            CacheController.GetInstance().LastPic =
                ((CacheObject)this.eventTable[this.mstrURL]).FilePath = strFilePath;
            return;
        }
    }
}