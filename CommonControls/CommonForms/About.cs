using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class About : Form
    {
        public About(string Description, string PS
            )
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;


            textBoxBuiltVersion.Text = "Version : " + Application.ProductVersion;
            textBoxBuiltVersion.Text+= " , Build : "+System.Reflection.Assembly.GetCallingAssembly().GetName().Version;
            labelDesc.Text = Description;
            labelPS.Text = PS;
        }

        private void linkLabelmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string Link = ((LinkLabel)sender).Text;
            System.Diagnostics.Process.Start("mailto:" + Link);
        }

        private void linkLabelsait_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string Link = ((LinkLabel)sender).Text;
            System.Diagnostics.Process.Start("http:\\" + Link);
        }

        private void textBoxBitcoin_MouseClick(object sender, MouseEventArgs e)
        {
            TextBox t = (TextBox)sender;
            Clipboard.SetText(t.Text);
            MessageBox.Show("Адрес скопирован в буффер обмена");
        }
    }
}
