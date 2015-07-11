using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Lipsis.Core;


namespace Lipsis.Languages.Markup {
    public unsafe class MarkupDocument {
        private const byte MarkupTAG_OPEN = (byte)'<';
        private const byte MarkupTAG_CLOSE = (byte)'>';
        private const byte MarkupTAG_SCOPECLOSE = (byte)'/';
        private const byte Markup_CHAR = (byte)'\'';
        private const byte Markup_STR = (byte)'"';
        private const byte Markup_TERMINATE = (byte)'\\';
        private static void* NULLPTR = (void*)0;

        private MarkupElement p_Root;

        public MarkupDocument() {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
        }

        #region Wrapper constructors to Parse function
        public MarkupDocument(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parse(data, textTagName, textTags, noScopeTags, p_Root);
            OnDocumentLoaded();
        }
        public MarkupDocument(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parse(data, textTagName, textTags, noScopeTags, p_Root);
            OnDocumentLoaded();
        }
        public unsafe MarkupDocument(byte* data, int length, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            p_Root = new MarkupElement("{LIP_DOCUMENT}");
            Parse(data, length, textTagName, textTags, noScopeTags, p_Root);
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

        #region Parser
        public static LinkedList<MarkupElement> Parse(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            return Parse(data, textTagName, textTags, noScopeTags);
        }
        public static void Parse(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
            Parse(Encoding.ASCII.GetBytes(data), textTagName, textTags, noScopeTags, root);
        }
        public static LinkedList<MarkupElement> Parse(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            MarkupElement root = new MarkupElement("{LIP_DOCUMENT");
            
            Parse(
                data,
                textTagName,
                textTags,
                noScopeTags,
                root);


            //get all the elements from the parent node
            LinkedList<MarkupElement> buffer = new LinkedList<MarkupElement>();
            LinkedList<Node> children = root.Children;
            IEnumerator<Node> e = children.GetEnumerator();
            while (e.MoveNext()) {
                MarkupElement c = e.Current as MarkupElement;
                buffer.AddLast(c);
            }

            //clean up
            e.Dispose();
            return buffer;
        }
        public static void Parse(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
            fixed (byte* ptr = data) {
                Parse(ptr, data.Length, textTagName, textTags, noScopeTags, root);
            }
        }
        public static void Parse(byte* data, int length, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
            //define the pointer which is right at the end of the data
            byte* dataEnd = data + length;

            //define the element in which we are currently adding tags to.
            Node current = root as Node;
                        
            //define the pointers which will hold the text which is added as a text tag once
            //a tag is hit in the data
            byte* innerTextBufferPtr = data;
            byte* innerTextBufferPtrEnd = data;

            //define the list of items that will store the names of every node
            //which are currently in the 
            LinkedList<STRPTR> currentNameList = new LinkedList<STRPTR>();
            LinkedListNode<STRPTR> currentName = null;
            STRPTR currentNamePtr = default(STRPTR);

            //iterate over the data
            while (data < dataEnd) {
                //is it a tag?
                if (*data == MarkupTAG_OPEN) {
                    //save where the end of the text is
                    innerTextBufferPtrEnd = data - 1;            
                    data++;

                    //is the tag we are about to read, a close tag?
                    bool closeTag = false;
                    Helpers.SkipWhitespaces(ref data, dataEnd);
                    if (*data == MarkupTAG_SCOPECLOSE) {
                        data++;
                        closeTag = true;
                    }

                    //define whether or not the close tag character is in the
                    //code, if it isn't, this isn't a tag
                    bool closeCharFound = false;

                    #region read comment
                    //block comment? (<!--)
                    if (*data == '!' && data < dataEnd - 2 &&
                        *(data + 1) == '-' &&
                        *(data + 2) == '-') { 
                        
                        //skip over the comment
                        data += 3;
                        while (data < dataEnd - 3) {
                            if (*data == '-' &&
                               *(data + 1) == '-' &&
                               *(data + 2) == MarkupTAG_CLOSE) {
                                   data += 3;
                                   break;
                            }
                            data++;
                        }
                        continue;
                    }
                    #endregion

                    #region read name
                    //read the name of the tag
                    byte* tagNamePtr, tagNamePtrEnd;
                    readName(ref data, dataEnd, out tagNamePtr, out tagNamePtrEnd, false, false, ref closeCharFound);
                    bool tagHasQM = *tagNamePtr == '?';
                    #endregion

                    #region close tag?
                    //if it's a close tag, check if the close tag is closing the current tag
                    if (closeTag) {
                       bool closingCurrent = compareData(
                            tagNamePtr,
                            currentNamePtr.PTR,
                            tagNamePtrEnd + 1,
                            currentNamePtr.PTREND + 1);
                        
                        //buffer what the current tag is in case we need to add text to it
                        Node oldCurrent = current;
                        
                        //are we closing the current tag?
                        //if so, we just select the parent of the current tag
                        if (closingCurrent) {
                            deallocString(currentNamePtr);
                            currentName = currentName.Previous;
                            current = current.Parent;
                            currentNameList.RemoveLast();
                        }
                        else if(currentName != null) {
                            //find how many nodes up we need to go to find
                            //what tag is being closed
                            LinkedListNode<STRPTR> currentSeek = currentName.Previous;
                            Node currentSeekNode = current.Parent;
                            while (currentSeek != null) {
                                STRPTR currentSeekValue = currentSeek.Value;

                                //if we find a match, we have the element which this tag is closing.
                                if (compareData(
                                    tagNamePtr,
                                    currentSeekValue.PTR,
                                    tagNamePtrEnd + 1,
                                    currentSeekValue.PTREND + 1)) { 
                                    
                                    //select the nodes parent as the current
                                    currentName = currentSeek.Previous;
                                    current = currentSeekNode.Parent;

                                    //clean up
                                    if (currentName != null) {
                                        while (currentName.Next != null) {
                                            STRPTR dealloc = currentName.Next.Value;
                                            currentNameList.RemoveLast();

                                            //causes heap corruption for some reason....
                                            //deallocString(dealloc);
                                        }
                                    }
                                    break;
                                }

                                currentSeek = currentSeek.Previous;
                                currentSeekNode = currentSeekNode.Parent;
                            }
                        }


                        //set the current names pointer to the new pointer 
                        if (currentName == null) {
                            currentNamePtr = default(STRPTR);
                        }
                        else {
                            currentNamePtr = currentName.Value;    
                        }
                        
                        //since we know it is a close tag, skip to the end of the tag
                        while (data < dataEnd && *data != MarkupTAG_CLOSE) { data++; }
                        data++;

                        /*
                         *  is there text to be added before the element is added?
                         *  
                         *  (c&p from text string being added when an element is being added
                        */
                        #region text
                        if (innerTextBufferPtr != innerTextBufferPtrEnd) {
                            //create the text element
                            if (!isEmptyData(innerTextBufferPtr, innerTextBufferPtrEnd - 1)) {
                                string str = Helpers.ReadString(innerTextBufferPtr, innerTextBufferPtrEnd);
                                if (oldCurrent is MarkupTextElement) {
                                    (oldCurrent as MarkupTextElement).Text = str;
                                }
                                else {
                                    oldCurrent.AddChild(new MarkupTextElement(textTagName, str));
                                }      
                            }
                        }

                        //reset
                        innerTextBufferPtr = data;
                        innerTextBufferPtrEnd = data;
                        #endregion
                        continue;
                    }
                    #endregion

                    //are we currently in a text tag? meaning we need to ignore any children being added?
                    //and just assume all data we are reading right now is within that element
                    if (current is MarkupTextElement) {
                        innerTextBufferPtrEnd = data;
                        continue;
                    }

                    //because we now know it's an open tag, start building the element
                    MarkupElement element;
                    string tagNameStr = Helpers.ReadString(tagNamePtr, tagNamePtrEnd);
                    if (textTags.Contains(tagNameStr.ToLower())) {
                        element = new MarkupTextElement(tagNameStr, "");
                    }
                    else { element = new MarkupElement(tagNameStr); }
                    
                    //flag to show if the tag terminates with a "/>"
                    //or has no scope.
                    bool tagDoesTerminate = false;
                    if (tagHasQM ||
                        noScopeTags.Contains(tagNameStr.ToLower()) ||
                        *tagNamePtr == '!' || *tagNamePtr == '@') {
                        tagDoesTerminate = true;
                    }


                    #region attributes
                    //read the attributes for this tag
                    while (data < dataEnd) {
                        Helpers.SkipWhitespaces(ref data, dataEnd);
                        if (*data == MarkupTAG_CLOSE) { break; }
                        if (tagHasQM && *data == '?') { break; }

                        //tag closed?
                        if (*data == MarkupTAG_SCOPECLOSE) {
                            tagDoesTerminate = true;
                            break;
                        }

                        //read the name
                        byte* attrNamePtr, attrNamePtrEnd;
                        if (readName(ref data, dataEnd, out attrNamePtr, out attrNamePtrEnd, false, tagHasQM, ref closeCharFound)) { break; }
                        
                        //skip to the value
                        if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }

                        //read the value
                        byte* attrValuePtr = data, attrValuePtrEnd = (byte*)NULLPTR;
                        if (*data == '=') {
                            data++;
                            if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }

                            //read the value
                            readStringValue(ref data, dataEnd, out attrValuePtr, out attrValuePtrEnd, tagHasQM, ref closeCharFound);
                        }

                        //add the attribute
                        element.AddAttribute(
                            Helpers.ReadString(attrNamePtr, attrNamePtrEnd),
                            Helpers.ReadString(attrValuePtr, attrValuePtrEnd));
                        
                    }

                    #endregion

                    //skip to the end of the tag
                    while (data < dataEnd && *data != MarkupTAG_CLOSE) {
                        if (*data == MarkupTAG_SCOPECLOSE) { tagDoesTerminate = true; }
                        data++; 
                    }
                    if (*data == MarkupTAG_CLOSE) { closeCharFound = true; }
                    
                    #region create inner text if required
                    //is there text to be added before the element is added?
                    if (innerTextBufferPtr != innerTextBufferPtrEnd) { 
                        //create the text element
                        if (!isEmptyData(innerTextBufferPtr, innerTextBufferPtrEnd - 1)) {
                            string str = Helpers.ReadString(innerTextBufferPtr, innerTextBufferPtrEnd);
                            if (current is MarkupTextElement) {
                               (current as MarkupTextElement).Text = str;
                            }
                            else {
                                current.AddChild(new MarkupTextElement(textTagName, str));
                            }                            
                        }
                    }

                    //reset
                    innerTextBufferPtr = data + 1;
                    innerTextBufferPtrEnd = data + 1;
                    #endregion

                    //is this a valid tag?
                    if (!closeCharFound) {
                        //just presume what we just processed was text
                        innerTextBufferPtrEnd = data;
                        continue;
                    }

                    //add the tag
                    current.AddChild(element);

                    //change the current name to the new element
                    //but only if the tag has not been terminated immidiately
                    //(otherwise there is no point adding to it.)
                    if (!tagDoesTerminate) {
                        current = element;
                        currentNamePtr = allocString(element.TagName);
                        currentName = currentNameList.AddLast(currentNamePtr);
                    }
                }

                innerTextBufferPtrEnd++;
                data++;
            }

