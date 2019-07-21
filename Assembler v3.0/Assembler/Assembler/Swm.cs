namespace Opcode
{
    class Swm : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get; set; }

        public Swm (string fastAdd)
        {
            Name = "swm";
            FastAdd = new FastAdd(fastAdd);
        }

        public Swm (int fastAdd)
        {
            Name = "swm";
            FastAdd = new FastAdd(fastAdd);
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1010, FastAdd);
        }
    }
}
