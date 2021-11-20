using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.Services;
using System;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class AuthorWrapper : BaseWrapper<Author, AuthorId>
    {
        public AuthorWrapper(Author model) : base(model) { }

        public string FirstName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public DateTime? DateOfBirth
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public string MugshotPath
        {
            get => GetValue<string>();
            set => SetValue(DomainHelpers.SetPicturePath(value, "AuthorPics"));
        }

        public string Biography
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string NotesOld
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        
        public Guid NationalityId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }
    }
}
