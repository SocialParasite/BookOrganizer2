using System;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public static class Events
    {
        public class FormatCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class FormatUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class FormatDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
