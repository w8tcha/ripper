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
        /// Gets or sets the Hidden Options
        /// "Offline Modus" Setting
        /// </summary>
        public bool bOfflMod { get; set; }

        /// <summary>
        /// Gets or sets the "Firefox Exension Enabled" Setting
        /// </summary>
        public bool bExtension { get; set; }

        /// <summary>
        /// Gets or sets the Path to the Text File
        /// </summary>
        public string sTxtFolder { get; set; }

        /// <summary>
        /// Gets or sets the "Clipboard Watch" Setting
        /// </summary>
        public bool bClipBWatch { get; set; }

        /// <summary>
        /// Gets or sets the Create Folder for Every Post Setting
        /// </summary>
        public bool bSubDirs { get; set; }

        /// <summary>
        /// Gets or sets the Create New Folder for Every Thread Setting
        /// </summary>
        public bool bDownInSepFolder { get; set; }

        /// <summary>
        /// Gets or sets the Automatically presses Thank You Button Setting
        /// </summary>
        public bool bAutoThank { get; set; }

        /// <summary>
        /// Gets or sets the Save PostIDs for Checking Setting
        /// </summary>
        public bool bSavePids { get; set; }

        /// <summary>
        /// Gets or sets the Show Tray PopUps Setting
        /// </summary>
        public bool bShowPopUps { get; set; }

        /// <summary>
        /// Gets or sets the Show Complete all Downloads PopUp Setting
        /// </summary>
        public bool bShowCompletePopUp { get; set; }

        /// <summary>
        /// "Always on Top" Setting
        /// </summary>
        public bool bTopMost { get; set; }

        /// <summary>
        /// Gets or sets the min. Image Count for Thanks Setting
        /// </summary>
        public int iMinImageCount { get; set; }

        /// <summary>
        /// Gets or sets the Max. Multi. Downloads Setting
        /// </summary>
        public int iThreadLimit { get; set; }

        /// <summary>
        /// Gets or sets the The Downloadfolder Location
        /// </summary>
        public string sDownloadFolder { get; set; }

        /// <summary>
        /// Gets or sets the "Download Options" Setting
        /// </summary>
        public string sDownloadOptions { get; set; }

        /// <summary>
        /// Gets or sets the Current Language Setting
        /// </summary>
        public string sLanguage { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public string sUser { get; set; }

        /// <summary>
        /// Gets or sets the User Password
        /// </summary>
        public string sPass { get; set; }

        /// <summary>
        /// Gets or sets the Window Position Left
        /// </summary>
        public int iWindowLeft { get; set; }

        /// <summary>
        /// Gets or sets the Window Position Top
        /// </summary>
        public int iWindowTop { get; set; }

        /// <summary>
        /// Gets or sets the Window Width
        /// </summary>
        public int iWindowWidth { get; set; }

        /// <summary>
        /// Gets or sets the Window Height
        /// </summary>
        public int iWindowHeight { get; set; }
    }
}
