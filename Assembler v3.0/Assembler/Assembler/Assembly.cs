﻿using System;
using System.Collections;
using System.Collections.Generic;
using Opcode;
using AST;
using Utilities;

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
        private ASTree program = new ASTree();
        private ModuleManager importManager;

        public Assembly(string programName)
        {
            this.programName = programName;

            string[] lines = CodeIO.LoadFile(programName);
            //Clearing Code
            //Removing comments and trimming
            Utilities.Utilities.VerbouseOut("-=-=Clearing code from comments=-=-");
            lines = ClearCode(lines);
            string[][] parsed;
            Utilities.Utilities.VerbouseOut("-=-=Parsing lines=-=-");
            parsed = SplitCode(lines);
            //Get hat
            Utilities.Utilities.VerbouseOut("-=-=Parsing libraries imports=-=-");
            ArrayList imports = GetImports(parsed);
            //Catching imports
            Utilities.Utilities.VerbouseOut("Importing");
            importManager = new ModuleManager(imports);

            //Calling preprocessor
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Inserting macroses...");
            parsed = InsertAllMacro(parsed);
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Updated code: ");
            if (Program.verboseMode)
            {
                for (int i = 0; i < parsed.Length; i++)
                {
                    Console.Write(i + ":\t");
                    for (int j = 0; j < parsed[i].Length; j++)
                    {
                        Console.Write(parsed[i][j] + " ");
                    }
                    Console.WriteLine();
                }
            }
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Finding definitions...");
            importManager.CatchDefines(parsed);
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Applying definitions...");
            for (int i = 0; i<parsed.Length; i++)
            {
                for(int j = 0; j<parsed[i].Length; j++)
                {
                    var toPaste = importManager.GetDefinition(parsed[i][j]);
                    if (toPaste != null)
                    {
                        parsed[i][j] = toPaste;
                    }
                }
            }
            Utilities.Utilities.VerbouseOut("-=-=Parsing pExts=-=-");
            ArrayList pexts = GetPexts(parsed);
            importManager.ImportPexts(pexts);
            Utilities.Utilities.VerbouseOut("Removing preprocessor code from source...");
            parsed = ClearHatAfterImport(parsed);
            //Catching labels
            Utilities.Utilities.VerbouseOut("Parsing labels...");
            CodeLine[] code = LabelCatcher(parsed);
            //Converting code to object form
            Utilities.Utilities.VerbouseOut("Converting code to object form");
            for (int i = 0; i<code.Length; i++)
            {
                string opcode = code[i].code[0].ToLower();
                string label = code[i].label;
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
                    program.InsertSubTree(program.Count, importManager.LookUpPext(opcode, code[i].code));
                }
                if (label != null)
                {
                    program.AddLabel(label);
                }
            }
            Utilities.Utilities.VerbouseOut("Linking labels...");
            for (int i = 0; i<program.Count; i++)
            {
                if (program[i].opcode is Jmp || program[i].opcode is Jnc)
                {
                    if (program[i].opcode.Arg1 == null)
                    {
                        ((PCChanger)program[i].opcode).Link = program[program[i].opcode.FastAdd.toInt()+1];
                        Utilities.Utilities.VerbouseOut("LABEL_LINKER","Linked: "+ program[i].opcode.Name + " " + program[i].opcode.FastAdd.ToString() + " to "+ ((PCChanger)program[i].opcode).Link.ToString());
                    } else
                    {
                        ((PCChanger)program[i].opcode).Link = program.GetLabel(program[i].opcode.Arg1);
                    }
                }
            }
            if (Program.verboseMode)
            {
                Console.WriteLine("---Final code---");
                int length = program.Count;
                for (int i = 1; i<=length; i++)
                {
                    Console.Write(i + ":\t");
                    IOpcode line = program.Get(i).opcode;
                    Console.Write(line.Name+"\t");
                    if (line.Arg1 != null)
                    {
                        Console.Write(line.Arg1+"\t");
                    }
                    if (line is Mov)
                    {
                        if (line.Arg2 != null)
                            Console.Write(line.Arg2+ "\t");
                    }
                    if (line.FastAdd != null)
                    {
                        Console.Write(line.FastAdd.GetValue());
                    }
                    Console.WriteLine();
                }
            }
        }

        private string[][] InsertAllMacro(string[][] parsed)
        {
            var temp = new List<string[]>();
            foreach(var line in parsed)
            {
                string[] args = new string[line.Length - 1];
                for (int i = 0; i<line.Length-1; i++)
                {
                    args[i] = line[i + 1];
                }
                string[][] toPaste = importManager.LookUpMacro(line[0], args);
                if (toPaste != null)
                {
                    foreach (var code in toPaste)
                    {
                        temp.Add(code);
                    }
                } else
                {
                    if (line != null)
                        temp.Add(line);
                }
            }
            string[][] linesEdited = temp.ToArray();
            return linesEdited;
        }

        public Dictionary<string, ASTNode> GetLabels()
        {
            return null;
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
            if (Program.verboseMode)
            {
                foreach (var label in linesEdited)
                {
                    if (label.label != null)
                        Utilities.Utilities.VerbouseOut("Found label \"" + label.label+"\"");
                }
            }
            return linesEdited;
        }

        private static string[][] ClearHatAfterImport(string[][] parsed)
        {
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i][0] == "#import")
                {
                    parsed[i] = null;
                } else if (parsed[i][0] == "#pext")
                {
                    parsed[i] = null;
                } else if (parsed[i][0] == "#define")
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
                if (parsed[i][0] == "#pext")
                {
                    pexts.Add(new pextData(parsed[i]));
                    Utilities.Utilities.VerbouseOut("\tAdded " + parsed[i][1] + " pext with mounting point "+ parsed[i][2]);
                }
            }
            return pexts;
        }

        private static ArrayList GetImports(string[][] parsed)
        {
            ArrayList imports = new ArrayList();
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i][0] == "#import")
                {
                    imports.Add(parsed[i][1]);
                    Utilities.Utilities.VerbouseOut("\tAdded "+ parsed[i][1]+" library");
                }
            }
            return imports;
        }

        private static string[][] SplitCode(string[] lines)
        {
            string[][] parsed;
            var parsedLines = new List<string[]>();
            string[] delimeters = { " ", ",", ", " };
            foreach (var line in lines)
            {
                parsedLines.Add(line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries));
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

        public byte[] Make4BitCode()
        {
            byte[] program = new byte[256];
            for (int i = 0; i<this.program.Count; i++)
            {
                program[i] = ((IOpcode)this.program[i]).toMachineCode().MachineCode4bit();
            }
            return program;
        }

        public byte[] Make8bitCode()
        {
            byte[] program = new byte[256 * 256 * 3];
            for (int i = 0; i<this.program.Count; i++)
            {
                short line = ((IOpcode)this.program[i]).toMachineCode().MachineCode8bit();
                program[i * 3] = (byte)((line & 0b111100000000) >> 8);
                program[i * 3+1] = (byte)((line & 0b11110000) >> 4);
                program[i * 3+2] = (byte)((line & 0b1111));
            }
            return program;
        }
    }
}
