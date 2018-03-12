using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Win32Api
{
    public static class hid
    {
        #region Constants
        // from hidpi.h
        // Typedef enum defines a set of integer constants for HidP_Report_Type
        public const short HidP_Input = 0;
        public const short HidP_Output = 1;
        public const short HidP_Feature = 2;
        #endregion

        #region Structures
        /// <summary>
        /// Provides details about a single USB device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }
        /// <summary>
        /// Provides the capabilities of a HID device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HidCaps
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;
        }       

        // ******************************************************************************
        // Structures and classes for API calls, listed alphabetically
        // ******************************************************************************

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public short VendorID;
            public short ProductID;
            public short VersionNumber;
        }       

        // If IsRange is false, UsageMin is the Usage and UsageMax is unused.
        // If IsStringRange is false, StringMin is the string index and StringMax is unused.
        // If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.

        [StructLayout(LayoutKind.Sequential)]
        public struct HidP_Value_Caps
        {
            public short UsagePage;
            public byte ReportID;
            public int IsAlias;
            public short BitField;
            public short LinkCollection;
            public short LinkUsage;
            public short LinkUsagePage;
            public int IsRange;
            public int IsStringRange;
            public int IsDesignatorRange;
            public int IsAbsolute;
            public int HasNull;
            public byte Reserved;
            public short BitSize;
            public short ReportCount;
            public short Reserved2;
            public short Reserved3;
            public short Reserved4;
            public short Reserved5;
            public short Reserved6;
            public int LogicalMin;
            public int LogicalMax;
            public int PhysicalMin;
            public int PhysicalMax;
            public short UsageMin;
            public short UsageMax;
            public short StringMin;
            public short StringMax;
            public short DesignatorMin;
            public short DesignatorMax;
            public short DataIndexMin;
            public short DataIndexMax;
        }

        
        #endregion
        

        #region P/Invoke
        [DllImport("hid.dll", SetLastError = true)]
        static public extern bool HidD_FlushQueue(IntPtr HidDeviceObject);
        /// <summary>
        /// Gets the GUID that Windows uses to represent HID class devices
        /// </summary>
        /// <param name="gHid">An out parameter to take the Guid</param>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid gHid);
        
        /// <summary>
        /// Gets details from an open device. Reserves a block of memory which must be freed.
        /// </summary>
        /// <param name="hFile">Device file handle</param>
        /// <param name="lpData">Reference to the preparsed data block</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetPreparsedData( IntPtr hFile, out IntPtr lpData);
        /// <summary>
        /// Frees the memory block reserved above.
        /// </summary>
        /// <param name="pData">Reference to preparsed data returned in call to GetPreparsedData</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_FreePreparsedData(ref IntPtr pData);
        /// <summary>
        /// Gets a device's capabilities from the preparsed data.
        /// </summary>
        /// <param name="lpData">Preparsed data reference</param>
        /// <param name="oCaps">HidCaps structure to receive the capabilities</param>
        /// <returns>True if successful</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetCaps( IntPtr lpData, out HidCaps oCaps);
        

        [DllImport("hid.dll")]
        static public extern int HidD_GetAttributes( IntPtr HidDeviceObject, out HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll")]
        static public extern bool HidD_GetFeature( IntPtr HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll")]
        static public extern bool HidD_GetInputReport( IntPtr HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);
               

        [DllImport("hid.dll")]
        static public extern bool HidD_GetNumInputBuffers( IntPtr HidDeviceObject, ref int NumberBuffers);        

        [DllImport("hid.dll")]
        static public extern bool HidD_SetFeature( IntPtr HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll")]
        static public extern bool HidD_SetNumInputBuffers( IntPtr HidDeviceObject, int NumberBuffers);

        [DllImport("hid.dll")]
        static public extern bool HidD_SetOutputReport( IntPtr HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);        

        [DllImport("hid.dll")]
        static public extern int HidP_GetValueCaps(short ReportType, ref byte ValueCaps, ref short ValueCapsLength,  IntPtr PreparsedData);


        #endregion

        #region Public methods
        /// <summary>
        /// Registers a window to receive windows messages when a device is inserted/removed. Need to call this
        /// from a form when its handle has been created, not in the form constructor. Use form's OnHandleCreated override.
        /// </summary>
        /// <param name="hWnd">Handle to window that will receive messages</param>
        /// <param name="gClass">Class of devices to get messages for</param>
        /// <returns>A handle used when unregistering</returns>
        public static IntPtr RegisterForUsbEvents( IntPtr hWnd, Guid gClass)
        {
            user32.DeviceBroadcastInterface oInterfaceIn = new user32.DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
            oInterfaceIn.ClassGuid = gClass;
            oInterfaceIn.DeviceType = kernel32.DEVTYP_DEVICEINTERFACE;
            oInterfaceIn.Reserved = 0;
            return user32.RegisterDeviceNotification(hWnd, oInterfaceIn, kernel32.DEVICE_NOTIFY_WINDOW_HANDLE);
        }
        /// <summary>
        /// Unregisters notifications. Can be used in form dispose
        /// </summary>
        /// <param name="hHandle">Handle returned from RegisterForUSBEvents</param>
        /// <returns>True if successful</returns>
        public static bool UnregisterForUsbEvents( IntPtr hHandle)
        {
            return user32.UnregisterDeviceNotification(hHandle);
        }
        /// <summary>
        /// Helper to get the HID guid.
        /// </summary>
        public static Guid HIDGuid
        {
            get
            {
                Guid gHid ;
                HidD_GetHidGuid(out gHid);
                return gHid;                
            }
        }
        #endregion
    }
}
