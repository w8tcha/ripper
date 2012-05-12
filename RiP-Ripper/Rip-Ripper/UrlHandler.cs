// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlHandler.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System;

    /// <summary>
    /// Class to Convert rip urls in to xml url
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
            string sXmlUrl;

            switch (comboBoxValue)
            {
                case 0:
                    {
                        sXmlUrl = string.Format(
                            "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}",
                            Convert.ToInt64(inputUrl));
                        break;
                    }

                case 1:
                    {
                        sXmlUrl = string.Format(
                            "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}",
                            Convert.ToInt64(inputUrl));
                        break;
                    }

                default:
                    {
                        sXmlUrl = inputUrl;

                        // Make sure url starts with http://
                        if (sXmlUrl.IndexOf("http://") != 0)
                        {
                            return string.Empty;
                        }
                        
                        /*Old VB Forums 3.x
                        sXmlUrl = sXmlUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        sXmlUrl = sXmlUrl.Replace("showthread.php?goto=newpost&t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        sXmlUrl = sXmlUrl.Replace("showthread.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");*/

                        // Old VB Forums 3.x
                        if (sXmlUrl.Contains("showthread.php?t="))
                        {
                            // Threads
                            sXmlUrl = sXmlUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
                        }
                        else if (sXmlUrl.Contains("showpost.php?p="))
                        {
                            // Posts
                            sXmlUrl = sXmlUrl.Replace("showpost.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");
                        }
                        else if (!sXmlUrl.Contains("#post") && sXmlUrl.Contains("showthread.php?"))
                        { // New VB Forums 4.x
                            // http://rip-productions.net/showthread.php?0123456-Thread-Title
                            // Threads
                            string sThreadId = sXmlUrl.Substring(sXmlUrl.IndexOf(".php?") + 5);

                            if (sXmlUrl.Contains("-"))
                            {
                                sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                            }

                            sXmlUrl =
                                string.Format(
                                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}",
                                    Convert.ToInt64(sThreadId));
                        }
                        else if (sXmlUrl.Contains("goto=newpost") && sXmlUrl.Contains("showthread.php?"))
                        {
                            // http://rip-productions.net/showthread.php?0123456-Thread-Title&goto=newpost#post12345
                            // Threads
                            string sThreadId = sXmlUrl.Substring(sXmlUrl.IndexOf(".php?") + 5);

                            if (sXmlUrl.Contains("-"))
                            {
                                sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                            }

                            sXmlUrl =
                                string.Format(
                                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}",
                                    Convert.ToInt64(sThreadId));
                        }
                        else if (sXmlUrl.Contains("&p=") && sXmlUrl.Contains("#post"))
                        {
                            // http://rip-productions.net/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                            // Posts
                            string sPostId = sXmlUrl.Substring(sXmlUrl.IndexOf("#post") + 5);
                            sXmlUrl =
                                string.Format(
                                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}",
                                    Convert.ToInt64(sPostId));
                        }
                        else if (!sXmlUrl.Contains(".php") && !sXmlUrl.Contains("#post"))
                        {
                            // http://rip-productions.net/subforumname/01234-threadtitle.html
                            // Threads
                            string sThreadId = sXmlUrl.Substring(sXmlUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                            if (sXmlUrl.Contains("-"))
                            {
                                sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                            }

                            sXmlUrl =
                                string.Format(
                                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}",
                                    Convert.ToInt64(sThreadId));
                        }
                        else if (!sXmlUrl.Contains(".php") && sXmlUrl.Contains("#post"))
                        {
                            // Posts
                            string sPostId = sXmlUrl.Substring(sXmlUrl.IndexOf("#post") + 5);
                            sXmlUrl =
                                string.Format(
                                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}",
                                    Convert.ToInt64(sPostId));
                        }

                        break;
                    }
            }

            return sXmlUrl;
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
            string sNewUrl = inputUrl;

            if (sNewUrl.Contains("showthread.php?t="))
            {
                // Threads
                sNewUrl = sNewUrl.Replace("showthread.php?t=", "getSTDpost-imgXML.php?dpver=2&threadid=");
            }
            else if (sNewUrl.Contains("showpost.php?p="))
            {
                // Posts
                sNewUrl = sNewUrl.Replace("showpost.php?p=", "getSTDpost-imgXML.php?dpver=2&postid=");
            }
            else if (!sNewUrl.Contains("#post") && sNewUrl.Contains("showthread.php?"))
            {
                // Threads
                string sThreadId = sNewUrl.Substring(sNewUrl.IndexOf(".php?") + 5);

                if (sNewUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                sNewUrl = string.Format(
                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}", Convert.ToInt64(sThreadId));
            }
            else if (sNewUrl.Contains("goto=newpost") && sNewUrl.Contains("showthread.php?"))
            {
                // Threads
                string sThreadId = sNewUrl.Substring(sNewUrl.IndexOf(".php?") + 5);

                if (sNewUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                sNewUrl = string.Format(
                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}", Convert.ToInt64(sThreadId));
            }
            else if (sNewUrl.Contains("&p=") && sNewUrl.Contains("#post"))
            {
                // Posts
                string sPostId = sNewUrl.Substring(sNewUrl.IndexOf("#post") + 5);
                sNewUrl = string.Format(
                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}", Convert.ToInt64(sPostId));
            }
            else if (!sNewUrl.Contains(".php") && !sNewUrl.Contains("#post"))
            {
                // Threads
                string sThreadId = sNewUrl.Substring(sNewUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                if (sNewUrl.Contains("-"))
                {
                    sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                }

                sNewUrl = string.Format(
                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&threadid={0}", Convert.ToInt64(sThreadId));
            }
            else if (!sNewUrl.Contains(".php") && sNewUrl.Contains("#post"))
            {
                // Posts
                string sPostId = sNewUrl.Substring(sNewUrl.IndexOf("#post") + 5);
                sNewUrl = string.Format(
                    "http://rip-productions.net/getSTDpost-imgXML.php?dpver=2&postid={0}", Convert.ToInt64(sPostId));
            }

            return sNewUrl;
        }
    }
}