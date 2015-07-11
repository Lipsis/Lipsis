using System;
using System.IO;
using System.Collections.Generic;

namespace Lipsis.Languages.Markup.HTML {
    public class HTMLDocument : MarkupDocument {
        private static LinkedList<string> p_NoScopeTags;
        private static LinkedList<string> p_TextTags;
        static HTMLDocument() { 
            //initialize the no scope tags and text tags list
            p_NoScopeTags = new LinkedList<string>();
            p_TextTags = new LinkedList<string>();

            //add the default tag names for tags with no scopes and text tags
            string[] noScopeTags = { 
                "meta"                       
            };
            string[] textTags = { 
                "title",
                "script",
                "style",
                "a",
                "pre"
            };
            foreach (string s in noScopeTags) { p_NoScopeTags.AddLast(s); }
            foreach (string s in textTags) { p_TextTags.AddLast(s); }           
        }

        #region Wrapper constructors for MarkupDocument
        public HTMLDocument() : base() { }
        public HTMLDocument(string data) : base(data, "span", p_TextTags, p_NoScopeTags) { }
        public HTMLDocument(byte[] data) : base(data, "span", p_TextTags, p_NoScopeTags) { }
        public unsafe HTMLDocument(byte* data, int length) : base(data, length, "span", p_TextTags, p_NoScopeTags) { }
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
            return new HTMLDocument(File.ReadAllBytes(filename));
        }
    }
}