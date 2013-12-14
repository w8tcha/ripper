// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieManager.cs" company="The Watcher">
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
    using System.Collections;
    using Ripper.Objects;

    /// <summary>
    /// Class for handling Cookies
    /// </summary>
    public class CookieManager
    {
        /// <summary>
        /// The Cookie Table
        /// </summary>
        private readonly Hashtable cookieTable;

        private IDictionaryEnumerator ckieEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieManager"/> class.
        /// </summary>
        public CookieManager()
        {
            this.cookieTable = new Hashtable();
        }

        /// <summary>
        /// Gets or sets the Cookie Manager Instance
        /// </summary>
        public static CookieManager Instance { get; set; }

        /// <summary>
        /// Gets the Cookie Manager instance.
        /// </summary>
        /// <returns>
        /// The cookie manager instance
        /// </returns>
        public static CookieManager GetInstance()
        {
            return Instance ?? (Instance = new CookieManager());
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="sKey">The s key.</param>
        /// <param name="sValue">The s value.</param>
        public void SetCookie(string sKey, string sValue)
        {
            if (this.cookieTable.ContainsKey(sKey))
            {
                // Just Update
                this.cookieTable[sKey] = sValue;
            }
            else
            {
                this.cookieTable.Add(sKey, sValue);
            }
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the Cookie</returns>
        public string GetCookie(string key)
        {
            return this.cookieTable.ContainsKey(key) ? (string)this.cookieTable[key] : string.Empty;
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
        /// Deletes all cookies.
        /// </summary>
        public void DeleteAllCookies()
        {
            /*this.ResetCookiePos();

            ArrayList arrCookieKeys = new ArrayList();

            CookiePair cPair = NextCookie();

            while (cPair != null)
            {
                arrCookieKeys.Add(cPair.Key);

                cPair = this.NextCookie();
            }

            if (arrCookieKeys.Count <= 0)
            {
                return;
            }

            foreach (object t in arrCookieKeys)
            {
                this.DeleteCookie((string)t);
            }*
             */

            this.cookieTable.Clear();
        }

        /// <summary>
        /// Gets the cookie string.
        /// </summary>
        /// <returns>Returns the Cookie String</returns>
        public string GetCookieString()
        {
            this.ResetCookiePos();

            string sCookies = string.Empty;

            CookiePair cPair = this.NextCookie();

            while (cPair != null)
            {
                sCookies += string.Format("{0}={1};", cPair.Key, cPair.Value);

                cPair = this.NextCookie();
            }

            if (sCookies.Substring(sCookies.Length - 1, 1) == ";")
            {
                sCookies = sCookies.Substring(0, sCookies.Length - 1);
            }

            return sCookies;
        }

        /// <summary>
        /// Nexts the cookie.
        /// </summary>
        /// <returns>
        /// The next cookie.
        /// </returns>
        public CookiePair NextCookie()
        {
            if (this.ckieEnumerator == null)
            {
                this.ResetCookiePos();
            }

            CookiePair newpair = null;

            if (this.ckieEnumerator.MoveNext())
            {
                newpair = new CookiePair { Key = (string)this.ckieEnumerator.Key, Value = (string)this.ckieEnumerator.Value };
            }

            return newpair;
        }

        /// <summary>
        /// Resets the cookie pos.
        /// </summary>
        public void ResetCookiePos()
        {
            if (this.ckieEnumerator == null)
            {
                this.ckieEnumerator = this.cookieTable.GetEnumerator();
            }

            this.ckieEnumerator.Reset();
        }
    }
}