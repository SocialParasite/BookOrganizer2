using System;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public static class Events
    {
        public class NationalityCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class NationalityUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class NationalityNameChanged
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        
        public class NationalityDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
