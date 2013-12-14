// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtractHelper.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Ripper.Core.Components;
    using Ripper.Core.Objects;

    /// <summary>
    /// Helper Class to Extract Content from forum XML Pages
    /// </summary>
    public class ExtractHelper
    {
        /// <summary>
        /// Attempts to extract hot linked and thumb-&gt;FullScale images.
        /// </summary>
        /// <param name="strDump">
        /// The STR dump.
        /// </param>
        /// <returns>
        /// The extract images.
        /// </returns>
        public static List<ImageInfo> ExtractImages(string strDump)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();
            Hashtable rtnHashChk = new Hashtable();

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(strDump));

                foreach (DataRow row in ds.Tables["Image"].Rows)
                {
                    string thumbUrl;

                    try
                    {
                        thumbUrl = row["thumb_url"].ToString();
                    }
                    catch (Exception)
                    {
                        thumbUrl = string.Empty;
                    }

                    ImageInfo newPicPool = new ImageInfo
                                               {
                                                   ImageUrl = row["main_url"].ToString(),
                                                   ThumbnailUrl = thumbUrl
                                               };

                    newPicPool.ImageUrl = Regex.Replace(newPicPool.ImageUrl, @"""", string.Empty);

                    //////////////////////////////////////////////////////////////////////////
                    if (Utility.IsImageNoneSense(newPicPool.ImageUrl))
                    {
                        continue;
                    }

                    newPicPool.ImageUrl = Utility.ReplaceHexWithAscii(newPicPool.ImageUrl);

                    // Remove anonym.to from Link if exists
                    if (newPicPool.ImageUrl.Contains("anonym.to"))
                    {
                        newPicPool.ImageUrl = newPicPool.ImageUrl.Replace("http://www.anonym.to/?", string.Empty);
                    }

                    // Remove redirect
                    if (newPicPool.ImageUrl.Contains("redirect-to"))
                    {
                        newPicPool.ImageUrl =
                            newPicPool.ImageUrl.Replace(
                                string.Format(
                                    "{0}redirect-to/?redirect=",
                                    CacheController.Instance().UserSettings.ForumURL),
                                string.Empty);
                    }

                    // Get Real Url
                    if (newPicPool.ImageUrl.Contains("/out/out.php?x="))
                    {
                        var req = (HttpWebRequest)WebRequest.Create(newPicPool.ImageUrl);

                        req.Referer = newPicPool.ImageUrl;
                        req.Timeout = 20000;

                        var res = (HttpWebResponse)req.GetResponse();

                        newPicPool.ImageUrl = res.ResponseUri.ToString();

                        res.Close();
                    }

                    if (rtnHashChk.Contains(newPicPool.ImageUrl))
                    {
                        continue;
                    }

                    rtnList.Add(newPicPool);
                    rtnHashChk.Add(newPicPool.ImageUrl, "OK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }

            return rtnList;
        }

        /// <summary>
        /// Extracts links leading to other threads and posts for indices crawling.
        /// </summary>
        /// <param name="xmlDump">
        /// The XML dump.
        /// </param>
        /// <returns>
        /// The extract rip URL's.
        /// </returns>
        public static List<ImageInfo> ExtractRiPUrls(string xmlDump)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();
            Hashtable rtnHashChk = new Hashtable();

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlDump));

                foreach (ImageInfo newPicPool in
                    from DataRow row in ds.Tables["Image"].Rows
                    select
                        new ImageInfo
                            {
                                ImageUrl = row["main_url"].ToString(),
                                ThumbnailUrl =
                                    row["thumb_url"] != null ? row["thumb_url"].ToString() : string.Empty
                            })
                {
                    newPicPool.ImageUrl = Utility.ReplaceHexWithAscii(newPicPool.ImageUrl);

                    if (rtnHashChk.Contains(newPicPool.ImageUrl))
                    {
                        continue;
                    }

                    rtnList.Add(newPicPool);
                    rtnHashChk.Add(newPicPool.ImageUrl, "OK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }

            return rtnList;
        }

        /// <summary>
        /// Extracts the thread to posts.
        /// </summary>
        /// <param name="xmlDump">
        /// The XML dump.
        /// </param>
        /// <returns>
        /// The extract thread to posts.
        /// </returns>
        public static List<ImageInfo> ExtractThreadtoPosts(string xmlDump)
        {
            List<ImageInfo> rtnList = new List<ImageInfo>();
            Hashtable rtnHashChk = new Hashtable();

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlDump));

                foreach (ImageInfo newPicPool in
                    ds.Tables["post"].Rows.Cast<DataRow>()
                        .Where(row => row["id"] != null)
                        .Select(row => new ImageInfo { ImageUrl = row["id"].ToString() }))
                {
                    newPicPool.ImageUrl = Utility.ReplaceHexWithAscii(newPicPool.ImageUrl);

                    if (rtnHashChk.Contains(newPicPool.ImageUrl))
                    {
                        continue;
                    }

                    rtnList.Add(newPicPool);
                    rtnHashChk.Add(newPicPool.ImageUrl, "OK");
                }
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(xmlDump, ex.StackTrace, null);
            }

            return rtnList;
        }
    }
}