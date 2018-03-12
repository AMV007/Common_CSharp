using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;
using System.Threading;

namespace CommonControls
{
    public class c_AppConf
    {
        private string appConfigurationPath;
        private static string applicationPath = c_CommonFunc.ApplicationPath;

        private Mutex AppConfMutex;

        public bool ReadyToSave = true; // иногда бывает, что надо блокировать сохранение

        public virtual object SaveO
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /*
         * really need only to implement override
         *  public override object SaveO
            {
                get
                {
                    return conf;
                }
                set
                {
                    conf = (Confstruct)value;
                }
            }
         * where conf - something of configuration
         * 
         * you need also
         * 
         *  private volatile static cc_AppConf InternalInstance;
            public static cc_AppConf Instance
            {
                get
                {
                    if (InternalInstance == null) InternalInstance = new cc_AppConf();
                    return InternalInstance;
                }
            }
         * 
         * and constructor
         * */

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

        public virtual string ApplicationPath
        {
            get
            {
                return applicationPath;
            }
        }

        public virtual void Save()
        {
            if (!ReadyToSave) return;

            // записываем конфигурацию приложения	
            Exception exc = null;
            if (AppConfMutex.WaitOne())
            {
                StreamWriter file = null;
                try
                {
                    XmlSerializer writer = new XmlSerializer(SaveO.GetType());
                    file = new StreamWriter(appConfigurationPath);
                    writer.Serialize(file, SaveO);                                        
                }
                catch (Exception ex)
                {
                    exc = ex;
                }
                finally
                {
                    if (file != null) file.Close();
                    AppConfMutex.ReleaseMutex();
                }
            }
            else exc = new Exception("Не могу получить доступ к конфигурации для сохранения");

            if (exc != null) throw exc;
        }

        public virtual void SetDefaultConfiguration()
        {
        }

        public virtual bool NeeedDefaultConf()
        {
            return false;
        }

        public virtual void Load()
        {
            Exception exc = null;

            if (AppConfMutex.WaitOne())
            {
                StreamReader file = null;
                try
                {
                    if (!File.Exists(appConfigurationPath) || SaveO == null)
                    {
                        SetDefaultConfiguration();
                        bool LastReady = ReadyToSave;
                        ReadyToSave = true;
                        Save();
                        ReadyToSave = LastReady;
                    }
                    // Insert code to read the stream here.
                    XmlSerializer reader = new XmlSerializer(SaveO.GetType());

                    file = new StreamReader(appConfigurationPath);

                    // Deserialize the content of the file into a Book object.
                    SaveO = reader.Deserialize(file);

                    if (NeeedDefaultConf())
                    {
                        SetDefaultConfiguration();
                        bool LastReady = ReadyToSave;
                        ReadyToSave = true;
                        Save();
                        ReadyToSave = LastReady;
                    }

                }
                catch (Exception ex)
                {                    
                    exc = ex;

                    SetDefaultConfiguration();
                    bool LastReady = ReadyToSave;
                    ReadyToSave = true;
                    try
                    {// бывает reader не успевает освободить файл
                        Save();
                    }
                    catch {};
                    ReadyToSave = LastReady;
                }
                finally
                {
                    if (file != null) file.Close();
                    AppConfMutex.ReleaseMutex();
                }
            }
            else exc = new Exception("Не могу получить доступ к конфигурации для считывания");

            if (exc != null) throw exc;
        }

        public c_AppConf()
        {           
            appConfigurationPath = applicationPath + "\\appconf.xml";
            AppConfMutex = new Mutex();
        }
    }
}
