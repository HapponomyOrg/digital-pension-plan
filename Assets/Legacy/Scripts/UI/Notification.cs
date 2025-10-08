using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [Obsolete]
    public class Notification
    {
        public string Title { get; }
        public string Message { get; }
        public Action Action { get; }
        public NotificationColor NotificationColor { get; }

        public Notification(string title, string message, Action action, NotificationColor notificationColor)
        {
            Title = title;
            Message = message;
            Action = action;
            NotificationColor = notificationColor;
        }

    }

    public enum NotificationColor
    {
        Red,
        Green,
        Yellow
    }
}
