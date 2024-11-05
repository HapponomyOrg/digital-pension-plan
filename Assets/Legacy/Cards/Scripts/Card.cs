using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    [field: SerializeField] public byte ID { get; private set;}
    [field:SerializeField]public string Name { get; private set; }
    //TODO set this to private again
    [field: SerializeField] public int Value { get; set; }
    [field: SerializeField] public Sprite Art { get; private set; }
}

