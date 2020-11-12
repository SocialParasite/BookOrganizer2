﻿using System;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
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

        public class DeleteFormat
        {
            public Guid Id { get; set; }
        }
    }
}
