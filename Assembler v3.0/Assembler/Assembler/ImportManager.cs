using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AST;
using Utilities;

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
            } else
            {
                return null;
            }
        }

        public void CatchDefines(string[][] text)
        {
            for (int i = 0; i<text.Length; i++)
            {
                if (text[i][0] == "#define")
                {
                    AddDefinition(text[i][1], text[i][2]);
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
            } else
            {
                return null;
            }
        }
    }
}
