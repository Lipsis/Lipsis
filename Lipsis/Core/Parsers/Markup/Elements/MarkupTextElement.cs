using System;

namespace Lipsis.Core {
    public sealed class MarkupTextElement : MarkupElement {
        internal MarkupTextElement(string tagName, string text) : base(tagName) {
            Text = text;
        }

        public string Text { get; set; }
    }
}