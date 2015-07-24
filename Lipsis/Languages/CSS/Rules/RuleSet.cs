using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public unsafe class CSSRuleSet : ICSSScope {
        private static byte* IMPORTANT_STRING;
        private const int IMPORTANT_STRING_LENGTH = 9;
        static CSSRuleSet() {
            IMPORTANT_STRING = (byte*)Marshal.StringToCoTaskMemAnsi("important").ToPointer();
        }
        
        private LinkedList<CSSRule> p_Rules;

        public CSSRuleSet() {
            p_Rules = new LinkedList<CSSRule>();
        }

        public CSSRule AddRule(string name, string value) {
            return AddRule(name, value, false);
        }
        public CSSRule AddRule(string name, string value, bool important) {
            CSSRule buffer = new CSSRule(name, value, important);
            p_Rules.AddLast(buffer);
            return buffer;
        }
        public bool RemoveRule(string name) { 
            //get the rules that have the name specified
            LinkedList<CSSRule> rules = GetRules(name);
            if (rules.Count == 0) { return false; }

            //count how many success remove calls we have 
            //on the linked list and return true if the 
            //amount of removed meets the amount we needed
            //to remove.
            int count = 0;
            IEnumerator<CSSRule> e = rules.GetEnumerator();
            while (e.MoveNext()) {
                if (p_Rules.Remove(e.Current)) { count++; }
            }

            //clean up
            e.Dispose();
            return count == rules.Count;
        }

        public LinkedList<CSSRule> GetRules(string name) {
            LinkedList<CSSRule> buffer = new LinkedList<CSSRule>();
            name = name.ToLower();

            //look for all the rules with the name specified
            IEnumerator<CSSRule> e = p_Rules.GetEnumerator();
            while (e.MoveNext()) {
                CSSRule current = e.Current;
                if (current.Name.ToLower() == name) {
                    buffer.AddLast(current);
                }
            }

            //clean up
            e.Dispose();
            return buffer;
        }

        public string this[string name]  {
            get {
                LinkedList<CSSRule> rules = GetRules(name);
                if (rules.Count == 0) { return null; }
                return rules.Last.Value.Value;
            }
            set {
                LinkedList<CSSRule> rules = GetRules(name);
                
                //add?
                if (rules.Count == 0) {
                    AddRule(name, value);
                    return;
                }

                //set the value of the last rule we found
                LinkedListNode<CSSRule> last = rules.Last;
                CSSRule rule = last.Value;
                rule.Value = value;
                last.Value = rule;
            }
        }
        public string this[string name, int index] {
            get {
                return Helpers.LinkedListGetValueByIndex(
                    GetRules(name),
                    index).Value.Value;
            }
        }

        public static CSSRuleSet Parse(string data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSRuleSet Parse(byte[] data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSRuleSet Parse(ref byte* data, int length) {
            return Parse(ref data, length, Encoding.ASCII);
        }
        public static CSSRuleSet Parse(ref byte* data, byte* dataEnd) {
            return Parse(ref data, dataEnd, Encoding.ASCII);
        }

        public static CSSRuleSet Parse(string data, Encoding encoder) {
            return Parse(encoder.GetBytes(data), encoder);
        }
        public static CSSRuleSet Parse(byte[] data, Encoding encoder) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length, encoder);
            }
        }
        public static CSSRuleSet Parse(ref byte* data, int length, Encoding encoder) {
            return Parse(ref data, data + length, encoder);
        }
        public static CSSRuleSet Parse(ref byte* data, byte* dataEnd, Encoding encoder) {
            //define the rule set we add everything to
            CSSRuleSet buffer = new CSSRuleSet();
            
            //read the data
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

                //we hit the end of a scope?
                if (*data == '}') { break; }

                //have we hit an alphadecimal character?
                if (isNameValueCharacter(*data)) {                    
                    #region read the name
                    byte* nameStart = data;
                    byte* nameEnd = dataEnd - 1;
                    while (data < dataEnd) {
                        if (!isNameValueCharacter(*data) || *data == ':') { 
                            nameEnd = data - 1; 
                            break; 
                        }
                        data++;
                    }
                    #endregion 

                    //find the ":" character which seperates the name and value
                    while (data < dataEnd && *data++ != ':') ;
                            
                    //skip to the beginning of the value
                    while (data < dataEnd && !isNameValueCharacter(*data)) { data++; }

                    #region read the value
                    byte* valueStart = data;
                    byte* valueEnd = dataEnd - 1;
                    bool important = false;
                    while (data < dataEnd) {
                        if (*data == ';' || *data == '}') { valueEnd = data - 1; break; }

                        #region !important?
                        if (*data == '!' && data < dataEnd - IMPORTANT_STRING_LENGTH) {
                            //read and compare the upcoming data with the "important" string
                            byte* dataCopy = data + 1;
                            byte* dataCopyEnd = data + IMPORTANT_STRING_LENGTH;
                            byte* impCopy = IMPORTANT_STRING;
                            bool match = true;
                            while (dataCopy < dataCopyEnd) {
                                if (toLower(*dataCopy++) != *impCopy++) {
                                    match = false;
                                    break;
                                }
                            }

                            //match? if so, we do not have the !important inside the value
                            //so we finish reading the value here.
                            if (match) {
                                important = true;
                                valueEnd = data - 1;
                                break;
                            }
                        }
                        #endregion

                        //string open?
                        if (*data == '"' || *data == '\'') { 
                            //skip over it
                            byte closeStr = *data++;
                            while (data < dataEnd) {
                                if (*data == '\\') { data += 2; continue; }
                                if (*data == closeStr) { break; }
                                data++;
                            }
                        }

                        data++;
                    }


                    //trim the value (remove the whitespaces at the end of the value)
                    while (!isNameValueCharacter(*valueEnd)) { valueEnd--; }
                    #endregion

                    //skip to the rule seperate (;)
                    while (data < dataEnd && *data != ';' && *data != '}') { data++; }

                    //create the rule
                    buffer.AddRule(
                        Helpers.ReadString(nameStart, nameEnd, encoder),
                        Helpers.ReadString(valueStart, valueEnd, encoder),
                        important);

                    //hit the end of this ruleset?
                    if (*data == '}') { break; }
                }

                //skip to the next character
                data++;
            }

            //clean up
            return buffer;
        }

        private static bool isNameValueCharacter(byte b) {
            return (
                    b != ' ' &&
                    b != '\n' &&
                    b != '\r' &&
                    b != '\t' &&
                    b != '{' &&
                    b != '}');

        }
        private static byte toLower(byte b) {
            if (b >= 'A' && b <= 'Z') {
                return (byte)((b - 'A') + 'a');
            }
            return b;
        }

        public override string ToString() {
            CSSRule[] rules = Helpers.LinkedListToArray(p_Rules);
            return Helpers.FlattenToString(rules, " ");
           
        }
    }
}