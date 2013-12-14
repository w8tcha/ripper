// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolHelper.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using Ripper.Core.Components;

    /// <summary>
    /// Helper Class for the rip Protocol
    /// </summary>
    public class ProtocolHelper
    {
        /// <summary>
        /// Registers the rip URL protocol.
        /// </summary>
        /// <returns>Returns if the Protocol was registered</returns>
        public static bool RegisterRipUrlProtocol()
        {
            var protocolRegisterd = false;

            const string UrlProtocol = "vg";

            try
            {
                var registryKey = Registry.ClassesRoot.OpenSubKey(UrlProtocol, true);

                var currentApplicationPath = string.Format("\"{0}\" %1", Application.ExecutablePath);

                if (registryKey == null)
                {
                    registryKey = Registry.ClassesRoot.CreateSubKey(UrlProtocol);

                    registryKey.SetValue(null, "VG-Ripper vg Protocol");
                    registryKey.SetValue("URL Protocol", string.Empty);

                    Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\Shell", UrlProtocol));
                    Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\Shell\\open", UrlProtocol));

                    registryKey =
                        Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\Shell\\open\\command", UrlProtocol));

                    registryKey.SetValue(null, currentApplicationPath);

                    protocolRegisterd = true;
                }
                else
                {
                    registryKey =
                        Registry.ClassesRoot.OpenSubKey(string.Format("{0}\\Shell\\open\\command", UrlProtocol));

                    var applicationPath = registryKey.GetValue(string.Empty);

                    if (!applicationPath.Equals(currentApplicationPath))
                    {
                        // Update Path if Incorrect?!
                        Registry.SetValue(
                            string.Format("HKEY_CLASSES_ROOT\\{0}\\Shell\\open\\command", UrlProtocol),
                            null,
                            currentApplicationPath);
                    }

                    protocolRegisterd = true;
                }

                registryKey.Close();
            }
            catch (Exception)
            {
                return protocolRegisterd;
            }

            return true;
        }

        /// <summary>
        /// Process the arguments.
        /// </summary>
        /// <param name="parseArgument">The parse argument.</param>
        /// <returns>Returns the XML Rip Url</returns>
        public static string ProccesArguments(string parseArgument)
        {
            if (string.IsNullOrEmpty(parseArgument))
            {
                return string.Empty;
            }

            var args = parseArgument.Split(':');

            if (args[0].Trim().ToLower() != "vg")
            {
                return string.Empty;
            }

            var ripType = args[1].Split('?');

            if (ripType.Length <= 1)
            {
                return string.Empty;
            }

            switch (ripType[0].Trim().ToUpper())
            {
                case "RIPTHREAD":
                    {
                        // "vg:RipThread?id=123"
                        string[] details = ripType[1].Split('=');
                        if (details.Length > 1)
                        {
                            return string.Format(
                                "{0}showthread.php?t={1}",
                                CacheController.Instance().UserSettings.ForumURL,
                                details[1].Trim());
                        }
                    }

                    break;
                case "RIPPOST":
                    {
                        // "vg:RipPost?id=123"
                        string[] details = ripType[1].Split('=');

                        if (details.Length > 1)
                        {
                            return string.Format(
                                "{0}showpost.php?p={1}",
                                CacheController.Instance().UserSettings.ForumURL,
                                details[1].Trim());
                        }
                    }

                    break;
            }

            return string.Empty;
        } 
    }
}