using BookOrganizer2.Domain.BookProfile.GenreProfile;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class GenreWrapper : BaseWrapper<Genre, GenreId>
    {
        public GenreWrapper(Genre model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
