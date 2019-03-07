using System.Collections;

namespace Assembler
{
    class Assembly
    {
        private string programName;
        private ArrayList imports;
        private ArrayList pexts;
        private ArrayList listing;

        public Assembly(string programName)
        {
            this.programName = programName;
            listing = new ArrayList();
            imports = new ArrayList();
            pexts = new ArrayList();
        }



    }
}
