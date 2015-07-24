using System.Text;
using Lipsis.Languages.Markup;

namespace Lipsis.Languages.CSS {
    internal sealed unsafe class CSSPseudoClass_Not : ICSSPseudoClassArgument {
        private CSSSelector p_Selector;

        public CSSPseudoClass_Not(byte* data, byte* dataEnd, Encoding encoder) {
            p_Selector = CSSSelector.Parse(ref data, dataEnd, encoder);
        }

        public bool AppliesTo(MarkupElement originalElement, MarkupElement pipeElement) {
            return false;
        }

        public override string ToString() {
            return "(" + p_Selector.ToString() + ")";
        }
    }
}