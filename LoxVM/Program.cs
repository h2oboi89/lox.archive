using System;
using System.Diagnostics;
using System.IO;

namespace LoxVM
{
    class Program
    {
        private static readonly VirtualMachine vm = new VirtualMachine();

        static void Main(string[] args)
        {

            var exitCode = 0;

            if (args.Length == 0)
            {
                Repl();
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                Console.WriteLine("Usage: LoxVM [path]");
                exitCode = 1;
            }

            if (Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }

            Environment.Exit(exitCode);
        }

        private static void Repl()
        {
            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine();

                vm.Interpret(input);
            }
        }

        private static void RunFile(string path)
        {
            var result = vm.Interpret(File.ReadAllText(path));

            switch (result)
            {
                case VirtualMachine.Result.COMPILE_ERROR:
                case VirtualMachine.Result.RUNTIME_ERROR:
                    Environment.Exit(1);
                    break;
            }
        }
    }
}
