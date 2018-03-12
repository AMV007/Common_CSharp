using System;
using System.Security;


namespace CommonControls.CommonMethods.LongIO
{
    class Path
    {
        private static readonly int MaxDirectoryLength = 0xffff;
        public static readonly char AltDirectorySeparatorChar = '/';
        public static readonly char DirectorySeparatorChar = '\\';
        internal const string DirectorySeparatorCharAsString = @"\";
        public static readonly char VolumeSeparatorChar = ':';
        internal static readonly int MaxPath = 0xffff;

        internal static readonly char[] TrimEndChars = new char[] { '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', '\x00a0' };

        private static readonly char[] InvalidFileNameChars = new char[] { 
            '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
            '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
            '\x001c', '\x001d', '\x001e', '\x001f', ':', '*', '?', '\\', '/'
         };
        [Obsolete("Please use GetInvalidPathChars or GetInvalidFileNameChars instead.")]
        public static readonly char[] InvalidPathChars = new char[] { 
            '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
            '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
            '\x001c', '\x001d', '\x001e', '\x001f'
         };
        private static readonly char[] InvalidPathCharsWithAdditionalChecks = new char[] { 
            '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
            '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
            '\x001c', '\x001d', '\x001e', '\x001f', '*', '?'
         };

        private static readonly char[] RealInvalidPathChars = new char[] { 
            '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', 
            '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', 
            '\x001c', '\x001d', '\x001e', '\x001f'
         };
        private static readonly char[] s_Base32Char = new char[] { 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5'
         };

        internal static bool HasIllegalCharacters(string path, bool checkAdditional)
        {
            if (checkAdditional)
            {
                return (path.IndexOfAny(InvalidPathCharsWithAdditionalChecks) >= 0);
            }
            return (path.IndexOfAny(RealInvalidPathChars) >= 0);
        }

        internal static void CheckInvalidPathChars(string path, bool checkAdditional = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (HasIllegalCharacters(path, checkAdditional))
            {
                throw new ArgumentException("Argument_InvalidPathChars");
            }
        }

        public static string GetExtension(string path)
        {
            if (path == null)
            {
                return null;
            }
            CheckInvalidPathChars(path, false);
            int length = path.Length;
            int startIndex = length;
            while (--startIndex >= 0)
            {
                char ch = path[startIndex];
                if (ch == '.')
                {
                    if (startIndex != (length - 1))
                    {
                        return path.Substring(startIndex, length - startIndex);
                    }
                    return string.Empty;
                }
                if (((ch == DirectorySeparatorChar) || (ch == AltDirectorySeparatorChar)) || (ch == VolumeSeparatorChar))
                {
                    break;
                }
            }
            return string.Empty;
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            path = GetFileName(path);
            if (path == null)
            {
                return null;
            }
            int length = path.LastIndexOf('.');
            if (length == -1)
            {
                return path;
            }
            return path.Substring(0, length);
        }

        public static string GetFileName(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path, false);
                int length = path.Length;
                int num2 = length;
                while (--num2 >= 0)
                {
                    char ch = path[num2];
                    if (((ch == DirectorySeparatorChar) || (ch == AltDirectorySeparatorChar)) || (ch == VolumeSeparatorChar))
                    {
                        return path.Substring(num2 + 1, (length - num2) - 1);
                    }
                }
            }
            return path;
        }

        internal static string RemoveLongPathPrefix(string path)
        {
            if (!path.StartsWith(@"\\?\", StringComparison.Ordinal))
            {
                return path;
            }
            if (path.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
            {
                return path.Remove(2, 6);
            }
            return path.Substring(4);
        }

        internal static string GetFullPathInternal(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return path;
        }

        public static string GetFullPath(string path)
        {
            string fullPathInternal = GetFullPathInternal(path);
            //FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, fullPathInternal, false, false);
            return fullPathInternal;
        }

        internal static bool IsDirectorySeparator(char c)
        {
            if (c != DirectorySeparatorChar)
            {
                return (c == AltDirectorySeparatorChar);
            }
            return true;
        }

        internal static int GetRootLength(string path)
        {
            CheckInvalidPathChars(path, false);
            int num = 0;
            int length = path.Length;
            if ((length >= 1) && IsDirectorySeparator(path[0]))
            {
                num = 1;
                if ((length >= 2) && IsDirectorySeparator(path[1]))
                {
                    num = 2;
                    int num3 = 2;
                    while ((num < length) && (((path[num] != DirectorySeparatorChar) && (path[num] != AltDirectorySeparatorChar)) || (--num3 > 0)))
                    {
                        num++;
                    }
                }
                return num;
            }
            if ((length >= 2) && (path[1] == VolumeSeparatorChar))
            {
                num = 2;
                if ((length >= 3) && IsDirectorySeparator(path[2]))
                {
                    num++;
                }
            }
            return num;
        }

        public static string GetDirectoryName(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path, false);
                string str = path;
                if (path.Length > 0)
                {
                    try
                    {
                        string str2 = RemoveLongPathPrefix(path);
                        int length = 0;
                        while (((length < str2.Length) && (str2[length] != '?')) && (str2[length] != '*'))
                        {
                            length++;
                        }
                        if (length > 0)
                        {
                            GetFullPath(str2.Substring(0, length));
                        }
                    }
                    catch (SecurityException)
                    {
                        if (path.IndexOf("~", StringComparison.Ordinal) != -1)
                        {
                            str = path;
                        }
                    }
                    catch (System.IO.PathTooLongException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (System.IO.IOException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                path = str;
                int rootLength = GetRootLength(path);
                if (path.Length > rootLength)
                {
                    int num2 = path.Length;
                    if (num2 == rootLength)
                    {
                        return null;
                    }
                    while (((num2 > rootLength) && (path[--num2] != DirectorySeparatorChar)) && (path[num2] != AltDirectorySeparatorChar))
                    {
                    }
                    return path.Substring(0, num2);
                }
            }
            return null;
        }      
    }
}
