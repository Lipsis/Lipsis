using System;
using System.Text;
using System.Collections.Generic;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public unsafe sealed class CSSDeclaration {
        private LinkedList<CSSSelector> p_Selectors;
        private CSSRuleSet p_Ruleset;

        public CSSDeclaration() {
            p_Ruleset = new CSSRuleSet();
            p_Selectors = new LinkedList<CSSSelector>();
        }
        public CSSDeclaration(LinkedList<CSSSelector> selectors) {
            p_Selectors = selectors;
            p_Ruleset = new CSSRuleSet();
        }
        public CSSDeclaration(CSSRuleSet ruleset) {
            p_Selectors = new LinkedList<CSSSelector>();
            p_Ruleset = ruleset;
        }
        public CSSDeclaration(LinkedList<CSSSelector> selectors, CSSRuleSet ruleset) {
            p_Selectors = selectors;
            p_Ruleset = ruleset;
        }

        public void AddSelector(CSSSelector selector) {
            p_Selectors.AddLast(selector);
        }
        public bool RemoveSelector(CSSSelector selector) {
            return p_Selectors.Remove(selector);
        }

        public CSSRule AddRule(string name, string value) {
            return p_Ruleset.AddRule(name, value);
        }
        public bool RemoveRule(string name) {
            return p_Ruleset.RemoveRule(name);
        }

        public LinkedList<CSSSelector> Selectors { get { return p_Selectors; } }
        public CSSRuleSet RuleSet { get { return p_Ruleset; } }

        public static CSSDeclaration Parse(string data) {
            return Parse(Encoding.ASCII.GetBytes(data));
        }
        public static CSSDeclaration Parse(byte[] data) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length);
            }
        }
        public static CSSDeclaration Parse(ref byte* data, int length) {
            return Parse(ref data, data + length);
        }
        public static CSSDeclaration Parse(ref byte* data, byte* dataEnd) {
            //read selectors
            LinkedList<CSSSelector> selectors = new LinkedList<CSSSelector>();
            while (data < dataEnd) {
                //block comment? (/**/)
                if (*data == '/' && data < dataEnd - 2 && *(data + 1) == '*') {
                    //skip to the end of the block comment
                    while (data < dataEnd - 2) {
                        if (*data == '*' && *(data + 1) == '/') { data += 2; break; }
                        data++;
                    }
                }

                //
                CSSSelector selector = CSSSelector.Parse(ref data, dataEnd);
                selectors.AddLast(selector);
                if (*data != ',') { break; }
                data++;
            }

            //read the css rules
            if (*data != '{') { return null; }
            data++;
            CSSRuleSet set = CSSRuleSet.Parse(ref data, dataEnd);

            return new CSSDeclaration(
                selectors,
                set);
        }

        public override string ToString() {
            CSSSelector[] selectors = Helpers.LinkedListToArray(p_Selectors);
            return Helpers.FlattenToString(selectors, ", ");
        }
    }
}