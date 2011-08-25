//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Base class for storage bound items in the Shell Namespace.
    /// Storage bound items include files and folders.
    /// </summary>
    abstract public class ShellFileSystemObject : ShellObject
    {
        #region Internal Properties

        /// <summary>
        /// Return the native ShellFileSystemObject object as newer IShellItem2
        /// </summary>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">If the native object cannot be created.
        /// The ErrorCode member will contain the external error code.</exception>
        internal override IShellItem2 NativeShellItem2
        {
            get
            {
                if (nativeShellItem == null)
                {
                    Guid guid = new Guid(ShellIIDGuid.IShellItem2);
                    int retCode = ShellNativeMethods.SHCreateItemFromParsingName(Path, IntPtr.Zero, ref guid, out nativeShellItem);

                    if (nativeShellItem == null || !CoreErrorHelper.Succeeded(retCode))
                    {
                        throw new  ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));

                    }
                }
                return nativeShellItem;
            }
        }

        /// <summary>
        /// Return the native ShellFileSystemObject object
        /// </summary>
        internal override IShellItem NativeShellItem
        {
            get
            {
                return NativeShellItem2;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a new ShellFolder or ShellFileSystemObject for an existing path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public new ShellFileSystemObject FromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (File.Exists(path))
            {
                return ShellFile.FromFilePath(path);
            }
            else if (Directory.Exists(path))
            {
                return ShellFolder.FromFolderPath(path);
            }
            else
            {
                throw new FileNotFoundException("This Path is not Valid", path);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Creates a ShellFileSystemObject given a native IShellItem interface
        /// </summary>
        /// <param name="nativeShellItem"></param>
        /// <returns>A newly constructed ShellFileSystemObject object</returns>
        internal new static ShellFileSystemObject FromShellItem(IShellItem nativeShellItem)
        {
            string path = null;

            // Get the full system path from the native IShellItem
            path = ShellHelper.GetParsingName(nativeShellItem);

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Cannot create a ShellFileSystemObject from", "nativeShellItem");

            ShellFileSystemObject item = null;

            if (File.Exists(path))
            {
                item = new ShellFile();
            }
            else if (Directory.Exists(path))
            {
                item = new ShellFolder();
            }
            else if (path.StartsWith("::")) // It's a special folder, we'll use it
            {
                //TODO: Construct a special type for registered non-folder Shell items
                item = new ShellFile();
            }
            else
            {
                return null; // There aren't any other options left
            }

            item.Path = path;                
            item.nativeShellItem = (IShellItem2)nativeShellItem;

            return item;        
        }

        #endregion
    }
}
