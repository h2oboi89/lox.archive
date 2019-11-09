using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            DefineAst(outputDirectory, "Expression", new string[]
            {
                "Binary     : Expression left, Token operator, Expression right",
                "Grouping   : Expression expression"  ,
                "Literal    : object value",
                "Unary      : Token operator, Expression right"
            });
        }

        private static readonly List<string> output = new List<string>();

        private static void AppendLine(string s = null)
        {
            output.Add(s ?? string.Empty);
        }

        private static string FormatOutput()
        {
            var sb = new StringBuilder();

            var tab = "    ";
            var tabLevel = 0;

            foreach (var line in output)
            {
                var indent = string.Empty;

                switch (line)
                {
                    case "{":
                        indent = tab.Repeat(tabLevel++);
                        break;
                    case "}":
                        indent = tab.Repeat(--tabLevel);
                        break;
                    default:
                        indent = tab.Repeat(tabLevel);
                        break;
                }

                sb.AppendLine(indent + line);
            }

            return sb.ToString();
        }

        private static void DefineAst(string outputDirectory, string baseName, IEnumerable<string> types)
        {
            AppendLine("namespace LoxFramework.AST");
            AppendLine("{");
            AppendLine($"abstract class {baseName} {{ }}");

            foreach (var type in types)
            {
                var (className, fields, _) = type.Split(':').Select(s => s.Trim());
                AppendLine();
                DefineType(baseName, className, fields);
            }

            AppendLine("}");

            var folder = Path.Combine(outputDirectory, "AST");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, $"{baseName}.cs");

            File.WriteAllText(file, FormatOutput());
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
            var fieldParts = fields.Split(',').Select(s => s.Trim());

            AppendLine($"class {className} : {baseName}");
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

            AppendLine("}");
        }
    }
}