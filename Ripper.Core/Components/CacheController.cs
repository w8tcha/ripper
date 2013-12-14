// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheController.cs" company="The Watcher">
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
    #region

    using System.Collections;

    using Ripper.Core.Objects;

    #endregion

    /// <summary>
    ///   Cache Controller Class.
    /// </summary>
    public class CacheController
    {
        /// <summary>
        /// The CacheController Instance
        /// </summary>
        private static CacheController _instance;

        /// <summary>
        /// The Event Table
        /// </summary>
        private Hashtable eventTable;

        /// <summary>
        /// Prevents a default instance of the <see cref="CacheController"/> class from being created.
        /// </summary>
        private CacheController()
        {
            this.eventTable = new Hashtable();
            this.UserSettings = new SettingBase();
        }

        /// <summary>
        /// Gets or sets the User Settings
        /// </summary>
        public SettingBase UserSettings { get; set; }

        /// <summary>
        ///   Gets or sets the Universal string, Last pic Race conditions happen a lot on this string, 
        ///   but it's function is non-critical enough to ignore those races.
        /// </summary>
        public string LastPic { get; set; }

        /// <summary>
        /// Gets or sets the event table.
        /// </summary>
        /// <value>
        /// The event table.
        /// </value>
        public Hashtable EventTable
        {
            get
            {
                return this.eventTable;
            }

            set
            {
                this.eventTable = value;
            }
        }

        #region Public Methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>
        /// Returns the instance.
        /// </returns>
        public static CacheController Instance()
        {
            return _instance ?? (_instance = new CacheController());
        }
        
        /// <summary>
        /// Erase the Event Table
        /// </summary>
        public void EraseEventTable()
        {
            this.eventTable.Clear();
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>
        /// Returns the Object
        /// </returns>
        public CacheObject GetObject(string url)
        {
            if (this.eventTable.ContainsKey(url))
            {
                return (CacheObject)this.eventTable[url];
            }

            return null;
        }
        #endregion
    }
}