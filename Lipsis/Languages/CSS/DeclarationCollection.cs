using System;
using System.Collections;
using System.Collections.Generic;

namespace Lipsis.Languages.CSS {

    public class CSSDeclarationCollection : ICSSScope, IEnumerable {
        private LinkedList<CSSDeclaration> p_Declarations;

        public CSSDeclarationCollection() {
            p_Declarations = new LinkedList<CSSDeclaration>();
        }
        public CSSDeclarationCollection(LinkedList<CSSDeclaration> declarations) {
            p_Declarations = declarations;
        }

        public void AddDeclarations(CSSDeclarationCollection declarations) {
            AddDeclarations(declarations.p_Declarations);
        }
        public void AddDeclarations(LinkedList<CSSDeclaration> declarations) {
            IEnumerator<CSSDeclaration> e = declarations.GetEnumerator();
            while (e.MoveNext()) {
                p_Declarations.AddLast(e.Current);
            }
            e.Dispose(); ;
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

        public LinkedList<CSSDeclaration> Declarations { get { return p_Declarations; } }

        public IEnumerator GetEnumerator() {
            return p_Declarations.GetEnumerator();
        }
    }
}