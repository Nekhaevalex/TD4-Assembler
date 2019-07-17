using System;

namespace Opcode
{
    class FastAdd
    {
        private int value = 0;
        public int GetValue()
        {
            return value;
        }
        public FastAdd(string value)
        {
            if (value.Length > 2)
            {
                if (value[1] == 'x')
                {
                    this.value = Convert.ToUInt16(value, 16) & 0b11111111;
                } else if (value[1] == 'b')
                {
                    this.value = Convert.ToUInt16(value, 2) & 0b11111111;
                } else
                {
                    throw new Exception("Unknown base");
                }
            } else
            {
                this.value = Convert.ToUInt16(value, 10) & 0b11111111;
            }
        }
        public FastAdd (int value)
        {
            this.value = value;
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
