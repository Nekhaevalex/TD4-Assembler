using AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler
{

    public struct pextData
    {
        public string pextName;
        public int mountPoint;
        public pextData(string[] parsedData)
        {
            pextName = parsedData[1];
            mountPoint = int.Parse(parsedData[2]);
        }
    }
    class ModuleManager
    {
        readonly Dictionary<string, Macros> imports = new Dictionary<string, Macros>();
        Dictionary<string, Pext> pexts = new Dictionary<string, Pext>();
        readonly Dictionary<string, string> definitions = new Dictionary<string, string>();

        public ModuleManager(ArrayList importsList)
        {
            Utilities.Utilities.VerbouseOut("Reading macroses list");
            foreach (string macrosName in importsList)
            {
                imports = imports.Concat(CodeIO.LoadMacros(macrosName)).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
            }
        }

        public void ImportPexts(ArrayList pextsList)
        {
            Utilities.Utilities.VerbouseOut("Reading pext list");
            foreach (pextData pextName in pextsList)
            {
                pexts = pexts.Concat(CodeIO.LoadPext(pextName.pextName, pextName.mountPoint)).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
            }
        }

        public void AddDefinition(string name, string value)
        {
            definitions.Add(name, value);
        }

        public string GetDefinition(string name)
        {
            if (definitions.ContainsKey(name))
            {
                return definitions[name];
            }
            else
            {
                return null;
            }
        }

        private void MapDefine(string name, string toMap, int amount)
        {
            if (toMap[0] == '\"' && toMap[toMap.Length - 1] == '\"')
            {
                string clear = toMap.Substring(1, toMap.Length - 2);
                amount = clear.Length;
                int step = clear.Length / amount;
                for (int i = 0; i < clear.Length; i += step)
                {
                    AddDefinition(name + "[" + (i / step).ToString() + "]", '\'' + (clear).Substring(i, step) + '\'');
                }
                AddDefinition(name + ".length", clear.Length.ToString());
            }
            else
            {
                int additionalNulls = amount - ((toMap.Length - 1) % amount);
                string a = new string('0', additionalNulls);
                int step = (a + toMap).Length / amount;
                for (int i = 0; i < (toMap + a).Length; i += step)
                {
                    AddDefinition(name + "[" + (i / step).ToString() + "]", (toMap + a).Substring(i, step));
                }
            }
        }

        public void CatchDefines(string[][] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i][0] == "#define")
                {
                    AddDefinition(text[i][1], text[i][2]);
                }
                else if (text[i][0] == "#map")
                {
                    if (text[i].Length > 3)
                    {
                        MapDefine(text[i][2], text[i][1], int.Parse(text[i][3]));
                    }
                    else
                    {
                        MapDefine(text[i][2], text[i][1], Program.eightBit ? 256 : 16);
                    }
                }
            }
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "DEFINITIONS:");
            if (Program.verboseMode)
            {
                foreach (string s in definitions.Keys)
                {
                    Console.WriteLine("\t{0}\t:\t{1}", s, definitions[s]);
                }
            }
        }

        internal ASTree LookUpPext(string opcode, string[] args)
        {
            Pext toReplace = pexts[opcode];
            return toReplace.GeneratePextCode(args);
        }
        internal string[][] LookUpMacro(string opcode, string[] args)
        {
            if (imports.ContainsKey(opcode))
            {
                Macros toReplace = imports[opcode];
                return toReplace.GenerateMacroCode(args);
            }
            else
            {
                return null;
            }
        }
    }
}
