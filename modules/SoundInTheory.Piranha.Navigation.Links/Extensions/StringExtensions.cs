using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceEnd(this string input, string oldValue, string newValue, bool caseSensitive = true)
        {
            if (input == null || string.IsNullOrWhiteSpace(oldValue))
            {
                return input;
            }

            return Regex.Replace(input, $@"{Regex.Escape(oldValue)}$", newValue ?? "", caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
        }

        public static string RemoveUrlPath(this string url, string path)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(path))
            {
                return url;
            }

            return url
                .TrimEnd('/')
                .ReplaceEnd(path.Trim('/'), "", caseSensitive: false)
                .TrimEnd('/');
        }

        public static string AppendUrlPath(this string input, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return input ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return "/" + path.Trim('/');
            }

            return input.TrimEnd('/') + "/" + path.Trim('/');
        }
    }
}
