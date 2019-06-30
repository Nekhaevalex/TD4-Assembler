using System;
using System.Collections;
using System.Collections.Generic;

namespace Assembler
{
    class Assembly
    {
        struct CodeLine
        {
            public string label;
            public string[] code;
        }

        private string programName;
        private ArrayList program = new ArrayList();

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
            //Catching labels
            CodeLine[] code = LabelCatcher(parsed);
            //Converting code to object form
            for (int i = 0; i<code.Length; i++)
            {
                string opcode = code[i].code[0].ToLower();
                if (opcode == "add")
                {
                    program.Add(new Add(code[i].code[1], code[i].code[2]));
                } else if (opcode == "in")
                {
                    program.Add(new In(code[i].code[1]));
                } else if (opcode == "jmp")
                {
                    program.Add(new Jmp(code[i].code[1]));
                } else if (opcode == "jnc")
                {
                    program.Add(new Jnc(code[i].code[1]));
                } else if (opcode == "ld")
                {
                    program.Add(new Ld(code[i].code[1]));
                } else if (opcode == "mov")
                {
                    if (code[i].code.Length == 3)
                    {
                        program.Add(new Mov(code[i].code[1], code[i].code[2]));
                    }
                    else if (code[i].code.Length > 3)
                    {
                        program.Add(new Mov(code[i].code[1], code[i].code[2], code[i].code[3]));
                    }
                } else if (opcode == "out")
                {
                    program.Add(new Out(code[i].code[1]));
                } else if (opcode == "st")
                {
                    program.Add(new St(code[i].code[1]));
                } else if (opcode == "swi")
                {
                    program.Add(new Swi(code[i].code[1]));
                } else if (opcode == "swm")
                {
                    program.Add(new Swm(code[i].code[1]));
                } else
                {
                    importManager.LookUpMacros(opcode);
                }
            }
        }

        private CodeLine[] LabelCatcher(string[][] parsed)
        {
            CodeLine[] code = new CodeLine[parsed.Length];
            for(int i = 0; i<parsed.Length; i++)
            {
                if (parsed[i][0].Contains(":"))
                {
                    code[i].label = parsed[i][0].Substring(0, parsed[i][0].IndexOf(':'));
                    code[i].code = new string[parsed[i].Length-1];
                    for (int j = 0; j < parsed[i].Length-1; j++)
                    {
                        code[i].code[j+1] = parsed[i][j];
                    }
                } else
                {
                    code[i].code = new string[parsed[i].Length];
                    for (int j = 0; j < parsed[i].Length; j++)
                    {
                        code[i].code[j] = parsed[i][j];
                    }
                }
            }
            for (int i = 0; i<code.Length; i++)
            {
                if (code[i].code.Length == 0)
                {
                    code[i + 1].label = code[i].label;
                    code[i].label = "";
                }
            }
            var temp = new List<CodeLine>();
            foreach (var s in code)
            {
                if (s.code.Length != 0)
                    temp.Add(s);
            }
            var linesEdited = temp.ToArray();
            return linesEdited;
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

        public static string[] ClearCode(string[] lines)
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
