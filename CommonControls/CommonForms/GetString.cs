using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class GetString : Form
    {
        public enum ViewType
        {
            TypeCombobox =0,
            TypeTextBox=1
        }

        private Control CurrentControl=null;

        public string ReturnValue
        {
            get
            {
                return CurrentControl.Text;                
            }
        }

        public GetString(ViewType ThisViewType, string Header, string [] ItemRange)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            this.Text = Header;
            switch (ThisViewType)
            {
                case ViewType.TypeCombobox:
                    CurrentControl = comboBoxText;
                    if (ItemRange!=null) comboBoxText.Items.AddRange(ItemRange);
                    if (comboBoxText.Items.Count > 0) comboBoxText.SelectedIndex = 0;
                    break;
                case ViewType.TypeTextBox:
                    CurrentControl=textBoxText;
                    if (ItemRange != null && ItemRange.Length > 0) CurrentControl.Text = ItemRange[0];
                    break;
            }

            CurrentControl.Visible = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void GetString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((Keys)(e.KeyChar)) == Keys.Escape) this.Close();
        }

        private void textBoxText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void comboBoxText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }       
    }
}