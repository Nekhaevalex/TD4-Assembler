namespace Opcode
{
    class Mov : IOpcode
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public int Page { get; set; }
        public int Word { get; set; }

        public Mov(string arg1, string arg2, string fastAdd)
        {
            Name = "mov";
            Arg1 = arg1.ToLower();
            Arg2 = arg2.ToLower();
            FastAdd = new FastAdd(fastAdd);
        }

        public Mov(string arg1, string arg2)
        {
            Name = "mov";
            Arg1 = arg1.ToLower();
            FastAdd = FastAdd.Null;
            if (FastAdd.IsFastAdd(arg2))
            {
                FastAdd = new FastAdd(arg2);
            } else
            {
                Arg2 = arg2;
            }
        }

        public bool isCrossRegister()
        {
            if (((Arg1 == "a") || (Arg1 == "b")) && ((Arg2 == "a") || (Arg2 == "b")))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return (Name + " " + Arg1 + " " + Arg2 + " " + FastAdd.ToString());
        }

        public MachineWord toMachineCode()
        {
            switch (Arg1)
            {
                case "a":
                    if (Arg2 == "b")
                    {
                        return new MachineWord(0b0001, FastAdd);
                    } else
                    {
                        return new MachineWord(0b0011, FastAdd);
                    }
                case "b":
                    if (Arg2 == "a")
                    {
                        return new MachineWord(0b0100, FastAdd);
                    } else
                    {
                        return new MachineWord(0b0111, FastAdd);
                    }
                default:
                    return MachineWord.NoOperation;
            }
        }
    }
}
