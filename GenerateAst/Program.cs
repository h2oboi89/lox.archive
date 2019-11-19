using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateAst
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateAst <output directory>)");
                Environment.Exit(1);
            }

            var outputDirectory = args[0];

            DefineAst("Expression", new string[]
            {
                "Assignment : Token name, Expression value",
                "Binary     : Expression left, Token operator, Expression right",
                "Grouping   : Expression expression",
                "Literal    : object value",
                "Unary      : Token operator, Expression right",
                "Variable   : Token name"
            });

            GenerateFile(outputDirectory, "Expression");

            DefineAst("Statement", new string[]
            {
                "Block      : IEnumerable<Statement> statements",
                "Expression : Expression expression",
                "Print      : Expression expression",
                "Variable   : Token name, Expression initializer"
            });

            GenerateFile(outputDirectory, "Statement");
        }

        private static void GenerateFile(string outputDirectory, string filename)
        {
            var folder = Path.Combine(outputDirectory, "AST");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, $"{filename}.cs");

            using (var writer = new StreamWriter(File.OpenWrite(file)))
            {
                foreach (var line in FormatOutput())
                {
                    writer.WriteLine(line);
                }
            }
        }

        private static readonly Queue<string> output = new Queue<string>();

        private static void AppendLine(string s = null)
        {
            output.Enqueue(s ?? string.Empty);
        }

        private static IEnumerable<string> FormatOutput()
        {
            var tab = "    ";
            var tabLevel = 0;

            while (output.Count > 0)
            {
                var line = output.Dequeue();
                string indent;

                switch (line)
                {
                    case "{":
                        indent = tab.Repeat(tabLevel++);
                        break;
                    case "}":
                        indent = tab.Repeat(--tabLevel);
                        break;
                    case "":
                        indent = string.Empty;
                        break;
                    default:
                        indent = tab.Repeat(tabLevel);
                        break;
                }

                yield return indent + line;
            }
        }

        private static void DefineAst(string baseName, IEnumerable<string> types)
        {
            AppendLine("// Generated code, do not modify.");
            AppendLine("#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member");
            AppendLine("using LoxFramework.Scanning;");
            AppendLine("using System.Collections.Generic;");
            AppendLine();
            AppendLine("namespace LoxFramework.AST");
            AppendLine("{");

            DefineVisitor(baseName, types);

            AppendLine();

            DefineTypes(baseName, types);

            AppendLine("}");
            AppendLine("#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member");
        }

        private static void DefineVisitor(string baseName, IEnumerable<string> types)
        {
            AppendLine($"public interface I{baseName}Visitor<T>");
            AppendLine("{");

            foreach (var type in types)
            {
                var (className, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseName}";

                AppendLine($"T Visit{extendedClass}({extendedClass} {baseName.ToLower()});");
            }

            AppendLine("}");
        }

        private static void DefineTypes(string baseName, IEnumerable<string> types)
        {
            // base class
            AppendLine($"public abstract class {baseName}");
            AppendLine("{");
            AppendLine($"public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
            AppendLine("}");

            // extension classes
            foreach (var type in types)
            {
                AppendLine();

                var (className, fields, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseName}";

                DefineType(baseName, extendedClass, fields);
            }
        }

        private static readonly Dictionary<string, string> keywordMap = new Dictionary<string, string>
        {
            { "operator", "op" }
        };

        private static string FilterKeywords(string str)
        {
            foreach (KeyValuePair<string, string> entry in keywordMap)
            {
                str = str.Replace(entry.Key, entry.Value);
            }

            return str;
        }

        private static void DefineType(string baseName, string className, string fields)
        {
            var fieldParts = fields.SplitTrim(',');

            AppendLine($"public class {className} : {baseName}");
            AppendLine("{");

            // fields
            foreach (var field in fieldParts)
            {
                var (type, name, _) = field.Split(' ');
                AppendLine($"public readonly {type} {name.ToUppercaseFirst()};");
            }

            AppendLine();

            // constructor
            AppendLine($"public {className}({FilterKeywords(fields)})");
            AppendLine("{");
            foreach (var field in fieldParts)
            {
                var (_, name, _) = field.Split(' ');
                AppendLine($"{name.ToUppercaseFirst()} = {FilterKeywords(name)};");
            }
            AppendLine("}");

            AppendLine();

            // visitor pattern
            AppendLine($"public override T Accept<T>(I{baseName}Visitor<T> visitor)");
            AppendLine("{");
            AppendLine($"return visitor.Visit{className}(this);");
            AppendLine("}");

            AppendLine("}");
        }
    }
}