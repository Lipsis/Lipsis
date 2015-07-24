
using Lipsis.Core;

namespace Lipsis.Languages.Markup {
    public interface IMarkupElementFactory {


        MarkupElement Create(string tagName);
        MarkupTextElement CreateText(string tagName, string text);


    }

    public sealed class MarkupStandardElementFactory : IMarkupElementFactory {
        private static MarkupStandardElementFactory p_Instance;
        public static MarkupStandardElementFactory Instance { get { return p_Instance; } }

        static MarkupStandardElementFactory() {
            p_Instance = new MarkupStandardElementFactory();
        }
        private MarkupStandardElementFactory() { }


        public MarkupElement Create(string tagName) {
            return new MarkupElement(tagName);
        }
        public MarkupTextElement CreateText(string tagName, string text) {
            return new MarkupTextElement(tagName, text);
        }

    }
}