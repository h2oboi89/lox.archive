using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GenerateAst
{
    class Program
    {
        private static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                args = new string[] { GetExecutingDirectoryName() };
            }

            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateAst <output directory>)");
                Environment.Exit(1);
            }

            var outputDirectory = args[0];

            GenerateType("Expression", outputDirectory, new string[]
            {
                "Assignment : Token name, Expression value",
                "Binary     : Expression left, Token operator, Expression right",
                "Call       : Expression callee, Token paren, IEnumerable<Expression> arguments",
                "Grouping   : Expression expression",
                "Literal    : object value",
                "Logical    : Expression left, Token operator, Expression right",
                "Unary      : Token operator, Expression right",
                "Variable   : Token name"
            });

            GenerateType("Statement", outputDirectory, new string[]
            {
                "Block      : IEnumerable<Statement> statements",
                "Break      : Token keyword",
                "Class      : Token name, IEnumerable<FunctionStatement> methods",
                "Continue   : Token keyword",
                "Expression : Expression expression",
                "Function   : Token name, IEnumerable<Token> parameters, IEnumerable<Statement> body" ,
                "If         : Expression condition, Statement thenBranch, Statement elseBranch",
                "Loop       : Statement initializer, Expression condition, Expression increment, Statement body",
                "Return     : Token keyword, Expression value",
                "Variable   : Token name, Expression initializer"
            });
        }

        private static readonly OutputQueue output = new OutputQueue();

        private static void GenerateType(string baseName, string outputDirectory, IEnumerable<string> types)
        {
            DefineAst(baseName, types);

            GenerateFile(outputDirectory, baseName);
        }

        #region AST Definition
        private static void DefineAst(string baseName, IEnumerable<string> types)
        {
            output.Enqueue("// Generated code, do not modify.");
            output.Enqueue("#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member");
            output.Enqueue("using LoxFramework.Scanning;");
            output.Enqueue("using System.Collections.Generic;");
            output.Enqueue();
            output.Enqueue("namespace LoxFramework.AST");
            output.Enqueue("{");

            DefineVisitor(baseName, types);

            output.Enqueue();

            DefineTypes(baseName, types);

            output.Enqueue("}");
            output.Enqueue("#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member");
        }

        private static void DefineVisitor(string baseName, IEnumerable<string> types)
        {
            output.Enqueue($"interface I{baseName}Visitor<T>");
            output.Enqueue("{");

            foreach (var type in types)
            {
                var (className, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseName}";

                output.Enqueue($"T Visit{extendedClass}({extendedClass} {baseName.ToLower()});");
            }

            output.Enqueue("}");
        }

        private static void DefineTypes(string baseName, IEnumerable<string> types)
        {
            // base class
            output.Enqueue($"abstract class {baseName}");
            output.Enqueue("{");
            output.Enqueue($"public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
            output.Enqueue("}");

            // extension classes
            foreach (var type in types)
            {
                output.Enqueue();

                var (className, fields, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseName}";

                DefineType(baseName, extendedClass, fields);
            }
        }

        private static void DefineType(string baseName, string className, string fields)
        {

            output.Enqueue($"class {className} : {baseName}");
            output.Enqueue("{");

            var fieldParts = fields.SplitTrim(',');

            // fields
            foreach (var field in fieldParts)
            {
                var (type, name, _) = field.Split(' ');
                output.Enqueue($"public readonly {type} {name.ToUppercaseFirst()};");
            }

            output.Enqueue();

            // constructor
            output.Enqueue($"public {className}({KeywordFilter.Filter(fields)})");
            output.Enqueue("{");
            foreach (var field in fieldParts)
            {
                var (_, name, _) = field.Split(' ');
                output.Enqueue($"{name.ToUppercaseFirst()} = {KeywordFilter.Filter(name)};");
            }
            output.Enqueue("}");

            output.Enqueue();

            // visitor pattern
            output.Enqueue($"public override T Accept<T>(I{baseName}Visitor<T> visitor)");
            output.Enqueue("{");
            output.Enqueue($"return visitor.Visit{className}(this);");
            output.Enqueue("}");

            output.Enqueue("}");
        }
        #endregion

        private static void GenerateFile(string outputDirectory, string filename)
        {
            var folder = Path.Combine(outputDirectory, "AST");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, $"{filename}.cs");

            using (var writer = new StreamWriter(File.Create(file)))
            {
                foreach (var line in output.Publish())
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}