            //is there text left?
            if (innerTextBufferPtrEnd != innerTextBufferPtr &&
                innerTextBufferPtrEnd > innerTextBufferPtr &&
                !isEmptyData(innerTextBufferPtr, innerTextBufferPtrEnd - 1)) {
                    current.AddChild(new MarkupTextElement(
                        textTagName,
                        Helpers.ReadString(
                            innerTextBufferPtr,
                            innerTextBufferPtrEnd)));

            }
            
            //clean up
            IEnumerator<STRPTR> destructCurrentNameList = currentNameList.GetEnumerator();
            while (destructCurrentNameList.MoveNext()) {
                deallocString(destructCurrentNameList.Current);
            }
            destructCurrentNameList.Dispose();
        }

        private static unsafe bool readName(ref byte* ptr, byte* endPtr, out byte* strStart, out byte* strEnd, bool valueMode, bool tagHasQM, ref bool closeCharFound) {
            //returns false if the end of stream was NOT hit.
            strStart = endPtr;
            strEnd = (byte*)NULLPTR;

            //skip whitespaces so we are sure we are at the start of the name
            if (Helpers.SkipWhitespaces(ref ptr, endPtr)) { return true; }
            strStart = ptr;

            //read the name/value
            while (ptr < endPtr) {  
                if (valueMode) {
                    //is this an invalid character for a value?  
                    if (*ptr == ' ' ||
                       *ptr == '\t' ||
                       *ptr == '>' ||
                       *ptr == '/' ||
                       (tagHasQM && *ptr == '?')) {
                        if (*ptr == '>') { closeCharFound = true; }
                        strEnd = ptr - 1;
                        return false;
                    }
                }
                else {
                    //is this an invalid character for a name?  
                    if (*ptr != '!' &&
                        (!tagHasQM ? *ptr != '?' : true) &&
                        *ptr != ':' &&
                        *ptr != '-' &&
                        *ptr != '@' &&
                        !((*ptr >= 'a' && *ptr <= 'z') ||
                          (*ptr >= 'A' && *ptr <= 'Z') ||
                          (*ptr >= '0' && *ptr <= '9'))) {
                             if (*ptr == '?') { closeCharFound = true; }
                             strEnd = ptr - 1;     
                             return false;
                    }
                }
                
                ptr++;
            }

            return true;
        }
        private static unsafe bool readStringValue(ref byte* ptr, byte* ptrEnd, out byte* strPtr, out byte* strEnd, bool tagHasQM, ref bool closeCharFound) { 
            /*
                this function assumes that ptr is on a non-whitespace character
             * 
             *  returns false if the end of stream was NOT hit.
            */

            //define what string character we should look for which terminates the string we're reading
            byte terminateChar = *ptr++;
            strPtr = ptr;
            strEnd = (byte*)NULLPTR;

            //is it a valid terminate charatcter? if not, we just assume it should
            //be a readValue call
            if (terminateChar != Markup_CHAR &&
               terminateChar != Markup_STR) {
                   ptr--;
                   return readName(ref ptr, ptrEnd, out strPtr, out strEnd, true, tagHasQM, ref closeCharFound);                   
            }

            //read the stream
            while (ptr < ptrEnd) { 
                
                //ignore the character after this one?
                if (*ptr == Markup_TERMINATE) {
                    ptr += 2;
                    continue;
                }

                //hit the end of the string?
                if (*ptr == terminateChar) {
                    strEnd = ptr - 1;
                    ptr++;
                    return false;
                }

                ptr++;
            }

            return true;
        }

