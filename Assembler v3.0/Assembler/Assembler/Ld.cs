namespace Opcode
{
    class Ld : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Ld(string fastAdd)
        {
            Name = "ld";
            FastAdd = new FastAdd(fastAdd);
        }
        public override string ToString()
        {
            return (Name + " " + FastAdd.ToString());
        }
        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1100, FastAdd);
        }
    }
}