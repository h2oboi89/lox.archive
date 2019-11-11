using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework
{
    static class Extensions
    {
        /// <summary>
        ///     Wrapper for dictionary key access.
        ///     Returns null instead of throwing exception.
        /// </summary>
        /// <param name="dict">Dictionary to get value from.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with key if key exists; otherwise null.</returns>
        internal static TokenType? Get(this Dictionary<string, TokenType> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Similar to <see cref="string.Substring(int, int)"/>.
        /// </summary>
        /// <param name="str">String to extract substring from.</param>
        /// <param name="startIndex">The inclusive zero-based starting character position of a substring in this instance.</param>
        /// <param name="endIndex">The exclusive zero-based ending character position of a substring in this instance.</param>
        /// <returns>
        ///     A string that is equivalent to the substring that begins at
        ///     startIndex in this instance and ends before endIndex, or 
        ///     System.String.Empty if start is equal to the length of this 
        ///     instance or endIndex.
        /// </returns>
        internal static string Extract(this string str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
}
