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
            AppendLine("// Generated code, do not modify.");
            AppendLine("#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member");
            AppendLine("namespace LoxFramework.AST");
            AppendLine("{");
            DefineVisitor(baseName, types);
            AppendLine();
            AppendLine($"public abstract class {baseName}");
            AppendLine("{");
            AppendLine($"public abstract T Accept<T>(IVisitor<T> visitor);");

            AppendLine("}");

            foreach (var type in types)
            {
                var (className, fields, _) = type.Split(':').Select(s => s.Trim());
                AppendLine();
                DefineType(baseName, className, fields);
            }

            AppendLine("}");
            AppendLine("#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member");

            var folder = Path.Combine(outputDirectory, "AST");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, $"{baseName}.cs");

            File.WriteAllText(file, FormatOutput());
        }

        private static void DefineVisitor(string baseName, IEnumerable<string> types)
        {
            AppendLine("public interface IVisitor<T>");
            AppendLine("{");

            foreach (var type in types)
            {
                var (typeName, _) = type.Split(':').Select(s => s.Trim());
                AppendLine($"T Visit{typeName}{baseName}({typeName}{baseName} {baseName.ToLower()});");
            }

            AppendLine("}");
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

            AppendLine($"public class {className}{baseName} : {baseName}");
            AppendLine("{");

            // fields
            foreach (var field in fieldParts)
            {
                var (type, name, _) = field.Split(' ');
                AppendLine($"public readonly {type} {name.ToUppercaseFirst()};");
            }

            AppendLine();

            // constructor
            AppendLine($"public {className}{baseName}({FilterKeywords(fields)})");
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
            AppendLine($"return visitor.Visit{className}{baseName}(this);");
            AppendLine("}");

            AppendLine("}");
        }
    }
}