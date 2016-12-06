using System.Linq;
using System.Text.RegularExpressions;

namespace lab8
{
    public static class Untilits
    {
        public static string NextTo(this string[] str, Regex pat)
        {
            for (var i = 0; i < str.Length - 1; i++)
                if (pat.IsMatch(str[i])) return str[i + 1];
            return "";
        }

        public static string PrevTo(this string[] str, Regex pat)
        {
            for (var i = 0; i < str.Length; i++)
                if (pat.IsMatch(str[i])) return str[i - 1];
            return "";
        }

        public static bool IsPresent(this string[] str, Regex pat)
                => str.Any(pat.IsMatch);

        public static string NextTo(this string[] str, string s) => str.NextTo(new Regex(s));

        public static string PrevTo(this string[] str, string s) => str.PrevTo(new Regex(s));

        public static bool IsPresent(this string[] str, string s) => str.IsPresent(new Regex(s));

        public static string TakeName(this string[] str, string s)
        {
            var res = "";
            for (var i = str.ToList().FindIndex(t => t.Contains(s)) + 1; i < str.Length; ++i)
                res += str[i];
            return res;
        }
    }
}