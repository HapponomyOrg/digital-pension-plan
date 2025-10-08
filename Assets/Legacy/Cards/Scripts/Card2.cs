using System;
using UnityEngine;

[Obsolete]
[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card2 : ScriptableObject
{
    [field: SerializeField] public byte ID { get; private set;}
    [field:SerializeField]public string Name { get; private set; }
    [field: SerializeField] public int Value { get; set; }
    [field: SerializeField] public Sprite Art { get; private set; }
}

