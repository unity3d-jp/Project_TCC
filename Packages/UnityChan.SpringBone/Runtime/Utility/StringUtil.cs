using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UTJ
{
    public class StringUtil
    {
        // OS-style glob matching
        public static bool GlobMatch(string stringToCheck, string pattern)
        {
            // http://stackoverflow.com/a/4146349
            return new Regex(
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(stringToCheck);
        }

        // Find first glob match in a list; returns null if no match
        public static string GlobFind(IEnumerable<string> stringsToCheck, string pattern)
        {
            foreach (var stringToCheck in stringsToCheck)
            {
                if (GlobMatch(stringToCheck, pattern))
                {
                    return stringToCheck;
                }
            }
            return null;
        }

        // Find all glob matches in a list
        public static List<string> GlobFindAll(IEnumerable<string> stringsToCheck, string pattern)
        {
            var matchedStrings = new List<string>();
            foreach (var stringToCheck in stringsToCheck)
            {
                if (GlobMatch(stringToCheck, pattern))
                {
                    matchedStrings.Add(stringToCheck);
                }
            }
            return matchedStrings;
        }
    }
}