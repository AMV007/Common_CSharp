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
    public partial class OscilControl : UserControl
    {
        DigitalControlsMathClass DigMath = new DigitalControlsMathClass();

        bool RemoveDC = false;
        int SyncChannel = 100000;
        double[,] SyncData = new double[1, 100];
        double[,] DrawData = new double[1, 100];
        int NumChannels = 1;
        int NumPointsToDrawPerChannel = 100;

        double[]    _Min = new double[2],
                    _Max = new double[2];

        volatile bool ThisRunning = true;

        XYPlotControl XYPlot = new XYPlotControl();
        public OscilControl()
        {
            InitializeComponent();
            XYPlot.Dock = DockStyle.Fill;
            XYPlot.SetShowFPS = true;
            XYPlot.SetValueBottomText = "sec";
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

            LastKnownPosition = 0;
            NumSyncData = 0;
            SyncFound = false;
        }


        #region SetData
        int InitialNumDrawPointsPerChannel = 100;
        public int GetSetNumDrawPset
        {
            set
            {
                DrawInUse.WaitOne();
                XYPlot.GetSetNumDrawPset = value;
                NumPointsToDrawPerChannel = value;
                InitialNumDrawPointsPerChannel = value;

                CalcParam();
                DrawInUse.ReleaseMutex();

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
                SyncChannel = NumChannels;
                CalcParam();
                DrawInUse.ReleaseMutex();
                hScrollBar1.Value = 0;
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

        public bool SetPause
        {
            set
            {
                XYPlot.SetPause = value;
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
        bool PrepareData(ref double[,] Data, bool DataRipped)
        {
            if (DataRipped)
            {
                LastKnownPosition = 0;
                NumSyncData = 0;
                SyncFound = false;
            }
            // обрабатывается ситуация - когда пришло меньше чем ожидалось
            int NumDataPerChannelIn = Data.Length / NumChannels;
            int NumCopyPset = Math.Min(NumDataPerChannelIn, NumPointsToDrawPerChannel - LastKnownPosition);
            int DataCounter = NumCopyPset;
            DigMath.CopyArrayDouble(DrawData, Data, LastKnownPosition, LastKnownPosition+NumCopyPset);

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
                        DigMath.CopyArrayDouble(SyncData, Data, DataCounter, DataCounter + NumCopyPsetSync);

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
                        XYPlot.Redraw(ref DrawData, true);
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
            XYPlot.EndTime = (InitialEndTime + (InitialEndTime * hScrollBar1.Value / 100.0f)) + XYPlot.BeginTime;
            NumPointsToDrawPerChannel = (int)(InitialNumDrawPointsPerChannel + (InitialNumDrawPointsPerChannel * hScrollBar1.Value / 100.0f));
            XYPlot.GetSetNumDrawPset = NumPointsToDrawPerChannel;
            CalcParam();
            DrawInUse.ReleaseMutex();
        }          
    }
}
