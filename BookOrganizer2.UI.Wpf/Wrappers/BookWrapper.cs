using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.Services;
using System;
using System.Collections.Generic;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class BookWrapper : BaseWrapper<Book, BookId>
    {
        public BookWrapper(Book model) : base(model) { }

        public string Title
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public int ReleaseYear
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int PageCount
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int WordCount
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Isbn
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string BookCoverPath
        {
            get => GetValue<string>();
            set => SetValue(DomainHelpers.SetPicturePath(value, "Covers"));
        }

        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string NotesOld
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool IsRead
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        //public Guid LanguageId
        //{
        //    get => GetValue<Guid>();
        //    set => SetValue(value);
        //}

        //public Guid PublisherId
        //{
        //    get => GetValue<Guid>();
        //    set => SetValue(value);
        //}

        //public List<Guid> AuthorIds
        //{
        //    get => GetValue<List<Guid>>();
        //    set => SetValue(value);
        //}

        //public List<Guid> FormatIds
        //{
        //    get => GetValue<List<Guid>>();
        //    set => SetValue(value);
        //}

        //public List<Guid> GenreIds
        //{
        //    get => GetValue<List<Guid>>();
        //    set => SetValue(value);
        //}
    }
}
