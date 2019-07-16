using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AST;

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
    class ImportManager
    {
        private ArrayList imports = new ArrayList();
        readonly Dictionary<string, Pext> pexts = new Dictionary<string, Pext>();

        public ImportManager(ArrayList importsList, ArrayList pextsList)
        {
            foreach (pextData pextName in pextsList)
            {
                pexts = pexts.Concat(CodeIO.LoadPext(pextName.pextName, pextName.mountPoint)).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
            }
        }

        private ArrayList extractMacros()
        {
            //Get imports
            //Get name  
            //Get args
            //Get body
            return null;
        }
        internal ASTree LookUpPext(string opcode, string[] args)
        {
            Pext toReplace = pexts[opcode];
            return toReplace.GeneratePextCode(args);
        }
    }
}