        private static unsafe bool compareData(byte* ptr1, byte* ptr2, byte* end1, byte* end2) {
            return compareData(ptr1, ptr2, end1, end2, false);
        }
        private static unsafe bool compareData(byte* ptr1, byte* ptr2, byte* end1, byte* end2, bool matchCase) { 
            //is the length of both pointers the same?
            if (ptr1 == end1 && ptr2 == end2) { return true; }
            if (ptr1 == end1 || ptr2 == end2) { return false; }
            if ((end1 - ptr1) != (end2 - ptr2)) { return false; }

            //check until we hit a mis-match
            while (ptr1 < end1 && ptr2 < end2) {
                //grab the 2 bytes to compare
                byte b1 = *ptr1++;
                byte b2 = *ptr2++;

                //make both lower case?
                if (!matchCase) {
                    if (b1 >= 'A' && b1 <= 'Z') { b1 -= (byte)'A'; b1 += (byte)'a'; }
                    if (b2 >= 'A' && b2 <= 'Z') { b2 -= (byte)'A'; b2 += (byte)'a'; }
                }

                if (b1 != b2) { return false; }
            }

            //no mis-match found after reading data, therefore, match
            return true;
        }
        private static unsafe bool isEmptyData(byte* ptr, byte* ptrEnd) { 
            //read throught the data. if we hit a wanted character
            //it means the data is not blank
            while (ptr < ptrEnd) {
                if (*ptr != ' ' &&
                    *ptr != '\n' &&
                    *ptr != '\r' &&
                    *ptr != '\t') {
                       return false;
                    }
                ptr++;
            }
            return true;
        }

        private static unsafe STRPTR allocString(string str) {
            IntPtr ptrM = Marshal.StringToCoTaskMemAnsi(str);
            byte* ptr = (byte*)ptrM.ToPointer();

            return new STRPTR((byte*)ptr,
                               ptr + str.Length - 1);
        }
        private static unsafe void deallocString(STRPTR ptr) {
            Marshal.FreeCoTaskMem((IntPtr)ptr.PTR);
        }

        /*
            This is not used where it should (functions that out a string ptr) 
            because we do not want those critical portions creating value types
            in memory (because of overhead in c#)
        */
        private unsafe struct STRPTR {
            public STRPTR(byte* ptr, byte* ptrEnd) {
                PTR = ptr;
                PTREND = ptrEnd;
            }

            public byte* PTR;
            public byte* PTREND;
        }

        #endregion

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