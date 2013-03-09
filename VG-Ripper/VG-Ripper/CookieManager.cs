// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieManager.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: VG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    #region

    using System.Collections;

    using RiPRipper.Objects;

    #endregion

    /// <summary>
    /// The cookie manager.
    /// </summary>
    public class CookieManager
    {
        #region Constants and Fields

        /// <summary>
        /// The m instance.
        /// </summary>
        public static CookieManager mInstance;

        /// <summary>
        /// The cookie table.
        /// </summary>
        private readonly Hashtable cookieTable;

        /// <summary>
        /// The ckie enumerator.
        /// </summary>
        private IDictionaryEnumerator ckieEnumerator;

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
        /// The get instance.
        /// </summary>
        /// <returns>
        /// The get instance.
        /// </returns>
        public static CookieManager GetInstance()
        {
            return mInstance ?? (mInstance = new CookieManager());
        }

        /// <summary>
        /// The delete all cookies.
        /// </summary>
        public void DeleteAllCookies()
        {
            this.ResetCookiePos();

            ArrayList arrCookieKeys = new ArrayList();

            CookiePair cPair = this.NextCookie();

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
            }
        }

        /// <summary>
        /// The delete cookie.
        /// </summary>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        public void DeleteCookie(string sKey)
        {
            if (this.cookieTable.ContainsKey(sKey))
            {
                this.cookieTable.Remove(sKey);
            }
        }

        /// <summary>
        /// The get cookie.
        /// </summary>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        /// <returns>
        /// The get cookie.
        /// </returns>
        public string GetCookie(string sKey)
        {
            if (this.cookieTable.ContainsKey(sKey))
            {
                return (string)this.cookieTable[sKey];
            }

            return string.Empty;
        }

        /// <summary>
        /// The get cookie string.
        /// </summary>
        /// <returns>
        /// The get cookie string.
        /// </returns>
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
        /// The next cookie.
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
                newpair = new CookiePair
                    {
                       Key = (string)this.ckieEnumerator.Key, Value = (string)this.ckieEnumerator.Value 
                    };
            }

            return newpair;
        }

        /// <summary>
        /// The reset cookie pos.
        /// </summary>
        public void ResetCookiePos()
        {
            if (this.ckieEnumerator == null)
            {
                this.ckieEnumerator = this.cookieTable.GetEnumerator();
            }

            this.ckieEnumerator.Reset();
        }

        /// <summary>
        /// The set cookie.
        /// </summary>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        /// <param name="sValue">
        /// The s value.
        /// </param>
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

        #endregion
    }
}