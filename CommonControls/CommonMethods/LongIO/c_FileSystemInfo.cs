using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace CommonControls.CommonMethods.LongIO
{
    public abstract class c_FileSystemInfo
    {
        protected const int MAX_PATH = 255;        
        protected readonly string m_FullPath;
        protected bool m_dataInitialised = false;
        internal Win32Api.kernel32.WIN32_FILE_ATTRIBUTE_DATA m_data = new Win32Api.kernel32.WIN32_FILE_ATTRIBUTE_DATA();

        public c_FileSystemInfo(string FullPath)
        {
            m_FullPath = FullPath;
        }

        public c_FileSystemInfo()
        {

        }


        public string FullName
        {
            get
            {
                return m_FullPath;
            }
        }

        //public string FullPath
        //{
        //    get
        //    {
        //        return m_FullPath;
        //    }
        //}

        public FileAttributes Attributes
        {
            get
            {
                return GetAttributes();
            }

            set
            {
                SetAttributes(value);
            }
        }

        public DateTime CreationTimeUtc
        {            
            get
            {
                InitData();
                return DateTime.FromFileTimeUtc((long)(((long)m_data.ftCreationTime.DateTimeHigh << 0x20) | m_data.ftCreationTime.DateTimeLow));
            }
            set
            {
                if (this is DirectoryInfo)
                {
                    Directory.SetCreationTimeUtc(m_FullPath, value);
                }
                else
                {
                    File.SetCreationTimeUtc(m_FullPath, value);
                }
                m_dataInitialised = false;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return CreationTimeUtc.ToLocalTime();
            }
            set
            {
                CreationTimeUtc = value.ToUniversalTime();
            }
        }

        public DateTime LastAccessTime
        {
            get
            {
                return this.LastAccessTimeUtc.ToLocalTime();
            }
            set
            {
                this.LastAccessTimeUtc = value.ToUniversalTime();
            }
        }

       
        public DateTime LastAccessTimeUtc
        {
          
            get
            {
                InitData();
                return DateTime.FromFileTimeUtc((long)(((long)m_data.ftLastAccessTime.DateTimeHigh << 0x20) | m_data.ftLastAccessTime.DateTimeLow));
            }
            set
            {
                if (this is DirectoryInfo)
                {
                    Directory.SetLastAccessTimeUtc(m_FullPath, value);
                }
                else
                {
                    File.SetLastAccessTimeUtc(m_FullPath, value);
                }
                m_dataInitialised = false;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return LastWriteTimeUtc.ToLocalTime();
            }
            set
            {
                LastWriteTimeUtc = value.ToUniversalTime();
            }
        }

        public DateTime LastWriteTimeUtc
        {
          
            get
            {
                InitData();                
                return DateTime.FromFileTimeUtc((long)(((long)m_data.ftLastWriteTime.DateTimeHigh << 0x20) | m_data.ftLastWriteTime.DateTimeLow));
            }
            set
            {
                if (this is DirectoryInfo)
                {
                    Directory.SetLastWriteTimeUtc(m_FullPath, value);
                }
                else
                {
                    File.SetLastWriteTimeUtc(m_FullPath, value);
                }
                m_dataInitialised = false;
            }
        }

        public static FileAttributes GetAttributes(string path)
        {
            if (path.Length < MAX_PATH)
            {
                return (FileAttributes)Win32Api.kernel32.GetFileAttributesA(path);
            }
            else
            {
                var longFilename = GetWin32LongPath(path);
                return (FileAttributes)Win32Api.kernel32.GetFileAttributesW(longFilename);
            }
        }      

        public FileAttributes GetAttributes()
        {
            return GetAttributes(m_FullPath);
        }

        public void InitData()
        {
            if (!m_dataInitialised) Refresh();            
        }

        public void Refresh()
        {
            if (m_FullPath.Length < MAX_PATH)
            {
                m_dataInitialised = Win32Api.kernel32.GetFileAttributesExA(m_FullPath, Win32Api.kernel32.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out m_data);
                if (m_dataInitialised) return;
            }
            
            m_dataInitialised = Win32Api.kernel32.GetFileAttributesExW(GetWin32LongPath(m_FullPath), Win32Api.kernel32.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out m_data);
            if (!m_dataInitialised) throw new Exception("Win32Api.kernel32.GetFileAttributesExW " + new Win32Exception(Marshal.GetLastWin32Error()).Message + " "+GetWin32LongPath(m_FullPath));            
        }


        public static void SetAttributes(string path, FileAttributes attributes)
        {
            if (path.Length < MAX_PATH)
            {
                Win32Api.kernel32.SetFileAttributesA(path, (int)attributes);
            }
            else
            {
                var longFilename = GetWin32LongPath(path);
                Win32Api.kernel32.SetFileAttributesW(longFilename, (int)attributes);
            }
        }

        public void SetAttributes(FileAttributes attributes)
        {
            SetAttributes(m_FullPath, attributes);
        }

        public static string GetWin32LongPath(string path)
        {
            if (path.StartsWith(@"\\?\")) return path;

            if (path.StartsWith("\\"))
            {
                path = @"\\?\UNC\" + path.Substring(2);
            }
            else if (path.Contains(":"))
            {
                path = @"\\?\" + path;
            }
            else
            {
                var currdir = Environment.CurrentDirectory;
                path = Combine(currdir, path);
                while (path.Contains("\\.\\")) path = path.Replace("\\.\\", "\\");
                path = @"\\?\" + path;
            }
            return path.TrimEnd('.'); ;
        }

        protected static string Combine(string path1, string path2)
        {
            return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.'); ;
        }

        [DebuggerStepThrough]
        public static void ThrowWin32Exception()
        {
            int code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new System.ComponentModel.Win32Exception(code);
            }
        }
    }
}
