using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CommonControls.Win32Api;

namespace CommonControls.Devices
{
    class c_HDD_Smart
    {

        public class C_SMART_SUBREGISTER
        {
            //
            // Feature register defines for SMART "sub commands"
            //
            public const byte READ_ATTRIBUTES = 0xD0;
            public const byte READ_THRESHOLDS = 0xD1;
            public const byte ENABLE_DISABLE_AUTOSAVE = 0xD2;
            public const byte SAVE_ATTRIBUTE_VALUES = 0xD3;
            public const byte EXECUTE_OFFLINE_DIAGS = 0xD4;
            public const byte SMART_READ_LOG = 0xD5;
            public const byte SMART_WRITE_LOG = 0xd6;
            public const byte ENABLE_SMART = 0xD8;
            public const byte DISABLE_SMART = 0xD9;
            public const byte RETURN_SMART_STATUS = 0xDA;
            public const byte ENABLE_DISABLE_AUTO_OFFLINE = 0xDB;
        }

        public class C_SMART_ATTRIBUTES
        {
            public const uint RAW_READ_ERROR_RATE = 1;
            public const uint THROUGHPUT_PERFORMANCE = 2;
            public const uint SPIN_UP_TIME = 3;
            public const uint START_STOP_COUNT = 4;
            public const uint START_REALLOCATION_SECTOR_COUNT = 5;
            public const uint SEEK_ERROR_RATE = 7;
            public const uint POWER_ON_HOURS_COUNT = 9;
            public const uint SPIN_RETRY_COUNT = 10;
            public const uint RECALIBRATION_RETRIES = 11;
            public const uint DEVICE_POWER_CYCLE_COUNT = 12;
            public const uint SOFT_READ_ERROR_RATE = 13;
            public const uint LOAD_UNLOAD_CYCLE_COUNT = 193;
            public const uint TEMPERATURE = 194;
            public const uint ECC_ON_THE_FLY_COUNT = 195;
            public const uint REALLOCATION_EVENT_COUNT = 196;
            public const uint CURRENT_PENDING_SECTOR_COUNT = 197;
            public const uint UNCORRECTABLE_SECTOR_COUNT = 198;
            public const uint ULTRA_DMA_CRC_ERROR_COUNT = 199;
            public const uint WRITE_ERROR_RATE = 200;
            public const uint TA_COUNTER_INCREASED = 202;
            public const uint GSENSE_ERROR_RATE = 221;
            public const uint POWER_OFF_RETRACT_COUNT = 228;
            public const uint MAX_ATTRIBUTES = 256;
        }

        public class C_SMART_ATTRIB_INDEX
        {
            public const int INDEX = 0;
            public const int UNKNOWN1 = 1;
            public const int UNKNOWN2 = 2;
            public const int VALUE = 3;
            public const int WORST = 4;
            public const int RAW = 5;
        }

