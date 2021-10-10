using System;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// It cleans special characters from input string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A cleaned string containing only lower and upper case letters, numbers, points and underscoar </returns>
        public static string Cleaned(this string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z\. _]", string.Empty);
        }

        public static DateTime AsDateTime(this string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return DateTime.MinValue;
            }
            int.TryParse(input.Substring(0, 4), out var anno);
            int.TryParse(input.Substring(5, 2), out var mes);
            int.TryParse(input.Substring(8, 2), out var dia);

            int.TryParse(input.Substring(11, 2), out var hora);
            int.TryParse(input.Substring(14, 2), out var minutos);
            int.TryParse(input.Substring(17, 2), out var segundos);

            return new DateTime(anno, mes, dia, hora, minutos, segundos);
        }
    }
}
