﻿using AST;
using Assembler;
using System;

namespace Opcode
{
    class Jmp : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2 { get => throw new ArgumentException(); set => throw new ArgumentException(); }

        public Jmp(string arg1)
        {
            Name = "jmp";
            if (FastAdd.IsFastAdd(arg1))
            {
                FastAdd = new FastAdd(arg1);
            }
            else
            {
                //JMPL
                Arg1 = arg1;
                //TODO: Implement label search
                
            }

        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1111, FastAdd);
        }
    }
}