        private static ST_SMART_DETAILS[] SMARTAttributes = new ST_SMART_DETAILS[]{
            new ST_SMART_DETAILS(1,1,"Raw Read Error Rate","This attribute value depends of read errors, disk surface condition and indicates the rate of hardware read errors that occurred when reading data from a disk surface. Lower values indicate that there is a problem with either disk surface or read/write heads"),
            new ST_SMART_DETAILS(2,0,"Throughput Performance","Overall (general) throughput performance of a hard disk drive. If the value of this attribute is deceasing there is a high probability of troubles with your disk."),
            new ST_SMART_DETAILS(3,0,"Spin Up Time","Average time of spindle spin up (from zero RPM (Revolutions Per Minute) to fully operational). Active SMART shows the raw value of this attribute in milliseconds or seconds"),
            new ST_SMART_DETAILS(4,0,"Start/Stop Count","This raw value of this attribute is a count of hard disk spindle start/stop cycles"),
            new ST_SMART_DETAILS(5,1,"Reallocated Sectors Count","Count of reallocated sectors. When the hard drive finds a read/write/verification error, it marks this sector as 'reallocated' and transfers data to a special reserved area (spare area).This process is also known as remapping and 'reallocated' sectors are called remaps. This is why, on a modern hard disks, you can not see 'bad blocks' while testing the surface - all bad blocks are hidden in reallocated sectors. However, the more sectors that are reallocated, the more a sudden decrease (up to 10% and more) can be noticed in the disk read/write speed."),
            new ST_SMART_DETAILS(6,0,"Read Channel Margin","Margin of a channel while reading data. The function of this attribute is not specified."),
            new ST_SMART_DETAILS(7,0,"Seek Error Rate","Rate of seek errors of the magnetic heads. If there is a failure in the mechanical positioning system, a servo damage or a thermal widening of the hard disk, seek errors arise. More seek errors indicates a worsening condition of a disk surface and the disk mechanical subsystem."),
            new ST_SMART_DETAILS(8,0,"Seek Time Performance","Average performance of seek operations of the magnetic heads. If this attribute is decreasing, it is a sign of problems in the hard disk drive mechanical subsystem. "),
            new ST_SMART_DETAILS(9,0,"Power-On Hours","Count of hours in power-on state. The raw value of this attribute shows total count of hours (or minutes, or seconds, depending on manufacturer) in power-on state. A decrease of this attribute value to the critical level (threshold) indicates a decrease of the MTBF (Mean Time Between Failures).However, in reality, even if the MTBF value falls to zero, it does not mean that the MTBF resource is completely exhausted and the drive will not function normally."),
            new ST_SMART_DETAILS(10,0,"Spin Retry Count","Count of retry of spin start attempts. This attribute stores a total count of the spin start attempts to reach the fully operational speed (under the condition that the first attempt was unsuccessful). A decrease of this attribute value is a sign of problems in the hard disk mechanical subsystem."),
            new ST_SMART_DETAILS(11,0,"Recalibration Retries","This attribute indicates the number of times recalibration was requested (under the condition that the first attempt was unsuccessful). A decrease of this attribute value is a sign of problems in the hard disk mechanical subsystem."),
            new ST_SMART_DETAILS(12,0,"Device Power Cycle Count","This attribute indicates the count of full hard disk power on/off cycles."),
            new ST_SMART_DETAILS(13,0,"Soft Read Error Rate","This is the rate of 'program' read errors occurring when reading data from a disk surface."),            
            new ST_SMART_DETAILS(184,1,"End-to-End error","This attribute is a part of HP's SMART IV technology and it means that after transferring through the cache RAM data buffer the parity data between the host and the hard drive did not match."),            
            new ST_SMART_DETAILS(187,0,"Reported Uncorrectable Errors","A number of errors that could not be recovered using hardware ECC (see attribute 195)."),            
            new ST_SMART_DETAILS(188,1,"Command Timeout","A number of aborted operations due to HDD timeout. Normally this attribute value should be equal to zero and if the value is far above zero, then most likely there will be some serious problems with power supply or an oxidized data cable."),            
            new ST_SMART_DETAILS(189,0,"High Fly Writes","Green Arrow Down.svg HDD producers implement a Fly Height Monitor that attempts to provide additional protections for write operations by detecting when a recording head is flying outside its normal operating range. If an unsafe fly height condition is encountered, the write process is stopped, and the information is rewritten or reallocated to a safe region of the hard drive. This attribute indicates the count of these errors detected over the lifetime of the drive. This feature is implemented in most modern Seagate drives[citation needed] and some of Western Digital’s drives, beginning with the WD Enterprise WDE18300 and WDE9180 Ultra2 SCSI hard drives, and will be included on all future WD Enterprise products"),            
            new ST_SMART_DETAILS(190,0,"Airflow Temperature (WDC)","Airflow temperature on Western Digital HDs (Same as temp. [C2], but current value is 50 less for some models. Marked as obsolete.)"),            
            new ST_SMART_DETAILS(192,0,"Power-off retract count","Number of times the heads are loaded off the media. Heads can be unloaded without actually powering off.[citation needed] (or Emergency Retract Cycle count – Fujitsu)"),
            new ST_SMART_DETAILS(193,0,"Load/Unload Cycle Count","Count of load/unload cycles into Landing Zone position. For more info see the Head Load/Unload Technology description."),
            new ST_SMART_DETAILS(194,0,"Temperature","Hard disk drive temperature. The raw value of this attribute shows built-in heat sensor registrations (in degrees centigrade)."),
            new ST_SMART_DETAILS(195,0,"Hardware ECC Recovered","Time between ECC-corrected errors[citation needed] or number of ECC on-the-fly errors. Sources differ on this point."),
            new ST_SMART_DETAILS(196,1,"Reallocation Event Count","Count of remap operations (transfering data from a bad sector to a special reserved disk area - spare area). The raw value of this attribute shows the total number of attempts to transfer data from reallocated sectors to a spare area. Unsuccessful attempts are counted as well as successful."),
            new ST_SMART_DETAILS(197,1,"Current Pending Sector Count","Current count of unstable sectors (waiting for remapping). The raw value of this attribute indicates the total number of sectors waiting for remapping. Later, when some of these sectors are read successfully, the value is decreased. If errors still occur when reading some sector, the hard drive will try to restore the data, transfer it to the reserved disk area (spare area) and mark this sector as remapped. If this attribute value remains at zero, it indicates that the quality of the corresponding surface area is low."),
            new ST_SMART_DETAILS(198,1,"Uncorrectable Sector Count","Quantity of uncorrectable errors. The raw value of this attribute indicates the total number of uncorrectable errors when reading/writing a sector. A rise in the value of this attribute indicates that there are evident defects of the disk surface and/or there are problems in the hard disk drive mechanical subsystem."),
            new ST_SMART_DETAILS(199,0,"UltraDMA CRC Error Count","This attribute value depends of read errors, disk surface condition and indicates the rate of hardware read errors that occurred when reading data from a disk surface. Lower values indicate that there is a problem with either disk surface or read/write heads"),
            new ST_SMART_DETAILS(200,0,"Write Error Rate (Multi Zone Error Rate)","Write data errors rate. This attribute indicates the total number of errors found when writing a sector. The higher the raw value, the worse the disk surface condition and/or mechanical subsystem is."),
            new ST_SMART_DETAILS(220,1,"Disk Shift","Shift of disks towards spindle. The raw value of this attribute indicates how much the disk has shifted. Unit measure is unknown. For more info see G-Force Protection technology description (click to learn more on Seagate website ). NOTE: Shift of disks is possible as a result of a strong shock or a fall, or for other reasons."),
            new ST_SMART_DETAILS(221,0,"G-Sense Error Rate","Rate of errors occurring as a result of impact loads. This attribute stores an indication of a shock-sensitive sensor, that is, the total quantity of errors occurring as a result of internal impact loads (dropping drive, wrong installation, etc.)."),            
            new ST_SMART_DETAILS(222,0,"Loaded Hours","Loading on magnetic heads actuator caused by the general operating time. Only time when the actuator was in the operating position is counted."),
            new ST_SMART_DETAILS(223,0,"Load/Unload Retry Count","Loading on magnetic heads actuator caused by numerous recurrences of operations like: reading, recording, positioning of heads, etc. Only the time when heads were in the operating position is counted."),
            new ST_SMART_DETAILS(224,0,"Load Friction","Loading on magnetic heads actuator caused by friction in mechanical parts of the store. Only the time when heads were in the operating position is counted."),
            new ST_SMART_DETAILS(226,0,"Load-in Time","Total time of loading on the magnetic heads actuator. This attribute indicates total time in which the drive was under load (on the assumption that the magnetic heads were in operating mode and out of the parking area)."),
            new ST_SMART_DETAILS(227,0,"Torque Amplification Count","Count of efforts of the rotating moment of a drive."),
            new ST_SMART_DETAILS(228,0,"Power-Off Retract Count","This attribute shows a count of the number of times the drive was powered down."),
            new ST_SMART_DETAILS(230,0,"GMR Head Amplitude","Amplitude of heads trembling (GMR-head) in running mode"),
            new ST_SMART_DETAILS(0,0,"Unknown","Unknown")
        };

