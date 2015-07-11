using System;
using System.IO;
using System.Collections.Generic;

namespace Lipsis.Languages.Markup.XML {
    public class XMLDocument : MarkupDocument {
        private Version p_Version;
        
        #region Wrapper constructors for MarkupDocument

        public XMLDocument(string data) : base(data, "span", new LinkedList<string>(), new LinkedList<string>()) { }
        public XMLDocument(byte[] data) : base(data, "span", new LinkedList<string>(), new LinkedList<string>()) { }
        public unsafe XMLDocument(byte* data, int length) : base(data, length, "span", new LinkedList<string>(), new LinkedList<string>()) { }

        #endregion

        public Version Version { get { return p_Version; } }

        protected override void OnDocumentLoaded() {
            base.OnDocumentLoaded();

            //declare the element which includes the information 
            //about this document.
            MarkupElement descriptor = null;

            //the first node in the tree MUST be a XML descriptor (?xml)
            //we also remove this descriptor so the caller only see's the
            //xml content
            bool valid = true;
            LinkedList<MarkupElement> children = Elements;
            if (children.Count == 0) { valid = false; }
            else {
                descriptor = children.First.Value;
                descriptor.Remove();
                if (descriptor.TagName.ToLower() != "?xml") { valid = false; }
            }
            if (!valid) {
                throw new Exception("No XML descriptor found!");
            }

            //get the version information
            string versionStr = descriptor["version"];
            if (versionStr == null) {
                throw new Exception("No XML version specified!");
            }
            p_Version = new Version(versionStr);
            
        }
        public static XMLDocument FromFile(string filename) {
            return new XMLDocument(File.ReadAllBytes(filename));
        }
    }
}