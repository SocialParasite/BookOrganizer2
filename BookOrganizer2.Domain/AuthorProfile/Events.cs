using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Common;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public static class Events
    {
        public class AuthorCreated
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MugshotPath { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Biography { get; set; }
            public string NotesOld { get; set; }
            public Nationality Nationality { get; set; }
            public ICollection<Note> Notes { get; set; }
        }

        public class AuthorUpdated
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MugshotPath { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Biography { get; set; }
            public string NotesOld { get; set; }
            public Nationality Nationality { get; set; }
            public ICollection<Note> Notes { get; set; }
        }

        public class AuthorsFirstNameChanged
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
        }
        public class AuthorsLastNameChanged
        {
            public Guid Id { get; set; }
            public string LastName { get; set; }
        }

        public class AuthorsMugshotPathChanged
        {
            public Guid Id { get; set; }
            public string MugshotPath { get; set; }
        }

        public class AuthorDateOfBirthChanged
        {
            public Guid Id { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class AuthorsBiographyChanged
        {
            public Guid Id { get; set; }
            public string Biography { get; set; }
        }

        public class AuthorsNotesOldChanged
        {
            public Guid Id { get; set; }
            public string NotesOld { get; set; }
        }

        public class AuthorsNotesChanged
        {
            public Guid Id { get; set; }
            public ICollection<Note> Notes { get; set; }
        }

        public class NationalityChanged
        {
            public Guid Id { get; set; }
            public Nationality Nationality { get; set; }
        }

        public class AuthorDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
