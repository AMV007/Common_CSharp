using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CommonControls
{
    public class c_CRC
    {
        public static int GetCRC(string File)
        {
            StreamReader fsreader = new StreamReader(File);
            BinaryReader freader = new BinaryReader(fsreader.BaseStream);
            long Position=0;
            byte[] TempData;
            int CRCData=0;

            do
            {
                TempData = freader.ReadBytes(1024);
                GetCRC(ref TempData, ref Position, ref CRCData);
            } while (TempData.Length != 0);

            freader.Close();
            fsreader.Close();

            return CRCData;
        }

        public static int GetCRC(ref byte[] Data)
        {
            int Result = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                Result += (Data[i] << (i & 0xF));
            }

            return Result;
        }

        public static void GetCRC(ref byte[] Data, ref long Offset, ref int CRCData)
        {            
            for (int i = 0; i < Data.Length; i++)
            {
                CRCData += (Data[i] << (int)(Offset & 0xF));
                Offset++;
            }
        }

        public c_CRC()
        {
        }
    }
}
