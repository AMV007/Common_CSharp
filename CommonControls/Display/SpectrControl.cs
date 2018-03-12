using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CommonControls.Display
{
    public partial class SpectrControl : UserControl
    {
        DigitalControlsMathClass DigMath = new DigitalControlsMathClass();

        bool RemoveDC = false;
        int SyncChannel = 100000;
        int AnalysisChannel = 0;
        bool AccumulateResult = false;       
        double Frequency = 1.0;
        
        double[,] SyncData = new double[1, 100];
        double[,] DrawData = new double[1, 100];        
        double [,] FFTChannelData = new double[1, 100];
        double[,] AccumulateArray = new double[1, 100];
        int NumChannels = 1;
        int NumPointsToDrawPerChannel = 100;

        double[] ChannelsScale = null;
        double[] ChannelsShift = null;

        volatile bool ThisRunning = true;

        DateTime LastShowTime;

        DigitalControlsMathClass.FFTWindowTypeEnum SpectrWindow = DigitalControlsMathClass.FFTWindowTypeEnum.Rectangular;

        double[] _Min = new double[2],
                _Max = new double[2];


        XYPlotControl XYPlot = new XYPlotControl();
        public SpectrControl()
        {
            
            InitializeComponent();
            XYPlot.Dock = DockStyle.Fill;
            XYPlot.SetShowFPS = true;
            XYPlot.SetValueBottomText = "Hz";
            LastShowTime = DateTime.Now;
            //XYPlot.SetLogScaleBottom = true;
            panel1.Controls.Add(XYPlot);
        }

        protected override void DestroyHandle()
        {            
            XYPlot.Dispose();
            while (XYPlot.GetThisRunning) Thread.Sleep(10);
                 
            base.DestroyHandle();
            ThisRunning = false;
        }

        void CalcParam()
        {
            SyncData = new double[NumChannels, NumPointsToDrawPerChannel];
            DrawData = new double[NumChannels, NumPointsToDrawPerChannel];
            AccumulateArray = new double[NumChannels, NumPointsToDrawPerChannel/2];
            FFTChannelData = new double[NumChannels, NumPointsToDrawPerChannel/2];

            LastKnownPosition = 0;
            NumSyncData = 0;
            SyncFound = false;

            AccumulateCounter = 0;
        }


        #region SetData
        int InitialNumDrawPointsPerChannel = 100;
        public int GetSetNumDrawPset
        {
            set
            {
                if (DrawInUse.WaitOne())
                {
                    NumPointsToDrawPerChannel = value;
                    // должно быть степенью двойки
                    double r = Math.Log(NumPointsToDrawPerChannel, 2);
                    if ((r - (int)r) > 0) NumPointsToDrawPerChannel = (int)Math.Pow(2, (int)r);                    
                    InitialNumDrawPointsPerChannel = NumPointsToDrawPerChannel;
                    CalcParam();

                    XYPlot.GetSetNumDrawPset = NumPointsToDrawPerChannel / 2;
                    DrawInUse.ReleaseMutex();
                }

                hScrollBar1.Value = 0;
            }

            get
            {
                return XYPlot.GetSetNumDrawPset;
            }
        }

        public int GetSetNumChannels
        {
            set
            {
                DrawInUse.WaitOne();
                XYPlot.GetSetNumChannels = value;

                NumChannels = value;
                _Min = new double[NumChannels + 1];
                _Max = new double[NumChannels + 1];
                ChannelsShift = new double[NumChannels];
                ChannelsScale = new double[NumChannels];
                for (int i=0;i<ChannelsScale.Length;i++)ChannelsScale[i]=1;                

                SyncChannel = NumChannels;
                CalcParam();                
                hScrollBar1.Value = 0;

                DrawInUse.ReleaseMutex();
            }
            get
            {
                return XYPlot.GetSetNumChannels;
            }
        }

        public bool[] GetSetChannelEnabled
        {
            set
            {
                XYPlot.GetSetChannelEnabled = value;
            }
            get
            {
                return XYPlot.GetSetChannelEnabled;
            }
        }

        public bool GetSetSplitChannels
        {
            set
            {
                XYPlot.GetSetSplitChannels = value;
            }
            get
            {
                return XYPlot.GetSetSplitChannels;
            }
        }

        public Color[] GetSetChannelColor
        {
            set
            {
                XYPlot.GetSetChannelColor = value;
            }
            get
            {
                return XYPlot.GetSetChannelColor;
            }
        }

        public Color GetSetBackGroundColor
        {
            set
            {
                XYPlot.GetSetBackGroundColor = value;
            }
            get
            {
                return XYPlot.GetSetBackGroundColor;
            }
        }

        public Color GetSetGridColor
        {
            set
            {
                XYPlot.GetSetGridColor = value;
            }
            get
            {
                return XYPlot.GetSetGridColor;
            }
        }

        public Color GetSetScaleColor
        {
            set
            {
                XYPlot.GetSetScaleColor = value;
            }
            get
            {
                return XYPlot.GetSetScaleColor;
            }
        }

        public double GetSetBeginTime
        {
            set
            {
                XYPlot.BeginTime = value;
            }
            get
            {
                return XYPlot.BeginTime;
            }
        }

        double InitialEndTime = 1;
        public double GetSetEndTime
        {
            set
            {
                XYPlot.EndTime = value;
                InitialEndTime = value;
                hScrollBar1.Value = 0;
            }
            get
            {
                return XYPlot.EndTime;
            }
        }

        public bool GetSetRemoveDC
        {
            set
            {
                RemoveDC = value;
            }
            get
            {
                return RemoveDC;
            }
        }

        public int GetSetSyncChannel
        {
            set
            {
                SyncChannel = value;
            }
            get
            {
                return SyncChannel;
            }
        }

        public double GetSetFrequency
        {
            set
            {
                Frequency = value;
            }
            get
            {
                return Frequency;
            }
        }

        public bool SetPause
        {
            set
            {
                XYPlot.SetPause = value;
            }
        }

        public DigitalControlsMathClass.FFTWindowTypeEnum SetSpectrWindow
        {
            set
            {
                SpectrWindow = value;
            }
        }

        public event EventHandler AnalysisResultShow;
        public int SetAnalysis
        {
            set
            {
                AnalysisChannel = value;
            }
        }

        public bool SetAccumulateResult
        {
            set
            {
                AccumulateResult = value;

                if (DrawInUse.WaitOne())
                {
                    if (AccumulateResult)
                    {
                        AccumulateCounter = 0;
                        AccumulateArray = new double[NumChannels, NumPointsToDrawPerChannel];
                    }
                    DrawInUse.ReleaseMutex();
                }
            }
        }

        public double [] SetAllChannelShift
        {
            set
            {
                value.CopyTo(ChannelsShift,0);                
            }
        }

        public double SetTotalMax
        {
            set
            {
                XYPlot.SetTotalMax = value;
            }
        }

        public double SetTotalMin
        {
            set
            {
                XYPlot.SetTotalMin = value;
            }
        }

        public bool GetThisRunning
        {
            get
            {
                return ThisRunning;
            }
        }
        #endregion        

        public void CalcScaleNow()
        {
            XYPlot.CalcScaleNow();
        }

        int LastKnownPosition = 0;
        int NumSyncData = 0;
        bool SyncFound = false;
        uint AccumulateCounter = 0;
        bool PrepareData(ref double[,] Data, bool DataRipped)
        {
            if (DataRipped)
            { // поток даннык прервался
                LastKnownPosition = 0;
                NumSyncData = 0;
                SyncFound = false;
            }
            // обрабатывается ситуация - когда пришло меньше чем ожидалось
            int NumDataPerChannelIn = Data.Length / NumChannels;
            int NumCopyPset = Math.Min(NumDataPerChannelIn, NumPointsToDrawPerChannel - LastKnownPosition);
            int DataCounter = NumCopyPset;
            DigMath.CopyArrayDouble(DrawData, Data, LastKnownPosition, LastKnownPosition + NumCopyPset);

            LastKnownPosition += NumCopyPset;
            if (LastKnownPosition >= NumPointsToDrawPerChannel)
            {
                LastKnownPosition = 0;
            }
            else return false;

            if (RemoveDC)
            {
                if (!SyncFound) DigMath.FindMinMax(DrawData, _Min, _Max, null);

                DigMath.RemoveDC(DrawData, _Min, _Max, 0, NumPointsToDrawPerChannel);
            }

            if (SyncChannel < NumChannels)
            { // синхронизация по каналу  
                int NumSyncDataPosition = 0;
                if (!SyncFound)
                { // ищем точку входа синхронизации
                    for (int j = 1; j < NumPointsToDrawPerChannel; j++)
                    {
                        if (DrawData[SyncChannel, j] > 0 && DrawData[SyncChannel, j - 1] < 0)
                        {
                            NumSyncDataPosition = j;
                            SyncFound = true;
                            break;
                        }
                    }

                    if (!SyncFound) return false;
                }

                // просто копируем данные
                int NumCopyPsetSync = NumPointsToDrawPerChannel - NumSyncData - NumSyncDataPosition;
                for (int i = 0; i < NumChannels; i++)
                {
                    for (int j = 0; j < NumCopyPsetSync; j++)
                    {
                        SyncData[i, j + NumSyncData] = DrawData[i, j + NumSyncDataPosition];
                    }
                }

                NumSyncData += NumCopyPsetSync;

                if (NumDataPerChannelIn > NumCopyPset && NumSyncData < NumPointsToDrawPerChannel)
                { // есть отброшенные данные и мы не добрали данные - то продолжаем
                    NumCopyPsetSync = NumPointsToDrawPerChannel - NumSyncData;
                    NumCopyPsetSync = Math.Min(NumCopyPsetSync, NumDataPerChannelIn - (DataCounter / NumChannels));
                    if (NumCopyPsetSync > 0)
                    {
                        DigMath.CopyArrayDouble(SyncData, Data, DataCounter, DataCounter+NumCopyPsetSync);

                        if (RemoveDC) DigMath.RemoveDC(SyncData, _Min, _Max, NumSyncData, NumSyncData + NumCopyPsetSync);
                    }
                    NumSyncData += NumCopyPsetSync;
                }

                if (NumSyncData < NumPointsToDrawPerChannel)
                {
                    return false;
                }
                else
                { // буффер заполнился 
                    DigMath.CopyArrayDouble(DrawData, SyncData);                    
                    NumSyncData = 0;
                    SyncFound = false;
                }
            }
            else SyncFound = false;


            for (int i=0;i<NumChannels;i++)
            {
                if (XYPlot.GetSetChannelEnabled[i])
                {
                    double SNR = 0, THD = 0, PeakFreq = 0, Power = 0, SINAD = 0, SFDR=0, ENOB=0;

                    double ACComponent = 0, DCComponent = 0;
                    if (AnalysisChannel == i)
                    {
                        DigMath.ACDCEstimate(DrawData, i, ref ACComponent, ref DCComponent);
                    }
                    
                    int error1=DigMath.DoFFT(DrawData, i, SpectrWindow,
                        Frequency, ref PeakFreq, ref Power, ref SNR, ref THD, ref SINAD, ref SFDR, ref ENOB);

                    if (error1 != 0) throw new Exception("Не удалось FFT ошибка " + error1.ToString());

                    if (AnalysisChannel == i)
                    {                        
                        if (AnalysisResultShow.Target != null)
                        {
                            TimeSpan Diff = DateTime.Now - LastShowTime;
                            if (Diff.TotalMilliseconds > 330)
                            {
                                LastShowTime = DateTime.Now;
                                double[] Analys = new double[9] { SNR, THD, PeakFreq, Power, ACComponent, DCComponent, SINAD, SFDR, ENOB};
                                AnalysisResultShow(Analys, EventArgs.Empty);
                            }
                        }                         
                    }
                }                
            }

            if (ChannelsShift != null)
            {                
                DigMath.Calibrate(FFTChannelData,ChannelsScale ,ChannelsShift, 0, FFTChannelData.GetLength(1));
            }

            DigMath.CopyArrayDouble(FFTChannelData, DrawData);

            if (AccumulateResult)
            {
                DigMath.FindAccumulateResult(FFTChannelData, AccumulateArray, ref AccumulateCounter);
            }

            return true;
        }

        Mutex DrawInUse = new Mutex();
        public void Redraw(ref double[,] Data, bool DataRipped)
        {
            if (DrawInUse.WaitOne())
            {
                try
                {
                    if (PrepareData(ref Data, DataRipped))
                    {
                        XYPlot.Redraw(ref FFTChannelData, true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DrawInUse.ReleaseMutex();
                }
            }
        }



        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            DrawInUse.WaitOne();            
            NumPointsToDrawPerChannel = (int)(InitialNumDrawPointsPerChannel + (InitialNumDrawPointsPerChannel * hScrollBar1.Value / 100.0f));
            // должно быть степенью двойки
            double r = Math.Log(NumPointsToDrawPerChannel, 2);
            if ((r - (int)r) > 0) NumPointsToDrawPerChannel = (int)Math.Pow(2, (int)r);

            XYPlot.EndTime = (InitialEndTime*NumPointsToDrawPerChannel/InitialNumDrawPointsPerChannel)+ XYPlot.BeginTime;
            XYPlot.GetSetNumDrawPset = NumPointsToDrawPerChannel/2;
            CalcParam();
            DrawInUse.ReleaseMutex();
        }          
    }
}
