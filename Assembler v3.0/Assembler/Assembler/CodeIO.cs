using System;
using System.IO;

namespace Assembler
{
    class CodeIO
    {
        public static void LoadFile(string path)
        {
            try
            {
                string[] sourceCode = File.ReadAllLines(path);
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
    }
}
