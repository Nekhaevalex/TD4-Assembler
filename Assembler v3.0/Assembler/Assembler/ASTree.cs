using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opcode;

namespace AST
{
    class ASTree
    {
        private ASTNode nodes;
        private ASTNode lastNode;
        private int count;
        private Dictionary<string, ASTNode> labels = new Dictionary<string, ASTNode>();
        public int Count {
            get {
                return count;
            }
        }

        public ASTNode GetLabel(string label)
        {
            if (labels.ContainsKey(label))
            {
                return labels[label];
            } else
            {
                return null;
            }
            
        }

        public void AddLabel(string title)
        {
            labels.Add(title, lastNode);
        }

        public void Insert(int i, ASTNode node)
        {
            ASTNode pointer = Get(i);
            node.Child = pointer.Child;
            pointer.Child.Parent = node;
            pointer.Child = node;
            count++;
        }

        public void InsertSubTree(int i, ASTree tree)
        {
            ASTNode pointer = Get(i);
            tree.lastNode.Child = pointer.Child;
            if (pointer.Child != null) {
                pointer.Child.Parent = tree.lastNode;
            }
            pointer.Child = tree.nodes.Child;
            count += tree.count;
            lastNode = Get(count);
        }

        public ASTNode Get(int i)
        {
            if (nodes.Child != null)
            {
                ASTNode pointer = nodes;
                for (int j = 0; j<i; j++)
                {
                    if (pointer.Child != null)
                    {
                        pointer = pointer.Child;
                    } else
                    {
                        pointer = null;
                        break;
                    }
                }
                return pointer;
            } else
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
        public ASTree() {
            nodes = new ASTNode();
            lastNode = nodes;
            count = 0;
        }
    }
}
