using System;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;


namespace CommonControls.CommonMethods.LongIO
{
    public class File : c_FileSystemInfo
    {
        public static bool Exists(string path)
        {
            if (path.Length < MAX_PATH) return System.IO.File.Exists(path);
            var attr = Win32Api.kernel32.GetFileAttributesW(GetWin32LongPath(path));
            return (attr != Win32Api.kernel32.INVALID_FILE_ATTRIBUTES && ((attr & Win32Api.kernel32.FILE_ATTRIBUTE_ARCHIVE) == Win32Api.kernel32.FILE_ATTRIBUTE_ARCHIVE));
        }

        public static void Delete(string path)
        {
            if (path.Length < MAX_PATH) System.IO.File.Delete(path);
            else
            {
                bool ok = Win32Api.kernel32.DeleteFileW(GetWin32LongPath(path));
                if (!ok) ThrowWin32Exception();
            }
        }

        public static void AppendAllText(string path, string contents)
        {
            AppendAllText(path, contents, Encoding.Default);
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            if (path.Length < MAX_PATH)
            {
                System.IO.File.AppendAllText(path, contents, encoding);
            }
            else
            {
                var fileHandle = CreateFileForAppend(GetWin32LongPath(path));
                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                {
                    var bytes = encoding.GetBytes(contents);
                    fs.Position = fs.Length;
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }    

        public static void WriteAllText(string path, string contents)
        {
            WriteAllText(path, contents, Encoding.Default);
        }
        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            if (path.Length < MAX_PATH)
            {
                System.IO.File.WriteAllText(path, contents, encoding);
            }
            else
            {
                var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                {
                    var bytes = encoding.GetBytes(contents);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }     

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (path.Length < MAX_PATH)
            {
                System.IO.File.WriteAllBytes(path, bytes);
            }
            else
            {
                var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

    

        public static void Copy(string sourceFileName, string destFileName)
        {
            Copy(sourceFileName, destFileName, false);
        }


        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Copy(sourceFileName, destFileName, overwrite);
            else
            {
                var ok = Win32Api.kernel32.CopyFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName), !overwrite);
                if (!ok) ThrowWin32Exception();
            }
        }


        public static void Move(string sourceFileName, string destFileName)
        {
            if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Move(sourceFileName, destFileName);
            else
            {
                var ok = Win32Api.kernel32.MoveFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName));
                if (!ok) ThrowWin32Exception();
            }
        }

     

        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.Default);
        }

     

        public static string ReadAllText(string path, Encoding encoding)
        {
            if (path.Length < MAX_PATH) { return System.IO.File.ReadAllText(path, encoding); }
            var fileHandle = GetFileHandle(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
            {
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                return encoding.GetString(data);
            }
        }

    
        public static string[] ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.Default);
        }

  
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            if (path.Length < MAX_PATH) { return System.IO.File.ReadAllLines(path, encoding); }
            var fileHandle = GetFileHandle(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
            {
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                var str = encoding.GetString(data);
                if (str.Contains("\r")) return str.Split(new[] { "\r\n" }, StringSplitOptions.None);
                return str.Split('\n');
            }
        }

     
        public static byte[] ReadAllBytes(string path)
        {
            if (path.Length < MAX_PATH) return System.IO.File.ReadAllBytes(path);
            var fileHandle = GetFileHandle(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
            {
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                return data;
            }
        }

    


        #region Helper methods

        private static SafeFileHandle CreateFileForWrite(string filename)
        {
            if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
            SafeFileHandle hfile = Win32Api.kernel32.CreateFile(filename, (int)Win32Api.kernel32.FILE_GENERIC_WRITE, Win32Api.kernel32.FILE_SHARE_NONE, IntPtr.Zero, Win32Api.kernel32.CREATE_ALWAYS, 0, IntPtr.Zero);
            if (hfile.IsInvalid) ThrowWin32Exception();
            return hfile;
        }

        private static SafeFileHandle CreateFileForAppend(string filename)
        {
            if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
            SafeFileHandle hfile = Win32Api.kernel32.CreateFile(filename, (int)Win32Api.kernel32.FILE_GENERIC_WRITE, Win32Api.kernel32.FILE_SHARE_NONE, IntPtr.Zero, Win32Api.kernel32.CREATE_NEW, 0, IntPtr.Zero);
            if (hfile.IsInvalid)
            {
                hfile = Win32Api.kernel32.CreateFile(filename, (int)Win32Api.kernel32.FILE_GENERIC_WRITE, Win32Api.kernel32.FILE_SHARE_NONE, IntPtr.Zero, Win32Api.kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
                if (hfile.IsInvalid) ThrowWin32Exception();
            }
            return hfile;
        }

        internal static SafeFileHandle GetFileHandle(string filename)
        {
            if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
            SafeFileHandle hfile = Win32Api.kernel32.CreateFile(filename, (int)Win32Api.kernel32.FILE_GENERIC_READ, Win32Api.kernel32.FILE_SHARE_READ, IntPtr.Zero, Win32Api.kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
            if (hfile.IsInvalid) ThrowWin32Exception();
            return hfile;
        }

        internal static SafeFileHandle GetFileHandleWithWrite(string filename)
        {
            if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
            SafeFileHandle hfile = Win32Api.kernel32.CreateFile(filename, (int)(Win32Api.kernel32.FILE_GENERIC_READ | Win32Api.kernel32.FILE_GENERIC_WRITE | Win32Api.kernel32.FILE_WRITE_ATTRIBUTES), Win32Api.kernel32.FILE_SHARE_NONE, IntPtr.Zero, Win32Api.kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
            if (hfile.IsInvalid) ThrowWin32Exception();
            return hfile;
        }

        public static System.IO.FileStream GetFileStream(string filename, FileAccess access = FileAccess.Read)
        {
            var longFilename = GetWin32LongPath(filename);
            SafeFileHandle hfile;
            if (access == FileAccess.Write)
            {
                hfile = Win32Api.kernel32.CreateFile(longFilename, (int)(Win32Api.kernel32.FILE_GENERIC_READ | Win32Api.kernel32.FILE_GENERIC_WRITE | Win32Api.kernel32.FILE_WRITE_ATTRIBUTES), Win32Api.kernel32.FILE_SHARE_NONE, IntPtr.Zero, Win32Api.kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
            }
            else
            {
                hfile = Win32Api.kernel32.CreateFile(longFilename, (int)Win32Api.kernel32.FILE_GENERIC_READ, Win32Api.kernel32.FILE_SHARE_READ, IntPtr.Zero, Win32Api.kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
            }

            if (hfile.IsInvalid) ThrowWin32Exception();

            return new System.IO.FileStream(hfile, access);
        }
     
        #endregion

        public static void SetCreationTimeUtc(string path, DateTime creationTime)
        {
            long cTime = 0;
            long aTime = 0;
            long wTime = 0;

            using (var handle = GetFileHandleWithWrite(path))
            {
                Win32Api.kernel32.GetFileTime(handle, ref cTime, ref aTime, ref wTime);
                var fileTime = creationTime.ToFileTimeUtc();
                if (!Win32Api.kernel32.SetFileTime(handle, ref fileTime, ref aTime, ref wTime))
                {
                    throw new Win32Exception();
                }
            }
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            SetCreationTime(path, creationTime.ToUniversalTime());
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTime)
        {
            long cTime = 0;
            long aTime = 0;
            long wTime = 0;

            using (var handle = GetFileHandleWithWrite(path))
            {
                Win32Api.kernel32.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                var fileTime = lastAccessTime.ToFileTimeUtc();
                if (!Win32Api.kernel32.SetFileTime(handle, ref cTime, ref fileTime, ref wTime))
                {
                    throw new Win32Exception();
                }
            }
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTime)
        {
            long cTime = 0;
            long aTime = 0;
            long wTime = 0;

            using (var handle = GetFileHandleWithWrite(path))
            {
                Win32Api.kernel32.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                var fileTime = lastWriteTime.ToFileTimeUtc();
                if (!Win32Api.kernel32.SetFileTime(handle, ref cTime, ref aTime, ref fileTime))
                {
                    throw new Win32Exception();
                }
            }
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
        }
       
     

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            long cTime = 0;
            long aTime = 0;
            long wTime = 0;

            using (var handle = GetFileHandleWithWrite(path))
            {
                Win32Api.kernel32.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                return DateTime.FromFileTimeUtc(wTime);
            }
        }

        public static DateTime GetLastWriteTime(string path)
        {
            return GetLastWriteTimeUtc(path).ToLocalTime();
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            long cTime = 0;
            long aTime = 0;
            long wTime = 0;

            using (var handle = GetFileHandleWithWrite(path))
            {
                Win32Api.kernel32.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                return DateTime.FromFileTimeUtc(cTime);
            }
        }

        public static DateTime GetCreationTime(string path)
        {
            return GetCreationTimeUtc(path).ToLocalTime();
        }
    }
}
