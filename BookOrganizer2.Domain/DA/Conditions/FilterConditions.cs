namespace BookOrganizer2.Domain.DA.Conditions
{
    public enum BookMaintenanceFilterCondition
    {
        NoFilter,
        NoDescription,
        PlaceholderCover,
        NoAuthors,
        NoPublisher
    }

    public enum AuthorMaintenanceFilterCondition
    {
        NoFilter,
        NoDateOfBirth,
        NoMugshot,
        NoNationality,
        NoBooks,
        NoBio
    }

    public enum PublisherMaintenanceFilterCondition
    {
        NoFilter,
        NoLogoPicture,
        NoBooks,
        NoDescription
    }

    public enum SeriesMaintenanceFilterCondition
    {
        NoFilter,
        NoPicture,
        NoBooks,
        NoDescription
    }

    public enum SeriesFilterCondition
    {
        NoFilter,
        NotStarted,
        PartlyRead,
        NotFullyOwned
    }
}
