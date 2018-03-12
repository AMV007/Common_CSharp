using System.IO;

namespace CommonControls.CommonMethods.LongIO
{
    public class DirectoryInfo : c_FileSystemInfo
    {
        public DirectoryInfo(string path) : base(path)
        {            
        }

        public void Create()
        {
            LongIO.Directory.CreateDirectory(m_FullPath);
        }

        public  void Delete()
        {
            LongIO.Directory.Delete(m_FullPath, false);
        }      

        public  bool Exists
        {
            get
            {
                return LongIO.Directory.Exists(m_FullPath);
            }
        }

        public string Name
        {
            get
            {
                return GetDirName(m_FullPath);
            }
        }

      

        private static string GetDirName(string fullPath)
        {
            if (fullPath.Length > 3)
            {
                string path = fullPath;
                if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    path = fullPath.Substring(0, fullPath.Length - 1);
                }
                return Path.GetFileName(path);
            }
            return fullPath;
        }


        public  LongIO.DirectoryInfo[] GetDirectories()
        {
            return LongIO.Directory.GetDirectories(m_FullPath, null, SearchOption.TopDirectoryOnly);
        }


        public  LongIO.FileInfo[] GetFiles()
        {
            return LongIO.Directory.GetFiles(m_FullPath, null, SearchOption.TopDirectoryOnly);
        }

       
    }

}
