using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Services;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class PublisherWrapper : BaseWrapper<Publisher, PublisherId>
    {
        public PublisherWrapper(Publisher model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        
        public string LogoPath
        {
            get => GetValue<string>();
            set => SetValue(DomainHelpers.SetPicturePath(value, "PublisherLogos"));
        }

        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
