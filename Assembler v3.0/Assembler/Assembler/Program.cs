using Microsoft.Extensions.CommandLineUtils;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication commandLine = new CommandLineApplication(throwOnUnexpectedArg: false);
            CommandArgument filename = null;

            string sourceFile;
            string outputFile;
            string links;
            bool verboseMode;
            bool optimize;

            commandLine.Command("source", (target) => filename = target.Argument("fullname", "Assembly source code", false));
            CommandOption output = commandLine.Option("-o | --output <output>", "Ouput file name", CommandOptionType.SingleValue);
            CommandOption libraries = commandLine.Option("-l | --link <location>", "Libraries location (if not default)", CommandOptionType.SingleValue);
            CommandOption verbose = commandLine.Option("-v | --verbose", "Verbose mode", CommandOptionType.NoValue);
            CommandOption optimization = commandLine.Option("-O | --Optimize", "Optimize transitions in program", CommandOptionType.NoValue);
            commandLine.HelpOption("-? | -h | --help");
            commandLine.OnExecute(() =>
            {
                if (output.HasValue())
                {
                    sourceFile = filename.Value;
                    outputFile = output.Value();
                    verboseMode = verbose.HasValue();
                    optimize = optimization.HasValue();
                    links = libraries.Value();
                }
                return 0;
            });
            commandLine.Execute(args);
        }
    }
}
