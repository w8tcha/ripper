// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadManager.cs" company="The Watcher">
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
        /// <summary>
        /// The thread table
        /// </summary>
        private readonly Hashtable threadTable;

        /// <summary>
        /// The thresh hold
        /// </summary>
        private int threshHold;

        /// <summary>
        /// The suspend
        /// </summary>
        private bool suspend;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadManager"/> class.
        /// </summary>
        public ThreadManager()
        {
            this.threadTable = new Hashtable();
            this.SetThreadThreshHold(3);
        }

        /// <summary>
        /// Gets or sets the ThreadManager instance
        /// </summary>
        public static ThreadManager Instance { get; set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>Returns the Thread manager instance</returns>
        public static ThreadManager GetInstance()
        {
            return Instance ?? (Instance = new ThreadManager());
        }

        /// <summary>
        /// Gets the thread count.
        /// </summary>
        /// <returns>Returns the Thread Count</returns>
        public int GetThreadCount()
        {
            return this.threadTable.Count;
        }

        /// <summary>
        /// Gets the thread thresh hold.
        /// </summary>
        /// <returns>Returns the thread thresh hold.</returns>
        public int GetThreadThreshHold()
        {
            return this.threshHold;
        }

        /// <summary>
        /// Sets the thread thresh hold.
        /// </summary>
        /// <param name="threshold">The Threshold.</param>
        public void SetThreadThreshHold(int threshold)
        {
            this.threshHold = threshold;
        }

        /// <summary>
        /// Launches the thread.
        /// </summary>
        /// <param name="threadId">The thread unique identifier.</param>
        /// <param name="start">The start.</param>
        /// <returns>Returns if thread was launched or not</returns>
        public bool LaunchThread(string threadId, ThreadStart start)
        {
            if (this.threadTable.Count >= this.threshHold || this.threadTable.ContainsKey(threadId))
            {
                return false;
            }

            var threadGet = new Thread(start) { IsBackground = true };

            this.threadTable.Add(threadId, threadGet);

            threadGet.Start();

            return true;
        }

        /// <summary>
        /// Determines whether [is system ready for new thread].
        /// </summary>
        /// <returns>Returns if system ready for new thread</returns>
        public bool IsSystemReadyForNewThread()
        {
            if (this.threadTable.Count >= this.threshHold)
            {
                return false;
            }

            return !this.suspend;
        }

        /// <summary>
        /// Removes the thread by id.
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
                var thrdEnumerator = this.threadTable.GetEnumerator();

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
                this.threadTable.Clear();
            }
            finally
            {
                this.suspend = false;
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
                var thrdEnumerator = this.threadTable.GetEnumerator();

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
                this.suspend = true;
            }
        }

        /// <summary>
        /// Resumes all threads.
        /// </summary>
        public void ResumeAllThreads()
        {
            try
            {
                var thrdEnumerator = this.threadTable.GetEnumerator();
                while (thrdEnumerator.MoveNext())
                {
                    Monitor.Exit(thrdEnumerator.Value);
                }
            }
            finally
            {
                this.suspend = false;
            }
        }
    }
}