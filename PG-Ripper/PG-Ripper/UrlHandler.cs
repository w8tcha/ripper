using System;

namespace PGRipper
{
    public class UrlHandler
    {
        /// <summary>
        /// Parse Job Url or ID
        /// </summary>
        /// <returns>Html Url</returns>
        public static String GetHtmlUrl(string sInputUrl, int iComboBoxValue)
        {
            string sHtmlUrl;

            switch (iComboBoxValue)
            {
                case 0:
                    {
                        sHtmlUrl = string.Format("{0}showthread.php?t={1}", MainForm.userSettings.sForumUrl,
                                                 Convert.ToInt64(sInputUrl));
                        break;
                    }
                case 1:
                    {
                        sHtmlUrl = string.Format("{0}showpost.php?p={1}", MainForm.userSettings.sForumUrl, Convert.ToInt64(sInputUrl));
                        break;
                    }
                default:
                    {
                        sHtmlUrl = sInputUrl;

                        // Make sure url starts with http://
                        if (sHtmlUrl.IndexOf("http://") != 0)
                        {
                            return string.Empty;
                        }

                        if (MainForm.userSettings.sForumUrl.Contains(@"http://rip-") ||
                            MainForm.userSettings.sForumUrl.Contains(@"http://www.rip-"))
                        {


                            if (sHtmlUrl.Contains(".html") && !sHtmlUrl.Contains(".php"))
                            {
                                // http://mydomain.com/subforumname/01234-threadtitle.html
                                // Threads
                                // http://mydomain.com/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                                // Posts
                                //string sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("#post") + 5);

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

                                sHtmlUrl = string.Format("{0}showthread.php?t={1}", MainForm.userSettings.sForumUrl,
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

                                sHtmlUrl = string.Format("{0}showthread.php?t={1}", MainForm.userSettings.sForumUrl,
                                                         Convert.ToInt64(sThreadId));
                            }
                            else if (sHtmlUrl.Contains("&p=") && sHtmlUrl.Contains("#post"))
                            {
                                // http://mydomain.com/showthread.php?0123456-Thread-Title&p=123456&viewfull=1#post123456
                                // Posts
                                //string sPostId = sHtmlUrl.Substring(sHtmlUrl.IndexOf("#post") + 5);

                                return sHtmlUrl;
                            }
                        }


                        break;
                    }
            }

            return sHtmlUrl;
        }
    }
}