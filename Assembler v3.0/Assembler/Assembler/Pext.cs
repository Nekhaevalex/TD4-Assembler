using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Opcode;
using AST;
using Utilities;

namespace Assembler
{
    class Pext
    {
        string opname;
        int[] placeholders;
        int result;
        int mountPoint;

        public Pext(string pextLine, int mountPoint)
        {
            this.mountPoint = mountPoint;
            string[] parsed;
            char[] delimeters = { ':', '|', '>' };
            parsed = pextLine.Split(delimeters);
            opname = Regex.Replace(parsed[0], @"\s+", "");
            placeholders = new int[parsed.Length - 2];
            for (int i = 1; i<parsed.Length-1; i++)
            {
                placeholders[i - 1] = int.Parse(parsed[i]);
            }
            result = int.Parse(parsed[parsed.Length - 1]);
            Utilities.Utilities.VerbouseOut("Parsed pext: \"" + opname + "\" with " + placeholders.Length + " arguments and result " + result.ToString());
        }
        public string GetOpcode()
        {
            return opname;
        }

        internal struct dictLine
        {
            public string argName;
            public int argAddr;
        }

        public ASTree GeneratePextCode(string[] arguments)
        {
            ASTree pextTree = new ASTree();
            //Preventing memory loss simply by storing B register content firstly.
            //To do it we have to move arguments which represents B on the first poistion.
            //We will use some simple sorting
            for (int i = 1; i < arguments.Length; i++)
            {
                for (int j = i; j<arguments.Length; j++)
                {
                    if (arguments[j] == "b")
                    {
                        dictLine cache = new dictLine();
                        //save
                        cache.argAddr = placeholders[i-1];
                        cache.argName = arguments[i];
                        //mov
                        placeholders[i-1] = placeholders[j-1];
                        arguments[i] = arguments[j];
                        //mov from cache
                        placeholders[j-1] = cache.argAddr;
                        arguments[j] = cache.argName;
                        break;
                    }
                }
            }
            pextTree.Add(new Swm(mountPoint));
            for (int i = 1; i < arguments.Length; i++)
            {
                if (arguments[i] == "a")
                {
                    pextTree.Add(new Mov("b", "a")); //may cause memory loss (but fixed earlier)
                    pextTree.Add(new St(placeholders[i-1].ToString()));
                } else if (arguments[i] == "b")
                {
                    pextTree.Add(new St(placeholders[i-1].ToString()));
                } else
                {
                    pextTree.Add(new Mov("b", arguments[i]));
                    pextTree.Add(new St(placeholders[i-1].ToString()));
                }
            }
            pextTree.Add(new Ld(result.ToString()));
            return pextTree;
        }
    }
}
