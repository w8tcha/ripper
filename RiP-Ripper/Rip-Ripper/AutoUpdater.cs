// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoUpdater.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#if (!RIPRIPPERX)

namespace RiPRipper
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using ICSharpCode.SharpZipLib.Zip;

    #endregion

    /// <summary>
    /// Class to download Update and restart the app
    /// </summary>
    internal class AutoUpdater
    {
        #region Public Methods

        /// <summary>
        /// TryUpdate: Invoke this method when you are ready to run the update checking thread
        /// </summary>
        public static void TryUpdate()
        {
            Thread backgroundThread = new Thread(Update) { IsBackground = true };
            backgroundThread.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Auto Updater, download latest version and restart it.
        /// </summary>
        private static void Update()
        {
            try
            {
                // Download Update 
                string sDownloadURL = string.Format(
                    "http://www.watchersnet.de/rip-ripper/RiPRipper{0}.zip", 
                    VersionCheck.OnlineVersion.Replace(".", string.Empty));

                string sTempZIP = string.Format("{0}\\temp.zip", Application.StartupPath);

                WebClient client = new WebClient();
                client.DownloadFile(sDownloadURL, sTempZIP);

                client.Dispose();

                if (File.Exists(sTempZIP))
                {
                    // Extract Zip file
                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(sTempZIP, Application.StartupPath + "\\temp", null);

                    File.Delete(sTempZIP);

                    // Check for Microsoft.WindowsAPICodePack.dll
                    if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll")))
                    {
                        File.Copy(
                            string.Format("{0}\\temp\\Microsoft.WindowsAPICodePack.dll", Application.StartupPath), 
                            string.Format("{0}\\Microsoft.WindowsAPICodePack.dll", Application.StartupPath));
                    }

                    // Check for Microsoft.WindowsAPICodePack.Shell.dll
                    if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll")))
                    {
                        File.Copy(
                            string.Format("{0}\\temp\\Microsoft.WindowsAPICodePack.Shell.dll", Application.StartupPath), 
                            string.Format("{0}\\Microsoft.WindowsAPICodePack.Shell.dll", Application.StartupPath));
                    }

                    if (File.Exists(string.Format("{0}\\temp\\RiPRipper.exe", Application.StartupPath)))
                    {
                        // Replace Exe
                        File.Replace(
                            string.Format("{0}\\temp\\RiPRipper.exe", Application.StartupPath), 
                            Assembly.GetExecutingAssembly().Location, 
                            "RiPRipper.bak");

                        if (Directory.Exists(string.Format("{0}\\temp", Application.StartupPath)))
                        {
                            Directory.Delete(string.Format("{0}\\temp", Application.StartupPath), true);
                        }

                        ProcessStartInfo upgradeProcess = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                        Process.Start(upgradeProcess);
                        Environment.Exit(0);
                    }
                    else
                    {
                        goto HARD_RETURN;
                    }
                }
                else
                {
                    goto HARD_RETURN;
                }
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                // Finally Delete Temp Directory
                if (Directory.Exists(string.Format("{0}\\temp", Application.StartupPath)))
                {
                    Directory.Delete(string.Format("{0}\\temp", Application.StartupPath), true);
                }
            }

            HARD_RETURN:
            return;
        }

        #endregion
    }
}

#endif