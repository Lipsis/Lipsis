using Lipsis.Languages.Markup;


namespace Lipsis.Languages.CSS {
    public interface ICSSSelectable {
        bool AppliesTo(MarkupElement originalElement, MarkupElement pipeElement);
    }
}