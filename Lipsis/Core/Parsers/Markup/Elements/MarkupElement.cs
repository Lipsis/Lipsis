using System;
using System.Collections.Generic;


namespace Lipsis.Core {
    public class MarkupElement : Node {

        private string p_TagName;
        private LinkedList<MarkupAttribute> p_Attributes;

        internal MarkupElement(string tagName) {
            p_TagName = tagName;
            p_Attributes = new LinkedList<MarkupAttribute>();
        }
        
        public string TagName { get { return p_TagName; } }
        public LinkedList<MarkupAttribute> Attributes { get { return Helpers.CloneLinkedList(p_Attributes, null); } }

        public MarkupAttribute AddAttribute(string name, string value) { 
            //does it already exist? if so, just set its value
            MarkupAttribute? ret = GetAttribute(name);
            MarkupAttribute buffer = default(MarkupAttribute);
            if (ret.HasValue) {
                buffer = ret.Value;
                buffer.Value = value;
                p_Attributes.Find(ret.Value).Value = buffer;
            }

            //create the new element
            buffer = new MarkupAttribute(name, value);
            p_Attributes.AddLast(buffer);
            return buffer;
        }
        public void RemoveAttribute(string name) { 
            //look for the html attribute with the name
            MarkupAttribute? ret = GetAttribute(name);
            if (!ret.HasValue) { return; }

            //remove it
            p_Attributes.Remove(ret.Value);
        }

        public bool HasAttribute(string name) {
            return GetAttribute(name).HasValue;
        }
        public MarkupAttribute? GetAttribute(string name) { 
            //convert the name to lower case so we can 
            //match case-insensitivly
            name = name.ToLower();

            //look for the attribute
            IEnumerator<MarkupAttribute> e = p_Attributes.GetEnumerator();
            while (e.MoveNext()) {
                if (e.Current.Name.ToLower() == name) { 
                    //clean up
                    MarkupAttribute a = e.Current;
                    e.Dispose();
                    return a;
                }
            }

            //not found
            return null;
        }

        public string this[string attributeName] {
            get {
                return GetAttribute(attributeName).Value.Value;
            }
        }

        /*
            Search functions
        */
        #region Search functions
        public LinkedList<MarkupElement> Find(string tagName) {
            return Find(tagName, false);
        }
        public LinkedList<MarkupElement> Find(string tagName, bool matchCase) {
            return Find(tagName, matchCase, true);
        }
        public LinkedList<MarkupElement> Find(string tagName, bool matchCase, bool matchWhole) {
            return Find(tagName, true, matchCase, matchWhole);
        }
        public LinkedList<MarkupElement> Find(string tagName, bool deepSearch, bool matchCase, bool matchWhole) {
            return findCore(
                tagName,
                deepSearch,
                matchCase,
                matchWhole,
                Children,
                false,
                false,
                false);
        }

