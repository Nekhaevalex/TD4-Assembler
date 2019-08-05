using Opcode;
using System;
using System.Collections.Generic;

namespace AST
{
    class ASTree
    {
        private ASTNode nodes;
        private ASTNode lastNode;
        private int count;
        private Dictionary<string, ASTNode> labels = new Dictionary<string, ASTNode>();
        public int Count
        {
            get
            {
                return count;
            }
        }

        public ASTNode GetLabel(string label)
        {
            if (labels.ContainsKey(label))
            {
                return labels[label];
            }
            else
            {
                return null;
            }

        }

        public int GetNumber(ASTNode node)
        {
            ASTNode pointer;
            int point = 0;
            for (int i = 1; i <= Count; i++)
            {
                pointer = Get(i);
                if (pointer == node)
                {
                    point = i;
                }
            }
            return point;
        }

        public void AddLabel(string title)
        {
            labels.Add(title, lastNode);
        }

        public void Insert(int i, ASTNode node)
        {
            ASTNode pointer = Get(i - 1);
            node.Child = pointer.Child;
            node.Parent = pointer;
            node.Child.Parent = node;
            pointer.Child = node;
            count++;
        }

        public void InsertSubTree(int i, ASTree tree)
        {
            ASTNode pointer = Get(i);
            tree.lastNode.Child = pointer.Child;
            if (pointer.Child != null)
            {
                pointer.Child.Parent = tree.lastNode;
            }
            pointer.Child = tree.nodes.Child;
            count += tree.count;
            lastNode = Get(count);
        }

        public void Remove(int i)
        {
            ASTNode node = Get(i);
            node.Remove();
            count--;
        }

        public ASTNode Get(int i)
        {
            if (nodes.Child != null)
            {
                ASTNode pointer = nodes;
                for (int j = 0; j < i; j++)
                {
                    if (pointer.Child != null)
                    {
                        pointer = pointer.Child;
                    }
                    else
                    {
                        pointer = null;
                        break;
                    }
                }
                return pointer;
            }
            else
            {
                return null;
            }
        }

        public ASTNode this[int key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Insert(key, value);
            }
        }

        public void Add(IOpcode opcode)
        {
            lastNode.Add(opcode);
            lastNode = lastNode.Child;
            count++;
        }
        public ASTree()
        {
            nodes = new ASTNode();
            lastNode = nodes;
            count = 0;
        }

        public void PrintCode()
        {
            int length = Count;
            for (int i = 1; i <= length; i++)
            {
                Console.Write(i + ":\t");
                IOpcode line = Get(i).opcode;
                Console.Write(line.Name + "\t");
                if (line.Arg1 != null)
                {
                    Console.Write(line.Arg1 + "\t");
                }
                if (line is Mov)
                {
                    if (line.Arg2 != null)
                        Console.Write(line.Arg2 + "\t");
                }
                if (line.FastAdd != null)
                {
                    Console.Write(line.FastAdd.GetValue());
                }
                Console.WriteLine();
            }
        }
    }
}
