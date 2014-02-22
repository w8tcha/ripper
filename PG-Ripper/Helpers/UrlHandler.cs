// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlHandler.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;

    using Ripper.Core.Components;

    /// <summary>
    /// Formats the URLs
    /// </summary>
    public class UrlHandler
    {
        /// <summary>
        /// Parse Job Url or ID
        /// </summary>
        /// <param name="inputUrl">The input URL.</param>
        /// <param name="comboBoxValue">The Input Type.</param>
        /// <returns>
        /// Html Url
        /// </returns>
        public static string GetHtmlUrl(string inputUrl, int comboBoxValue)
        {
            string sHtmlUrl;

            switch (comboBoxValue)
            {
                case 0:
                    {
                        sHtmlUrl = string.Format(
                            "{0}showthread.php?t={1}", CacheController.Instance().UserSettings.CurrentForumUrl, Convert.ToInt64(inputUrl));
                        break;
                    }

                case 1:
                    {
                        sHtmlUrl = string.Format(
                            "{0}showpost.php?p={1}#post{1}", CacheController.Instance().UserSettings.CurrentForumUrl, Convert.ToInt64(inputUrl));
                        break;
                    }

                default:
                    {
                        sHtmlUrl = inputUrl;

                        // Make sure url starts with http://
                        if (sHtmlUrl.IndexOf("http://") != 0)
                        {
                            return string.Empty;
                        }


                        if (sHtmlUrl.Contains(".html") && !sHtmlUrl.Contains(".php"))
                        {
                            // http://mydomain.com/subforumname/01234-threadtitle.html
                            // Threads
                            // http://mydomain.com/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                            // Posts
                            ////string sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("#post") + 5);

                            return sHtmlUrl;
                        }

                        if (!sHtmlUrl.Contains("#post") && sHtmlUrl.Contains("showthread.php?t="))
                        {
                            // http://mydomain.com/showthread.php?t=0123456
                            // Threats
                            return sHtmlUrl;
                        }

                        if (!sHtmlUrl.Contains("#post") && sHtmlUrl.Contains("showthread.php?"))
                        {
                            // New VB Forums 4.x
                            // http://mydomain.com/showthread.php?0123456-Thread-Title
                            // Threads
                            string sThreadId = sHtmlUrl.Substring(sHtmlUrl.IndexOf(".php?") + 5);

                            if (sHtmlUrl.Contains("-"))
                            {
                                sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                            }

                            sHtmlUrl = string.Format(
                                "{0}showthread.php?t={1}",
                                CacheController.Instance().UserSettings.CurrentForumUrl,
                                Convert.ToInt64(sThreadId));
                        }
                        else if (sHtmlUrl.Contains("goto=newpost") && sHtmlUrl.Contains("showthread.php?"))
                        {
                            // http://mydomain.com/showthread.php?0123456-Thread-Title&goto=newpost#post12345
                            // Threads
                            string sThreadId = sHtmlUrl.Substring(sHtmlUrl.IndexOf(".php?") + 5);

                            if (sHtmlUrl.Contains("-"))
                            {
                                sThreadId = sThreadId.Remove(sThreadId.IndexOf("-"));
                            }

                            sHtmlUrl = string.Format(
                                "{0}showthread.php?t={1}",
                                CacheController.Instance().UserSettings.CurrentForumUrl,
                                Convert.ToInt64(sThreadId));
                        }
                        else if (sHtmlUrl.Contains("&p=") && sHtmlUrl.Contains("#post"))
                        {
                            // http://mydomain.com/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                            // Posts
                            ////string sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("#post") + 5);

                            return sHtmlUrl;
                        }


                        break;
                    }
            }

            return sHtmlUrl;
        }
    }
}