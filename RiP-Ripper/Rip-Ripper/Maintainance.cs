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
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    #endregion

    /// <summary>
    /// Maintainance Class.
    /// </summary>
    public class Maintainance
    {
        #region Constants and Fields

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static Maintainance Instance { get; set; }

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
            return Instance ?? (Instance = new Maintainance());
        }

        /// <summary>
        /// Count The Images from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns How Many Images the Post contains
        /// </returns>
        public IList<string> GetAllPostIds(string xmlPayload)
        {
            var postIds = new List<string>();

            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                postIds.AddRange(from DataRow row in dataSet.Tables["post"].Rows select row["id"].ToString());
            }
            catch (Exception)
            {
                return postIds;
            }

            return postIds;
        }

        /// <summary>
        /// Count The Images from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns How Many Images the Post contains
        /// </returns>
        public int CountImagesFromXML(string xmlPayload)
        {
            try
            {
                var dataSet = new DataSet();

                dataSet.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in dataSet.Tables["post"].Rows)
                {
                    return Convert.ToInt32(row["imageCount"]);
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// The extract forum title from xml.
        /// </summary>
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns the Forum Title
        /// </returns>
        public string ExtractForumTitleFromXML(string xmlPayload)
        {
            string sForumTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlPayload));

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
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns the Post Title
        /// </returns>
        public string ExtractPostTitleFromXML(string xmlPayload)
        {
            string sPostTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlPayload));

                foreach (DataRow row in ds.Tables["post"].Rows)
                {
                    sPostTitle = row["title"].ToString();

                    if (sPostTitle == string.Empty)
                    {
                        sPostTitle = string.Format("post# {0}", row["id"]);
                    }
                    else if (sPostTitle == string.Format("Re: {0}", this.ExtractTitleFromXML(xmlPayload)))
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
        /// <param name="xmlPayload">
        /// The xml payload.
        /// </param>
        /// <returns>
        /// Returns the Title
        /// </returns>
        public string ExtractTitleFromXML(string xmlPayload)
        {
            string sTitle = string.Empty;

            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(new StringReader(xmlPayload));

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

        #endregion

        #region Methods

        #endregion
    }
}