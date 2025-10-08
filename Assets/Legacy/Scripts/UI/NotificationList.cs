using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [Obsolete]
    public class NotificationList : MonoBehaviour
    {
        [SerializeField] private NotificationDisplay NotificationDisplayPrefab;

        public static NotificationList Instance { get; private set; }

        private readonly Queue<NotificationDisplay> notifications = new Queue<NotificationDisplay>();

        [SerializeField] private Transform notificationStack;
        [SerializeField] private Transform nextNotification;
        [SerializeField] private Transform displayedNotification;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            AddNotification(new Notification("test", "bas", () => print("test action"), NotificationColor.Yellow));
            AddNotification(new Notification("Sell", "kaartje ", null, NotificationColor.Green));
            AddNotification(new Notification("heudg", "omhg", null, NotificationColor.Red));
            AddNotification(new Notification("greerf", "bril", null, NotificationColor.Red));
            AddNotification(new Notification("jegeghe", "pindakaas", null, NotificationColor.Yellow));
            AddNotification(new Notification("vishengel", "tekoop hand", null, NotificationColor.Green));
        }

        public void AddNotification(Notification notification)
        {
            var pos = notifications.Count switch
            {
                > 1 => notificationStack.position,
                > 0 => nextNotification.position,
                _ => displayedNotification.position
            };

            var display = Instantiate(NotificationDisplayPrefab, pos, Quaternion.identity, transform);
            display.name = notifications.Count.ToString();
            display.SetDisplay(this, notification);
            notifications.Enqueue(display);
        }

        public void RemoveNotification()
        {
            var display = notifications.Dequeue();
            Destroy(display.gameObject);
            MoveNotifications();
        }

        private void MoveNotifications()
        {
            var notifs = notifications.ToArray();

            for (var i = 0; i < notifs.Length; i++)
            {
                var pos = i switch
                {
                    > 1 => notificationStack.position,
                    > 0 => nextNotification.position,
                    _ => displayedNotification.position
                };

                notifs[i].transform.position = pos;
            }
        }
    }
}

