using System;
using System.IO;
using System.Threading;

namespace CommonControls.WinCECommon
{
	/// <summary>
	/// Summary description for ErrorDataWork.
	/// </summary>
    public class c_ErrorDataWork
    {
        public string ErrorFilePath = "";

        private int ErrorCount = 0;
        public int GetErrorCount
        {
            get
            {
                return ErrorCount;
            }
        }

        public event EventHandler ErrorSygnal = null;
        private void OnErrorSygnal(Exception ex)
        {
            if (ErrorSygnal != null)
            {
                ErrorSygnal.Invoke(ex.ToString(), EventArgs.Empty);
            }
        }


        Mutex SyncMutex = new Mutex();
        public void ErrorProcess(System.Exception ex)
        {
            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;                    

                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G"));

                    if (ex.Message != null) FileStream.WriteLine("Ошибка: " + ex.Message);

                    if (ex.StackTrace != null) FileStream.WriteLine("Стек: " + ex.StackTrace);

                    FileStream.WriteLine();

                    FileStream.Close();
                }
                catch { }
                finally
                {
                    SyncMutex.ReleaseMutex();
                    OnErrorSygnal(ex);
                }
            }
        }

        public void ErrorLog(string[] Errors)
        {
            if (Errors.Length == 0) return;

            if (SyncMutex.WaitOne())
            {
                try
                {
                    ErrorCount++;

                    StreamWriter FileStream = new StreamWriter(ErrorFilePath, true, System.Text.Encoding.Unicode);

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G"));
                    for (int i = 0; i < Errors.Length; i++)
                    {
                        FileStream.WriteLine(Errors[i]);
                    }

                    FileStream.WriteLine();

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

                    FileStream.WriteLine("Время: " + DateTime.Now.ToString("G") + " | " + Error);
                    FileStream.Close();
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

        private c_ErrorDataWork()
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                ErrorFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Error.log";                
            }
            else
            { // чтобы работало как под WinCE так и под ПК
                ErrorFilePath = c_CommonFunc.ApplicationPath+"\\Error.log";                
            }

            if (File.Exists(ErrorFilePath)) System.IO.File.Delete(ErrorFilePath);
        }
    }
}
