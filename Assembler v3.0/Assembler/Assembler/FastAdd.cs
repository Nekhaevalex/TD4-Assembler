using System;
using System.Text;
using Assembler;
using System.Data;

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
                    if (Program.eightBit)
                    {
                        if (value[0] == '\'' && value[value.Length - 1] == '\'' && value.Length == 3)
                        {
                            this.value = Encoding.ASCII.GetBytes(value)[1];
                        }
                    } else
                    {
                        throw new Exception("Unknown base");
                    }
                }
            } else
            {
                this.value = Convert.ToUInt16(value, 10) & 0b11111111;
            }
            if (this.value < 0)
            {
                this.value = ~this.value;
            }
        }
        public FastAdd (int value)
        {
            this.value = value;
        }

        public static FastAdd Null => new FastAdd("0");

        public static bool IsFastAdd(string value)
        {
            if (value[0] == '\'' && value[value.Length - 1] == '\'' && value.Length == 3)
            {
                return true;
            }
            foreach (char c in value)
            {
                if ((c < '0' || c > '9') && c != '-')
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public int toInt()
        {
            return value;
        }
        public byte toByte()
        {
            return (byte)value;
        }
    }
}
