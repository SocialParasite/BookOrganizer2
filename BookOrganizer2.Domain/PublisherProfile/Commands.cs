using System;

namespace BookOrganizer2.Domain.PublisherProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string LogoPath { get; set; }
            public string Description { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string LogoPath { get; set; }
            public string Description { get; set; }

        }

        public class SetName
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class SetLogoPath
        {
            public Guid Id { get; set; }
            public string LogoPath { get; set; }
        }
        public class SetDescription
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }
        public class DeletePublisher
        {
            public Guid Id { get; set; }
        }
    }
}
