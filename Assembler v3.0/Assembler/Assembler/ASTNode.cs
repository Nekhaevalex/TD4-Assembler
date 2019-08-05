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
            if (this.Child != null)
            {
                child.Child = this.Child;
            }
            else
            {
                child.Child = null;
            }
            if (this.Child != null)
            {
                this.Child.Parent = child;
            }
            this.Child = child;

        }

        internal ASTNode Child { get; set; } = null;
        internal ASTNode Parent { get; set; } = null;

        public void Remove()
        {
            if (Parent != null && Child != null)
                Parent.Child = Child;
            if (Parent != null && Child != null)
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
            string toRet = opcode.Name.ToString() + " ";
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
