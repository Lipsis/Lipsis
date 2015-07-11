using System;
using System.Runtime.InteropServices;

namespace Lipsis.Core {
    public static partial class Helpers {
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