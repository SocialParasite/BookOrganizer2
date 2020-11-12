using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class FormatWrapper : BaseWrapper<Format, FormatId>
    {
        public FormatWrapper(Format model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
