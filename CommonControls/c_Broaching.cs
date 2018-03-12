using System;
using System.IO;

namespace CommonControls
{
	/// <summary>
	/// Summary description for Broaching.
	/// </summary>
	public class c_Broaching
	{
        public byte[] BroachingData;
        c_ErrorDataWork ErrorWork = c_ErrorDataWork.Instance;		
		BroachingAttributesStruct BroachingAttributesInternal;
		
		public int	NormalBroachingSize=12*1024,                  
					BroachingAttributesSize=128,
					BroachingAttributesBegin ;

        public DateTime BroachingCompilationDate = DateTime.Now;

		public struct BroachingAttributesStruct
		{
			public string CompanyName;
			public string DeviceName;
			public string SerialNumber;			
			public string CPUName;
			public uint CPUClockRate;
			public uint FirmwareVersion;
			public char DeviceRevision;
			public string Comment;
			public ushort CRC;
			public bool Initialized;
		}

		public c_Broaching(int NewBroachingSize)
		{
            if (NewBroachingSize != 0) NormalBroachingSize = NewBroachingSize;
            BroachingAttributesBegin = NormalBroachingSize - BroachingAttributesSize;
			//
			// TODO: Add constructor logic here
			//
			BroachingAttributesInternal = new BroachingAttributesStruct();
			BroachingAttributesInternal.Initialized=false;
            BroachingData = new byte[NormalBroachingSize];
		}		

		public void LoadBroaching(string FileName)
		{
            FileInfo FileInfoThis = new FileInfo(FileName);

            BroachingCompilationDate = FileInfoThis.LastWriteTime;
			
			FileStream BroachingFileStream = File.OpenRead(FileName);							

			BroachingData = new byte[Math.Min(BroachingFileStream.Length,NormalBroachingSize)];				
			
			BroachingFileStream.Read(BroachingData,0,BroachingData.Length);	

			BroachingFileStream.Close();
			
			BroachingAttributesInternal.Initialized=false;
		}	

		public void SaveBroaching(string FileName)
		{
			FileStream BroachingFileStream = File.OpenWrite(FileName);											
			
			BroachingFileStream.Write(BroachingData,0,BroachingData.Length);		
			
			BroachingFileStream.Close();
		}

		public BroachingAttributesStruct BroachingAttributes
		{
			get
			{
				if (!BroachingAttributesInternal.Initialized)
				{				
					int Counter = BroachingAttributesBegin;	
				
					BroachingAttributesInternal.CompanyName=ReadDataBroachingArray(ref Counter, 16);
					BroachingAttributesInternal.DeviceName=ReadDataBroachingArray(ref Counter, 16);
					BroachingAttributesInternal.SerialNumber=ReadDataBroachingArray(ref Counter, 16);
					BroachingAttributesInternal.CPUName=ReadDataBroachingArray(ref Counter, 16);
					BroachingAttributesInternal.CPUClockRate=ReadDataBroachingArray(ref Counter);
					BroachingAttributesInternal.FirmwareVersion=ReadDataBroachingArray(ref Counter);
					
					BroachingAttributesInternal.DeviceRevision=ReadDataBroachingArray(ref Counter, 1)[0];
					BroachingAttributesInternal.Comment=ReadDataBroachingArray(ref Counter, 53);

					string CRC=ReadDataBroachingArray(ref Counter, 2);
					BroachingAttributesInternal.CRC=(ushort)((byte)CRC[0]|((byte)CRC[1])<<8);
					BroachingAttributesInternal.Initialized=true;
				}

				return BroachingAttributesInternal;
			}
			set
			{
				int Counter = BroachingAttributesBegin;	
				BroachingAttributesInternal=value;
				
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.CompanyName,16);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.DeviceName,16);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.SerialNumber,16);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.CPUName,16);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.CPUClockRate);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.FirmwareVersion);

				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.DeviceRevision.ToString(),1);
				WriteDataBroachingArray(ref Counter, BroachingAttributesInternal.Comment,53);
				char CRC1=(char)(BroachingAttributesInternal.CRC&0xff);
				char CRC2=(char)(BroachingAttributesInternal.CRC&0xff);
				
				WriteDataBroachingArray(ref Counter, CRC1.ToString()+CRC2.ToString(),2);
				BroachingAttributesInternal.Initialized=true;
			}
		}

		uint ReadDataBroachingArray(ref int Counter)
		{
			byte[] ReturnData = new byte[4];
			
			for (int i=Counter, Counter0=0;Counter0<ReturnData.Length;i++,Counter0++)
			{
				ReturnData[Counter0]=BroachingData[i];
			}			
			Counter+=4;

			uint ReturnValue=(uint)(ReturnData[0]<<0|ReturnData[1]<<8|ReturnData[2]<<16|ReturnData[3]<<24);
			return ReturnValue;
		}

		string ReadDataBroachingArray(ref int Counter, int Number)
		{
			string ReturnData = "";
			for (int i=Counter;i<Counter+Number;i++)
			{
				ReturnData+=(char)BroachingData[i];
			}
			Counter+=Number;
			return ReturnData;
		}	
	
		void WriteDataBroachingArray(ref int Counter, uint ValueUnt)
		{				
			for (int i=Counter, Offset=0;i<Counter+4;i++, Offset+=8)
			{
				BroachingData[i]=(byte)((ValueUnt>>Offset)&0xff);
			}			
			Counter+=4;		
		}

		void WriteDataBroachingArray(ref int Counter, string ValueString, int Number)
		{			
			for (int i=Counter, Length=0;i<Counter+Number;i++, Length++)
			{
				if (Length<ValueString.Length)
					BroachingData[i]=(byte)ValueString[Length];
				else
					BroachingData[i]=0;
			}
			Counter+=Number;			
		}		
	}
}
