namespace BookOrganizer2.Domain.Shared
{
    public interface IIdentifiable<out T> where T: ValueObject
    {
        T Id { get; }
    }
}
