using System;
using System.Text;
using System.Collections.Generic;

using Lipsis.Core;
using Lipsis.Languages.Markup;

namespace Lipsis.Languages.CSS {
    public sealed unsafe class CSSSelector {
        private LinkedList<CSSSelectorType> p_Types;
        private CSSSelector(LinkedList<CSSSelectorType> types) {
            p_Types = types;
        }

        public LinkedList<CSSSelectorType> Types { get { return p_Types; } }

        #region Parsing
        public static CSSSelector Parse(string data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSSelector Parse(byte[] data) {
            return Parse(data, Encoding.ASCII);
        }
        public static CSSSelector Parse(ref byte* data, int length) {
            return Parse(ref data, length, Encoding.ASCII);
        }
        public static CSSSelector Parse(ref byte* data, byte* dataEnd) {
            return Parse(ref data, dataEnd, Encoding.ASCII);
        }

        public static CSSSelector Parse(string data, Encoding encoder) {
            return Parse(encoder.GetBytes(data), encoder);
        }
        public static CSSSelector Parse(byte[] data, Encoding encoder) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, ptr + data.Length, encoder);
            }
        }
        public static CSSSelector Parse(ref byte* data, int length, Encoding encoder) {
            return Parse(ref data, data + length, encoder);
        }
        public static CSSSelector Parse(ref byte* data, byte* dataEnd, Encoding encoder) { 
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
                readName(ref data, dataEnd, out namePtr, out nameEnd);
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
                        Helpers.ReadString(valPtr, valPtrEnd, encoder),
                        CSSSelectorAttributeCompareType.WhitespaceSplitMatch));
                }
                #endregion

                //read until we hit either a scope open or pseudo element/class character
                Helpers.SkipWhitespaces(ref data, dataEnd);
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
                            readName(ref data, dataEnd, out attributeNamePtr, out attributeNameEndPtr);
                            
                            //just name ([name])
                            if (*data == ']') { 
                                data++;

                                attributes.AddLast(new CSSSelectorAttribute(
                                    Helpers.ReadString(attributeNamePtr, attributeNameEndPtr, encoder),
                                    null,
                                    CSSSelectorAttributeCompareType.HasAttribute));
                                continue; 
                            }
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
                            readStringValue(ref data, dataEnd, out valuePtr, out valuePtrEnd);
                            #endregion

                            //read to the end of the attribute
                            while (data < dataEnd && *data != ']') { data++; }
                            data++;
                            
                            //add the attribute
                            attributes.AddLast(new CSSSelectorAttribute(
                                Helpers.ReadString(attributeNamePtr, attributeNameEndPtr, encoder),
                                Helpers.ReadString(valuePtr, valuePtrEnd, encoder),
                                compareType));
                        
                            //is there another attribute to read?
                            if (*data != '[') { break; }
                        }

                        data++;
                    }
                }
                #endregion

                #region read the pseudo element/class
                CSSPseudoClass pseudoClass = CSSPseudoClass.None;
                CSSPseudoElement pseudoElement = CSSPseudoElement.None;
                LinkedList<CSSSelectorType.pseudoClassWithArg> pseudoClassArguments = new LinkedList<CSSSelectorType.pseudoClassWithArg>();

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
                    readName(ref data, dataEnd, out pseudoPtr, out pseudoPtrEnd);

                    //process the name
                    bool found = false;
                    bool isClass = false;
                    CSSPseudoClass classType = CSSPseudoClass.None;
                    string pseudoName = Helpers.ReadString(pseudoPtr, pseudoPtrEnd, encoder);
                    pseudoName = pseudoName.ToLower();
                    if (elementOnly) {
                        pseudoElement |= parsePseudoElement(pseudoName, out found);      
                    }

                    if (!found) {
                        classType = parsePseudoClass(pseudoName, out found);
                        pseudoClass |= classType;
                        if (found) { isClass = true; }
                        if (!found && !elementOnly) {
                            pseudoElement |= parsePseudoElement(pseudoName, out found);
                        }

                    }
                    
                    #region read the pseudo arguments
                    //read the arguments?
                    if (isClass && *data == '(') {
                        data++;

                        //read the value
                        Helpers.SkipWhitespaces(ref data, dataEnd);
                        byte* argumentPtr, argumentPtrEnd;
                        readStringValue(ref data, dataEnd, out argumentPtr, out argumentPtrEnd);

                        //go to the end of the argument scope
                        while (data < dataEnd && *data++ != ')') ;

                        #region parse the argument data
                        ICSSPseudoClassArgument arg = null;
                        bool addArg = false;

                        //nth?
                        if (classType >= CSSPseudoClass._LIP_NTH_MIN &&
                            classType <= CSSPseudoClass._LIP_NTH_MAX) {
                                arg = new CSSPseudoClass_Nth(argumentPtr, argumentPtrEnd + 1, classType, out addArg);
                        }
                        //not?
                        else if (classType == CSSPseudoClass.Not) {
                            arg = new CSSPseudoClass_Not(argumentPtr, argumentPtrEnd + 1, encoder);
                            addArg = true;
                        }
                        

                        #endregion

                        //add the argument
                        if (addArg) {
                            pseudoClassArguments.AddLast(new CSSSelectorType.pseudoClassWithArg(
                                classType,
                                arg));
                        }
                    }
                    #endregion
                }

                #endregion

                //add the selector
                CSSSelectorType selectorType = new CSSSelectorType(
                    Helpers.ReadString(namePtr, nameEnd, encoder),
                    type,
                    attributes,
                    pseudoClass,
                    pseudoElement,
                    preRelation,
                    pseudoClassArguments);
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

        private static CSSPseudoElement parsePseudoElement(string value, out bool success) {
            success = true;
            switch (value) { 
                case "after": return CSSPseudoElement.After;
                case "before": return CSSPseudoElement.Before;
                case "first-letter": return CSSPseudoElement.FirstLetter;
                case "first-line": return CSSPseudoElement.FirstLine;
                case "selection": return CSSPseudoElement.Selection;
                case "backdrop": return CSSPseudoElement.Backdrop;

                default:
                    success = false;
                    return CSSPseudoElement.None;
            }

        }
        private static CSSPseudoClass parsePseudoClass(string value, out bool success) {
            success = true;
            switch (value) {
                case "default": return CSSPseudoClass.Default;
                case "active": return CSSPseudoClass.Active;
                case "checked": return CSSPseudoClass.Checked;
                case "disabled": return CSSPseudoClass.Disabled;
                case "empty": return CSSPseudoClass.Empty;
                case "enabled": return CSSPseudoClass.Enabled;
                case "first": return CSSPseudoClass.First;
                case "first-child": return CSSPseudoClass.FirstChild;
                case "first-of-type": return CSSPseudoClass.FirstOfType;
                case "fullscreen": return CSSPseudoClass.FullScreen;
                case "focus": return CSSPseudoClass.Focus;
                case "hover": return CSSPseudoClass.Hover;
                case "indeterminate": return CSSPseudoClass.Indeterminate;
                case "in-range": return CSSPseudoClass.InRange;
                case "invalid": return CSSPseudoClass.Invalid;
                case "last-child": return CSSPseudoClass.LastChild;
                case "last-of-type": return CSSPseudoClass.LastOfType;
                case "left": return CSSPseudoClass.Left;
                case "link": return CSSPseudoClass.Link;
                case "only-child": return CSSPseudoClass.OnlyChild;
                case "only-of-type": return CSSPseudoClass.OnlyOfType;
                case "optional": return CSSPseudoClass.Optional;
                case "out-of-range": return CSSPseudoClass.OutOfRange;
                case "read-only": return CSSPseudoClass.ReadOnly;
                case "read-write": return CSSPseudoClass.ReadWrite;
                case "required": return CSSPseudoClass.Required;
                case "right": return CSSPseudoClass.Right;
                case "root": return CSSPseudoClass.Root;
                case "scope": return CSSPseudoClass.Scope;
                case "target": return CSSPseudoClass.Target;
                case "valid": return CSSPseudoClass.Valid;
                case "visited": return CSSPseudoClass.Visited;

                case "dir": return CSSPseudoClass.Dir;
                case "lang": return CSSPseudoClass.Lang;
                case "not": return CSSPseudoClass.Not;
                case "nth-child": return CSSPseudoClass.NthChild;
                case "nth-last-child": return CSSPseudoClass.NthLastChild;
                case "nth-last-of-type": return CSSPseudoClass.NthLastOfType;
                case "nth-of-type": return CSSPseudoClass.NthOfType;

                default:
                    success = false;
                    return CSSPseudoClass.None;
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
            strEnd = dataEnd - 1;
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