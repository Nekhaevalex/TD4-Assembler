using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Assembler
{
    class Program
    {
        public static string outputFile;
        public static string links;
        public static bool optimize;
        public static bool verboseMode;
        static void Main(string[] args)
        {
            CommandLineApplication commandLine = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "TD4",
                Description = "(c) 2019 JL Computer Inc. TD4+ CPU Developer tools."
            };

            string outputFile;
            string links;
            bool optimize;

            var argument = commandLine.Argument("filename", "Source .s file", false);
            CommandOption output = commandLine.Option("-o | --output <output>", "Ouput file name", CommandOptionType.SingleValue);
            CommandOption libraries = commandLine.Option("-l | --link <location>", "Libraries location (if not default)", CommandOptionType.SingleValue);
            CommandOption verbose = commandLine.Option("-v | --verbose", "Verbose mode", CommandOptionType.NoValue);
            CommandOption optimization = commandLine.Option("-O | --Optimize", "Optimize transitions in program", CommandOptionType.NoValue);
            commandLine.HelpOption("-? | -h | --help");
            commandLine.OnExecute(() =>
            {
                if (output.HasValue())
                {
                    outputFile = output.Value();
                    verboseMode = verbose.HasValue();
                    optimize = optimization.HasValue();
                    links = libraries.Value();

                    Assembly assembly = new Assembly(argument.Value);
                } else
                {
                    commandLine.ShowHint();
                }
                return 0;
            });
            commandLine.Execute(args);
        }
    }
}
