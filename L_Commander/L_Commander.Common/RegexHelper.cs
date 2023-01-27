using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace L_Commander.Common
{
    public static class RegexHelper
    {
        public static bool ValidateRegex(string regex)
        {
            if (string.IsNullOrEmpty(regex))
            {
                return false;
            }

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Regex(regex);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static bool CheckIfMatches(string input, string pattern, RegexOptions options) =>
            Regex.IsMatch(input, pattern, options);

        public static IList<Match> GetMatches(string input, string pattern, RegexOptions options) =>
            Regex.Matches(input, pattern, options);

        public static string Replace(string input, string pattern, string replacement, RegexOptions options) =>
            Regex.Replace(input, pattern, replacement, options);
    }
}
