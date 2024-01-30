
using System.Linq;
using System.Text.RegularExpressions;

namespace ThePocket
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            var x = str.Replace("_", "");
            if (x.Length == 0) return "null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])", m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
            return char.ToLower(x[0]) + x[1..];
        }

        public static string ToPascalCase(this string word)
        {
            return string.Join("" , word.Split('_')
                .Select(w => w.Trim())
                .Where(w => w.Length > 0)
                .Select(w => w.Substring(0,1).ToUpper() + w.Substring(1).ToLower()));
        }
    }
}