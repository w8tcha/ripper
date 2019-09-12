// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHelper.cs" company="The Watcher">
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
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Ripper.Core.Objects;

    /// <summary>
    /// Class To Load and Save the Settings
    /// </summary>
    public class SettingsHelper
    {
        /// <summary>
        /// Loads All the  settings.
        /// </summary>
        /// <returns>The Settings Class</returns>
        public static SettingBase LoadSettings()
        {
            var settings = new SettingBase();

            if (File.Exists(Path.Combine(Application.StartupPath, "Settings.xml")))
            {
                var serializer = new XmlSerializer(typeof(SettingBase));
                var textreader = new StreamReader(Path.Combine(Application.StartupPath, "Settings.xml"));

                settings = (SettingBase)serializer.Deserialize(textreader);
                textreader.Close();
            }

            SaveSettings(settings);

            return settings;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="currentSettings">The current settings.</param>
        public static void SaveSettings(SettingBase currentSettings)
        {
            var serializer = new XmlSerializer(typeof(SettingBase));
            var textreader = new StreamWriter(Path.Combine(Application.StartupPath, "Settings.xml"));

            serializer.Serialize(textreader, currentSettings);
            textreader.Close();
        }        
    }
}