        public LinkedList<MarkupElement> Find(string[] query, FindMode mode) {
            return Find(query, false, mode);
        }
        
        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, FindMode mode) {
            return Find(
                query,
                matchCase,
                false,
                mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, FindMode mode) {
            return Find(query, matchCase, false, mode);
        }

        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, bool matchWhole, FindMode mode) {
            return Find(query, true, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, bool matchWhole, FindMode mode) {
            return Find(query, true, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool matchCase, bool[] matchWhole, FindMode mode) {
            return Find(query, true, matchCase, matchWhole, mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool[] matchCase, bool[] matchWhole, FindMode mode) {
            return Find(query, true, matchCase, matchWhole, mode);
        }

        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool matchCase, bool matchWhole, FindMode mode) {
            return Find(
                query,
                deepSearch,
                createBoolArray(matchCase, query.Length),
                createBoolArray(matchWhole, query.Length),
                mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool[] matchCase, bool matchWhole, FindMode mode) {
            return Find(
                query,
                deepSearch,
                matchCase,
                createBoolArray(matchWhole, query.Length),
                mode);
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool matchCase, bool[] matchWhole, FindMode mode) {
            return Find(
                query,
                deepSearch,
                createBoolArray(matchCase, query.Length),
                matchWhole,
                mode);
                
        }
        public LinkedList<MarkupElement> Find(string[] query, bool deepSearch, bool[] matchCase, bool[] matchWhole, FindMode mode) { 
            //check to make sure that the amount of queries matches the find mode
            int minQuery = 0;
            if ((mode & FindMode.TagName) == FindMode.TagName) { minQuery++; }
            if ((mode & FindMode.AttributeName) == FindMode.AttributeName) { minQuery++; }
            if ((mode & FindMode.AttributeValue) == FindMode.AttributeValue) { minQuery++; }
            if ((mode & FindMode.Text) == FindMode.Text) { minQuery++; }
            if (query.Length != minQuery) {
                throw new Exception("Expected " + minQuery + " query strings to match the mode \"" + mode + "\".");
            }
            if (matchCase.Length != minQuery) {
                throw new Exception("Expected " + minQuery + " match case options to match the mode \"" + mode + "\".");
            }
            if (matchWhole.Length != minQuery) {
                throw new Exception("Expected " + minQuery + " match case options to match the mode \"" + mode + "\".");
            }

            //define the initial list in which we search on. 
            LinkedList<Node> list = Children;

            /*
                Each mode has the exact same code except we tweak the 
                arguments for findCore respectively.
                
                We pipe the list into the findCore function and push the result
                back into the list so we reduce it each time.             
            */

            //find by tag name?
            int querySelector=0;
            if ((mode & FindMode.TagName) == FindMode.TagName) {
                list = Helpers.ConvertLinkedListType<Node, MarkupElement>(
                    findCore(
                        query[querySelector],
                        deepSearch,
                        matchCase[querySelector],
                        matchWhole[querySelector++],
                        list,
                        false,
                        false,
                        false));
            }

            //find by attribute name?
            if ((mode & FindMode.AttributeName) == FindMode.AttributeName) { 
                list = Helpers.ConvertLinkedListType<Node, MarkupElement>(
                    findCore(
                        query[querySelector],
                        deepSearch,
                        matchCase[querySelector],
                        matchWhole[querySelector++],
                        list,
                        false,
                        true,
                        false));
            }

            //find by attribute value?
            if ((mode & FindMode.AttributeValue) == FindMode.AttributeValue) {
                list = Helpers.ConvertLinkedListType<Node, MarkupElement>(
                    findCore(
                        query[querySelector],
                        deepSearch,
                        matchCase[querySelector],
                        matchWhole[querySelector++],
                        list,
                        false,
                        true,
                        true));
            }

            //find by text?
            if ((mode & FindMode.Text) == FindMode.Text) {
                list = Helpers.ConvertLinkedListType<Node, MarkupElement>(
                    findCore(
                        query[querySelector],
                        deepSearch,
                        matchCase[querySelector],
                        matchWhole[querySelector++],
                        list,
                        true,
                        false,
                        false));
            }

            //return the list
            return Helpers.ConvertLinkedListType<MarkupElement, Node>(list);
        }

        private LinkedList<MarkupElement> findCore(string query, bool deep, bool matchCase, bool matchWhole, LinkedList<Node> list, bool textFind, bool attributeFind, bool attributeValue) {
            LinkedList<MarkupElement> buffer = new LinkedList<MarkupElement>();
            findCore(
                query,
                deep,
                matchCase,
                matchWhole,
                list,
                buffer,
                textFind,
                attributeFind,
                attributeValue);
            return buffer;
        }
        private void findCore(string query, bool deep, bool matchCase, bool matchWhole, LinkedList<MarkupElement> list, LinkedList<MarkupElement> buffer, bool textFind, bool attributeFind, bool attributeValue) {
            findCore(
                query,
                deep,
                matchCase,
                matchWhole,
                Helpers.ConvertLinkedListType<Node, MarkupElement>(list),
                buffer,
                textFind,
                attributeFind,
                attributeFind);
        }
        private void findCore(string query, bool deep, bool matchCase, bool matchWhole, LinkedList<Node> list, LinkedList<MarkupElement> buffer, bool textFind, bool attributeFind, bool attributeValue) { 
            //check for valid call
            //if it's a text find AND attribute find, we actually call findCore twice
            //one to match attributes and pipe the result into a text find call
            if (textFind && attributeFind) {
                LinkedList<MarkupElement> pipe = new LinkedList<MarkupElement>();
                findCore(query, deep, matchCase, matchWhole, list, pipe, false, attributeFind, attributeValue);
                findCore(query, deep, matchCase, matchWhole, pipe, buffer, true, false, false);
                return;
            }
            if (!attributeFind && attributeValue) {
                throw new Exception("Illigal call, attributeValue cannot be true when attributeFind is false.");
            }
            
            //case insensitive? make the query lower case so we can compare two lower case strings together
            if (!matchCase) { query = query.ToLower(); }

            //enumerate over all the child nodes
            IEnumerator<Node> children = list.GetEnumerator();
           
            while (children.MoveNext()) {
                MarkupElement current = children.Current as MarkupElement;
                bool match = false;

                //is it an attribute find?
                //if so, iterate over all the attributes and find the name
                if (attributeFind) {
                    IEnumerator<MarkupAttribute> attrEnum = current.Attributes.GetEnumerator();
                    while (attrEnum.MoveNext()) {
                        MarkupAttribute attr = attrEnum.Current;
                        string attrCompare = (attributeValue ? attr.Value : attr.Name);
                        match = matchCore(attrCompare, query, matchCase, matchWhole);
                        if (match) { break; }
                    }

                    //clean up
                    attrEnum.Dispose();
                }
                //text find?
                else if (textFind && current is MarkupTextElement) {
                    match = matchCore(
                        (current as MarkupTextElement).Text,
                        query,
                        matchCase,
                        matchWhole);
                }
                else {
                    //tag compare
                    match = matchCore(current.TagName, query, matchCase, matchWhole);
                } 

                //is it a match?
                if (match) {
                    buffer.AddLast(current);
                }

                //recursive?
                if (deep) {
                    findCore(
                        query,
                        deep,
                        matchCase,
                        matchWhole,
                        current.Children,
                        buffer,
                        textFind,
                        attributeFind,
                        attributeValue);
                }
            }


            //clean up
            children.Dispose();
        }
        private bool matchCore(string a, string b, bool matchCase, bool matchWhole) { 
            //if the query is * we return true since it means the caller
            //whats everything
            if (b == "*") { return true; }

            //already assumes b is lower case IF match case is true
            //(b = query)
            if (!matchCase) { a = a.ToLower(); }
            return
                matchWhole ?
                a == b :
                a.Contains(b);
        }

        private bool[] createBoolArray(bool vals, int length) {
            bool[] buffer = new bool[length];
            for (int c = 0; c < length; c++) { buffer[c] = vals; }
            return buffer;
        }

        #endregion

        public override string ToString() {
            //define the buffer to return, start with the start of the tag
            string buffer = "";
            buffer += "<" + p_TagName;

            //convert the attributes to an array so we can handle it easily
            MarkupAttribute[] att = Helpers.LinkedListToArray(p_Attributes);

            //add the attributes
            if (att.Length != 0) {
                buffer += " ";
                buffer += Helpers.FlattenToString(att, " ");
            }

            //clean up
            buffer += ">";
            return buffer;
        }

        [Flags]
        public enum FindMode { 
            TagName = 0x1,
            AttributeName = 0x2,
            AttributeValue = 0x4,
            Text = 0x8,

            Attribute = AttributeName | AttributeValue
            
        }
    }
}