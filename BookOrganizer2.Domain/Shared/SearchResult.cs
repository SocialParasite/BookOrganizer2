using System;

namespace BookOrganizer2.Domain.Shared
{
    public class SearchResult
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
        public string ParentType { get; init; }
    }
}
