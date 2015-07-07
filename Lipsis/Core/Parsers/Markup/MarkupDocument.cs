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
        public MarkupDocument(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parsers.ParseMarkup(data, textTagName, textTags, noScopeTags, p_Root);
            OnDocumentLoaded();
        }
        public MarkupDocument(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parsers.ParseMarkup(data, textTagName, textTags, noScopeTags, p_Root);
            OnDocumentLoaded();
        }
        public unsafe MarkupDocument(byte* data, int length, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parsers.ParseMarkup(data, length, textTagName, textTags, noScopeTags, p_Root);
            OnDocumentLoaded();
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
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, mode);
        }

        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, bool[] matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, bool[] matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, matchCase, matchWhole, mode);
        }

        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, deepSearch, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool[] matchCase, bool matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, deepSearch, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool matchCase, bool[] matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, deepSearch, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool[] matchCase, bool[] matchWhole, MarkupElement.FindMode mode) {
            return p_Root.Find(query, deepSearch, matchCase, matchWhole, mode);
        }
        #endregion

        public LinkedList<MarkupElement> GetElementsByTagName(string tagName) { return Find(tagName); }
        public LinkedList<MarkupElement> GetElementsById(string id) {
            return Find(
                new string[] { "id", id },
                new bool[] { false, true },
                new bool[] { true, true },
                MarkupElement.FindMode.Attribute);
        }
        public LinkedList<MarkupElement> GetElementsByClassName(string className) {
            //since an element can have multiple classes, allow for a multi-class search
            string[] classNames = className.Split(' ');
            int classNamesLength = classNames.Length;

            //get all the elements with a class attribute
            LinkedList<MarkupElement> classedElements = Find(
                new string[] { "class" },
                new bool[] { false },
                new bool[] { true },
                MarkupElement.FindMode.AttributeName);

            //return all elements with a class?
            if (className == "*") { return classedElements; }

            //grab all the elements with ALL of the classes specified
            LinkedList<MarkupElement> buffer = new LinkedList<MarkupElement>();
            IEnumerator<MarkupElement> e = classedElements.GetEnumerator();
            while (e.MoveNext()) {
                string[] compare = e.Current["class"].Split(' ');
                int compareLength = compare.Length;

                //count how many names match the class name of this element
                int match = 0;
                for (int c = 0; c < classNamesLength; c++) {
                    if (Array.IndexOf(compare, classNames[c]) != -1) {
                        match++;

                        if (match == classNamesLength) { break; }
                    }
                }

                //add it? (element has all the classes we want)
                if (match == classNamesLength) {
                    buffer.AddLast(e.Current);
                }
            }

            //clean up
            e.Dispose();
            return buffer;

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

        protected virtual void OnDocumentLoaded() { }
        
        public LinkedList<MarkupElement> Elements {
            get {
               return Helpers.ConvertLinkedListType<MarkupElement, Node>(p_Root.Children);
            }
        }
        public MarkupElement Root { get { return p_Root; } }

        public static MarkupDocument FromFile(string filename, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            //read the file
            byte[] data = File.ReadAllBytes(filename);

            MarkupDocument doc = new MarkupDocument(
                data,
                textTagName,
                textTags,
                noScopeTags);
            data = null;
            return doc;
        }
    }
}