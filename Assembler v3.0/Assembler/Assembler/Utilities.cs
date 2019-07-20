using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler;

namespace Utilities
{
    public class Utilities
    {
        public static void VerbouseOut(string message)
        {
            if (Program.verboseMode)
            {
                Console.WriteLine(message);
            }
        }

        public static void VerbouseOut(string process, string message)
        {
            if (Program.verboseMode)
            {
                Console.WriteLine("["+process+"]: "+message);
            }
        }
    }
}
