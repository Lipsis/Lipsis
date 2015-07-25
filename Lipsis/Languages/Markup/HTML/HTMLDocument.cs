using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Lipsis.Languages.Markup.HTML {
    public class HTMLDocument : MarkupDocument {
        private static List<string> p_NoScopeTags;
        private static List<string> p_TextTags;
        static HTMLDocument() { 
            //add the default tag names for tags with no scopes and text tags
            string[] noScopeTags = { 
                "meta",
                "link",
                "input"
            };
            string[] textTags = { 
                "title",
                "script",
                "style",
                "pre"
            };
            p_NoScopeTags = new List<string>(noScopeTags);
            p_TextTags = new List<string>(textTags);         
        }

        #region Wrapper constructors for MarkupDocument
        public HTMLDocument() : base() { }

        public HTMLDocument(string data) : this(data, Encoding.ASCII) { }
        public HTMLDocument(byte[] data) : this(data, Encoding.ASCII) { }
        public unsafe HTMLDocument(byte* data, int length) : this(data, length, Encoding.ASCII) { }

        public HTMLDocument(string data, Encoding encoder) : base(data, "span", p_TextTags, p_NoScopeTags, MarkupStandardElementFactory.Instance, encoder) { }
        public HTMLDocument(byte[] data, Encoding encoder) : base(data, "span", p_TextTags, p_NoScopeTags, MarkupStandardElementFactory.Instance, encoder) { }
        public unsafe HTMLDocument(byte* data, int length, Encoding encoder) : base(data, length, "span", p_TextTags, p_NoScopeTags, MarkupStandardElementFactory.Instance, encoder) { }
        #endregion

        public string Title {
            get {                
                //is there a title tag?
                MarkupElement titleTag = GetElementByTagName("title");
                if (titleTag == null) { return null; }

                //return the title value
                return (titleTag as MarkupTextElement).Text;
            }
        }

        public MarkupElement Header {
            get {
                return GetElementByTagName("head");
            }
        }
        public MarkupElement Body {
            get {
                return GetElementByTagName("body");
            }
        }
        public LinkedList<MarkupElement> Styles {
            get { return GetElementsByTagName("style"); }
        }
        public LinkedList<MarkupElement> Scripts {
            get { return GetElementsByTagName("script"); }
        }

        protected override void OnDocumentLoaded() {
            base.OnDocumentLoaded();
        }


        public static HTMLDocument FromFile(string filename) {
            return FromFile(filename, Encoding.ASCII);
        }
        public static HTMLDocument FromFile(string filename, Encoding encoder) {
            return new HTMLDocument(File.ReadAllBytes(filename), encoder);
        }
    }
}