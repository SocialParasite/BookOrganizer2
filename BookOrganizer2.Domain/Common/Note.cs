using System;

namespace BookOrganizer2.Domain.Common
{
    public sealed class Note
    {
        public NoteId Id { get; private set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
