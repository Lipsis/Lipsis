using System;

namespace Lipsis.Languages.CSS {
    public struct CSSSelectorAttribute {
        private string p_Name, p_Value, p_ValuePublic;
        private CSSSelectorAttributeCompareType p_CompareType;

        public CSSSelectorAttribute(string name, string value, CSSSelectorAttributeCompareType compareType) {
            p_Name = name;
            p_Value = value;
            p_ValuePublic = value;
            p_CompareType = compareType;

            //if the compare type is a dashed prefix compare, add "-" onto the value
            //so when we match, it doesnt have to add it on everytime.
            if (compareType == CSSSelectorAttributeCompareType.DashSplitPrefixMatch) {
                p_Value += "-";
            }
        }

        public string Name { get { return p_Name; } }
        public string Value { get { return p_ValuePublic; } }
        public CSSSelectorAttributeCompareType CompareType { get { return p_CompareType; } }

        public bool Match(string value) {
            switch (p_CompareType) {
                case CSSSelectorAttributeCompareType.Match:
                    return p_Value == value;
                case CSSSelectorAttributeCompareType.Contains:
                    return value.Contains(p_Value);

                case CSSSelectorAttributeCompareType.WhitespaceSplitMatch:
                    return
                        Array.IndexOf(
                            value.Split(' '),
                            p_Value) != -1;
                case CSSSelectorAttributeCompareType.DashSplitPrefixMatch:
                    return value.StartsWith(p_Value); //value would be value- set from constructor

                case CSSSelectorAttributeCompareType.PrefixMatch:
                    return value.StartsWith(p_Value);
                case CSSSelectorAttributeCompareType.SuffixMatch:
                    return value.EndsWith(p_Value);
            }
            return false;
        }

        public override string ToString() {
            //just return the name of the attribute?
            if (p_CompareType == CSSSelectorAttributeCompareType.HasAttribute) {
                return "[" + p_Name + "]";
            }

            //define what the compare string is
            string compareStr = "=";
            switch (p_CompareType) {
                case CSSSelectorAttributeCompareType.Contains: compareStr = "*="; break;
                case CSSSelectorAttributeCompareType.DashSplitPrefixMatch: compareStr = "|="; break;
                case CSSSelectorAttributeCompareType.PrefixMatch: compareStr = "^="; break;
                case CSSSelectorAttributeCompareType.SuffixMatch: compareStr = "$="; break;
                case CSSSelectorAttributeCompareType.WhitespaceSplitMatch: compareStr = "~="; break;            
            }

            return
                "[" +
                p_Name +
                compareStr +
                "\"" + p_ValuePublic + "\"]";
        }
    }
}