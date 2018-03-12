using System;
using System.Collections.Generic;
using System.Text;

namespace CommonControls.CommonMethods.MyEncoding
{
    public class c_base64
    {
        private const string base64_chars =
             "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
             "abcdefghijklmnopqrstuvwxyz" +
             "0123456789+/";

        private static bool is_base64(char c)
        {
            return (char.IsLetterOrDigit(c) || (c == '+') || (c == '/'));
        }

        // закодировать
        public static string base64_encode(string bytes_to_encode)
        {
            string ret = "";
            int i = 0;
            int j = 0;
            char[] char_array_3 = new char[3],
                  char_array_4 = new char[4];

            int in_len = bytes_to_encode.Length;
            int bytesToencodeIndex = 0;

            while (in_len-- > 0)
            {
                char_array_3[i++] = bytes_to_encode[bytesToencodeIndex++];
                if (i == 3)
                {
                    char_array_4[0] = (char)((char_array_3[0] & 0xfc) >> 2);
                    char_array_4[1] = (char)(((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4));
                    char_array_4[2] = (char)(((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6));
                    char_array_4[3] = (char)(char_array_3[2] & 0x3f);

                    for (i = 0; (i < 4); i++)
                        ret += base64_chars[char_array_4[i]];
                    i = 0;
                }
            }

            if (i != 0)
            {
                for (j = i; j < 3; j++)
                    char_array_3[j] = '\0';

                char_array_4[0] = (char)((char_array_3[0] & 0xfc) >> 2);
                char_array_4[1] = (char)(((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4));
                char_array_4[2] = (char)(((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6));
                char_array_4[3] = (char)(char_array_3[2] & 0x3f);

                for (j = 0; (j < i + 1); j++)
                    ret += base64_chars[char_array_4[j]];

                while ((i++ < 3))
                    ret += '=';

            }

            return ret;
        }

        // раскдировать
        public static string decode(string encoded_string)
        {
            int in_len = encoded_string.Length;
            int i = 0;
            int j = 0;
            int in_ = 0;
            char[] char_array_4 = new char[4],
                char_array_3 = new char[3];
            string ret = "";

            while ((in_len-- > 0)
                      && (encoded_string[in_] != '=')
                      && is_base64(encoded_string[in_]))
            {
                char_array_4[i++] = encoded_string[in_]; in_++;
                if (i == 4)
                {
                    for (i = 0; i < 4; i++)
                        char_array_4[i] = (char)base64_chars.IndexOf(char_array_4[i]);

                    char_array_3[0] = (char)((char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4));
                    char_array_3[1] = (char)(((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2));
                    char_array_3[2] = (char)(((char_array_4[2] & 0x3) << 6) + char_array_4[3]);

                    for (i = 0; (i < 3); i++)
                        ret += char_array_3[i];
                    i = 0;
                }
            }

            if (i != 0)
            {
                for (j = i; j < 4; j++)
                    char_array_4[j] = (char)0;

                for (j = 0; j < 4; j++)
                    char_array_4[j] = (char)base64_chars.IndexOf(char_array_4[j]);

                char_array_3[0] = (char)((char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4));
                char_array_3[1] = (char)(((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2));
                char_array_3[2] = (char)(((char_array_4[2] & 0x3) << 6) + char_array_4[3]);

                for (j = 0; (j < i - 1); j++) ret += char_array_3[j];
            }

            return ret;
        }
    }
}
