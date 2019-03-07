namespace Assembler
{
    class Mov : IOpcode
    {

        private byte _fastAdd;
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }

        public Mov(string arg1, string arg2, string fastAdd)
        {
            Name = "mov";
            Arg1 = arg1;
            Arg2 = arg2;
            FastAdd = new FastAdd(fastAdd);
        }

        public Mov(string arg1, string arg2)
        {
            Name = "mov";
            Arg1 = arg1;
            FastAdd = FastAdd.Null;
            if (FastAdd.IsFastAdd(arg2))
            {
                FastAdd = new FastAdd(arg2);
            } else
            {
                Arg2 = arg2;
            }
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
