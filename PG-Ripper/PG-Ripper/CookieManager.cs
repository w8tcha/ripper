//////////////////////////////////////////////////////////////////////////
// Code Named: PG-Ripper
// Function  : Extracts Images posted on PG forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher 
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the PG-Ripper project base.

using System.Collections;

namespace PGRipper
{
    using PGRipper.Objects;

    public class CookieManager
	{
		private readonly Hashtable cookieTable;

		IDictionaryEnumerator ckieEnumerator;

		public static CookieManager mInstance;
		public static CookieManager GetInstance()
		{
		    return mInstance ?? (mInstance = new CookieManager());
		}

        public CookieManager()
		{
			cookieTable = new Hashtable();
		}

		public void SetCookie(string sKey, string sValue)
		{
			if ( cookieTable.ContainsKey(sKey) )
			{
				// Just Update
				cookieTable[sKey] = sValue;
			}
			else
			{
				cookieTable.Add(sKey, sValue);
			}
		}

		public string GetCookie(string sKey)
		{
			if ( cookieTable.ContainsKey(sKey) )
				return (string)cookieTable[sKey];

			return string.Empty;
		}

		public void DeleteCookie(string sKey)
		{
            if (cookieTable.ContainsKey(sKey))
            {
                cookieTable.Remove(sKey);
            }
		}

		public void DeleteAllCookies()
		{
			ResetCookiePos();

		    ArrayList arrCookieKeys = new ArrayList();

			CookiePairS cPair = NextCookie();

			while (cPair != null)
			{
				arrCookieKeys.Add( cPair.Key );

				cPair = NextCookie();
			}

            if (arrCookieKeys.Count <= 0)
            {
                return;
            }

		    foreach (object t in arrCookieKeys)
            {
                DeleteCookie((string) t);
            }
		}

		public string GetCookieString()
		{
			ResetCookiePos();

		    string sCookies = string.Empty;

			CookiePairS cPair = NextCookie();

            while (cPair != null)
            {
                sCookies += string.Format("{0}={1};", cPair.Key, cPair.Value);

                cPair = NextCookie();
            }

			if ( sCookies.Substring( sCookies.Length-1, 1 ) == ";" )
				sCookies = sCookies.Substring(0, sCookies.Length-1);

			return sCookies;
		}

		public CookiePairS NextCookie()
		{
			if (ckieEnumerator == null)
				ResetCookiePos();

			CookiePairS newpair = null;

			if (ckieEnumerator.MoveNext())
			{
				newpair = new CookiePairS {Key = (string) ckieEnumerator.Key, Value = (string) ckieEnumerator.Value};
			}
				
			return newpair;
		}

		public void ResetCookiePos()
		{
            if (ckieEnumerator == null)
            {
                ckieEnumerator = cookieTable.GetEnumerator();
            }

		    ckieEnumerator.Reset();
		}

	}
}
