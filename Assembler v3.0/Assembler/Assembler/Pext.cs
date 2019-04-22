using System.Text.RegularExpressions;

namespace Assembler
{
    class Pext
    {
        string opname;
        int[] arguments;
        int result;
        int pextID;

        public Pext(string pextLine)
        {
            string patern = @"\w++";
            Match m = Regex.Match(pextLine, patern, RegexOptions.Singleline);

        }
    }
}
