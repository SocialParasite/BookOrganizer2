using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.BookProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int ReleaseYear { get; set; }
            public int PageCount { get; set; }
            public int WordCount { get; set; }
            public string Isbn { get; set; }
            public string BookCoverPath { get; set; }
            public string Description { get; set; }
            public string Notes { get; set; }
            public bool IsRead { get; set; }
            public Language Language { get; set; }
            public Publisher Publisher { get; set; }
            public ICollection<Author> Authors { get; set; }
            public ICollection<BookReadDate> BookReadDates { get; set; }
            public ICollection<Format> Formats { get; set; }
            public ICollection<Genre> Genres { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int ReleaseYear { get; set; }
            public int PageCount { get; set; }
            public int WordCount { get; set; }
            public string Isbn { get; set; }
            public string BookCoverPath { get; set; }
            public string Description { get; set; }
            public string Notes { get; set; }
            public bool IsRead { get; set; }
            public Language Language { get; set; }
            public Publisher Publisher { get; set; }
            public ICollection<Author> Authors { get; set; }
            public ICollection<BookReadDate> BookReadDates { get; set; }
            public ICollection<Format> Formats { get; set; }
            public ICollection<Genre> Genres { get; set; }
        }

        public class SetTitle
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public class SetReleaseYear
        {
            public Guid Id { get; set; }
            public int ReleaseYear { get; set; }
        }

        public class SetPageCount
        {
            public Guid Id { get; set; }
            public int PageCount { get; set; }
        }

        public class SetWordCount
        {
            public Guid Id { get; set; }
            public int WordCount { get; set; }
        }

        public class SetIsbn
        {
            public Guid Id { get; set; }
            public string Isbn { get; set; }
        }
        public class SetBookCoverPath
        {
            public Guid Id { get; set; }
            public string BookCoverPath { get; set; }
        }

        public class SetDescription
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class SetNotes
        {
            public Guid Id { get; set; }
            public string Notes { get; set; }
        }

        public class SetIsRead
        {
            public Guid Id { get; set; }
            public bool IsRead { get; set; }
        }

        public class SetLanguage
        {
            public Guid Id { get; set; }
            public Language Language { get; set; }
        }

        public class SetPublisher
        {
            public Guid Id { get; set; }
            public Publisher Publisher { get; set; }
        }
        
        public class SetAuthors
        {
            public Guid Id { get; set; }
            public ICollection<Author> Authors { get; set; }
        }

        public class SetBookReadDates
        {
            public Guid Id { get; set; }
            public ICollection<BookReadDate> BookReadDates { get; set; }
        }

        public class SetFormats
        {
            public Guid Id { get; set; }
            public ICollection<Format> Formats { get; set; }
        }

        public class SetGenres
        {
            public Guid Id { get; set; }
            public ICollection<Genre> Genres { get; set; }
        }

        public class Delete
        {
            public Guid Id { get; set; }
        }
    }
}
