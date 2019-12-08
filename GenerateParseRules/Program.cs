using CodeGeneration.Utilities;
using Scanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GenerateParseRules
{
    class Program
    {
        private static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

        private static readonly OutputQueue output = new OutputQueue();

        class RuleSpec
        {
            public readonly TokenType TokenType;
            public readonly string Prefix;
            public readonly string Infix;
            public readonly string Precedence;

            public RuleSpec(TokenType tokenType, string prefix, string infix, string precedence)
            {
                TokenType = tokenType;
                Prefix = prefix;
                Infix = infix;
                Precedence = precedence;
            }
        }

        private static readonly List<RuleSpec> rules = new List<RuleSpec>
        {
            new RuleSpec(TokenType.LEFT_PAREN, "Grouping", null, "NONE"),
            new RuleSpec(TokenType.RIGHT_PAREN, null, null, "NONE"),
            new RuleSpec(TokenType.LEFT_BRACE, null, null, "NONE"),
            new RuleSpec(TokenType.RIGHT_BRACE, null, null, "NONE"),
            new RuleSpec(TokenType.COMMA, null, null, "NONE"),
            new RuleSpec(TokenType.DOT, null, null, "NONE"),
            new RuleSpec(TokenType.MINUS, "Unary", "Binary", "TERM"),
            new RuleSpec(TokenType.MODULO, null, null, "NONE"),
            new RuleSpec(TokenType.PLUS, null, "Binary", "TERM"),
            new RuleSpec(TokenType.SEMICOLON, null, null, "NONE"),
            new RuleSpec(TokenType.SLASH, null, "Binary", "FACTOR"),
            new RuleSpec(TokenType.STAR, null, "Binary", "FACTOR"),

            new RuleSpec(TokenType.BANG, "Unary", null, "NONE"),
            new RuleSpec(TokenType.BANG_EQUAL, null, "Binary", "EQUALITY"),
            new RuleSpec(TokenType.EQUAL, null, null, "NONE"),
            new RuleSpec(TokenType.EQUAL_EQUAL, null, "Binary", "EQUALITY"),
            new RuleSpec(TokenType.GREATER, null, "Binary", "COMPARISON"),
            new RuleSpec(TokenType.GREATER_EQUAL, "Binary", null, "COMPARISON"),
            new RuleSpec(TokenType.LESS, null, "Binary", "COMPARISON"),
            new RuleSpec(TokenType.LESS_EQUAL, null, "Binary", "COMPARISON"),

            new RuleSpec(TokenType.IDENTIFIER, null, null, "NONE"),
            new RuleSpec(TokenType.STRING, null, null, "NONE"),
            new RuleSpec(TokenType.NUMBER, "Number", null, "NONE"),

            new RuleSpec(TokenType.AND, null, null, "NONE"),
            new RuleSpec(TokenType.BREAK, null, null, "NONE"),
            new RuleSpec(TokenType.CLASS, null, null, "NONE"),
            new RuleSpec(TokenType.CONTINUE, null, null, "NONE"),
            new RuleSpec(TokenType.ELSE, null, null, "NONE"),
            new RuleSpec(TokenType.FALSE, "Literal", null, "NONE"),
            new RuleSpec(TokenType.FUN, null, null, "NONE"),
            new RuleSpec(TokenType.FOR, null, null, "NONE"),
            new RuleSpec(TokenType.IF, null, null, "NONE"),
            new RuleSpec(TokenType.NIL, "Literal", null, "NONE"),
            new RuleSpec(TokenType.OR, null, null, "NONE"),
            new RuleSpec(TokenType.RETURN, null, null, "NONE"),
            new RuleSpec(TokenType.SUPER, null, null, "NONE"),
            new RuleSpec(TokenType.THIS, null, null, "NONE"),
            new RuleSpec(TokenType.TRUE, "Literal", null, "NONE"),
            new RuleSpec(TokenType.VAR, null, null, "NONE"),
            new RuleSpec(TokenType.WHILE, null, null, "NONE"),

            new RuleSpec(TokenType.EOF, null, null, "NONE"),
        };

        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                args = new string[] { GetExecutingDirectoryName() };
            }

            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateParseRules <output directory>)");
                Environment.Exit(1);
            }

            var outputDirectory = args[0];

            ValidateRules();

            GenerateClass();

            GenerateFile(outputDirectory);
        }

        private static void ValidateRules()
        {
            var tokenTypes = ((TokenType[])Enum.GetValues(typeof(TokenType))).ToList();

            if (rules.Count != tokenTypes.Count)
            {
                throw new Exception("TokenType mismatch.");
            }

            foreach (var tokenType in tokenTypes.Enumerate())
            {
                if (rules[tokenType.Index].TokenType != tokenType.Value)
                {
                    throw new Exception($"TokenType mismatch. Expected {tokenType.Value}, got {rules[tokenType.Index]}");
                }
            }
        }

        private static void GenerateClass()
        {
            output.Enqueue("// Generated code, do not modify.");
            output.Enqueue("using Scanning;");
            output.Enqueue("using System;");
            output.Enqueue();
            output.Enqueue("namespace LoxVM.Compiling");
            output.Enqueue("{");

            output.Enqueue("class ParseRule");
            output.Enqueue("{");

            GenerateStaticParts();

            GenerateRules();

            output.Enqueue("}");

            output.Enqueue("}");
        }

        private static void GenerateStaticParts()
        {
            // fields
            output.Enqueue("public Action Prefix { get; private set; }");
            output.Enqueue("public Action Infix { get; private set; }");
            output.Enqueue("public Precedence Precedence { get; private set; }");
            output.Enqueue();

            // constructor
            output.Enqueue("private ParseRule(Action prefix, Action infix, Precedence precedence)");
            output.Enqueue("{");
            output.Enqueue("Prefix = prefix;");
            output.Enqueue("Infix = infix;");
            output.Enqueue("Precedence = precedence;");
            output.Enqueue("}");
            output.Enqueue();

            // methods
            // GetRule
            output.Enqueue("public static ParseRule GetRule(TokenType tokenType)");
            output.Enqueue("{");
            output.Enqueue("return rules[(int)tokenType];");
            output.Enqueue("}");
            output.Enqueue();
        }

        private static void GenerateRules()
        {
            output.Enqueue("private static readonly ParseRule[] rules = new ParseRule[]");
            output.Enqueue("{");

            var ruleEntries = new List<(string Rule, string Comment)>();
            foreach (var rule in rules)
            {
                var prefix = ParserMethod(rule.Prefix);
                var infix = ParserMethod(rule.Infix);
                var precedence = rule.Precedence;

                ruleEntries.Add((
                    $"new ParseRule({prefix}, {infix}, Precedence.{precedence}),",
                    $"// TokenType.{rule.TokenType}"
                ));
            }

            ruleEntries.Last().Rule.TrimEnd(',');

            var maxLength = ruleEntries.Aggregate(0, (max, cur) => max > cur.Rule.Length ? max : cur.Rule.Length);

            foreach (var (Rule, Comment) in ruleEntries)
            {
                var requiredSpaces = maxLength - Rule.Length + 5;

                output.Enqueue($"{Rule}{" ".Repeat(requiredSpaces)}{Comment}");
            }

            output.Enqueue("};");
        }

        private static string ParserMethod(string methodName)
        {
            if (methodName == null)
            {
                return "null";
            }
            else
            {
                return $"Parser.{methodName}";
            }
        }

        private static void GenerateFile(string outputDirectory)
        {
            var folder = Path.Combine(outputDirectory, "Compiling");

            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, "ParseRule.cs");

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
