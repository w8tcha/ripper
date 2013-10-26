// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstanceController.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //   //  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper
{
    using System.Windows.Forms;

    using Microsoft.VisualBasic.ApplicationServices;

    /// <summary>
    /// Single Instance Controller
    /// </summary>
    public class SingleInstanceController : WindowsFormsApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstanceController"/> class.
        /// </summary>
        public SingleInstanceController()
        {
            // Set whether the application is single instance
            this.IsSingleInstance = true;

            this.StartupNextInstance += SingleInstanceController_StartupNextInstance;

            this.Startup += SingleInstanceController_Startup;
        }

        /// <summary>
        /// When overridden in a derived class, allows a designer to emit code that configures the splash screen and main form.
        /// </summary>
        protected override void OnCreateMainForm()
        {
            // Instantiate your main application form
            this.MainForm = new MainForm();
        }

        /// <summary>
        /// Handles the Startup event of the SingleInstanceController control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualBasic.ApplicationServices.StartupEventArgs"/> instance containing the event data.</param>
        private static void SingleInstanceController_Startup(object sender, StartupEventArgs e)
        {
            if (e.CommandLine.Count <= 0 || string.IsNullOrEmpty(e.CommandLine[0]))
            {
                return;
            }
            
            var extractedUrl = Utility.ProccesArguments(e.CommandLine[0]);
                
            if (!string.IsNullOrEmpty(extractedUrl))
            {
                Clipboard.SetText(extractedUrl);
            }
        }

        /// <summary>
        /// Handles the StartupNextInstance event of the this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs"/> instance containing the event data.</param>
        private static void SingleInstanceController_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            if (e.CommandLine.Count <= 0 || string.IsNullOrEmpty(e.CommandLine[0]))
            {
                return;
            }

            var extractedUrl = Utility.ProccesArguments(e.CommandLine[0]);

            if (!string.IsNullOrEmpty(extractedUrl))
            {
                Clipboard.SetText(extractedUrl);
            }
        }        
    }
}