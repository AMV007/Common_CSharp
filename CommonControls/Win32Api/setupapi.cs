using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Win32Api
{
    public static class setupapi
    {
        // API declarations relating to device management (SetupDixxx and
        // RegisterDeviceNotification functions).

        // ******************************************************************************
        // API constants
        // ******************************************************************************

        // from dbt.h
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DBT_DEVTYP_HANDLE = 6;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;        

        // from setupapi.h   
        public enum DIGF
        {
            DEFAULT = 0x00000001,
            PRESENT = 0x00000002,
            ALLCLASSES = 0x00000004,
            PROFILE = 0x00000008,
            DEVICEINTERFACE = 0x00000010
        }

        // ******************************************************************************
        // Structures and classes for API calls, listed alphabetically
        // ******************************************************************************

        // There are two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

        

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public int dbch_handle;
            public int dbch_hdevnotify;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public System.Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public System.Guid ClassGuid;
            public int DevInst;
            public int Reserved;
        }

        /// <summary>
        /// Access to the path for a device
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            public string DevicePath;
        }
        

        /// <summary>
        /// Allocates an InfoSet memory block within Windows that contains details of devices.
        /// </summary>
        /// <param name="gClass">Class guid (e.g. HID guid)</param>
        /// <param name="strEnumerator">Not used</param>
        /// <param name="hParent">Not used</param>
        /// <param name="nFlags">Type of device details required (DIGCF_ constants)</param>
        /// <returns>A reference to the InfoSet</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, uint nFlags);
        /// <summary>
        /// Frees InfoSet allocated in call to above.
        /// </summary>
        /// <param name="lpInfoSet">Reference to InfoSet</param>
        /// <returns>true if successful</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);
        /// <summary>
        /// Gets the DeviceInterfaceData for a device from an InfoSet.
        /// </summary>
        /// <param name="lpDeviceInfoSet">InfoSet to access</param>
        /// <param name="nDeviceInfoData">Not used</param>
        /// <param name="gClass">Device class guid</param>
        /// <param name="nIndex">Index into InfoSet for device</param>
        /// <param name="oInterfaceData">DeviceInterfaceData to fill with data</param>
        /// <returns>True if successful, false if not (e.g. when index is passed end of InfoSet)</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr lpDeviceInfoSet, uint nDeviceInfoData, ref Guid gClass, uint nIndex, ref SP_DEVICE_INTERFACE_DATA oInterfaceData);
        /// <summary>
        /// SetupDiGetDeviceInterfaceDetail
        /// Gets the interface detail from a DeviceInterfaceData. This is pretty much the device path.
        /// You call this twice, once to get the size of the struct you need to send (nDeviceInterfaceDetailDataSize=0)
        /// and once again when you've allocated the required space.
        /// </summary>
        /// <param name="lpDeviceInfoSet">InfoSet to access</param>
        /// <param name="oInterfaceData">DeviceInterfaceData to use</param>
        /// <param name="oDetailData">DeviceInterfaceDetailData to fill with data</param>
        /// <param name="nDeviceInterfaceDetailDataSize">The size of the above</param>
        /// <param name="nRequiredSize">The required size of the above when above is set as zero</param>
        /// <param name="lpDeviceInfoData">Not used</param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA oInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA oInterfaceData, IntPtr oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA oInterfaceData, byte[] oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        
        

        [DllImport("setupapi.dll")]
        static public extern int SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, int hwndParent);        
        
    }
}
