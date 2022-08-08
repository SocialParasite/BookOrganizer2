using Prism.Events;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class SearchEvent<T> : PubSubEvent<string> where T : class
    {
    }

    public class SearchEvent : PubSubEvent<SearchEventArgs>
    {
    }

    public class SearchEventArgs
    {
        public string SearchTerm { get; set; }
    }
}
