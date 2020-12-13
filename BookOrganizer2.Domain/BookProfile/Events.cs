using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.BookProfile
{
    public static class Events
    {
        public class BookCreated
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

        public class BookUpdated
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

        public class TitleChanged
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public class ReleaseYearChanged
        {
            public Guid Id { get; set; }
            public int ReleaseYear { get; set; }
        }

        public class PageCountChanged
        {
            public Guid Id { get; set; }
            public int PageCount { get; set; }
        }

        public class WordCountChanged
        {
            public Guid Id { get; set; }
            public int WordCount { get; set; }
        }

        public class IsbnChanged
        {
            public Guid Id { get; set; }
            public string Isbn { get; set; }
        }
        public class BookCoverPathChanged
        {
            public Guid Id { get; set; }
            public string BookCoverPath { get; set; }
        }

        public class DescriptionChanged
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class NotesChanged
        {
            public Guid Id { get; set; }
            public string Notes { get; set; }
        }

        public class IsReadChanged
        {
            public Guid Id { get; set; }
            public bool IsRead { get; set; }
        }

        public class LanguageChanged
        {
            public Guid Id { get; set; }
            public Language Language { get; set; }
        }

        public class PublisherChanged
        {
            public Guid Id { get; set; }
            public Publisher Publisher { get; set; }
        }

        public class AuthorsChanged
        {
            public Guid Id { get; set; }
            public ICollection<Author> Authors { get; set; }
        }

        public class BookReadDatesChanged
        {
            public Guid Id { get; set; }
            public ICollection<BookReadDate> BookReadDates { get; set; }
        }

        public class FormatsChanged
        {
            public Guid Id { get; set; }
            public ICollection<Format> Formats { get; set; }
        }

        public class GenresChanged
        {
            public Guid Id { get; set; }
            public ICollection<Genre> Genres { get; set; }
        }

        public class BookDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
