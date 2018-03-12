using System;
using System.Collections.Generic;
using System.Text;
//using System.IO;
using CommonControls.CommonMethods.LongIO;

namespace CommonControls
{
    
    public class c_SysIO
    {
        public static void DeleteFile(FileInfo fi)
        {            
            if (fi.Exists)
            {
                if ((fi.Attributes & (FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                fi.Delete();
            }
        }

        public static void DeleteFile(string FileName)
        {
            DeleteFile(new FileInfo(FileName));
        }

        public static void DeleteDirectoriesAndSubDirectories(DirectoryInfo Dir)
        {
            if (Dir.FullName.Contains("System Volume Information")) return;
            FileInfo[] fis = Dir.GetFiles();
            foreach (FileInfo fi in fis)
            {                
                DeleteFile(fi);                
            }

            DirectoryInfo[] dis = Dir.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                DeleteDirectoriesAndSubDirectories(di);                
            }

            if ((Dir.Attributes & (FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System)) != 0)
            {
                Dir.Attributes = FileAttributes.Normal;
            }

            Dir.Delete();
        }

        public static bool CopyDirectory(DirectoryInfo DirSource, DirectoryInfo DirDestination)
        {
            if (!DirDestination.Exists)
            {
                DirDestination.Create();
            }

            FileInfo[] Files = DirSource.GetFiles();
            foreach (FileInfo fi in Files)
            {
                FileInfo TempFi = new FileInfo(DirDestination.FullName+"\\" + fi.Name);
                if (TempFi.Exists)
                {
                    DeleteFile(TempFi);
                }
                fi.CopyTo(TempFi.FullName);
            }

            DirectoryInfo[] Dirs = DirSource.GetDirectories();
            foreach (DirectoryInfo di in Dirs)
            {
                CopyDirectory(di, new DirectoryInfo(DirDestination.FullName+"\\"+di.Name));
            }

            return true;
        }

        public static void CreateDirectoryWithSubDirs(string Dir)
        {
            for (int i = 0; i < Dir.Length; i++)
            {
                if (Dir[i] == '\\')
                {
                    string TempDir = Dir.Substring(0,i);
                    if (!Directory.Exists(TempDir))
                    {
                        Directory.CreateDirectory(TempDir);
                    }
                }
            }
           
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }

        public static FileInfo CopyFile(FileInfo Source, string Destination, bool owerwrite)
        {
            FileInfo Destfi=new FileInfo(Destination);
            if (!Directory.Exists(Destfi.DirectoryName))
            {
                CreateDirectoryWithSubDirs(Destfi.DirectoryName);
            } 
            else if (owerwrite)
            {
                if (Destfi.Exists)
                {
                    DeleteFile(Destfi);
                }
            }            

            return Source.CopyTo(Destination);
        }

        public static string FormatSize(long FileSize)
        {
            string Result = "";
            float ShowSize = FileSize;
            if (FileSize < (1024 * 1024))
            {
                ShowSize /= (1024);
                Result = ShowSize.ToString("f3") + " Êá";
            }
            else if (FileSize < (1024 * 1024 * 1024))
            {
                ShowSize /= (1024 * 1024);
                Result = ShowSize.ToString("f3") + " Ìá";
            }
            else
            {
                ShowSize /= (1024 * 1024 * 1024);
                Result = ShowSize.ToString("f3") + " Ãá";
            }

            return Result;
        }
    }
}
