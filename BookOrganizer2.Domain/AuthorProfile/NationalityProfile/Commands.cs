﻿using System;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Delete
        {
            public Guid Id { get; set; }
        }
    }
}
