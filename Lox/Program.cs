using LoxFramework;
using System;
using System.IO;

namespace Lox
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter.OnError += (_, e) =>
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            };

            Interpreter.OnStatus += (_, e) => Console.WriteLine(e.Message);

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
                RunPrompt();
            }
        }

        static void RunFile(string path)
        {
            Interpreter.Run(File.ReadAllText(path));
        }

        static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Interpreter.Run(Console.ReadLine());
            }
        }
    }
}
