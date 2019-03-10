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
        }
    }
}
