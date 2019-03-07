using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Out : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        public FastAdd FastAdd { get; set; }

        public Out(string arg1)
        {
            Name = "out";
            if (arg1 == "b")
            {
                Arg1 = arg1;
            } else if (FastAdd.IsFastAdd(arg1))
            {
                Arg1 = null;
                FastAdd = new FastAdd(arg1);
            } else
            {
                //
            }
        }

        public Out(string arg1, string fastAdd)
        {
            Name = "out";
            if (arg1 == "b")
            {
                Arg1 = arg1;
            } else
            {
                //
            }
            FastAdd = new FastAdd(fastAdd);
        }

        public MachineWord toMachineCode()
        {
            switch(Arg1)
            {
                case "b":
                    return new MachineWord(0b1001, FastAdd);
                case null:
                    return new MachineWord(0b1011, FastAdd);
                default:
                    return MachineWord.NoOperation;
            }
        }
    }
}
