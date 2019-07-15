using System;

namespace Opcode
{
    class In : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public In(string arg1)
        {
            Name = "in";
            Arg1 = arg1;
            FastAdd = FastAdd.Null;
        }

        public In(string arg1, string fastAdd)
        {
            Name = "in";
            Arg1 = arg1;
            FastAdd = new FastAdd(fastAdd);
        }

        public MachineWord toMachineCode()
        {
            switch (Arg1)
            {
                case "a":
                    return new MachineWord(0b0010, FastAdd);
                case "b":
                    return new MachineWord(0b0110, FastAdd);
                default:
                    return MachineWord.NoOperation;
            }
            throw new NotImplementedException();
        }
    }
}
