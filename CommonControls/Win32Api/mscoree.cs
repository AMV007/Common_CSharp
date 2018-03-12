using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Win32Api
{
    public static class mscoree
    {
        [DllImport("mscoree.dll")]
        public static extern int GetCORSystemDirectory(
            [MarshalAs(UnmanagedType.LPWStr)]StringBuilder pbuffer,
            int cchBuffer, ref int dwlength);       
    }

   
}
