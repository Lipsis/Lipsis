using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public unsafe class CSSSheet {
        private static STRPTR STR_NAMESPACE;

        static CSSSheet() {
            STR_NAMESPACE = allocString("namespace");
        }

        private LinkedList<CSSDeclaration> p_Declarations;
        public CSSSheet() {
            p_Declarations = new LinkedList<CSSDeclaration>();
        }
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
                //block comment? (/**/)
                if (*data == '/' && data < dataEnd - 2 && *(data + 1) == '*') {
                    //skip to the end of the block comment
                    while (data < dataEnd - 2) {
                        if (*data == '*' && *(data + 1) == '/') { data += 2; break; }
                        data++;
                    }
                }

                //skip to the beginning of data
                Helpers.SkipWhitespaces(ref data, dataEnd);

                #region line starting with "@"?
                if (*data == '@') {
                    data++;

                    //define the list which will contains all the sub-values (split ' ')
                    LinkedList<STRPTR> components = new LinkedList<STRPTR>();

                    //skip to beginning of the data
                    if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }

                    #region read components
                    byte* strPtr = data;
                    byte* strPtrEnd = (byte*)0;
                    while (data < dataEnd) {
                        
                        //string?
                        if (*data == '"' || *data == '\'') { 
                            //read the string
                            byte strTerminate = *data++;
                            while (data < dataEnd) {
                                if (*data == '\\') { data += 2; continue; }
                                if (*data == strTerminate) {                                    
                                    strPtrEnd = data - 1;
                                    data++;
                                    break;
                                }
                                data++;
                            }
                        }
                        
                        //space/end of line
                        if (*data == ' ' || *data == ';' || *data == '\n') {
                            strPtrEnd = data - 1;

                            if (!isBlank(strPtr, strPtrEnd)) {
                                components.AddLast(new STRPTR(strPtr, strPtrEnd));
                            }
                            
                            //end of line?
                            if (*data == ';' || *data == '\n') { data++; break; }

                            //skip the beginning of the next component
                            if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }
                            strPtr = data;
                            strPtrEnd = (byte*)0;
                        }

                        data++;
                    }
                    #endregion
                                
                    //any components found?
                    if (components.Count == 0) { continue; }

                    #region namespace?
                    if (compareStr(components.First.Value, STR_NAMESPACE, false)) {



                    }
                    #endregion

                    continue;
                }
                #endregion

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
                if (*data == '}') { data++; }
                
                //add it
                sheet.AddDeclaration(selectors, set);
            }
            
            return sheet;
        }

        private static bool compareStr(STRPTR str1, STRPTR str2, bool matchCase) {
            return
                compareStr(
                str1.PTR,
                str2.PTR,
                str1.ENDPRR + 1,
                str2.ENDPRR + 1,
                matchCase);
        }
        private static bool compareStr(byte* ptr1, byte* ptr2, byte* ptrEnd1, byte* ptrEnd2, bool matchCase) { 
            //same length?
            if (ptrEnd1 - ptr1 != ptrEnd2 - ptr2) { return false; }

            //read through the data
            while (ptr1 < ptrEnd1) { 
                //get the two bytes we compare and set them to lower case if
                //we do not match case
                byte b1 = *ptr1++;
                byte b2 = *ptr2++;
                if (!matchCase) {
                    b1 = toLower(b1);
                    b2 = toLower(b2);
                }

                //match?
                if (b1 != b2) { return false; }
            }

            //no-mismatch found
            return true;
        }
        private static byte toLower(byte b) {
            if (b >= 'A' && b <= 'Z') {
                return (byte)((b - 'A') + 'a');
            }
            return b;
        }
        private static bool isBlank(byte* ptr, byte* ptrEnd) {
            while (ptr < ptrEnd) {
                if (*ptr != ' ' &&
                    *ptr != '\n' &&
                    *ptr != '\t' &&
                    *ptr != '\r') {
                        return false;
                }
                ptr++;
            }
            return true;
        }
        private static STRPTR allocString(string str) {
            //alocate the string on the heap
            IntPtr ptr = Marshal.StringToCoTaskMemAnsi(str);
            byte* ptrUnmanaged = (byte*)ptr.ToPointer();

            //return the STRPTR structure for the string
            return new STRPTR(
                ptrUnmanaged,
                ptrUnmanaged + str.Length - 1);
        }
        private struct STRPTR {
            public STRPTR(byte* ptr, byte* endPtr) {
                PTR = ptr;
                ENDPRR = endPtr;
            }
            public byte* PTR;
            public byte* ENDPRR;
        }
    }
}