        public static ST_SMART_DETAILS GetAttrDesc(int ID)
        {
            for (int i = 0; i < SMARTAttributes.Length; i++)
            {
                if (SMARTAttributes[i].m_ucAttribId == ID) return SMARTAttributes[i];
            }
            
            ST_SMART_DETAILS xx = new ST_SMART_DETAILS();
            xx = SMARTAttributes[SMARTAttributes.Length - 1];
            xx.m_ucAttribId = ID;
            return xx;
        }

       
#region Structs

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GETVERSIONINPARAMS
        {
            public byte bVersion;
            public byte bRevision;
            public byte bReserved;
            public byte bIDEDeviceMap;
            public uint fCapabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_IDSECTOR
        {
            public ushort wGenConfig;
            public ushort wNumCyls;
            public ushort wReserved;
            public ushort wNumHeads;
            public ushort wbytesPerTrack;
            public ushort wbytesPerSector;
            public ushort wSectorsPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] wVendorUnique;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public char[] sSerialNumber;
            public ushort wBufferType;
            public ushort wBufferSize;
            public ushort wECCSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] sFirmwareRev;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 39)]
            public char[] sModelNumber;
            public ushort wMoreVendorUnique;
            public ushort wDoubleushortIO;
            public ushort wCapabilities;
            public ushort wReserved1;
            public ushort wPIOTiming;
            public ushort wDMATiming;
            public ushort wBS;
            public ushort wNumCurrentCyls;
            public ushort wNumCurrentHeads;
            public ushort wNumCurrentSectorsPerTrack;
            public ushort ulCurrentSectorCapacity;
            public ushort wMultSectorStuff;
            public uint ulTotalAddressableSectors;
            public ushort wSingleushortDMA;
            public ushort wMultiushortDMA;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 127)]
            public byte[] bReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_SMART_INFO
        {
            public byte m_ucAttribIndex;
            public uint m_dwAttribValue;
            public byte m_ucValue;
            public byte m_ucWorst;
            public uint m_dwThreshold;
            public ST_SMART_DETAILS Details;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_SMART_DETAILS
        {
            public int m_ucAttribId;            
            public int m_bCritical;            
            public string m_csAttribName;
            public string m_csAttribDetails;

            public ST_SMART_DETAILS(int newID, int NewCritical, string NewName, string NewDetails)
            {
                m_ucAttribId = newID;
                m_bCritical = NewCritical;
                m_csAttribName = NewName;
                m_csAttribDetails = NewDetails;
            }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_DRIVE_INFO
        {
            public GETVERSIONINPARAMS m_stGVIP;
            public ST_IDSECTOR m_stInfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ST_SMART_INFO[] m_stSmartInfo;
            public byte m_ucSmartValues;
            public byte m_ucDriveIndex;
            public bool m_SmartEnabled;
            public string m_csErrorString;            
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IDEREGS
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SENDCMDINPARAMS
        {
            public uint cBufferSize;
            public IDEREGS irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
            public byte bBuffer;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DRIVERSTATUS
        {
            public byte bDriverError;
            public byte bIDEError;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SENDCMDOUTPARAMS
        {
            public uint cBufferSize;
            public DRIVERSTATUS DriverStatus;
            public byte bBuffer;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_DRIVERSTAT
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ST_ATAOUTPARAM
        {
            public uint cBufferSize;
            public ST_DRIVERSTAT DriverStatus;
            public byte bBuffer;
        } ;
#endregion

        // ******************************************************************************
        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }
        
        uint SMART_GET_VERSION = CTL_CODE(CTL_CODE_FILE_DEVICE.IOCTL_DISK_BASE, 0x0020, CTL_CODE_METHOD.BUFFERED, CTL_CODE_FILE_ACCESS.READ);
        uint SMART_SEND_DRIVE_COMMAND = CTL_CODE(CTL_CODE_FILE_DEVICE.IOCTL_DISK_BASE, 0x0021, CTL_CODE_METHOD.BUFFERED, CTL_CODE_FILE_ACCESS.READ_WRITE);
        uint SMART_RCV_DRIVE_DATA = CTL_CODE(CTL_CODE_FILE_DEVICE.IOCTL_DISK_BASE, 0x0022, CTL_CODE_METHOD.BUFFERED, CTL_CODE_FILE_ACCESS.READ_WRITE);
        //
        // Bits returned in the fCapabilities member of GETVERSIONINPARAMS
        //
        public const uint CAP_ATA_ID_CMD = 1;       // ATA ID command supported
        public const uint CAP_ATAPI_ID_CMD = 2;       // ATAPI ID command supported
        public const uint CAP_SMART_CMD = 4;       // SMART commannds supported

        //
        // Valid values for the bCommandReg member of IDEREGS.
        //

        public const byte ATAPI_ID_CMD = 0xA1;            // Returns ID sector for ATAPI.
        public const byte ID_CMD = 0xEC;            // Returns ID sector for ATA.
        public const byte SMART_CMD = 0xB0;            // Performs SMART cmd.
        // Requires valid bFeaturesReg,
        // bCylLowReg, and bCylHighReg

        const int READ_ATTRIBUTE_BUFFER_SIZE = 512;
        const int IDENTIFY_BUFFER_SIZE = 512;
        const int READ_THRESHOLD_BUFFER_SIZE = 512;
        const int SMART_LOG_SECTOR_SIZE = 512;

        public ST_DRIVE_INFO[] GetSMARTForAllDrives()
        {
            System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();

            ST_DRIVE_INFO[] SMARTValues = new ST_DRIVE_INFO[allDrives.Length];
            int Count=0;
            for (byte i = 0; i < allDrives.Length; i++)
            {
                if (allDrives[i].DriveType == System.IO.DriveType.Fixed)
                {
                    if (GetSMART(i, ref SMARTValues[i]))
                    {
                        Count++;
                    }
                }
            }

            ST_DRIVE_INFO[] res = new ST_DRIVE_INFO[Count];
            Array.Copy(SMARTValues, res,Count);
            return res;
        }

        public bool GetSMART(byte ucDriveIndex, ref ST_DRIVE_INFO m_stDrivesInfo)
        {            
            uint dwRet = 0;
            string DriveName = "\\\\.\\PHYSICALDRIVE" + ucDriveIndex;
            IntPtr hDevice = kernel32.CreateFile(DriveName, F_GENERIC_.READ_WRITE, F_FILE_SHARE_.READ_WRITE, IntPtr.Zero, F_Creation.OPEN_EXISTING, F_ATTRIBUTE_.SYSTEM, IntPtr.Zero);
            if (hDevice == (new IntPtr(-1)))
            {
                dwRet = (uint)Marshal.GetLastWin32Error();
                return false;
            }

            m_stDrivesInfo.m_ucDriveIndex = ucDriveIndex;

            object TempObj = m_stDrivesInfo.m_stGVIP;
            bool bRet = kernel32.DeviceIoControl(hDevice, SMART_GET_VERSION, null, ref TempObj, ref dwRet);
            m_stDrivesInfo.m_stGVIP = (GETVERSIONINPARAMS)TempObj;
            if (bRet)
            {
                if ((m_stDrivesInfo.m_stGVIP.fCapabilities & CAP_SMART_CMD) == CAP_SMART_CMD)
                {
                    if (IsSmartEnabled(hDevice, ucDriveIndex, ref m_stDrivesInfo))
                    {
                        m_stDrivesInfo.m_SmartEnabled = true;
                        bRet = CollectDriveInfo(hDevice, ucDriveIndex, ref m_stDrivesInfo);
                        bRet = ReadSMARTAttributes(hDevice, ucDriveIndex, ref m_stDrivesInfo);
                    }
                    else
                    {
                        m_stDrivesInfo.m_SmartEnabled = false;
                    }
                }
            }
            kernel32.CloseHandle(hDevice);
            return true;
        }

        public const byte SMART_CYL_LOW = 0x4F;
        public const byte SMART_CYL_HI = 0xC2;
        public const byte DRIVE_HEAD_REG = 0xA0;
        bool IsSmartEnabled(IntPtr hDevice, byte ucDriveIndex, ref ST_DRIVE_INFO m_stDrivesInfo)
        {
            SENDCMDINPARAMS stCIP = new SENDCMDINPARAMS();
            SENDCMDOUTPARAMS stCOP = new SENDCMDOUTPARAMS();
            uint dwRet = 0;
            bool bRet = false;

            stCIP.cBufferSize = 0;
            stCIP.bDriveNumber = ucDriveIndex;
            stCIP.irDriveRegs.bFeaturesReg = C_SMART_SUBREGISTER.ENABLE_SMART;
            stCIP.irDriveRegs.bSectorCountReg = 1;
            stCIP.irDriveRegs.bSectorNumberReg = 1;
            stCIP.irDriveRegs.bCylLowReg = SMART_CYL_LOW;
            stCIP.irDriveRegs.bCylHighReg = SMART_CYL_HI;
            stCIP.irDriveRegs.bDriveHeadReg = DRIVE_HEAD_REG;
            stCIP.irDriveRegs.bCommandReg = SMART_CMD;
            object OutObj = stCOP;

            bRet = kernel32.DeviceIoControl(hDevice, SMART_SEND_DRIVE_COMMAND, stCIP, ref OutObj, ref dwRet);
            if (bRet)
            {

            }
            else
            {
                dwRet = (uint)Marshal.GetLastWin32Error();
                m_stDrivesInfo.m_csErrorString = "Error " + dwRet + " in reading SMART Enabled flag";
            }
            return bRet;
        }

        object ArrayToStructure(char[] Data, System.Type structureType, int Offset)
        {
            IntPtr ArrayPtr = Marshal.AllocHGlobal(Data.Length);
            for (int i = 0; i < Data.Length; i++)
            {
                Marshal.WriteByte(ArrayPtr, i, (byte)Data[i]);
            }
            IntPtr NewArrayPtr = new IntPtr(ArrayPtr.ToInt32() + Offset);
            object res = Marshal.PtrToStructure(NewArrayPtr, structureType);
            Marshal.FreeHGlobal(ArrayPtr);
            return res;
        }

        void ConvertString(ref char[] Data)
        {
            char[] TempData = new char[Data.Length];

            for (int nC1 = 0; nC1 < Data.Length; nC1 += 2)
            {
                if ((nC1 + 1) < Data.Length)
                {
                    TempData[nC1] = Data[nC1 + 1];
                    TempData[nC1 + 1] = Data[nC1];
                }
            }
            string xx = new string(TempData);
            xx=xx.Trim();
            xx=xx.Normalize();            
            TempData=xx.ToCharArray();
            TempData.CopyTo(Data,0);            
        }

        bool CollectDriveInfo(IntPtr hDevice, byte ucDriveIndex, ref ST_DRIVE_INFO m_stDrivesInfo)
        {
            bool bRet = false;
            SENDCMDINPARAMS stCIP = new SENDCMDINPARAMS();
            uint dwRet = 0;

            const int OUT_BUFFER_SIZE = IDENTIFY_BUFFER_SIZE + 16;
            char[] szOutput = new char[OUT_BUFFER_SIZE];

            stCIP.cBufferSize = IDENTIFY_BUFFER_SIZE;
            stCIP.bDriveNumber = ucDriveIndex;
            stCIP.irDriveRegs.bFeaturesReg = 0;
            stCIP.irDriveRegs.bSectorCountReg = 1;
            stCIP.irDriveRegs.bSectorNumberReg = 1;
            stCIP.irDriveRegs.bCylLowReg = 0;
            stCIP.irDriveRegs.bCylHighReg = 0;
            stCIP.irDriveRegs.bDriveHeadReg = DRIVE_HEAD_REG;
            stCIP.irDriveRegs.bCommandReg = ID_CMD;

            object TempO = szOutput;
            bRet = kernel32.DeviceIoControl(hDevice, SMART_RCV_DRIVE_DATA, stCIP, ref TempO, ref dwRet);
            szOutput = (char[])TempO;
            if (bRet)
            {                
                m_stDrivesInfo.m_stInfo = (ST_IDSECTOR)ArrayToStructure(szOutput, m_stDrivesInfo.m_stInfo.GetType(), 16);
                ConvertString(ref m_stDrivesInfo.m_stInfo.sModelNumber);
                ConvertString(ref m_stDrivesInfo.m_stInfo.sSerialNumber);
                ConvertString(ref m_stDrivesInfo.m_stInfo.sFirmwareRev);
            }
            else
            {
                dwRet = (uint)Marshal.GetLastWin32Error();
            }
            return bRet;
        }

        uint ReadUint(char[] Data, int Offset)
        {
            int res = 0;
            for (int i = 0; i < 4; i++)
            {
                res |= (Data[Offset++] << (i * 8));
            }
            return (uint)res;
        }

        bool ReadSMARTAttributes(IntPtr hDevice, byte ucDriveIndex, ref ST_DRIVE_INFO m_stDrivesInfo)
        {
            SENDCMDINPARAMS stCIP = new SENDCMDINPARAMS();
            uint dwRet = 0;
            bool bRet = false;
            char[] szAttributes = new char[Marshal.SizeOf(typeof(ST_ATAOUTPARAM)) + READ_ATTRIBUTE_BUFFER_SIZE - 1];
            m_stDrivesInfo.m_stSmartInfo = new ST_SMART_INFO[256];

            stCIP.cBufferSize = READ_ATTRIBUTE_BUFFER_SIZE;
            stCIP.bDriveNumber = ucDriveIndex;
            stCIP.irDriveRegs.bFeaturesReg = C_SMART_SUBREGISTER.READ_ATTRIBUTES;
            stCIP.irDriveRegs.bSectorCountReg = 1;
            stCIP.irDriveRegs.bSectorNumberReg = 1;
            stCIP.irDriveRegs.bCylLowReg = SMART_CYL_LOW;
            stCIP.irDriveRegs.bCylHighReg = SMART_CYL_HI;
            stCIP.irDriveRegs.bDriveHeadReg = DRIVE_HEAD_REG;
            stCIP.irDriveRegs.bCommandReg = SMART_CMD;

            object TempO = szAttributes;
            bRet = kernel32.DeviceIoControl(hDevice, SMART_RCV_DRIVE_DATA, stCIP, ref TempO, ref dwRet);
            szAttributes = (char[])TempO;
            if (bRet)
            {
                m_stDrivesInfo.m_ucSmartValues = 0;
                m_stDrivesInfo.m_ucDriveIndex = ucDriveIndex;
             
                int pT1 = Marshal.SizeOf(typeof(ST_DRIVERSTAT)) + 4; // buffer        
                int pT3, pT2;
                for (int ucT1 = 0; ucT1 < 30; ++ucT1)
                {
                    pT3 = pT1 + 2 + ucT1 * 12;
                    pT2 = pT3 + C_SMART_ATTRIB_INDEX.RAW;

                    for (int i = 2; i < 7; i++)
                    {
                        szAttributes[pT3 + C_SMART_ATTRIB_INDEX.RAW + i] = (char)0;
                    }

                    if (szAttributes[pT3 + C_SMART_ATTRIB_INDEX.INDEX] != 0)
                    {
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues] = new ST_SMART_INFO();
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_ucAttribIndex = (byte)szAttributes[pT3 + C_SMART_ATTRIB_INDEX.INDEX];
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_ucValue = (byte)szAttributes[pT3 + C_SMART_ATTRIB_INDEX.VALUE];
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_ucWorst = (byte)szAttributes[pT3 + C_SMART_ATTRIB_INDEX.WORST];
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_dwAttribValue = ReadUint(szAttributes, pT2);
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_dwThreshold = uint.MaxValue;
                        m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].Details = GetAttrDesc(m_stDrivesInfo.m_stSmartInfo[m_stDrivesInfo.m_ucSmartValues].m_ucAttribIndex);
                        m_stDrivesInfo.m_ucSmartValues++;
                    }
                }
            }
            else
                dwRet = (uint)Marshal.GetLastWin32Error();

            stCIP.irDriveRegs.bFeaturesReg = C_SMART_SUBREGISTER.READ_THRESHOLDS;
            stCIP.cBufferSize = READ_THRESHOLD_BUFFER_SIZE; // Is same as attrib size
            TempO = szAttributes;
            bRet = kernel32.DeviceIoControl(hDevice, SMART_RCV_DRIVE_DATA, stCIP, ref TempO, ref dwRet);
            szAttributes = (char[])TempO;
            if (bRet)
            {
                int pT1 = Marshal.SizeOf(typeof(ST_DRIVERSTAT)) + 4; // buffer        
                int pT3, pT2;
                for (int ucT1 = 0; ucT1 < 30; ++ucT1)
                {
                    pT2 = pT1 + 2 + ucT1 * 12 + 5;
                    pT3 = pT1 + 2 + ucT1 * 12;

                    for (int i = 2; i < 7; i++)
                    {
                        szAttributes[pT3 + C_SMART_ATTRIB_INDEX.RAW + i] = (char)0;
                    }

                    if (szAttributes[pT3] != 0)
                    {
                        for (int i = 0; i < m_stDrivesInfo.m_ucSmartValues; i++)
                        {
                            if (m_stDrivesInfo.m_stSmartInfo[i].m_ucAttribIndex == szAttributes[pT3])
                            {
                                m_stDrivesInfo.m_stSmartInfo[i].m_dwThreshold = szAttributes[pT2];
                                break;
                            }
                        }
                    }
                }
            }
            return bRet;
        }
    }
}
