//////////////////////////////////////////////////////////////////////////
// Code Named: RiP-Ripper
// Function  : Extracts Images posted on RiP forums and attempts to fetch
//			   them to disk.
//
// This software is licensed under the MIT license. See license.txt for
// details.
// 
// Copyright (c) The Watcher
// Partial Rights Reserved.
// 
//////////////////////////////////////////////////////////////////////////
// This file is part of the RiP Ripper project base.

using System.Collections;
using System.Threading;
using System;

namespace RiPRipper
{
    /// <summary>
    /// Custom thread controller.
    /// Bad architecture, poor design. But it works and it works better than the more
    /// 'complex' models featuring thousands of lines of coding. So whatever..
    /// </summary>
    public class ThreadManager
    {

        private int mThreshHold;

        private readonly Hashtable threadTable;

        private bool bSuspend;

        public static ThreadManager mInstance;

        public static ThreadManager GetInstance()
        {
            return mInstance ?? (mInstance = new ThreadManager());
        }

        public ThreadManager()
        {
            threadTable = new Hashtable();
            SetThreadThreshHold(3);
        }

        public int GetThreadCount()
        {
            return threadTable.Count;
        }

        public int GetThreadThreshHold()
        {
            return mThreshHold;
        }

        public void SetThreadThreshHold(int iTHold)
        {
            mThreshHold = iTHold;
        }

        public bool LaunchThread(string threadId, ThreadStart start)
        {
            if (threadTable.Count >= mThreshHold || threadTable.ContainsKey(threadId))
            {
                return false;
            }

            Thread threadGet = new Thread(start) { IsBackground = true };

            threadTable.Add(threadId, threadGet);

            threadGet.Start();
            return true;
        }

        public bool IsSystemReadyForNewThread()
        {
            if (threadTable.Count >= mThreshHold) return false;

            if (bSuspend) return false;

            return true;
        }

        public void RemoveThreadbyId(string threadId)
        {
            if (threadTable.ContainsKey(threadId))
            {
                threadTable.Remove(threadId);
            }
        }

        public void DismantleAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = threadTable.GetEnumerator();

                while (thrdEnumerator.MoveNext())
                {
                    if (((Thread)thrdEnumerator.Value).IsAlive)
                    {
                        Monitor.Exit(thrdEnumerator.Value);
                    }
                }
            }
            catch (Exception)
            {
                threadTable.Clear();
            }
            finally
            {
                bSuspend = false;
                threadTable.Clear();
            }
        }

        public void HoldAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = threadTable.GetEnumerator();

                while (thrdEnumerator.MoveNext())
                {
                    if (((Thread)thrdEnumerator.Value).IsAlive)
                    {
                        Monitor.Enter(thrdEnumerator.Value);
                    }
                }
            }
            finally
            {
                bSuspend = true;
            }
        }

        public void ResumeAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = threadTable.GetEnumerator();
                while (thrdEnumerator.MoveNext())
                {
                    Monitor.Exit(thrdEnumerator.Value);
                }
            }
            finally
            {
                bSuspend = false;
            }
        }
    }
}