using System;

using Lipsis.Core;

namespace Lipsis.Languages.Markup {
    public sealed class MarkupTextElement : MarkupElement {
        internal MarkupTextElement(string tagName, string text) : base(tagName) {
            Text = text;
        }

        public string Text { get; set; }

        protected override Node CloneCreateNode(Node original) {
            MarkupTextElement o = original as MarkupTextElement;
            return new MarkupTextElement(
                TagName,
                Text);
        }
    }
}