using System;

namespace Lipsis.Core {

    /*
        Handler for arithmetic functions on an unknown numerical type 
    */
    public struct ArithmeticNumeric {
        private object p_Value;
        private NumericalType p_Type;

        public ArithmeticNumeric(object value) {
            p_Value = value;

            //get what type the value is
            if (value is sbyte) { p_Type = NumericalType.SBYTE; }
            else if (value is byte) { p_Type = NumericalType.BYTE; }
            else if (value is short) { p_Type = NumericalType.INT16; }
            else if (value is ushort) { p_Type = NumericalType.UINT16; }
            else if (value is int) { p_Type = NumericalType.INT32; }
            else if (value is uint) { p_Type = NumericalType.UINT32; }
            else if (value is long) { p_Type = NumericalType.INT64; }
            else if (value is ulong) { p_Type = NumericalType.UINT64; }
            else if (value is float) { p_Type = NumericalType.FLOAT; }
            else if (value is double) { p_Type = NumericalType.DOUBLE; }
            else { 
                //it's invalid
                throw new Exception("Invalid numerical object");
            }
        }

        public bool IsDecimal { get { return p_Type == NumericalType.FLOAT || p_Type == NumericalType.DOUBLE; } }
        public bool IsInteger { get { return !IsDecimal; } }
        public sbyte Size {
            get {
                switch (p_Type) { 
                    case NumericalType.BYTE:
                    case NumericalType.SBYTE:
                        return 1;
                    case NumericalType.INT16:
                    case NumericalType.UINT16:
                        return 2;
                    case NumericalType.INT32:
                    case NumericalType.UINT32:
                    case NumericalType.FLOAT:
                        return 4;
                    case NumericalType.INT64:
                    case NumericalType.UINT64:
                    case NumericalType.DOUBLE:
                        return 8;
                }
                return 8;
            }
        }
        public object RAWObject { get { return p_Value; } }

