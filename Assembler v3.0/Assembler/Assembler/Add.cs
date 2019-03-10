﻿using System;

namespace Assembler
{
    class Add : IOpcode
    {
        private byte _fastAdd;

        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }

        public Add(string arg1, string fastAdd)
        {
            Name = "add";
            Arg1 = arg1;
            FastAdd = new FastAdd(fastAdd);
        }

        public MachineWord toMachineCode()
        {
            switch (Arg1)
            {
                case "a":
                    return new MachineWord(0b0000, FastAdd);
                case "b":
                    return new MachineWord(0b0101, FastAdd);
                default:
                    return MachineWord.NoOperation;
            }
        }
    }
}