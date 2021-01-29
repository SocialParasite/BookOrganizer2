using System;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public static class Events
    {
        public class Created
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Updated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        
        public class Deleted
        {
            public Guid Id { get; set; }
        }
    }
}
