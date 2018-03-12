using System;
using System.IO;
using System.Threading;

namespace CommonControls
{
	/// <summary>
	/// Summary description for ErrorDataWork.
	/// </summary>
	public class c_ErrorDataWork
	{		
        public string ErrorFilePath="";
        public bool EncryptLog = false;

        public event EventHandler ErrorSygnal=null;        
        private void OnErrorSygnal(Exception ex)
        {
            if (ErrorSygnal != null)
            {
                ErrorSygnal(ex.ToString(), EventArgs.Empty);
            }
        }

        public event EventHandler ErrorSygnalAsync = null;
        private void OnErrorSygnalAsync(Exception ex)
        {
            if (ErrorSygnalAsync != null)
            {
                ErrorSygnalAsync.BeginInvoke(ex.ToString(), EventArgs.Empty,null,null);
            }
        }

        private void OnErrorSygnal(string exception)
        {
            if (ErrorSygnal != null)
            {
                ErrorSygnal(exception, EventArgs.Empty);
            }
        }

        private void OnErrorSygnalAsync(string exception)
        {
            if (ErrorSygnalAsync != null)
            {
                ErrorSygnalAsync.BeginInvoke(exception, EventArgs.Empty, null, null);
            }
        }

        public event EventHandler ErrorLogSygnalAsync = null;
        private void OnErrorLogSygnalAsync(string ex)
        {
            if (ErrorLogSygnalAsync != null)
            {
                ErrorLogSygnalAsync.BeginInvoke(ex, EventArgs.Empty,null,null);
            }
        }

        public event EventHandler ErrorLogSygnal = null;
        private void OnErrorLogSygnal(string ex)
        {
            if (ErrorLogSygnal != null)
            {
                ErrorLogSygnal(ex, EventArgs.Empty);
            }
        }

        private int ErrorCount=0;
        public int GetErrorCount
        {
            get
            {
                return ErrorCount;
            }
        }        

        
        Mutex SyncMutex = new Mutex();
        //
        // Summary:
        //     Processing Error into log
        //
		public void ErrorProcess(System.Exception ex, bool NoCallbackSygnal=false)
		{
            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;                    
                    
                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G"));

                    if (ex.Message != null) FileStream.WriteLine("Ошибка: " + ex.Message);

                    if (ex.HelpLink != null) FileStream.WriteLine("Описание: " + ex.HelpLink);

                    if (ex.StackTrace != null) FileStream.WriteLine("Стек: " + ex.StackTrace);

                    FileStream.WriteLine();

                    FileStream.Close();
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                }
            }
            if (!NoCallbackSygnal)
            {
                OnErrorSygnal(ex);
                OnErrorSygnalAsync(ex);
            }
		}

        //
        // Summary:
        //     Processing Error into log
        //
        public void ErrorProcess(string exception)
        {
            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;

                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G"));

                    FileStream.WriteLine("Ошибка: " + exception);                    

                    FileStream.WriteLine();

                    FileStream.Close();
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                }
            }
            OnErrorSygnal(exception);
            OnErrorSygnalAsync(exception);
        }

        //
        // Summary:
        //     possible encryption for data string
        //
        string PossibleEncrypted(string sourcestring)
        {
            if(EncryptLog)
            {
                string NewString = "ENCD";
                for (int i = 0; i < sourcestring.Length; i++)
                {
                    NewString += ((short)sourcestring[i]).ToString()+"|";
                }
                return NewString;
            }
            return sourcestring;
        }

        public void ErrorLog(string [] Errors, bool MessageOnly=false)
        {
            if (Errors.Length == 0) return;

            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;

                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    if(!MessageOnly) FileStream.WriteLine("Время: " + DateTime.Now.ToString("G"));
                    for (int i = 0; i < Errors.Length; i++)
                    {
                        FileStream.WriteLine(PossibleEncrypted(Errors[i]));
                        OnErrorLogSygnal(Errors[i]);
                        OnErrorLogSygnalAsync(Errors[i]);
                    }

                    if (!MessageOnly) FileStream.WriteLine();

                    FileStream.Close();
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                }
            }
        }

        public void ErrorLog(string Error)
        {
            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;

                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G") + " | " + PossibleEncrypted(Error));                       
                    FileStream.Close();

                    OnErrorLogSygnal(Error);
                    OnErrorLogSygnalAsync(Error);
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                }
            }
        }

        public string[] GetErrors()
        {
            string[] ReturnValue = null;
            if (SyncMutex.WaitOne())
            {
                try
                {
                    System.Collections.ArrayList StringArray = new System.Collections.ArrayList();

                    StreamReader FileStream = new StreamReader(ErrorFilePath, System.Text.Encoding.Unicode);
                    
                    string TempString;
                    while ((TempString = FileStream.ReadLine()) != null)
                    {
                        StringArray.Add(TempString);
                    }

                    FileStream.Close();
                    ReturnValue = (string[])StringArray.ToArray(typeof(string));
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                }
            }

            return ReturnValue;
        }

        private static c_ErrorDataWork ThisInstance;
        public static c_ErrorDataWork Instance
        {
            get
            {
                if (ThisInstance == null) ThisInstance = new c_ErrorDataWork();
                return ThisInstance;
            }
        }

        public c_ErrorDataWork()
        {
            ErrorFilePath = c_CommonFunc.ApplicationPath + "\\Error.log";
            System.IO.File.Delete(ErrorFilePath);	           
        }
		
		public c_ErrorDataWork(string LogFilePath)
		{
            ErrorFilePath = LogFilePath;
            System.IO.File.Delete(ErrorFilePath);	
		}
	}
}

