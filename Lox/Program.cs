using LoxFramework;
using System;
using System.IO;
using System.Reflection;

namespace Lox
{
    /// <summary>
    /// Lox interpreter for consoles.
    /// </summary>
    class Program
    {
        private static bool InPrompt = false;

        static void Main(string[] args)
        {
            Interpreter.Error += (_, e) =>
            {
                Console.WriteLine(e.Message);

                if (!InPrompt) Environment.Exit(1);
            };

            Interpreter.Out += (_, e) =>
            {
                if (e.Optional && !InPrompt) return;

                Console.WriteLine(e.Message);
            };

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                Environment.Exit(1);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                InPrompt = true;
                RunPrompt();
            }
        }

        static void RunFile(string path)
        {
            Interpreter.Run(File.ReadAllText(path));
        }

        static void RunPrompt()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"{assemblyName.Name} {assemblyName.Version}");

            while (true)
            {
                Console.Write("> ");
                Interpreter.Run(Console.ReadLine());
            }
        }
    }
}
