using System;

namespace Lipsis.Core {
    public static partial class Helpers {
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