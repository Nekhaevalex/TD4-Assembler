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

        internal ASTNode Child { get; set; } = null;
        internal ASTNode Parent { get; set; } = null;

        public void Remove()
        {
            Parent.Child = Child;
            Child.Parent = Parent;
        }

        public int GetNumber()
        {
            ASTNode pointer = Parent;
            int point = 0;
            while (pointer.Parent != null)
            {
                pointer = pointer.Parent;
                point++;
            }
            return point;
        }

        public override string ToString()
        {
            string toRet = opcode.Name.ToString()+" ";
            if (opcode.Arg1 != null)
            {
                toRet += opcode.Arg1.ToString();
                toRet += " ";
            }
            if (opcode.Arg2 != null)
            {
                toRet += opcode.Arg2.ToString();
                toRet += " ";
            }
            if (opcode.FastAdd != null)
            {
                toRet += opcode.FastAdd.ToString();
            }
            return toRet;
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
