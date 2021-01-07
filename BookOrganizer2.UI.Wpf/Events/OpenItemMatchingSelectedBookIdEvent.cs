using Prism.Events;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class OpenItemMatchingSelectedBookIdEvent<Guid> : PubSubEvent<Guid>
    {
    }
}
