using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
[Obsolete]
public class PointDonationoverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text points;

    private void OnEnable()
    {
        string pointText = PlayerManager.Instance.Points > 1 ? "Pension Points" : "Pension Point";
        string oneOrMore = PlayerManager.Instance.Points > 1 ? "one or more" : "your";
        string oneOf = PlayerManager.Instance.Points > 1 ? "" : "one of ";
        points.text = $"Currently you have {PlayerManager.Instance.Points} {pointText}, \n Do you want to donate {oneOrMore} {pointText} to {oneOf}the other players?";
    }
}
