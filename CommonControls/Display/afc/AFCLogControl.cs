using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
//using System.Text;
using System.Windows.Forms;

namespace CommonControls
{
    public partial class AFCLogControl : UserControl
    {
        Pen[] DrawPens = new Pen[8] 
            { 
            Pens.Red,
            Pens.Yellow,
            Pens.Green,
            Pens.Blue,
            Pens.Brown,
            Pens.Coral,
            Pens.Gold,
            Pens.Gray            
            };
        public double[] FrequencyArray;
                
        public struct ChannelValuesArray
        {
            public double[] Values;
        }
        public ChannelValuesArray[] ValuesArray;
        Graphics ThisGraphics;

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Redraw();
        }

        protected override void OnResize(EventArgs e)
        {
            //base.OnResize(e);
            ThisGraphics = this.CreateGraphics();
            Redraw();
        }

        public AFCLogControl()
        {
            InitializeComponent();
            InitializeControl();
        }


        void InitializeControl()
        {
            ThisGraphics = this.CreateGraphics();
        }

        [Category("Config"),
        Description("Количество отображаемых каналов")]
        int NumChannels = 1;
        public int NumDrawChannels
        {
            set
            {
                NumChannels = value;
            }
            get
            {
                return NumChannels;
            }
        }

        void ArrayMaxMin(ref ChannelValuesArray[] Values, ref double Max, ref double Min)
		{           
			Max=-1000000;
			Min=1000000;
			for (int i=0;i<Values.Length;i++)
			{
                for (int j = 0; j < Values[i].Values.Length; j++)
                {
                    Max = Math.Max(Values[i].Values[j], Max);
                    Min = Math.Min(Values[i].Values[j], Min);
                }
			}
		}


        double ScaleMaximum = 10;
        double ScaleMinimum = -10;

        int PositionGraphicsX = 30;
        int PositionGraphicsY = 0;
        int GraphicsWidth = 0;
        int GraphicsHeigth = 0;

        void DrawScale()
        {
            int ThisWidth = this.Width-1;

            PositionGraphicsX = 30;
            PositionGraphicsY = 0;
            GraphicsWidth = ThisWidth - PositionGraphicsX;
            GraphicsHeigth = this.Height - 30;           

			ThisGraphics.Clear(SystemColors.Control);

			System.Drawing.Font ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.139131F, System.Drawing.FontStyle.Regular, 
				System.Drawing.GraphicsUnit.Point, 204);

            for (int i = 0; i < FrequencyArray.Length; i++)
			{
				ThisGraphics.DrawLine(Pens.Black,
                    PositionGraphicsX + (GraphicsWidth * i / (FrequencyArray.Length - 1)), 0,
                    PositionGraphicsX + (GraphicsWidth * i / (FrequencyArray.Length - 1)), GraphicsHeigth + 5);

                float BeginPointX = 30.0f + (GraphicsWidth * i / (FrequencyArray.Length - 1)) - (ScaleFont.Size * (FrequencyArray[i].ToString("f0").Length / 2.0f));
                if ((BeginPointX + (GraphicsWidth / (FrequencyArray.Length - 1))) > ThisWidth)
                    BeginPointX = ThisWidth - (ScaleFont.Size * (FrequencyArray[i].ToString("f0").Length));
                ThisGraphics.DrawString(FrequencyArray[i].ToString("f0"),
					ScaleFont,
					Brushes.Black,
					new RectangleF
					(
                    BeginPointX,
					(float)GraphicsHeigth+6,
                    (float)(GraphicsWidth / (FrequencyArray.Length - 1)),
					30.0f
					)				
				);					
			}
			
			ArrayMaxMin(ref ValuesArray,ref ScaleMaximum, ref ScaleMinimum);

			int NumPointsValues=GraphicsHeigth/15;
			for (int i=0;i<NumPointsValues;i++)
			{				
				ThisGraphics.DrawLine(Pens.Black,
					PositionGraphicsX-5,
                    PositionGraphicsY+(GraphicsHeigth*i/(NumPointsValues-1)),
					ThisWidth,
                    PositionGraphicsY+(GraphicsHeigth*i/(NumPointsValues-1)));
                
                ThisGraphics.DrawString((((ScaleMaximum - ScaleMinimum) * (NumPointsValues-i-1) / (NumPointsValues - 1)) + ScaleMinimum).ToString("f0"),
					ScaleFont,
					Brushes.Black,
					new RectangleF(0.0f,
                    Math.Max((GraphicsHeigth * i / (NumPointsValues - 1)) - (ScaleFont.Height/2.0f),0),
					(float)PositionGraphicsX,
					(float)(GraphicsHeigth/(NumPointsValues-1))
					)
				);
					
			}

        }

        public void Redraw()
        {
            DrawScale();
            Point[] DrawPoints = new Point[FrequencyArray.Length];

            for (int NumCh = 0; NumCh < ValuesArray.Length; NumCh++)
            {
                for (int NumVal = 0; NumVal < ValuesArray[NumCh].Values.Length; NumVal++)
                {
                    double tempvalue=ValuesArray[NumCh].Values[NumVal]-ScaleMinimum;
                    tempvalue = tempvalue * GraphicsHeigth / (ScaleMaximum - ScaleMinimum);                    
                    DrawPoints[NumVal].Y = (int)tempvalue+PositionGraphicsY;
                    DrawPoints[NumVal].X = (NumVal*GraphicsWidth / (ValuesArray[NumCh].Values.Length-1))+PositionGraphicsX;                    
                }                
                ThisGraphics.DrawLines(DrawPens[NumCh], DrawPoints);              
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AFCLogControl
            // 
            this.Name = "AFCLogControl";
            this.Size = new System.Drawing.Size(235, 189);
            this.ResumeLayout(false);

        }

    }
}