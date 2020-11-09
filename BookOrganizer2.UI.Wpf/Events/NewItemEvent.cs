using Prism.Events;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class NewItemEvent : PubSubEvent<NewItemEventArgs> { }

    public class NewItemEventArgs
    {
        //public string ItemType { get; set; }
    }
}
