using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using CommonControls.Win32Api;
using System.Runtime.InteropServices;

namespace CommonControls.Devices
{
    public class DeviceManagement
    {

        // For detecting devices and receiving device notifications.

        // Used in error messages:
        const string ModuleName = "DeviceManagement";

        // For viewing results of API calls in debug.write statements:
        Debugging MyDebugging = new Debugging();

        public static string[] GetHidDevicesPaths(string Contains)
        {
            string[] DevicesPaths;
            Guid HidDevices;
            CommonControls.Win32Api.hid.HidD_GetHidGuid(out HidDevices);

            if (CommonControls.Devices.DeviceManagement.FindDeviceFromGuid(HidDevices, out DevicesPaths))
            {
                System.Collections.ArrayList NewCollection = new System.Collections.ArrayList(DevicesPaths);
                for (int i = 0; i < NewCollection.Count && (Contains != null && Contains!=""); i++)
                {
                    if (!NewCollection[i].ToString().Contains(Contains))
                    {
                        NewCollection.RemoveAt(i);
                        i--;
                    }
                }

                return (string[])NewCollection.ToArray(typeof(string));
            }

            return null;
        }

        public static bool DeviceNameMatch(Message m, string mydevicePathName)
        {

            // Purpose    : Compares two device path names. Used to find out if the device name
            //            : of a recently attached or removed device matches the name of a
            //            : device the application is communicating with.

            // Accepts    : m - a WM_DEVICECHANGE message. A call to RegisterDeviceNotification
            //            : causes WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine.
            //            : mydevicePathName - a device pathname returned by SetupDiGetDeviceInterfaceDetail
            //            : in an SP_DEVICE_INTERFACE_DETAIL_DATA structure.

            // Returns    : True if the names match, False if not.

            try
            {
                user32.DeviceBroadcastInterface DevBroadcastDeviceInterface = new user32.DeviceBroadcastInterface();
                setupapi.DEV_BROADCAST_HDR DevBroadcastHeader = new setupapi.DEV_BROADCAST_HDR();

                // The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.
                Marshal.PtrToStructure(m.LParam, DevBroadcastHeader);

                if (DevBroadcastHeader.dbch_devicetype == setupapi.DBT_DEVTYP_DEVICEINTERFACE)
                {

                    // The dbch_devicetype parameter indicates that the event applies to a device interface.
                    // So the structure in LParam is actually a DEV_BROADCAST_INTERFACE structure,
                    // which begins with a DEV_BROADCAST_HDR.

                    // Obtain the number of characters in dbch_name by subtracting the 28 bytes
                    // in the other members of the structure and dividing by 2 because there are
                    // 2 bytes per character.
                    int StringSize = System.Convert.ToInt32((DevBroadcastHeader.dbch_size - 28) / 2);

                    // The dbcc_name parameter of DevBroadcastDeviceInterface contains the device name.
                    // Trim dbcc_name to match the size of the string.
                    DevBroadcastDeviceInterface.Name = (new char[StringSize + 1]).ToString();

                    // Marshal data from the unmanaged block pointed to by m.LParam
                    // to the managed object DevBroadcastDeviceInterface.
                    Marshal.PtrToStructure(m.LParam, DevBroadcastDeviceInterface);

                    // Store the device name in a String.
                    string DeviceNameString = DevBroadcastDeviceInterface.Name.Substring(0, StringSize);

                    Debug.WriteLine("Device Name = " + DeviceNameString);
                    Debug.WriteLine("");

                    // Compare the name of the newly attached device with the name of the device
                    // the application is accessing (mydevicePathName).
                    // Set ignorecase True.
                    if (string.Compare(DeviceNameString, mydevicePathName, true) == 0)
                    {
                        // The name matches.
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                HandleException(ModuleName + ":" + System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            // It's a different device.
            return false;
        }


        public static bool FindDeviceFromGuid(System.Guid myGuid, out string[] devicePathName)
        {

            // Purpose    : Uses SetupDi API functions to retrieve the device path name of an
            //            : attached device that belongs to an interface class.
            // Accepts    : myGuid - an interface class GUID.
            //            : devicePathName - a pointer to an array of strings that will contain
            //            : the device path names of attached devices.
            // Returns    : True if at least one device is found, False if not.
            
            System.Collections.ArrayList DevicePaths = new System.Collections.ArrayList();
            bool DeviceFound = false;
            try
            {
                IntPtr DeviceInfoSet =
                   setupapi.SetupDiGetClassDevs(
                       ref myGuid,
                       null,
                       IntPtr.Zero,
                       (int)(setupapi.DIGF.PRESENT | setupapi.DIGF.DEVICEINTERFACE));

                Debug.WriteLine(Debugging.ResultOfAPICall("SetupDiClassDevs"));
                
                // The cbSize element of the MyDeviceInterfaceData structure must be set to
                // the structure's size in bytes. The size is 28 bytes.                
                setupapi.SP_DEVICE_INTERFACE_DATA MyDeviceInterfaceData = new setupapi.SP_DEVICE_INTERFACE_DATA();
                MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData);                

                while(setupapi.SetupDiEnumDeviceInterfaces(
                                 DeviceInfoSet,
                                 0,
                                 ref myGuid,
                                 (uint)DevicePaths.Count,
                                 ref MyDeviceInterfaceData))
                {
                    Debug.WriteLine(Debugging.ResultOfAPICall("SetupDiEnumDeviceInterfaces"));
                    uint BufferSize = 0;         
                    
                    bool Success = setupapi.SetupDiGetDeviceInterfaceDetail(
                                    DeviceInfoSet,
                                    ref MyDeviceInterfaceData,
                                    IntPtr.Zero, 0, ref BufferSize, IntPtr.Zero);                                       
                    
                    // Store the structure's size.                                            
                    byte[] TempPath = new byte[BufferSize+4+2];
                    BitConverter.GetBytes((int)5).CopyTo(TempPath,0);
                    // Call SetupDiGetDeviceInterfaceDetail again.
                    // This time, pass a pointer to DetailDataBuffer
                   
                    Success = setupapi.SetupDiGetDeviceInterfaceDetail(
                                            DeviceInfoSet,
                                            ref MyDeviceInterfaceData,
                                            TempPath,
                                            BufferSize,
                                            ref BufferSize,
                                            IntPtr.Zero);
                    Debug.WriteLine(Debugging.ResultOfAPICall("SetupDiGetDeviceInterfaceDetail"));                   

                    char[] TempChar = new char[BufferSize];
                    Array.Copy(TempPath, 3, TempChar, 0, BufferSize);
                    string SingledevicePathName = new string(TempChar);
                    DevicePaths.Add(SingledevicePathName);
                    DeviceFound = true;                   
                }              

                setupapi.SetupDiDestroyDeviceInfoList(DeviceInfoSet);
                Debug.WriteLine(Debugging.ResultOfAPICall("SetupDiDestroyDeviceInfoList"));

            }
            catch (Exception ex)
            {
                HandleException(ModuleName + ":" + System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            devicePathName =(string[]) DevicePaths.ToArray(typeof(string));

            return DeviceFound;
        }


        public static bool RegisterForDeviceNotifications(string devicePathName, IntPtr formHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
        {

            // Purpose    : Request to receive a notification when a device is attached or removed.

            // Accepts    : devicePathName - a handle to a device.
            //            : formHandle - a handle to the window that will receive device events.
            //            : classGuid - an interface class GUID.
            // 

            // Returns    : True on success, False on failure.

            // A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.
            user32.DeviceBroadcastInterface DevBroadcastDeviceInterface = new user32.DeviceBroadcastInterface();
            
            int size;

            try
            {
                // Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.

                // Set the size.
                size = Marshal.SizeOf(DevBroadcastDeviceInterface);
                DevBroadcastDeviceInterface.Size = size;

                // Request to receive notifications about a class of devices.
                DevBroadcastDeviceInterface.DeviceType = setupapi.DBT_DEVTYP_DEVICEINTERFACE;

                DevBroadcastDeviceInterface.Reserved = 0;

                // Specify the interface class to receive notifications about.
                DevBroadcastDeviceInterface.ClassGuid = classGuid;          
                               

                // ***
                // API function:
                // RegisterDeviceNotification
                // Purpose:
                // Request to receive notification messages when a device in an interface class
                // is attached or removed.
                // Accepts:
                // Aa handle to the window that will receive device events
                // A pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of
                // device to send notifications for,
                // DEVICE_NOTIFY_WINDOW_HANDLE to indicate that Handle is a window handle.
                // Returns:
                // A device notification handle or NULL on failure.
                // ***

                deviceNotificationHandle = user32.RegisterDeviceNotification(formHandle, DevBroadcastDeviceInterface, setupapi.DEVICE_NOTIFY_WINDOW_HANDLE);
                

                // Find out if RegisterDeviceNotification was successful.
                if (deviceNotificationHandle.ToInt32() == IntPtr.Zero.ToInt32())
                {
                    Debug.WriteLine("RegisterDeviceNotification error");
                    return false;
                }

            }
            catch (Exception ex)
            {
                HandleException(ModuleName + ":" + System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            return true;
        }


        public static void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
        {

            // Purpose    : Requests to stop receiving notification messages when a device in an
            //              interface class is attached or removed.
            // Accepts    : deviceNotificationHandle - a handle returned previously by
            //              RegisterDeviceNotification

            try
            {

                // ***
                // API function: UnregisterDeviceNotification
                // Purpose: Stop receiving notification messages.
                // Accepts: a handle returned previously by RegisterDeviceNotification
                // Returns: True on success, False on failure.
                // ***

                // Ignore failures.
                user32.UnregisterDeviceNotification(deviceNotificationHandle);

            }
            catch (Exception ex)
            {
                HandleException(ModuleName + ":" + System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

        }

        static public void HandleException(string moduleName, Exception e)
        {

            // Purpose    : Provides a central mechanism for exception handling.
            //            : Displays a message box that describes the exception.

            // Accepts    : moduleName - the module where the exception occurred.
            //            : e - the exception

            string Message;
            string Caption;

            try
            {
                // Create an error message.
                Message = "Exception: " + e.Message + Environment.NewLine + "Module: " + moduleName + Environment.NewLine + "Method: " + e.TargetSite.Name;

                // Specify a caption.
                Caption = "Unexpected Exception";

                // Display the message in a message box.
                MessageBox.Show(Message, Caption, MessageBoxButtons.OK);
                Debug.Write(Message);
            }
            finally { }

        }

    }
}
