using System;

namespace BookOrganizer2.Domain.PublisherProfile
{
    public static class Events
    {
        public class PublisherCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string LogoPath { get; set; }
            public string Description { get; set; }
        }

        public class AuthorUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string LogoPath { get; set; }
            public string Description { get; set; }
        }

        public class PublishersNameChanged
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class PublishersLogoPathChanged
        {
            public Guid Id { get; set; }
            public string LogoPath { get; set; }
        }
        public class PublishersDescriptionChanged
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class PublisherDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
