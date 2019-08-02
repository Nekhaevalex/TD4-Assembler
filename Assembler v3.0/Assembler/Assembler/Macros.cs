using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AST;

namespace Assembler
{
    class Macros
    {
        private string opname;
        private string[] placeholders;
        private string[][] macrosText;

        public int ArgumentsLength
        {
            get
            {
                return placeholders.Length;
            }
        }

        public Macros(string opname, string[] placeholders)
        {
            this.opname = opname;
            this.placeholders = placeholders;
            macrosText = new string[0][];
        }
        public void AddLine(string[] line)
        {
            string[][] macrosTextCache = new string[macrosText.Length][];
            for (int i = 0; i<macrosText.Length; i++)
            {
                macrosTextCache[i] = macrosText[i];
            }
            macrosText = new string[macrosText.Length + 1][];
            for (int i = 0; i<macrosTextCache.Length; i++)
            {
                macrosText[i] = macrosTextCache[i];
            }
            macrosText[macrosTextCache.Length] = line;
        }

        public string[][] GenerateMacroCode(string[] arguments)
        {
            Dictionary<string, string> replacers = new Dictionary<string, string>();
            if (arguments.Length != ArgumentsLength)
                return null;
            for (int i = 0; i<arguments.Length; i++)
            {
                replacers.Add(placeholders[i], arguments[i]);
            }
            string[][] macroCopy = new string[macrosText.Length][];
            for (int i = 0; i<macrosText.Length; i++)
            {
                macroCopy[i] = new string[macrosText[i].Length];
                for (int j = 0; j<macrosText[i].Length; j++)
                {
                    macroCopy[i][j] = macrosText[i][j];
                }
            }
            for (int i = 0; i<macroCopy.Length; i++)
            {
                for (int j = 0; j<macroCopy[i].Length; j++)
                {
                    string rep = null;
                    if (replacers.ContainsKey(macroCopy[i][j]))
                    {
                        rep = replacers[macroCopy[i][j]];
                    }
                    if (rep != null)
                    {
                        macroCopy[i][j] = rep;
                    }
                }
            }
            return macroCopy;
        }
    }
}
