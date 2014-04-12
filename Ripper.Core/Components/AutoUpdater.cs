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

//#if (!RIPRIPPERX)

namespace Ripper.Core.Components
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
    public class AutoUpdater
    {
        #region Public Methods

        /// <summary>
        /// TryUpdate: Invoke this method when you are ready to run the update checking thread
        /// </summary>
        /// <param name="programName">Name of the program.</param>
        /// <param name="assembly">The assembly.</param>
        public static void TryUpdate(string programName, Assembly assembly)
        {
            var backgroundThread = programName.Equals("Ripper.Services")
                                       ? new Thread(() => UpdateServicesDll(assembly)) { IsBackground = true }
                                       : new Thread(() => UpdateProgram(programName, assembly)) { IsBackground = true };

            backgroundThread.Start();
        }

        /// <summary>
        /// Cleanup after the update.
        /// </summary>
        /// <param name="programName">Name of the program.</param>
        public static void CleanupAfterUpdate(string programName)
        {
            var tempFolder = Path.Combine(Application.StartupPath, "temp");

            // Delete Backup Files From AutoUpdate
            if (File.Exists(Path.Combine(Application.StartupPath, string.Format("{0}.bak", programName))))
            {
                File.Delete(Path.Combine(Application.StartupPath, string.Format("{0}.bak", programName)));
            }

            if (File.Exists(Path.Combine(Application.StartupPath, "Ripper.Services.bak")))
            {
                File.Delete(Path.Combine(Application.StartupPath, "Ripper.Services.bak"));
            }

            if (File.Exists(Path.Combine(Application.StartupPath, "Ripper.Core.bak")))
            {
                File.Delete(Path.Combine(Application.StartupPath, "Ripper.Core.bak"));
            }

            // Finally Delete Temp Directory
            DeleteFolder(tempFolder);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Auto Updater, download latest version and restart it.
        /// </summary>
        /// <param name="programName">Name of the program.</param>
        /// <param name="assembly">The assembly.</param>
        private static void UpdateProgram(string programName, Assembly assembly)
        {
            var tempFolder = Path.Combine(Application.StartupPath, "temp");

            try
            {
                // Download Update 
                var downloadURL = string.Format(
                    "http://www.watchersnet.de/rip-ripper/{0}{1}.zip",
                    programName,
                    VersionCheck.OnlineVersion);

                var tempZIP = Path.Combine(Application.StartupPath, "temp.zip");

                var client = new WebClient();
                client.DownloadFile(downloadURL, tempZIP);

                client.Dispose();

                if (!File.Exists(tempZIP))
                {
                    return;
                }

                // Extract Zip file
                var fastZip = new FastZip();
                fastZip.ExtractZip(tempZIP, tempFolder, null);

                File.Delete(tempZIP);

                if (programName.Equals("PG-Ripper"))
                {
                    programName = "PGRipper";
                }

                if (!File.Exists(Path.Combine(tempFolder, string.Format("{0}.exe", programName))))
                {
                    return;
                }

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

                // Check for Ripper.Services.dll
                if (!File.Exists(Path.Combine(Application.StartupPath, "Ripper.Services.dll")))
                {
                    File.Copy(
                        Path.Combine(tempFolder, "Ripper.Services.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Services.dll"));
                }
                else
                {
                    File.Replace(
                        Path.Combine(tempFolder, "Ripper.Services.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Services.dll"),
                       "Ripper.Services.bak");
                }

                // Check for Ripper.Core.dll
                if (!File.Exists(Path.Combine(Application.StartupPath, "Ripper.Core.dll")))
                {
                    File.Copy(
                        Path.Combine(tempFolder, "Ripper.Core.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Core.dll"));
                }
                else
                {
                    File.Replace(
                        Path.Combine(tempFolder, "Ripper.Core.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Core.dll"),
                       "Ripper.Core.bak");
                }

                if (!File.Exists(Path.Combine(Application.StartupPath, string.Format("{0}.exe", programName))))
                {
                    return;
                }

                // Replace Exe
                File.Replace(
                    Path.Combine(tempFolder, string.Format("{0}.exe", programName)),
                    assembly.Location,
                    string.Format("{0}.bak", programName));

                DeleteFolder(tempFolder);

                var upgradeProcess = new ProcessStartInfo(assembly.Location);
                Process.Start(upgradeProcess);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                // Finally Delete Temp Directory
                DeleteFolder(tempFolder);
            }
        }

        /// <summary>
        /// Auto Updater, download latest version and restart it.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private static void UpdateServicesDll(Assembly assembly)
        {
            var tempFolder = Path.Combine(Application.StartupPath, "temp");

            try
            {
                // Download Update 
                const string DownloadURL = "http://www.watchersnet.de/rip-ripper/Ripper.Services.dll";

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                var client = new WebClient();
                client.DownloadFile(DownloadURL, Path.Combine(tempFolder, "Ripper.Services.dll"));

                client.Dispose();

                if (!File.Exists(Path.Combine(tempFolder, "Ripper.Services.dll")))
                {
                    return;
                }

                if (!File.Exists(Path.Combine(Application.StartupPath, "Ripper.Services.dll")))
                {
                    File.Copy(
                        Path.Combine(tempFolder, "Ripper.Services.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Services.dll"));
                }
                else
                {
                    File.Replace(
                        Path.Combine(tempFolder, "Ripper.Services.dll"),
                        Path.Combine(Application.StartupPath, "Ripper.Services.dll"),
                        "Ripper.Services.bak");
                }

                var upgradeProcess = new ProcessStartInfo(assembly.Location);
                Process.Start(upgradeProcess);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Utility.SaveOnCrash(ex.Message, ex.StackTrace, null);
            }
            finally
            {
                // Finally Delete Temp Directory
                DeleteFolder(tempFolder);
            }
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns if Folder was deleted</returns>
        private static bool DeleteFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}

//#endif