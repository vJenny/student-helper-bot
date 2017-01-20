using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudentHelperBot.Utilits
{
    public static class Utilits
    {
        public static string TakeName(this string[] str) => 
            string.Join(" ", str.Skip(1).Select
                (ss => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ss.ToLower())));

        public static string TakeNextNumber(this string[] str) => 
            str.Length < 2 ? "" : str[1];
    }
}