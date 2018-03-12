using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Threading;

namespace CommonControls.Display
{
    public partial class XYPlotControl : UserControl
    {
        DigitalControlsMathClass DigMath = new DigitalControlsMathClass();

        bool ThisRunning = true;

        bool SplitChannels = false;
        bool DrawPause = false;
        bool PossibleCalcMinMax = true;

        bool ShowFPS = false;
        int NumChannels = 1;
        int NumPointsToDrawPerChannel = 100;

        bool LogScaleBottom = false;
        bool LogScaleLeft = false;

        double[,] DrawData = new double[1, 100];
        double[,] ZeroData = new double[0, 0];

        bool[] ChannelEnabled = new bool[2] { true,true};

        Microsoft.DirectX.Direct3D.Device VideoDevice = null; // Our rendering device		        
        Microsoft.DirectX.Direct3D.Line DirectXLine = null;
        Microsoft.DirectX.Direct3D.Line ScaleLine = null;
        Microsoft.DirectX.Direct3D.Line GridLine = null;
        Microsoft.DirectX.Direct3D.Font DirectDrawFont = null;

        Microsoft.DirectX.Direct3D.Sprite DXFontSprite = null;

        float[] drawPointsXCoord = new float[3];
        Vector2[] drawPoints = new Vector2[3];
        Vector2[] drawScaleRectangle = new Vector2[5];
        Vector2[] drawScaleGridVertical = new Vector2[2];
        Vector2[] drawScaleBottom = new Vector2[2];
        Vector2[] drawScale1 = new Vector2[2];
        Vector2[] drawScale2 = new Vector2[2];
        Vector2[] drawScale3 = new Vector2[2];

        Color[] DrawChannelColors = new Color[]{Color.Red, Color.Green, Color.Blue, Color.Yellow,
                                         Color.Brown, Color.DarkGray, Color.DarkViolet, Color.Khaki};
        Color DrawBackgroundColor = Color.WhiteSmoke;
        Color DrawScaleColor = Color.Black;
        Color DrawGridColor = Color.Black;
        Color PointerColor = Color.Black;
        Color PointerBackColor = Color.Yellow;

        Point DrawOffset = new Point(40, 0);
        Rectangle DrawRectangle = new Rectangle(10, 10, 10, 10);
        Rectangle DrawRectangleScaleBottom = new Rectangle(10, 10, 10, 10);
        Rectangle DrawRectangleScaleLeft = new Rectangle(10, 10, 10, 10);

        Rectangle[] ChannelDrawRectangle = new Rectangle[1];

        double[] _Min = new double[2],
                _Max = new double[2],
                _NewMin = new double[2],
                _NewMax = new double[2];

        double TotalMin = -1e+15, // ниже него нельз€ прыгать
                TotalMax = 1e+15; // выше него нельз€ прыгать

        double BeginBottom = 0.15, EndBottom = 1;
        double InitialBottomBegin = 0.15, InitialBottomEnd = 1;
        double InitialLeftBegin = 0.15, InitialLeftEnd = 1;
        Point[] ZoomPoints = new Point[2];

        string []DrawScaleBottomText =  new string[1];
        string ValueBottomText = "";
        string ValueLeftText = "";

        public XYPlotControl()
        {
            InitializeComponent();
            InitializeGraphics();
            DigMath.FindMinMax(DrawData, _Min, _Max, ChannelEnabled);
            CalcParam();            
        }

        void InitializeGraphics()
        {
            initDirectX(this.Handle);
        }

        #region DirectX
        Mutex VidoDeviceInUse = new Mutex();
        bool LockVideoDevice()
        {           
            return VidoDeviceInUse.WaitOne();            
        }

        void UnlockVideoDevice()
        {
            VidoDeviceInUse.ReleaseMutex();
        }
        void initDirectX(IntPtr DrawHandle)
        {
            DisposeDirectX();
            if (!ThisRunning || DrawHandle == IntPtr.Zero) return;

            LockVideoDevice();

            Microsoft.DirectX.Direct3D.PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;        
            VideoDevice = new Device(0, DeviceType.Hardware, DrawHandle, CreateFlags.SoftwareVertexProcessing, presentParams);

            VideoDevice.DeviceReset += new EventHandler(VideoDevice_DeviceReset);
            VideoDevice.DeviceLost += new EventHandler(VideoDevice_DeviceLost);
            UnlockVideoDevice();

            VideoDevice_DeviceReset(VideoDevice, null);
        }

        void VideoDevice_DeviceLost(object sender, EventArgs e)
        {
            DisposeDirectX();           
        }

        void VideoDevice_DeviceReset(object sender, EventArgs e)
        {
            Device dev = (Device)sender;

            if (dev == null || dev.Disposed) return;

            if (LockVideoDevice())
            {
                // Turn off culling, so we see the front and back of the triangle
                dev.RenderState.CullMode = Cull.None;

                // Turn on the ZBuffer
                dev.RenderState.ZBufferEnable = false;
                dev.RenderState.Lighting = false;    //make sure lighting is enabled	
                dev.RenderState.SpecularEnable = false;

                if (DirectXLine != null) DirectXLine.Dispose();
                DirectXLine = new Line(dev);
                DirectXLine.Antialias = true;

                if (ScaleLine != null) ScaleLine.Dispose();
                ScaleLine = new Line(dev);
                ScaleLine.GlLines = true;

                if (GridLine != null) GridLine.Dispose();
                GridLine = new Line(dev);
                GridLine.PatternScale = 0.1f;

                if (DirectDrawFont != null) DirectDrawFont.Dispose();
                Microsoft.DirectX.Direct3D.FontDescription ThisFontDescription = new Microsoft.DirectX.Direct3D.FontDescription();
                ThisFontDescription.Height = 13;
                ThisFontDescription.Width = (int)(ThisFontDescription.Height / 3.0f);
                ThisFontDescription.Weight = FontWeight.Normal;
                ThisFontDescription.Quality = FontQuality.Default;
                DirectDrawFont = new Microsoft.DirectX.Direct3D.Font(dev, ThisFontDescription);

                if (DXFontSprite != null) DXFontSprite.Dispose();
                DXFontSprite = new Sprite(dev);               

                UnlockVideoDevice();
            }
        }

