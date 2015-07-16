using System;
using System.Collections.Generic;

namespace Lipsis.Core {
    public static partial class Helpers {

        public static T[] LinkedListToArray<T>(LinkedList<T> list) { 
            //get the number of items in the list so we know how much memory
            //to allocate for the array we return
            int count = list.Count;
            T[] buffer = new T[count];

           
            //copy the list to the buffer
            list.CopyTo(buffer, 0);
            return buffer;
        }
        public static LinkedList<T> CloneLinkedList<T>(LinkedList<T> list, T[] exclusions) { 
            //create a new LinkedList and iterate through the list to clone and copy
            //all items (except exclusions) to it.
            LinkedList<T> buffer = new LinkedList<T>();

            //create the enumerator which we move through the list and copy each node
            //we also get the length of the exclusions so we don't have to make a .Length
            //call per iteration.
            IEnumerator<T> e = list.GetEnumerator();
            int exclusionsLength = exclusions != null ? exclusions.Length : 0;
            while (e.MoveNext()) { 
                
                //exclude this item?
                T current = e.Current;
                if (exclusions != null) {
                    bool exclude = false;
                    for (int c = 0; c < exclusionsLength; c++) {
                        if (exclusions[c].Equals(current)) {
                            exclude = true;
                            break;
                        }
                    }
                    if (exclude) { continue; }
                }
                
                //add the item to the buffer
                buffer.AddLast(current);
            } 


            //clean up
            e.Dispose();
            return buffer;
        }

        public static LinkedList<To> ConvertLinkedListType<To, From>(LinkedList<From> list) where To : class where From : class {
            IEnumerator<From> e = list.GetEnumerator();
            LinkedList<To> buffer = new LinkedList<To>();
            while (e.MoveNext()) {
                buffer.AddLast(e.Current as To);
            }
            e.Dispose();
            return buffer;
        }

        public static LinkedListNode<T> LinkedListGetValueByIndex<T>(LinkedList<T> list, int index) {
            //valid index?
            if (index < 0 || index >= list.Count) {
                throw new Exception("Index is out or range");
            }
            
            int cIndex = 0;

            //enumerate over the list until the index matches the index we want
            LinkedListNode<T> current = list.First;
            while (current != null) {
                if (cIndex == index) { return current; }
                current = current.Next;
            }

            //not found
            throw new Exception("Index not found");
        }

        public static void AddLinkedList<T>(LinkedList<T> list, LinkedList<T> addList) {
            IEnumerator<T> e = addList.GetEnumerator();
            while (e.MoveNext()) {
                list.AddLast(e.Current);
            }
            e.Dispose();
        }
    }
}