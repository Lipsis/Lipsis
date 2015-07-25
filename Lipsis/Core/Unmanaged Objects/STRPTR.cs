using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Lipsis.Core {
    public unsafe struct STRPTR : IDisposable {
        public byte* PTR, PTREND;

        public STRPTR(byte* ptr, byte* ptrEnd) {
            PTR = ptr;
            PTREND = ptrEnd;
        }

        public int Length {
            get {
                if (PTREND == (byte*)0) { return 0; }
                return (int)(PTREND - PTR) + 1;
            }
        }

        public void ToUpper() { 
            byte* ptr = PTR;
            byte* end = PTREND;
            while (ptr <= end) {
                if (*ptr >= 'a' && *ptr <= 'z') {
                    *ptr = (byte)((*ptr - 'a') + 'A');
                }
                ptr++;
            }
        }
        public void ToLower() {
            byte* ptr = PTR;
            byte* end = PTREND;
            while (ptr <= end) {
                if (*ptr >= 'A' && *ptr <= 'Z') {
                    *ptr = (byte)((*ptr - 'A') + 'a');
                }
                ptr++;
            }
        }

        public string GetString() {
            return GetString(Encoding.ASCII);
        }
        public string GetString(Encoding encoder) {
            return Helpers.ReadString(PTR, PTREND, encoder);
        }

        public void Destroy() {
            //already destroyed?
            if (PTR == (byte*)0) { return; }

            Marshal.FreeCoTaskMem((IntPtr)PTR);

            PTR = PTREND = (byte*)0;
        }        
        public void Dispose() { Destroy(); }

        public override string ToString() {
            return
                "[" +
                    Helpers.ChangeNumericalBase(((ulong)PTR).ToString(), 10, 16) + " - " +
                    Helpers.ChangeNumericalBase(((ulong)PTREND).ToString(), 10, 16) +
                    " (" + Length + " bytes)] " +
                "\"" + GetString() + "\"";
        }
        public override bool Equals(object obj) {
            if (!(obj is STRPTR)) { return false; }
            return ((STRPTR)obj) == this;
        }

        public static STRPTR AllocateString(string str) {
            IntPtr alloc = Marshal.StringToCoTaskMemAnsi(str);
            byte* ptr = (byte*)alloc.ToPointer();

            return new STRPTR(ptr, ptr + str.Length - 1);
        }


        public static bool operator ==(STRPTR s1, STRPTR s2) {
            //get the two pointers to compare
            byte* ptr1 = s1.PTR;
            byte* ptr2 = s2.PTR;
            byte* ptrEnd1 = s1.PTREND + 1;
            byte* ptrEnd2 = s2.PTREND + 1;

            //are they the same length?
            if ((int)(ptrEnd1 - ptr1) != (int)(ptrEnd2 - ptr2)) { return false; }

            //perform compare
            while (ptr1 < ptrEnd1) {
                if (*ptr1++ != *ptr2++) { 
                    return false;
                }
            }
            return true;

        }
        public static bool operator !=(STRPTR s1, STRPTR s2) {
            return !(s1 == s2);
        }

        public static implicit operator STRPTR(string val) {
            return AllocateString(val);
        }
        public static implicit operator string(STRPTR ptr) {
            return ptr.GetString();
        }
    }
}