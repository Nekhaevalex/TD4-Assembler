using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AST;
using Opcode;

namespace Assembler
{
    class CodeIO
    {
        public static string[] LoadFile(string path)
        {
            try
            {
                string[] sourceCode = File.ReadAllLines(path);
                return sourceCode;
            }
            catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found, IOException: {0}", e.Source);
                }
                throw;
            }
        }

        public static void WriteSource(Binary assembly)
        {
            Utilities.Utilities.VerbouseOut("SOURCE WRITER", "Attempt to save into " + Program.outputFile + "...");
            try
            {
                Binary binary = assembly;
                string[] code = new string[binary.Count];
                for (int i = 0; i<binary.Count; i++)
                {
                    if (binary[i] is Jmp)
                    {
                        code[i] = ((Jmp)binary[i]).ToString();
                    } else if (binary[i] is Jnc)
                    {
                        code[i] = ((Jnc)binary[i]).ToString();
                    }
                    else
                    {
                        if (binary[i] != null)
                            code[i] = binary[i].ToString();
                    }
                }
                File.WriteAllLines(Program.outputFile, code);
            } catch (IOException e)
            {
                Console.WriteLine("Shit happens: " + e.Source);
            }
        }

        public static void WriteAssembly(Binary binary)
        {
            Utilities.Utilities.VerbouseOut("ASSEMBLY WRITER", "Attempt to save into " + Program.outputFile + "...");
            try
            {
                Tracer tracer = new Tracer(binary.GetMatrixStack());
                File.WriteAllBytes(Program.outputFile, binary.MakeBuild());
            }
            catch (IOException e)
            {
                Console.WriteLine("Shit happens: "+e.Source);
            }
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
        public static Dictionary<string, Macros> LoadMacros(string path)
        {
            Dictionary<string, Macros> macroses = new Dictionary<string, Macros>();
            try
            {
                string[] text = File.ReadAllLines(path);
                text = Assembly.ClearCode(text);
                string[][] macrosText = SplitCode(text);
                bool insideMacro = false;
                Macros newMacros = new Macros(null, null);
                string macrosName = null;
                foreach (string[] line in macrosText)
                {
                    if (line[0] == "#include")
                    {
                        macroses = macroses.Concat(LoadMacros(line[1])).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
                    } 
                    if (line[0] == "#macro")
                    {
                        macrosName = line[1];
                        int macrosArgsAmount = line.Length - 2;
                        if (line[line.Length - 1] == "{")
                        {
                            macrosArgsAmount--;
                            insideMacro = true;
                        }
                        string[] args = new string[macrosArgsAmount];
                        for (int i = 2; i < 2 + macrosArgsAmount; i++)
                        {
                            args[i - 2] = line[i];
                        }
                        newMacros = new Macros(macrosName, args);
                    }
                    else if (line[0] == "{")
                    {
                        insideMacro = true;
                    }
                    else if (line[0] == "}")
                    {
                        insideMacro = false;
                        macroses.Add(macrosName, newMacros);
                    }
                    else if (insideMacro)
                    {
                        newMacros.AddLine(line);
                    }

                }
            } catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Macros file not found, IOException: {0}", e.Source);
                }
            }
            return macroses;
        }

        public static Dictionary<string, Pext> LoadPext(string path, int mountPoint)
        {
            Dictionary<string, Pext> loadedPexts = new Dictionary<string, Pext>();
            try
            {
                string[] pextCode = File.ReadAllLines(path);
                pextCode = Assembly.ClearCode(pextCode);
                Pext newOpcode;
                for (int i = 0; i<pextCode.Length; i++)
                {
                    newOpcode = new Pext(pextCode[i], mountPoint);
                    //Finish LoadPext
                    loadedPexts.Add(newOpcode.GetOpcode(), newOpcode);
                }
            } catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Pext not found, IOException: {0}", e.Source);
                }
            }
            return loadedPexts;
        }
    }
}
