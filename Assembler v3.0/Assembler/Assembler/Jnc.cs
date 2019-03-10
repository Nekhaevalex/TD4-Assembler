﻿namespace Assembler
{
    class Jnc : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }

        public Jnc(string arg1)
        {
            if (FastAdd.IsFastAdd(arg1))
            {
                FastAdd = new FastAdd(arg1);
            } else
            {
                //JNCL
                Arg1 = arg1;
                //TODO: Implement label search
            }
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1110, FastAdd);
        }
    }
}