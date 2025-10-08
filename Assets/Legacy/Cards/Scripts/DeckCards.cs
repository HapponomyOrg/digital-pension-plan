using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Obsolete]
[Serializable]
public class DeckCards
{
    [field: SerializeField] public Card card { get; private set; }
    [field: SerializeField] public int amount { get; private set; }
}
