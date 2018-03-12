using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Win32Api
{
    public static class rasapi32
    {
        #region Constansts
        
        const int MAX_PATH = 260;

        public const int RAS_MaxAreaCode = 10;
        public const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;
        public const int RAS_MaxDeviceName = 128;
        public const int RAS_MaxDeviceType = 16;
        public const int RAS_MaxDnsSuffix = 256;
        public const int RAS_MaxEntryName = 256;
        public const int RAS_MaxFacilities = 200;
        public const int RAS_MaxPadType = 32;
        public const int RAS_MaxPath = 260;
        public const int RAS_MaxPhoneNumber = 128;
        public const int RAS_MaxUserData = 200;
        public const int RAS_MaxX25Address = 200;

        public const int DNLEN = 15;
        public const int UNLEN = 256;
        public const int PWLEN = 256;

        public enum RASCONNSTATE
        {
            OpenPort = 0,
            PortOpened,
            ConnectDevice,
            DeviceConnected,
            AllDevicesConnected,
            Authenticate,
            AuthNotify,
            AuthRetry,
            AuthCallback,
            AuthChangePassword,
            AuthProject,
            AuthLinkSpeed,
            AuthAck,
            ReAuthenticate,
            Authenticated,
            PrepareForCallback,
            WaitForModemReset,
            WaitForCallback,
            Projected,
            SubEntryConnected,
            SubEntryDisconnected,

            Interactive = 0x1000,
            RetryAuthentication,
            CallbackSetByCaller,
            PasswordExpired,
            InvokeEapUI,

            Connected = 0x2000,
            Disconnected
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public class RASCONNSTATUS
        {
            public readonly int dwSize = Marshal.SizeOf(typeof(RASCONNSTATUS));
            [MarshalAs(UnmanagedType.U4)]
            public RASCONNSTATE rasconnstate;
            int dwError;            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
            public string szDeviceType;            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
            public string szDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
            public string szPhoneNumber;            
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public struct RASCONN
        {
            public int dwSize;
            public IntPtr hrasconn;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType)]
            public string szDeviceType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName)]
            public string szDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPhonebook;
            public int dwSubEntry;
            public Guid guidEntry;
            public int dwFlags;
            public Guid luid;
        }        

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RASENTRYNAME
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szEntryName;
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
            public string szPhonebook;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RASEAPINFO
        {
            public uint dwSizeofEapInfo;
            IntPtr pbEapInfo;
        }
        

        [StructLayout(LayoutKind.Sequential)]
        public class RASDIALEXTENSIONS
        {
            public readonly int dwSize = Marshal.SizeOf(typeof(RASDIALEXTENSIONS));
            public uint dwfOptions = 0;
            public int hwndParent = 0;
            public int reserved = 0;
            public int reserved1 = 0;
            public RASEAPINFO RasEapInfo = new RASEAPINFO();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class RASDIALPARAMS
        {
            public readonly int dwSize = Marshal.SizeOf(typeof(RASDIALPARAMS));
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)RAS_MaxEntryName + 1)]
            public string szEntryName = null;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)RAS_MaxPhoneNumber + 1)]
            public string szPhoneNumber = null;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)RAS_MaxCallbackNumber + 1)]
            public string szCallbackNumber = null;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)UNLEN + 1)]
            public string szUserName = null;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)PWLEN + 1)]
            public string szPassword = null;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst =
                (int)DNLEN + 1)]
            public string szDomain = null;
            public int dwSubEntry = 0;
            public int dwCallbackId = 0;
        }

        #endregion

        [DllImport("rasapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RasEnumConnections(
            [In, Out] RASCONN[] rasconn,
            [In, Out] ref int cb,
            [Out] out int connections);

        public static RASCONN[] GetActiveConnections()
        {
            RASCONN[] connections = new RASCONN[1];
            connections[0].dwSize = Marshal.SizeOf(typeof(RASCONN));
            //Get entries count
            int connectionsCount = 0;
            int cb = Marshal.SizeOf(typeof(RASCONN));
            int nRet = RasEnumConnections(connections, ref cb, out connectionsCount);
            if (nRet != (int)winerror.ERROR_SUCCESS && nRet != (int)winerror.ERROR_BUFFER_TOO_SMALL)
                throw new Exception("RasEnumConnections"+nRet.ToString());
            if (connectionsCount == 0)
                return new RASCONN[0];

            // create array with specified entries number
            connections = new RASCONN[connectionsCount];
            for (int i = 0; i < connections.Length; i++)
            {
                connections[i].dwSize = Marshal.SizeOf(typeof(RASCONN));
            }
            nRet = RasEnumConnections(connections, ref cb, out connectionsCount);
            if (nRet != (int)winerror.ERROR_SUCCESS)
                throw new Exception("RasEnumConnections"+nRet.ToString());

            return connections;
            
        }
        public static string[] GetActiveConnectionsNames()
        {
            RASCONN[] connections=GetActiveConnections();

            string[] result = new string [connections.Length];

            for (int i = 0; i < connections.Length; i++)
            {
                result[i] = connections[i].szEntryName;
            }

            return result;
        }

        [DllImport("rasapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RasEnumEntries(
            IntPtr reserved,
            IntPtr lpszPhonebook,
            [In, Out] RASENTRYNAME[] lprasentryname,
            ref int lpcb,
            ref int lpcEntries);


        public static RASENTRYNAME[] GetConnections()
        {
            int cb = Marshal.SizeOf(typeof(RASENTRYNAME)), 
                entries = 0;
            RASENTRYNAME[] entryNames = new RASENTRYNAME[1];
            entryNames[0].dwSize = Marshal.SizeOf(typeof(RASENTRYNAME));
            //Get entry number
            uint nRet = RasEnumEntries(IntPtr.Zero, IntPtr.Zero, entryNames, ref cb, ref entries);
            if (entries == 0) return new RASENTRYNAME[0];
            string[] _EntryNames = new string[entries];
            entryNames = new RASENTRYNAME[entries];
            for (int i = 0; i < entries; i++)
            {
                entryNames[i].dwSize = Marshal.SizeOf(typeof(RASENTRYNAME));
            }

            nRet = RasEnumEntries(IntPtr.Zero, IntPtr.Zero, entryNames, ref cb, ref entries);
            if (nRet != (int)winerror.ERROR_SUCCESS)
                throw new Exception("RasEnumEntries" + nRet.ToString());

            return entryNames;     
        }

        public static string[] GetConnectionsNames()
        {
            RASENTRYNAME[] entryNames = GetConnections();
            string [] result = new string[entryNames.Length];
            for (int i = 0; i < entryNames.Length; i++)
            {
                result[i] = entryNames[i].szEntryName;            
            }
            return result;
        }

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RasDial(
            [In]RASDIALEXTENSIONS lpRasDialExtensions,
            [In]string lpszPhonebook,
            [In]RASDIALPARAMS lpRasDialParams,
            uint dwNotifierType,
            Delegate lpvNotifier,
            ref IntPtr lphRasConn);

        [DllImport("rasapi32.dll", SetLastError = true)]
        public static extern uint RasHangUp(IntPtr hRasConn);

        [DllImport("rasapi32.dll")]
        public static extern int RasGetConnectStatus(int hrasconn, [In,Out] RASCONNSTATUS lprasconnstatus);

        [DllImport("rasapi32.dll")]
        public static extern int RasGetConnectStatus(IntPtr hrasconn, [In,Out] RASCONNSTATUS lprasconnstatus);

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public extern static uint RasGetErrorString(
        uint uErrorValue,    // error to get string for
        StringBuilder lpszErrorString,  // buffer to hold error string
        [In]int cBufSize       // size, in characters, of buffer
        );



    }
}
