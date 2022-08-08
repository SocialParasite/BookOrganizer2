using System;

namespace BookOrganizer2.DA.Repositories.Shared
{
    public static class StringExtensions
    {
        public static string EquallyDividedSubstring(this string text, string searchTerm, int substringLength = 50)
        {
            var index = text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
            var maxLength = substringLength;
            var contentLength = text.Length;

            if (maxLength > contentLength)
            {
                maxLength = contentLength;
            }

            if (index <= maxLength / 2)
            {
                index = 0;
            }
            else
            {
                index -= maxLength / 2;

                if (index + maxLength > contentLength)
                {
                    maxLength = contentLength - index;
                }
            }

            var result = text.Substring(index, maxLength);
            return result;
        }
    }
}
