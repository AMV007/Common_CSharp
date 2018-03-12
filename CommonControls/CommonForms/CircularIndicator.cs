using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonControls
{
    public partial class CircularIndicator : UserControl
    {
        Color ThisColor;
        Graphics ThisGraphics;     
        
        [Category("Colors"),
        Description("Цвет цвета")]
        public Color Value
        {
            set
            {
                ThisColor = value;                
                Redraw();
            }
            get
            {
                return ThisColor;
            }           
        }

        [Category("Labels"),
        Description("Описание")]
        public String IndicatorLabel
        {
            set
            {
                labelIndicator.Text = value;
            }
            get
            {
                return labelIndicator.Text;
            }
        }

        void InitGraphics()
        {
            ThisGraphics = panelIndicator.CreateGraphics();
        }
        /*      
        
        void RedrawBackground(int index)
        {
            Backgorund[index] = new Bitmap(panelIndicator.Width, panelIndicator.Height);
            Graphics ThisGraphics = Graphics.FromImage(Backgorund[index]);
            Pen DrawPen = new Pen(Brushes.Olive, 2);
            ThisGraphics.DrawEllipse(DrawPen, 0, 0, panelIndicator.Width-1, panelIndicator.Height-1);

            Rectangle ThisRectangle = new Rectangle(2, 2, panelIndicator.Width - 5, panelIndicator.Height - 5);
            SolidBrush NewBrush = new SolidBrush(ThisColors[index]);
            ThisGraphics.FillEllipse(NewBrush, ThisRectangle);
        }
         
        void InitImages()
        {
            for (int i=0;i<ThisColors.Length;i++)
            {
                if (ThisColors[i] == ThisColor)
                {
                    ThisColorIndex = i;
                    Redraw();
                    return;
                }
            }

            Color[] NewColors = new Color[ThisColors.Length+1];
            ThisColors.CopyTo(NewColors,0);
            NewColors[NewColors.Length-1] = ThisColor;
            ThisColors = NewColors;
               
            Bitmap [] NewImages = new Bitmap[Backgorund.Length+1];
            Backgorund.CopyTo(NewImages,0);
            NewImages[NewImages.Length - 1] = new Bitmap(1,1);
            Backgorund= NewImages;
            RedrawBackground(Backgorund.Length - 1);  

            ThisColorIndex = Backgorund.Length - 1;           
            Redraw();
        }         
       */
        public CircularIndicator()
        {
            InitializeComponent();           
            ThisColor = Color.Red;            
            Redraw();
        }

        void Redraw()
        {            
            Pen DrawPen = new Pen(Brushes.Olive, 2);
            ThisGraphics.DrawEllipse(DrawPen, 0, 0, panelIndicator.Width - 1, panelIndicator.Height - 1);

            Rectangle ThisRectangle = new Rectangle(2, 2, panelIndicator.Width - 5, panelIndicator.Height - 5);
            SolidBrush NewBrush = new SolidBrush(ThisColor);
            ThisGraphics.FillEllipse(NewBrush, ThisRectangle);     
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            panelIndicator.Height = this.Height;
            panelIndicator.Width = panelIndicator.Height;
            labelIndicator.Location = new Point(panelIndicator.Width + 5, 0);
            InitGraphics();
            Redraw();
        }        

        private void labelIndicator_SizeChanged(object sender, EventArgs e)
        {
            this.Width = labelIndicator.Width + panelIndicator.Width + 5;
        }

        private void panelIndicator_Paint(object sender, PaintEventArgs e)
        {            
            Redraw();
        }

       
    }
}
