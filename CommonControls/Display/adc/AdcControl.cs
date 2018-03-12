using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonControls
{
	/// <summary>
	/// Summary description for AdcControl.
	/// </summary>
	/// 	
	public partial class AdcControl : UserControl
	{
       	System.Drawing.Graphics graphicsmain;				
		System.Drawing.Pen BorderPen;					
		System.Drawing.Point []BorderPoints;
		
		float	maxvaluevolt=2.5f,
				minvaluevolt=-2.5f,							
				middle_value=0;
		int 			
			minvalue=-32768,
			maxvalue=32767,
			offsetMaxValuePset=0,
			OffsetMinValuePset=0,
			values_array_index=0;

		double angle=0;	

		int[] values_array = new int[1]; 
		

		private System.Windows.Forms.TextBox textBoxMinValueVolts;
		private System.Windows.Forms.TextBox textBoxMaxValueVolts;
		private System.Windows.Forms.Label labelMinValuePset;
		private System.Windows.Forms.Label labelMaxValuePset;
		private System.Windows.Forms.Label labelCurrentValuePset;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelADCname;		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxCurrentValueVolts;
		private PanelADC panel2;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
				
		
		protected override void OnSizeChanged(EventArgs e)
		{
            if (this.Width != 0 && this.Height != 0)
            {
                initGraph();               
            }
		}


		[Description("Тип АЦП")]
		public string ADCType
		{
			set
			{				
				labelADCname.Text=value;
			}
			get
			{
                return labelADCname.Text;
			}
		}

		[Description("Тип единиц измерения")	]
		public string ValueMeasure
		{
			set
			{
				label2.Text=value;				
			}
			get
			{
                return label2.Text;
			}
		}


		[Description("Текущее значение")	]
		public int ADCValue
		{
			set
			{							
				Redraw(value);
			}
			get
			{
				return int.Parse(labelCurrentValuePset.Text);
			}
		}

		[Category("Limits"),
		Description("Максимальный предел в точках")	]
		public int MaxValuePset
		{
			set
			{
				maxvalue=value;									
				labelMaxValuePset.Text=value.ToString();
			}
			get
			{
				return int.Parse(labelMaxValuePset.Text);
			}
		}

		[Category("Limits"),
		Description("Минимальный предел в точках")	]
		public int MinValuePset
		{
			set
			{
				minvalue=value;				
				labelMinValuePset.Text=value.ToString();
			}
			get
			{
				return int.Parse(labelMinValuePset.Text);
			}
		}


		[Category("Limits"),
		Description("Максимальный предел в измеряемых единицах")	]
		public float MaxValueVolt
		{
			set
			{
				maxvaluevolt=value;
				textBoxMaxValueVolts.Text=value.ToString("f8");
			}
			get
			{
				return maxvaluevolt;
			}
		}

		[Category("Limits"),
		Description("Минимальный предел в измеряемых единицах")	]
		public float MinValueVolt
		{
			set
			{
				minvaluevolt=value;
				textBoxMinValueVolts.Text=value.ToString("f8");
			}
			get
			{
				return minvaluevolt;
			}
		}

		

		// сдвиги - позволяющие учесть, что шкала АЦП - разная по длине в положительную и отрицательную стороны
		[Category("Offsets"),
		Description("Сдвиг максимального значения АЦП - учитывает неравномерность в положительную сторону, необходим для точного отображения - в измеряемых единицах")	]
		public int OffsetMaxPset
		{
			set
			{
				offsetMaxValuePset=value;
			}
			get
			{
				return offsetMaxValuePset;
			}
		}

		[Category("Offsets"),
		Description("Сдвиг минимального значения АЦП- учитывает неравномерность в отрицательную сторону, необходим для точного отображения - в измеряемых единицах")]
		public int OffsetMinPset
		{
			set
			{
				OffsetMinValuePset=value;
			}
			get
			{
				return OffsetMinValuePset;
			}
		}

		float GetValueFromPsetUnsigned(int currentPset)
        {
            // преобразование в знаковое
            currentPset = GetSignedPsetFromUnsigned(currentPset);
            float result = ((currentPset - (minvalue + OffsetMinValuePset)) * (maxvaluevolt - minvaluevolt) / ((maxvalue + offsetMaxValuePset) - (minvalue + OffsetMinValuePset))) + minvaluevolt;
            return result;
		}


		float GetValueFromPsetSigned(int currentPset)
		{							
			float result=((currentPset-(minvalue+OffsetMinValuePset))*(maxvaluevolt-minvaluevolt)/((maxvalue+offsetMaxValuePset)-(minvalue+OffsetMinValuePset)))+minvaluevolt;			
            return result;
		}


        int GetSignedPsetFromUnsigned(int UnsignedValue)
        {
            return (int)(short)UnsignedValue;
        }
		
		public AdcControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();            
			
			initGraph();			
			
			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}		

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdcControl));
            this.textBoxMinValueVolts = new System.Windows.Forms.TextBox();
            this.textBoxMaxValueVolts = new System.Windows.Forms.TextBox();
            this.labelMinValuePset = new System.Windows.Forms.Label();
            this.labelMaxValuePset = new System.Windows.Forms.Label();
            this.labelCurrentValuePset = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelADCname = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCurrentValueVolts = new System.Windows.Forms.TextBox();
            this.panel2 = new CommonControls.PanelADC();
            this.SuspendLayout();
            // 
            // textBoxMinValueVolts
            // 
            this.textBoxMinValueVolts.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMinValueVolts.Location = new System.Drawing.Point(8, 16);
            this.textBoxMinValueVolts.Name = "textBoxMinValueVolts";
            this.textBoxMinValueVolts.Size = new System.Drawing.Size(40, 22);
            this.textBoxMinValueVolts.TabIndex = 4;
            this.textBoxMinValueVolts.Text = "-2,5";
            this.textBoxMinValueVolts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxValueVolts_KeyPress);
            // 
            // textBoxMaxValueVolts
            // 
            this.textBoxMaxValueVolts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMaxValueVolts.Location = new System.Drawing.Point(173, 16);
            this.textBoxMaxValueVolts.Name = "textBoxMaxValueVolts";
            this.textBoxMaxValueVolts.Size = new System.Drawing.Size(40, 22);
            this.textBoxMaxValueVolts.TabIndex = 5;
            this.textBoxMaxValueVolts.Text = "2,5";
            this.textBoxMaxValueVolts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxValueVolts_KeyPress);
            // 
            // labelMinValuePset
            // 
            this.labelMinValuePset.BackColor = System.Drawing.SystemColors.Control;
            this.labelMinValuePset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelMinValuePset.Location = new System.Drawing.Point(8, 0);
            this.labelMinValuePset.Name = "labelMinValuePset";
            this.labelMinValuePset.Size = new System.Drawing.Size(72, 16);
            this.labelMinValuePset.TabIndex = 7;
            this.labelMinValuePset.Text = "-32768";
            this.labelMinValuePset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMaxValuePset
            // 
            this.labelMaxValuePset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxValuePset.BackColor = System.Drawing.SystemColors.Control;
            this.labelMaxValuePset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelMaxValuePset.Location = new System.Drawing.Point(149, 0);
            this.labelMaxValuePset.Name = "labelMaxValuePset";
            this.labelMaxValuePset.Size = new System.Drawing.Size(64, 16);
            this.labelMaxValuePset.TabIndex = 8;
            this.labelMaxValuePset.Text = "32767";
            this.labelMaxValuePset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCurrentValuePset
            // 
            this.labelCurrentValuePset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentValuePset.BackColor = System.Drawing.SystemColors.Control;
            this.labelCurrentValuePset.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCurrentValuePset.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCurrentValuePset.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelCurrentValuePset.Location = new System.Drawing.Point(32, 170);
            this.labelCurrentValuePset.Name = "labelCurrentValuePset";
            this.labelCurrentValuePset.Size = new System.Drawing.Size(101, 16);
            this.labelCurrentValuePset.TabIndex = 9;
            this.labelCurrentValuePset.Text = "-32767";
            this.labelCurrentValuePset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Gainsboro;
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Location = new System.Drawing.Point(133, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "В";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelADCname
            // 
            this.labelADCname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelADCname.BackColor = System.Drawing.Color.Gainsboro;
            this.labelADCname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelADCname.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelADCname.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelADCname.Location = new System.Drawing.Point(149, 170);
            this.labelADCname.Name = "labelADCname";
            this.labelADCname.Size = new System.Drawing.Size(72, 32);
            this.labelADCname.TabIndex = 13;
            this.labelADCname.Text = "Первый";
            this.labelADCname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label1.Location = new System.Drawing.Point(133, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "т.";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(80, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "0 т.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxCurrentValueVolts
            // 
            this.textBoxCurrentValueVolts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurrentValueVolts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCurrentValueVolts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxCurrentValueVolts.Location = new System.Drawing.Point(0, 186);
            this.textBoxCurrentValueVolts.Name = "textBoxCurrentValueVolts";
            this.textBoxCurrentValueVolts.ReadOnly = true;
            this.textBoxCurrentValueVolts.Size = new System.Drawing.Size(133, 24);
            this.textBoxCurrentValueVolts.TabIndex = 14;
            this.textBoxCurrentValueVolts.Text = "-2,00000000";
            this.textBoxCurrentValueVolts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Location = new System.Drawing.Point(1, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(219, 140);
            this.panel2.TabIndex = 17;
            // 
            // AdcControl
            // 
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.labelMaxValuePset);
            this.Controls.Add(this.labelMinValuePset);
            this.Controls.Add(this.textBoxMaxValueVolts);
            this.Controls.Add(this.textBoxMinValueVolts);
            this.Controls.Add(this.labelADCname);
            this.Controls.Add(this.textBoxCurrentValueVolts);
            this.Controls.Add(this.labelCurrentValuePset);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Name = "AdcControl";
            this.Size = new System.Drawing.Size(221, 210);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		
		
		
		private void textBoxMaxValueVolts_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
            if (e.KeyChar == 13)
            {
                maxvaluevolt = float.Parse(textBoxMaxValueVolts.Text);
                minvaluevolt = float.Parse(textBoxMinValueVolts.Text);

                if (minvaluevolt > maxvaluevolt)
                {
                    minvaluevolt = maxvaluevolt;
                    textBoxMinValueVolts.Text = minvaluevolt.ToString("f8");
                };

                float result = GetValueFromPsetSigned(int.Parse(labelCurrentValuePset.Text));
                textBoxCurrentValueVolts.Text = result.ToString("f8");
            };
		}

        object SyncObject = new object();
		private void Redraw(int newvalue)
		{
            if (System.Threading.Monitor.TryEnter(SyncObject, 25))
            {
                // преобразование в знаковое - зависит от типа АЦП
                newvalue = GetSignedPsetFromUnsigned(newvalue);

                // подсчет среднего арифметического

                //общая сумма значений
                middle_value -= values_array[values_array_index];

                values_array[values_array_index] = newvalue;

                // общая сумма значений
                middle_value += newvalue;

                // среднее значение
                newvalue = (int)((float)middle_value / values_array.Length);

                if (values_array_index < (values_array.Length - 1)) values_array_index++;
                else values_array_index = 0;

                labelCurrentValuePset.Text = newvalue.ToString();

                float result = GetValueFromPsetSigned(newvalue);//((newvalue-(minvalue+OffsetMinValuePset))*(maxvaluevolt-minvaluevolt)/((maxvalue+offsetMaxValuePset)-(minvalue+OffsetMinValuePset)))+minvaluevolt;
                textBoxCurrentValueVolts.Text = result.ToString("f8");

                angle = ((newvalue - minvalue) * (Math.PI * 4 / 6) / (maxvalue - minvalue)) + (Math.PI * 7 / 6);

                panel2.Redraw(angle);

                System.Threading.Monitor.Exit(SyncObject);
            }
		}

		void initGraph()
		{			
			BorderPoints = new System.Drawing.Point[8];
			
			int psetBorderClear=7;
			BorderPoints[0].X=psetBorderClear;
			BorderPoints[0].Y=0;
			BorderPoints[1].X=0;
			BorderPoints[1].Y=psetBorderClear;
			BorderPoints[2].X=0;
			BorderPoints[2].Y=this.Height-psetBorderClear;
			BorderPoints[3].X=psetBorderClear;
			BorderPoints[3].Y=this.Height;
			BorderPoints[4].X=this.Width-psetBorderClear;
			BorderPoints[4].Y=this.Height;
			BorderPoints[5].X=this.Width;
			BorderPoints[5].Y=this.Height-psetBorderClear;
			BorderPoints[6].X=this.Width;
			BorderPoints[6].Y=psetBorderClear;
			BorderPoints[7].X=this.Width-psetBorderClear;
			BorderPoints[7].Y=0;

			System.Drawing.Drawing2D.GraphicsPath shape = new System.Drawing.Drawing2D.GraphicsPath();
			shape.AddPolygon(BorderPoints);
			this.Region = new System.Drawing.Region(shape);

			BackgroundImage = new Bitmap(this.Width,this.Height);
			graphicsmain=Graphics.FromImage(BackgroundImage);
			BorderPen = new Pen(System.Drawing.Brushes.Black,2);
			graphicsmain.Clear(Color.Gainsboro);
			graphicsmain.DrawPolygon(BorderPen,BorderPoints);		
		}	
	};
}
