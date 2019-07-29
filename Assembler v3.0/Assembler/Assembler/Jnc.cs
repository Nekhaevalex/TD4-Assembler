using AST;

namespace Opcode
{
    class Jnc : IOpcode, PCChanger
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get; set; }
        private ASTNode link;
        public ASTNode Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;
                FastAdd = new FastAdd(Link.GetNumber());
            }
        }

        public int Page { get; set; }
        public int Word { get; set; }

        public Jnc(string arg1)
        {
            Name = "jnc";
            if (FastAdd.IsFastAdd(arg1))
            {
                FastAdd = new FastAdd(arg1);
            } else
            {
                //JNCL
                Arg1 = arg1;
                //TODO: Implement label search
                FastAdd = new FastAdd(Link.GetNumber());
            }
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1110, FastAdd);
        }

        public override string ToString()
        {
            FastAdd = new FastAdd(Link.GetNumber());
            return "jnc " + FastAdd.ToString();
        }
    }
}