        void DisposeDirectX()
        {
            if (VideoDevice == null || VideoDevice.Disposed) return;
            if (LockVideoDevice())
            {
                try
                {
                    VideoDevice.DeviceLost -= VideoDevice_DeviceLost;
                    VideoDevice.DeviceReset -= VideoDevice_DeviceReset;
                    if (DirectXLine != null)
                    {
                        DirectXLine.Dispose();
                        DirectXLine = null;
                    }
                    if (ScaleLine != null)
                    {
                        ScaleLine.Dispose();
                        ScaleLine = null;
                    }
                    if (GridLine != null)
                    {
                        GridLine.Dispose();
                        GridLine = null;
                    }
                    if (DirectDrawFont != null)
                    {
                        DirectDrawFont.Dispose();
                        DirectDrawFont = null;
                    }
                    if (DXFontSprite != null)
                    {
                        DXFontSprite.Dispose();
                        DXFontSprite = null;
                    }

                    VideoDevice.Dispose();                    
                }               
                finally
                {
                    UnlockVideoDevice();
                }
            }
        }
        #endregion

        #region SetData
        public int GetSetNumDrawPset
        {
            set
            {
                if (LockVideoDevice())
                {
                    NumPointsToDrawPerChannel = value;
                    CalcParam();
                    UnlockVideoDevice();
                }
                Redraw(ref ZeroData, true);
            }

            get
            {
                return NumPointsToDrawPerChannel;
            }
        }

        public int GetSetNumChannels
        {
            set
            {
                if (LockVideoDevice())
                {
                    NumChannels = value;
                    ChannelEnabled = new bool[NumChannels];
                    _Min = new double[NumChannels + 1];
                    _Max = new double[NumChannels + 1];
                    _NewMin = new double[NumChannels + 1];
                    _NewMax = new double[NumChannels + 1];
                    ChannelDrawRectangle = new Rectangle[NumChannels];
                    CalcParam();
                    UnlockVideoDevice();
                }

                Redraw(ref ZeroData, true);
            }
            get
            {
                return NumChannels;
            }
        }

        public bool[] GetSetChannelEnabled
        {
            set
            {
                ChannelEnabled = value;
                Redraw(ref ZeroData, true);
            }
            get
            {
                return ChannelEnabled;
            }
        }

        public bool GetSetSplitChannels
        {
            set
            {
                SplitChannels = value;
                Redraw(ref ZeroData, true);
            }
            get
            {
                return SplitChannels;
            }
        }

        public Color[] GetSetChannelColor
        {
            set
            {
                DrawChannelColors = value;
                Redraw(ref ZeroData, true);
            }
            get
            {
                return DrawChannelColors;
            }
        }

        public Color GetSetBackGroundColor
        {
            set
            {
                DrawBackgroundColor = value;
                if ((DrawBackgroundColor.B + DrawBackgroundColor.R + DrawBackgroundColor.G) < 255) PointerColor = Color.White;
                else PointerColor = Color.Black;
                Redraw(ref ZeroData, true);
            }
            get
            {
                return DrawBackgroundColor;
            }
        }

        public Color GetSetGridColor
        {
            set
            {
                DrawGridColor = value;
                Redraw(ref ZeroData, true);
            }
            get
            {
                return DrawGridColor;
            }
        }

        public Color GetSetScaleColor
        {
            set
            {
                DrawScaleColor = value;                
                Redraw(ref ZeroData, true);
            }
            get
            {
                return DrawScaleColor;
            }
        }

        public double BeginTime
        {
            set
            {
                if (LockVideoDevice())
                {
                    BeginBottom = value;
                    InitialBottomBegin = value;                    
                    CalcParam();
                    UnlockVideoDevice();
                }
                Redraw(ref ZeroData, true);
            }
            get
            {
                return InitialBottomBegin;
            }
        }

        public double EndTime
        {
            set
            {
                if (LockVideoDevice())
                {
                    EndBottom = value;
                    InitialBottomEnd = value;                    
                    CalcParam();
                    UnlockVideoDevice();
                }
                Redraw(ref ZeroData, true);
            }
            get
            {
                return InitialBottomEnd;
            }
        }



        public bool SetLogScaleBottom
        {
            set
            {
                LogScaleBottom = value;
            }
        }

        public bool SetLogScaleLeft
        {
            set
            {
                LogScaleLeft = value;
            }
        }

        public bool SetPause
        {
            set
            {
                DrawPause = value;
            }
        }

        public string SetValueBottomText
        {
            set
            {
                ValueBottomText = value;
                Redraw(ref ZeroData, true);
            }
        }

        public string SetValueLeftText
        {
            set
            {
                ValueLeftText = value;
                Redraw(ref ZeroData, true);
            }
        }

        DateTime LastTimeFPS = DateTime.Now;
        TimeSpan FPSTimeSpan;
        uint CounterFPS = 0;
        double FPSCounter = 0;
        public bool SetShowFPS
        {
            set
            {
                ShowFPS = value;
            }
        }

        public double SetTotalMin
        {
            set
            {
                TotalMin = value;
                if (TotalMax <= TotalMin)
                    TotalMax = TotalMin + 1e-15;
                for (int i = 0; i <= NumChannels; i++)
                {
                    _Min[i] = Math.Max(TotalMin, _Min[i]);
                }
                Redraw(ref ZeroData, true);
            }
            get
            {
                return TotalMin;
            }
        }

