using System.Text;


namespace CommonControls.CommonMethods.LongIO
{
   
    public class FileInfo : c_FileSystemInfo
    {
        private readonly string m_name;
        public FileInfo(string path) : base(path)
        {
            m_name= Path.GetFileName(path);
        }   

        public bool Exists
        {
            get
            {
                return LongIO.File.Exists(m_FullPath);
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }
        public string DirectoryName
        {
            get
            {
                return Path.GetDirectoryName(m_FullPath);
            }
        }

        public long Length
        {            
            get
            {
                InitData();                       
                return (((long)m_data.nFileSizeHigh << 0x20) | (m_data.nFileSizeLow & ((long)0xffffffffL)));
            }
        }


        public void Delete()
        {
            LongIO.File.Delete(m_FullPath);
        }

    
        public void AppendAllText(string contents)
        {
            LongIO.File.AppendAllText(m_FullPath, contents);
        }

 
        public void AppendAllText(string contents, Encoding encoding)
        {
            LongIO.File.AppendAllText(m_FullPath, contents, encoding);
        }

   
        public void WriteAllText(string contents)
        {
            LongIO.File.WriteAllText(m_FullPath, contents);
        }

        public void WriteAllText(string contents, Encoding encoding)
        {
            LongIO.File.WriteAllText(m_FullPath, contents, encoding);
        }


        public void WriteAllBytes(byte[] bytes)
        {
            LongIO.File.WriteAllBytes(m_FullPath, bytes);
        }


        public FileInfo CopyTo(string destFileName)
        {
            File.Copy(m_FullPath, destFileName);
            return new FileInfo(destFileName);
        }

     

        public void Copy(string destFileName, bool overwrite)
        {
            File.Copy(m_FullPath, destFileName, overwrite);
        }

      

        public void Move(string destFileName)
        {
            File.Move(m_FullPath, destFileName);
        }

     

        public string ReadAllText()
        {
            return File.ReadAllText(m_FullPath);
        }

     

        public string ReadAllText(Encoding encoding)
        {
            return File.ReadAllText(m_FullPath, encoding);
        }


        public  string[] ReadAllLines()
        {
            return File.ReadAllLines(m_FullPath);
        }

    
        public  string[] ReadAllLines(Encoding encoding)
        {
            return File.ReadAllLines(m_FullPath, encoding);
        }
       

         public  byte[] ReadAllBytes()
        {
            return File.ReadAllBytes(m_FullPath);
        }  
    }
}
