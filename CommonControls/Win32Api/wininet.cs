using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Win32Api
{
    public static class wininet
    {
        public enum INTERNET_FLAGS
        {
            AUTODIAL_FORCE_ONLINE = 1,
            AUTODIAL_FORCE_UNATTENDED = 2,
            AUTODIAL_FAILIFSECURITYCHECK = 4,
            AUTODIAL_OVERRIDE_NET_PRESENT = 8,

            DIAL_FORCE_PROMPT     =0x2000,
            DIAL_SHOW_OFFLINE     =0x4000,
            DIAL_UNATTENDED       =0x8000,
        }

        public enum Connection_State
        {
            INTERNET_CONNECTION_ERROR = 0x00,            //Local system uses a proxy server to connect to the Internet.        

            INTERNET_CONNECTION_CONFIGURED = 0x40,            //Local system has a valid connection to the Internet, but it might or might not be currently connected.
            INTERNET_CONNECTION_LAN = 0x02,            //Local system uses a local area network to connect to the Internet.
            INTERNET_CONNECTION_MODEM = 0x01,            //Local system uses a modem to connect to the Internet.
            INTERNET_CONNECTION_MODEM_BUSY = 0x08,            //No longer used.
            INTERNET_CONNECTION_OFFLINE = 0x20,            //Local system is in offline mode.
            INTERNET_CONNECTION_PROXY = 0x04,            //Local system uses a proxy server to connect to the Internet.        
        }
        

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern int InternetAttemptConnect(uint res);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern int  InternetAutodialHangup(uint res);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetConnectedState(out Connection_State flags, long reserved);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern int InternetDial(int parent, string connectionid, int flags, int connection, int reserved);
    }
}
