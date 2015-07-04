using System;
using System.Runtime.InteropServices;

namespace Lipsis.Core {
    public static partial class Helpers {

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

        public static unsafe void* GetArrayPointer<T>(T[] array, int structSize) { 
            //allocate a block of memory to store the array data
            byte* ptr = (byte*)Marshal.AllocCoTaskMem(array.Length * structSize).ToPointer();
            byte* ptrSeek = ptr;

            //copy each element of the array into the block of memory allocated
            int length = array.Length;
            for (int c = 0; c < length; c++) { 
                
                //get the object data
                T entry = array[c];
                byte* data;
                int dataLength;
                GetObjectData(entry, out data, out dataLength);

                //write the data to the pointer
                CopyMemory((IntPtr)(ptrSeek), (IntPtr)data, dataLength);
                ptrSeek += dataLength;

                //clean up
                Marshal.FreeCoTaskMem((IntPtr)data);
            }


            //
            return ptr;
        }
        public static unsafe void GetObjectData(object obj, out byte* data, out int length) { 
            //get the size of the object 
            int size = Marshal.SizeOf(obj);

            //allocate a block of memory to copy the object data to
            IntPtr ptr = Marshal.AllocCoTaskMem(size);
            
            //copy the data from the object to the pointer
            Marshal.StructureToPtr(obj, ptr, false);

            data = (byte*)ptr.ToPointer();
            length = size;

        }
    }
}