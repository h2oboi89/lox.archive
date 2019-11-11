using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenerateAst
{
    class Program
    {
        const string baseType = "Expression";
        static readonly string baseName = baseType.ToLower();

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateAst <output directory>)");
                Environment.Exit(1);
            }

            var outputDirectory = args[0];

            DefineAst(new string[]
            {
                $"Binary     : {baseType} left, Token operator, {baseType} right",
                $"Grouping   : {baseType} {baseName}",
                $"Literal    : object value",
                $"Unary      : Token operator, {baseType} right"
            });

            GenerateFile(outputDirectory);
        }

        private static void GenerateFile(string outputDirectory)
        {
            var folder = Path.Combine(outputDirectory, "AST");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, $"{baseType}.cs");

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

        private static void DefineAst(IEnumerable<string> types)
        {
            AppendLine("// Generated code, do not modify.");
            AppendLine("#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member");
            AppendLine("namespace LoxFramework.AST");
            AppendLine("{");

            DefineVisitor(types);

            AppendLine();

            DefineTypes(types);

            AppendLine("}");
            AppendLine("#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member");
        }

        private static void DefineVisitor(IEnumerable<string> types)
        {
            AppendLine("public interface IVisitor<T>");
            AppendLine("{");

            foreach (var type in types)
            {
                var (className, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseType}";

                AppendLine($"T Visit{extendedClass}({extendedClass} {baseName});");
            }

            AppendLine("}");
        }

        private static void DefineTypes(IEnumerable<string> types)
        {
            // base class
            AppendLine($"public abstract class {baseType}");
            AppendLine("{");
            AppendLine($"public abstract T Accept<T>(IVisitor<T> visitor);");
            AppendLine("}");

            // extension classes
            foreach (var type in types)
            {
                AppendLine();

                var (className, fields, _) = type.SplitTrim(':');

                var extendedClass = $"{className}{baseType}";

                DefineType(extendedClass, fields);
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

        private static void DefineType(string className, string fields)
        {
            var fieldParts = fields.SplitTrim(',');

            AppendLine($"public class {className} : {baseType}");
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
            AppendLine($"public override T Accept<T>(IVisitor<T> visitor)");
            AppendLine("{");
            AppendLine($"return visitor.Visit{className}(this);");
            AppendLine("}");

            AppendLine("}");
        }
    }
}