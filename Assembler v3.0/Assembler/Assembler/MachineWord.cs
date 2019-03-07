namespace Assembler
{
    class MachineWord
    {
        private int operation;
        private FastAdd fastadd;

        public MachineWord(int operation, FastAdd fastadd)
        {
            this.operation = operation;
            this.fastadd = fastadd;
        }

        public byte MachineCode()
        {
            return (byte)((operation << 4) & 0b11110000 + fastadd.toByte() & 0b00001111);
        }

        public static MachineWord NoOperation => new MachineWord(0, new FastAdd("0"));
    }
}
