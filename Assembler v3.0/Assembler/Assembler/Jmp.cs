﻿using AST;
using Assembler;
using System;

namespace Opcode
{
    class Jmp : IOpcode, PCChanger
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }
        public string Arg2
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
        private ASTNode link;
        public ASTNode Link {
            get {
                return link;
            }
            set {
                link = value;
                FastAdd = new FastAdd(Link.GetNumber());
            }
        }

        public int Page { get; set; }
        public int Word { get; set; }

        public Jmp(string arg1)
        {
            Name = "jmp";
            if (FastAdd.IsFastAdd(arg1))
            {
                FastAdd = new FastAdd(arg1);
                Arg1 = null;
            }
            else
            {
                //JMPL
                Arg1 = arg1;
            }

        }

        public Jmp(ASTNode target)
        {
            Name = "jmp";
            Link = target;
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1111, FastAdd);
        }

        public override string ToString()
        {
            FastAdd = new FastAdd(Link.GetNumber());
            return "jmp " + FastAdd.ToString();
        }
    }
}
