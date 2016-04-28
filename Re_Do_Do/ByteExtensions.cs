using System;
using Microsoft.SPOT;

namespace Re_Do_Do
{
    static class ByteExtensions
    {
        private static char[] _hexCharacterTable = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        static readonly string hex = "0123456789ABCDEF";

#if MF_FRAMEWORK_VERSION_V4_1
    public static string ToHexString(byte[] array, string delimiter = "-")
#else
        public static string ToHexString(this byte[] array, string delimiter = "-")
#endif
        {
            if (array.Length > 0)
            {
                // it's faster to concatenate inside a char array than to
                // use string concatenation
                char[] delimeterArray = delimiter.ToCharArray();
                char[] chars = new char[array.Length * 2 + delimeterArray.Length * (array.Length - 1)];

                int j = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    chars[j++] = (char)_hexCharacterTable[(array[i] & 0xF0) >> 4];
                    chars[j++] = (char)_hexCharacterTable[array[i] & 0x0F];

                    if (i != array.Length - 1)
                    {
                        foreach (char c in delimeterArray)
                        {
                            chars[j++] = c;
                        }

                    }
                }

                return new string(chars);
            }
            else
            {
                return string.Empty;
            }
        }

        public static byte[] ToHexByte(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = ToByteFromHex(hex.Substring(i, 2));
            return bytes;
            
        }

        public static byte ToByteFromHex(string hexNumber)
        {
            if (hexNumber.Length > 2)
                throw new InvalidCastException("The number to convert is too large for a byte, or not hexadecimal");

            string number = hexNumber.ToUpper();
            int value = 0;

            if (number.Length == 1)
            {
                value = hex.IndexOf(number);
            }
            else
            {
                value = hex.IndexOf(number[0]) << 4;
                value = value + hex.IndexOf(number[1]);
            }

            return (byte)value;
        }
    }
}
