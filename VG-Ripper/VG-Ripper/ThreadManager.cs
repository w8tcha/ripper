//////////////////////////////////////////////////////////////////////////
// Code Named: VG-Ripper
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

namespace Ripper
{
    using System;
    using System.Collections;
    using System.Threading;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadManager"/> class.
        /// </summary>
        public ThreadManager()
        {
            this.threadTable = new Hashtable();
            this.SetThreadThreshHold(3);
        }

        /// <summary>
        /// Gets the thread count.
        /// </summary>
        /// <returns></returns>
        public int GetThreadCount()
        {
            return this.threadTable.Count;
        }

        /// <summary>
        /// Gets the thread thresh hold.
        /// </summary>
        /// <returns></returns>
        public int GetThreadThreshHold()
        {
            return this.mThreshHold;
        }

        public void SetThreadThreshHold(int iTHold)
        {
            this.mThreshHold = iTHold;
        }

        /// <summary>
        /// Launches the thread.
        /// </summary>
        /// <param name="threadId">The thread unique identifier.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public bool LaunchThread(string threadId, ThreadStart start)
        {
            if (this.threadTable.Count >= this.mThreshHold || this.threadTable.ContainsKey(threadId))
            {
                return false;
            }

            Thread threadGet = new Thread(start) { IsBackground = true };

            this.threadTable.Add(threadId, threadGet);

            threadGet.Start();

            return true;
        }

        /// <summary>
        /// Determines whether [is system ready for new thread].
        /// </summary>
        /// <returns></returns>
        public bool IsSystemReadyForNewThread()
        {
            if (this.threadTable.Count >= this.mThreshHold)
            {
                return false;
            }

            return !this.bSuspend;
        }

        /// <summary>
        /// Removes the threadby id.
        /// </summary>
        /// <param name="threadId">The thread id.</param>
        public void RemoveThreadbyId(string threadId)
        {
            if (this.threadTable.ContainsKey(threadId))
            {
                this.threadTable.Remove(threadId);
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