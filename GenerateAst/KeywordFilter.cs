using System.Collections.Generic;

namespace GenerateAst
{
    static class KeywordFilter
    {
        private static readonly Dictionary<string, string> keywordMap = new Dictionary<string, string>
            {
                { "operator", "op" }
            };

        public static string Filter(string str)
        {
            foreach (KeyValuePair<string, string> entry in keywordMap)
            {
                str = str.Replace(entry.Key, entry.Value);
            }

            return str;
        }
    }
}