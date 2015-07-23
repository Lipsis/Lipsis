using System;
using System.Text;
using System.Collections.Generic;

using Lipsis.Core;
using Lipsis.Languages.Markup;

namespace Lipsis.Languages.CSS {
    public struct CSSSelectorType {
        private string p_Query;
        private CSSSelectorElementTargetType p_Type;
        private LinkedList<CSSSelectorAttribute> p_Attributes;
        private CSSSelectorPreSelectorRelationship p_ParentRelationship; 

        private CSSPseudoClass p_PseudoClass;
        private CSSPseudoElement p_PseudoElement;

        private LinkedList<pseudoClassWithArg> p_PseudoClassArguments;

        internal CSSSelectorType(string query, 
                                 CSSSelectorElementTargetType type, 
                                 LinkedList<CSSSelectorAttribute> attributes,
                                 CSSPseudoClass pseudoClass,
                                 CSSPseudoElement pseudoElement,
                                 CSSSelectorPreSelectorRelationship relationship,
                                 LinkedList<pseudoClassWithArg> pseudoClassArguments) {
            p_Query = query;
            p_Type = type;
            p_Attributes = attributes;
            p_PseudoElement = pseudoElement;
            p_PseudoClass = pseudoClass;
            p_ParentRelationship = relationship;
            p_PseudoClassArguments = pseudoClassArguments;
        }

        public string Query { get { return p_Query; } }

        public bool HasClass(CSSPseudoClass compare) {
            return (p_PseudoClass & compare) == compare;
        }
        public bool HasElement(CSSPseudoElement compare) {
            return (p_PseudoElement & compare) == compare;
        }

        public CSSPseudoClass PseudoClass { get { return p_PseudoClass; } }
        public CSSPseudoElement PseudoElement { get { return p_PseudoElement; } }

        public CSSSelectorPreSelectorRelationship PreSelectorRelationship { get { return p_ParentRelationship; } }

        public CSSSelectorElementTargetType TargetType { get { return p_Type; } }
        public bool IsAll { get { return p_Type == CSSSelectorElementTargetType.All; } }
        public bool IsTagType { get { return p_Type == CSSSelectorElementTargetType.Tag; } }
        public bool IsClassType { get { return p_Type == CSSSelectorElementTargetType.Class; } }
        public bool IsIDType { get { return p_Type == CSSSelectorElementTargetType.ID; } }
       
        public override string ToString() {
            string buffer = "";

            //add the type
            switch (p_Type) {
                case CSSSelectorElementTargetType.All: buffer = "*"; break;
                case CSSSelectorElementTargetType.Class: buffer = "."; break;
                case CSSSelectorElementTargetType.ID: buffer = "#"; break;
            }

            buffer += p_Query;

            //add the attributes
            CSSSelectorAttribute[] attributes = Helpers.LinkedListToArray(p_Attributes);
            buffer += Helpers.FlattenToString(attributes, "");

            #region add the pseudo classes
            long[] pseudoClassMatches = detectEnumValues(
                (long)p_PseudoClass,
                1,
                (long)CSSPseudoClass._LIP_MAX);
            int pseudoClassMatchesCount = pseudoClassMatches.Length;
            for (int c = 0; c < pseudoClassMatchesCount; c++) {
                long value = pseudoClassMatches[c];
                
                //add the string value
                buffer += ":" + getPseudoClassString((CSSPseudoClass)value);

                //require arguments?
                if (value >= (long)CSSPseudoClass._LIP_ARG_MIN_VALUE &&
                   value <= (long)CSSPseudoClass._LIP_ARG_MAX_VALUE) {
                       CSSPseudoClass cls = (CSSPseudoClass)value;

                        //look for the argument
                        IEnumerator<pseudoClassWithArg> e = p_PseudoClassArguments.GetEnumerator();
                        while (e.MoveNext()) {
                            pseudoClassWithArg current = e.Current;
                            if (current.cls != cls) { continue; }
                            buffer += current.argument;
                            break;
                        }
                        e.Dispose();
                }
            }
            #endregion

            #region add the pseudo elements
            long[] pseudoElementMatches = detectEnumValues(
                (long)p_PseudoElement,
                1,
                (long)CSSPseudoElement._LIP_MAX);
            int pseudoElementMatchesCount = pseudoElementMatches.Length;
            for (int c = 0; c < pseudoElementMatchesCount; c++) {
                CSSPseudoElement value = (CSSPseudoElement)pseudoElementMatches[c];
                buffer += "::" + getPseudoElementString(value);
            }
            #endregion

            return buffer;
        }

        private long[] detectEnumValues(long value, long min, long max) {
            long[] buffer = new long[0];

            //cycle through the min/max and look for any 
            //power of 2 that is inside the value.
            for (long c = min; c != max; c <<= 1) {
                if ((value & c) == c) {
                    Array.Resize(ref buffer, buffer.Length + 1);
                    buffer[buffer.Length - 1] = c;
                }
            }

            return buffer;
        }
        private string getPseudoClassString(CSSPseudoClass cls) {
            switch (cls) {
                case CSSPseudoClass.FirstChild: return "first-child";
                case CSSPseudoClass.FirstOfType: return "first-of-type";
                case CSSPseudoClass.InRange: return "in-range";
                case CSSPseudoClass.LastChild: return "last-child";
                case CSSPseudoClass.LastOfType: return "last-of-type";
                case CSSPseudoClass.NthChild: return "nth-child";
                case CSSPseudoClass.NthLastChild: return "nth-last-child";
                case CSSPseudoClass.NthLastOfType: return "nth-last-of-type";
                case CSSPseudoClass.NthOfType: return "nth-of-type";
                case CSSPseudoClass.OnlyChild: return "only-child";
                case CSSPseudoClass.OnlyOfType: return "only-of-type";
                case CSSPseudoClass.OutOfRange: return "out-of-range";
                case CSSPseudoClass.ReadOnly: return "read-only";
                case CSSPseudoClass.ReadWrite: return "read-write";
                
                default:
                    return cls.ToString().ToLower();
            }
        }
        private string getPseudoElementString(CSSPseudoElement element) {
            switch (element) {
                case CSSPseudoElement.FirstLetter: return "first-letter";
                case CSSPseudoElement.FirstLine: return "first-line";
                default:
                    return element.ToString().ToLower();
            }
        }

        internal struct pseudoClassWithArg {
            public pseudoClassWithArg(CSSPseudoClass c, ICSSPseudoClassArgument a) {
                cls = c;
                argument = a;
            }

            public CSSPseudoClass cls;
            public ICSSPseudoClassArgument argument;
        }
    }
}