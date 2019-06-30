using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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

        public static void LoadMacros(string path)
        {

        }

        public static ArrayList LoadPext(string path)
        {
            ArrayList pext = new ArrayList();
            try
            {
                string[] pextCode = File.ReadAllLines(path);
                pextCode = Assembly.ClearCode(pextCode);
                for (int i = 0; i<pextCode.Length; i++)
                {
                    Pext newOpcode = new Pext(pextCode[i]);
                }
                //Finish LoadPext
            } catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Pext not found, IOException: {0}", e.Source);
                }
            }
            return ;
        }
    }
}
