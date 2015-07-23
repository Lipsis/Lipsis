using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Lipsis.Core {
    public class Node : IEnumerable, ICloneable {
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
        internal Node(LinkedList<Node> children) {
            p_Children = children;
            p_ChildCount = children.Count;
        }

        public int Index { get { return p_Index; } }
        public int ChildCount { get { return p_ChildCount; } }

        public Node Parent { get { return p_Parent; } }
        public Node FirstNode { get { return p_Children.First.Value; } }
        public Node LastNode { get { return p_LastNode; } }

        public LinkedList<Node> Children { get { return p_Children; } }
        public LinkedList<Node> Siblings {
            get {
                LinkedList<Node> buffer = new LinkedList<Node>();

                //add all the child elements of the parent BUT this instance
                if (p_Parent == null) { return buffer; }
                IEnumerator<Node> e = p_Parent.p_Children.GetEnumerator();
                while (e.MoveNext()) {
                    Node current = e.Current;
                    if (current == this) { continue; }
                    buffer.AddLast(current);
                }
                return buffer;
            }
        }

        #region Siblings
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

        public Node AdjacentSibling(int offset) { 
            //zero?
            if (offset == 0) { return this; }
            
            //immidiate left/right?
            if (offset == -1) { return p_LeftSibling; }
            if (offset == 1) { return p_RightSibling; }

            //deturmine which list of siblings we look at (if it's left/right)
            bool leftList = offset < 0;
            if (leftList) { offset = -offset; }
            LinkedList<Node> list = (leftList ? LeftSiblings : RightSiblings);
            LinkedListNode<Node> currentNode = (leftList ? list.Last : list.First);

            //iterate over the nodes in the siblings list
            int index = 1;
            while (currentNode != null) {
                //index match?
                if (index == offset) { return currentNode.Value; }

                //seek to the next node
                currentNode = (leftList ?
                    currentNode.Previous :
                    currentNode.Next);
                index++;
            }
            return null;
        }
        #endregion

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
            
            node.p_RightSibling.p_LeftSibling = node.p_LeftSibling;
            if (node.p_LeftSibling != null) { 
                node.p_LeftSibling.p_RightSibling = node.p_RightSibling; 
            }

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

        public Node AddChildBefore(Node before, Node node) { 
            //does the before node exist?
            if (before.p_Parent != this) {
                throw new Exception("Node to add before is not a member of this instance");
            }
            
            //remove the node from its current parent
            if (node.p_Parent != null) {
                node.Remove();
            }

            //set the indexes of the before/new node
            node.p_Index = before.Index;
            before.p_Index++;
            
            //update the index of every node after the node we add behind
            //to accompany the new node
            LinkedListNode<Node> seekCurrent = before.p_InternalInstance.Next;
            while (seekCurrent != null) {
                seekCurrent.Value.p_Index++;
                seekCurrent = seekCurrent.Next;
            }

            //insert
            node.p_InternalInstance = new LinkedListNode<Node>(node);
            p_Children.AddBefore(before.p_InternalInstance, node.p_InternalInstance);


            node.p_LeftSibling = before.p_LeftSibling;
            node.p_RightSibling = before;

            before.p_LeftSibling.p_RightSibling = node;
            before.p_LeftSibling = node;
            

            //update
            p_ChildCount++;
            return node;
        }
        public Node AddChildAfter(Node after, Node node) { 
            //the after node must be a member of this node
            if (after.p_Parent != this) {
                throw new Exception("Node to add before is not a member of this instance");
            }

            //remove the node from it's current parent
            if (node.p_Parent != null) {
                node.Remove();
            }

            //just add at the end of the child list?
            if (p_LastNode == after) { AddChild(node); }

            //just call the AddChildBefore where we use the 
            //after node's right sibling
            AddChildBefore(
                after.p_RightSibling,
                node);
            return node;
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
            //iterate over each node and clear it's links etc..
            LinkedListNode<Node> currentNode = FirstNode.p_InternalInstance;
            while (currentNode != null) {
                Node current = currentNode.Value;

                current.p_Parent = null;
                current.p_LeftSibling = null;
                current.p_RightSibling = null;
                current.p_Index = -1;

                currentNode = currentNode.Next;
            }

            //clear
            p_Children.Clear();
            p_ChildCount = 0;
        }

        public IEnumerator GetEnumerator() { return p_Children.GetEnumerator(); }

        public object Clone() { 
            //define the node to return (which will be the childs parent)
            Node buffer = CloneCreateNode(this);

            //clone every node
            LinkedList<Node> children = new LinkedList<Node>();
            IEnumerator<Node> e = p_Children.GetEnumerator();
            Node previous = null;
            int index = 0;
            while (e.MoveNext()) {
                Node cloned = e.Current.Clone() as Node;
                cloned.p_Parent = buffer;

                //setup the clone so it is setup in
                //the right place where it is in the
                //child list.
                cloned.p_Index = index++;
                cloned.p_LeftSibling = previous;
                if (previous != null) {
                    previous.p_RightSibling = cloned;
                }
                previous = cloned;

                //add the cloned node to the child list.
                children.AddLast(cloned);
            }

            //clean up
            e.Dispose();
            buffer.p_Children = children;
            buffer.p_ChildCount = index;
            buffer.p_LastNode = previous;
            return buffer;
        }
        protected virtual Node CloneCreateNode(Node original) { return new Node(); }

        public override string ToString() {
            return "Index = " + Index + ", Children = " + ChildCount;
        }
    }
}