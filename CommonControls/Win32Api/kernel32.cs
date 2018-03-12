using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CommonControls.Win32Api
{
    //public static class F_FileAttributes
    //{
    //    public const uint Readonly = 0x00000001;
    //    public const uint Hidden = 0x00000002;
    //    public const uint System = 0x00000004;
    //    public const uint Directory = 0x00000010;
    //    public const uint Archive = 0x00000020;
    //    public const uint Device = 0x00000040;
    //    public const uint Normal = 0x00000080;
    //    public const uint Temporary = 0x00000100;
    //    public const uint SparseFile = 0x00000200;
    //    public const uint ReparsePoint = 0x00000400;
    //    public const uint Compressed = 0x00000800;
    //    public const uint Offline = 0x00001000;
    //    public const uint NotContentIndexed = 0x00002000;
    //    public const uint Encrypted = 0x00004000;
    //    public const uint Write_Through = 0x80000000;
    //    public const uint Overlapped = 0x40000000;
    //    public const uint NoBuffering = 0x20000000;
    //    public const uint RandomAccess = 0x10000000;
    //    public const uint SequentialScan = 0x08000000;
    //    public const uint DeleteOnClose = 0x04000000;
    //    public const uint BackupSemantics = 0x02000000;
    //    public const uint PosixSemantics = 0x01000000;
    //    public const uint OpenReparsePoint = 0x00200000;
    //    public const uint OpenNoRecall = 0x00100000;
    //    public const uint FirstPipeInstance = 0x00080000;
    //}

    public static class F_GENERIC_
    {
        public const uint READ = 0x80000000;

        public const uint WRITE = 0x40000000;

        public const uint EXECUTE = 0x20000000;

        public const uint ALL = 0x10000000;

        public const uint READ_WRITE = READ | WRITE; // my type
    }

    public static class F_FILE_SHARE_
    {
        /// <summary>
        ///
        /// </summary>
        public const uint NONE = 0x00000000;
        /// <summary>
        /// Enables subsequent open operations on an object to request read access.
        /// Otherwise, other processes cannot open the object if they request read access.
        /// If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        public const uint READ = 0x00000001;
        /// <summary>
        /// Enables subsequent open operations on an object to request write access.
        /// Otherwise, other processes cannot open the object if they request write access.
        /// If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        public const uint WRITE = 0x00000002;
        /// <summary>
        /// Enables subsequent open operations on an object to request delete access.
        /// Otherwise, other processes cannot open the object if they request delete access.
        /// If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        public const uint DELETE = 0x00000004;

        public const uint READ_WRITE = READ | WRITE; // my type
    }

    public static class F_Creation
    {
        /// <summary>
        /// Creates a new file. The function fails if a specified file exists.
        /// </summary>
        public const uint CREATE_NEW = 1;
        /// <summary>
        /// Creates a new file, always.
        /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes,
        /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
        /// </summary>
        public const uint CREATE_ALWAYS = 2;
        /// <summary>
        /// Opens a file, always.
        /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
        /// </summary>
        public const uint OPEN_EXISTING = 3;
        /// <summary>
        /// Opens a file. The function fails if the file does not exist.
        /// </summary>
        public const uint OPEN_ALWAYS = 4;
        /// <summary>
        /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
        /// The calling process must open the file with the GENERIC_WRITE access right.
        /// </summary>
        public const uint TRUNCATE_EXISTING = 5;
    }

    public static class F_ATTRIBUTE_
    {
        public const uint READONLY = 0x00000001;
        public const uint HIDDEN = 0x00000002;
        public const uint SYSTEM = 0x00000004;
        public const uint DIRECTORY = 0x00000010;
        public const uint ARCHIVE = 0x00000020;
        public const uint DEVICE = 0x00000040;
        public const uint NORMAL = 0x00000080;
        public const uint TEMPORARY = 0x00000100;
        public const uint SPARSE_FILE = 0x00000200;
        public const uint REPARSE_POINT = 0x00000400;
        public const uint COMPRESSED = 0x00000800;
        public const uint OFFLINE = 0x00001000;
        public const uint NOT_CONTENT_INDEXED = 0x00002000;
        public const uint ENCRYPTED = 0x00004000;
    }

    public static class F_FILE_FLAG_
    {
        public const uint BACKUP_SEMANTICS = 0x02000000;
        public const uint DELETE_ON_CLOSE = 0x04000000;
        public const uint NO_BUFFERING = 0x20000000;
        public const uint OPEN_NO_RECALL = 0x00100000;
        public const uint OPEN_REPARSE_POINT = 0x00200000;
        public const uint OVERLAPPED = 0x40000000;
        public const uint POSIX_SEMANTICS = 0x0100000;
        public const uint RANDOM_ACCESS = 0x10000000;
        public const uint SEQUENTIAL_SCAN = 0x08000000;
        public const uint WRITE_THROUGH = 0x80000000;
    }

    public static class F_SecurityFILE_
    {
        public const uint ADD_FILE = 0x0002;
        public const uint ADD_SUBDIRECTORY = 0x0004;
        public const uint APPEND_DATA = 0x0004;
        public const uint CREATE_PIPE_INSTANCE = 0x0004;
        public const uint FILE_EXECUTE = 0x0020;
    }

    public static class CTL_CODE_METHOD
    {
        public const uint BUFFERED = 0;
        public const uint IN_DIRECT = 1;
        public const uint OUT_DIRECT = 2;
        public const uint NEITHER = 3;
    }

    public static class CTL_CODE_FILE_ACCESS
    {
        public const uint ANY = 0;
        public const uint SPECIAL = (ANY);
        public const uint READ = (0x0001);    // file & pipe
        public const uint WRITE = (0x0002);    // file & pipe
        public const uint READ_WRITE = (READ | WRITE);
    }

    public static class CTL_CODE_FILE_DEVICE
    {
        public const uint BEEP = 0x00000001;
        public const uint CD_ROM = 0x00000002;
        public const uint CD_ROM_FILE_SYSTEM = 0x00000003;
        public const uint CONTROLLER = 0x00000004;
        public const uint DATALINK = 0x00000005;
        public const uint DFS = 0x00000006;
        public const uint DISK = 0x00000007;
        public const uint DISK_FILE_SYSTEM = 0x00000008;
        public const uint FILE_SYSTEM = 0x00000009;
        public const uint INPORT_PORT = 0x0000000a;
        public const uint KEYBOARD = 0x0000000b;
        public const uint MAILSLOT = 0x0000000c;
        public const uint MIDI_IN = 0x0000000d;
        public const uint MIDI_OUT = 0x0000000e;
        public const uint MOUSE = 0x0000000f;
        public const uint MULTI_UNC_PROVIDER = 0x00000010;
        public const uint NAMED_PIPE = 0x00000011;
        public const uint NETWORK = 0x00000012;
        public const uint NETWORK_BROWSER = 0x00000013;
        public const uint NETWORK_FILE_SYSTEM = 0x00000014;
        public const uint NULL = 0x00000015;
        public const uint PARALLEL_PORT = 0x00000016;
        public const uint PHYSICAL_NETCARD = 0x00000017;
        public const uint PRINTER = 0x00000018;
        public const uint SCANNER = 0x00000019;
        public const uint SERIAL_MOUSE_PORT = 0x0000001a;
        public const uint SERIAL_PORT = 0x0000001b;
        public const uint SCREEN = 0x0000001c;
        public const uint SOUND = 0x0000001d;
        public const uint STREAMS = 0x0000001e;
        public const uint TAPE = 0x0000001f;
        public const uint TAPE_FILE_SYSTEM = 0x00000020;
        public const uint TRANSPORT = 0x00000021;
        public const uint UNKNOWN = 0x00000022;
        public const uint VIDEO = 0x00000023;
        public const uint VIRTUAL_DISK = 0x00000024;
        public const uint WAVE_IN = 0x00000025;
        public const uint WAVE_OUT = 0x00000026;
        public const uint PORT_8042 = 0x00000027;
        public const uint NETWORK_REDIRECTOR = 0x00000028;
        public const uint BATTERY = 0x00000029;
        public const uint BUS_EXTENDER = 0x0000002a;
        public const uint MODEM = 0x0000002b;
        public const uint VDM = 0x0000002c;
        public const uint MASS_STORAGE = 0x0000002d;
        public const uint SMB = 0x0000002e;
        public const uint KS = 0x0000002f;
        public const uint CHANGER = 0x00000030;
        public const uint SMARTCARD = 0x00000031;
        public const uint ACPI = 0x00000032;
        public const uint DVD = 0x00000033;
        public const uint FULLSCREEN_VIDEO = 0x00000034;
        public const uint DFS_FILE_SYSTEM = 0x00000035;
        public const uint DFS_VOLUME = 0x00000036;
        public const uint SERENUM = 0x00000037;
        public const uint TERMSRV = 0x00000038;
        public const uint KSEC = 0x00000039;
        public const uint FIPS = 0x0000003A;
        public const uint INFINIBAND = 0x0000003B;
        public const uint IOCTL_DISK_BASE = DISK;
    }

    public static class kernel32
    {
        // API declarations relating to file I/O.
        // ******************************************************************************
        // API constants
        // ******************************************************************************

        #region Constants
        /// <summary>WParam for above : A device was inserted</summary>
        public const int DEVICE_ARRIVAL = 0x8000;
        /// <summary>WParam for above : A device was removed</summary>
        public const int DEVICE_REMOVECOMPLETE = 0x8004;
        /// <summary>Used in SetupDiClassDevs to get devices present in the system</summary>
        public const int DIGCF_PRESENT = 0x02;
        /// <summary>Used in SetupDiClassDevs to get device interface details</summary>
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        /// <summary>Used when registering for device insert/remove messages : specifies the type of device</summary>
        public const int DEVTYP_DEVICEINTERFACE = 0x05;
        /// <summary>Used when registering for device insert/remove messages : we're giving the API call a window handle</summary>
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        /// <summary>Purges Win32 transmit buffer by aborting the current transmission.</summary>
        public const uint PURGE_TXABORT = 0x01;
        /// <summary>Purges Win32 receive buffer by aborting the current receive.</summary>
        public const uint PURGE_RXABORT = 0x02;
        /// <summary>Purges Win32 transmit buffer by clearing it.</summary>
        public const uint PURGE_TXCLEAR = 0x04;
        /// <summary>Purges Win32 receive buffer by clearing it.</summary>
        public const uint PURGE_RXCLEAR = 0x08;


        /// <summary>Infinite timeout</summary>
        public const uint INFINITE = 0xFFFFFFFF;
        /// <summary>Simple representation of a null handle : a closed stream will get this handle. Note it is public for comparison by higher level classes.</summary>
        public static IntPtr NullHandle = IntPtr.Zero;
        /// <summary>Simple representation of the handle returned when CreateFile fails.</summary>
        public static IntPtr InvalidHandleValue = new IntPtr(-1);
        //------------------------------ DIOC
        public const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        public const uint FILE_DEVICE_HAL = 0x00000101;
        public const uint FILE_DEVICE_CONSOLE = 0x00000102;
        public const uint FILE_DEVICE_PSL = 0x00000103;
        public const uint METHOD_BUFFERED = 0;
        public const uint METHOD_IN_DIRECT = 1;
        public const uint METHOD_OUT_DIRECT = 2;
        public const uint METHOD_NEITHER = 3;
        public const uint FILE_ANY_ACCESS = 0;
        public const uint FILE_READ_ACCESS = 0x0001;
        public const uint FILE_WRITE_ACCESS = 0x0002;

        internal const int FILE_ATTRIBUTE_ARCHIVE = 0x20;
        internal const int INVALID_FILE_ATTRIBUTES = -1;

        internal const int FILE_READ_DATA = 0x0001;
        internal const int FILE_WRITE_DATA = 0x0002;
        internal const int FILE_APPEND_DATA = 0x0004;
        internal const int FILE_READ_EA = 0x0008;
        internal const int FILE_WRITE_EA = 0x0010;

        internal const int FILE_READ_ATTRIBUTES = 0x0080;
        internal const int FILE_WRITE_ATTRIBUTES = 0x0100;

        internal const int FILE_SHARE_NONE = 0x00000000;
        internal const int FILE_SHARE_READ = 0x00000001;

        internal const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

        internal const long FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE |
                                                    FILE_WRITE_DATA |
                                                    FILE_WRITE_ATTRIBUTES |
                                                    FILE_WRITE_EA |
                                                    FILE_APPEND_DATA |
                                                    SYNCHRONIZE;

        internal const long FILE_GENERIC_READ = STANDARD_RIGHTS_READ |
                                                FILE_READ_DATA |
                                                FILE_READ_ATTRIBUTES |
                                                FILE_READ_EA |
                                                SYNCHRONIZE;



        internal const long READ_CONTROL = 0x00020000L;
        internal const long STANDARD_RIGHTS_READ = READ_CONTROL;
        internal const long STANDARD_RIGHTS_WRITE = READ_CONTROL;

        internal const long SYNCHRONIZE = 0x00100000L;

        internal const int CREATE_NEW = 1;
        internal const int CREATE_ALWAYS = 2;
        internal const int OPEN_EXISTING = 3;

        internal const int MAX_PATH = 260;
        internal const int MAX_ALTERNATE = 14;

        //------------------------------- File


        public enum GET_FILEEX_INFO_LEVELS
        {
            GetFileExInfoStandard,
            GetFileExMaxInfoLevel
        }
        public enum DriveType : uint
        {
            /// <summary>The drive type cannot be determined.</summary>
            Unknown = 0,    //DRIVE_UNKNOWN
            /// <summary>The root path is invalid, for example, no volume is mounted at the path.</summary>
            Error = 1,        //DRIVE_NO_ROOT_DIR
            /// <summary>The drive is a type that has removable media, for example, a floppy drive or removable hard disk.</summary>
            Removable = 2,    //DRIVE_REMOVABLE
            /// <summary>The drive is a type that cannot be removed, for example, a fixed hard drive.</summary>
            Fixed = 3,        //DRIVE_FIXED
            /// <summary>The drive is a remote (network) drive.</summary>
            Remote = 4,        //DRIVE_REMOTE
            /// <summary>The drive is a CD-ROM drive.</summary>
            CDROM = 5,        //DRIVE_CDROM
            /// <summary>The drive is a RAM disk.</summary>
            RAMDisk = 6        //DRIVE_RAMDISK
        }

        // ƒл€ SetFilePointer
        [Flags]
        public enum F_PtrFILE_ : uint
        {
            BEGIN = 0,
            CURRENT = 1,
            END = 2
        }
        //-------------------------------

        public const int WAIT_TIMEOUT = 0x102;
        public const short WAIT_OBJECT_0 = 0;

        private const int DIFFERENCE = 11;
        public enum Resources
        {
            RT_CURSOR = 1,
            RT_BITMAP = 2,
            RT_ICON = 3,
            RT_MENU = 4,
            RT_DIALOG = 5,
            RT_STRING = 6,
            RT_FONTDIR = 7,
            RT_FONT = 8,
            RT_ACCELERATOR = 9,
            RT_RCDATA = 10,
            RT_MESSAGETABLE = 11,

            RT_GROUP_CURSOR = RT_CURSOR + DIFFERENCE,
            RT_GROUP_ICON = RT_ICON + DIFFERENCE,
            RT_VERSION = 16,
            RT_DLGINCLUDE = 17,

            RT_PLUGPLAY = 19,
            RT_VXD = 20,
            RT_ANICURSOR = 21,
            RT_ANIICON = 22,

            RT_HTML = 23,
        }

        /// <summary>
        /// File attributes are metadata values stored by the file system on disk and are used by the system and are available to developers via various file I/O APIs.
        /// </summary>
        [Flags]
        [CLSCompliant(false)]
        public enum FileAttributes : uint
        {
            /// <summary>
            /// A file that is read-only. Applications can read the file, but cannot write to it or delete it. This attribute is not honored on directories. For more information, see "You cannot view or change the Read-only or the System attributes of folders in Windows Server 2003, in Windows XP, or in Windows Vista".
            /// </summary>
            Readonly = 0x00000001,

            /// <summary>
            /// The file or directory is hidden. It is not included in an ordinary directory listing.
            /// </summary>
            Hidden = 0x00000002,

            /// <summary>
            /// A file or directory that the operating system uses a part of, or uses exclusively.
            /// </summary>
            System = 0x00000004,

            /// <summary>
            /// The handle that identifies a directory.
            /// </summary>
            Directory = 0x00000010,

            /// <summary>
            /// A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal.
            /// </summary>
            Archive = 0x00000020,

            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            Device = 0x00000040,

            /// <summary>
            /// A file that does not have other attributes set. This attribute is valid only when used alone.
            /// </summary>
            Normal = 0x00000080,

            /// <summary>
            /// A file that is being used for temporary storage. File systems avoid writing data back to mass storage if sufficient cache memory is available, because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing the data. Otherwise, the data is written after the handle is closed.
            /// </summary>
            Temporary = 0x00000100,

            /// <summary>
            /// A file that is a sparse file.
            /// </summary>
            SparseFile = 0x00000200,

            /// <summary>
            /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
            /// </summary>
            ReparsePoint = 0x00000400,

            /// <summary>
            /// A file or directory that is compressed. For a file, all of the data in the file is compressed. For a directory, compression is the default for newly created files and subdirectories.
            /// </summary>
            Compressed = 0x00000800,

            /// <summary>
            /// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage. This attribute is used by Remote Storage, which is the hierarchical storage management software. Applications should not arbitrarily change this attribute.
            /// </summary>
            Offline = 0x00001000,

            /// <summary>
            /// The file or directory is not to be indexed by the content indexing service.
            /// </summary>
            NotContentIndexed = 0x00002000,

            /// <summary>
            /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
            /// </summary>
            Encrypted = 0x00004000,

            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            Virtual = 0x00010000
        }


        #endregion

        // ******************************************************************************
        // Structures and classes for API calls, listed alphabetically
        // ******************************************************************************    

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint DateTimeLow;
            public uint DateTimeHigh;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            public System.IO.FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
            public uint nFileSizeLow;  //|
            public uint dwReserved0;   //|
            public uint dwReserved1;   //v
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OVERLAPPED
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public IntPtr hEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }

        // API declarations relating to Windows error messages.

        // ******************************************************************************
        // API constants
        // ******************************************************************************

        public const short FORMAT_MESSAGE_FROM_SYSTEM = 0x1000; // S;

        // ******************************************************************************
        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        // ******************************************************************************
        // API functions, listed alphabetically
        // ******************************************************************************        

        [DllImport("kernel32.dll")]
        public static extern int CancelIo(IntPtr hFile);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateEvent(ref SECURITY_ATTRIBUTES SecurityAttributes, int bManualReset, int bInitialState, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr
          CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr lpTemplate);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr
          CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr lpTemplate);

        [DllImport("kernel32.dll")]
        public static extern int ReadFile(IntPtr hFile, ref byte lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern uint GetFileSize(IntPtr hFile, out uint lpFileSizeHigh);

        [DllImport("kernel32.dll")]
        public static extern bool GetFileSizeEx(IntPtr hFile, out long lpFileSize);

        [DllImport("kernel32.dll")]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern int WriteFile(IntPtr hFile, ref byte lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern int WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern int WriteFile(IntPtr hFile, double[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int FormatMessage(int dwFlags, ref long lpSource, int dwMessageId, int dwLanguageZId, string lpBuffer, int nSize, int Arguments);

        [DllImport("kernel32.dll")]
        public static extern bool GetOverlappedResult(IntPtr hFile,
           ref System.Threading.NativeOverlapped lpOverlapped,
           ref uint lpNumberOfBytesTransferred, bool bWait);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
            [In] byte[] lpInBuffer, uint nInBufferSize,
            byte[] lpOutBuffer, uint nOutBufferSize,
            ref uint lpBytesReturned, ref System.Threading.NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
            IntPtr lpInBuffer, int nInBufferSize,
            IntPtr lpOutBuffer, int nOutBufferSize,
            ref uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CopyFileW(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetFileAttributesW(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int GetFileAttributesA(string lpFileName);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteFileW(string lpFileName);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool MoveFileW(string lpExistingFileName, string lpNewFileName);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);


        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool FindClose(IntPtr hFindFile);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RemoveDirectory(string path);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SetFileAttributesW(string lpFileName, int fileAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int SetFileAttributesA(string lpFileName, int fileAttributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]

        internal static extern bool GetFileAttributesExW(string lpFileName,
          GET_FILEEX_INFO_LEVELS fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]

        internal static extern bool GetFileAttributesExA(string lpFileName,
          GET_FILEEX_INFO_LEVELS fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        private static IntPtr StructToPtr(ref object InO, ref int InOSize)
        {
            IntPtr InOPtr = IntPtr.Zero;
            if (InO != null)
            {
                if (!InO.GetType().IsArray)
                {
                    InOSize = Marshal.SizeOf(InO);
                    InOPtr = Marshal.AllocHGlobal(InOSize);
                    Marshal.StructureToPtr(InO, InOPtr, true);
                }
                else
                {
                    char[] TmpArray = (char[])InO;
                    InOSize = TmpArray.Length;
                    InOPtr = Marshal.AllocHGlobal(InOSize);
                    for (int i = 0; i < TmpArray.Length; i++)
                    {
                        Marshal.WriteByte(InOPtr, i, (byte)TmpArray[i]);
                    }
                }
            }
            return InOPtr;
        }

        private static object PtrToStruct(IntPtr InOPtr, ref object InO)
        {
            if (InOPtr != IntPtr.Zero)
            {
                if (!InO.GetType().IsArray)
                {
                    InO = Marshal.PtrToStructure(InOPtr, InO.GetType());
                }
                else
                {
                    char[] TmpArray = (char[])InO;
                    for (int i = 0; i < TmpArray.Length; i++)
                    {
                        TmpArray[i] = (char)Marshal.ReadByte(InOPtr, i);
                    }
                }
                Marshal.FreeHGlobal(InOPtr);
            }
            return InO;
        }

        public static bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
          object lpInBuffer,
          ref object lpOutBuffer,
          ref uint lpBytesReturned)
        {
            bool res;
            int InBufferSize = 0;
            int OutBufferSize = 0;

            IntPtr InBuffer = StructToPtr(ref lpInBuffer, ref InBufferSize);
            IntPtr OutBuffer = StructToPtr(ref lpOutBuffer, ref OutBufferSize);

            res = DeviceIoControl(hDevice, dwIoControlCode, InBuffer, InBufferSize, OutBuffer, OutBufferSize, ref lpBytesReturned, IntPtr.Zero);

            PtrToStruct(InBuffer, ref lpInBuffer); // просто очищ€ем пам€ть
            lpOutBuffer = PtrToStruct(OutBuffer, ref lpOutBuffer);

            return res;
        }

        /// <summary>
        /// The GetDriveType function determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive
        /// </summary>
        /// <param name="lpRootPathName">A pointer to a null-terminated string that specifies the root directory and returns information about the disk.A trailing backslash is required. If this parameter is NULL, the function uses the root of the current directory.</param>
        [DllImport("kernel32.dll")]
        public static extern DriveType GetDriveType([MarshalAs(UnmanagedType.LPStr)] string lpRootPathName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, int lpName, int lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, int lpName, string lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, int lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, Resources lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        public static System.Drawing.Icon LoadResourceIcon(string FileName, string IconName)
        {

            System.Drawing.Icon result = null;
            IntPtr File = LoadLibrary(FileName);
            if (File != null && File != IntPtr.Zero && File != (IntPtr)(-1))
            {
                IconName += (char)0;
                IntPtr p_Icon = user32.LoadIcon(File, IconName);
                if (p_Icon != null && p_Icon != IntPtr.Zero)
                {
                    result = System.Drawing.Icon.FromHandle(p_Icon);
                    CloseHandle(p_Icon);
                }
                FreeLibrary(File);
            }
            return result;
        }

        public static System.Drawing.Icon LoadResourceIcon(string FileName, int IconNumber)
        {

            System.Drawing.Icon result = null;
            IntPtr File = LoadLibrary(FileName);
            if (File != null && File != IntPtr.Zero && File != (IntPtr)(-1))
            {
                IntPtr p_Icon = user32.LoadIcon(File, (IntPtr)IconNumber);
                if (p_Icon != null && p_Icon != IntPtr.Zero)
                {
                    result = System.Drawing.Icon.FromHandle(p_Icon);
                    CloseHandle(p_Icon);
                }
                FreeLibrary(File);
            }
            return result;
        }
    }
}
