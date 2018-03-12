using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonControls
{
    public class c_CircularBuffer
    {
        uint ReadPointer,
            WritePointer;
        ulong ReadCounter,
                WriteCounter;
        uint BufferSize = 1;
        byte[] Data;

        AutoResetEvent WriteExecuted = new AutoResetEvent(false),
            ReadExecuted = new AutoResetEvent(false);
        

        public uint p_BufferSize
        {
            set
            {
                BufferSize = value;
                Data = new byte[BufferSize];
                FlushBuffer();
            }
            get
            {
                return BufferSize;
            }
        }

        bool UseEvents = false;

        public bool p_UseEvents
        {
            set
            {
                UseEvents = value;
            }
            get
            {
                return UseEvents;
            }
        }

        public c_CircularBuffer()
        {
        }

        ~c_CircularBuffer()
        {

        }

        public bool WriteData(ref byte[] TempData)
        {
            return WriteData(ref TempData, 0, (uint)TempData.Length);
        }

        public bool WriteData(ref byte[] TempData, uint Offset, uint Counter)
        {
            if (GetFIFOCounterFree() < Counter) return false;
            if ((Offset + Counter) > TempData.Length) return false;

            if ((WritePointer + Counter) < BufferSize)
            {
                Array.Copy(TempData, Offset, Data, WritePointer, Counter);
                WritePointer += Counter;
            }
            else
            {
                uint Size1 = BufferSize - WritePointer;
                uint Size2 = Counter - Size1;

                Array.Copy(TempData, Offset, Data, WritePointer, Size1);
                Array.Copy(TempData, Offset + Size1, Data, 0, Size2);

                WritePointer = Size2;
            }

            WriteCounter += Counter;

            if (UseEvents)
            {
                WriteExecuted.Set();
            }
            return true;
        }

        public bool ReadData(ref byte[] TempData)
        {
            return ReadData(ref TempData, 0, (uint)TempData.Length);

        }

        public bool ReadData(ref byte[] TempData, uint Offset, uint Counter)
        {
            if (GetFIFOCounter() < Counter) return false;
            if ((Offset + Counter) > TempData.Length) return false;

            if ((ReadPointer + Counter) < BufferSize)
            {
                Array.Copy(Data, ReadPointer, TempData, Offset, Counter);
                ReadPointer += Counter;
            }
            else
            {
                uint Size1 = BufferSize - ReadPointer;
                uint Size2 = Counter - Size1;

                Array.Copy(Data, ReadPointer, TempData, Offset, Size1);
                Array.Copy(Data, 0, TempData, Offset + Size1, Size2);

                ReadPointer = Size2;
            }

            ReadCounter += Counter;

            if (UseEvents)
            {
                ReadExecuted.Set();
            }
            return true;
        }


        public uint GetFIFOCounter()
        {            
            return (uint)(WriteCounter - ReadCounter);
        }

        public uint GetFIFOCounterWithWait()
        {
            if (UseEvents&&(GetFIFOCounter()==0))
            {
                WaitWrite();
            }
            return GetFIFOCounter();
        }


        public uint GetFIFOCounterFree()
        {
            return BufferSize - GetFIFOCounter();
        }

        public uint GetFIFOCounterFreeWithWait()
        {
            if (UseEvents && (GetFIFOCounterFree()==0))
            {
                WaitRead();
            }
            return GetFIFOCounterFree();
        }

        bool AbortWaitEvent = false;
        public void AbortWait(bool SetAbort)
        {
            AbortWaitEvent = SetAbort;
            if (SetAbort)
            {                
                WriteExecuted.Set();
                ReadExecuted.Set();
            }
        }        

        public void WaitRead()
        {
            if (AbortWaitEvent) return;
            ReadExecuted.WaitOne();
        }

        public void WaitWrite()
        {
            if (AbortWaitEvent) return;
            WriteExecuted.WaitOne();
        }

        public void FlushBuffer()
        {
            ReadCounter =
            WriteCounter = 0;
            ReadPointer =
            WritePointer = 0;
            ReadExecuted.Reset();
            WriteExecuted.Reset();
        }

    }
}
