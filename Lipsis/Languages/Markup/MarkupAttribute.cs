using System;

namespace Lipsis.Languages.Markup {

    public struct MarkupAttribute {

        private string p_Name, p_Value;
        private bool p_IsNumeric;

        internal MarkupAttribute(string name, string value) {
            p_Name = name;
            p_Value = value;
            p_IsNumeric = false;
            updateState();
        }

        public string Name { get { return p_Name; } }
        public string Value {
            get { return p_Value; }
            set { 
                p_Value = value;
                updateState();
            }
        }

        public bool IsNumeric { get { return p_IsNumeric; } }

        private void updateState() {  
            //is the value numeric?
            decimal dTemp;
            p_IsNumeric = decimal.TryParse(Value, out dTemp);

        }

        public static implicit operator bool(MarkupAttribute atr) {
            string value = atr.Value;
            if (value == "1" || value == "") { return true; }
            return value.ToLower() == "true";
        }
        public static implicit operator string(MarkupAttribute atr) { return atr.Value; }
        public static implicit operator ulong(MarkupAttribute atr) { return Convert.ToUInt64(atr.Value); }
        public static implicit operator long(MarkupAttribute atr) { return Convert.ToInt64(atr.Value); }
        public static implicit operator uint(MarkupAttribute atr) { return Convert.ToUInt32(atr.Value); }
        public static implicit operator int(MarkupAttribute atr) { return Convert.ToInt32(atr.Value); }
        public static implicit operator ushort(MarkupAttribute atr) { return Convert.ToUInt16(atr.Value); }
        public static implicit operator short(MarkupAttribute atr) { return Convert.ToInt16(atr.Value); }
        public static implicit operator byte(MarkupAttribute atr) { return Convert.ToByte(atr.Value); }
        public static implicit operator sbyte(MarkupAttribute atr) { return Convert.ToSByte(atr.Value); }
        public static implicit operator decimal(MarkupAttribute atr) { return Convert.ToDecimal(atr.Value); }
        public static implicit operator double(MarkupAttribute atr) { return Convert.ToDouble(atr.Value); }
        public static implicit operator float(MarkupAttribute atr) { return Convert.ToSingle(atr.Value); }


        public override string ToString() {
            //return just the value?
            if (p_Name == null) { return "\"" + p_Value + "\""; }
            if (p_Value == null) { return p_Name; }

            return
                Name + "=\"" + Value + "\"";
        }
    }

}