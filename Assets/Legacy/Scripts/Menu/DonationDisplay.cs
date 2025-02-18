using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class DonationDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerPoints;
    [SerializeField] private Image background;

    [SerializeField] private Color backgroundLight;
    [SerializeField] private Color backgroundDark;

    [field: SerializeField] public Button DonateButton { get; private set; }

    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;

    public void SetDisplay(PlayerData player)
    {
        playerName.text = player.Name;
        playerPoints.text = player.Points.ToString();
    }

    public void SetBackground(bool dark)
    {
        background.color = dark ? backgroundDark : backgroundLight;
    }
}
