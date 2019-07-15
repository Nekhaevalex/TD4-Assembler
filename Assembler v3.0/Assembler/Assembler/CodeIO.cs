using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public static void WriteAssembly(Assembly assembly)
        {
            try
            {
                if (!Program.eightBit)
                {
                    File.WriteAllBytes(Program.outputFile, assembly.Make4BitCode());
                } else
                {
                    File.WriteAllBytes(Program.outputFile, assembly.Make8bitCode());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Shit happens: "+e.Source);
            }
        }

        public static void LoadMacros(string path)
        {

        }

        public static ArrayList LoadPext(string path, int mountPoint)
        {
            ArrayList pext = new ArrayList();
            try
            {
                string[] pextCode = File.ReadAllLines(path);
                pextCode = Assembly.ClearCode(pextCode);
                for (int i = 0; i<pextCode.Length; i++)
                {
                    Pext newOpcode = new Pext(pextCode[i], mountPoint);
                }
                //Finish LoadPext
            } catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Pext not found, IOException: {0}", e.Source);
                }
            }
            return pext;
        }
    }
}
