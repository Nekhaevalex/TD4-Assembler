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
            OptimizeMemoryCalls();
            OptimizeRegistersTransactions();
            if (Program.verboseMode)
            {
                program.PrintCode();
            }
        }

        private void OptimizeMemoryCalls()
        {
            Utilities.Utilities.VerbouseOut("OPTIMIZER", "Starting SWM optimization", System.ConsoleColor.Yellow);
            int lastSWM = 255;
            int removed = 0;
            for (int i = 1; i <= program.Count; i++)
            {
                IOpcode node = program[i].opcode;
                if (node is Swm)
                {
                    if (node.FastAdd.toInt() == lastSWM)
                    {
                        Utilities.Utilities.VerbouseOut("OPTIMIZER", "Removed SWM on line " + (i + removed), System.ConsoleColor.Yellow);
                        program.Remove(i);
                        removed++;
                    }
                    else
                    {
                        lastSWM = node.FastAdd.toInt();
                    }
                }
                if (node is Jmp || node is Jnc)
                {
                    lastSWM = 255;
                }
            }
        }

        private void OptimizeRegistersTransactions()
        {
            Utilities.Utilities.VerbouseOut("OPTIMIZER", "Starting registers transactions optimization", System.ConsoleColor.Yellow);
            int lastA = 255;
            int lastB = 255;
            bool lockA = false;
            bool lockB = false;
            bool remove = false;
            int removed = 0;
            for (int i = 1; i <= program.Count; i++)
            {
                IOpcode node = program[i].opcode;
                if (node is Mov)
                {
                    if (node.Arg1 == "a")
                    {
                        if (node.Arg2 == "b")
                        {
                            if (lastA == lastB && !lockA)
                            {
                                remove = true;
                            }
                            else
                            {
                                lastA = lastB;
                            }
                        }
                        else if (node.FastAdd.toInt() == lastA && !lockA)
                        {
                            remove = true;
                        }
                        else
                        {
                            lastA = node.FastAdd.toInt();
                            lockA = false;
                        }
                    }
                    else if (node.Arg1 == "b")
                    {
                        if (node.Arg2 == "a")
                        {
                            if (lastA == lastB && !lockB)
                            {
                                remove = true;
                            }
                            else
                            {
                                lastB = lastA;
                            }
                        }
                        else if (node.FastAdd.toInt() == lastB && !lockB)
                        {
                            remove = true;
                        }
                        else
                        {
                            lastB = node.FastAdd.toInt();
                            lockB = false;
                        }
                    }
                }
                else if (node is Jmp || node is Jnc)
                {
                    lastA = 255;
                    lastB = 255;
                }
                else if (node is In || node is Ld || node is Add)
                {
                    if (node is In)
                    {
                        if (node.Arg1 == "a")
                        {
                            lockA = true;
                        }
                        else if (node.Arg1 == "b")
                        {
                            lockB = true;
                        }
                    }
                    else if (node is Ld)
                    {
                        lockB = true;
                    }
                    else if (node is Add)
                    {
                        if (node.Arg1 == "a")
                        {
                            if (node.FastAdd.toInt() != 0)
                            {
                                lockA = true;
                            }
                            if (node.FastAdd.toInt() == 0)
                            {
                                remove = true;
                            }
                        }
                        else if (node.Arg2 == "b")
                        {
                            if (node.FastAdd.toInt() != 0)
                            {
                                lockB = true;
                            }
                            if (node.FastAdd.toInt() == 0)
                            {
                                remove = true;
                            }
                        }
                    }
                }
                if (remove)
                {
                    Utilities.Utilities.VerbouseOut("OPTIMIZER", "Removed reducent MOV on line " + (i + removed), System.ConsoleColor.Yellow);
                    program.Remove(i);
                    i--;
                    removed++;
                    remove = false;
                }

            }
        }
        private void OptimizeThroughTransactions()
        {

        }
        public ASTree GetOptimizedAssembly()
        {
            return program;
        }
    }
}