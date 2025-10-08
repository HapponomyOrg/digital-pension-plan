using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Cards.Scripts;

namespace Version1.Host.Scripts
{
    /*public partial class CardGame : MonoBehaviour
    {

       [SerializeField] private Cards.Scripts.CardLibrary cardLibrary;

        /*private static readonly Dictionary<CardRarity, List<CardData>> CardCategories = new()
        {
            { CardRarity.COMMON, new List<> {, "bakery", "construction", "gym", "carpenter", "butcher" } },
            { CardRarity.UNCOMMON, new List<string> { "bus", "powerplant", "hospital", "library", "package" } },
            { CardRarity.RARE, new List<string> { "hightech", "space", "asteroid" } },
            { CardRarity.ULTRARARE, new List<string> { "warpdrive" } }
        };#1#

        private void Start()
        {
            var test = CreateDeck(20);
            Debug.Log(test);
        }


        private static readonly Dictionary<CardRarity, double> CardsPerPlayer = new()
        {
            { CardRarity.COMMON, 8 },
            { CardRarity.UNCOMMON, 5.34 },
            { CardRarity.RARE, 2.4 },
            { CardRarity.ULTRARARE, 0.54 }
        };

        private static readonly Dictionary<CardRarity, int> CardPoints = new()
        {
            { CardRarity.COMMON, 1 },
            { CardRarity.UNCOMMON, 2 },
            { CardRarity.RARE, 3 },
            { CardRarity.ULTRARARE, 5 }
        };

        private const int MinPointsPerPlayer = 7;
        private static readonly double AvgCards = 244.0 / 15;

        public Dictionary<CardData, int> CreateDeck(int numPlayers)
        {
            /*foreach (var card in cardLibrary.cards)
            {
            }#1#
            
            Dictionary<CardData, int> deck = InitializeDeck();
            int minCards = (int)Math.Floor(AvgCards * numPlayers);
            int minPoints = MinPointsPerPlayer * numPlayers;
            int totalCards = 0, totalPoints = 0;
            Dictionary<CardRarity, int> curIndices = Enum.GetValues(typeof(CardRarity))
                .Cast<CardRarity>()
                .ToDictionary(type => type, _ => 0);

            foreach (var rarity in Enum.GetValues(typeof(CardRarity)).Cast<CardRarity>())
            {
                int newSets = (int)Math.Floor(CardsPerPlayer[rarity] * numPlayers / 4);
                totalPoints += AddCardSets(deck, rarity, curIndices, newSets);
                totalCards += 4 * newSets;
            }

            int typeIndex = 0;
            CardRarity[] rarities = Enum.GetValues(typeof(CardRarity)).Cast<CardRarity>().ToArray();

            while (totalPoints < minPoints)
            {
                CardRarity rarity = rarities[typeIndex];
                if (curIndices[rarity] < CardsPerPlayer[rarity] * numPlayers)
                {
                    totalPoints += AddCardSets(deck, rarity, curIndices, 1);
                }

                typeIndex = (typeIndex + 1) % rarities.Length;
            }

            if (totalCards < minCards)
            {
                AddCardSets(deck, CardRarity.COMMON, curIndices, (int)Math.Round((minCards - totalCards) / 4.0));
            }

            return deck;
        }

        private Dictionary<CardData, int> InitializeDeck()
        {
            Dictionary<CardData, int> deck = new Dictionary<CardData, int>();
            foreach (var card in cardLibrary.cards)
            {
                    deck[card] = 0;
            }

            return deck;
        }

        private int AddCardSets(Dictionary<CardData, int> deck, CardRarity rarity,
            Dictionary<CardRarity, int> curIndices, int numSets)
        {
            int points = 0;

            foreach (var card in cardLibrary.cards)
            {
                /*if (card.Value == )
                {
                    
                }#1#
            }
            
            /*List<string> cards = CardCategories[rarity];#1#
            int curIndex = curIndices[rarity];

            for (int i = 0; i < numSets; i++)
            {
                if (curIndex >= cardLibrary.cards.Length) curIndex = 0;
                deck[cardLibrary.cards[curIndex]] += 4;
                points += CardPoints[rarity];
                curIndex++;
            }

            curIndices[rarity] = curIndex;
            return points;
        }
    }*/
}
