using Assembler;
using AST;
using Opcode;

namespace Optimizer
{
    class Optimizer
    {
        private ASTree program;
        public Optimizer(Assembly assembly)
        {
            program = assembly.GetTree();
            OptimizeDuplicates();
            OptimizeMemoryCalls();
        }

        private void OptimizeDuplicates()
        {
            Utilities.Utilities.VerbouseOut("OPTIMIZER", "Looking for duplicates...");
            IOpcode prev = program[1].opcode;
            for (int i = 2; i <= program.Count; i++)
            {
                IOpcode node = program[i].opcode;
                if ((node.Name == prev.Name) && (node.Arg1 == prev.Arg1) && (node.FastAdd.toInt() == prev.FastAdd.toInt()))
                {
                    if ((node is Mov) || (node is Swi) || (node is Swm) || (node is Ld) || (node is St))
                    {
                        program.Remove(i);
                    }
                }
                else
                {
                    prev = node;
                }
            }
        }

        private void OptimizeMemoryCalls()
        {
            Utilities.Utilities.VerbouseOut("OPTIMIZER", "Starting SWM optimization");
            int lastSWM = 0;
            int removed = 0;
            for (int i = 1; i <= program.Count; i++)
            {
                IOpcode node = program[i].opcode;
                if (node is Swm)
                {
                    if (node.FastAdd.toInt() == lastSWM)
                    {
                        Utilities.Utilities.VerbouseOut("OPTIMIZER", "Removed SWM on line " + (i + removed));
                        program.Remove(i);
                        removed++;
                    }
                    else
                    {
                        lastSWM = node.FastAdd.toInt();
                    }
                }
            }
            if (Program.verboseMode)
            {
                program.PrintCode();
            }
        }

        public ASTree GetOptimizedAssembly()
        {
            return program;
        }
    }
}