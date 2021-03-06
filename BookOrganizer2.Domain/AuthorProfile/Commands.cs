﻿using System;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MugshotPath { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Biography { get; set; }
            public string Notes { get; set; }
            public Nationality Nationality { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MugshotPath { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Biography { get; set; }
            public string Notes { get; set; }
            public Nationality Nationality { get; set; }
        }

        public class SetAuthorsFirstName
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
        }

        public class SetAuthorsLastName
        {
            public Guid Id { get; set; }
            public string LastName { get; set; }
        }

        public class SetMugshotPath
        {
            public Guid Id { get; set; }
            public string MugshotPath { get; set; }
        }

        public class SetAuthorDateOfBirth
        {
            public Guid Id { get; set; }
            public DateTime DataOfBirth { get; set; }
        }

        public class SetBiography
        {
            public Guid Id { get; set; }
            public string Biography { get; set; }
        }

        public class SetNotes
        {
            public Guid Id { get; set; }
            public string Notes { get; set; }
        }

        public class SetNationality
        {
            public Guid Id { get; set; }
            public Guid NationalityId { get; set; }
        }

        public class DeleteAuthor
        {
            public Guid Id { get; set; }
        }
    }
}
