namespace Assembler
{
    interface IOpcode
    {
        string Arg1 { get; set; }
        string Name { get; set; }
        FastAdd FastAdd { get; set; }

        MachineWord toMachineCode();
    }
}