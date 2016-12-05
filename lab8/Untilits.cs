using System.Linq;

namespace lab8
{
    public static class Untilits
    {
            public static string NextTo(this string[] str, string pat)
            {
                for (var i = 0; i < str.Length - 1; ++i)
                    if (str[i].Contains(pat)) return str[i + 1];
                return "";
            }

        public static bool IsPresent(this string[] str, string pat) 
                => str.Any(t => t.Contains(pat));
    }
}