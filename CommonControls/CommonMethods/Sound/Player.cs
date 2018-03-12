using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CommonControls.Win32Api;
namespace CommonControls.CommonMethods.Sound
{
    public class Player
    {
        private string Pcommand;
        private bool isOpen;


        public Player()
        {            
        }

        ~Player()
        {
            Close();
        }

        public void Close()
        {
            Pcommand = "close MediaFile";
            Win32Api.winmm.mciSendString(Pcommand, null, 0, IntPtr.Zero);
            isOpen = false;
        }


        public bool Open(string strFilePath)
        {
            Close();
            if (string.IsNullOrEmpty(strFilePath)) return false;
            if (!System.IO.File.Exists(strFilePath)) return false;            

            string sCommand = "open \"" + strFilePath + "\" type mpegvideo alias MediaFile";            
            int res = Win32Api.winmm.mciSendString(sCommand, null, 0, IntPtr.Zero);
            if (res == 0)
            {
                isOpen = true;
                return true;
            }
            return false;
        }


        public bool Play(bool loop=false)
        {
            if (isOpen)
            {
                Pcommand = "play MediaFile";                                                     
                if (loop)
                    Pcommand += " REPEAT";                
                int res = Win32Api.winmm.mciSendString(Pcommand, null, 0, IntPtr.Zero);                                      
                if (res == 0) return true;
            }
            return false;
        }

        public bool Play(string FileName)
        {
            if (Open(FileName))
            {
                return Play(false);                       
            }
            return false;
        }

    }
}
