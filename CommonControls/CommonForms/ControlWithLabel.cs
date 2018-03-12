using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class ControlWithLabel : UserControl
    {
        e_SupportedControls variableControlType = e_SupportedControls.Unknown;
        Control variableControl=null;
        public enum e_SupportedControls
        {
            Unknown,
            TextBox,
            ComboBox,
            NumericUpDown
        }

        [Category("Vcntrl param"),
        Description("Label Text")]
        public string LabelText
        {
            set
            {                
                labelMain.Text = value;
                CalcVarControlPos();
            }
            get
            {
                return labelMain.Text;
            }
        }

        [Category("Vcntrl val")]        
        public string CText
        {
            set
            {
                SetControlProperty("Text",value);                
            }
            get
            {
                return (string)GetControlProperty("Text","");            
            }
        }

        [Category("Vcntrl val")]
        public decimal CValue
        {
            set
            {
                SetControlProperty("Value", value);
            }
            get
            {
                return (decimal)GetControlProperty("Value", (decimal)0);                       
            }
        }

        [Category("Vcntrl val")]
        public decimal CMaximum
        {
            set
            {
                SetControlProperty("Maximum", value);
            }
            get
            {
                return (decimal)GetControlProperty("Maximum", (decimal)0);
            }
        }

        [Category("Vcntrl val")]
        public decimal CMinimum
        {
            set
            {
                SetControlProperty("Minimum", value);
            }
            get
            {
                return (decimal)GetControlProperty("Minimum", (decimal)0);
            }
        }

        [Category("Vcntrl val")]
        public ComboBox.ObjectCollection CComboItems
        {
            set
            {
                SetControlProperty("Items", value);
            }
            get
            {
                return (ComboBox.ObjectCollection)GetControlProperty("Items", null);
            }
        }

        [Category("Vcntrl val")]
        public string CComboCurrentItem
        {
            get
            {
                if (variableControlType == e_SupportedControls.ComboBox)
                {
                    ComboBox xx = (ComboBox)variableControl;
                    if (xx.Items.Count > 0 && xx.SelectedIndex>=0)
                    {
                        return xx.Items[xx.SelectedIndex].ToString();
                    }
                }
                return "";
            }
            set
            {
                if (variableControlType == e_SupportedControls.ComboBox)
                {
                    ComboBox xx = (ComboBox)variableControl;
                    if (xx.Items.Count > 0 && value!="")
                    {
                        xx.SelectedItem = value;
                    }
                }
                
            }
        }        

        [Category("Vcntrl param"),
        Description("Variable Control Type")]        
        // not var, because, it's must be initialized first, and
        // initialization processed by alphabet
        public e_SupportedControls AVarControlType
        {
            set
            {
                if (variableControl != null)
                {
                    this.Controls.Remove(variableControl);
                    variableControl.Dispose();
                    variableControl = null;
                }
                variableControlType = value;
                switch (variableControlType)
                {                    
                    case e_SupportedControls.ComboBox:
                        variableControl = new ComboBox();
                        break;                    
                    case e_SupportedControls.NumericUpDown:
                        variableControl = new NumericUpDown();
                        ((NumericUpDown)variableControl).Maximum = 100;
                        ((NumericUpDown)variableControl).Minimum = 0;
                        break;                   
                    case e_SupportedControls.TextBox:
                        variableControl = new TextBox();
                        break;
                    default: return;
                }

                variableControl.Anchor = (System.Windows.Forms.AnchorStyles)(
                                        System.Windows.Forms.AnchorStyles.Top |
                                        System.Windows.Forms.AnchorStyles.Bottom
                            //| System.Windows.Forms.AnchorStyles.Left)
                            //| System.Windows.Forms.AnchorStyles.Right)
                            );
                variableControl.Location = new System.Drawing.Point(3, 3);
                variableControl.Name = "VariableControl";
                variableControl.Size = new System.Drawing.Size(114, 22);
                variableControl.TabIndex = 1;                
                CalcVarControlPos();

                variableControl.TextChanged += new EventHandler(variableControl_TextChanged);
                this.Controls.Add(variableControl);                 
            }
            get
            {
                return variableControlType;
            }
        }

        void variableControl_TextChanged(object sender, EventArgs e)
        {
            if (OnTextChanged != null)
            {
                OnTextChanged.Invoke(sender, e);
            }
        }

        [Category("Vcntrl events"),
        Description("Variable Control Text Changed event")]        
        public new event EventHandler OnTextChanged = null;       


        void SetControlProperty(string PropName, object Val)
        {
            if (variableControl != null)
            {
                System.Reflection.PropertyInfo Prop = variableControl.GetType().GetProperty(PropName);
                if (Prop != null)
                {
                    Prop.SetValue(variableControl, Val, null);
                }
            }
        }

        object GetControlProperty(string PropName, object defVal)
        {
            if (variableControl != null)
            {
                System.Reflection.PropertyInfo Prop = variableControl.GetType().GetProperty(PropName);
                if (Prop != null)
                {
                    return Prop.GetValue(variableControl, null);
                }
            }
            return defVal;         
        }

        public ControlWithLabel()
        {
            InitializeComponent();            
            AVarControlType = e_SupportedControls.TextBox;
        }

        void CalcVarControlPos()
        {
            if (variableControl != null)
            {
                variableControl.Location = new Point(labelMain.Bounds.Right, variableControl.Location.Y);
                variableControl.Width = this.Width - variableControl.Location.X-2;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CalcVarControlPos();
        }       
    }
}
