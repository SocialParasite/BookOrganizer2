namespace BookOrganizer2.Domain.Shared
{
    public interface IIdentifiable<T> where T: ValueObject
    {
        T Id { get; }
    }
}
