using UnityEngine;

namespace Version1.Cards.Scripts
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Version2/Cards/CardData")]
    public class CardData: ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set;}
        [field:SerializeField]public string Name { get; private set; }
        [field: SerializeField] public int Value { get; private set; }
        [field: SerializeField] public Sprite Art { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        
    }
}