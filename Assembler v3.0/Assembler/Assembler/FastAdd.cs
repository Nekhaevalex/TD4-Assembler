namespace Assembler
{
    class FastAdd
    {
        private int value = 0;

        public FastAdd(string value)
        {
            this.value = int.Parse(value) & 0b00001111;
        }

        public static FastAdd Null => new FastAdd("0");

        public static bool IsFastAdd(string value)
        {
            foreach (char c in value)
            {
                if ((c < '0' || c > '9') && c != '-')
                    return false;
            }

            return true;
        }

        public byte toByte()
        {
            return (byte)value;
        }
    }
}
