using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace CommonControls.WinCECommon
{
    public class c_AppConf
    {           
        private string appConfigurationPath;        
        private string applicationPath=null;

        public bool ReadyToSave = true;

        public virtual string AppConfigurationPath
        {
            set
            {
                appConfigurationPath = value;
            }
            get
            {
                return appConfigurationPath;
            }
        }

        public virtual void Save(ref object Data)
        {
            if (!ReadyToSave) return;

            // записываем конфигурацию приложения	
            Exception exc = null;
            StreamWriter file=null;
            try
            {
                XmlSerializer writer = new XmlSerializer(Data.GetType());
                file = new StreamWriter(appConfigurationPath);
                writer.Serialize(file, Data);
            }
            catch (Exception ex)
            {
                exc = ex;
            }
            finally
            {
                if (file !=null)file.Close();
            }
            if (exc != null) throw exc;
        }      
       

        public virtual void SetDefaultConfiguration(ref object Data)
        {
        }

        public virtual void Load(ref object Data)
        {
            Exception exc = null;
            StreamReader file = null;
            try
            {
                if (!File.Exists(appConfigurationPath) || Data == null)
                {
                    SetDefaultConfiguration(ref Data);
                    bool LastReady = ReadyToSave;
                    ReadyToSave = true;
                    Save(ref Data);
                    ReadyToSave = LastReady;
                }
                // Insert code to read the stream here.
                XmlSerializer reader = new XmlSerializer(Data.GetType());

                file = new StreamReader(appConfigurationPath);

                // Deserialize the content of the file into a Book object.
                Data=reader.Deserialize(file);
            }
            catch (Exception ex)
            {
                exc = ex;
            }
            finally
            {
                if (file!=null) file.Close();
            }
            if (exc != null) throw exc;
        }

        public c_AppConf()
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                applicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                appConfigurationPath = applicationPath + "\\appconf.xml";
            }
            else
            {
                applicationPath = c_CommonFunc.ApplicationPath;
                appConfigurationPath = applicationPath + "\\appconf.xml";
            }
        }
    }
}
