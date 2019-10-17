using System;
using System.Collections.Generic;

namespace LoxFramework
{
    static class ExtensionMethods
    {
        public static string Extract(this String str, int start, int end)
        {
            return str.Substring(start, end - start);
        }

        public static TokenType? Get(this Dictionary<string, TokenType> dict, string key)
        {
            try
            {
                return dict[key];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
