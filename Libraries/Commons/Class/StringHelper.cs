
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace Common.Library;


    /// <summary>
    /// Represents an helper for <see cref="string"/> class.
    /// </summary>
    public static partial class StringHelper
    {
        /// <summary>
        /// Represents the comparer to use to compare two <see cref="string"/> together.
        /// </summary>
        private static readonly StringComparer _ordinalStringComparer = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Represents the disallowed character on a filename.
        /// </summary>
        private static readonly string disallowedCharacters = "/\\:*?\"<>|\t\r\n";

        private static readonly Regex _regex = MyRegex();

        #region Methods

        /// <summary>
        /// Check if every couple value in specified <paramref name="values"/> are equal.
        /// </summary>
        /// <param name="values">Array of couple values to check.</param>
        /// <returns>Returns <see langword="true"/> if all couple values are equal,
        ///  <see langword="false"/> otherwise.</returns>
        public static bool Equals(
            params (string?, string?)[] values)
        {
            if (values is null || values.Length == 0)
            {
                return false;
            }

            if (values.Any(value => !_ordinalStringComparer.Equals(value.Item1, value.Item2)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if any value of the specified <paramref name="values"/> is <see langword="null"/> or
        ///  <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="values">Array of values to check.</param>
        /// <returns>Returns <see langword="true"/> if any of the specified <paramref name="values"/>
        ///  is <see langword="null"/> or <see cref="string.Empty"/>, <see langword="false"/> otherwise.</returns>
        public static bool IsAnyNullOrEmpty(params string?[] values)
        {
            if (values is null || values.Length == 0)
            {
                return false;
            }

            return values.Any(string.IsNullOrEmpty);
        }

        /// <summary>
        /// Format the <paramref name="input"/> value with the first character as an upper character.
        /// </summary>
        /// <param name="input">Value to format.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Occurs when the <paramref name="input"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Occurs when the <paramref name="input"/> is not <see langword="null" /> but is empty.</exception>
        public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };



        public static string RemoveIntegratedImageInEmail(this string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return MyRegex().Replace(value, string.Empty);
        }

        [GeneratedRegex("\\[(http|https)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_]*)?([a-zA-Z0-9\\-\\?\\,\\'\\/\\+&%\\$#_]+)\\]")]
        private static partial Regex MyRegex();


        public static bool ContainsSpecialCaraters(this string text)
        {

            List<char> _chars = new List<char>() { '@', '-', '!', '_','?', '&' ,'.' };
            bool l_ret = false;
            if( text == null)
            {
                return false;
            }
            foreach(var c in _chars)
            {
                l_ret |= text.Contains(c);  
            }

            return l_ret;   
        }



        public static string RemoveSpecialCaraters(this string text)
        {

            List<char> _chars = new List<char>() { '@', '-', '!', '_', '?', '&','.',':',';',':' };
            string  l_ret = text;
            if (text == null)
            {
                return text;
            }
            foreach (var c in _chars)
            {
                l_ret = l_ret.Replace($"{c}", " ");
            }

            return l_ret;
        }



        public static string RemoveSpecialCaraters(this string text, params char[] chars)
        {

           
            string  l_ret = text;
            if (text == null)
            {
                return text;
            }
            foreach (var c in chars)
            {
                l_ret = l_ret.Replace($"{c}", " ");
            }

            return l_ret;
        }



        #endregion Methods
    }

