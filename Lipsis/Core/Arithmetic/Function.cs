using System;
using System.Collections.Generic;

namespace Lipsis.Core {
    public abstract unsafe class ArithmeticFunction {
        private string p_Name;

        public ArithmeticFunction(string name) {
            p_Name = name;

            //name CANNOT be just one character as it will 
            //conflict with substitutes
            int length = name.Length;
            if (length == 1) {
                throw new Exception("Name cannot be a single character because it may conflict with substitutes");
            }

            //blank?
            if(length == 0){
                throw new Exception("Name cannot be blank");
            }

            //alphabetical only
            fixed (char* fixedPtr = name.ToCharArray()) {
                char* ptr = (char*)fixedPtr;
                char* ptrEnd = ptr + length;
                while (ptr < ptrEnd) {
                    byte b = (byte)*ptr;
                    if (!((b >= 'A' && b <= 'Z') ||
                          (b >= 'a' && b <= 'z'))) {
                             throw new Exception("Invalid function name \"" + name + "\". Must contain ONLY alphabetical characters");
                    }
                    ptr++;
                }
            }
        }

        public string Name { get { return p_Name; } }

        public ArithmeticNumeric Call(ArithmeticQueue queue) {
            return OnCall(queue.Calculate());
        }
        public ArithmeticNumeric Call(ArithmeticQueue queue, LinkedList<ArithmeticSubstitute> substitutes) {
            return OnCall(queue.Calculate(substitutes));
        }
        public abstract ArithmeticNumeric Call(LinkedList<ArithmeticSubstitute> substitutes);

        public abstract ArithmeticFunction Create(ArithmeticQueue queue);

        protected abstract ArithmeticNumeric OnCall(ArithmeticNumeric arg);

        public static ArithmeticFunction FromDelegate(string name, OnCallHandler handler) {
            return new arithmeticFunctionDelegation(name, handler);
        }

        public delegate ArithmeticNumeric OnCallHandler(ArithmeticNumeric arg);
        private class arithmeticFunctionDelegation : ArithmeticFunction {
            private ArithmeticQueue p_Queue;
            private OnCallHandler p_Handler;

            public arithmeticFunctionDelegation(string name, OnCallHandler handler) : this(name, handler, null) { }
            private arithmeticFunctionDelegation(string name, OnCallHandler handler, ArithmeticQueue queue) : base(name) {
                p_Queue = queue;
                p_Handler = handler;
            }

            public override ArithmeticNumeric Call(LinkedList<ArithmeticSubstitute> substitutes) {
                return Call(p_Queue, substitutes);
            }

            public override ArithmeticFunction Create(ArithmeticQueue queue) {
                return new arithmeticFunctionDelegation(p_Name, p_Handler, queue);
            }

            protected override ArithmeticNumeric OnCall(ArithmeticNumeric arg) {
                return p_Handler(arg);
            }

            public override string ToString() {
                return p_Name + "(" + p_Queue + ")";
            }
        }
    }
}