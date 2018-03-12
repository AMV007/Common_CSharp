using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonControls;

namespace TestCommonControls
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool Play(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                CommonControls.Win32Api.winmm.MCI_OPEN_PARMS MCI_Open = new CommonControls.Win32Api.winmm.MCI_OPEN_PARMS();
                MCI_Open.dwCallback = IntPtr.Zero;
                MCI_Open.wDeviceID = 0;
                MCI_Open.lpstrDeviceType = "waveaudio";
                MCI_Open.lpstrElementName = FileName;
                MCI_Open.lpstrAlias = "";

                int res=CommonControls.Win32Api.winmm.mciSendCommand(0, CommonControls.Win32Api.winmm.e_MSI_CONMMAND.MCI_OPEN,
                    0x2 | 0x00001000 | 0x00000200, //MCI_WAIT | MCI_OPEN_TYPE | MCI_OPEN_ELEMENT,
                    ref MCI_Open);                
                
                CommonControls.Win32Api.winmm.MCI_PLAY_PARMS MCI_Play = new CommonControls.Win32Api.winmm.MCI_PLAY_PARMS();
                MCI_Play.dwCallback = IntPtr.Zero;
                MCI_Play.dwFrom = 0;
                MCI_Play.dwTo = 0;
                res = CommonControls.Win32Api.winmm.mciSendCommand(0, CommonControls.Win32Api.winmm.e_MSI_CONMMAND.MCI_PLAY,
                    0x00000001, //MCI_NOTIFY
                    ref MCI_Play);
                res = res;
                
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommonControls.CommonMethods.Sound.Player xx = new CommonControls.CommonMethods.Sound.Player();
            Play("G:\\музыка\\лево и право.wav");      
        }
    }
}
