using System;
using TMPro;
using UnityEngine;
[Obsolete]
public class PlayerRecord : MonoBehaviour
{
    public DateTime LastContact;
    [SerializeField] public TMP_Text PlayerID;
    [SerializeField] public TMP_Text PlayerName;
    [SerializeField] public TMP_Text Balance;
    [SerializeField] public TMP_Text Cards;
    [SerializeField] public TMP_Text Points;
}
