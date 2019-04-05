using System;
using System.IO;

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

        public static void LoadPext(string path)
        {
            try
            {
                string[] pextCode = File.ReadAllLines(path);
                //Finish LoadPext
            } catch (IOException e)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Pext not found, IOException: {0}", e.Source);
                }
            }
        }
    }
}
