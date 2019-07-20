using AST;
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
        public ASTNode Link { get; set; }

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
                //TODO: Implement label search
                
            }

        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1111, FastAdd);
        }
    }
}
