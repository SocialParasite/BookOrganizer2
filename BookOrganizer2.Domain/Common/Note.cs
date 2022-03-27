using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.Common
{
    public interface INote
    {
        NoteId Id { get; }
        string Title { get; set; }
        string Content { get; set; }
    }

    public sealed class Note : INote
    {
        public NoteId Id { get; private set; }
        public string Title { get; set; }
        public string Content { get; set; }

        private Note() { }

        public static Note NewNote => new() { Id = new NoteId(SequentialGuid.NewSequentialGuid()) };
    }
}
