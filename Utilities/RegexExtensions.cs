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
        /// <summary>
        /// Regex extension method to check for any match within a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">Object type contained in list, must be <see cref="DatabaseObject"/></typeparam>
        /// <param name="rgx"><see cref="Regex"/> object to use for matching</param>
        /// <param name="strings"><see cref="List{T}"/> of strings to check against <paramref name="rgx"/></param>
        /// <returns>True if <paramref name="rgx"/> matches any string in <paramref name="strings"/>, false otherwise</returns>
        public static bool IsMatchList<T>(this Regex rgx, List<T> strings) where T : DatabaseObject
        {
            if (strings == null) return false;
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
