﻿using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace Assembler
{
    class Program
    {
        public static string outputFile;
        public static string links;
        public static bool optimize;
        public static bool verboseMode;
        public static bool eightBit;
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
            CommandOption output = commandLine.Option("-o | --output <output>", "Ouput file name", CommandOptionType.SingleValue);
            CommandOption libraries = commandLine.Option("-l | --link <location>", "Libraries location (if not default)", CommandOptionType.SingleValue);
            CommandOption verbose = commandLine.Option("-v | --verbose", "Verbose mode", CommandOptionType.NoValue);
            CommandOption optimization = commandLine.Option("-O | --Optimize", "Optimize assembly (experimental)", CommandOptionType.NoValue);
            CommandOption eightBitMode = commandLine.Option("-n | --NewIm", "Use 8bit machine compilation", CommandOptionType.NoValue);
            commandLine.HelpOption("-? | -h | --help");
            commandLine.OnExecute(() =>
            {
            if (output.HasValue())
            {
                outputFile = output.Value();
                if (outputFile == null)
                {
                    outputFile = "a.out";
                }
                verboseMode = verbose.HasValue();
                if (verboseMode)
                    {
                        Console.WriteLine("---Verbose mode---");
                    }
                optimize = optimization.HasValue();
                links = libraries.Value();
                if (links == null)
                {
                    links = Directory.GetCurrentDirectory();
                }
                eightBit = eightBitMode.HasValue();
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
