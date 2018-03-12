using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CommonControls.WinCEApi
{
    public static class coredll
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
        /// <summary>CreateFile : Open file for read</summary>
        public const uint GENERIC_READ = 0x80000000;
        /// <summary>CreateFile : Open file for write</summary>
        public const uint GENERIC_WRITE = 0x40000000;
        /// <summary>CreateFile : file share for write</summary>
        public const uint FILE_SHARE_WRITE = 0x2;
        /// <summary>CreateFile : file share for read</summary>
        public const uint FILE_SHARE_READ = 0x1;
        /// <summary>CreateFile : Open handle for overlapped operations</summary>
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        /// <summary>CreateFile : Resource to be "created" must exist</summary>
        public const uint OPEN_EXISTING = 3;
        /// <summary>CreateFile : Resource will be "created" or existing will be used</summary>
        public const uint OPEN_ALWAYS = 4;        
        /// <summary>Infinite timeout</summary>
        public const uint INFINITE = 0xFFFFFFFF;
        /// <summary>Simple representation of a null handle : a closed stream will get this handle. Note it is public for comparison by higher level classes.</summary>
        public static IntPtr NullHandle = IntPtr.Zero;
        /// <summary>Simple representation of the handle returned when CreateFile fails.</summary>
        public static IntPtr InvalidHandleValue = new IntPtr(-1);

        public const int WAIT_TIMEOUT = 0x102;
        public const short WAIT_OBJECT_0 = 0;

        public enum WaveFormats
        {
            INVALIDFORMAT = 0x00000000,       /* invalid format */
            _11_kHz_Mono_8bit = 0x00000001,         /* 11.025 kHz, Mono,   8-bit  */
            _11_kHz_Stereo_8bit = 0x00000002,         /* 11.025 kHz, Stereo, 8-bit  */
            _11_kHz_Mono_16bit = 0x00000004,       /* 11.025 kHz, Mono,   16-bit */
            _11_kHz_Stereo_16bit = 0x00000008,       /* 11.025 kHz, Stereo, 16-bit */
            _22_kHz_Mono_8bit = 0x00000010,       /* 22.05  kHz, Mono,   8-bit  */
            _22_kHz_Stereo_8bit = 0x00000020,       /* 22.05  kHz, Stereo, 8-bit  */
            _22_kHz_Mono_16bit = 0x00000040,       /* 22.05  kHz, Mono,   16-bit */
            _22_kHz_Stereo_16bit = 0x00000080,       /* 22.05  kHz, Stereo, 16-bit */
            _44_kHz_Mono_8bit = 0x00000100,       /* 44.1   kHz, Mono,   8-bit  */
            _44_kHz_Stereo_8bit = 0x00000200,       /* 44.1   kHz, Stereo, 8-bit  */
            _44_kHz_Mono_16bit = 0x00000400,       /* 44.1   kHz, Mono,   16-bit */
            _44_kHz_Stereo_16bit = 0x00000800       /* 44.1   kHz, Stereo, 16-bit */
        }

        public enum WaveDevSupport
        {
            Pitch_Control = 0x0001,   /* supports pitch control */
            PlaybackRate_Control = 0x0002,   /* supports playback rate control */
            Volume_Control = 0x0004,   /* supports volume control */
            Left_Right_Volume_Control = 0x0008,   /* separate left-right volume control */
            //SYNC           0x0010, /* Windows CE only supports asynchronous audio devices */
            SAMPLEACCURATE = 0x0020,
            DIRECTSOUND = 0x0040,
        }

        #endregion

        // ******************************************************************************
        // Structures and classes for API calls, listed alphabetically
        // ******************************************************************************

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height { get { return Bottom - Top; } }
            public int Width { get { return Right - Left; } }
            public Size Size { get { return new Size(Width, Height); } }

            public Point Location { get { return new Point(Left, Top); } }

            // Handy method for converting to a System.Drawing.Rectangle
            public Rectangle ToRectangle()
            { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }

            public static RECT FromRectangle(Rectangle rectangle)
            {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                  ^ ((Width << 0x1a) | (Width >> 6))
                  ^ ((Height << 7) | (Height >> 0x19));
            }

            #region Operator overloads

            public static implicit operator Rectangle(RECT rect)
            {
                return rect.ToRectangle();
            }

            public static implicit operator RECT(Rectangle rect)
            {
                return FromRectangle(rect);
            }

            #endregion
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WAVEOUTCAPS
        {
            public short wMid;
            public short wPid;
            public int vDriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;

            public int dwFormats;
            public short wChannels;

            short Reserved;

            public uint dwSupport;
        }

        // API declarations relating to Windows error messages.

        // ******************************************************************************
        // API constants
        // ******************************************************************************

        public const short FORMAT_MESSAGE_FROM_SYSTEM = 0x1000; // S;

        // ******************************************************************************
        // API functions, listed alphabetically
        // ******************************************************************************

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("coredll.dll")]
        public static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite,
out UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("coredll.dll")]
        public static extern Boolean ReadFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToRead,
out UInt32 lpNumberOfBytesReaded, IntPtr lpOverlapped);

        [DllImport("coredll.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);


        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int DeviceIoControl(
            IntPtr hDevice,
            int dwIoControlCode,
            byte[] lpInBuffer,
            uint nInBufferSize,
            byte[] lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
           IntPtr lpOverlapped);


        [DllImport("coredll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("coredll.dll", EntryPoint = "GetProcAddressW", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("coredll", EntryPoint = "GetDC", SetLastError = true)]
        public static extern IntPtr GetDCCE(IntPtr hWnd);

        [DllImport("coredll.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
        public static extern int ReleaseDCCE(IntPtr hWnd, IntPtr hDC);

        [DllImport("coredll")]
        public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("coredll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("coredll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("coredll.dll", EntryPoint = "waveOutGetNumDevs", SetLastError = true)]
        public static extern int waveOutGetNumDevs();

        [DllImport("coredll.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint waveOutGetDevCaps(uint DeviceID, ref WAVEOUTCAPS pwoc, uint cbwoc);

        public static WAVEOUTCAPS WaveOutGetDevCaps(uint DeviceID)
        {
            WAVEOUTCAPS Caps = new WAVEOUTCAPS();
            waveOutGetDevCaps(DeviceID, ref Caps, (uint)Marshal.SizeOf(Caps));
            return Caps;
        }
    }
}
