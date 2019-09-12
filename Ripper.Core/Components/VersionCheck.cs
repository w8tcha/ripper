// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCheck.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Core.Components
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Comparing Version Check from online xml file with Assembly
    /// </summary>
    public class VersionCheck
    {
        /// <summary>
        /// Gets or sets the Version String of the Online Version
        /// </summary>
        public static string OnlineVersion { get; set; }

        /// <summary>
        /// Check if Update is available
        /// </summary>
        /// <param name="ripperAssembly">The ripper assembly.</param>
        /// <param name="name">The program name.</param>
        /// <param name="releaseNotes">The release notes.</param>
        /// <returns>
        /// Returns if Update is available
        /// </returns>
        public static bool UpdateAvailable(Assembly ripperAssembly, string name, out string releaseNotes)
        {
            releaseNotes = string.Empty;

            // Proper Format: (MAJOR).(MINOR).(BUILD).(REVISION) Decimal makes it easier to judge versions
            try
            {
                var dbCurrentVersion =
                    double.Parse(
                        $"{ripperAssembly.GetName().Version.Major}.{ripperAssembly.GetName().Version.Minor}.{ripperAssembly.GetName().Version.Build}.{ripperAssembly.GetName().Version.Revision}");

                var ds = new DataSet();

                ds.ReadXml("http://www.watchersnet.de/rip-ripper/ripperUpdates.xml");

                foreach (var row in
                    from DataRow row in ds.Tables["ripper"].Rows
                    let nameRow = row["name"].ToString()
                    where nameRow.Equals(name)
                    select row)
                {
                    var notes = row["notes"].ToString().Replace("\\n", "\n");

                    releaseNotes = $"\n\nChange log:\n{notes}";
                    OnlineVersion = row["version"].ToString();
                }

                var dbLatestVersion = double.Parse(OnlineVersion);

                if (dbLatestVersion > dbCurrentVersion)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
