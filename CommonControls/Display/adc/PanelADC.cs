using System;
using System.Drawing;

namespace CommonControls
{
	/// <summary>
	/// Summary description for PanelADC.
	/// </summary>
	class PanelADC: System.Windows.Forms.Panel
	{			
		Graphics bufferGraphics;
		Bitmap bufferScaleBitmap;
		Graphics bufferScaleGraphics;

		Pen ScalePen;		
		Point BeginPoint;

		Point []PolygonPints;
		Point []Razmetka;					
		Rectangle ScaleRectanglelow;	
		Rectangle ScaleRectanglemiddle;

		int length,
			MaxLengthScale;
		double angle;
				
		protected override void OnSizeChanged(EventArgs e)
		{
            if (this.Width!=0&&this.Height!=0)init();
		}
		
		void DrawScale()
		{
			//
			// рисуем шкалу
			//

			// общий фон			
			bufferScaleGraphics.Clear(System.Drawing.Color.Gainsboro);	

			// рисуем фон разметки
			int ScaleLength=MaxLengthScale;
			ScaleRectanglelow=new System.Drawing.Rectangle(BeginPoint.X-(ScaleLength),BeginPoint.Y-(ScaleLength),2*(ScaleLength),2*(ScaleLength));						
			bufferScaleGraphics.FillPie(System.Drawing.Brushes.LightGreen,ScaleRectanglelow,200,140);
			
			
			ScaleLength=length-15;
			ScaleRectanglemiddle=new System.Drawing.Rectangle(BeginPoint.X-(ScaleLength),BeginPoint.Y-(ScaleLength),2*(ScaleLength),2*(ScaleLength));
			bufferScaleGraphics.FillPie(System.Drawing.Brushes.Blue,ScaleRectanglemiddle,200,140);			

			ScaleLength=25;
			ScaleRectanglemiddle=new System.Drawing.Rectangle(BeginPoint.X-(ScaleLength),BeginPoint.Y-(ScaleLength),2*(ScaleLength),2*(ScaleLength));
			//bufferScaleGraphics.FillPie(System.Drawing.Brushes.WhiteSmoke,ScaleRectanglemiddle,0,360);			
			
			// рисуем разметку 			
			bufferScaleGraphics.DrawPie(System.Drawing.Pens.Black,ScaleRectanglelow,240,60);			
			bufferScaleGraphics.DrawPie(System.Drawing.Pens.Black,ScaleRectanglelow,210,120); //			
			bufferScaleGraphics.DrawPie(System.Drawing.Pens.Black,ScaleRectanglelow,200,140);						

			// центральная линия
			ScalePen.Width=3;
			bufferScaleGraphics.DrawLine(ScalePen,BeginPoint.X,BeginPoint.Y,BeginPoint.X,BeginPoint.Y-MaxLengthScale);
			ScalePen.Width=2;

			// Плюс и минус
			int smeshenieY=10,
				smeshenieX=40,
				dlina=10;
			bufferScaleGraphics.DrawLine(ScalePen,smeshenieX,BeginPoint.Y-smeshenieY,smeshenieX+dlina,BeginPoint.Y-smeshenieY);
			bufferScaleGraphics.DrawLine(ScalePen,this.Width-smeshenieX,BeginPoint.Y-smeshenieY,this.Width-smeshenieX-dlina,BeginPoint.Y-smeshenieY);
			bufferScaleGraphics.DrawLine(ScalePen,this.Width-smeshenieX-(dlina/2),BeginPoint.Y-smeshenieY+(dlina/2),this.Width-smeshenieX-(dlina/2),BeginPoint.Y-smeshenieY-(dlina/2));

			
			// линии по краям
			Razmetka= new System.Drawing.Point[3]{BeginPoint,BeginPoint,BeginPoint};
			// левая 
			Razmetka[0].X=((int)(Math.Cos(Math.PI*11/6)*(MaxLengthScale)))+BeginPoint.X;
			Razmetka[0].Y=((int)(Math.Sin(Math.PI*11/6)*(MaxLengthScale)))+BeginPoint.Y;						
			
			// правая
			Razmetka[2].X=((int)(Math.Cos(Math.PI*7/6)*(MaxLengthScale)))+BeginPoint.X;
			Razmetka[2].Y=((int)(Math.Sin(Math.PI*7/6)*(MaxLengthScale)))+BeginPoint.Y;	

			ScalePen.Color=System.Drawing.Color.Red;
			bufferScaleGraphics.DrawLines(ScalePen,Razmetka);
			ScalePen.Color=System.Drawing.Color.Black;
					
			bufferScaleGraphics.FillEllipse(System.Drawing.Brushes.Gainsboro,ScaleRectanglemiddle);			
			bufferScaleGraphics.DrawArc(System.Drawing.Pens.Black,ScaleRectanglemiddle,200,140);			
		}
		
		public void Redraw(double newangle)
		{            
			angle=newangle;

            // Вычисляем новое значение стрелки и рисуем ее
            // конечная точка
            PolygonPints[3].X = ((int)(Math.Cos(angle) * length)) + BeginPoint.X;
            PolygonPints[3].Y = ((int)(Math.Sin(angle) * length)) + BeginPoint.Y;

            // точки в начале левая
            PolygonPints[0].X = ((int)(Math.Cos(angle + Math.PI / 2) * 8)) + BeginPoint.X;
            PolygonPints[0].Y = ((int)(Math.Sin(angle + Math.PI / 2) * 8)) + BeginPoint.Y;

            // точка с другой стороны
            PolygonPints[1].X = ((int)(Math.Cos(angle + Math.PI) * 5)) + BeginPoint.X;
            PolygonPints[1].Y = ((int)(Math.Sin(angle + Math.PI) * 5)) + BeginPoint.Y;

            // точка в начале правая
            PolygonPints[2].X = ((int)(Math.Cos(angle - Math.PI / 2) * 8)) + BeginPoint.X;
            PolygonPints[2].Y = ((int)(Math.Sin(angle - Math.PI / 2) * 8)) + BeginPoint.Y;           
            
            bufferGraphics.DrawImage(bufferScaleBitmap,0,0);			
			bufferGraphics.FillPolygon(System.Drawing.Brushes.Yellow,PolygonPints);
			bufferGraphics.DrawPolygon(System.Drawing.Pens.Black,PolygonPints);
					
			Invalidate();
		}

		
		void init()
		{
            DoubleBuffered = true;
            MaxLengthScale = (int)Math.Min(this.Height - 5, (this.Width/2)/Math.Cos(Math.PI/6));
			length=MaxLengthScale-2;
			BeginPoint= new Point(this.Width/2,this.Height-5);
						
			BackgroundImage = new Bitmap(this.Width, this.Height);			
			bufferGraphics = Graphics.FromImage(BackgroundImage);
			
			bufferScaleBitmap = new Bitmap(this.Width, this.Height);
			bufferScaleGraphics = Graphics.FromImage(bufferScaleBitmap);

			angle=Math.PI*3/2;
			DrawScale();
			Redraw(angle);
		}

		public PanelADC()
		{
			//
			// TODO: Add constructor logic here
			//						
			PolygonPints = new System.Drawing.Point[4];	
			ScalePen= new Pen(System.Drawing.Brushes.Black,2);			
			init();		
		}
	}
}
