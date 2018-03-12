using System;
using System.Collections.Generic;
using System.IO;

namespace CommonControls.CommonMethods.LongIO
{
    public class Directory : c_FileSystemInfo
    {
        public static DirectoryInfo CreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            if (path.Length < MAX_PATH)
            {
                System.IO.Directory.CreateDirectory(path);
            }
            else
            {
                var paths = GetAllPathsFromPath(GetWin32LongPath(path));
                foreach (var item in paths)
                {
                    if (!LongExists(item))
                    {
                        var ok = Win32Api.kernel32.CreateDirectory(item, IntPtr.Zero);
                        if (!ok)
                        {
                            ThrowWin32Exception();
                        }
                    }
                }
            }
            return new DirectoryInfo(path);
        }    

        public static void Delete(string path)
        {
            Delete(path, false);
        }

        public static void Delete(string path, bool recursive)
        {
            if (path.Length < MAX_PATH && !recursive)
            {
                System.IO.Directory.Delete(path, false);
            }
            else
            {
                if (!recursive)
                {
                    bool ok = Win32Api.kernel32.RemoveDirectory(GetWin32LongPath(path));
                    if (!ok) ThrowWin32Exception();
                }
                else
                {
                    DeleteDirectories(new LongIO.DirectoryInfo[] { new LongIO.DirectoryInfo(GetWin32LongPath(path)) });
                }
            }
        }

        private static void DeleteDirectories(LongIO.DirectoryInfo[] directories)
        {
            foreach (LongIO.DirectoryInfo directory in directories)
            {
                LongIO.FileInfo[] files = LongIO.Directory.GetFiles(directory.FullName, null, System.IO.SearchOption.TopDirectoryOnly);
                foreach (LongIO.FileInfo file in files)
                {
                    file.Delete();
                }
                directories = LongIO.Directory.GetDirectories(directory.FullName, null, System.IO.SearchOption.TopDirectoryOnly);
                DeleteDirectories(directories);
                bool ok = Win32Api.kernel32.RemoveDirectory(GetWin32LongPath(directory.FullName));
                if (!ok) ThrowWin32Exception();
            }
        }

        public static bool Exists(string path)
        {
            if (path.Length < MAX_PATH) return System.IO.Directory.Exists(path);
            return LongExists(GetWin32LongPath(path));
        }

        private static bool LongExists(string path)
        {
            var attr = Win32Api.kernel32.GetFileAttributesW(path);
            return (attr != Win32Api.kernel32.INVALID_FILE_ATTRIBUTES && ((attr & Win32Api.kernel32.FILE_ATTRIBUTE_DIRECTORY) == Win32Api.kernel32.FILE_ATTRIBUTE_DIRECTORY));
        }


        public static LongIO.DirectoryInfo[] GetDirectories(string path)
        {
            return GetDirectories(path, null, SearchOption.TopDirectoryOnly);
        }      

        public static LongIO.DirectoryInfo[] GetDirectories(string path, string searchPattern)
        {
            return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static LongIO.DirectoryInfo[] GetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption)
        {
            searchPattern = searchPattern ?? "*";
            var dirs = new List<LongIO.DirectoryInfo>();
            InternalGetDirectories(path, searchPattern, searchOption, ref dirs);
            return dirs.ToArray();
        }

