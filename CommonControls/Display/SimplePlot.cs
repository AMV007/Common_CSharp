using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.Display
{    
    public partial class SimplePlot : Form
    {
        double [] ValuesY = new double[10];
        double [] ValuesX = new double[10];
        double [] ValuesXDC = new double[10];
        double [] ValuesYDC = new double[10];
        double  Ymin = -1,
                Ymax = 1;
        double diffY = 2;
        double diffY2 = 1;
        double diffX = 2;
        double diffX2 = 1;
        // КОЭФФициент уменьшения , чтобы график вписался
        float coeff = 0.85f;

        double  Xmin=-1,
                Xmax = -1;

        Graphics gr, // work graphics
                 sgr; // scale graphics


        public double ValuesXMin
        {
            get
            {
                return Xmin;
            }
            set
            {
                Xmin = value;
                if (Xmax<= Xmin)
                    Xmax= Xmin + 0.1;               

                CalcValuesX();
                Redraw();
            }
        }        

        public double ValuesXMax
        {
            get
            {
                return Xmax;
            }
            set
            {
                Xmax= value;
                if (Xmax<= Xmin)
                    Xmin = Xmax- 0.1;                

                CalcValuesX();
                Redraw();
            }
        }

        void CalcValuesX()
        {
            diffX = Xmax - Xmin;
            diffX2 = diffX / 2;

            for (int i = 0; i < ValuesX.Length; i++)
            {
                ValuesX[i] = (i * (Xmax - Xmin) / (ValuesX.Length-1)) + Xmin;
                ValuesXDC[i] = ValuesX[i] - (diffX2 + Xmin);
            }
        }

        void InitGraphics()
        {            
            if (DrawMutex.WaitOne())
            {
                panelPlot.BackgroundImage = new Bitmap(panelPlot.Width, panelPlot.Height);
                gr = Graphics.FromImage(panelPlot.BackgroundImage);

                panelScale.BackgroundImage = new Bitmap(panelScale.Width, panelScale.Height);
                sgr = Graphics.FromImage(panelScale.BackgroundImage);

                grf = panelPlot.CreateGraphics();

                DrawMutex.ReleaseMutex();
            }
            Redraw();
        }
        
        public SimplePlot()
        {
            InitializeComponent();
            InitGraphics();

            panelPlot.Paint += new PaintEventHandler(panelPlot_Paint);
        }

        void panelPlot_Paint(object sender, PaintEventArgs e)
        {
            
        }

        string FormatNumber(double value)
        {
            int LengthPoss=9;
            string res=value.ToString();
            if (res.Length>LengthPoss)
            {
                res = value.ToString("e3");
                if (res.Length > LengthPoss)
                {
                    res = value.ToString("e2");
                    if (res.Length > LengthPoss)
                    {
                        res = value.ToString("e1");
                    }
                }
            }            
            return res;
        }
                

        public void SetDataY(double []NewValuesY)
        {
            if (DrawMutex.WaitOne())
            {
                ValuesY = NewValuesY;
                ValuesYDC = new double[ValuesY.Length];                
                if (ValuesX.Length != ValuesY.Length)
                {
                    ValuesX = new double[ValuesY.Length];
                    ValuesXDC = new double[ValuesY.Length];
                    CalcValuesX();                    
                }

                Ymin = double.MaxValue;
                Ymax = double.MinValue;
                for (int i = 0; i < NewValuesY.Length; i++)
                {
                    Ymin = Math.Min(Ymin, NewValuesY[i]);
                    Ymax = Math.Max(Ymax, NewValuesY[i]);
                }
                if (Ymax <= Ymin) Ymax = Ymin + 0.1;

                diffY = (Ymax - Ymin);                
                diffY2 = diffY / 2;

                for (int i = 0; i < NewValuesY.Length; i++)
                {
                    ValuesYDC[i] = ValuesY[i]-(diffY2 + Ymin);
                }

                DrawMutex.ReleaseMutex();
            }
            Redraw();
        }                

        float TranslateYCoordToPanelGr(double Value)
        {
            int height = panelPlot.Height;
            return (float)(-((Value * height * coeff) / diffY) + (height / 2.0f));
        }

        double TranslatePanelGrToYCoord(float Value)
        {
            int height = panelPlot.Height;
            double result=-diffY*(Value-(height / 2.0f))/(height * coeff);
            result += (diffY2 + Ymin);
            return result;
        }

        float TranslateXCoordToPanelGr(double Value)
        {
            int width = panelPlot.Width;            
            return (float)(((Value * width) / diffX) + (width / 2.0f));
        }

        double TranslatePanelGrToXCoord(float Value)
        {
            int width = panelPlot.Width;
            double result=diffX*(Value-(width / 2.0f))/(double)width;
            result += ((diffX/2) + Xmin);
            return result;
        }


        Font ScaleFont = new Font("Arial", 8);
        void DrawScaleLineY(Graphics grScale,
            Graphics gr,string Value, double yvalue, float LineWidth)
        {
            int height = panelPlot.Height;
            int width = panelPlot.Width;
             
            Pen CurrPen = new Pen(Color.Black, LineWidth);
            CurrPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            float yreal = TranslateYCoordToPanelGr(yvalue - (diffY2 + Ymin));
            gr.DrawLine(CurrPen, 0, yreal, width , yreal);

            SizeF tSize = grScale.MeasureString(Value, ScaleFont);
            tSize.Width = panelScale.Width - width - tSize.Width - 2;
            tSize.Height = yreal - tSize.Height / 2;
            grScale.DrawString(Value, ScaleFont, Brushes.Black, tSize.ToPointF());
        }

        void DrawScaleLineX(Graphics grScale,
            Graphics gr, string Value, double xvalue, float LineWidth)
        {
            int height = panelPlot.Height;
            int width = panelPlot.Width;
            
            Pen CurrPen = new Pen(Color.Black, LineWidth);
            CurrPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            float xreal = TranslateXCoordToPanelGr(xvalue-(diffX2 + Xmin));
            gr.DrawLine(CurrPen, xreal, 0, xreal, height);

            SizeF tSize = grScale.MeasureString(Value, ScaleFont);
            tSize.Height = height + 2;
            tSize.Width = xreal- tSize.Width/2;
            grScale.DrawString(Value, ScaleFont, Brushes.Black, tSize.ToPointF());
        }

        void DrawScale(Graphics gr)
        {            
            sgr.Clear(Color.WhiteSmoke);
            int ScaleWidth = panelScale.Width;
            int ScaleHeight = panelScale.Height;
            int height = panelPlot.Height;
            int width = panelPlot.Width;
           
            // min max
            DrawScaleLineY(sgr, gr, FormatNumber(Ymin), Ymin, 1);
            DrawScaleLineY(sgr, gr, FormatNumber(Ymax), Ymax, 1);
            // zero
            DrawScaleLineY(sgr, gr, "0", 0, 2);
            DrawScaleLineX(sgr, gr, "0", 0, 2);

            sgr.DrawString(FormatNumber(Xmin), ScaleFont, Brushes.Black, new PointF(ScaleWidth - width, height + 3));

            float SizeWidth = sgr.MeasureString(FormatNumber(ValuesXMax), ScaleFont).Width;
            sgr.DrawString(FormatNumber(ValuesXMax), ScaleFont, Brushes.Black, new PointF(ScaleWidth - SizeWidth - 3, height + 3));
        }

        System.Threading.Mutex DrawMutex = new System.Threading.Mutex();
        void Redraw()
        {
            if (DrawMutex.WaitOne())
            {                
                gr.Clear(Color.WhiteSmoke);
                int width = panelPlot.Width;
                int height = panelPlot.Height;                

                DrawScale(gr);
               
                PointF [] points = new PointF[ValuesY.Length];
                
                for (int i = 0; i < ValuesY.Length; i++)
                {
                    points[i].X = TranslateXCoordToPanelGr(ValuesXDC[i]);
                    points[i].Y = TranslateYCoordToPanelGr(ValuesYDC[i]);
                }

                gr.DrawLines(Pens.Red, points);                

                DrawMutex.ReleaseMutex();
            }
        }        

        int DrawValueX = -100;
        int DrawValueY = -100;
        PointF LastCirclePoint = new Point();
        Graphics grf =null;       
        bool DrawCurrentValues(MouseEventArgs CurMouseArgs)
        {
            if (CurMouseArgs.Button != MouseButtons.Left || !MouseDowned) return false;

            if (Cursor.Current != null)
            {
                Cursor.Current.Dispose();
                Cursor.Current = null;
            }

            if (DrawValueX == CurMouseArgs.X&&DrawValueY==CurMouseArgs.Y) return true;

            float Circlerad = 2.5f;

            int ValueYNumber = (int)((float)CurMouseArgs.X * (float)ValuesY.Length / panelPlot.Width);
            ValueYNumber = c_CommonFunc.MinMax(ValueYNumber, 0, ValuesY.Length - 1);  

            PointF NewCirclePoint = new PointF(TranslateXCoordToPanelGr(ValuesXDC[ValueYNumber]) - Circlerad,
                    TranslateYCoordToPanelGr(ValuesYDC[ValueYNumber])- Circlerad);            

            if (DrawValueX != -100)
            {
                Rectangle trec;
                if (DrawValueY != CurMouseArgs.Y)
                {
                    trec = new Rectangle(0, (int)(DrawValueY), panelPlot.Width, (int)(1));
                    panelPlot.Invalidate(trec, false);
                }

                if (DrawValueX != CurMouseArgs.X)
                {
                    trec = new Rectangle((int)(DrawValueX), 0, (int)(1), panelPlot.Height);
                    panelPlot.Invalidate(trec, false);
                }

                if (LastCirclePoint != NewCirclePoint)
                {
                    trec = new Rectangle((int)(LastCirclePoint.X), (int)(LastCirclePoint.Y),
                        (int)(Circlerad * 2)+1, (int)(Circlerad * 2)+1);
                    panelPlot.Invalidate(trec, false);
                }
            }            
            
            toolTip1.Show("x = " + TranslatePanelGrToXCoord(CurMouseArgs.X).ToString() + "\n" +
                "y = " + TranslatePanelGrToYCoord(CurMouseArgs.Y).ToString()+"\n"+
                "val= " + ValuesY[ValueYNumber].ToString(),
                    panelPlot, panelPlot.Width / 2, panelPlot.Height-35);

            if (DrawMutex.WaitOne())
            {                
                grf.DrawLine(Pens.Blue, CurMouseArgs.X, 0, CurMouseArgs.X, panelPlot.Height);
                grf.DrawLine(Pens.Blue, 0, CurMouseArgs.Y, panelPlot.Width, CurMouseArgs.Y);
             
                grf.FillEllipse(Brushes.Blue, NewCirclePoint.X, NewCirclePoint.Y,
                    Circlerad * 2, Circlerad*2);

                DrawMutex.ReleaseMutex();
            }

            DrawValueX = CurMouseArgs.X;
            DrawValueY = CurMouseArgs.Y;
            LastCirclePoint = NewCirclePoint;

            return true;
        }

        bool MouseEntered = false;
        private void panelPlot_MouseEnter(object sender, EventArgs e)
        {
            MouseEntered = true;
            Cursor.Current = Cursors.Cross;
        }

        private void panelPlot_MouseLeave(object sender, EventArgs e)
        {
            MouseEntered = false;
            Cursor.Current = Cursors.Default;
        }

        bool MouseDowned = false;
        private void panelPlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseEntered)
            {
                MouseDowned = true;

                if (e.Button == MouseButtons.Left)
                {
                    DrawCurrentValues(e);
                }
            }                
        }

        private void panelPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > -10 && e.Y > -10 && e.X <= panelPlot.Width+10 && e.Y <= panelPlot.Height+10)
            {
                Cursor.Current = Cursors.Cross;
                if (e.Button == MouseButtons.Left)
                {
                    DrawCurrentValues(e);
                }
                else
                {
                   if (Cursor.Current == null)
                        Cursor.Current = Cursors.Cross;
                }
            }
                        
        }

        private void panelPlot_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDowned = false;
            toolTip1.SetToolTip(panelPlot, "");
            DrawValueX = -100;

            if (MouseEntered)
            {
                if (Cursor.Current == null)
                    Cursor.Current = Cursors.Cross;
                panelPlot.Invalidate();           
            }
        }

        private void panelScale_SizeChanged(object sender, EventArgs e)
        {
            InitGraphics();
        }

        private void ImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DrawMutex.WaitOne())
            {
                System.IO.Stream myStream;

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = Application.StartupPath;
                if (System.IO.Directory.Exists(Application.StartupPath + "\\data"))
                {
                    saveFileDialog1.InitialDirectory += "\\data";
                }
                saveFileDialog1.Filter = "JPEG (*.jpg)|*.jpg";
                saveFileDialog1.Filter += "|BMP (*.bmp)|*.bmp";
                saveFileDialog1.Filter += "|GIF (*.gif)|*.gif";
                saveFileDialog1.FilterIndex = 1;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        try
                        {
                            if (myStream.Length == 0)
                            {
                                System.Drawing.Font fn = new Font(FontFamily.GenericMonospace, 8);

                                sgr.DrawString(this.Parent.Text, fn, Brushes.Red, 0, panelScale.Height - 15);

                                sgr.DrawImage(panelPlot.BackgroundImage, panelPlot.Location.X - panelScale.Location.X,
                                    panelPlot.Location.Y - panelScale.Location.Y,
                                    panelPlot.Width, panelPlot.Height);

                                switch (saveFileDialog1.FilterIndex)
                                {
                                    case 1: //"JPEG":								
                                        panelScale.BackgroundImage.Save(myStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                    case 2: //"BMP":
                                        panelScale.BackgroundImage.Save(myStream, System.Drawing.Imaging.ImageFormat.Bmp);
                                        break;
                                    case 3: //"GIF":
                                        panelScale.BackgroundImage.Save(myStream, System.Drawing.Imaging.ImageFormat.Gif);
                                        break;
                                };                                
                            };
                        }
                        catch (Exception ex)
                        {
                            c_ErrorDataWork.Instance.ErrorProcess(ex);
                        }
                        myStream.Close();
                    };
                };

                DrawMutex.ReleaseMutex();
            }
            Redraw();
        }

        string FormatVal(double Val)
        {
            if (Math.Abs(Val) > 1e5 || Math.Abs(Val) < 1e-5)
            {
                return Val.ToString("e5");
            }
            else
            {
                return Val.ToString("f5");
            }
        }

        void SaveData(string FileName)
        {
            System.IO.StreamWriter DataWriter = null;
            try
            {
                DataWriter = new System.IO.StreamWriter(FileName);
                DataWriter.WriteLine("Y Values                  X Values");
                for (int i = 0; i < ValuesY.Length; i++)
                {
                    DataWriter.WriteLine(FormatVal(ValuesY[i]) +
                        "     " + FormatVal(ValuesX[i]));
                }
            }
            catch (Exception ex)
            {
                c_ErrorDataWork.Instance.ErrorProcess(ex);
            }
            finally
            {
                if (DataWriter != null)
                {
                    DataWriter.Close();
                }
            }
        }
        private void DataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DrawMutex.WaitOne())
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = Application.StartupPath;
                if (System.IO.Directory.Exists(Application.StartupPath + "\\data"))
                {
                    saveFileDialog1.InitialDirectory += "\\data";
                }
                saveFileDialog1.Filter = "JPEG (*.jpg)|*.jpg";
                saveFileDialog1.Filter += "|BMP (*.bmp)|*.bmp";
                saveFileDialog1.Filter += "|GIF (*.gif)|*.gif";
                saveFileDialog1.FilterIndex = 1;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SaveData(saveFileDialog1.FileName);                    
                }

                DrawMutex.ReleaseMutex();
            }
            Redraw();
        }

        private void DataToReporttxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveData(c_CommonFunc.ApplicationPath+"\\report.txt");
        }
       
    }
}
