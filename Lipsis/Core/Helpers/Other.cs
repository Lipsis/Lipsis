using System;
using System.Diagnostics;

namespace Lipsis.Core {
    public static unsafe partial class Helpers {


        public static string ChangeNumericalBase(string value, int sourceBase, int destBase) {
            #region valid?
            if (sourceBase < 1 || sourceBase > 63) {
                throw new Exception("Source base must be between 1 and 63");
            }
            if (destBase < 1 || sourceBase > 63) {
                throw new Exception("Destination base must be between 1 and 63");
            }

            int strLength = value.Length;
            if (strLength == 0) { return "0"; }
            #endregion

            //same bases?
            if (sourceBase == destBase) { return value; }

            //charset string to contain all characters from base 1 to 63
            const string charSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";            

            //convert the value to an unmanaged pointer so we can quickly access
            //the characters from the string.
            //we also reverse the value so it the low order unit is at the beginning
            //(so we can easily go through it)
            char[] valueCharArray = value.ToCharArray();
            Array.Reverse(valueCharArray);
            fixed (char* fixedPtr = valueCharArray) {
                char* ptr = fixedPtr;
                char* ptrEnd = ptr + strLength;

                #region convert to base 10?
                if (sourceBase != 10) { 
                    //define the return value
                    ulong buffer = 0;
                    ulong currentMultiplier = 1;

                    //iterate through the value backwards
                    //and add it's numerical value to the buffer
                    while (ptr < ptrEnd) {
                        byte unit = (byte)*ptr++;

                        //convert the unit to upper case (if it's "f" and it's only base 16 conversion)
                        if (destBase < 17 && unit >= 'a' && unit <= 'z') {
                            unit = (byte)((unit - 'a') + 'A');
                        }

                        //convert the unit to a single source base unit
                        sbyte sbUnit = (sbyte)charSet.IndexOf((char)unit);
                        if (sbUnit == -1 || sbUnit > sourceBase) {
                            throw new Exception("Invalid base " + sourceBase + " string \"" + value + "\"");
                        }

                        //add the single unit to the buffer
                        buffer += (currentMultiplier * (byte)sbUnit);
                        currentMultiplier *= (uint)sourceBase;
                    }

                    //convert from base 10 to the destination base
                    if (destBase == 10) { return buffer.ToString(); }
                    return ChangeNumericalBase(
                        buffer.ToString(),
                        10,
                        destBase);

                }
                #endregion

                #region convert from base 10?
                else if (destBase != 10) {
                    /*
                        this portion is called when the source base is 10.
                    */

                    //convert the value to a integer so we can perform math
                    //operations on it.
                    ulong intValue = Convert.ToUInt64(value);
                    if (intValue == 0) { return "0"; }

                    //calculate how long the return string would be
                    int len = 1;
                    ulong intValueBuffer = intValue;
                    while ((intValueBuffer /= (uint)destBase) != 0) { len++; }

                    //allocate a block of memory for the return buffer
                    char[] returnBuffer = new char[len];
                    int returnBufferIndex = 0;

                    //
                    while (intValue != 0) {
                        sbyte unit = (sbyte)(intValue % (uint)destBase);
                        intValue /= (uint)destBase;
                        
                        //get the character associated with the digit
                        returnBuffer[returnBufferIndex++] = charSet[unit];
                    }

                    Array.Reverse(returnBuffer);
                    return new string(returnBuffer);
                    //reverse the buffer so that the highest order unit is
                    //at the beginning of the value
                    //char[] bufferChar = buffer.ToCharArray();
                    //Array.Reverse(bufferChar);
                    //return new string(bufferChar);
                }
                #endregion
            }

            return value;            
        }



        public static double Time(TimingCallback callback) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            callback();
            watch.Stop();
            return (watch.ElapsedTicks * 1.0f / Stopwatch.Frequency) * 1000;
        }
        public static double Time<T>(T state, TimingCallbackWithState<T> callback) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            callback(state);
            watch.Stop();
            return (watch.ElapsedTicks * 1.0f / Stopwatch.Frequency) * 1000;
        }

        public delegate void TimingCallback();
        public delegate void TimingCallbackWithState<T>(T state);
    }
}