﻿namespace BookOrganizer2.DA.Repositories.Shared
{
    public static class StringExtensions
    {
        public static string EquallyDividedSubstring(this string text, string searchTerm)
        {
            var index = text.IndexOf(searchTerm);
            var maxLength = 50;
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
