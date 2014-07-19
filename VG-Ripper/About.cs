// --------------------------------------------------------------------------------------------------------------------
// <copyright file="About.cs" company="The Watcher">
//  Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//  Code Named: VG-Ripper
//  Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    #region

    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    using Ripper.Core.Components;

    #endregion

    /// <summary>
    /// About Dialog Class
    /// </summary>
    public partial class About : Form
    {
        #region Constants and Fields

        /// <summary>
        ///   Current Language Resource
        /// </summary>
        private ResourceManager rm;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            this.LoadSettings();

#if (RIPRIPPER)
            label1.Text = String.Format(
                "Viper Girls Ripper {0}.{1}.{2}{3}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"));
#elif (RIPRIPPERX)
            label1.Text = String.Format(
                "Viper Girls Ripper X {0}.{1}.{2}{3}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"),
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"));
#else
            this.label1.Text = string.Format(
                "Viper Girls Ripper {0}.{1}.{2}{3}", 
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("0"), 
                Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("0"));
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all needed Settings from Config
        /// </summary>
        public void LoadSettings()
        {
            this.TopMost = CacheController.Instance().UserSettings.TopMost;

            switch (CacheController.Instance().UserSettings.Language)
            {
                case "de-DE":
                    this.rm = new ResourceManager("Ripper.Languages.german", Assembly.GetExecutingAssembly());
                    break;
                case "fr-FR":
                    this.rm = new ResourceManager("Ripper.Languages.french", Assembly.GetExecutingAssembly());
                    break;
                case "en-EN":
                    this.rm = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                    break;
                default:
                    this.rm = new ResourceManager("Ripper.Languages.english", Assembly.GetExecutingAssembly());
                    break;
            }

            this.AdjustCulture();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set Form Language
        /// </summary>
        private void AdjustCulture()
        {
            this.btnClose.Text = this.rm.GetString("btnClose");
        }

        /// <summary>
        /// Close About Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open Ripper Website in Default Web browser
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://Ripper.CodePlex.com/");
        }

        #endregion
    }
}