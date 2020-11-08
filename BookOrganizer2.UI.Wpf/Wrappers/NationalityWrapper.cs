using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class NationalityWrapper : BaseWrapper<Nationality, NationalityId>
    {
        public NationalityWrapper(Nationality model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
