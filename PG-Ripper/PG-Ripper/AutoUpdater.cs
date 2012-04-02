// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoUpdater.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on VB forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if (!PGRIPPERX)

namespace PGRipper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Class to download Update and restart the app
    /// </summary>
    internal class AutoUpdater
    {
        /// <summary>
        /// TryUpdate: Invoke this method when you are ready to run the update checking thread
        /// </summary>
        public static void TryUpdate()
        {
            Thread backgroundThread = new Thread(Update) { IsBackground = true };

            backgroundThread.Start();
        }

        /// <summary>
        /// Auto Updater, download latest version and restart it.
        /// </summary>
        private static void Update()
        {
            try
            {
                // Download Update 
                string sDownloadURL = string.Format("http://www.watchersnet.de/rip-ripper/PG-Ripper{0}.zip", VersionCheck.OnlineVersion.Replace(".", string.Empty));
                string sTempZIP = Path.Combine(Application.StartupPath, "temp.zip");

                WebClient client = new WebClient();
                client.DownloadFile(sDownloadURL, sTempZIP);

                client.Dispose();

                if (File.Exists(sTempZIP))
                {
                    // Extract Zip file
                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(sTempZIP, Path.Combine(Application.StartupPath, "temp"), null);

                    File.Delete(sTempZIP);

                    if (File.Exists(Path.Combine(Application.StartupPath, "temp\\PGRipper.exe")))
                    {
                        // Check for Microsoft.WindowsAPICodePack.dll
                        if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll")))
                        {
                            File.Copy(Path.Combine(Application.StartupPath, "temp\\Microsoft.WindowsAPICodePack.dll"), Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.dll"));
                        }

                        // Check for Microsoft.WindowsAPICodePack.Shell.dll
                        if (!File.Exists(Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll")))
                        {
                            File.Copy(Path.Combine(Application.StartupPath, "temp\\Microsoft.WindowsAPICodePack.Shell.dll"), Path.Combine(Application.StartupPath, "Microsoft.WindowsAPICodePack.Shell.dll"));
                        }

                        // Replace Exe
                        File.Replace(Application.StartupPath + "\\temp\\PGRipper.exe", Assembly.GetExecutingAssembly().Location, "PGRipper.bak");

                        if (Directory.Exists(Application.StartupPath + "\\temp"))
                        {
                            Directory.Delete(Application.StartupPath + "\\temp", true);
                        }

                        ProcessStartInfo upgradeProcess = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                        Process.Start(upgradeProcess);
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                // Finally Delete Temp Directory
                if (Directory.Exists(Application.StartupPath + "\\temp"))
                {
                    Directory.Delete(Application.StartupPath + "\\temp", true);
                }
            }
        }
    }
}
#endif