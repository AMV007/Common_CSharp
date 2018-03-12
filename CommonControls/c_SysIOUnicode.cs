using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;
using CommonControls.Win32Api;
//using log4net;

namespace CommonControls
{
    public class c_SysIOUnicode
    {
        /* 
All sample code is provided by the Inge Henriksen for illustrative purposes only. These examples have not been thoroughly tested under all conditions. Inge Henriksen, therefore, cannot guarantee or imply reliability, serviceability, or function of these programs.
 */



        /// <summary>
        /// Class for communicating with the Windows kernel library for low-level disk access.
        /// The main purpose of this class is to allow for longer file paths than System.IO.File,
        /// this class handles file paths up to 32,767 characters. 
        /// Note: Always be careful when accessing this class from a managed multi-threaded application
        /// as the unmanaged Windows kernel is different, this "crash" causes application unstability 
        /// if not handled properly.
        /// TODO: Look into if there are any significant gains on 64-bit systems using another kind of 
        /// core component than kernel32.dll.
        /// </summary>        

        #region DLLImport's
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool CreateDirectoryW(string lpPathName, IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFileW(string lpFileName, uint dwDesiredAccess,
                                              uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                              uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint SetFilePointer(SafeFileHandle hFile, long lDistanceToMove, IntPtr lpDistanceToMoveHigh, Win32Api.kernel32.F_PtrFILE_ MoveType);

        #endregion

        // uint GetMode( FileMode mode )
        // Converts the filemode constant to win32 constant
        #region GetMode
        private uint GetMode(FileMode mode)
        {
            //Log.Debug("Get Win32 file mode");
            uint umode = 0;
            switch (mode)
            {
                case FileMode.CreateNew:
                    umode = F_Creation.CREATE_NEW;
                    break;
                case FileMode.Create:
                    umode = F_Creation.CREATE_ALWAYS;
                    break;
                case FileMode.Append:
                    umode = F_Creation.OPEN_ALWAYS;
                    break;
                case FileMode.Open:
                    umode = F_Creation.OPEN_EXISTING;
                    break;
                case FileMode.OpenOrCreate:
                    umode = F_Creation.OPEN_ALWAYS;
                    break;
                case FileMode.Truncate:
                    umode = F_Creation.TRUNCATE_EXISTING;
                    break;
            }
            return umode;
        }
        #endregion

        // uint GetAccess(FileAccess access)
        // Converts the FileAccess constant to win32 constant
        #region GetAccess
        private uint GetAccess(FileAccess access)
        {
            //Log.Debug("Get Win32 file access");
            uint uaccess = 0;
            switch (access)
            {
                case FileAccess.Read:
                    uaccess = F_GENERIC_.READ;
                    break;
                case FileAccess.ReadWrite:
                    uaccess = F_GENERIC_.READ_WRITE;
                    break;
                case FileAccess.Write:
                    uaccess = F_GENERIC_.WRITE;
                    break;
            }
            return uaccess;
        }
        #endregion

        // uint GetShare(FileShare share)
        // Converts the FileShare constant to win32 constant
        #region GetShare
        private uint GetShare(FileShare share)
        {
            //Log.Debug("Get Win32 file share");
            uint ushare = 0;
            switch (share)
            {
                case FileShare.Read:
                    ushare = F_FILE_SHARE_.READ;
                    break;
                case FileShare.ReadWrite:
                    ushare = F_FILE_SHARE_.READ_WRITE;
                    break;
                case FileShare.Write:
                    ushare = F_FILE_SHARE_.WRITE;
                    break;
                case FileShare.Delete:
                    ushare = F_FILE_SHARE_.DELETE;
                    break;
                case FileShare.None:
                    ushare = F_FILE_SHARE_.NONE;
                    break;

            }
            return ushare;
        }
        #endregion

        /*
        public bool Move(string existingFile, string newFile)
        {
            //Log.Debug(String.Format("Rename the file \"{0}\" to \"{1}\"", existingFile, newFile));
            return Win32File.MoveFile(existingFile, newFile);
        }
         * */

        public bool CreateDirectory(string filepath)
        {
            //Log.Debug(String.Format("Create the directory \"{0}\"", filepath));
            // If file path is disk file path then prepend it with \\?\
            // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
            if (filepath.StartsWith(@"\\"))
                filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
            else
                filepath = @"\\?\" + filepath;
            return CreateDirectoryW(filepath, IntPtr.Zero);
        }

        public FileStream Open(string filepath, FileMode mode, uint uaccess)
        {
            //Log.Debug(String.Format("Open the file \"{0}\"", filepath));

            //opened in the specified mode and path, with read/write access and not shared
            FileStream fs = null;
            uint umode = GetMode(mode);
            uint ushare = 0;    // Not shared
            if (mode == FileMode.Append) uaccess = F_SecurityFILE_.APPEND_DATA;

            // If file path is disk file path then prepend it with \\?\
            // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
            if (filepath.StartsWith(@"\\"))
                filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
            else filepath = @"\\?\" + filepath;

            SafeFileHandle sh = CreateFileW(filepath, uaccess, ushare, IntPtr.Zero, umode, (uint)kernel32.FileAttributes.Normal, IntPtr.Zero);
            int iError = Marshal.GetLastWin32Error();
            if ((iError > 0 && !(mode == FileMode.Append && iError == (int)Win32Api.winerror.ERROR_ALREADY_EXISTS)) || sh.IsInvalid)
            {
                //Log.Error(String.Format("Error opening file; Win32 Error: {0}", iError));
                throw new Exception("Error opening file; Win32 Error:" + iError);
            }
            else
            {
                fs = new FileStream(sh, FileAccess.ReadWrite);
            }

            // if opened in append mode
            if (mode == FileMode.Append)
            {
                if (!sh.IsInvalid)
                {
                    SetFilePointer(sh, 0, IntPtr.Zero, kernel32.F_PtrFILE_.END);
                }
            }

            //Log.Debug(String.Format("The file \"{0}\" is now open", filepath));
            return fs;
        }

        public FileStream Open(string filepath, FileMode mode, FileAccess access)
        {
            //Log.Debug(String.Format("Open the file \"{0}\"", filepath));

            //opened in the specified mode and access and not shared
            FileStream fs = null;
            uint umode = GetMode(mode);
            uint uaccess = GetAccess(access);
            uint ushare = 0;    // Exclusive lock of the file

            if (mode == FileMode.Append) uaccess = (uint)F_SecurityFILE_.APPEND_DATA;

            // If file path is disk file path then prepend it with \\?\
            // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
            if (filepath.StartsWith(@"\\"))
                filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
            else
                filepath = @"\\?\" + filepath;

            SafeFileHandle sh = CreateFileW(filepath, uaccess, ushare, IntPtr.Zero, umode, (uint)kernel32.FileAttributes.Normal, IntPtr.Zero);
            int iError = Marshal.GetLastWin32Error();
            if ((iError > 0 && !(mode == FileMode.Append && iError != (int)Win32Api.winerror.ERROR_ALREADY_EXISTS)) || sh.IsInvalid)
            {
                //Log.Error(String.Format("Error opening file; Win32 Error: {0}", iError));
                throw new Exception("Error opening file; Win32 Error:" + iError);
            }
            else
            {
                fs = new FileStream(sh, access);
            }

            // if opened in append mode
            if (mode == FileMode.Append)
            {
                if (!sh.IsInvalid)
                {
                    SetFilePointer(sh, 0, IntPtr.Zero, kernel32.F_PtrFILE_.END);
                }
            }

            //Log.Debug(String.Format("The file \"{0}\" is now open", filepath));
            return fs;
        }

        public FileStream Open(string filepath, FileMode mode, FileAccess access, FileShare share)
        {
            //opened in the specified mode , access and  share
            FileStream fs = null;
            uint umode = GetMode(mode);
            uint uaccess = GetAccess(access);
            uint ushare = GetShare(share);
            if (mode == FileMode.Append) uaccess = F_SecurityFILE_.APPEND_DATA;

            // If file path is disk file path then prepend it with \\?\
            // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
            if (filepath.StartsWith(@"\\"))
                filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
            else
                filepath = @"\\?\" + filepath;
            SafeFileHandle sh = CreateFileW(filepath, uaccess, ushare, IntPtr.Zero, umode, (uint)kernel32.FileAttributes.Normal, IntPtr.Zero);
            int iError = Marshal.GetLastWin32Error();
            if ((iError > 0 && !(mode == FileMode.Append && iError != (int)Win32Api.winerror.ERROR_ALREADY_EXISTS)) || sh.IsInvalid)
            {
                throw new Exception("Error opening file Win32 Error:" + iError);
            }
            else
            {
                fs = new FileStream(sh, access);
            }
            // if opened in append mode
            if (mode == FileMode.Append)
            {
                if (!sh.IsInvalid)
                {
                    SetFilePointer(sh, 0, IntPtr.Zero, kernel32.F_PtrFILE_.END);
                }
            }
            return fs;
        }

        public FileStream OpenRead(string filepath)
        {
            //Log.Debug(String.Format("Open the file \"{0}\"", filepath));
            // Open readonly file mode open(String, FileMode.Open, FileAccess.Read, FileShare.Read)
            return Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public FileStream OpenWrite(string filepath)
        {
            //Log.Debug(String.Format("Open the file \"{0}\" for writing", filepath));
            //open writable open(String, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None).
            return Open(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public bool Delete(string filepath)
        {
            //Log.Debug(String.Format("Delete the file \"{0}\"", filepath));
            // If file path is disk file path then prepend it with \\?\
            // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
            if (filepath.StartsWith(@"\\"))
                filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
            else
                filepath = @"\\?\" + filepath;
            //Log.Debug(String.Format("The file \"{0}\" has been deleted", filepath));
            return DeleteFile(filepath);
        }

        /// <summary>
        /// Operates just like GetFileSystemInfos except it is recursive, operates on more than
        /// one FileSystemInfo, ensures no paths are duplicateed, and includes the original
        /// items in the return.
        /// </summary>
        /// <param name="files">A collection of files and/or directories to expand.</param>
        /// <returns>A Dictionary of all file, directories, and subfiles and subdirectories of 
        /// those directories and subdirectories.  The key is the fullpath, and the value
        /// is the FileSystemInfo.</returns>
        public static Dictionary<string, FileSystemInfo> ExpandFileSystemInfos(FileSystemInfo[] files)
        {
            Dictionary<string, FileSystemInfo> resultingFiles = new Dictionary<string, FileSystemInfo>();

            //GetFileSystemInfosRecursively will expand everything, except it may contain duplicates
            foreach (FileSystemInfo file in GetFileSystemInfosRecursively(files))
            {//so we just go through adding them to the Dictionary
                if (!resultingFiles.ContainsKey(file.FullName))
                {
                    resultingFiles.Add(file.FullName, file);
                }
            }
            return resultingFiles;
        }

        //helper method for ExpandFileSystemInfos
        private static List<FileSystemInfo> GetFileSystemInfosRecursively(FileSystemInfo[] files)
        {
            List<FileSystemInfo> resultingFiles = new List<FileSystemInfo>();
            foreach (FileSystemInfo file in files)
            {
                if (file is DirectoryInfo)
                {
                    //get its sub items and pass to another function to process
                    DirectoryInfo dir = (DirectoryInfo)file;

                    //recursive call, passing in subdirectories and files of current directory.  
                    //The result returned will be all subdirectories and files nested within the current
                    //dir.  So we add them to our collection of results for each directory we encounter.
                    resultingFiles.AddRange(GetFileSystemInfosRecursively(dir.GetFileSystemInfos()));
                }
                //else do nothing, it is a file we already know about
            }
            //resultingFiles now contains all sub items
            //It does not include the directories and files in files however, so we add them as well
            resultingFiles.AddRange(files);

            return resultingFiles;
        }

    }
}