        public double SetTotalMax
        {
            set
            {
                TotalMax = value;
                if (TotalMin >= TotalMax)
                    TotalMin = TotalMax - 1e-15;
                for (int i = 0; i <=NumChannels;i++ )
                {
                    _Max[i] = Math.Min(TotalMax, _Max[i]);
                }
                Redraw(ref ZeroData, true);
            }

            get
            {
                return TotalMax;
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

        #region ControlChanged
        
        protected override void OnSizeChanged(EventArgs e)
        {            
            if (LockVideoDevice())
            {
                base.OnSizeChanged(e);

                if (this.Width >(DrawOffset.X+1) && this.Height >(DrawOffset.Y+1))
                {
                    CalcParam();
                    initDirectX(this.Handle);
                }
                else
                {
                    DisposeDirectX();
                }

                UnlockVideoDevice();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Redraw(ref ZeroData, true);
        }

        protected override void CreateHandle()
        {
            ThisRunning = true;
            base.CreateHandle();
        }

        protected override void DestroyHandle()
        {            
            DisposeDirectX();
            base.DestroyHandle();
            ThisRunning = false;
        }
        
        bool DrawCurrentValues(MouseEventArgs CurMouseArgs)
        {
            if (VideoDevice == null||VideoDevice.Disposed) return false;

            if (CurMouseArgs.Button != MouseButtons.Left || !MouseDowned) return false;

            if (Cursor.Current != null)
            {
                Cursor.Current.Dispose();
                Cursor.Current = null;
            }

            Redraw(ref ZeroData, false);

            int CoordX = CurMouseArgs.X;
            int CoordY = CurMouseArgs.Y;

            CoordX = Math.Min(CoordX, DrawRectangle.Right);
            CoordX = Math.Max(CoordX, DrawRectangle.Left);
            CoordY = Math.Min(CoordY, DrawRectangle.Bottom);
            CoordY = Math.Max(CoordY, DrawRectangle.Top);

            double ValueBottom = 0;
            if (LogScaleBottom)
            {
                ValueBottom = _CalcXValueLog(CoordX, BeginBottom, EndBottom);
            }
            else
            {
                ValueBottom = _CalcXValue(CoordX, BeginBottom, EndBottom);
            }

            double ValueLeft = 0;
            if (SplitChannels)
            {
                for (int i = 0; i < NumChannels; i++)
                {
                    if (CurMouseArgs.Y <= ChannelDrawRectangle[i].Bottom && CurMouseArgs.Y >= ChannelDrawRectangle[i].Top)
                    {
                        ValueLeft = _CalcYValueFull(CoordY, _Min[i], _Max[i], ChannelDrawRectangle[i]);
                        break;
                    }
                }
                    
            }
            else
            {
                ValueLeft = _CalcYValue(CoordY, _Min[NumChannels], _Max[NumChannels]);
            }

            Color ClearColor = PointerBackColor;            

            LockVideoDevice();
            VideoDevice.BeginScene();
            // вертикальна€ лини€
            drawScale1[0].X = drawScale1[1].X = CoordX;
            drawScale1[0].Y = DrawRectangle.Bottom;
            drawScale1[1].Y = DrawRectangle.Top;
            ScaleLine.Draw(drawScale1, PointerColor);

            // горизонтальна€ лини€
            drawScale2[0].Y = drawScale2[1].Y = CoordY;
            drawScale2[0].X = DrawRectangle.Left;
            drawScale2[1].X = DrawRectangle.Right;
            ScaleLine.Draw(drawScale2, PointerColor);                        
            
            // снизу
            string DrawValue = ValueBottom.ToString("f6");
            string DrawValue1 = ValueBottom.ToString();
            if (DrawValue1.Length < DrawValue.Length) DrawValue = DrawValue1;

            DrawValue += " " + ValueBottomText;
            int DrawValueWidth = DirectDrawFont.Description.Width * (DrawValue.Length + 6);             
            Rectangle DrawTextRect = new Rectangle(new Point((int)drawScale1[0].X - (DrawValueWidth / 2), DrawRectangle.Bottom), new Size(DrawValueWidth, DirectDrawFont.Description.Height+1));
            VideoDevice.Clear(ClearFlags.Target, ClearColor, 1.0f, 0, new Rectangle[1] { DrawTextRect });
            DirectDrawFont.DrawText(null, DrawValue, DrawTextRect, DrawTextFormat.Center | DrawTextFormat.NoClip, DrawScaleColor);

            // слева
            DrawValue = ValueLeft.ToString("f3");
            DrawValue1 = ValueLeft.ToString();
            if (DrawValue1.Length < DrawValue.Length) DrawValue = DrawValue1;
           
            DrawValueWidth = (DirectDrawFont.Description.Width+1) * DrawValue.Length;
            DrawValueWidth = Math.Max(DrawValueWidth, DrawOffset.X);
            DrawTextRect = new Rectangle(new Point(0, (int)drawScale2[0].Y - (DirectDrawFont.Description.Height / 2)), new Size(DrawValueWidth, DirectDrawFont.Description.Height + 1));
            VideoDevice.Clear(ClearFlags.Target, ClearColor, 1.0f, 0, new Rectangle[1] { DrawTextRect });
            DirectDrawFont.DrawText(null, DrawValue, DrawTextRect, DrawTextFormat.Left, DrawScaleColor);                                 

            VideoDevice.EndScene();
            VideoDevice.Present();
            UnlockVideoDevice();            

            return true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseEntered)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!DrawCurrentValues(e))
                    {
                        Cursor.Current = Cursors.Cross;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (VideoDevice == null || VideoDevice.Disposed) return;
                    Redraw(ref ZeroData, false);
                    LockVideoDevice();
                    VideoDevice.BeginScene();

                    if (!SplitChannels)
                    {
                        drawScale1[0].X = ZoomPoints[0].X;
                        drawScale1[0].Y = ZoomPoints[0].Y;
                        drawScale1[1].X = e.X;
                        drawScale1[1].Y = ZoomPoints[0].Y;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[0].X = e.X;
                        drawScale1[0].Y = e.Y;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[1].X = ZoomPoints[0].X;
                        drawScale1[1].Y = e.Y;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[0].X = ZoomPoints[0].X;
                        drawScale1[0].Y = ZoomPoints[0].Y;
                        ScaleLine.Draw(drawScale1, PointerColor);
                    }
                    else
                    {
                        drawScale1[0].X = ZoomPoints[0].X;
                        drawScale1[0].Y = DrawRectangle.Top;
                        drawScale1[1].X = e.X;
                        drawScale1[1].Y = DrawRectangle.Top;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[0].X = e.X;
                        drawScale1[0].Y = DrawRectangle.Bottom;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[1].X = ZoomPoints[0].X;
                        drawScale1[1].Y = DrawRectangle.Bottom;
                        ScaleLine.Draw(drawScale1, PointerColor);

                        drawScale1[0].X = ZoomPoints[0].X;
                        drawScale1[0].Y = DrawRectangle.Top;
                        ScaleLine.Draw(drawScale1, PointerColor);
                    }

                    VideoDevice.EndScene();
                    VideoDevice.Present();
                    UnlockVideoDevice();
                }
                else
                {
                    if (Cursor.Current==null)
                        Cursor.Current = Cursors.Cross;
                }
            }
        }

        bool MouseEntered = false;
        protected override void OnMouseEnter(EventArgs e)
        {            
            MouseEntered = true;
            Cursor.Current = Cursors.Cross;
        }

        protected override void OnMouseLeave(EventArgs e)
        {            
            MouseEntered = false;
            Cursor.Current = Cursors.Default;
        }

        bool MouseDowned = false;
        bool LastPauseValue = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {            
            if (MouseEntered)
            {
                MouseDowned = true;
                LastPauseValue = DrawPause;
                DrawPause = true;     

                if (e.Button == MouseButtons.Left)
                {
                    DrawCurrentValues(e);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    ZoomPoints[0] = e.Location;
                }
            }
        }

