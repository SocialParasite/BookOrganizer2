using System;

namespace BookOrganizer2.Domain.BookProfile.LanguageProfile
{
    public static class Events
    {
        public class LanguageCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class LanguageUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class LanguageDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
