using System;
using System.Collections;

namespace Assembler
{
    class ImportManager
    {
        private ArrayList imports;
        private ArrayList pexts;

        public ImportManager(ArrayList imports, ArrayList pexts)
        {
            this.imports = imports;
            this.pexts = pexts;
            foreach (string pextName in this.pexts)
            {
                CodeIO.LoadPext(pextName);
            }
        }



        internal void LookUpMacros(string opcode)
        {
            throw new NotImplementedException();
        }
    }
}
