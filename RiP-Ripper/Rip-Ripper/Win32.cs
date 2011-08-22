// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Win32.cs" company="The Watcher">
//   Copyright (c) The Watcher
// </copyright>
// <summary>
//   Defines the Win32 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#if (!RIPRIPPERX)
namespace RiPRipper
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Win32 Class Interop
    /// </summary>
    public class Win32
    {
        /// <summary>
        /// Set Clipboard Viewer
        /// </summary>
        /// <param name="hWndNewViewer">
        /// The h wnd new viewer.
        /// </param>
        /// <returns>
        /// The set clipboard viewer.
        /// </returns>
        [DllImport("User32.dll")]
        public static extern int SetClipboardViewer(int hWndNewViewer);

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="wMsg">
        /// The w msg.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The send message.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
    }
}
#endif
