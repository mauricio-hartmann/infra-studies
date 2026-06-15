using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace IS.Core.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeToUpper(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category == UnicodeCategory.NonSpacingMark)
                    continue;

                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                    builder.Append(c);
            }

            return Regex.Replace(builder.ToString(), @"\s+", " ")
                        .Trim()
                        .ToUpperInvariant();
        }
    }
}
