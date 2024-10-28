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
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str?.IndexOf(value, 0, comparisonType) != -1;
        }

        public static bool MatchesPath(this string str, string path)
        {
            return (path == str || path == Path.GetFileName(str) || path == Path.GetFileNameWithoutExtension(str));
        }
    }
}
