// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageDownloader.cs" company="The Watcher">
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
    using System.Collections;

    using Ripper.Core.Components;
    using Ripper.Services.ImageHosts;

    /// <summary>
    /// ImageDownloader is the bridging class between this routine and the
    /// ServiceTemplate base class (which is the parent to all hosting site's
    /// fetch code).
    /// </summary>
    public class ImageDownloader
    {
        /// <summary>
        /// The image URL
        /// </summary>
        private string imageURL = string.Empty;

        /// <summary>
        /// The thumb image URL
        /// </summary>
        private string thumbImageURL = string.Empty;

        /// <summary>
        /// The event table
        /// </summary>
        private Hashtable eventTable;

        /// <summary>
        /// The save path
        /// </summary>
        private string savePath = string.Empty;

        /// <summary>
        /// The x service
        /// </summary>
        private ServiceTemplate xService;

        /// <summary>
        /// The image name
        /// </summary>
        private string imageName = string.Empty;

        /// <summary>
        /// The image number
        /// </summary>
        private int imageNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDownloader" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="url">The URL.</param>
        /// <param name="thumbUrl">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImageDownloader(
            string savePath,
            string url,
            string thumbUrl,
            string imageName,
            int imageNumber,
            Hashtable hashTable)
        {
            this.imageURL = url;
            this.thumbImageURL = thumbUrl;
            this.eventTable = hashTable;
            this.savePath = savePath;
            this.imageName = imageName;
            this.imageNumber = imageNumber;
        }

        /// <summary>
        /// Generals the downloader.
        /// </summary>
        public void GeneralDownloader()
        {
            this.xService = new uploadimages_net(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the upload image.
        /// </summary>
        public void GetUploadImage()
        {
            this.xService = new uploadimages_net(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the fapomatic.
        /// </summary>
        public void GetFapomatic()
        {
            this.xService = new fapomatic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image venue.
        /// </summary>
        public void GetImageVenue()
        {
            this.xService = new imagevenue(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the image venue download.
        /// </summary>
        public void GetImageVenueNew()
        {
            this.xService = new imagevenueNew(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the moast download.
        /// </summary>
        public void GetMoast()
        {
            this.xService = new Moast(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetwatermarkIt()
        {
            this.xService = new watermarkIt(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicBux()
        {
            this.xService = new PicBux(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicturesUpload()
        {
            this.xService = new PicturesUpload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHigh()
        {
            this.xService = new ImageHigh(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImage2Share()
        {
            this.xService = new Image2Share(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPaintedOver()
        {
            this.xService = new PaintedOver(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetDumbARump()
        {
            this.xService = new DumbARump(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageCrack()
        {
            this.xService = new ImageCrack(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTenPix()
        {
            this.xService = new TenPix(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSupload()
        {
            this.xService = new Supload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageThrust()
        {
            this.xService = new ImageThrust(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareAPic()
        {
            this.xService = new ShareAPic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFileDen()
        {
            this.xService = new FileDen(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicTiger()
        {
            this.xService = new PicTiger(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicTiger2()
        {
            this.xService = new PicTiger2(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMyPhotos()
        {
            this.xService = new MyPhotos(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTheImageHosting()
        {
            this.xService = new TheImageHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetZShare()
        {
            this.xService = new ZShare(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetKeepMyFile()
        {
            this.xService = new KeepMyFile(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageBeaver()
        {
            this.xService = new ImageBeaver(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareAvenue()
        {
            this.xService = new ShareAvenue(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetGlowFoto()
        {
            this.xService = new GlowFoto(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetJpgHosting()
        {
            this.xService = new JpgHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetJpgHosting2()
        {
            this.xService = new JpgHosting2(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFling()
        {
            this.xService = new ImageFling(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetYourPix()
        {
            this.xService = new YourPix(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreeImageHost()
        {
            this.xService = new FreeImageHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreeShare()
        {
            this.xService = new FreeShare(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSuprFile()
        {
            this.xService = new SuprFile(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetLetMeHost()
        {
            this.xService = new LetMeHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFileHost()
        {
            this.xService = new FileHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTheFreeImageHosting()
        {
            this.xService = new TheFreeImageHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetYesAlbum()
        {
            this.xService = new YesAlbum(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicsPlace()
        {
            this.xService = new PicsPlace(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetXsHosting()
        {
            this.xService = new XsHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetCelebs()
        {
            this.xService = new Celebs(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetRipHq()
        {
            this.xService = new RipHq(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBenuri()
        {
            this.xService = new Benuri(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHaven()
        {
            this.xService = new ImageHaven(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImagePundit()
        {
            this.xService = new ImagePundit(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUploadEm()
        {
            this.xService = new UploadEm(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUpPix()
        {
            this.xService = new UpPix(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixHosting()
        {
            this.xService = new PixHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPussyUpload()
        {
            this.xService = new PussyUpload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetHotLinkImage()
        {
            this.xService = new HotLinkImage(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the Image Bam downloader.
        /// </summary>
        public void GetImageBam()
        {
            this.xService = new ImageBam(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        public void GetImageHosting()
        {
            this.xService = new ImageHosting(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAllYouCanUpload()
        {
            this.xService = new AllYouCanUpload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetLargeImageHost()
        {
            this.xService = new LargeImageHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetRadikal()
        {
            this.xService = new Radikal(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        public void GetPixUp()
        {
            this.xService = new PixUp(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFreePornDumpster()
        {
            this.xService = new FreePornDumpster(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageSocket()
        {
            this.xService = new ImageSocket(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetStormFactory()
        {
            this.xService = new StormFactory(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicHoarder()
        {
            this.xService = new PicHoarder(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMultiPics()
        {
            this.xService = new MultiPics(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFoco()
        {
            this.xService = new ImageFoco(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSpeedImg()
        {
            this.xService = new SpeedImg(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetDollarLink()
        {
            this.xService = new DollarLink(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicEasy()
        {
            this.xService = new PicEasy(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicturesHoster()
        {
            this.xService = new PicturesHoster(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicJackal()
        {
            this.xService = new PicJackal(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAmazingDickSSl()
        {
            this.xService = new AmazingDickSSl(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImagesGal()
        {
            this.xService = new ImagesGal(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBigPics()
        {
            this.xService = new BigPics(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetXPhotoSharing()
        {
            this.xService = new XPhotoSharing(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetBusyUpload()
        {
            this.xService = new BusyUpload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetUpMyPhoto()
        {
            this.xService = new UpMyPhoto(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetTurboImageHost()
        {
            this.xService = new TurboImageHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetAbload()
        {
            this.xService = new Abload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageDoza()
        {
            this.xService = new ImageDoza(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageWam()
        {
            this.xService = new ImageWam(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageFlea()
        {
            this.xService = new ImageFlea(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageCargo()
        {
            this.xService = new ImageCargo(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixSlam()
        {
            this.xService = new PixSlam(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHost()
        {
            this.xService = new ImageHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
            this.xService.StartDownload();
        }

        public void GetMyImageHost()
        {
            this.xService = new MyImageHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetShareNxs()
        {
            this.xService = new ShareNxs(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetKemiPic()
        {
            this.xService = new KemiPic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFotoTube()
        {
            this.xService = new FotoTube(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImmage()
        {
            this.xService = new Immage(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetIpicture()
        {
            this.xService = new Ipicture(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPornImgHost()
        {
            this.xService = new PornImgHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Gets the ImageTwist download
        /// </summary>
        public void GetImageTwist()
        {
            this.xService = new ImageTwist(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        public void GetImageWaste()
        {
            this.xService = new ImageWaste(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPixHost()
        {
            this.xService = new PixHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFastPic()
        {
            this.xService = new FastPic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetPicDir()
        {
            this.xService = new PicDir(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFotoSik()
        {
            this.xService = new FotoSik(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetDailyPoa()
        {
            this.xService = new DailyPoa(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHostLi()
        {
            this.xService = new ImageHostLi(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetStooorage()
        {
            this.xService = new Stooorage(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImagePorter()
        {
            this.xService = new ImagePorter(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetFileMad()
        {
            this.xService = new FileMad(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetMyPixHost()
        {
            this.xService = new MyPixHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetSevenBucket()
        {
            this.xService = new SevenBucket(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        public void GetImageHyper()
        {
            this.xService = new ImageHyper(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageGiga Download
        /// </summary>
        public void GetImageGiga()
        {
            this.xService = new ImageGiga(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageSwitch Download
        /// </summary>
        public void GetImageSwitch()
        {
            this.xService = new ImageSwitch(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgChili Download
        /// </summary>
        public void GetImgChili()
        {
            this.xService = new ImgChili(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgDepot Download
        /// </summary>
        public void GetImgDepot()
        {
            this.xService = new ImgDepot(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageUpper Download
        /// </summary>
        public void GetImageUpper()
        {
            this.xService = new ImageUpper(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImagePad Download
        /// </summary>
        public void GetImagePad()
        {
            this.xService = new ImagePad(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageBunk Download
        /// </summary>
        public void GetImageBunk()
        {
            this.xService = new ImageBunk(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PimpAndHost Download
        /// </summary>
        public void GetPimpAndHost()
        {
            this.xService = new PimpAndHost(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get DumpPix Download
        /// </summary>
        public void GetDumpPix()
        {
            this.xService = new DumpPix(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get Hoooster Download
        /// </summary>
        public void GetHoooster()
        {
            this.xService = new Hoooster(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PixHub Download
        /// </summary>
        public void GetPixHub()
        {
            this.xService = new PixHub(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PixRoute Download
        /// </summary>
        public void GetPixRoute()
        {
            this.xService = new PixRoute(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImagePicasa Download
        /// </summary>
        public void GetImagePicasa()
        {
            this.xService = new ImagePicasa(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get DirectUpload Download
        /// </summary>
        public void GetDirectUpload()
        {
            this.xService = new DirectUpload(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgBox Download
        /// </summary>
        public void GetImgBox()
        {
            this.xService = new ImgBox(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgDino Download
        /// </summary>
        public void GetImgDino()
        {
            this.xService = new ImgDino(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgWoot Download
        /// </summary>
        public void GetImgWoot()
        {
            this.xService = new ImgWoot(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImageEer Download
        /// </summary>
        public void GetImageEer()
        {
            this.xService = new ImageEer(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgPo Download
        /// </summary>
        public void GetImgPo()
        {
            this.xService = new ImgPo(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImGah Download
        /// </summary>
        public void GetImGah()
        {
            this.xService = new ImGah(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgUr Download
        /// </summary>
        public void GetImgUr()
        {
            this.xService = new ImgUr(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get TusPics Download
        /// </summary>
        public void GetTusPics()
        {
            this.xService = new TusPics(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get FreeImagePic Download
        /// </summary>
        public void GetFreeImagePic()
        {
            this.xService = new FreeImagePic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get Celeb Sweet Download
        /// </summary>
        public void GetCelebSweet()
        {
            this.xService = new CelebSweet(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get SexyImg Download
        /// </summary>
        public void GetSexyImg()
        {
            this.xService = new SexyImg(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageJumbo Download
        /// </summary>
        public void GetImageJumbo()
        {
            this.xService = new ImageJumbo(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageDax Download
        /// </summary>
        public void GetImageDax()
        {
            this.xService = new ImageDax(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get HosterBin Download
        /// </summary>
        public void GetHosterBin()
        {
            this.xService = new HosterBin(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImgBabes Download
        /// </summary>
        public void GetImgBabes()
        {
            this.xService = new ImgBabes(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImagesIon Download
        /// </summary>
        public void GetImagesIon()
        {
            this.xService = new ImagesIon(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get PictureDip Download
        /// </summary>
        public void GetPictureDip()
        {
            this.xService = new PictureDip(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownload();
        }

        /// <summary>
        /// Get ImageZilla Download
        /// </summary>
        public void GetImageZilla()
        {
            this.xService = new ImageZilla(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PicturesIon Download
        /// </summary>
        public void GetPicturesIon()
        {
            this.xService = new PicturesIon(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImageHostHQ Download
        /// </summary>
        public void GetImageHostHq()
        {
            this.xService = new ImageHostHq(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PixTreat Download
        /// </summary>
        public void GetPixTreat()
        {
            this.xService = new PixTreat(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PremiumPics Download
        /// </summary>
        public void GetPremiumPics()
        {
            this.xService = new PremiumPics(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgDollar Download
        /// </summary>
        public void GetImgDollar()
        {
            this.xService = new ImgDollar(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgFlare Download
        /// </summary>
        public void GetImgFlare()
        {
            this.xService = new ImgFlare(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get XLocker Download
        /// </summary>
        public void GetXLocker()
        {
            this.xService = new XLocker(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImageDunk Download
        /// </summary>
        public void GetImageDunk()
        {
            this.xService = new ImageDunk(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get Perverzia Download
        /// </summary>
        public void GetPerverzia()
        {
            this.xService = new Perverzia(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ViewCube Download
        /// </summary>
        public void GetViewCube()
        {
            this.xService = new ViewCube(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgSpice Download
        /// </summary>
        public void GetImgSpice()
        {
            this.xService = new ImgSpice(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImagesAholic Download
        /// </summary>
        public void GetImagesAholic()
        {
            this.xService = new ImagesAholic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImageShack Download
        /// </summary>
        public void GetImageShack()
        {
            this.xService = new ImageShack(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PostImage Download
        /// </summary>
        public void GetPostImage()
        {
            this.xService = new PostImage(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PostImg Download
        /// </summary>
        public void GetPostImg()
        {
            this.xService = new PostImg(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgPaying Download
        /// </summary>
        public void GetImgPaying()
        {
            this.xService = new ImgPaying(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get TruePic Download
        /// </summary>
        public void GetTruePic()
        {
            this.xService = new TruePic(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PixPal Download
        /// </summary>
        public void GetPixPal()
        {
            this.xService = new PixPal(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ViperII Download
        /// </summary>
        public void GetViperII()
        {
            this.xService = new ViperII(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgSee Download
        /// </summary>
        public void GetImgSee()
        {
            this.xService = new ImgSee(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgMega Download
        /// </summary>
        public void GetImgMega()
        {
            this.xService = new ImgMega(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgClick Download
        /// </summary>
        public void GetImgClick()
        {
            this.xService = new ImgClick(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get Imaaage Download
        /// </summary>
        public void GetImaaage()
        {
            this.xService = new Imaaage(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImageBugs Download
        /// </summary>
        public void GetImageBugs()
        {
            this.xService = new ImageBugs(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get Pictomania Download
        /// </summary>
        public void GetPictomania()
        {
            this.xService = new Pictomania(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
        
        /// <summary>
        /// Get ImgDap Download
        /// </summary>
        public void GetImgDap()
        {
            this.xService = new ImgDap(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get FileSpit Download
        /// </summary>
        public void GetFileSpit()
        {
            this.xService = new FileSpit(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get ImgBanana Download
        /// </summary>
        public void GetImgBanana()
        {
            this.xService = new ImgBanana(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PixLiv Download
        /// </summary>
        public void GetPixLiv()
        {
            this.xService = new PixLiv(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Get PicExposed Download
        /// </summary>
        public void GetPicExposed()
        {
            this.xService = new PicExposed(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }

        /// <summary>
        /// Hot linked image fetcher...
        /// </summary>
        public void GetImageHotLinked()
        {
            this.xService = new HotLinkedImage(
               ref this.savePath,
               ref this.imageURL,
               ref this.thumbImageURL,
               ref this.imageName,
               ref this.imageNumber,
               ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
        public void GetImgTrex()
        {
            this.xService = new ImgTrex(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
        public void GetGallerynova()
        {
            this.xService = new Gallerynova(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
        public void GetLeechimg()
        {
            this.xService = new LeechImg(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
        public void GetImgDriveCo()
        {
            this.xService = new ImgDriveCo(
                ref this.savePath,
                ref this.imageURL,
                ref this.thumbImageURL,
                ref this.imageName,
                ref this.imageNumber,
                ref this.eventTable);
            this.xService.StartDownloadAsync();
        }
    }
}