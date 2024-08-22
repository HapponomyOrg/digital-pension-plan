using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NotificationDisplay : MonoBehaviour
    {
        [SerializeField] private Image background;
    
        [SerializeField] private Button closeButton;
        [SerializeField] private Button actionButton;

        [SerializeField] private TMP_Text titleDisplay;
        [SerializeField] private TMP_Text messageDisplay;

        [SerializeField] private Color green = new Color(27, 159, 132);
        [SerializeField] private Color darkGreen = new Color(22, 130, 108);

        [SerializeField] private Color yellow = new Color(253, 182, 80);
        [SerializeField] private Color darkYellow = new Color(226, 163, 72);

        [SerializeField] private Color red = new Color(251, 99, 78);
        [SerializeField] private Color darkRed = new Color(222, 88, 69);

        public void SetDisplay(NotificationList notificationList, Notification notification)
        {
            var colors = GetColors(notification.NotificationColor);
            
            
            if (notification.Action == null)
                actionButton.gameObject.SetActive(false);
            else
            {
                actionButton.onClick.AddListener(notification.Action.Invoke);
                actionButton.onClick.AddListener(notificationList.RemoveNotification);
                actionButton.targetGraphic.color = colors.Item2;
            }

            closeButton.onClick.AddListener(notificationList.RemoveNotification);
            closeButton.targetGraphic.color = colors.Item2;
            
            
            background.color = colors.Item1;

            titleDisplay.text = notification.Title;
            messageDisplay.text = notification.Message;
        }

        private Tuple<Color, Color> GetColors(NotificationColor color)
        {
            return color switch
            {
                NotificationColor.Red => new Tuple<Color, Color>(red, darkRed),
                NotificationColor.Green => new Tuple<Color, Color>(green, darkGreen),
                NotificationColor.Yellow => new Tuple<Color, Color>(yellow, darkYellow),
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
    }
}
