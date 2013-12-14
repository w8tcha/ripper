// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadManager.cs" company="The Watcher">
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadManager"/> class.
        /// </summary>
        public ThreadManager()
        {
            this.threadTable = new Hashtable();
            this.SetThreadThreshHold(3);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The Thread Manager Instance</returns>
        public static ThreadManager GetInstance()
        {
            return mInstance ?? (mInstance = new ThreadManager());
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

        /// <summary>
        /// Sets the thread thresh hold.
        /// </summary>
        /// <param name="iTHold">The i T hold.</param>
        public void SetThreadThreshHold(int iTHold)
        {
            this.mThreshHold = iTHold;
        }

        /// <summary>
        /// Launches the thread.
        /// </summary>
        /// <param name="threadId">The thread id.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public bool LaunchThread(string threadId, ThreadStart start)
        {
            if (this.threadTable.Count >= this.mThreshHold || this.threadTable.ContainsKey(threadId))
            {
                return false;
            }

            Thread threadGet = new Thread(start);
            this.threadTable.Add(threadId, threadGet);
            threadGet.Start();
            return true;
        }

        /// <summary>
        /// Determines whether [is system ready for new thread].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is system ready for new thread]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSystemReadyForNewThread()
        {
            if (this.threadTable.Count >= this.mThreshHold)
            {
                return false;
            }

            return !this.bSuspend;
        }


        /// <summary>
        /// Gets the threadby id.
        /// </summary>
        /// <param name="threadId">The thread id.</param>
        /// <returns></returns>
        public Thread GetThreadbyId(string threadId)
        {
            if (this.threadTable.ContainsKey(threadId))
            {
                return (Thread)this.threadTable[threadId];
            }

            return null;
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

        /// <summary>
        /// Dismantles all threads.
        /// </summary>
        public void DismantleAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = this.threadTable.GetEnumerator();

                while (thrdEnumerator.MoveNext())
                {
                    if (((Thread)thrdEnumerator.Value).IsAlive)
                    {
                        Monitor.Exit(thrdEnumerator.Value);
                    }
                }
            }
            catch (System.Exception)
            {
                this.threadTable.Clear();
            }
            finally
            {
                this.bSuspend = false;
                this.threadTable.Clear();
            }
        }

        /// <summary>
        /// Holds all threads.
        /// </summary>
        public void HoldAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = this.threadTable.GetEnumerator();

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
                this.bSuspend = true;
            }
        }

        /// <summary>
        /// Resumes all threads.
        /// </summary>
        public void ResumeAllThreads()
        {
            try
            {
                IDictionaryEnumerator thrdEnumerator = this.threadTable.GetEnumerator();
                while (thrdEnumerator.MoveNext())
                {
                    Monitor.Exit(thrdEnumerator.Value);
                }
            }
            finally
            {
                this.bSuspend = false;
            }
        }
    }
}