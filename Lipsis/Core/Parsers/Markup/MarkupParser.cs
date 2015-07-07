using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Lipsis.Core {
    public static unsafe class Parsers {
        private const byte MarkupTAG_OPEN = (byte)'<';
        private const byte MarkupTAG_CLOSE = (byte)'>';
        private const byte MarkupTAG_SCOPECLOSE = (byte)'/';
        private const byte Markup_CHAR = (byte)'\'';
        private const byte Markup_STR = (byte)'"';
        private const byte Markup_TERMINATE = (byte)'\\';
        private unsafe static void* NULLPTR = (void*)0;

        public static LinkedList<MarkupElement> ParseMarkup(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            return ParseMarkup(data, textTagName, textTags, noScopeTags, new MarkupElement("{LIP_DOCUMENT}"));
        }
        public static LinkedList<MarkupElement> ParseMarkup(string data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
            return ParseMarkup(Encoding.ASCII.GetBytes(data), textTagName, textTags, noScopeTags, root);
        }
        public unsafe static LinkedList<MarkupElement> ParseMarkup(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags) {
            return ParseMarkup(
                data,
                textTagName,
                textTags,
                noScopeTags,
                new MarkupElement("{LIP_DOCUMENT}"));
        }
        public static unsafe LinkedList<MarkupElement> ParseMarkup(byte[] data, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
            fixed (byte* ptr = data) {
                return ParseMarkup(ptr, data.Length, textTagName, textTags, noScopeTags, root);
            }
        }
        public static unsafe LinkedList<MarkupElement> ParseMarkup(byte* data, int length, string textTagName, LinkedList<string> textTags, LinkedList<string> noScopeTags, MarkupElement root) {
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
                    skipWhitespaces(ref data, dataEnd);
                    if (*data == MarkupTAG_SCOPECLOSE) {
                        data++;
                        closeTag = true;
                    }

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
                    readName(ref data, dataEnd, out tagNamePtr, out tagNamePtrEnd, false);
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
                        else {
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
                                string str = readString(innerTextBufferPtr, innerTextBufferPtrEnd);
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
                    string tagNameStr = readString(tagNamePtr, tagNamePtrEnd);
                    if (textTags.Contains(tagNameStr.ToLower())) {
                        element = new MarkupTextElement(tagNameStr, "");
                    }
                    else { element = new MarkupElement(tagNameStr); }
                    current.AddChild(element);

                    //flag to show if the tag terminates with a "/>"
                    //or has no scope.
                    bool tagDoesTerminate = false;
                    if (noScopeTags.Contains(tagNameStr.ToLower()) ||
                        *tagNamePtr == '!' || *tagNamePtr == '@') {
                        tagDoesTerminate = true;
                    }


                    #region attributes
                    //read the attributes for this tag
                    while (data < dataEnd) {
                        skipWhitespaces(ref data, dataEnd);
                        if (*data == MarkupTAG_CLOSE) { break; }

                        //tag closed?
                        if (*data == MarkupTAG_SCOPECLOSE) {
                            tagDoesTerminate = true;
                            break;
                        }

                        //read the name
                        byte* attrNamePtr, attrNamePtrEnd;
                        if (readName(ref data, dataEnd, out attrNamePtr, out attrNamePtrEnd, false)) { break; }
                        
                        //skip to the value
                        if (skipWhitespaces(ref data, dataEnd)) { break; }

                        //read the value
                        byte* attrValuePtr = data, attrValuePtrEnd = (byte*)NULLPTR;
                        if (*data == '=') {
                            data++;
                            if (skipWhitespaces(ref data, dataEnd)) { break; }

                            //read the value
                            readStringValue(ref data, dataEnd, out attrValuePtr, out attrValuePtrEnd);
                        }

                        

                        //add the attribute
                        element.AddAttribute(
                            readString(attrNamePtr, attrNamePtrEnd),
                            readString(attrValuePtr, attrValuePtrEnd));
                    }

                    #endregion

                    //skip to the end of the tag
                    while (data < dataEnd && *data != MarkupTAG_CLOSE) {
                        if (*data == MarkupTAG_SCOPECLOSE) { tagDoesTerminate = true; }
                        data++; 
                    }
                    
                    #region create inner text if required
                    //is there text to be added before the element is added?
                    if (innerTextBufferPtr != innerTextBufferPtrEnd) { 
                        //create the text element
                        if (!isEmptyData(innerTextBufferPtr, innerTextBufferPtrEnd - 1)) {
                            string str = readString(innerTextBufferPtr, innerTextBufferPtrEnd);
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
                        readString(
                            innerTextBufferPtr,
                            innerTextBufferPtrEnd)));

            }

            //get all the elements from the parent node
            LinkedList<MarkupElement> buffer = new LinkedList<MarkupElement>();
            current = current.Root as MarkupElement;
            LinkedList<Node> children = current.Children;
            IEnumerator<Node> e = children.GetEnumerator();
            while (e.MoveNext()) {
                MarkupElement c = e.Current as MarkupElement;
                buffer.AddLast(c);
            }
            
            //clean up
            IEnumerator<STRPTR> destructCurrentNameList = currentNameList.GetEnumerator();
            while (destructCurrentNameList.MoveNext()) {
                deallocString(destructCurrentNameList.Current);
            }
            destructCurrentNameList.Dispose();
            e.Dispose();
            return buffer;
        }

        private static unsafe string readString(byte* ptr, byte* endPtr) {
            //blank string?
            if (endPtr == (byte*)NULLPTR) { return ""; }

            //allocate a block of memory which has one extra byte of data at the end
            //to serve as a null terminator so that the string constructor will know
            //when to stop reading the pointer we send to it
            int length = (int)(endPtr - ptr) + 1;
            char* buffer = (char*)Marshal.AllocCoTaskMem((length + 1) * sizeof(char)).ToPointer();
            char* bufferPtr = (char*)buffer;
            *(buffer + length) = '\0';
                        
            //read the data
            while (ptr < endPtr + 1) {
                *bufferPtr++ = (char)*ptr++;
            }

            //clean up
            string ret = new string(buffer);
            Marshal.FreeCoTaskMem((IntPtr)buffer);
            return ret;           
        }
        private static unsafe bool readName(ref byte* ptr, byte* endPtr, out byte* strStart, out byte* strEnd, bool valueMode) {
            //returns false if the end of stream was NOT hit.
            strStart = endPtr;
            strEnd = (byte*)NULLPTR;

            //skip whitespaces so we are sure we are at the start of the name
            if (skipWhitespaces(ref ptr, endPtr)) { return true; }
            strStart = ptr;

            //read the name/value
            while (ptr < endPtr) {  
                if (valueMode) {
                    //is this an invalid character for a value?  
                    if (*ptr == ' ' ||
                       *ptr == '\t' ||
                       *ptr == '>' ||
                       *ptr == '/') {
                        strEnd = ptr - 1;
                        return false;
                    }
                }
                else {
                    //is this an invalid character for a name?  
                    if (*ptr != '!' &&
                        *ptr != ':' &&
                        *ptr != '-' &&
                        *ptr != '@' &&
                        !((*ptr >= 'a' && *ptr <= 'z') ||
                          (*ptr >= 'A' && *ptr <= 'Z') ||
                          (*ptr >= '0' && *ptr <= '9'))) {
                             strEnd = ptr - 1;     
                             return false;
                    }
                }
                
                ptr++;
            }

            return true;
        }
        private static unsafe bool skipWhitespaces(ref byte* ptr, byte* endPtr) {
            //returns false if the end of stream was NOT hit.
            while (ptr < endPtr) {                
                if (!(
                    *ptr == ' ' ||
                    *ptr == '\t' ||
                    *ptr == '\n' ||
                    *ptr == '\r')) {
                        return false;
                }
                
                ptr++;
            }
            return true;
        }
        private static unsafe bool readStringValue(ref byte* ptr, byte* ptrEnd, out byte* strPtr, out byte* strEnd) { 
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
                   return readName(ref ptr, ptrEnd, out strPtr, out strEnd, true);                   
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
    }
}