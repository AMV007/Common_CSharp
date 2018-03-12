using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonControls.Display
{
    public class DigitalControlsMathClass
    {
        public enum FFTWindowTypeEnum
        {
            Rectangular = 0,
            Hanning,
            Hamming,
            Blackman_Harris,
            Exact_Blackman,
            Blackman,
            Flat_Top,
            Term_4_Blackman_Harris,
            Term_7_Blackman_Harris,
            Low_Sidelobe
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowConst
        {
            double enbw;
            double coherentgain;
        }

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _Calibrate(double[,] Values, double[] Scale, double[] Offset, int BeginPosition,
                                int EndPosition, int Dimension0, int Dimension1);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _RemoveDC(double [,]Values, double [] Min, double [] Max, int BeginPosition, 
								int EndPosition, int Dimension0, int Dimension1);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _RemoveDC_All(double[,] Values, int Dimension0, int Dimension1);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _ConvertDimensionalArray(ushort[] DataIn, double [,]DataOut, int Dimension0, int Dimension1, 
                                                                                    int BeginDim1, int EndDim1);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern double _FindPadding(double min, double max);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _FindMinMax(double [,] Values, double [] Min, double [] Max, int ValuesDimension0, 
								 int ValuesDimension1, int MinMaxDimension, bool [] ChannelEnabled);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _CopyArrayDouble(double[,] ArrayDest, double[,] ArraySour, int Dimension0,
                                                    int DestDimension1, int SourceDimension1, 
                                                    int BeginPositionDestDim1, int EndPositionDestDim1);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern void _FindAccumulateResult(double [,]Values, int ValuesDimension0, int ValuesDimension1, 
											double [,] Accumulator, ref uint AccumulateCounter);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern double _mod(double Value1, double Value2);


        [DllImport("DigitalControlsMathapi.dll")]
        public static extern int _ACDCEstimate(double[,] Values, int ValuesDimension0, int ValuesChannel, 
                                                    ref double ACComponent, ref double DCComponent);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern int _CalcSygnParam(double[,] Values, int ValuesDimension0, int ValuesChannel, 
                                            ref double SNR, ref double THD, ref double SINAD, ref double SFDR, ref double ENOB,
                                            double PeakFreq, double SampleFreq, ref WindowConst WC);

        [DllImport("DigitalControlsMathapi.dll")]
        public static extern int _DoFFT(double[,] Values, int ValuesDimension1, int ValuesChannel, FFTWindowTypeEnum win, double freq,
                                        ref double PeakSygnalFreq, ref double pwr, ref double SNR, ref double THD, ref double SINAD,
                                        ref double SFDR, ref double ENOB);





        public void Calibrate(double[,] Values, double[] Scale, double[] Offset, int BeginPosition,
                                int EndPosition)
        {
            _Calibrate(Values, Scale, Offset, BeginPosition, EndPosition, Values.GetLength(0), Values.GetLength(1));
        }
        
        public void RemoveDC(double[,] Values, double[] Min, double[] Max, int BeginPosition,
                                int EndPosition)
        {
            _RemoveDC(Values, Min, Max, BeginPosition, EndPosition, Values.GetLength(0), Values.GetLength(1));
        }

        
        public void RemoveDC_All(double[,] Values)
        {
            _RemoveDC_All(Values, Values.GetLength(0), Values.GetLength(1));
        }


        public void ConvertDimensionalArray(ushort [] DataIn, double[,] DataOut)
        {
            _ConvertDimensionalArray(DataIn, DataOut, DataOut.GetLength(0), DataOut.GetLength(1), 0, DataOut.GetLength(1));
        }

        
        public double FindPadding(double min, double max)
        {
            return _FindPadding(min, max);
        }

        
        public void FindMinMax(double[,] Values, double[] Min, double[] Max, bool[] ChannelEnabled)
        {
            _FindMinMax(Values, Min, Max, Values.GetLength(0), Values.GetLength(1), Min.Length, ChannelEnabled);
        }

        
        public void CopyArrayDouble(double[,] ArrayDest, double[,] ArraySour)
        {
            _CopyArrayDouble(ArrayDest, ArraySour, ArrayDest.GetLength(0), ArrayDest.GetLength(1), ArraySour.GetLength(1), 0, ArrayDest.GetLength(1));
        }

        public void CopyArrayDouble(double[,] ArrayDest, double[,] ArraySour, int BeginPositionDestDim1, int EndPositionDestDim1)
        {
            _CopyArrayDouble(ArrayDest, ArraySour, ArrayDest.GetLength(0), ArrayDest.GetLength(1), ArraySour.GetLength(1), BeginPositionDestDim1, EndPositionDestDim1);
        }

        
        public void FindAccumulateResult(double[,] Values, double[,] Accumulator, ref uint AccumulateCounter)
        {
            _FindAccumulateResult(Values, Values.GetLength(0), Values.GetLength(1), Accumulator,ref AccumulateCounter);
        }

        
        public double mod(double Value1, double Value2)
        {
            return _mod(Value1, Value2);
        }


        
        public int ACDCEstimate([In, Out]double[,] Values, int ValuesChannel,
                                                    ref double ACComponent, ref double DCComponent)
        {
            return _ACDCEstimate(Values, Values.GetLength(1), ValuesChannel, ref ACComponent, ref DCComponent);
        }


        public int CalcSygnParam(double[,] Values, int ValuesChannel,
                                            ref double SNR, ref double THD, ref double SINAD, ref double SFDR, ref double ENOB,
                                            double PeakFreq, double SampleFreq,
                                            ref WindowConst WC)
        {
            return _CalcSygnParam(Values, Values.GetLength(1), ValuesChannel,
                                            ref SNR, ref THD, ref SINAD, ref SFDR, ref ENOB, PeakFreq, SampleFreq,
                                            ref WC); 
        }

        
        public int DoFFT(double[,] Values, int ValuesChannel, FFTWindowTypeEnum win, double freq,
                                        ref double PeakSygnalFreq, ref double pwr, ref double SNR, ref double THD, ref double SINAD,
                                        ref double SFDR, ref double ENOB)
        {
            return _DoFFT(Values, Values.GetLength(1), ValuesChannel, win, freq,
                                        ref PeakSygnalFreq, ref pwr, ref SNR, ref THD, ref SINAD, ref SFDR, ref ENOB);
        }
         
    }
}