        void PrepareToDrawSize(ref int X, ref int Y)
        {
            X = Math.Max(DrawRectangle.Left, X);
            X = Math.Min(DrawRectangle.Right, X);

            Y = Math.Min(DrawRectangle.Bottom, Y);
            Y = Math.Max(DrawRectangle.Top, Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseDowned = false;

            if (MouseEntered)
            {
                DrawPause = LastPauseValue;

                if (e.Button == MouseButtons.Left)
                {
                    Redraw(ref ZeroData, true);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (VideoDevice == null || VideoDevice.Disposed) return;

                    int ZoomXBegin = Math.Min(e.X, ZoomPoints[0].X);
                    int ZoomXEnd = Math.Max(e.X, ZoomPoints[0].X);

                    // поскольку ось Y инвертирована
                    int ZoomYBegin = Math.Max(e.Y, ZoomPoints[0].Y);
                    int ZoomYEnd = Math.Min(e.Y, ZoomPoints[0].Y);

                    PrepareToDrawSize(ref ZoomXBegin, ref ZoomYBegin);
                    PrepareToDrawSize(ref ZoomXEnd, ref ZoomYEnd);

                    double NewBeginBottom = 0;
                    double NewEndBottom = 0;

                    if (LogScaleBottom)
                    {
                        NewBeginBottom = _CalcXValueLog(ZoomXBegin, BeginBottom, EndBottom);
                        NewEndBottom = _CalcXValueLog(ZoomXEnd, BeginBottom, EndBottom);
                    }
                    else
                    {
                        NewBeginBottom = _CalcXValue(ZoomXBegin, BeginBottom, EndBottom);
                        NewEndBottom = _CalcXValue(ZoomXEnd, BeginBottom, EndBottom);
                    }

                    BeginBottom = NewBeginBottom;
                    EndBottom = NewEndBottom;

                    if (!SplitChannels)
                    { // еще и мен€ем остальное
                        PossibleCalcMinMax = false;

                        double NewBeginLeft = _CalcYValue(ZoomYBegin, _Min[NumChannels], _Max[NumChannels]);
                        double NewEndLeft = _CalcYValue(ZoomYEnd, _Min[NumChannels], _Max[NumChannels]);

                        _Min[NumChannels] = NewBeginLeft;
                        _Max[NumChannels] = NewEndLeft;
                    }

                    if (e.X > this.Width || e.X<0 || e.Y > this.Height || e.Y < 0)
                    {
                        PossibleCalcMinMax = true;

                        BeginBottom = InitialBottomBegin;
                        EndBottom = InitialBottomEnd;
                        
                        _Min[NumChannels] = InitialLeftBegin;
                        _Max[NumChannels] = InitialLeftEnd;
                    }                    

                    CalcParam();
                    Redraw(ref ZeroData, true);
                }
            }           
        }        

        #endregion

        public void CalcScaleNow()
        {
            if (LockVideoDevice())
            {
                TotalMin = -1e+15; // ниже него нельз€ прыгать
                TotalMax = 1e+15; // выше него нельз€ прыгать

                PossibleCalcMinMax = true;
                RecalcMinMax(0.9, 0.7);
                UnlockVideoDevice();
            }
        }

        void CalcParam()
        {
            DrawRectangle = new Rectangle(DrawOffset, new Size(this.Width - DrawOffset.X, this.Height - 20));
            
            int NumPointsSpace = 3;
            for (int i = 0; i < NumChannels; i++)
            {
                ChannelDrawRectangle[i] = new Rectangle(new Point(DrawRectangle.Left, (DrawRectangle.Height * i / NumChannels) + NumPointsSpace),
                                                        new Size(DrawRectangle.Width, (DrawRectangle.Height / NumChannels) - NumPointsSpace));
            }

            DrawRectangleScaleBottom = new Rectangle(new Point(DrawRectangle.Left, DrawRectangle.Bottom),
                                                                            new Size(DrawRectangle.Width, 10 + 13)); // 13 - высота фонта

            DrawRectangleScaleLeft = new Rectangle(new Point(0, 0), new Size(DrawOffset.X, DrawRectangle.Height));

            if (DrawData.GetLength(0) != NumChannels || DrawData.GetLength(1) != NumPointsToDrawPerChannel)
            {
                DrawData = new double[NumChannels, NumPointsToDrawPerChannel];
            }            
            drawPoints = new Vector2[NumPointsToDrawPerChannel];
            drawPointsXCoord = new float[NumPointsToDrawPerChannel];

            double BottomOffset = BeginBottom-InitialBottomBegin;
            double BottomScale = (EndBottom - BeginBottom)/(InitialBottomEnd - InitialBottomBegin);
            double BottomOffsetToPset = BottomOffset * NumPointsToDrawPerChannel / (InitialBottomEnd - InitialBottomBegin);            
            for (int i = 0; i<NumPointsToDrawPerChannel; i++)
            {
                if (LogScaleBottom)
                {
                    drawPointsXCoord[i] = _CalcXCoordLog(i, BottomOffsetToPset, BottomOffsetToPset + (NumPointsToDrawPerChannel * BottomScale));                    
                }
                else
                {
                    drawPointsXCoord[i] = _CalcXCoord(i, BottomOffsetToPset, BottomOffsetToPset + (NumPointsToDrawPerChannel * BottomScale));                    
                }                
            }

            // рамка обща€
            drawScaleRectangle = new Vector2[5];
            drawScaleRectangle[0] = new Vector2(DrawRectangle.Left, DrawRectangle.Top);
            drawScaleRectangle[1] = new Vector2(DrawRectangle.Right, DrawRectangle.Top);
            drawScaleRectangle[2] = new Vector2(DrawRectangle.Right, DrawRectangle.Bottom);
            drawScaleRectangle[3] = new Vector2(DrawRectangle.Left, DrawRectangle.Bottom);
            drawScaleRectangle[4] = drawScaleRectangle[0];

            RecalcMinMax(0.9, 0.7);

            DrawBottomScale(true);            
        }

        #region MathFunc
        float _CalcXCoord(double Value, double Min, double Max)
        {
            double Result = (DrawRectangle.Left + ((double)(Value - Min) * (DrawRectangle.Width+1) / (Max - Min)));           
            return (float)Result;
        }    

        double _CalcXValue(float Coord, double Min, double Max)
        {
            double Result = (Min + ((double)(Coord - DrawRectangle.Left) * (Max - Min) / (DrawRectangle.Width+1)));
            Result = Math.Min(Result, Max);
            Result = Math.Max(Result, Min);
            return Result;
        }

        float _CalcXCoordLog(double Value, double Min, double Max)
        {
            double FirstLog = (Value - Min);
            FirstLog = Math.Max(FirstLog,1);            
            return (float)(DrawRectangle.Left + (Math.Log10(FirstLog) * (DrawRectangle.Width+1) / Math.Log10(Max - Min)));
        }

        double _CalcXValueLog(float Coord, double Min, double Max)
        {
            double FirstLog = (Coord - DrawRectangle.Left);
            FirstLog = Math.Max(FirstLog, 1);
            double Result = (FirstLog * Math.Log10(Max - Min) / (DrawRectangle.Width+1));
            return Math.Pow(10, Result) + Min;
        }

        float _CalcYCoord(double Value, double Min, double Max)
        {
            return (float)(DrawRectangle.Bottom - ((double)(Value - Min) * DrawRectangle.Height / (Max - Min)));
        }

        double _CalcYValue(float Coord, double Min, double Max)
        {
            return (float)(Min + ((double)(DrawRectangle.Bottom-Coord) * (Max - Min) / DrawRectangle.Height));
        }

        float _CalcYCoordFull(double Value, double Min, double Max, Rectangle Rect)
        {
            return (float)(Rect.Bottom - ((double)(Value - Min) * Rect.Height / (Max - Min)));
        }

        float _CalcYValueFull(double Coord, double Min, double Max, Rectangle Rect)
        {
            return (float)(Min + ((double)(Rect.Bottom - Coord) * (Max - Min) / Rect.Height));            
        }
        #endregion

        #region DrawScale        
        void DrawBottomScale(bool Recalc)
        {
            if (Recalc)
            {
                // временна€ шкала   
                double xStep = DigMath.FindPadding(BeginBottom, EndBottom);

                double xStart = BeginBottom + DigMath.mod(BeginBottom, xStep) - xStep;

                int NumPsetToTime = Math.Abs((int)((EndBottom - BeginBottom) / xStep)) + 2;

                drawScaleGridVertical = new Vector2[NumPsetToTime * 3];
                drawScaleBottom = new Vector2[(NumPsetToTime * 3) + (NumPsetToTime * 3 * 5)];
                DrawScaleBottomText = new string[NumPsetToTime];

                if (!LogScaleBottom)
                { // обычна€ шкала

                    for (int CntGrid = 0, CntScale = 0, TextCount = 0; CntScale < drawScaleBottom.Length; xStart += xStep, CntGrid += 3)
                    {

                        drawScaleGridVertical[CntGrid].X =
                                _CalcXCoord(xStart, BeginBottom, EndBottom);

                        if (drawScaleGridVertical[CntGrid].X < DrawRectangle.Left)
                        {
                            drawScaleGridVertical[CntGrid].X = DrawRectangle.Left;
                            DrawScaleBottomText[TextCount++] = null;
                        }
                        else
                        {
                            drawScaleGridVertical[CntGrid].Y = DrawRectangle.Bottom;
                            drawScaleGridVertical[CntGrid + 1].Y = DrawRectangle.Top;
                            drawScaleGridVertical[CntGrid + 2].Y = drawScaleGridVertical[CntGrid].Y;

                            DrawScaleBottomText[TextCount++] = xStart.ToString("e1");
                        }

                        drawScaleGridVertical[CntGrid + 1].X = drawScaleGridVertical[CntGrid + 2].X =
                            drawScaleBottom[CntScale].X = drawScaleBottom[CntScale + 1].X =
                            drawScaleBottom[CntScale + 2].X = drawScaleGridVertical[CntGrid].X;

                        drawScaleBottom[CntScale].Y = DrawRectangle.Bottom + 2;
                        drawScaleBottom[CntScale + 1].Y = drawScaleBottom[CntScale].Y + 5;
                        drawScaleBottom[CntScale + 2].Y = drawScaleBottom[CntScale].Y;
                        CntScale += 3;

                        double MiddleSize = (double)xStep / 5;
                        double OffsetMiddle = (double)xStart + MiddleSize;
                        for (int i = 0; i < 5; i++, OffsetMiddle += MiddleSize)
                        {

                            drawScaleBottom[CntScale].X = _CalcXCoord(OffsetMiddle, BeginBottom, EndBottom);


                            if (drawScaleBottom[CntScale].X < DrawRectangle.Left)
                            {
                                drawScaleBottom[CntScale].X = DrawRectangle.Left;
                            }

                            drawScaleBottom[CntScale + 1].X = drawScaleBottom[CntScale + 2].X = drawScaleBottom[CntScale].X;

                            drawScaleBottom[CntScale].Y = DrawRectangle.Bottom + DrawOffset.Y + 2;
                            drawScaleBottom[CntScale + 1].Y = drawScaleBottom[CntScale].Y + 3;
                            drawScaleBottom[CntScale + 2].Y = drawScaleBottom[CntScale].Y;
                            CntScale += 3;
                        }
                    }
                }
                else
                { // логарифмическа€ шкала                    
                    double LastGridPosotion = 0;
                    for (int CntGrid = 0, CntScale = 0, TextCount = 0; CntScale < drawScaleBottom.Length; xStart += xStep, CntGrid += 3, TextCount++)
                    {
                        drawScaleGridVertical[CntGrid].X =
                                _CalcXCoordLog((double)xStart, BeginBottom, EndBottom);

                        if (drawScaleGridVertical[CntGrid].X < DrawRectangle.Left || xStart<BeginBottom)
                        {
                            drawScaleGridVertical[CntGrid].X = DrawRectangle.Left;
                            DrawScaleBottomText[TextCount] = null;
                        }
                        else
                        {
                            drawScaleGridVertical[CntGrid].Y = DrawRectangle.Bottom + DrawOffset.Y;
                            drawScaleGridVertical[CntGrid + 1].Y = DrawRectangle.Top + DrawOffset.Y;
                            drawScaleGridVertical[CntGrid + 2].Y = drawScaleGridVertical[CntGrid].Y;

                            DrawScaleBottomText[TextCount] = xStart.ToString();
                        }

                        drawScaleGridVertical[CntGrid + 1].X = drawScaleGridVertical[CntGrid + 2].X =
                            drawScaleBottom[CntScale].X = drawScaleBottom[CntScale + 1].X =
                            drawScaleBottom[CntScale + 2].X = drawScaleGridVertical[CntGrid].X;

                        drawScaleBottom[CntScale].Y = DrawRectangle.Bottom + DrawOffset.Y + 2;
                        drawScaleBottom[CntScale + 1].Y = drawScaleBottom[CntScale].Y + 5;
                        drawScaleBottom[CntScale + 2].Y = drawScaleBottom[CntScale].Y;
                        CntScale += 3;

                        if ((drawScaleGridVertical[CntGrid].X - LastGridPosotion) < 30)
                        {
                            DrawScaleBottomText[TextCount] = null;
                        }
                        else
                        {
                            LastGridPosotion = drawScaleGridVertical[CntGrid].X;
                        }

                        double MiddleSize = (double)xStep / 5;
                        double OffsetMiddle = (double)xStart + MiddleSize;
                        for (int i = 0; i < 5; i++, OffsetMiddle += MiddleSize)
                        {
                            drawScaleBottom[CntScale].X = _CalcXCoordLog(OffsetMiddle, BeginBottom, EndBottom);

                            if (drawScaleBottom[CntScale].X < DrawRectangle.Left)
                            {
                                drawScaleBottom[CntScale].X = DrawRectangle.Left;
                            }

                            drawScaleBottom[CntScale + 1].X = drawScaleBottom[CntScale + 2].X = drawScaleBottom[CntScale].X;

                            drawScaleBottom[CntScale].Y = DrawRectangle.Bottom + DrawOffset.Y + 2;
                            drawScaleBottom[CntScale + 1].Y = drawScaleBottom[CntScale].Y + 3;
                            drawScaleBottom[CntScale + 2].Y = drawScaleBottom[CntScale].Y;
                            CntScale += 3;
                        }
                    }
                }
            }
            else
            {
                if (VideoDevice == null || VideoDevice.Disposed) return;
                GridLine.Begin();
                for (int i = 0; i < drawScaleGridVertical.Length; i += 3)
                {
                    if (drawScaleGridVertical[i].X > DrawOffset.X)
                    {
                        drawScale1[0] = drawScaleGridVertical[i];
                        drawScale1[1] = drawScaleGridVertical[i + 1];
                        GridLine.Draw(drawScale1, DrawGridColor);
                    }
                }
                GridLine.End();
                ScaleLine.Draw(drawScaleBottom, DrawScaleColor);

                DXFontSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
                DirectDrawFont.PreloadCharacters(0, 128);
                for (int i = 0; i < DrawScaleBottomText.Length; i++)
                {
                    if (DrawScaleBottomText[i] == null) continue;
                    double FirstPosition = drawScaleBottom[i * 18].X - (DirectDrawFont.Description.Width * DrawScaleBottomText[i].Length / 2);
                    DirectDrawFont.DrawText(DXFontSprite, DrawScaleBottomText[i], new Point((int)FirstPosition, DrawRectangle.Bottom + 5), DrawScaleColor);
                }
                DXFontSprite.End();
            }
        }


        void DrawLeftScale()
        {
            if (VideoDevice == null || VideoDevice.Disposed) return;
            int NumMiddlePoint = 5;            
            ScaleLine.Begin();
            if (SplitChannels)
            { // разделить каналы
                NumMiddlePoint = 3;
                // рисуем делительные полосы
                for (int Chan = 0; Chan < NumChannels; Chan++)
                {
                    Rectangle CellRect = ChannelDrawRectangle[Chan];

                    // рамка
                    drawScale1[0].X = CellRect.Left;
                    drawScale1[0].Y = CellRect.Top;
                    drawScale1[1].X = CellRect.Right;
                    drawScale1[1].Y = CellRect.Top;
                    ScaleLine.Draw(drawScale1, PointerColor);
                    drawScale1[0].X = CellRect.Right;
                    drawScale1[0].Y = CellRect.Bottom;
                    ScaleLine.Draw(drawScale1, PointerColor);
                    drawScale1[1].X = CellRect.Left;
                    drawScale1[1].Y = CellRect.Bottom;
                    ScaleLine.Draw(drawScale1, PointerColor);
                    drawScale1[0].X = CellRect.Left;
                    drawScale1[0].Y = CellRect.Top;
                    ScaleLine.Draw(drawScale1, PointerColor);

                    if (!ChannelEnabled[Chan]) continue;

                    // лини€ нул€
                    drawScale1[0].X = CellRect.Left;
                    drawScale1[1].Y = drawScale1[0].Y = _CalcYCoordFull(0, _Min[Chan], _Max[Chan], CellRect);
                    drawScale1[1].X = CellRect.Right;
                    if (drawScale1[1].Y > CellRect.Top && drawScale1[1].Y < CellRect.Bottom)
                    {
                        GridLine.Draw(drawScale1, DrawGridColor);
                    }

                    double xStep = DigMath.FindPadding(_Min[Chan], _Max[Chan]);

                    int NumPset = (int)((_Max[Chan] - _Min[Chan]) / xStep);
                    if (NumPset > 2) xStep = xStep * (NumPset / 2);

                    double xStart = (((int)(_Min[Chan] / xStep)) * xStep) - xStep;

                    // шкала слева
                    drawScale1[0].X = CellRect.Left - 2;
                    drawScale1[1].X = CellRect.Left - 2;
                    drawScale1[0].Y = CellRect.Top;
                    drawScale1[1].Y = CellRect.Bottom;
                    ScaleLine.Draw(drawScale1, DrawScaleColor);

                    drawScale1[0].X = CellRect.Left - 2;
                    drawScale1[1].X = drawScale1[0].X - 5;

                    int NumPositions = (DrawOffset.X / DirectDrawFont.Description.Width) - 2;

                    DXFontSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
                    DirectDrawFont.PreloadCharacters(0, 128);           
                    for (; xStart <= _Max[Chan]; xStart += xStep)
                    {
                        drawScale1[0].Y = _CalcYCoordFull(xStart, _Min[Chan], _Max[Chan], CellRect);
                        if (drawScale1[0].Y > CellRect.Bottom)
                        {
                            drawScale1[0].Y = CellRect.Bottom;
                        }
                        else
                        {
                            string Value = xStart.ToString();
                            int PsetPosition = Value.IndexOf(",");
                            if (Value.LastIndexOf('e') == -1) Value = Value.Substring(0, Math.Max(Math.Min(NumPositions, Value.Length), PsetPosition));
                            double FirstPosition = drawScale1[0].Y - (DirectDrawFont.Description.Height / 2);
                            if (FirstPosition < 0) FirstPosition = 0;
                            DirectDrawFont.DrawText(DXFontSprite, Value, new Point(1, (int)FirstPosition), DrawScaleColor);
                        }

                        drawScale1[1].Y = drawScale1[0].Y;
                        ScaleLine.Draw(drawScale1, DrawScaleColor);
                    }
                    DXFontSprite.End();
                }
            }
            else
            {
                double xStep = DigMath.FindPadding(_Min[NumChannels], _Max[NumChannels]);

                double xStart = (((int)(_Min[NumChannels] / xStep)) * xStep) - xStep;

                drawScale1[0].X = DrawRectangle.Left - 2;
                drawScale1[1].X = DrawRectangle.Left - 2;
                drawScale1[0].Y = DrawRectangle.Top;
                drawScale1[1].Y = DrawRectangle.Bottom;
                ScaleLine.Draw(drawScale1, DrawScaleColor);

                drawScale1[0].X = DrawRectangle.Left;
                drawScale1[1].X = DrawRectangle.Right;

                drawScale2[0].X = DrawRectangle.Left - 2;
                drawScale2[1].X = drawScale2[0].X - 5;

                drawScale3[0].X = DrawRectangle.Left - 2;
                drawScale3[1].X = drawScale2[0].X - 3;

                int NumPositions = (DrawOffset.X / DirectDrawFont.Description.Width) - 2;

                DXFontSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
                DirectDrawFont.PreloadCharacters(0, 128);     
                for (; xStart <= _Max[NumChannels]; xStart += xStep)
                {
                    drawScale1[0].Y = _CalcYCoord(xStart, _Min[NumChannels], _Max[NumChannels]);
                    if (drawScale1[0].Y > DrawRectangle.Bottom)
                    {
                        drawScale1[0].Y = DrawRectangle.Bottom;
                    }
                    else
                    {
                        string Value = xStart.ToString();
                        int PsetPosition = Value.IndexOf(",");
                        Value = Value.Substring(0, Math.Max(Math.Min(NumPositions, Value.Length), PsetPosition));
                        double FirstPosition = drawScale1[0].Y - (DirectDrawFont.Description.Height / 2);
                        if (FirstPosition < 0) FirstPosition = 0;
                        DirectDrawFont.DrawText(DXFontSprite, Value, new Point(1, (int)FirstPosition), DrawScaleColor);
                    }

                    drawScale2[0].Y = drawScale2[1].Y =
                        drawScale1[1].Y = drawScale1[0].Y;
                    GridLine.Draw(drawScale1, DrawGridColor);
                    ScaleLine.Draw(drawScale2, DrawScaleColor);

                    for (int i = 0; i < NumMiddlePoint; i++)
                    {
                        drawScale3[0].Y = drawScale3[1].Y = _CalcYCoord(xStart + (xStep * i / NumMiddlePoint), _Min[NumChannels], _Max[NumChannels]);
                        if (drawScale3[0].Y > DrawRectangle.Bottom) continue;
                        ScaleLine.Draw(drawScale3, DrawScaleColor);
                    }
                }
                DXFontSprite.End();

                ScaleLine.Draw(drawScaleRectangle, DrawScaleColor);
            }
           
            ScaleLine.End();
        }
        #endregion

        #region CalculationMinMax
        /*
         * обычно 0.9 и 0.7 
         * но дл€ большого демпфировани€
         * 0.95 и 0.05
         */
        void RecalcMinMax(double UpZapas, double DownZapas)
        {
            if (!PossibleCalcMinMax) return;
            DigMath.FindMinMax(DrawData, _NewMin, _NewMax, ChannelEnabled);
            int BeginValue = 0;
            int EndValue = NumChannels;
            if (!SplitChannels)
            {
                BeginValue = NumChannels;
                EndValue = NumChannels + 1;
            }
            // чтобы шкала не сильно дергалась
            for (int i = BeginValue; i <EndValue; i++)
            {                
                if (_NewMax[i] < double.MaxValue && _NewMin[i] > double.MinValue)
                {
                    double OldMinMaxMiddle = ((_Max[i] - _Min[i]) / 2) + _Min[i];
                    double NewMaxDiff = _NewMax[i] - OldMinMaxMiddle;
                    double OldMaxDiff = (_Max[i] - OldMinMaxMiddle);
                    double NewMinDiff = OldMinMaxMiddle - _NewMin[i];
                    double OldMinDiff = OldMinMaxMiddle - _Min[i];

                    if (NewMaxDiff > OldMaxDiff * UpZapas ||
                       NewMaxDiff < OldMaxDiff * DownZapas)
                    {
                        double Zapas = (_NewMax[i] - _NewMin[i]) * 0.1f;
                        _Max[i] = _NewMax[i] + Zapas;                        
                        _Max[i] = Math.Min(TotalMax, _Max[i]);
                        
                    }

                    if (NewMinDiff > OldMinDiff * UpZapas ||
                        NewMinDiff < OldMinDiff * DownZapas)
                    {
                        double Zapas = (_NewMax[i] - _NewMin[i]) * 0.1f;
                        _Min[i] = _NewMin[i] - Zapas;                        
                        _Min[i] = Math.Max(TotalMin, _Min[i]);                        
                    }
                }
            }

            InitialLeftBegin = _Min[NumChannels];
            InitialLeftEnd = _Max[NumChannels];
        }
       
        #endregion

        void CutPsetBorder(ref Vector2[] DrPoints, int Position, Rectangle DrRect)
        {
            DrPoints[Position].X = Math.Min(DrPoints[Position].X, DrRect.Right);
            DrPoints[Position].X = Math.Max(DrPoints[Position].X, DrRect.Left);
            DrPoints[Position].Y = Math.Min(DrPoints[Position].Y, DrRect.Bottom);
            DrPoints[Position].Y = Math.Max(DrPoints[Position].Y, DrRect.Top);
        }

        void CalcPsetBorder(ref Vector2[] DrPoints, int Position, ref Rectangle DrRect)
        {
            if (Position == 0) return;            

            float k = (DrPoints[Position].Y - DrPoints[Position - 1].Y) / (DrPoints[Position].X - DrPoints[Position - 1].X);
            float b = DrPoints[Position].Y - (k * DrPoints[Position].X);

            if (float.IsNaN(k)||float.IsInfinity(k)) return;

            if (DrPoints[Position].X > DrRect.Right)
            {
                DrPoints[Position].Y = (k * DrRect.Right) + b;
                DrPoints[Position].X = DrRect.Right;
                return;
            }
            else if (DrPoints[Position - 1].X > DrRect.Right)
            {
                DrPoints[Position - 1].Y = (k * DrRect.Right) + b;
                DrPoints[Position - 1].X = DrRect.Right;
                return;
            }            

            if (DrPoints[Position].X < DrRect.Left)
            {
                DrPoints[Position].Y = (k * DrRect.Left) + b;
                DrPoints[Position].X = DrRect.Left;
                return;
            }
            else if (DrPoints[Position - 1].X < DrRect.Left)
            {
                DrPoints[Position - 1].Y = (k * DrRect.Left) + b;
                DrPoints[Position - 1].X = DrRect.Left;
                return;
            }

            if (DrPoints[Position].Y < DrRect.Top)
            {
                DrPoints[Position].X = (DrRect.Top - b) / k;
                DrPoints[Position].Y = DrRect.Top;
                return;
            }
            else if (DrPoints[Position - 1].Y < DrRect.Top)
            {
                DrPoints[Position - 1].X = (DrRect.Top - b) / k;
                DrPoints[Position - 1].Y = DrRect.Top;
                return;
            }


            if (DrPoints[Position].Y > DrRect.Bottom)
            {
                DrPoints[Position].X = (DrRect.Bottom - b) / k;
                DrPoints[Position].Y = DrRect.Bottom;
                return;
            }
            else if (DrPoints[Position - 1].Y > DrRect.Bottom)
            {
                DrPoints[Position - 1].X = (DrRect.Bottom - b) / k;
                DrPoints[Position - 1].Y = DrRect.Bottom;
                return;
            }           
        }     

        void CalcRightBorderPoints(ref Vector2[] DrPoints, ref Rectangle DrRect)
        {            
            int BeginPosition=0;
            for (; BeginPosition < DrPoints.Length; BeginPosition++)
            {
                if (DrPoints[BeginPosition].X >= DrRect.Left)
                {              
                    if (BeginPosition > 0)
                    { // есть точки не вошедшие в отображение
                        CalcPsetBorder(ref DrPoints, BeginPosition, ref DrRect);                                       
                        for (int j = (BeginPosition-1); j >= 0; j--)
                        {
                            DrPoints[j] = DrPoints[BeginPosition-1];
                        }
                    }
                    break;
                }
            }

            int EndPosition = DrPoints.Length-1;
            for (; EndPosition >= BeginPosition; EndPosition--)
            {
                if (DrPoints[EndPosition].X <= DrRect.Right)
                {
                    if (EndPosition < (DrPoints.Length - 1))
                    {
                        EndPosition++;
                        CalcPsetBorder(ref DrPoints, EndPosition, ref DrRect);

                        for (int j = EndPosition; j < DrPoints.Length; j++)
                        {
                            DrPoints[j] = DrPoints[EndPosition];
                        }
                    }
                    break;
                }
            }
             
            for (int i = 0; i < DrPoints.Length; i++)
            {
                if (DrPoints[i].Y < DrRect.Top)
                { // вышли за верхний предел                       
                    CalcPsetBorder(ref DrPoints, i, ref DrRect);
                    int OutPosition = i;
                    for (; i < DrPoints.Length-2;)
                    {
                        if (DrPoints[++i+1].Y >= DrRect.Top)
                        { // вернулись                            
                            CalcPsetBorder(ref DrPoints, ++i, ref DrRect);                            
                            break;
                        }
                        DrPoints[i] = DrPoints[OutPosition];
                    }
                }
                
                if (DrPoints[i].Y > DrRect.Bottom)
                { // вышли за нижний предел
                    CalcPsetBorder(ref DrPoints, i, ref DrRect);
                    int OutPosition = i;
                    for (; i < DrPoints.Length-2; )
                    {
                        if (DrPoints[++i+1].Y <= DrRect.Bottom)
                        { // вернулись
                            CalcPsetBorder(ref DrPoints, ++i, ref DrRect);                            
                            break;
                        }
                        DrPoints[i] = DrPoints[OutPosition];
                    }                    
                }
            }

            for (int i = 0; i < DrPoints.Length; i++)
            {
                CutPsetBorder(ref DrPoints, i, DrRect);     
            }
            
        }

        public void Redraw(ref double[,] Data, bool Present)
        {
            if (VideoDevice == null || VideoDevice.Disposed || !ThisRunning) return;           
            if (Data.Length != 0 && DrawPause) return;

            if (VidoDeviceInUse.WaitOne(1, false))
            {
                if (Data.Length != 0) DigMath.CopyArrayDouble(DrawData, Data);
               

                RecalcMinMax(0.95, 0.05);                

                try
                {
                    // очищ€ем фон                    
                    VideoDevice.BeginScene();
                    VideoDevice.Clear(ClearFlags.Target, System.Drawing.SystemColors.Control, 1.0f, 0);
                    VideoDevice.Clear(ClearFlags.Target, DrawBackgroundColor, 1.0f, 0, new Rectangle[] { DrawRectangle });
                    DrawBottomScale(false);
                    DrawLeftScale();

                    DirectXLine.Begin();


                    for (int i = 0; i < NumChannels; i++)
                    {
                        if (!ChannelEnabled[i]) continue;

                        for (int j = 0; j < NumPointsToDrawPerChannel; j++)
                        {
                            if (!SplitChannels)
                            {
                                drawPoints[j].Y = _CalcYCoord(DrawData[i, j], _Min[NumChannels], _Max[NumChannels]);                                
                            }
                            else
                            {
                                drawPoints[j].Y = _CalcYCoordFull(DrawData[i, j], _Min[i], _Max[i], ChannelDrawRectangle[i]);                                
                            }

                            drawPoints[j].X = drawPointsXCoord[j];   
                        }

                        if (!SplitChannels)
                        {
                            CalcRightBorderPoints(ref drawPoints, ref DrawRectangle);
                        }
                        else
                        {
                            CalcRightBorderPoints(ref drawPoints, ref ChannelDrawRectangle[i]);
                        }
                        DirectXLine.Draw(drawPoints, DrawChannelColors[i]);                        
                    }
                  
                    DirectXLine.End();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (ShowFPS)
                    {
                        CounterFPS++;
                        FPSTimeSpan = DateTime.Now - LastTimeFPS;
                        if (FPSTimeSpan.TotalSeconds > 1)
                        {
                            FPSCounter = CounterFPS * 1000 / FPSTimeSpan.TotalMilliseconds;
                            LastTimeFPS = DateTime.Now;
                            CounterFPS = 0;
                        }
                        DirectDrawFont.DrawText(null, FPSCounter.ToString("f1"), new Point(2, DrawRectangle.Bottom + 2), DrawScaleColor);
                    }

                    if (VideoDevice != null)
                    {
                        VideoDevice.EndScene();
                        if (Present) VideoDevice.Present();                        
                    }
                    UnlockVideoDevice();
                }
                Thread.Sleep(0);
            }
        }
    }
}
