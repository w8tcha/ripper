// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieManager.cs" company="The Watcher">
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
    /// The cookie manager.
    /// </summary>
    public class CookieManager
    {
        #region Constants and Fields

        /// <summary>
        /// The Cookie Manager instance.
        /// </summary>
        private static CookieManager _instance;

        /// <summary>
        /// The cookie table.
        /// </summary>
        private readonly Hashtable cookieTable;

        /// <summary>
        /// The cookie enumerator
        /// </summary>
        private IDictionaryEnumerator cookieEnumerator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieManager"/> class.
        /// </summary>
        public CookieManager()
        {
            this.cookieTable = new Hashtable();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the CookieManager instance.
        /// </summary>
        /// <returns>
        /// Returns the CookieManager instance
        /// </returns>
        public static CookieManager GetInstance()
        {
            return _instance ?? (_instance = new CookieManager());
        }

        /// <summary>
        /// The delete all cookies.
        /// </summary>
        public void DeleteAllCookies()
        {
            this.ResetCookiePos();

            var cookieKeys = new ArrayList();

            var cookiePair = this.NextCookie();

            while (cookiePair != null)
            {
                cookieKeys.Add(cookiePair.Key);

                cookiePair = this.NextCookie();
            }

            if (cookieKeys.Count <= 0)
            {
                return;
            }

            foreach (object t in cookieKeys)
            {
                this.DeleteCookie((string)t);
            }
        }

        /// <summary>
        /// Deletes the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        public void DeleteCookie(string key)
        {
            if (this.cookieTable.ContainsKey(key))
            {
                this.cookieTable.Remove(key);
            }
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns the cookie
        /// </returns>
        public string GetCookie(string key)
        {
            if (this.cookieTable.ContainsKey(key))
            {
                return (string)this.cookieTable[key];
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the cookie string.
        /// </summary>
        /// <returns>
        /// Returns the cookie string.
        /// </returns>
        public string GetCookieString()
        {
            this.ResetCookiePos();

            var cookies = string.Empty;

            var cookiePair = this.NextCookie();

            while (cookiePair != null)
            {
                cookies += string.Format("{0}={1};", cookiePair.Key, cookiePair.Value);

                cookiePair = this.NextCookie();
            }

            if (cookies.Substring(cookies.Length - 1, 1) == ";")
            {
                cookies = cookies.Substring(0, cookies.Length - 1);
            }

            return cookies;
        }

        /// <summary>
        /// Gets the next the cookie.
        /// </summary>
        /// <returns>
        /// Returns the next cookie.
        /// </returns>
        public CookiePair NextCookie()
        {
            if (this.cookieEnumerator == null)
            {
                this.ResetCookiePos();
            }

            CookiePair newpair = null;

            if (this.cookieEnumerator.MoveNext())
            {
                newpair = new CookiePair
                    {
                       Key = (string)this.cookieEnumerator.Key, Value = (string)this.cookieEnumerator.Value 
                    };
            }

            return newpair;
        }

        /// <summary>
        /// Resets the cookie position.
        /// </summary>
        public void ResetCookiePos()
        {
            if (this.cookieEnumerator == null)
            {
                this.cookieEnumerator = this.cookieTable.GetEnumerator();
            }

            this.cookieEnumerator.Reset();
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetCookie(string key, string value)
        {
            if (this.cookieTable.ContainsKey(key))
            {
                // Just Update
                this.cookieTable[key] = value;
            }
            else
            {
                this.cookieTable.Add(key, value);
            }
        }

        #endregion
    }
}