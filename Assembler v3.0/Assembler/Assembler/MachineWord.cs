namespace Opcode
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

        public byte MachineCode4bit()
        {
            return (byte)((operation << 4) & 0b11110000 + fastadd.toByte() & 0b00001111);
        }

        public short MachineCode8bit()
        {
            return (short)((operation << 8) & 0b111100000000 + fastadd.toByte() & 0b000011111111);
        }

        public static MachineWord NoOperation => new MachineWord(0, new FastAdd("0"));
    }
}
