// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingBase.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper.Objects
{
    /// <summary>
    /// The Settings Base
    /// </summary>
    public class SettingBase : object
    {
        /// <summary>
        /// Gets or sets a value indicating whether Hidden Options
        /// "Offline Modus" Setting
        /// </summary>
        public bool OfflMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Firefox Exension Enabled" Setting
        /// </summary>
        public bool Extension { get; set; }

        /// <summary>
        /// Gets or sets the Path to the Text File
        /// </summary>
        public string TxtFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Clipboard Watch" Setting
        /// </summary>
        public bool ClipBWatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Create Folder for Every Post Setting
        /// </summary>
        public bool SubDirs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Create New Folder for Every Thread Setting
        /// </summary>
        public bool DownInSepFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Automatically presses Thank You Button Setting
        /// </summary>
        public bool AutoThank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Save PostIDs for Checking Setting
        /// </summary>
        public bool SavePids { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Show Tray PopUps Setting
        /// </summary>
        public bool ShowPopUps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Show Complete all Downloads PopUp Setting
        /// </summary>
        public bool ShowCompletePopUp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Always on Top" Setting
        /// </summary>
        public bool TopMost { get; set; }

        /// <summary>
        /// Gets or sets the min. Image Count for Thanks Setting
        /// </summary>
        public int MinImageCount { get; set; }

        /// <summary>
        /// Gets or sets the Max. Multi. Downloads Setting
        /// </summary>
        public int ThreadLimit { get; set; }

        /// <summary>
        /// Gets or sets the The Downloadfolder Location
        /// </summary>
        public string DownloadFolder { get; set; }

        /// <summary>
        /// Gets or sets the "Download Options" Setting
        /// </summary>
        public string DownloadOptions { get; set; }

        /// <summary>
        /// Gets or sets the Current Language Setting
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the User Password
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// Gets or sets the Window Position Left
        /// </summary>
        public int WindowLeft { get; set; }

        /// <summary>
        /// Gets or sets the Window Position Top
        /// </summary>
        public int WindowTop { get; set; }

        /// <summary>
        /// Gets or sets the Window Width
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Gets or sets the Window Height
        /// </summary>
        public int WindowHeight { get; set; }
    }
}
