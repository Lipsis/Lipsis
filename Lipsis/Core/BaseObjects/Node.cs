using System;
using System.Collections.Generic;

namespace Lipsis.Core {
    public class Node {
        internal Node() {
            p_Children = new LinkedList<Node>();
        }

        private LinkedList<Node> p_Children;
        private Node p_Parent;

        public Node Parent { get { return p_Parent; } }
        public LinkedList<Node> Children {
            get {
                return Helpers.CloneLinkedList(
                    p_Children, 
                    null);
            }
        }
        public LinkedList<Node> Siblings {
            get { 
                //is this node a member of another node?
                if (p_Parent == null) {
                    throw new Exception("Unable to get siblings while this node does not have a parent. Consider using the \"ChangeParent\" function");
                }

                return getSiblings();
            }
        }
        public Node First {
            get {
                return Children.First.Value;
            }
        }
        public Node Last {
            get {
                return Children.Last.Value;
            }
        }
        public Node Next { 
            get {
                //is there a parent?
                if (p_Parent == null) {
                    throw new Exception("Unable to get siblings while this node does not have a parent. Consider using the \"ChangeParent\" function");
                }

                //get the linked list node associted with this instance and 
                //get the instance of the next node from that.
                LinkedListNode<Node> n = p_Parent.Children.Find(this);
                return n.Next.Value;
            } 
        }
        public Node Previous {
            get {
                //is there a parent?
                if (p_Parent == null) {
                    throw new Exception("Unable to get siblings while this node does not have a parent. Consider using the \"ChangeParent\" function");
                }

                //get the linked list node associted with this instance and 
                //get the instance of the next node from that.
                LinkedListNode<Node> n = p_Parent.Children.Find(this);
                return n.Previous.Value;
            }
        }

        public bool IsRoot { get { return Parent != null; } }
        public Node Root {
            get { 
                //cycles up through the parent nodes until we hit the
                //root node (the node that does not have a parent
                Node current = this;
                while (current.Parent != null) {
                    current = current.Parent;
                }
                return current;
            }
        }

        public int Index {
            get{
                if (p_Parent == null) { return -1; }
                return p_Parent.IndexOfChild(this);
            }
        }
        public int Depth {
            get { 
                //return how many nodes from the root node we are
                int buffer = 0;
                Node current = this;
                while (current.Parent != null) {
                    current = current.Parent;
                    buffer++;
                }
                return buffer;
            }
        }
        public LinkedList<Node> InheritanceTree {
            get { 
                //define the return buffer
                LinkedList<Node> buffer = new LinkedList<Node>();

                //cycle up through the parents until the parent is null
                Node current = p_Parent;
                while (current != null) {
                    buffer.AddLast(current);
                    current = current.Parent;
                }

                return buffer;
            }
        }

        public Node AddChild(Node node) {
            addChildInternal(node, null, true, 0);
            return node;
        }
        public void RemoveChild(Node node) {
            removeChildInternal(node, true);
        }
        public int IndexOfChild(Node node) { 
            //create the enumarator so we can iterate over the child 
            //nodes to find the node in question
            IEnumerator<Node> e = p_Children.GetEnumerator();

            //find the index which the node is at in the array
            int buffer = 0;
            while (e.MoveNext()) {
                if (e.Current == node) {
                    e.Dispose();
                    return buffer;
                }
                buffer++;
            }

            //clean up
            e.Dispose();
            return -1;
        }

        public Node AddChildBefore(Node node, Node newNode) { 
            LinkedListNode<Node> n = p_Children.Find(node);
            p_Children.AddBefore(n, newNode);
            return newNode;
        }
        public Node AddChildAfter(Node node, Node newNode) {
            LinkedListNode<Node> n = p_Children.Find(node);
            p_Children.AddAfter(n, newNode);
            return newNode;
        }

        public Node ChangeParent(Node newParent) {
            //change parent
            Node oldParent = p_Parent;
            p_Parent = newParent;
                        
            //remove this instance from the current parent
            if (oldParent != null) {
                if (oldParent is Node) {
                    (oldParent as Node).removeChildInternal(this, false);
                }
                else {
                    oldParent.RemoveChild(this);
                }
            }
            
            //add this instance to the new parent
            if (newParent != null) {
                if (newParent is Node) {
                    (newParent as Node).addChildInternal(this, null, false, 0);
                }
                else {
                    newParent.AddChild(this);
                }
            }
            return newParent;
        }

        public void Remove() {
            if (Parent == null) {
                throw new Exception("Unable to remove node while this node does not have a parent. Consider using the \"ChangeParent\" function");
            }
            Parent.RemoveChild(this);
        }
        public void Add(Node parent) {
            parent.Add(this);
        }

        private LinkedList<Node> getSiblings() { 
            //does this node have a parent which is of this type? if so, we can just use the internal
            //"p_Children" field member and just remove this instance from the cloned linkedlist
            if (p_Parent is Node) {
                LinkedList<Node> lst = (p_Parent as Node).p_Children;
                return Helpers.CloneLinkedList(lst, new Node[] { this });   
            }

            //get the array from the parents children property and convert it into a linked list
            //(excluding this instance)
            return Helpers.CloneLinkedList(
                p_Parent.Children,
                new Node[] { this });
        }
        private void addChildInternal(Node node, LinkedListNode<Node> nodeCore, bool changeParent, int type) {
            /*
                type:   0 = add, 1=add before, 2=add after 
            */
            //already added?
            if (node.Parent == this) { return; }

            switch(type){
                case 0: p_Children.AddLast(node); break;
                case 1: p_Children.AddBefore(nodeCore, node); break;
                case 2: p_Children.AddAfter(nodeCore, node); break;
            }


            if (changeParent) { node.ChangeParent(this); }
        }
        private void removeChildInternal(Node node, bool changeParent) {
            p_Children.Remove(node);
            if (changeParent) { node.ChangeParent(null); }
        }
        
        public override string ToString() {
            return "Index = " + Index + ", Children = " + Children.Count;
        }
    }
}