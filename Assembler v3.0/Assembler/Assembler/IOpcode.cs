using System;

namespace Opcode
{
    interface IOpcode
    {
        string Arg1 { get; set; }
        string Arg2 { get; set; }
        string Name { get; set; }
        FastAdd FastAdd { get; set; }

        MachineWord toMachineCode();

    }

}