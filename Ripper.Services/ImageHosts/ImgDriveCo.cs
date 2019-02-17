// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImgDriveCo.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Services.ImageHosts
{
    #region

    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Net;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;
    using System.Collections.Specialized;
    using System.Text;

    #endregion

    /// <summary>
    /// Worker class to get images hosted on ImgDrive.co 
    /// </summary>
    /// <remarks>There is another imgdrive host at url imgdrive.net. 
    /// But they imgdrive.net has another html layout, so they
    /// aren't compatible.
    /// </remarks>
    public class ImgDriveCo : ServiceTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgDriveCo" /> class.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="imageHostURL">The image Host URL</param>
        /// <param name="thumbURL">The thumb URL.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageNumber">The image number.</param>
        /// <param name="hashTable">The hash table.</param>
        public ImgDriveCo(
            ref string savePath,
            ref string imageHostURL,
            ref string thumbURL,
            ref string imageName,
            ref int imageNumber,
            ref Hashtable hashTable)
            : base(savePath, imageHostURL, thumbURL, imageName, imageNumber, ref hashTable)
        {
        }

        /// <summary>
        /// Do the Download
        /// </summary>
        /// <returns>
        /// Returns if the Image was downloaded
        /// </returns>
        protected override bool DoDownload()
        {
            var imageURL = ImageLinkURL;
            string filePath = "";
            var page = this.GetImageHostPage(ref imageURL);
            string imageDownloadURL = "";

            var matches = Regex.Matches(page, @"<input type=""(hidden|submit)"" name=""(?<name>\w+)"" value=""(?<value>\w+)""", RegexOptions.Compiled);

            if (matches.Count > 0)
            {
                NameValueCollection values = new NameValueCollection();
                    
                foreach (Match m in matches)
                {
                    values.Add(m.Groups["name"].Value, m.Groups["value"].Value);
                }
                string html;

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    byte[] result = client.UploadValues(imageURL, "POST", values);
                    html = Encoding.UTF8.GetString(result);
                }
                var match = Regex.Match(html,
                        @"src=""(?<url>[^\""""]+)"" class=""pic"" alt=""(?<name>[^\""""]+)""",
                        RegexOptions.Compiled);
                if (match.Success)
                {
                    imageDownloadURL = match.Groups["url"].Value.Replace("&amp;", "&");
                    filePath = match.Groups["name"].Value;
                }
            }
            else
            {

                ((CacheObject)EventTable[imageURL]).IsDownloaded = false;
                return false;
            }

            // Finally Download the Image
            return this.DownloadImageAsync(imageDownloadURL, filePath);
        }
    }
}