using System;
using System.Text;
using System.Collections.Generic;

namespace Lipsis.Languages.CSS {
    public unsafe class CSSSheet {

        private LinkedList<CSSDeclaration> p_Declarations;

        public CSSSheet() { p_Declarations = new LinkedList<CSSDeclaration>(); }
        public CSSSheet(LinkedList<CSSDeclaration> declarations) {
            p_Declarations = declarations;
        }

        public void AddDeclaration(LinkedList<CSSSelector> selectors, CSSRuleSet rules) {
            AddDeclaration(new CSSDeclaration(selectors, rules));
        }
        public void AddDeclaration(CSSDeclaration declaration) {
            p_Declarations.AddLast(declaration);
        }

        public bool RemoveDeclaration(CSSDeclaration declaration) {
            return p_Declarations.Remove(declaration);
        }

        public static CSSSheet Parse(string data) {
            return Parse(Encoding.ASCII.GetBytes(data));
        }
        public static CSSSheet Parse(byte[] data) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length);
            }
        }
        public static CSSSheet Parse(ref byte* data, int length) {
            return Parse(ref data, data + length);
        }
        public static CSSSheet Parse(ref byte* data, byte* dataEnd) {
            CSSSheet sheet = new CSSSheet();

            
            //read the declarations
            while (data < dataEnd) {
                //read selectors
                LinkedList<CSSSelector> selectors = new LinkedList<CSSSelector>();
                while (data < dataEnd) {
                    CSSSelector selector = CSSSelector.Parse(ref data, dataEnd);
                    selectors.AddLast(selector);
                    if (*data != ',') { break; }
                    data++;
                }

                //read the css rules
                if (*data != '{') { break; }
                data++;
                CSSRuleSet set = CSSRuleSet.Parse(ref data, dataEnd);

                //at the end?
                if (*data != '}') { break; }
                data++;

                //add it
                sheet.AddDeclaration(selectors, set);
            }
            
            return sheet;
        }
    }
}