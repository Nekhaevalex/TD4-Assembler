using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using AST;

namespace Assembler
{
    class Pext
    {
        string opname;
        int[] arguments;
        int result;
        int mountPoint;

        public Pext(string pextLine, int mountPoint)
        {
            this.mountPoint = mountPoint;
            string[] parsed;
            char[] delimeters = { ':', '|', '>' };
            parsed = pextLine.Split(delimeters);
            opname = Regex.Replace(parsed[0], @"\s+", "");
            arguments = new int[parsed.Length - 2];
            for (int i = 1; i<parsed.Length-1; i++)
            {
                arguments[i - 1] = int.Parse(parsed[i]);
            }
            result = int.Parse(parsed[parsed.Length - 1]);
        }
        public ASTree GeneratePextCode()
        {
            ASTree pextTree = new ASTree();
        }
    }
}