        private static void InternalGetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption, ref List<LongIO.DirectoryInfo> dirs)
        {
            Win32Api.kernel32.WIN32_FIND_DATA findData;
            IntPtr findHandle = Win32Api.kernel32.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(path), searchPattern), out findData);

            try
            {
                if (findHandle != new IntPtr(-1))
                {

                    do
                    {
                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                string subdirectory = System.IO.Path.Combine(path, findData.cFileName);
                                dirs.Add(new LongIO.DirectoryInfo(GetCleanPath(subdirectory)));
                                if (searchOption == SearchOption.AllDirectories)
                                {
                                    InternalGetDirectories(subdirectory, searchPattern, searchOption, ref dirs);
                                }
                            }
                        }
                    } while (Win32Api.kernel32.FindNextFile(findHandle, out findData));
                    Win32Api.kernel32.FindClose(findHandle);
                }
                else
                {
                    ThrowWin32Exception();
                }
            }
            catch (Exception)
            {
                Win32Api.kernel32.FindClose(findHandle);
                throw;
            }
        }

        public static LongIO.FileInfo[] GetFiles(string path)
        {
            return GetFiles(path, null, SearchOption.TopDirectoryOnly);
        }        

        public static LongIO.FileInfo[] GetFiles(string path, string searchPattern)
        {
            return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }


        public static LongIO.FileInfo[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)
        {
            searchPattern = searchPattern ?? "*";

            var files = new List<LongIO.FileInfo>();
            var dirs = new List<LongIO.DirectoryInfo> { new LongIO.DirectoryInfo(path) };

            if (searchOption == SearchOption.AllDirectories)
            {
                //Add all the subpaths
                dirs.AddRange(LongIO.Directory.GetDirectories(path, null, SearchOption.AllDirectories));
            }

            foreach (var dir in dirs)
            {
                Win32Api.kernel32.WIN32_FIND_DATA findData;
                IntPtr findHandle = Win32Api.kernel32.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(dir.FullName), searchPattern), out findData);

                try
                {
                    if (findHandle != new IntPtr(-1))
                    {

                        do
                        {
                            if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) == 0)
                            {
                                string filename = System.IO.Path.Combine(dir.FullName, findData.cFileName);
                                files.Add(new FileInfo(GetCleanPath(filename)));
                            }
                        } while (Win32Api.kernel32.FindNextFile(findHandle, out findData));
                        Win32Api.kernel32.FindClose(findHandle);
                    }
                }
                catch (Exception)
                {
                    Win32Api.kernel32.FindClose(findHandle);
                    throw;
                }
            }

            return files.ToArray();
        }



        public static void Move(string sourceDirName, string destDirName)
        {
            if (sourceDirName.Length < MAX_PATH || destDirName.Length < MAX_PATH)
            {
                System.IO.Directory.Move(sourceDirName, destDirName);
            }
            else
            {
                var ok = Win32Api.kernel32.MoveFileW(GetWin32LongPath(sourceDirName), GetWin32LongPath(destDirName));
                if (!ok) ThrowWin32Exception();
            }
        }

        public static DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            return File.GetCreationTimeUtc(path);
        }

        public static void SetCreationTimeUtc(string path, DateTime creationTime)
        {
            File.SetCreationTimeUtc(path, creationTime);
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            File.SetCreationTime(path, creationTime);
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            File.SetLastAccessTime(path, lastAccessTime);
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTime)
        {
            File.SetLastAccessTimeUtc(path, lastAccessTime);
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            File.SetLastWriteTime(path, lastWriteTime);
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTime)
        {
            File.SetLastWriteTimeUtc(path, lastWriteTime);
        }

        #region Helper methods     

      
        private static string GetCleanPath(string path)
        {
            if (path.StartsWith(@"\\?\UNC\")) return @"\\" + path.Substring(8);
            if (path.StartsWith(@"\\?\")) return path.Substring(4);
            return path;
        }



        private static List<string> GetAllPathsFromPath(string path)
        {
            bool unc = false;
            var prefix = @"\\?\";
            if (path.StartsWith(prefix + @"UNC\"))
            {
                prefix += @"UNC\";
                unc = true;
            }
            var split = path.Split('\\');
            int i = unc ? 6 : 4;
            var list = new List<string>();
            var txt = "";

            for (int a = 0; a < i; a++)
            {
                if (a > 0) txt += "\\";
                txt += split[a];
            }
            for (; i < split.Length; i++)
            {
                txt = Combine(txt, split[i]);
                list.Add(txt);
            }

            return list;
        }



        #endregion
    }
}
