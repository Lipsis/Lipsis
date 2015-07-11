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

        private CSSSelectorPseudoClass p_PseudoClass;
        private CSSSelectorPseudoElement p_PseudoElement;

        internal CSSSelectorType(string query, 
                                 CSSSelectorElementTargetType type, 
                                 LinkedList<CSSSelectorAttribute> attributes,
                                 CSSSelectorPseudoClass pseudoClass,
                                 CSSSelectorPseudoElement pseudoElement,
                                 CSSSelectorPreSelectorRelationship relationship) {
            p_Query = query;
            p_Type = type;
            p_Attributes = attributes;
            p_PseudoElement = pseudoElement;
            p_PseudoClass = pseudoClass;
            p_ParentRelationship = relationship;
                                 }

        public string Query { get { return p_Query; } }

        public CSSSelectorPseudoClass PseudoClass { get { return p_PseudoClass; } }
        public CSSSelectorPseudoElement PseudoElement { get { return p_PseudoElement; } }

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
    }
}