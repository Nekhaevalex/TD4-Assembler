﻿namespace Opcode
{
    class St : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get; set; }
        public int Page { get; set; }
        public int Word { get; set; }

        public St(string fastAdd)
        {
            Name = "st";
            FastAdd = new FastAdd(fastAdd);
        }

        public override string ToString()
        {
            return (Name + " " + FastAdd.ToString());
        }
        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1101, FastAdd);
        }
    }
}