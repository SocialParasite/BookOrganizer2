using BookOrganizer2.Domain.BookProfile.LanguageProfile;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class LanguageWrapper : BaseWrapper<Language, LanguageId>
    {
        public LanguageWrapper(Language model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
