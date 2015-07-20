using System;
using System.Collections;
using System.Collections.Generic;

namespace Lipsis.Core {
    public class Node : IEnumerable {
        private LinkedList<Node> p_Children;
        private LinkedListNode<Node> p_InternalInstance;

        private int p_Index = -1;
        private int p_ChildCount = 0;
        private Node p_Parent;
        private Node p_LeftSibling;
        private Node p_RightSibling;

        private Node p_LastNode;

        internal Node() {
            //initialize the children instances
            p_Children = new LinkedList<Node>();    
        }

        public int Index { get { return p_Index; } }
        public LinkedList<Node> Children { get { return p_Children; } }

        public int ChildCount { get { return p_ChildCount; } }

        public Node Parent { get { return p_Parent; } }
        public Node FirstNode { get { return p_Children.First.Value; } }
        public Node LastNode { get { return p_LastNode; } }

        public Node LeftSibling { get { return p_LeftSibling; } }
        public Node RightSibling { get { return p_RightSibling; } }
        public LinkedList<Node> LeftSiblings {
            get {
                LinkedList<Node> buffer = new LinkedList<Node>();
                if (p_LeftSibling == null) { return buffer; }

                LinkedListNode<Node> current = p_LeftSibling.p_InternalInstance;
                while (current != null) {
                    buffer.AddFirst(current.Value);
                    current = current.Previous;
                }
                return buffer;
            }
        }
        public LinkedList<Node> RightSiblings {
            get {
                LinkedList<Node> buffer = new LinkedList<Node>();
                if (p_RightSibling == null) { return buffer; }

                LinkedListNode<Node> current = p_RightSibling.p_InternalInstance;
                while (current != null) {
                    buffer.AddLast(current.Value);
                    current = current.Next;
                }
                return buffer;
            }
        }

        public Node AddChild(Node node) { 
            //already exists?
            if (node.p_Parent == this) { return node; }
            
            //remove the node from the current nodes list
            if (node.p_Parent != null) { node.p_Parent.RemoveChild(node); }    
        
            //add the node to the children list
            LinkedListNode<Node> internalNode = p_Children.AddLast(node);
            node.p_LeftSibling = p_LastNode;
            if (p_LastNode != null) { p_LastNode.p_RightSibling = node; }
            p_LastNode = node;

            //setup the node
            node.p_InternalInstance = internalNode;
            node.p_Parent = this;
            node.p_Index = p_ChildCount++;
            return node;
        }
        public bool RemoveChild(Node node) {
            //is the node in this node?
            if (node.p_Parent != this) { return false; }

            //update the indexes of all the nodes after
            //the one to remove (the new indexes would be 
            //their current one - 1
            LinkedListNode<Node> seekCurrent = node.p_InternalInstance.Next;
            while (seekCurrent != null) {
                seekCurrent.Value.p_Index -= 1;
                seekCurrent = seekCurrent.Next;
            }

            //grab the node which might become the last node
            LinkedListNode<Node> possibleLast = node.p_InternalInstance.Previous;

            //remove the node
            p_Children.Remove(node.p_InternalInstance);
            p_ChildCount--;
            
            //unreference the node from this node.
            node.p_Parent = null;
            node.p_InternalInstance = null;
            node.p_Index = -1;
            node.p_LeftSibling = null;
            node.p_RightSibling = null;

            //last element?
            if (node == p_LastNode) {
                p_LastNode = possibleLast == null ? 
                    null : 
                    possibleLast.Value;
            }

            return true;
        }

        public void Add(Node newParent) {
            newParent.AddChild(this);
        }
        public void Remove() {
            if (p_Parent == null) {
                throw new Exception("No parent to remove from!");
            }
            p_Parent.RemoveChild(this);
        }

        public void Clear() { 
            //iterate over each node and remove it from this node
            LinkedListNode<Node> currentNode = FirstNode.p_InternalInstance;
            while (currentNode != null) {
                LinkedListNode<Node> next = currentNode.Next;
                RemoveChild(currentNode.Value);
                currentNode = next;
            }
        }

        public IEnumerator GetEnumerator() { return p_Children.GetEnumerator(); }

        public override string ToString() {
            return "Index = " + Index + ", Children = " + ChildCount;
        }
    }
}