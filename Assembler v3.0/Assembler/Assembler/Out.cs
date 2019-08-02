namespace Opcode
{
    class Out : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int Page { get; set; }
        public int Word { get; set; }

        public Out(string arg1)
        {
            Name = "out";
            if (arg1.ToLower() == "b")
            {
                Arg1 = arg1.ToLower();
            }
            else if (FastAdd.IsFastAdd(arg1))
            {
                Arg1 = null;
                FastAdd = new FastAdd(arg1);
            }
            else
            {
                //
            }
        }

        public Out(string arg1, string fastAdd)
        {
            Name = "out";
            if (arg1.ToLower() == "b")
            {
                Arg1 = arg1.ToLower();
            }
            else
            {
                //
            }
            FastAdd = new FastAdd(fastAdd);
        }

        public override string ToString()
        {
            if (Arg1 == "b")
            {
                return Name + " b";
            } else
            {
                return Name + " " + FastAdd.ToString();
            }
            
        }
        public MachineWord toMachineCode()
        {
            switch (Arg1)
            {
                case "b":
                    return new MachineWord(0b1001, FastAdd);
                case null:
                    return new MachineWord(0b1011, FastAdd);
                default:
                    return MachineWord.NoOperation;
            }
        }
    }
}
