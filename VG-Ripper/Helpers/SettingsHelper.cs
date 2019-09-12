// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHelper.cs" company="The Watcher">
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
    using System.Collections.Generic;
    using System.Configuration;
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
        /// The configuration
        /// </summary>
        private static readonly Configuration Conf =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        /// <summary>
        /// The app
        /// </summary>
        private static readonly AppSettingsSection App = (AppSettingsSection)Conf.Sections["appSettings"];

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="currentSettings">
        /// The current settings.
        /// </param>
        /// <returns>
        /// The load settings.
        /// </returns>
        public static SettingBase LoadSettings(SettingBase currentSettings)
        {
            var serializer = new XmlSerializer(typeof(List<SettingBase>));
            var textreader = new StreamReader(Path.Combine(Application.StartupPath, "Settings.xml"));

            var settings = (SettingBase)serializer.Deserialize(textreader);
            textreader.Close();

            return settings;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="currentSettings">The current settings.</param>
        public static void SaveSettings(SettingBase currentSettings)
        {
            var serializer = new XmlSerializer(typeof(List<SettingBase>));
            var textreader = new StreamWriter(Path.Combine(Application.StartupPath, "Settings.xml"));

            serializer.Serialize(textreader, currentSettings);
            textreader.Close();
        }

        /// <summary>
        /// Loads a Setting from the App.config
        /// </summary>
        /// <param name="sKey">Setting name</param>
        /// <returns>Setting value</returns>
        public static string LoadSetting(string sKey)
        {
            var setting = App.Settings[sKey].Value;

            return setting;
        }

        /// <summary>
        /// Saves a setting to the App.config
        /// </summary>
        /// <param name="key">Setting Name</param>
        /// <param name="value">Setting Value</param>
        public static void SaveSetting(string key, string value)
        {
            if (App.Settings[key] != null)
            {
                App.Settings.Remove(key);
            }

            App.Settings.Add(key, value);

            App.SectionInformation.ForceSave = true;
            Conf.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Delete a Setting
        /// </summary>
        /// <param name="sKey">Setting Name</param>
        public static void DeleteSetting(string sKey)
        {
            if (App.Settings[sKey] != null)
            {
                App.Settings.Remove(sKey);
            }

            App.SectionInformation.ForceSave = true;
            Conf.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}