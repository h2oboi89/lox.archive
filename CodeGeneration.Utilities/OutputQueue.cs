using System.Collections.Generic;

namespace CodeGeneration.Utilities
{
    public class OutputQueue
    {
        private readonly Queue<string> output = new Queue<string>();

        public void Enqueue(string s = null)
        {
            output.Enqueue(s ?? string.Empty);
        }

        public IEnumerable<string> Publish()
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
                    case "};":
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
    }
}