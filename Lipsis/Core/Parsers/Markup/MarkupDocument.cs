using System;
using System.IO;
using System.Collections.Generic;

namespace Lipsis.Core {
    public class MarkupDocument {
        private MarkupElement p_Root;


        public MarkupDocument() {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
        }

        #region Wrapper constructors to ParseMarkup
        public MarkupDocument(string data, string textTagName, LinkedList<string> textTags) : this() {
            Parsers.ParseMarkup(data, textTagName, textTags, p_Root);
        }
        public MarkupDocument(byte[] data, string textTagName, LinkedList<string> textTags) : this() {
            Parsers.ParseMarkup(data, textTagName, textTags, p_Root);
        }
        public unsafe MarkupDocument(byte* data, int length, string textTagName, LinkedList<string> textTags) : this() {
            Parsers.ParseMarkup(data, length, textTagName, textTags, p_Root);
        }
        #endregion

        #region Wrapper functions to MarkupElement
        public MarkupElement Add(string name) {
            return
                p_Root.AddChild(new MarkupElement(name)) as MarkupElement;
        } 
        public void Remove(MarkupElement element) {
            p_Root.RemoveChild(element);
        }


        public LinkedList<MarkupElement> Find(string tagName) {
            return p_Root.Find(tagName);
        }
        public LinkedList<MarkupElement> Find(string query, bool matchCase) {
            return p_Root.Find(query, matchCase);
        }
        public LinkedList<MarkupElement> Find(string query, bool matchCase, bool matchWhole) { 
            return p_Root.Find(query, matchCase, matchWhole);
        }
        public LinkedList<MarkupElement> Find(string query, bool deepSearch, bool matchCase, bool matchWhole) { 
            return p_Root.Find(query, deepSearch, matchCase, matchWhole);
        }

        public LinkedList<MarkupElement> Find(string[] query, MarkupElement.FindMode mode) {
            return p_Root.Find(query, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, deepSearch, matchCase, matchWhole, mode);
        }
        #endregion

        public LinkedList<MarkupElement> GetElementsByTagName(string tagName) { return Find(tagName); }
        public LinkedList<MarkupElement> GetElementsById(string id) {
            return getElementsByAttrId("id", id);
        }
        public LinkedList<MarkupElement> GetElementsByClassName(string className) {
            return getElementsByAttrId("class", className);
        }

        public MarkupElement GetElementByTagName(string tagName) {
            LinkedList<MarkupElement> result = Find(tagName);
            if (result.Count == 0) { return null; }
            return result.Last.Value;
        }
        public MarkupElement GetElementById(string id) {
            LinkedList<MarkupElement> result = GetElementsById(id);
            if (result.Count == 0) { return null; }
            return result.Last.Value;
        }
        public MarkupElement GetElementByClassName(string className) {
            LinkedList<MarkupElement> result = GetElementsByClassName(className);
            if (result.Count == 0) { return null; }
            return result.Last.Value;
        }


        private LinkedList<MarkupElement> getElementsByAttrId(string attr, string value) { 
            //get all elements with the attribute name
            LinkedList<MarkupElement> result = Find(
                new string[] { attr, value },
                false,
                true,
                MarkupElement.FindMode.Attribute);

            //as it's an id type value, we must match case on it.
            LinkedList<MarkupElement> buffer = new LinkedList<MarkupElement>();
            IEnumerator<MarkupElement> e = result.GetEnumerator();
            while (e.MoveNext()) { 
                //get the attribute for the id
                string cid = e.Current[attr];
                if (cid == value) {
                    buffer.AddLast(e.Current);
                }
            }

            //clean up
            e.Dispose();
            return buffer;
        }

        public LinkedList<MarkupElement> Elements {
            get {
               return Helpers.ConvertLinkedListType<MarkupElement, Node>(p_Root.Children);
            }
        }
        public MarkupElement Root { get { return p_Root; } }

        public static MarkupDocument FromFile(string filename, string textTagName, LinkedList<string> textTags) {
            return new MarkupDocument(
                File.ReadAllBytes(filename),
                textTagName,
                textTags);
        }
       
    }
}