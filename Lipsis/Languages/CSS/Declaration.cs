using System;
using System.Collections.Generic;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public sealed class CSSDeclaration {
        private LinkedList<CSSSelector> p_Selectors;
        private CSSRuleSet p_Ruleset;

        public CSSDeclaration() {
            p_Ruleset = new CSSRuleSet();
            p_Selectors = new LinkedList<CSSSelector>();
        }
        public CSSDeclaration(LinkedList<CSSSelector> selectors) {
            p_Selectors = selectors;
            p_Ruleset = new CSSRuleSet();
        }
        public CSSDeclaration(CSSRuleSet ruleset) {
            p_Selectors = new LinkedList<CSSSelector>();
            p_Ruleset = ruleset;
        }
        public CSSDeclaration(LinkedList<CSSSelector> selectors, CSSRuleSet ruleset) {
            p_Selectors = selectors;
            p_Ruleset = ruleset;
        }

        public void AddSelector(CSSSelector selector) {
            p_Selectors.AddLast(selector);
        }
        public bool RemoveSelector(CSSSelector selector) {
            return p_Selectors.Remove(selector);
        }

        public CSSRule AddRule(string name, string value) {
            return p_Ruleset.AddRule(name, value);
        }
        public bool RemoveRule(string name) {
            return p_Ruleset.RemoveRule(name);
        }

        public LinkedList<CSSSelector> Selectors { get { return p_Selectors; } }
        public CSSRuleSet RuleSet { get { return p_Ruleset; } }

        public override string ToString() {
            CSSSelector[] selectors = Helpers.LinkedListToArray(p_Selectors);
            return Helpers.FlattenToString(selectors, ", ");
        }
    }
}