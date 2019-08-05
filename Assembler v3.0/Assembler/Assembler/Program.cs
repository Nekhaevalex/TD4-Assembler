using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace Assembler
{
    class Program
    {
        public static string outputFile = "a.out";
        public static string links;
        public static bool optimize;
        public static bool verboseMode;
        public static bool makeBinary = true;
        public static bool eightBit = false;
        public static bool useTracer = false;
        static void Main(string[] args)
        {
            CommandLineApplication commandLine = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "TD4",
                Description = "(c) 2019 JL Computer Inc. TD4++ CPU Developer Kit.\nOptimizing assembler"
            };

            string outputFile;
            string links;
            bool optimize;

            var argument = commandLine.Argument("filename", "Source .s file", false);
            CommandOption output = commandLine.Option("-o | --Output <output>", "Ouput file name", CommandOptionType.SingleValue);
            CommandOption target = commandLine.Option("-t | --Target <target>", "Output target. \'td4+\' produces binary output for TD4+ processor. Classic TD4 programs should be assembled using this option. TD4+ is set by default. \'td4++\' produces code for TD4++ which have 8bit Im. \'asm\' produces assembler code. \'asm++\' produces 8bit assembler code", CommandOptionType.SingleValue);
            CommandOption libraries = commandLine.Option("-l | --Link <location>", "Libraries location (if not default)", CommandOptionType.SingleValue);
            CommandOption verbose = commandLine.Option("-v | --Verbose", "Verbose mode", CommandOptionType.NoValue);
            CommandOption useTracerKey = commandLine.Option("-T | --Tracer", "Use code tracer", CommandOptionType.NoValue);
            CommandOption optimization = commandLine.Option("-O | --Optimize", "Optimize assembly (experimental)", CommandOptionType.NoValue);
            commandLine.HelpOption("-? | -h | --Help");
            commandLine.OnExecute(() =>
            {
                if (output.HasValue())
                {
                    outputFile = output.Value();
                    if (outputFile == null)
                    {
                        outputFile = "a.out";
                    }
                    Program.outputFile = outputFile;
                    verboseMode = verbose.HasValue();
                    useTracer = useTracerKey.HasValue();
                    if (target.HasValue())
                    {
                        if (target.Value() == "td4+")
                        {
                            makeBinary = true;
                            eightBit = false;
                        }
                        else if (target.Value() == "td4++")
                        {
                            makeBinary = true;
                            eightBit = true;
                        }
                        else if (target.Value() == "asm")
                        {
                            makeBinary = false;
                            eightBit = false;
                        } else if (target.Value() == "asm++")
                        {
                            makeBinary = false;
                            eightBit = true;
                        }
                        else
                        {
                            Console.WriteLine("Unknown target: \"{0}\". Aborting.", target.Value());
                            Environment.Exit(1);
                        }
                    }
                    optimize = optimization.HasValue();
                    links = libraries.Value();
                    if (links == null)
                    {
                        links = Directory.GetCurrentDirectory();
                    }
                    if (verboseMode)
                    {
                        Console.WriteLine("TD4++ Assembler v3.0");
                        Console.WriteLine("-=-=Session info=-=-");
                        Console.WriteLine("Source file: " + argument.Value);
                        Console.WriteLine("Output file: " + outputFile);
                        Console.WriteLine("Libraries location: " + links);
                        Console.WriteLine("Use optimizer: " + optimize.ToString());
                        Console.WriteLine("Use tracer: " + useTracer.ToString());
                        Console.WriteLine("Target: {0}", target.Value());
                        Console.WriteLine("---Verbose mode---");
                    }
                    Assembly assembly = new Assembly(argument.Value);
                    Utilities.Utilities.VerbouseOut("Parsing finished");
                    if (optimize)
                    {
                        Optimizer.Optimizer opt = new Optimizer.Optimizer(assembly);
                        Utilities.Utilities.VerbouseOut("Optimiztion finished");
                    }
                    //Output
                    Utilities.Utilities.VerbouseOut("-=-=Writing=-=-");
                    if (makeBinary)
                    {
                        CodeIO.WriteAssembly(assembly.Linker());
                    } else
                    {
                        CodeIO.WriteSource(assembly.Linker());
                    }
                    Utilities.Utilities.VerbouseOut("DONE");
                }
                else
                {
                    commandLine.ShowHint();
                }
                return 0;
            });
            commandLine.Execute(args);
        }
    }
}
