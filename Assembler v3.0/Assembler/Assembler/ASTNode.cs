using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opcode;

namespace AST
{
    class ASTNode
    {
        public virtual IOpcode opcode { get; set; }
        private ASTNode parent = null;
        private ASTNode child = null;

        public ASTNode()
        {

        }
        public ASTNode(IOpcode opcode)
        {
            this.opcode = opcode;
        }

        public void Add(IOpcode opcode)
        {
            AddChild(new ASTNode(opcode));
        }

        public void AddChild(ASTNode child)
        {
            child.Parent = this;
            this.Child = child;
        }

        internal ASTNode Child { get => child; set => child = value; }
        internal ASTNode Parent { get => parent; set => parent = value; }

        public void Remove()
        {
            Parent.Child = Child;
            Child.Parent = Parent;
        }

        private void Remove(ASTNode child)
        {
            if (child.Parent == this)
            {
                child.Parent = null;
            }
            this.Child = child.Child;
            child = null;
        }
    }
}
