using System.IO;

namespace BookOrganizer2.Domain.DA.Conditions
{
    public enum BookFilterCondition
    {
        NoFilter,
        NoDescription,
        PlaceholderCover,
        NoAuthors,
        NoPublisher
    }

    public enum AuthorFilterCondition
    {
        NoFilter,
        NoDateOfBirth,
        NoMugshot,
        NoNationality,
        NoBooks,
        NoBio
    }

    public enum PublisherFilterCondition
    {
        NoFilter,
        NoLogoPicture,
        NoBooks,
        NoDescription
    }
}
