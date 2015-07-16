using System;

namespace Lipsis.Core {
    public struct ArithmeticOperand {
        private object p_Value;
        private bool p_IsInteger, p_IsDecimal, p_IsQueue, p_IsNegative;

        public ArithmeticOperand(object value) : this(value, false) { }
        public ArithmeticOperand(object value, bool isNegative) {
            p_Value = value;
            p_IsInteger = false;
            p_IsDecimal = false;
            p_IsQueue = false;
            p_IsNegative = isNegative;

            //queue?
            if (value is ArithmeticQueue) {
                p_IsQueue = true;
                return;
            }

            //substitute?
            if (value is char) { return; }

            //attempt to use the object as a numeric type
            ArithmeticNumeric numeric = (value is ArithmeticNumeric ?
                (ArithmeticNumeric)value :
                new ArithmeticNumeric(value));
            
            p_IsDecimal = numeric.IsDecimal;
            p_IsInteger = !p_IsDecimal;
            p_Value = numeric;
        }

        public object Value { get { return p_Value; } }
        public bool IsInteger { get { return p_IsInteger; } }
        public bool IsDecimal { get { return p_IsDecimal; } }
        public bool IsSubstitution { get { return !p_IsInteger && !p_IsDecimal && !p_IsQueue; } }
        public bool IsQueue { get { return p_IsQueue; } }
        public bool IsNumeric { get { return p_IsInteger || p_IsDecimal; } }
        public bool IsNegative {
            get { return p_IsNegative; }
            set { p_IsNegative = value; }
        }

        public static implicit operator ArithmeticOperand(ArithmeticQueue value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(sbyte value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(byte value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(short value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(ushort value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(int value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(uint value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(long value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(ulong value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(float value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(double value) { return new ArithmeticOperand(value); }
        public static implicit operator ArithmeticOperand(char value) { return new ArithmeticOperand(value); }

        public override string ToString() {
            return p_Value.ToString();
        }
    }
}