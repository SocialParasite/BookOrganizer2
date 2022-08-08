using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using System;
using System.Linq;

namespace BookOrganizer2.DA.Repositories.Shared
{
    public static class DataHelpers
    {
        public static string GetBookContent(Book book, string searchTerm, int substringLength)
        {
            if (book.Notes.Any(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            {
                var note = book.Notes.First(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Title.EquallyDividedSubstring(searchTerm, substringLength);
                return note;
            }

            if (book.Notes.Any(n => n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            {
                var note = book.Notes.First(b => b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Content.EquallyDividedSubstring(searchTerm, substringLength);
                return note;
            }

            if (book.Description is not null && book.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                return book.Description.EquallyDividedSubstring(searchTerm, substringLength);
            }

            return book.Title;
        }

        public static string GetAuthorContent(Author author, string searchTerm, int substringLength = 50)
        {
            if (author.Notes.Any(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            {
                var note = author.Notes.First(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Title.EquallyDividedSubstring(searchTerm, substringLength);
                return note;
            }

            if (author.Notes.Any(n => n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            {
                var note = author.Notes.First(b => b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Content.EquallyDividedSubstring(searchTerm, substringLength);
                return note;
            }

            if (author.Biography is not null && author.Biography.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                return author.Biography.EquallyDividedSubstring(searchTerm, substringLength);
            }

            return $"{author.LastName}, {author.FirstName}";
        }
    }
}
