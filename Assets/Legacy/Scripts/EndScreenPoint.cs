using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class EndScreenPoint : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerPoints;
    [SerializeField] private Image background;

    [SerializeField] private Color backgroundLight;
    [SerializeField] private Color backgroundDark;


    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;

    public void SetDisplay(PlayerDataEnd player)
    {
        playerName.text = player.Name;
        for (int i = 0; i < player.Points.Length; i++)
        {
            var _points = Instantiate(playerPoints, this.gameObject.transform);
            _points.text = player.Points[i].ToString();
        }
    }

    public void SetBackground(bool dark)
    {
        background.color = dark ? backgroundDark : backgroundLight;
    }
}
