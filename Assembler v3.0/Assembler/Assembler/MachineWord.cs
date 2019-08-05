namespace Opcode
{
    class MachineWord
    {
        private int operation;
        private FastAdd fastadd;

        public MachineWord(int operation, FastAdd fastadd)
        {
            this.operation = operation;
            this.fastadd = fastadd == null ? new FastAdd(0) : fastadd;
        }

        public byte MachineCode4bit()
        {
            return (byte)((operation << 4) & 0b11110000 + fastadd.toByte() & 0b00001111);
        }

        public int MachineCode8bit()
        {
            int shiftedOp = operation << 8;
            shiftedOp = shiftedOp & 0b111100000000;
            shiftedOp += (int)fastadd.toByte() & 0b000011111111;
            return shiftedOp;
        }

        public static MachineWord NoOperation => new MachineWord(0, new FastAdd("0"));
    }
}
