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
            switch (p_Type) {
                case CSSSelectorElementTargetType.All: buffer = "*"; break;
                case CSSSelectorElementTargetType.Class: buffer = "."; break;
                case CSSSelectorElementTargetType.ID: buffer = "#"; break;
            }

            buffer += p_Query;

            CSSSelectorAttribute[] attributes = Helpers.LinkedListToArray(p_Attributes);
            buffer += Helpers.FlattenToString(attributes, "");
            return buffer;
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