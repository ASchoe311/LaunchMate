using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaunchMate.Utilities
{
    public static class RegexExtensions
    {
        public static bool IsMatchList<T>(this Regex rgx, List<T> strings) where T : DatabaseObject
        {
            foreach (var item in strings)
            {
                if (rgx.IsMatch(item.Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
