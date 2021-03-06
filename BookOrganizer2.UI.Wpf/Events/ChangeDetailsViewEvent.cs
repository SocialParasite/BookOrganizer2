﻿using System.Windows.Media;
using Prism.Events;

namespace BookOrganizer2.UI.Wpf.Events
{
    public class ChangeDetailsViewEvent : PubSubEvent<ChangeDetailsViewEventArgs> { }

    public class ChangeDetailsViewEventArgs
    {
        public string Message { get; set; }
        public SolidColorBrush MessageBackgroundColor { get; set; }
    }
}
