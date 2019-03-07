namespace Assembler
{
    class Swi : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }

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
