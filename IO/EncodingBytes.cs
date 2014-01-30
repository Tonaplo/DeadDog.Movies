using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IO
{
    public static class EncodingBytes
    {
        public static byte GetByte(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            else if (encoding == Encoding.ASCII)
                return 1;
            else if (encoding == Encoding.UTF8)
                return 2;
            else if (encoding == Encoding.Unicode)
                return 3;
            else
                throw new ArgumentException("encoding", encoding.EncodingName + " is not supported.");
        }

        public static Encoding GetEncoding(byte value)
        {
            if (value == 1)
                return Encoding.ASCII;
            else if (value == 2)
                return Encoding.UTF8;
            else if (value == 3)
                return Encoding.Unicode;
            else
                throw new ArgumentException("value", value + " does not refer to a supported encoding.");
        }
    }
}