        /*
            These functions are pretty much copy&paste. 
        */
        public void Add(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float ret = (float)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (float)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (float)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (float)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (float)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (float)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (float)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (float)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (float)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (float)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double ret = (double)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret += (double)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret += (double)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret += (double)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret += (double)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret += (double)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret += (double)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret += (double)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret += (double)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret += (double)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret += (double)number.p_Value; }
                p_Value = ret;
            }
            #endregion
        }
        public void Subtract(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float ret = (float)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (float)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (float)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (float)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (float)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (float)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (float)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (float)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (float)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (float)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double ret = (double)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret -= (double)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret -= (double)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret -= (double)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret -= (double)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret -= (double)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret -= (double)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret -= (double)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret -= (double)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret -= (double)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret -= (double)number.p_Value; }
                p_Value = ret;
            }
            #endregion
        }        
        public void Multiply(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float ret = (float)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (float)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (float)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (float)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (float)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (float)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (float)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (float)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (float)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (float)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double ret = (double)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret *= (double)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret *= (double)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret *= (double)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret *= (double)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret *= (double)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret *= (double)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret *= (double)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret *= (double)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret *= (double)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret *= (double)number.p_Value; }
                p_Value = ret;
            }
            #endregion
        }
        public void Divide(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float ret = (float)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (float)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (float)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (float)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (float)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (float)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (float)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (float)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (float)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (float)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double ret = (double)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret /= (double)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret /= (double)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret /= (double)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret /= (double)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret /= (double)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret /= (double)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret /= (double)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret /= (double)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret /= (double)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret /= (double)number.p_Value; }
                p_Value = ret;
            }
            #endregion
        }        
        public void Modulus(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float ret = (float)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (float)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (float)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (float)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (float)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (float)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (float)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (float)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (float)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (float)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double ret = (double)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret %= (double)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret %= (double)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret %= (double)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret %= (double)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret %= (double)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret %= (double)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret %= (double)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret %= (double)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret %= (double)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret %= (double)number.p_Value; }
                p_Value = ret;
            }
            #endregion
        }
  
        public void ShiftLeft(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret <<= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret <<= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret <<= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret <<= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret <<= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret <<= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret <<= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret <<= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret <<= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret <<= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret <<= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret <<= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret <<= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret <<= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret <<= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret <<= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret <<= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret <<= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret <<= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret <<= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret <<= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret <<= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret <<= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret <<= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret <<= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret <<= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret <<= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret <<= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret <<= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret <<= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret <<= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret <<= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret <<= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret <<= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret <<= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret <<= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret <<= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret <<= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret <<= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret <<= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret <<= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret <<= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret <<= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret <<= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret <<= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret <<= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret <<= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret <<= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret <<= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret <<= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                throw new Exception("Not supported");
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                throw new Exception("Not supported");
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                throw new Exception("Not supported");
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                throw new Exception("Not supported");
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                throw new Exception("Not supported");
            }
            #endregion
        }        
        public void ShiftRight(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret >>= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret >>= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret >>= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret >>= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret >>= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret >>= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret >>= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret >>= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret >>= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret >>= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret >>= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret >>= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret >>= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret >>= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret >>= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret >>= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret >>= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret >>= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret >>= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret >>= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret >>= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret >>= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret >>= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret >>= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret >>= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret >>= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret >>= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret >>= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret >>= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret >>= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret >>= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret >>= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret >>= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret >>= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret >>= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret >>= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret >>= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret >>= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret >>= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret >>= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret >>= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret >>= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret >>= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret >>= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret >>= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret >>= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret >>= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret >>= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret >>= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret >>= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                throw new Exception("Not supported");
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                throw new Exception("Not supported");
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                throw new Exception("Not supported");
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                throw new Exception("Not supported");
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                throw new Exception("Not supported");
            }
            #endregion
        }
        public void BitwiseAnd(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret &= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret &= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret &= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret &= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret &= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret &= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret &= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret &= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret &= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret &= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                throw new Exception("Not supported");
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                throw new Exception("Not supported");
            }
            #endregion
        }
        public void BitwiseOr(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret |= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret |= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret |= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret |= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret |= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret |= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret |= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret |= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret |= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret |= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                throw new Exception("Not supported");
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                throw new Exception("Not supported");
            }
            #endregion
        }
        public void BitwiseXor(ArithmeticNumeric number) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte ret = (sbyte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (sbyte)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (sbyte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (sbyte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (sbyte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (sbyte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (sbyte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (sbyte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (sbyte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (sbyte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte ret = (byte)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (byte)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (byte)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (byte)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (byte)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (byte)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (byte)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (byte)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (byte)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (byte)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short ret = (short)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (short)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (short)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (short)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (short)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (short)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (short)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (short)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (short)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (short)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort ret = (ushort)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (ushort)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (ushort)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (ushort)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (ushort)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (ushort)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (ushort)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (ushort)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (ushort)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (ushort)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int ret = (int)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (int)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (int)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (int)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (int)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (int)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (int)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (int)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (int)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (int)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint ret = (uint)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (uint)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (uint)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (uint)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (uint)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (uint)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (uint)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (uint)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (uint)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (uint)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long ret = (long)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (long)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (long)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (long)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (long)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (long)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (long)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (long)(ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (long)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (long)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong ret = (ulong)p_Value;
                if (number.p_Type == NumericalType.SBYTE) { ret ^= (ulong)(sbyte)number.p_Value; }
                else if (number.p_Type == NumericalType.BYTE) { ret ^= (ulong)(byte)number.p_Value; }
                else if (number.p_Type == NumericalType.INT16) { ret ^= (ulong)(short)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT16) { ret ^= (ulong)(ushort)number.p_Value; }
                else if (number.p_Type == NumericalType.INT32) { ret ^= (ulong)(int)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT32) { ret ^= (ulong)(uint)number.p_Value; }
                else if (number.p_Type == NumericalType.INT64) { ret ^= (ulong)(long)number.p_Value; }
                else if (number.p_Type == NumericalType.UINT64) { ret ^= (ulong)number.p_Value; }
                else if (number.p_Type == NumericalType.FLOAT) { ret ^= (ulong)(float)number.p_Value; }
                else if (number.p_Type == NumericalType.DOUBLE) { ret ^= (ulong)(double)number.p_Value; }
                p_Value = ret;
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                throw new Exception("Not supported");
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                throw new Exception("Not supported");
            }
            #endregion
        }

        public bool Equals(ArithmeticNumeric compare) {
            #region SBYTE
            if (p_Type == NumericalType.SBYTE) {
                sbyte val = (sbyte)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (sbyte)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (sbyte)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (sbyte)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (sbyte)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (sbyte)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (sbyte)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (sbyte)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (sbyte)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (sbyte)(double)compare.p_Value; }
            }
            #endregion
            
            #region BYTE
            else if (p_Type == NumericalType.BYTE) {
                byte val = (byte)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (byte)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (byte)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (byte)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (byte)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (byte)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (byte)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (byte)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (byte)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (byte)(double)compare.p_Value; }
            }
            #endregion

            #region INT16
            else if (p_Type == NumericalType.INT16) {
                short val = (short)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (short)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (short)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (short)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (short)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (short)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (short)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (short)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (short)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (short)(double)compare.p_Value; }
            }
            #endregion

            #region UINT16
            else if (p_Type == NumericalType.UINT16) {
                ushort val = (ushort)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (ushort)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (ushort)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (ushort)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (ushort)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (ushort)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (ushort)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (ushort)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (ushort)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (ushort)(double)compare.p_Value; }
            }
            #endregion

            #region INT32
            else if (p_Type == NumericalType.INT32) {
                int val = (int)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (int)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (int)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (int)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (int)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (int)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (int)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (int)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (int)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (int)(double)compare.p_Value; }
            }
            #endregion

            #region UINT32
            else if (p_Type == NumericalType.UINT32) {
                uint val = (uint)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (uint)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (uint)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (uint)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (uint)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (uint)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (uint)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (uint)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (uint)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (uint)(double)compare.p_Value; }
            }
            #endregion

            #region INT64
            else if (p_Type == NumericalType.INT64) {
                long val = (long)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (long)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (long)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (long)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (long)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (long)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (long)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (long)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (long)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (long)(double)compare.p_Value; }
            }
            #endregion

            #region UINT64
            else if (p_Type == NumericalType.UINT64) {
                ulong val = (ulong)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (ulong)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (ulong)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (ulong)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (ulong)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (ulong)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (ulong)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (ulong)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (ulong)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (ulong)(double)compare.p_Value; }
            }
            #endregion

            #region FLOAT
            else if (p_Type == NumericalType.FLOAT) {
                float val = (float)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (float)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (float)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (float)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (float)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (float)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (float)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (float)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (float)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (float)(double)compare.p_Value; }
            }
            #endregion

            #region DOUBLE
            else if (p_Type == NumericalType.DOUBLE) {
                double val = (double)p_Value;
                if (compare.p_Type == NumericalType.SBYTE) { return val == (double)(sbyte)compare.p_Value; }
                if (compare.p_Type == NumericalType.BYTE) { return val == (double)(byte)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT16) { return val == (double)(short)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT16) { return val == (double)(ushort)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT32) { return val == (double)(int)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT32) { return val == (double)(uint)compare.p_Value; }
                if (compare.p_Type == NumericalType.INT64) { return val == (double)(long)compare.p_Value; }
                if (compare.p_Type == NumericalType.UINT64) { return val == (double)(ulong)compare.p_Value; }
                if (compare.p_Type == NumericalType.FLOAT) { return val == (double)(float)compare.p_Value; }
                if (compare.p_Type == NumericalType.DOUBLE) { return val == (double)compare.p_Value; }
            }
            #endregion

            //this should never happen
            throw new Exception("");
        }
        public override bool Equals(object obj) {
            if (!(obj is ArithmeticNumeric)) { return false; }
            return Equals((ArithmeticNumeric)obj);
        }

        #region operators
        public static ArithmeticNumeric operator +(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.Add(b);
            return a;
        }
        public static ArithmeticNumeric operator -(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.Subtract(b);
            return a;
        }
        public static ArithmeticNumeric operator -(ArithmeticNumeric a) {
            return 0 - a;
        }
        public static ArithmeticNumeric operator *(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.Multiply(b);
            return a;
        }
        public static ArithmeticNumeric operator /(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.Divide(b);
            return a;
        }
        public static ArithmeticNumeric operator %(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.Modulus(b);
            return a;
        }

        public static ArithmeticNumeric operator &(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.BitwiseAnd(b);
            return a;
        }
        public static ArithmeticNumeric operator |(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.BitwiseOr(b);
            return a;
        }
        public static ArithmeticNumeric operator ^(ArithmeticNumeric a, ArithmeticNumeric b) {
            a.BitwiseXor(b);
            return a;
        }

        public static bool operator ==(ArithmeticNumeric a, ArithmeticNumeric b) {
            return a.Equals(b);
        }
        public static bool operator !=(ArithmeticNumeric a, ArithmeticNumeric b) { 
            return !a.Equals(b);
        }

        public static implicit operator ArithmeticNumeric(sbyte value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(byte value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(short value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(ushort value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(int value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(uint value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(long value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(ulong value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(double value) { return new ArithmeticNumeric(value); }
        public static implicit operator ArithmeticNumeric(float value) { return new ArithmeticNumeric(value); }
        #endregion
        
        public static ArithmeticNumeric FromString(string str, bool isDecimal, byte length) {
            //get the amount of memory the number would need
            sbyte memSize = RequiredMemory(length, isDecimal);

            //return the string conversion to the correct type
            switch (memSize) {
                case 1: return Convert.ToSByte(str);
                case 2: return Convert.ToInt16(str);
                case 4:
                    if (isDecimal) { return Convert.ToSingle(str); }
                    return Convert.ToSingle(str);
                case 8:
                    if (isDecimal) { return Convert.ToDouble(str); }
                    return Convert.ToDouble(str);
            }

            //this should never happen
            throw new Exception("Invalid call");
        }
        public static ArithmeticNumeric CreateOfSize(int sizeInBytes, bool isDecimal) {
            switch (sizeInBytes) {
                case 1: return (byte)0;
                case 2: return (short)0;
                case 4:
                    if (isDecimal) { return (float)0; }
                    return (int)0;
                case 8:
                    if (isDecimal) { return (double)0; }
                    return (long)0;
                default:
                    
                    //round size to nearest valid size
                    if (sizeInBytes < 1) { sizeInBytes = 1; }
                    else if (sizeInBytes < 4) { sizeInBytes = 4; }
                    else if (sizeInBytes < 8) { sizeInBytes = 8; }
                    else if (sizeInBytes >= 8) {
                        throw new Exception("Requested size is not supported");
                    }

                    return CreateOfSize(sizeInBytes, isDecimal);
            }
        }
        public static sbyte SizeOfNumericObj(object value) {
            if (value is sbyte || value is byte) { return 1; }
            if (value is short || value is ushort) { return 2; }
            if (value is int || value is uint) { return 4; }
            if (value is long || value is ulong) { return 8; }
            if (value is float) { return 4; }
            if (value is double) { return 8; }
            return 1;
        }
        public static sbyte RequiredMemory(byte numOfCharacters, bool isDecimal) {
            //0-99
            if (numOfCharacters < 3 && !isDecimal) { return 1; }
            //0-9,999
            else if (numOfCharacters < 5 && !isDecimal) { return 2; }
            //0-999,999,999
            else if (numOfCharacters < 10) { return 4; }
            return 8;
        }

        private static ArithmeticNumeric p_Zero = 0;
        public static ArithmeticNumeric Zero { get { return p_Zero; } }

        public override string ToString() {
            return p_Value.ToString();
        }

        public enum NumericalType { 
            SBYTE = 0,
            BYTE = 1,

            INT16 = 2,
            UINT16 = 3,

            INT32 = 4,
            UINT32= 5,

            INT64 = 6,
            UINT64 = 7,

            FLOAT = 8,
            DOUBLE = 9
        }
    }
}