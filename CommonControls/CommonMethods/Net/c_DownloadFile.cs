using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CommonControls.CommonMethods.Net
{
    public class c_DownloadFile
    {        
        public event EventHandler DownloadProcessingEvent;        
        DownloadProperties Current;
        Thread DownloadProcessThread;

        public struct ProxySettings
        {
            public string ProxyAddress;
            public String UserName;
            public String Password;
            public int Port;
        }

        public struct DownloadProperties
        {
            public string DownloadType;
            public string DownloadPathSave;
            public string Filename;
            public string FullURL;
            public string ReferURL;
            public int BytesDownloaded;
            public long FileSize;
            public ProxySettings Proxy;
            public bool FileDownloaded;
            public Stream OutputData;
        }

        public struct DownloadProgress
        {
            public DownloadMessageType Type;
            public float Speed;
            public float Bytes;
            public float DownloadSize;
            public string RemainTime;
            public string FilePath;
            public string AdditionalInfo;
        }

        public enum DownloadMessageType
        {
            DownloadProgressShow = 0,
            DownloadFinished,
            DownloadPaused,
            DownloadError,
            DownloadStarted,
            DownloadStopped
        }

        private void SygnalEventProgress(DownloadProgress CurrentDownloadProgress)
        {
            if (DownloadProcessingEvent != null)
            {
                DownloadProcessingEvent.Invoke(CurrentDownloadProgress, null);
            }
        }

        void DetermineDownloadProperties(ref DownloadProperties Result)
        {
            Uri CurrentDownloadUri = new Uri(Result.FullURL);

            Result.DownloadType = CurrentDownloadUri.Scheme;

            Result.Filename = CurrentDownloadUri.Segments[CurrentDownloadUri.Segments.Length - 1];
            Result.ReferURL = Result.FullURL.Substring(0, Result.FullURL.Length - Result.Filename.Length);
        }

        public c_DownloadFile()
        {
        }

        public void StartDownload(DownloadProperties NewCurrent)
        {
            Current = NewCurrent;
            DetermineDownloadProperties(ref Current);
            Current.FileDownloaded = false;            

            DownloadFileThreadRunning = true;
            DownloadFileThreadPause = false;
            DownloadProcessThread = null;

            switch (Current.DownloadType)
            {
                case "http":
                    DownloadProcessThread = new Thread(new ThreadStart(DownloadFileProcessHTTP));
                    break;
                case "ftp":
                    DownloadProcessThread = new Thread(new ThreadStart(DownloadFileProcessFTP));
                    break;
                case "file": // localfile
                    DownloadProcessThread = new Thread(new ThreadStart(DownloadFileProcessFile));
                    break;
                default:
                    DownloadProcessThread = new Thread(new ThreadStart(DownloadFileProcessFile));
                    break;
            }
            DownloadProcessThread.Start();
        }


        bool DownloadFileThreadPause = false;
        public bool PauseDownloading
        {
            set
            {
                DownloadFileThreadPause = value;
            }
            get
            {
                return DownloadFileThreadPause;
            }
        }

        public bool DownloadFileThreadRunning = false;
        public void StopDownloading()
        {
            if (DownloadFileThreadRunning)
            {
                DownloadFileThreadRunning = false;

                while (!EndDownloadFileProcess.WaitOne(200, false))
                {
                    System.Windows.Forms.Application.DoEvents();
                }

                DownloadProcessThread.Abort();
                DownloadProcessThread = null;
            }
        }

        public void WaitFinish()
        {
            if (DownloadFileThreadRunning)
            {
                EndDownloadFileProcess.WaitOne();                               
            }
        }

        AutoResetEvent EndDownloadFileProcess = new AutoResetEvent(false);
        void DownloadFileProcessHTTP()
        {
            DownloadProgress CurrentDownloadProgress = new DownloadProgress();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Current.FullURL);

                if (!string.IsNullOrEmpty(Current.Proxy.ProxyAddress))
                {
                    WebProxy MyProxy = new WebProxy(Current.Proxy.ProxyAddress, Current.Proxy.Port);
                    MyProxy.Credentials = new NetworkCredential(Current.Proxy.UserName, Current.Proxy.Password);
                    request.Proxy = MyProxy;
                }

                if (Current.BytesDownloaded != 0) request.AddRange(Current.BytesDownloaded);
                request.KeepAlive = true;

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Display the status.
                string Status = response.StatusDescription;
                if (Current.BytesDownloaded == 0)
                {
                    Current.FileSize = (int)response.ContentLength;
                }

                // передаем размер файла
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadStarted;
                CurrentDownloadProgress.DownloadSize = Current.FileSize;
                SygnalEventProgress(CurrentDownloadProgress);                

                Stream dataStream = response.GetResponseStream();
                Current.Filename = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];

                DownloadFileStreamly(dataStream);

                request.Abort();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadError;
                CurrentDownloadProgress.AdditionalInfo = ex.ToString();
                SygnalEventProgress(CurrentDownloadProgress);                
                EndDownloadFileProcess.Set();
                throw ex;
            }
            finally
            {
                EndDownloadFileProcess.Set();
                
            }
        }

        void DownloadFileProcessFTP()
        {
            DownloadProgress CurrentDownloadProgress = new DownloadProgress();

            try
            {
                WebRequest request = (WebRequest)WebRequest.Create(Current.FullURL);

                if (Current.Proxy.ProxyAddress.Length != 0)
                {
                    Uri newUri = new Uri(Current.Proxy.ProxyAddress);
                    WebProxy MyProxy = (WebProxy)request.Proxy;
                    MyProxy.Address = newUri;
                    MyProxy.Credentials = new NetworkCredential(Current.Proxy.UserName, Current.Proxy.Password);
                    request.Proxy = MyProxy;
                }
                else
                {
                    if (Current.Proxy.UserName.Length != 0)
                    {
                        request.Credentials = new NetworkCredential(Current.Proxy.UserName, Current.Proxy.Password);
                    }
                    else
                    {
                        request.Credentials = CredentialCache.DefaultCredentials;
                    }
                }

                // Get the response.
                WebResponse response = (WebResponse)request.GetResponse();

                if (Current.BytesDownloaded == 0)
                {// если еще ничего -не закачано, то есть закачка только добавлена
                    Current.FileSize = response.ContentLength;
                }
                else
                {
                    if (Current.FileSize != response.ContentLength)
                    {// если размер файла изменился со времени последней закачки - то начинаем сначала
                        Current.FileSize = response.ContentLength;
                        Current.BytesDownloaded = 0;
                        System.IO.File.Delete(Current.DownloadPathSave + "\\" + Current.Filename); // убираем текущий файл
                    }
                }

                // передаем размер файла
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadStarted;
                CurrentDownloadProgress.DownloadSize = Current.FileSize;
                SygnalEventProgress(CurrentDownloadProgress);

                Stream dataStream = response.GetResponseStream();
                Current.Filename = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];

                DownloadFileStreamly(dataStream);

                request.Abort();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadError;
                CurrentDownloadProgress.AdditionalInfo = ex.ToString();
                SygnalEventProgress(CurrentDownloadProgress);
                EndDownloadFileProcess.Set();
                throw ex;
            }
            finally
            {
                EndDownloadFileProcess.Set();
            }
        }

        void DownloadFileProcessFile()
        {
            DownloadProgress CurrentDownloadProgress = new DownloadProgress();

            try
            {
                FileInfo FileInfoThis = new FileInfo(Current.FullURL);
                // передаем размер файла
                Current.FileSize = (int)FileInfoThis.Length;

                CurrentDownloadProgress.Type = DownloadMessageType.DownloadStarted;
                CurrentDownloadProgress.DownloadSize = Current.FileSize;
                SygnalEventProgress(CurrentDownloadProgress);

                Stream dataStream = FileInfoThis.OpenRead();
                dataStream.Position = Current.BytesDownloaded;
                Current.Filename = FileInfoThis.Name;

                DownloadFileStreamly(dataStream);

                dataStream.Close();
            }
            catch (Exception ex)
            {
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadError;
                CurrentDownloadProgress.AdditionalInfo = ex.ToString();
                SygnalEventProgress(CurrentDownloadProgress);
                EndDownloadFileProcess.Set();
                throw ex;
            }
            finally
            {
                EndDownloadFileProcess.Set();
            }
        }

        byte[] ReadBuffer = new byte[1024 * 128 * 1]; // кеш - 0.5 Мб        
        void DownloadFileStreamly(Stream dataStream)
        {            
            DownloadProgress CurrentDownloadProgress = new DownloadProgress();
            Stream WriteStream = null;
            Exception TempEx=null;
            try
            {  
                if (Current.DownloadPathSave != null)
                {
                    string FilePath = Current.DownloadPathSave + "\\" + Current.Filename;

                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.FileInfo xx = new System.IO.FileInfo(FilePath);
                        if (xx.Length > Current.FileSize)
                        {
                            xx.Delete();
                            Current.BytesDownloaded = 0;
                        }
                    }
                    else
                    {
                        Current.BytesDownloaded = 0;
                    }
                    WriteStream = new FileStream(FilePath, FileMode.OpenOrCreate);
                    CurrentDownloadProgress.FilePath = FilePath;
                }
                else WriteStream = Current.OutputData;

                if (Current.FileSize == Current.BytesDownloaded && Current.BytesDownloaded != 0 &&
                    Current.FileDownloaded)
                {
                    throw new Exception("Файл уже закачан " + Current.FullURL);
                }

                if (Current.FileSize > 0 && Current.BytesDownloaded == 0 &&
                    WriteStream.Length != Current.FileSize)
                {   // если известен размер файла и файл только что добавили, то создаем пустышку в размер файла                                
                    WriteStream.SetLength(Current.FileSize);
                }

                if (WriteStream.Length == Current.BytesDownloaded && WriteStream.Length != 0)
                {
                    Current.BytesDownloaded = 0;
                }

                WriteStream.Position = Current.BytesDownloaded;

                DateTime BeginTime = DateTime.Now;
                TimeSpan TimeDifference;
                TimeSpan RemainTime;                

                int BeginDownloadBytes = Current.BytesDownloaded;
                int ReadData = 1;
                int BufferOffset = 0;
                double LastTimeDifferenceSeconds = 0;

                while (DownloadFileThreadRunning)
                {
                    if (!DownloadFileThreadPause)
                    { // если не была задана пауза
                        ReadData = dataStream.Read(ReadBuffer, BufferOffset, ReadBuffer.Length - BufferOffset);
                        BufferOffset += ReadData;

                        TimeDifference = DateTime.Now - BeginTime;

                        if (((ReadBuffer.Length - BufferOffset) < 1024) || // осталось черезчур мало места в буффере                 
                            (ReadData == 0) ||   // закачка закончилась
                            (TimeDifference.TotalSeconds != LastTimeDifferenceSeconds)) // но не реже 1 раза в секунду
                        {
                            WriteStream.Write(ReadBuffer, 0, BufferOffset);
                            Current.BytesDownloaded += BufferOffset;                            

                            if (TimeDifference.TotalSeconds != LastTimeDifferenceSeconds)
                            {
                                CurrentDownloadProgress.DownloadSize = Math.Max(Current.BytesDownloaded, Current.FileSize);
                                CurrentDownloadProgress.Bytes = Current.BytesDownloaded;
                                CurrentDownloadProgress.Speed = (
                                    (Current.BytesDownloaded - BeginDownloadBytes) * 1000.0f
                                    / (float)TimeDifference.TotalMilliseconds);

                                RemainTime = new TimeSpan((long)
                                    ((TimeDifference.Ticks * (Current.FileSize - Current.BytesDownloaded))
                                    / (float)(Current.BytesDownloaded - BeginDownloadBytes))
                                    );

                                CurrentDownloadProgress.RemainTime = RemainTime.ToString();

                                CurrentDownloadProgress.Type = DownloadMessageType.DownloadProgressShow;
                                SygnalEventProgress(CurrentDownloadProgress);

                                if (TimeDifference.TotalSeconds >= 5)
                                {
                                    BeginTime = DateTime.Now;
                                    BeginDownloadBytes = Current.BytesDownloaded;
                                }
                            };

                            BufferOffset = 0;
                            LastTimeDifferenceSeconds = TimeDifference.TotalSeconds;
                        }
                    }
                    else
                    {// была задана пауза                        
                        CurrentDownloadProgress.Type = DownloadMessageType.DownloadPaused;
                        SygnalEventProgress(CurrentDownloadProgress);
                        Thread.Sleep(500);
                    }

                    if (ReadData == 0)
                    { // все закачали
                        if (Current.FileSize == -1)
                        { // не был известен размер файла
                            Current.FileSize = Current.BytesDownloaded;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TempEx = ex;
            //    ErrorWork(new Exception("Размер файла " + Current.FileSize));
            }
            finally
            {
                WriteStream.Close();

                // финальное состояние
                CurrentDownloadProgress.DownloadSize = Math.Max(Current.BytesDownloaded, Current.FileSize);
                CurrentDownloadProgress.Bytes = Current.BytesDownloaded;
                CurrentDownloadProgress.Type = DownloadMessageType.DownloadProgressShow;
                SygnalEventProgress(CurrentDownloadProgress);

                if (DownloadFileThreadRunning && Current.FileSize == Current.BytesDownloaded)
                {   // файл закончен
                    CurrentDownloadProgress.Type = DownloadMessageType.DownloadFinished;
                    SygnalEventProgress(CurrentDownloadProgress);
                    Current.FileDownloaded = true;
                }
                else
                { // была нажата кнопка стоп -поэтому - ничего не вызываем                
                    CurrentDownloadProgress.Type = DownloadMessageType.DownloadStopped;
                    CurrentDownloadProgress.AdditionalInfo = "DownloadFileThreadRunning " + DownloadFileThreadRunning.ToString() +
                        " Current.FileSize = " + Current.FileSize.ToString() +
                        " Current.BytesDownloaded = " + Current.BytesDownloaded.ToString();
                    SygnalEventProgress(CurrentDownloadProgress);                    
                }                

                DownloadFileThreadRunning = false;
            }
            if (TempEx != null) throw TempEx;
        }
    }
}
