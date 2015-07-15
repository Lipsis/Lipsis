using System;

namespace Lipsis.Core {
    public struct ArithmeticOperand {
        private object p_Value;
        private bool p_IsInteger, p_IsDecimal, p_IsScope, p_IsNegative;

        public ArithmeticOperand(object value) : this(value, false) { }
        public ArithmeticOperand(object value, bool isNegative) {
            p_Value = value;
            p_IsInteger = false;
            p_IsDecimal = false;
            p_IsScope = false;
            p_IsNegative = isNegative;

            if (value is ArithmeticScope) {
                p_IsScope = true;
                return;
            }

            p_IsDecimal = (value is float || value is double);
            if (!p_IsDecimal) {
                p_IsInteger = !(value is char);

                //invalid?
                if (!p_IsInteger) {
                    throw new Exception("Value is not a valid type for an operand");
                }
            }
        }

        public object Value { get { return p_Value; } }
        public bool IsInteger { get { return p_IsInteger; } }
        public bool IsDecimal { get { return p_IsDecimal; } }
        public bool IsSubstitution { get { return !p_IsInteger && !p_IsDecimal && !p_IsScope; } }
        public bool IsScope { get { return p_IsScope; } }
        public bool IsNumeric { get { return p_IsInteger || p_IsDecimal; } }
        public bool IsNegative { get { return p_IsNegative; } }

        public static implicit operator ArithmeticOperand(ArithmeticScope value) { return new ArithmeticOperand(value); }
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
    }
}