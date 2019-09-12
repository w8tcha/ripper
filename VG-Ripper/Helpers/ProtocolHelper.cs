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

            const string UrlProtocol = "vgr";

            try
            {
                var registryKey = Registry.ClassesRoot.OpenSubKey(UrlProtocol, true);

                var currentApplicationPath = $"\"{Application.ExecutablePath}\" %1";

                if (registryKey == null)
                {
                    registryKey = Registry.ClassesRoot.CreateSubKey(UrlProtocol);

                    registryKey.SetValue(null, "VG-Ripper vgr Protocol");
                    registryKey.SetValue("URL Protocol", string.Empty);

                    Registry.ClassesRoot.CreateSubKey($"{UrlProtocol}\\Shell");
                    Registry.ClassesRoot.CreateSubKey($"{UrlProtocol}\\Shell\\open");

                    registryKey =
                        Registry.ClassesRoot.CreateSubKey($"{UrlProtocol}\\Shell\\open\\command");

                    registryKey.SetValue(null, currentApplicationPath);

                    protocolRegisterd = true;
                }
                else
                {
                    registryKey =
                        Registry.ClassesRoot.OpenSubKey($"{UrlProtocol}\\Shell\\open\\command");

                    var applicationPath = registryKey.GetValue(string.Empty);

                    if (!applicationPath.Equals(currentApplicationPath))
                    {
                        // Update Path if Incorrect?!
                        Registry.SetValue(
                            $"HKEY_CLASSES_ROOT\\{UrlProtocol}\\Shell\\open\\command",
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

            if (args[0].Trim().ToLower() != "vgr")
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
                        var details = ripType[1].Split('=');
                        if (details.Length > 1)
                        {
                            return
                                $"{CacheController.Instance().UserSettings.ForumURL}showthread.php?t={details[1].Trim()}";
                        }
                    }

                    break;
                case "RIPPOST":
                    {
                        // "vg:RipPost?id=123"
                        var details = ripType[1].Split('=');

                        if (details.Length > 1)
                        {
                            return
                                $"{CacheController.Instance().UserSettings.ForumURL}showpost.php?p={details[1].Trim()}";
                        }
                    }

                    break;
            }

            return string.Empty;
        } 
    }
}