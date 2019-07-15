namespace Opcode
{
    class Swi : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Swi(string fastAdd)
        {
            Name = "swi";
            FastAdd = new FastAdd(fastAdd);
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1000, FastAdd);
        }
    }
}
