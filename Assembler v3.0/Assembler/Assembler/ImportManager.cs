using System;
using System.Collections;

using System;

namespace Assembler
{

    public struct pextData
    {
        public string pextName;
        public int mountPoint;
    }
    class ImportManager
    {
        private ArrayList imports = new ArrayList();
        private ArrayList pexts = new ArrayList();

        public ImportManager(ArrayList importsList, ArrayList pextsList)
        {
            foreach (pextData pextName in pextsList)
            {
                pexts.Add(CodeIO.LoadPext(pextName.pextName, pextName.mountPoint));
            }
        }

        private ArrayList extractMacros()
        {
            //Get name  
            //Get args
            //Get body
            return null;
        }

        internal void LookUpMacros(string opcode)
        {
            throw new NotImplementedException();
        }
    }
}
