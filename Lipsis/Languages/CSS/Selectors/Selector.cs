using System;
using System.Text;
using System.Collections.Generic;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public sealed unsafe class CSSSelector {
        private LinkedList<CSSSelectorType> p_Types;
        private CSSSelector(LinkedList<CSSSelectorType> types) {
            p_Types = types;
        }

        public LinkedList<CSSSelectorType> Types { get { return p_Types; } }

        #region Parsing
        public static CSSSelector Parse(string data) {
            return Parse(Encoding.ASCII.GetBytes(data));
        }
        public static CSSSelector Parse(byte[] data) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length);
            }
        }
        public static CSSSelector Parse(ref byte* data, int length) {
            return Parse(ref data, data + length);
        }
        public static CSSSelector Parse(ref byte* data, byte* dataEnd) { 
            //define the return buffer
            LinkedList<CSSSelectorType> buffer = new LinkedList<CSSSelectorType>();

            //define what the relationship of the previous selection to the 
            //current selector we are reading is.
            CSSSelectorPreSelectorRelationship preRelation = CSSSelectorPreSelectorRelationship.GeneralChild;
            
            //read through the data
            while (data < dataEnd) {
                //open of a scope?
                if (*data == '{') { break; }

                //at a selector divider?
                if (*data == ',') { break; }

                //ignore whitespaces
                if (isWhitespace(*data)) { data++; continue; }
               
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
                
                #region relationship definition?
                if (*data == '~' || *data == '>' || *data == '+' || *data == '|') {
                    switch ((char)*data) {
                        case '~': preRelation = CSSSelectorPreSelectorRelationship.GeneralSibling; break;
                        case '>': preRelation = CSSSelectorPreSelectorRelationship.ImmidiateChild; break;
                        case '+': preRelation = CSSSelectorPreSelectorRelationship.AdjacentSibling; break;
                        case '|': preRelation = CSSSelectorPreSelectorRelationship.NamespaceChild; break;
                    }
                    data++;
                    continue;
                }
                #endregion
                
                #region class/id/all?
                CSSSelectorElementTargetType type = CSSSelectorElementTargetType.Tag;
                if (*data == '.' || *data == '#' || *data == '*') {
                    switch ((char)*data) {
                        case '.': type = CSSSelectorElementTargetType.Class; break;
                        case '#': type = CSSSelectorElementTargetType.ID; break;
                        case '*': type = CSSSelectorElementTargetType.All; break;
                    }
                    data++;
                }
                #endregion

                #region read the name
                //read the name (only this selector is not *)
                byte* namePtr, nameEnd;
                if (readName(ref data, dataEnd, out namePtr, out nameEnd)) { return new CSSSelector(buffer); }
                #endregion

                #region read the attributes
                LinkedList<CSSSelectorAttribute> attributes = new LinkedList<CSSSelectorAttribute>();
                
                //wait! is the character straight after the name a #/.? 
                //if so, the selector wants the element to have id/class
                #region element id/class
                if (*data == '.' || *data == '#') {
                    bool isClass = *data++ == '.';

                    //read the id/class value
                    byte* valPtr, valPtrEnd;
                    readName(ref data, dataEnd, out valPtr, out valPtrEnd);

                    //add an attribute to say to compare id/class attributes on elements
                    attributes.AddLast(new CSSSelectorAttribute(
                        (isClass ? "class" : "id"),
                        Helpers.ReadString(valPtr, valPtrEnd),
                        CSSSelectorAttributeCompareType.WhitespaceSplitMatch));
                }
                #endregion

                //read until we hit either a scope open or pseudo element/class character
                if (Helpers.SkipWhitespaces(ref data, dataEnd)) { return new CSSSelector(buffer); }
                if(*data == '[') {
                    while (data < dataEnd) {
                        if (*data == '{' ||
                            *data == ':') { break; }
                        
                        //attribute open?
                        if (*data == '[') {
                            data++;

                            #region read name
                            //skip to where the name begins
                            Helpers.SkipWhitespaces(ref data, dataEnd);
                            byte* attributeNamePtr, attributeNameEndPtr;
                            if (readName(ref data, dataEnd, out attributeNamePtr, out attributeNameEndPtr)) { break; }
                            #endregion

                            #region read comparison type
                            //read until we hit a =
                            while (data < dataEnd && *data != '=') { data++; }
                            
                            //deturmine what the type of comparison it is
                            CSSSelectorAttributeCompareType compareType = CSSSelectorAttributeCompareType.Match;
                            switch ((char)*(data - 1)) {
                                case '~': compareType = CSSSelectorAttributeCompareType.WhitespaceSplitMatch; break;
                                case '|': compareType = CSSSelectorAttributeCompareType.DashSplitPrefixMatch; break;
                                case '^': compareType = CSSSelectorAttributeCompareType.PrefixMatch; break;
                                case '$': compareType = CSSSelectorAttributeCompareType.SuffixMatch; break;
                                case '*': compareType = CSSSelectorAttributeCompareType.Contains; break;
                            }
                            data++;
                            #endregion

                            #region read value
                            
                            //read the value
                            Helpers.SkipWhitespaces(ref data, dataEnd);
                            byte* valuePtr, valuePtrEnd;
                            if (readStringValue(ref data, dataEnd, out valuePtr, out valuePtrEnd)) { break; }
                            #endregion

                            //read to the end of the attribute
                            while (data < dataEnd && *data != ']') { data++; }
                            data++;
                            
                            //add the attribute
                            attributes.AddLast(new CSSSelectorAttribute(
                                Helpers.ReadString(attributeNamePtr, attributeNameEndPtr),
                                Helpers.ReadString(valuePtr, valuePtrEnd),
                                compareType));
                        
                            //is there another attribute to read?
                            if (*data != '[') { break; }
                        }

                        data++;
                    }
                }
                #endregion

                #region read the pseudo element/class
                CSSSelectorPseudoClass pseudoClass = CSSSelectorPseudoClass.None;
                CSSSelectorPseudoElement pseudoElement = CSSSelectorPseudoElement.None;

                //pseudo?
                while (*data == ':') {
                    data++;

                    //::? if so, it's only a pseudo element
                    //HOWEVER, we check the class type anyway in case.
                    //we just check the element type first.
                    bool elementOnly = false;
                    if (*data == ':') {
                        elementOnly = true;
                        data++; 
                    }

                    //read the pseudo
                    byte* pseudoPtr, pseudoPtrEnd;
                    if (readName(ref data, dataEnd, out pseudoPtr, out pseudoPtrEnd)) { break; }

                    //process the name
                    bool found = false;
                    string pseudoName = Helpers.ReadString(pseudoPtr, pseudoPtrEnd);
                    pseudoName = pseudoName.Replace("-", "");
                    if (elementOnly) {
                        pseudoElement |= tryParseEnum<CSSSelectorPseudoElement>(pseudoName, out found);       
                    }

                    if (!found) {
                        pseudoClass |= tryParseEnum<CSSSelectorPseudoClass>(pseudoName, out found);
                        if (!found && !elementOnly) {
                            pseudoElement |= tryParseEnum<CSSSelectorPseudoElement>(pseudoName, out found);
                        }
                    }
                }

                #endregion

                #region read the pseudo arguments

                //read the arguments?
                if (*data == '(') {
                    data++;

                    //read the value
                    Helpers.SkipWhitespaces(ref data, dataEnd);
                    byte* argumentPtr, argumentPtrEnd;
                    if (readStringValue(ref data, dataEnd, out argumentPtr, out argumentPtrEnd)) { break; }

                    //go to the end of the argument scope
                    while (data < dataEnd && *data++ != ')') ;
                }

                #endregion

                //add the selector
                CSSSelectorType selectorType = new CSSSelectorType(
                    Helpers.ReadString(namePtr, nameEnd),
                    type,
                    attributes,
                    pseudoClass,
                    pseudoElement,
                    preRelation);
                buffer.AddLast(selectorType);

                //reset the pre-relation
                preRelation = CSSSelectorPreSelectorRelationship.GeneralChild;
            }

            return new CSSSelector(buffer);
        }

        private static bool isWhitespace(byte b) {
            return
                b == ' ' ||
                b == '\t' ||
                b == '\n' ||
                b == '\r' ||
                b == ']' ||
                b == '{' ||
                b == ')';
        }
        private static bool isNameChar(byte b) {
            return
                (b == '-' || b == '_' || b == '%') ||
                (b >= 'A' && b <= 'Z') ||
                (b >= 'a' && b <= 'z') ||
                (b >= '0' && b <= '9');
        }

        private static T tryParseEnum<T>(string value, out bool success) where T : struct {
            try { 
                T val = (T)Enum.Parse(typeof(T), value, true);
                success = true;
                return val;
            }
            catch {
                success = false;
                return default(T); 
            }
        }

        internal static bool readName(ref byte* data, byte* dataEnd, out byte* strPtr, out byte* strEnd) {
            //read the name
            strPtr = data;
            strEnd = (byte*)0;
            while (data < dataEnd) {
                if (!isNameChar(*data)) {
                    strEnd = data - 1;
                    return false;
                }
                data++;
            }
            return true;
        }
        internal static bool readStringValue(ref byte* data, byte* dataEnd, out byte* strPtr, out byte* strEnd) { 
            //we assume the pointer is at the beginning of the value
            byte terminate = *data;
            strEnd = (byte*)0;

            //none-string encapsulated?
            if (terminate != '"' && terminate != '\'') {
                strPtr = data;
                while (data < dataEnd) {
                    if (isWhitespace(*data)) {
                        strEnd = data - 1;
                        return false;;
                    }
                    data++;
                }
                return true;
            }

            //read through the data
            data++; strPtr = data;
            while (data < dataEnd) { 
                //terminate?
                if (*data == terminate) {
                    strEnd = data - 1;
                    data++;
                    return false;
                }

                //ignore next character?
                if (*data == '\\') {
                    data += 2;
                    continue;
                }

                data++;
            }

            return true;
        }
        #endregion
        
        public override string ToString() {
            string buffer = "";
            
            //get the array of types
            CSSSelectorType[] types = Helpers.LinkedListToArray(p_Types);
            
            //add each selector and it's pre-relation character 
            for (int c = 0; c < types.Length; c++) {
                if (types[c].PreSelectorRelationship == CSSSelectorPreSelectorRelationship.NamespaceChild) {
                    buffer += "|";
                }

                if (c != 0) {
                    switch (types[c].PreSelectorRelationship) {
                        case CSSSelectorPreSelectorRelationship.GeneralChild: buffer += " "; break;
                        case CSSSelectorPreSelectorRelationship.GeneralSibling: buffer += " ~ "; break;
                        case CSSSelectorPreSelectorRelationship.ImmidiateChild: buffer += " > "; break;
                        case CSSSelectorPreSelectorRelationship.AdjacentSibling: buffer += " + "; break;
                    }
                }

                buffer += types[c].ToString();
            }


            return buffer;
        }
    }
}