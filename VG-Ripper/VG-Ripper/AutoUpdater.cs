// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoUpdater.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
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
            var tempFolder = Path.Combine(Application.StartupPath, "temp");

            try
            {
                // Download Update 
                var downloadURL = string.Format(
                    "http://www.watchersnet.de/rip-ripper/VG-Ripper{0}.zip", 
                    VersionCheck.OnlineVersion);

                var tempZIP = Path.Combine(Application.StartupPath, "temp.zip");

                var client = new WebClient();
                client.DownloadFile(downloadURL, tempZIP);

                client.Dispose();

                if (File.Exists(tempZIP))
                {
                    // Extract Zip file
                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempZIP, tempFolder, null);

                    File.Delete(tempZIP);

                    // Check for Microsoft.WindowsAPICodePack.dll
                    if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll")))
                    {
                        File.Copy(
                            Path.Combine(tempFolder, "Microsoft.WindowsAPICodePack.dll"),
                            Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll"));
                    }

                    // Check for Microsoft.WindowsAPICodePack.Shell.dll
                    if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll")))
                    {
                        File.Copy(
                            Path.Combine(tempFolder, "Microsoft.WindowsAPICodePack.Shell.dll"),
                            Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll"));
                    }

                    if (File.Exists(Path.Combine(Application.StartupPath, "VG-Ripper.exe")))
                    {
                        // Replace Exe
                        File.Replace(
                            Path.Combine(Application.StartupPath, string.Format("temp{0}VG-Ripper.exe", Path.DirectorySeparatorChar)), 
                            Assembly.GetExecutingAssembly().Location,
                            "VG-Ripper.bak");

                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }

                        var upgradeProcess = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
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
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }

            HARD_RETURN:
            return;
        }

        #endregion
    }
}

#endif