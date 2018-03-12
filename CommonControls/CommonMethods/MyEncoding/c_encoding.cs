using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonControls.CommonMethods.MyEncoding
{
    public class c_encoding
    {
        public enum e_Encoding_Type
        {
            t_7bit,
            t_8bit,
            t_binary,
            t_quoted_printable,
            t_base64,
            t_unknown
        }

        public static e_Encoding_Type GetEncoding(string DetectEncoding, bool DetectMIME)
        {
            if (string.IsNullOrEmpty(DetectEncoding)) return e_Encoding_Type.t_unknown;

            string DetectEncodingLower = DetectEncoding.ToLower();
            switch (DetectEncodingLower)
            {
                case "7bit": return e_Encoding_Type.t_7bit;
                case "8bit": return e_Encoding_Type.t_8bit;                
                case "binary": return e_Encoding_Type.t_binary;
                case "q":
                case "quoted-printable": return e_Encoding_Type.t_quoted_printable;
                case "b":
                case "base64": return e_Encoding_Type.t_base64;
                default:
                    if (DetectMIME)
                    {
                        Match match = Regex.Match(DetectEncoding, @"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
                        if (match.Success)
                        {
                            string encoding = match.Groups["encoding"].Value.ToLower();
                            return GetEncoding(encoding, false);
                        }
                    }
                    return e_Encoding_Type.t_unknown;
            }     
        }

        public static string GetCodePageMIME(string mimeString)
        {
            if (string.IsNullOrEmpty(mimeString)) return "";
            Match match = Regex.Match(mimeString, @"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
            if (match.Success)
            {
                return match.Groups["charset"].Value;                               
            }
            return "";
        }

        public static e_Encoding_Type GetEncodingMIME(string mimeString)
        {
            if (string.IsNullOrEmpty(mimeString)) return e_Encoding_Type.t_unknown;

            Match match = Regex.Match(mimeString, @"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
            if (match.Success)
            {                
                return GetEncoding(match.Groups["encoding"].Value.ToLower(), false);
            }
            return e_Encoding_Type.t_unknown;
        }

        public static string DecodeMIME(string mimeString)
        {
            return DecodeMIME(mimeString, e_Encoding_Type.t_unknown, "");
        }

        public static string DecodeMIME(string mimeString, e_Encoding_Type e_Type)
        {
            return DecodeMIME(mimeString, e_Type,"");
        }
        
        public static string DecodeMIME(string mimeString, e_Encoding_Type e_Type, string Charset)
        {
            // Example: mimeString = "=?Windows-1252?Q?Registered_Member_News=3A_WPC09_to_feature_Windows_7=2C_?==?Windows-1252?Q?_Office=2C_Exchange=2C_more=85?="
            // In this example two Q-encoded strings are defined!
            string encodedString = mimeString;
            string decodedString = "";
            while (encodedString.Length > 0)
            {
                Match match = Regex.Match(encodedString, @"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
                if (match.Success)
                {
                    string CurrCharset = match.Groups["charset"].Value;
                    if(string.IsNullOrEmpty(CurrCharset))
                    {
                        CurrCharset = Charset;                    
                    }

                    string value = match.Groups["value"].Value;
                    e_Encoding_Type e_TypeCurrent = GetEncoding(match.Groups["encoding"].Value.ToLower(), false);
                    if (e_TypeCurrent == e_Encoding_Type.t_unknown)
                    {
                        e_TypeCurrent = e_Type;
                    }

                    decodedString += Decode(value, e_TypeCurrent, CurrCharset);                   

                    // When multiple entries, subtract the currently decoded part                                        
                    encodedString = encodedString.Substring(match.Index + match.Length);
                }
                else
                {
                    // Unable to decode (not mime encoded)
                    if (!string.IsNullOrEmpty(decodedString))
                    {
                        return decodedString + encodedString;
                    }
                    else return mimeString;
                }
            }

            // Successfull
            return decodedString;
        }
        


        public static string Decode(string Source, e_Encoding_Type this_Encoding, string ThisCharset)
        {
            return DecodeCodePage(DecodeEncoding(Source, this_Encoding), ThisCharset);
        }

        public static string Decode(byte [] Source, e_Encoding_Type this_Encoding, string ThisCharset)
        {
            return DecodeCodePage(DecodeEncoding(Source, this_Encoding), ThisCharset);
        }

        public static string DecodeEncoding(string Source, e_Encoding_Type this_Encoding)
        {
            string res = "";

            switch (this_Encoding)
            {
                case e_Encoding_Type.t_base64:                    
                    try
                    {
                        // not all strings can be decoded
                        // for example W05vcnRvbiBBbnRpU3BhbV0gS3JlbWxpbi5ydTog0JXQttC10LTQvdC10LLQvdCw0Y8g0YDQsNGB0YHRi9C70LrQsA
                        // inpossible, but C++ working with it well ???
                        byte[] Tempval = Convert.FromBase64String(Source);
                        for (int i = 0; i < Tempval.Length; i++)
                            res += (char)Tempval[i];
                    }
                    catch
                    {
                        res = c_base64.decode(Source);
                    }
                    break;
                    
                case e_Encoding_Type.t_quoted_printable:
                    // not realized                               
                    //parse looking for =XX where XX is hexadecimal
                    Regex re = new Regex(
                        "(\\=([0-9A-F][0-9A-F]))",
                        RegexOptions.IgnoreCase
                    );
                    res = re.Replace(Source, new MatchEvaluator(HexDecoderEvaluator));
                    res = res.Replace('_', ' ');                  
                    break;
                        
                case e_Encoding_Type.t_7bit:
                case e_Encoding_Type.t_8bit:
                case e_Encoding_Type.t_binary:
                default:
                    return Source;                
            }
            return res;
        }
       
        public static string DecodeEncoding(byte[] Source, e_Encoding_Type this_Encoding)
        {
            if (Source==null||Source.Length == 0) return "";

            string res = "";

            switch (this_Encoding)
            {
                case e_Encoding_Type.t_base64:
                    try
                    {
                        // not all strings can be decoded
                        // for example W05vcnRvbiBBbnRpU3BhbV0gS3JlbWxpbi5ydTog0JXQttC10LTQvdC10LLQvdCw0Y8g0YDQsNGB0YHRi9C70LrQsA
                        // inpossible, but C++ working with it well ???
                        char[] TempArray = new char[Source.Length];
                        for (int i = 0; i < Source.Length; i++) TempArray[i] = (char)Source[i];
                        byte[] Tempval = Convert.FromBase64CharArray(TempArray, 0, TempArray.Length);
                        for (int i = 0; i < Tempval.Length; i++)
                            res += (char)Tempval[i];
                    }
                    catch
                    {
                        for (int i = 0; i < Source.Length; i++) res += (char)Source[i];
                        res = c_base64.decode(res);
                    }
                    break;

                case e_Encoding_Type.t_quoted_printable:
                    // not realized                               
                    //parse looking for =XX where XX is hexadecimal
                    Regex re = new Regex(
                        "(\\=([0-9A-F][0-9A-F]))",
                        RegexOptions.IgnoreCase
                    );
                    string SourceStr = "";
                    for (int i = 0; i < Source.Length; i++) SourceStr += (char)Source[i];

                    res = re.Replace(SourceStr, new MatchEvaluator(HexDecoderEvaluator));
                    res = res.Replace('_', ' ');                    
                    break;

                case e_Encoding_Type.t_7bit:
                case e_Encoding_Type.t_8bit:
                case e_Encoding_Type.t_binary:
                default:
                    for (int i = 0; i < Source.Length; i++)
                        res += (char)Source[i];                                        
                    break;
            }
            return res;
        }

        private static string HexDecoderEvaluator(Match m)
        {

            string hex = m.Groups[2].Value;
            int iHex = Convert.ToInt32(hex, 16);

            // Rerutn the string in the charset defined
            byte[] bytes = new byte[1];
            bytes[0] = Convert.ToByte(iHex);
            string res = "";
            for (int i = 0; i < bytes.Length; i++)
                res += (char)bytes[i];

            return res;
            // This will not work properly on "=85" in example string
            //		  char c = (char)iHex;
            //		return c.ToString();
        }

        public static string DecodeCodePage(string CurrString, string CurrCharset)
        {
            if (string.IsNullOrEmpty(CurrCharset)) return CurrString;

            try
            {
                byte[] TempByte = new byte[CurrString.Length];
                for (int i = 0; i < CurrString.Length; i++)
                    TempByte[i] = (byte)CurrString[i];

                return Encoding.GetEncoding(CurrCharset).GetString(TempByte);
            }
            catch
            {
                return CurrString;
            }
        }

        public static string DecodeCodePage(byte [] CurrString, string CurrCharset)
        {
            if (CurrString==null||CurrString.Length == 0) return "";

            try
            {
                return Encoding.GetEncoding(CurrCharset).GetString(CurrString);
            }
            catch
            {
                string res="";
                for (int i = 0; i < CurrString.Length; i++)
                    res += (char)CurrString[i];
                return res;
            }
        }    
    }
}
