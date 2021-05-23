using Prism.Events;
using System;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class StatusMessageChangedEvent<T> : PubSubEvent<Guid> where T : class
    {
    }

    public class StatusMessageChangedEvent : PubSubEvent<StatusMessageChangedEventArgs>
    {
    }
    public class StatusMessageChangedEventArgs
    {
        public string InfoText { get; set; }
        public int NumberOfItems { get; set; }
        public int AllItemsCount { get; set; }
    }
}
