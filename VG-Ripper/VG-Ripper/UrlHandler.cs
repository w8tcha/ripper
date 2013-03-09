// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlHandler.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System;

    /// <summary>
    /// Class to Convert rip URLs in to xml url
    /// </summary>
    public class UrlHandler
    {
        /// <summary>
        /// Parse Job Url or ID to Xml Url
        /// </summary>
        /// <param name="inputUrl">
        /// The Input Url.
        /// </param>
        /// <param name="comboBoxValue">
        /// The Combo Box Value.
        /// </param>
        /// <returns>
        /// Xml Url
        /// </returns>
        public static string GetXmlUrl(string inputUrl, int comboBoxValue)
        {
            string xmlUrl;

            switch (comboBoxValue)
            {
                case 0:
                    {
                        xmlUrl = string.Format(
                            "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                            CacheController.GetInstance().UserSettings.ForumURL,
                            Convert.ToInt64(inputUrl));
                        break;
                    }

                case 1:
                    {
                        xmlUrl = string.Format(
                            "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                            CacheController.GetInstance().UserSettings.ForumURL,
                            Convert.ToInt64(inputUrl));
                        break;
                    }

                default:
                    {
                        xmlUrl = inputUrl;

                        // Make sure url starts with http://
                        if (xmlUrl.IndexOf("http://") != 0)
                        {
                            return string.Empty;
                        }
                        
                        /*Old VB Forums 3.x
                        sXmlUrl = sXmlUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        sXmlUrl = sXmlUrl.Replace("showthread.php?goto=newpost&t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        sXmlUrl = sXmlUrl.Replace("showthread.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");*/

                        // Old VB Forums 3.x
                        if (xmlUrl.Contains("showthread.php?t="))
                        {
                            // Threads
                            xmlUrl = xmlUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        }
                        else if (xmlUrl.Contains("showpost.php?p="))
                        {
                            // Posts
                            xmlUrl = xmlUrl.Replace("showpost.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");
                        }
                        else if (!xmlUrl.Contains("#post") && xmlUrl.Contains("showthread.php?"))
                        { 
                            // New VB Forums 4.x
                            // http://vipergirls.to/showthread.php?0123456-Thread-Title
                            // Threads
                            var threadId = xmlUrl.Substring(xmlUrl.IndexOf(".php?") + 5);

                            if (xmlUrl.Contains("-"))
                            {
                                threadId = threadId.Remove(threadId.IndexOf("-"));
                            }

                            xmlUrl = string.Format(
                                "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                                CacheController.GetInstance().UserSettings.ForumURL,
                                Convert.ToInt64(threadId));
                        }
                        else if (xmlUrl.Contains("goto=newpost") && xmlUrl.Contains("showthread.php?"))
                        {
                            // http://vipergirls.to/showthread.php?0123456-Thread-Title&goto=newpost#post12345
                            // Threads
                            var threadId = xmlUrl.Substring(xmlUrl.IndexOf(".php?") + 5);

                            if (xmlUrl.Contains("-"))
                            {
                                threadId = threadId.Remove(threadId.IndexOf("-"));
                            }

                            xmlUrl = string.Format(
                                "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                                CacheController.GetInstance().UserSettings.ForumURL,
                                Convert.ToInt64(threadId));
                        }
                        else if (xmlUrl.Contains("&p=") && xmlUrl.Contains("#post"))
                        {
                            // http://vipergirls.to/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                            // Posts
                            var postId = xmlUrl.Substring(xmlUrl.IndexOf("#post") + 5);

                            xmlUrl = string.Format(
                                "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                                CacheController.GetInstance().UserSettings.ForumURL,
                                Convert.ToInt64(postId));
                        }
                        else if (!xmlUrl.Contains(".php") && !xmlUrl.Contains("#post"))
                        {
                            // http://vipergirls.to/subforumname/01234-threadtitle.html
                            // Threads
                            var threadId = xmlUrl.Substring(xmlUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                            if (xmlUrl.Contains("-"))
                            {
                                threadId = threadId.Remove(threadId.IndexOf("-"));
                            }

                            xmlUrl = string.Format(
                                "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                                CacheController.GetInstance().UserSettings.ForumURL,
                                Convert.ToInt64(threadId));
                        }
                        else if (!xmlUrl.Contains(".php") && xmlUrl.Contains("#post"))
                        {
                            // Posts
                            var postId = xmlUrl.Substring(xmlUrl.IndexOf("#post") + 5);

                            xmlUrl = string.Format(
                                "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                                CacheController.GetInstance().UserSettings.ForumURL,
                                Convert.ToInt64(postId));
                        }

                        break;
                    }
            }

            return xmlUrl;
        }

        /// <summary>
        /// Get the Index Url
        /// </summary>
        /// <param name="inputUrl">
        /// The input url.
        /// </param>
        /// <returns>
        /// Returns the Index Url
        /// </returns>
        public static string GetIndexUrl(string inputUrl)
        {
            var newUrl = inputUrl;

            if (newUrl.Contains("showthread.php?t="))
            {
                // Threads
                newUrl = newUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
            }
            else if (newUrl.Contains("showpost.php?p="))
            {
                // Posts
                newUrl = newUrl.Replace("showpost.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");
            }
            else if (!newUrl.Contains("#post") && newUrl.Contains("showthread.php?"))
            {
                // Threads
                string sThreadId = newUrl.Substring(newUrl.IndexOf(".php?") + 5);

                if (newUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                newUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                    CacheController.GetInstance().UserSettings.ForumURL,
                    Convert.ToInt64(sThreadId));
            }
            else if (newUrl.Contains("goto=newpost") && newUrl.Contains("showthread.php?"))
            {
                // Threads
                string sThreadId = newUrl.Substring(newUrl.IndexOf(".php?") + 5);

                if (newUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                newUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                    CacheController.GetInstance().UserSettings.ForumURL,
                    Convert.ToInt64(sThreadId));
            }
            else if (newUrl.Contains("&p=") && newUrl.Contains("#post"))
            {
                // Posts
                string sPostId = newUrl.Substring(newUrl.IndexOf("#post") + 5);
                newUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                    CacheController.GetInstance().UserSettings.ForumURL,
                    Convert.ToInt64(sPostId));
            }
            else if (!newUrl.Contains(".php") && !newUrl.Contains("#post"))
            {
                // Threads
                string sThreadId = newUrl.Substring(newUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                if (newUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                newUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&threadid={1}",
                    CacheController.GetInstance().UserSettings.ForumURL,
                    Convert.ToInt64(sThreadId));
            }
            else if (!newUrl.Contains(".php") && newUrl.Contains("#post"))
            {
                // Posts
                string sPostId = newUrl.Substring(newUrl.IndexOf("#post") + 5);
                newUrl = string.Format(
                    "{0}getSTDpost-imgXML.php?dpver=2&postid={1}",
                    CacheController.GetInstance().UserSettings.ForumURL,
                    Convert.ToInt64(sPostId));
            }

            return newUrl;
        }
    }
}