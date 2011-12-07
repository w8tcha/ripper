// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Maintainance.cs" company="The Watcher">
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

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;

    #endregion

    /// <summary>
    /// Maintainance Class.
    /// </summary>
    public class Maintainance
    {
        #region Constants and Fields

        /// <summary>
        /// The m instace.
        /// </summary>
        public static Maintainance mInstace;

        /// <summary>
        /// The xform.
        /// </summary>
        public static Form xform;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static Maintainance GetInstance()
        {
            return mInstace ?? (mInstace = new Maintainance());
        }

        /// <summary>
        /// count The Images from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns How Many Images the Post contains
        /// </returns>
        public int CountImagesFromXML(string xmlPayload)
        {
            int iImgCount;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlPayload));

                // iImageCount = ds.Tables["Image"].Rows.Count;
                iImgCount =
                    ds.Tables["Image"].Rows.Cast<DataRow>().Count(
                        row => !Utility.IsImageNoneSense(row["main_url"].ToString()));
            }
            catch (Exception)
            {
                return 0;
            }

            return iImgCount;
        }

        /// <summary>
        /// The extract forum title from xml.
        /// </summary>
        /// <param name="sXmlPayload">
        /// The s xml payload.
        /// </param>
        /// <returns>
        /// Returns the Forum Title
        /// </returns>
        public string ExtractForumTitleFromXML(string sXmlPayload)
        {
            string sForumTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(sXmlPayload));

                foreach (DataRow row in ds.Tables["forum"].Rows)
                {
                    sForumTitle = row["title"].ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            Utility.RemoveIllegalCharecters(sForumTitle);

            return sForumTitle;
        }

        /// <summary>
        /// The extract post title from xml.
        /// </summary>
        /// <param name="sXmlPayload">
        /// The s xml payload.
        /// </param>
        /// <returns>
        /// Returns the Post Title
        /// </returns>
        public string ExtractPostTitleFromXML(string sXmlPayload)
        {
            string sPostTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(sXmlPayload));

                foreach (DataRow row in ds.Tables["post"].Rows)
                {
                    sPostTitle = row["title"].ToString();

                    if (sPostTitle == string.Empty)
                    {
                        sPostTitle = string.Format("post# {0}", row["id"]);
                    }
                    else if (sPostTitle == string.Format("Re: {0}", this.ExtractTitleFromXML(sXmlPayload)))
                    {
                        sPostTitle = string.Format("post# {0}", row["id"]);
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            Utility.RemoveIllegalCharecters(sPostTitle);

            return sPostTitle;
        }

        /// <summary>
        /// The extract title from xml.
        /// </summary>
        /// <param name="sXmlPayload">
        /// The s xml payload.
        /// </param>
        /// <returns>
        /// Returns the Title
        /// </returns>
        public string ExtractTitleFromXML(string sXmlPayload)
        {
            string sTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(sXmlPayload));

                foreach (DataRow row in ds.Tables["thread"].Rows)
                {
                    sTitle = row["title"].ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return Utility.RemoveIllegalCharecters(sTitle);
        }

        /// <summary>
        /// The get post pages.
        /// </summary>
        /// <param name="sURL">
        /// The s url.
        /// </param>
        /// <returns>
        /// Returns the Post Pages Content
        /// </returns>
        public string GetPostPages(string sURL)
        {
            return GetRipPage(sURL);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get rip page.
        /// </summary>
        /// <param name="strURL">
        /// The str url.
        /// </param>
        /// <returns>
        /// Returns the Page Content as String
        /// </returns>
        private static string GetRipPage(string strURL)
        {
            string strPageRead;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                strPageRead = wc.DownloadString(strURL);

                wc.Dispose();
            }
            catch (Exception)
            {
                strPageRead = string.Empty;
            }

            return strPageRead;
        }

        #endregion
    }
}