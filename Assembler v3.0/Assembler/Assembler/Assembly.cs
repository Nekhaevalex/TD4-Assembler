using System;
using System.Collections;
using System.Collections.Generic;

namespace Assembler
{
    class Assembly
    {
        private string programName;

        public Assembly(string programName)
        {
            this.programName = programName;

            string[] lines = CodeIO.LoadFile(programName);
            //Clearing Code
            //Removing comments and trimming
            lines = ClearCode(lines);
            string[][] parsed;
            parsed = SplitCode(lines);
            //Get hat
            ArrayList imports = GetImports(parsed);
            ArrayList pexts = GetPexts(parsed);
            parsed = ClearHatAfterImport(parsed);
            //Catching imports & pexts
            ImportManager importManager = new ImportManager(imports, pexts);

        }

        private static string[][] ClearHatAfterImport(string[][] parsed)
        {
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i][0] == "%import")
                {
                    parsed[i] = null;
                } else if (parsed[i][0] == "%pext")
                {
                    parsed[i] = null;
                }
            }
            var temp = new List<string[]>();
            foreach (var s in parsed)
            {
                if (s != null)
                    temp.Add(s);
            }
            string[][] linesEdited = temp.ToArray();
            return linesEdited;
        }

        private ArrayList GetPexts(string[][] parsed)
        {
            ArrayList pexts = new ArrayList();
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i][0] == "%pext")
                {
                    pexts.Add(parsed[i][1]);
                }
            }
            return pexts;
        }

        private static ArrayList GetImports(string[][] parsed)
        {
            ArrayList imports = new ArrayList();
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i][0] == "%import")
                {
                    imports.Add(parsed[i][1]);
                }
            }
            return imports;
        }

        private static string[][] SplitCode(string[] lines)
        {
            string[][] parsed;
            var parsedLines = new List<string[]>();
            char[] delimeters = { ' ', ',' };
            foreach (var line in lines)
            {
                parsedLines.Add(line.Split(delimeters));
            }
            parsed = parsedLines.ToArray();
            return parsed;
        }

        private static string[] ClearCode(string[] lines)
        {
            char[] charsToTrim = { ' ', '\t' };
            for (int i = 0; i < lines.Length; i++)
            {
                int indexOfComment = lines[i].IndexOf(";");
                if (indexOfComment != -1)
                {
                    lines[i] = lines[i].Substring(0, indexOfComment);
                }
                indexOfComment = lines[i].IndexOf("//");
                if (indexOfComment != -1)
                {
                    lines[i] = lines[i].Substring(0, indexOfComment);
                }
                lines[i] = lines[i].Trim(charsToTrim);
            }
            var temp = new List<string>();
            foreach (var s in lines)
            {
                if (!string.IsNullOrEmpty(s))
                    temp.Add(s);
            }
            var linesEdited = temp.ToArray();
            return linesEdited;
        }
    }
}
