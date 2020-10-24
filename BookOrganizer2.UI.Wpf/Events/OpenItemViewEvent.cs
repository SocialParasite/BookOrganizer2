using Prism.Events;
using System;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class OpenItemViewEvent<T> : PubSubEvent<Guid> where T : class
    {
    }

    public class OpenItemViewEvent : PubSubEvent<OpenItemViewEventArgs>
    {
    }

    public class OpenItemViewEventArgs
    {
        public string ViewModelName { get; set; }
    }
}
