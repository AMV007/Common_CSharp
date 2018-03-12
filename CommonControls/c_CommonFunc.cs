using System;
using System.Collections.Generic;
using System.Text;

namespace CommonControls
{
    public class c_CommonFunc
    {

        public static string FormatSize(long Size)
        {
            return FormatSize(Size, 1024, 3);
        }

        public static string FormatSize(long Size, int KByteSize)
        {
            return FormatSize(Size, KByteSize, 3);
        }
        public static string FormatSize(long Size, int KByteSize, int NumDigitsAfterPset)
        {   
            string Result = "";
            if (Size == 0)
            {
                Result = "0  ·";
            }
            else
            {
                float ShowSize = Size;
                if (Size < (KByteSize * KByteSize))
                {
                    ShowSize /= (KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + "  ·";
                }
                else if (Size < (KByteSize * KByteSize * KByteSize))
                {
                    ShowSize /= (KByteSize * KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + " Ã·";
                }
                else
                {
                    ShowSize /= (KByteSize * KByteSize * KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + " √·";
                }
            }

            return Result;
        }

        public static string FormatSize(float Size)
        {
            return FormatSize(Size, 1024,3);
        }

        public static string FormatSize(float Size, int KByteSize)
        {
            return FormatSize(Size, KByteSize, 3);
        }
        public static string FormatSize(float Size, int KByteSize, int NumDigitsAfterPset)
        {            
            string Result = "";
            if (Size == 0)
            {
                Result = "0  ·";
            }
            else
            {
                float ShowSize = Size;
                if (Size < (KByteSize * KByteSize))
                {
                    ShowSize /= (KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + "  ·";
                }
                else if (Size < (KByteSize * KByteSize * KByteSize))
                {
                    ShowSize /= (KByteSize * KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + " Ã·";
                }
                else
                {
                    ShowSize /= (KByteSize * KByteSize * KByteSize);
                    Result = ShowSize.ToString("f" + NumDigitsAfterPset.ToString()) + " √·";
                }
            }

            return Result;
        }
       
        public static string GetString(byte[] InputString)
        {
            string result = "";
            for (int i = 0; i < InputString.Length; i++)
            {
                if (InputString[i] == 0) break;
                result += (char)InputString[i];
            }

            return result;
        }

        public static string ApplicationPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        public static int MinMax(int Value, int Min, int Max)
        {
            return Math.Max(Math.Min(Value, Max), Min);
        }

        public static float MinMax(float Value, float Min, float Max)
        {
            return Math.Max(Math.Min(Value, Max), Min);
        }

        public static double MinMax(double Value, double Min, double Max)
        {
            return Math.Max(Math.Min(Value, Max), Min);
        }
    }
}
