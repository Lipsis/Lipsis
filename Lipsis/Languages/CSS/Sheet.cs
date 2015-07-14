using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public unsafe sealed class CSSSheet : CSSDeclarationCollection {
        private static STRPTR STR_NAMESPACE;

        static CSSSheet() {
            STR_NAMESPACE = allocString("namespace");
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

                #region at-rule?
                if (*data == '@') {
                    data++;

                    //define the list which will contains all the arguments
                    LinkedList<STRPTR> arguments = new LinkedList<STRPTR>();

                    //if the definition has a scope, this is where it's declarations will be stored
                    ICSSScope scope = null;

                    //skip to beginning of the data
                    if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }

                    #region read the name
                    byte* namePtr = data;
                    byte* nameEndPtr = (byte*)0;
                    while (data < dataEnd) {
                        if (!(
                            (*data >= 'A' && *data <= 'Z') ||
                            (*data >= 'a' && *data <= 'z') ||
                            *data == '-')) {
                                nameEndPtr = data - 1;
                                Helpers.SkipWhitespaces(ref data, dataEnd);
                                break;
                        }
                        data++;
                    }
                    #endregion

                    #region read arguments
                    byte* strPtr = data;
                    byte* strPtrEnd = (byte*)0;
                    while (data < dataEnd) {
                        #region skip comments
                        //block comment? (/**/)
                        if (*data == '/' && data < dataEnd - 2 && *(data + 1) == '*') {
                            //skip to the end of the block comment
                            while (data < dataEnd - 2) {
                                if (*data == '*' && *(data + 1) == '/') { data += 2; break; }
                                data++;
                            }
                        }
                        #endregion

                        #region scope for declarations?
                        if (*data == '{') {
                            //add whatever was before the scope beginning 
                            strPtrEnd = data - 1;
                            if (!isBlank(strPtr, strPtrEnd)) {
                                arguments.AddLast(new STRPTR(strPtr, strPtrEnd));
                            }
                            data++;

                            //deturmine whethere we process the scope as a ruleset 
                            //or a declaration by sampling the upcoming data
                            //and seeing if we hit a tell-tail sign of a ruleset
                            //or a declaration
                            bool isRuleset = false;
                            byte* dataSamplePtr = data;
                            while (dataSamplePtr < dataEnd) {
                                if (*dataSamplePtr == ':') { isRuleset = true; break; }
                                if (*dataSamplePtr == '{') { break; }
                                dataSamplePtr++;
                            }

                            #region ruleset?
                            if (isRuleset) {
                                scope = CSSRuleSet.Parse(ref data, dataEnd);
                                if (*data == '}') { data++; }
                            }
                            #endregion

                            #region declarations?
                            else {
                                //create the scope declarations
                                CSSDeclarationCollection dScope = new CSSDeclarationCollection();
                                scope = dScope;

                                //scan declarations we hit the end of the scope
                                while (data < dataEnd) {
                                    CSSDeclaration d = CSSDeclaration.Parse(ref data, dataEnd);
                                    if (d == null) { break; }
                                    dScope.AddDeclaration(d);
                                    if (*data == '}') { data++; }

                                    if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }
                                    if (*data == '}') { data++; break; }

                                    continue;
                                }

                                break;
                            }
                            #endregion
                            break;
                        }
                        #endregion
                        
                        #region string?
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
                        #endregion

                        #region space/end of line
                        if (*data == ' ' || *data == ';') {
                            strPtrEnd = data - 1;

                            if (!isBlank(strPtr, strPtrEnd)) {
                                arguments.AddLast(new STRPTR(strPtr, strPtrEnd));
                            }
                            
                            //end of line?
                            if (*data == ';') { data++; break; }
                            data++;

                            //skip the beginning of the next component
                            if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }
                            strPtr = data;
                            strPtrEnd = (byte*)0;
                            continue;
                        }
                        #endregion

                        data++;
                    }
                    #endregion

                    handleAtRule(
                        sheet,
                        new STRPTR(namePtr, nameEndPtr),
                        arguments,
                        scope);
                    continue;
                }
                #endregion

                //read the declaration
                CSSDeclaration declaration = CSSDeclaration.Parse(ref data, dataEnd);
                if (declaration == null) { break; }

                //add the declaration
                sheet.AddDeclaration(declaration);

                //at the end?
                if (*data == '}') { data++; }
            }
            
            return sheet;
        }


        private static void handleAtRule(CSSSheet sheet, STRPTR name, LinkedList<STRPTR> arguments, ICSSScope scope) {
            string nameStr = Helpers.ReadString(name.PTR, name.ENDPRR);

            if (!(scope is CSSDeclarationCollection)) { return; }
            sheet.AddDeclarations((scope as CSSDeclarationCollection));

            return;
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