using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LaunchMate.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if one string is contained within another given a specific type of <see cref="StringComparison"/>
        /// </summary>
        /// <param name="str">Containing string</param>
        /// <param name="value">Contained string</param>
        /// <param name="comparisonType"><see cref="StringComparison"/> type</param>
        /// <returns>True if <paramref name="value"/> is contained within <paramref name="str"/> given comparison type <paramref name="comparisonType"/>, false otherwise</returns>
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str?.IndexOf(value, 0, comparisonType) != -1;
        }

        /// <summary>
        /// Check if a string matches a given file path
        /// </summary>
        /// <param name="str">String to check</param>
        /// <param name="path">File path for comparison</param>
        /// <returns>True if <paramref name="str"/> matches the file path in <paramref name="path"/>, false otherwise</returns>
        public static bool MatchesPath(this string str, string path)
        {
            return (path == str || path == Path.GetFileName(str) || path == Path.GetFileNameWithoutExtension(str));
        }
    }
}
