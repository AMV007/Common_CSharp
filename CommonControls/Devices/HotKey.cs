using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.Devices
{
    public class HotKey : IMessageFilter
    {        
        private const int id = 100;

        private IntPtr handle;
        public IntPtr Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        public event EventHandler HotKeyPressed= null;
        public event EventHandler KeyPressed = null;

        public HotKey(Keys key, Win32Api.user32.KeyModifiers modifier)
        {            
            RegisterHotKey(key, modifier);
            Application.AddMessageFilter(this);
        }

        ~HotKey()
        {
            Application.RemoveMessageFilter(this);
            Win32Api.user32.UnregisterHotKey(handle, id);
        }


        private void RegisterHotKey(Keys key, Win32Api.user32.KeyModifiers modifier)
        {
            if (key == Keys.None)
                return;

            bool isKeyRegisterd = Win32Api.user32.RegisterHotKey(handle, id, modifier, key);
            if (!isKeyRegisterd)
                throw new ApplicationException("Hotkey allready in use");
        }



        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)Win32Api.user32.WinMsg.WM_HOTKEY:
                    if (HotKeyPressed != null)
                    {
                        HotKeyPressed(this, new EventArgs());
                    }
                    return true;
                default:
                    KeyPressed(this, new EventArgs());
                    break;
            }
            return false;
        }

    }
}
