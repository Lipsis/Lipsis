using System;
using System.Text;

namespace Lipsis.Core {
    public static partial class Helpers {

        public static unsafe string ReadString(byte* ptr, byte* endPtr, Encoding encoder) {
            //blank string?
            if (endPtr == (byte*)0) { return ""; }

            //read the block of memory which the string is in
            //into a buffer
            int length = (int)(endPtr - ptr) + 1;
            if (length == 0) { return ""; }
            byte[] buffer = new byte[length];
            fixed (byte* locked = buffer) {
                byte* writePtr = locked;
                while (ptr < endPtr + 1) {
                    *writePtr++ = *ptr++;
                }
            }

            //read the block of data as a unicode string of characters
            string str = encoder.GetString(buffer, 0, length);
            
            //clean up
            buffer = null;
            return str;

        }
        
        public static unsafe bool SkipWhitespaces(ref byte* ptr, byte* endPtr) {
            //returns false if the end of stream was NOT hit.
            while (ptr < endPtr) {                
                if (!(
                    *ptr == ' ' ||
                    *ptr == '\t' ||
                    *ptr == '\n' ||
                    *ptr == '\r')) {
                        return false;
                }
                
                ptr++;
            }
            return true;
        }

        public static string FlattenToString<T>(T[] array, string seperator) {
            //create the string to return and buffer the length of the array
            //so we dont have to make calls to the Length property per iteration.
            string buffer = "";
            int length = array.Length;

            //flatten the array into a string with a seperator
            for (int c = 0; c < length; c++) {
                buffer +=
                    array[c].ToString();
                if (c != length - 1) {
                    buffer += seperator;
                }
            }

            return buffer;
        }
    }
}