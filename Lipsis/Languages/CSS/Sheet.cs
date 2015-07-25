using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public unsafe sealed class CSSSheet : CSSDeclarationCollection {
        private static STRPTR STR_NAMESPACE;

        static CSSSheet() {
            STR_NAMESPACE = "namespace";
        }

        public static CSSSheet Parse(string data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSSheet Parse(byte[] data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSSheet Parse(ref byte* data, int length) {
            return Parse(ref data, length, Encoding.ASCII);
        }
        public static CSSSheet Parse(ref byte* data, byte* dataEnd) {
            return Parse(ref data, dataEnd, Encoding.ASCII);
        }

        public static CSSSheet Parse(string data, Encoding encoder) {
            return Parse(encoder.GetBytes(data), encoder);
        }
        public static CSSSheet Parse(byte[] data, Encoding encoder) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length, encoder);
            }
        }
        public static CSSSheet Parse(ref byte* data, int length, Encoding encoder) {
            return Parse(ref data, data + length, encoder);
        }
        public static CSSSheet Parse(ref byte* data, byte* dataEnd, Encoding encoder) {
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

                            //deturmine whether we process the scope as a ruleset 
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
                                scope = CSSRuleSet.Parse(ref data, dataEnd, encoder);
                                if (*data == '}') { data++; }
                            }
                            #endregion

                            #region declarations?
                            else {
                                //read it as a CSSSheet to support nested at-rules
                                scope = Parse(ref data, dataEnd, encoder);
                                if (*data == '}') { data++; }
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
                        scope,
                        encoder);
                    continue;
                }
                #endregion

                //close already?
                if (*data == '}') { break; }

                //read the declaration
                CSSDeclaration declaration = CSSDeclaration.Parse(ref data, dataEnd, encoder);
                if (declaration == null) { break; }

                //add the declaration
                sheet.AddDeclaration(declaration);

                //at the end?
                if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }
                if (*data == '}') { data++; }
            }
            
            return sheet;
        }
        
        private static void handleAtRule(CSSSheet sheet, STRPTR name, LinkedList<STRPTR> arguments, ICSSScope scope, Encoding encoder) {
            string nameStr = name;


            return;
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
    }
}