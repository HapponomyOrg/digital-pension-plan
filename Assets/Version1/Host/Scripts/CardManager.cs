using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Cards.Scripts;
using Version1.Nats.Messages.Host;
using Random = UnityEngine.Random;

namespace Version1.Host.Scripts
{
    /// <summary>
    /// This script does al the logic of the cards for the host
    /// Call StartGame with the amount of players, and it will send the right amount of cards and money to the players.
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private Cards.Scripts.CardLibrary _cardLibrary;
        
        // This is the deck the host has for the game
        private List<CardData> _cardDeck;

        private void Start()
        {
            StartGame(new Dictionary<int, PlayerListPrefab>());
        }

        public CardManager(Cards.Scripts.CardLibrary cardLibrary)
        {
            _cardLibrary = cardLibrary;
        }

        public void StartGame(Dictionary<int,PlayerListPrefab> players)
        {
            _cardDeck = new List<CardData>();

            FillDeck(players.Count);
            ShuffleDeck();
            
            var cardsPerPlayer = CalculateCardsPerPlayer(players.Count);

            foreach (var player in players)
            {
                List<CardData> playerCards = TakeCards(cardsPerPlayer);

                int[] handCards = new int[playerCards.Count];

                for (int j = 0; j < playerCards.Count; j++)
                {
                    handCards[j] = playerCards[j].ID;
                }

                StartGameMessage msg;
                if (SessionData.Instance.InbalanceMode)
                {
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, player.Key,
                        CalculateBalancePerPlayer(player.Key), handCards, (int)SessionData.Instance.CurrentMoneySystem);
                }
                else
                {
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, player.Key, 6000,
                        handCards,
                        (int)SessionData.Instance.CurrentMoneySystem);
                }

                Debug.Log($"sent cards, msg: {msg}");
                
                Nats.NatsHost.C.Publish($"{SessionData.Instance.LobbyCode}", msg);
            }
        }

        private void FillDeck(int count)
        {
            var cardGame = new CardGame
            {
                cardLibrary = _cardLibrary
            };

            var deck = cardGame.CreateDeck(count);

            foreach (var card in deck)
            {
                for (var i = 0; i < card.Value; i++)
                {
                    _cardDeck.Add(card.Key);
                }
            }
        }

        private void ShuffleDeck()
        {
            Random.State originalState = Random.state;
            Random.InitState(SessionData.Instance.Seed);

            for (int i = 0; i < _cardDeck.Count; i++)
            {
                int randomIndex = Random.Range(i, _cardDeck.Count);
                (_cardDeck[i], _cardDeck[randomIndex]) = (_cardDeck[randomIndex], _cardDeck[i]);
            }

            Random.state = originalState;
        }

        private static int CalculateBalancePerPlayer(int playerNumber)
        {
            return (playerNumber % 4) switch
            {
                1 => 0,
                2 or 3 => 6000,
                0 => 14000,
                _ => 0
            };
        }

        private static int CalculateCardsPerPlayer(int numberOfPlayers)
        {
            return numberOfPlayers switch
            {
                <= 3 => 12,
                4 => 10,
                5 => 9,
                6 => 8,
                7 => 7,
                _ => 6  //TODO check dit
            };
        }

        public List<CardData> TakeCards(int amount)
        {
            List<CardData> takenCards = new List<CardData>();
            for (var i = 0; i < amount; i++)
            {
                if (_cardDeck.Count == 0)
                {
                    Debug.Log("Timo: Geen kaarten meer in de deck!");
                    return null;
                }

                CardData takenCard = _cardDeck[0];
                _cardDeck.RemoveAt(0);
                takenCards.Add(takenCard);
            }

            return takenCards;
        }
    }
    
    internal class CardGame
    {

        public Cards.Scripts.CardLibrary cardLibrary;
        
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
            }*/
            
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
                    
                }*/
            }
            
            /*List<string> cards = CardCategories[rarity];*/
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
    }
}

/*using System;
using System.Collections.Generic;
using UnityEngine;
using Version1.Cards.Scripts;
using Version1.Nats.Messages.Host;
using Random = UnityEngine.Random;

namespace Version1.Host.Scripts
{
    /// <summary>
    /// This script does al the logic of the cards for the host
    /// Call StartGame with the amount of players, and it will send the right amount of cards and money to the players.
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private Cards.Scripts.CardLibrary _cardLibrary;
        
        // This is the deck the host has for the game
        private List<CardData> _cardDeck;

        public CardManager(Cards.Scripts.CardLibrary cardLibrary)
        {
            _cardLibrary = cardLibrary;
        }

        public void StartGame(Dictionary<int,PlayerListPrefab> players)
        {
            _cardDeck = new List<CardData>();

            FillDeck();
            ShuffleDeck();

            var cardsPerPlayer = CalculateCardsPerPlayer(players.Count);

            foreach (var player in players)
            {
                List<CardData> playerCards = TakeCards(cardsPerPlayer);

                int[] handCards = new int[playerCards.Count];

                for (int j = 0; j < playerCards.Count; j++)
                {
                    handCards[j] = playerCards[j].ID;
                }

                StartGameMessage msg;
                if (SessionData.Instance.InbalanceMode)
                {
                    Debug.Log($"Players get something");
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, player.Key,
                        CalculateBalancePerPlayer(player.Key), handCards, (int)SessionData.Instance.CurrentMoneySystem);
                }
                else
                {
                    Debug.Log($"6000");
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, player.Key, 6000,
                        handCards,
                        (int)SessionData.Instance.CurrentMoneySystem);
                }

                Debug.Log($"sent cards, msg: {msg}");
                
                Nats.NatsHost.C.Publish($"{SessionData.Instance.LobbyCode}", msg);
            }
        }

        private void FillDeck()
        {
            foreach (var card in _cardLibrary.cards)
            {
                for (var i = 0; i < card.Amount; i++)
                {
                    _cardDeck.Add(card);
                }
            }
        }

        private void ShuffleDeck()
        {
            Random.State originalState = Random.state;
            Random.InitState(SessionData.Instance.Seed);

            for (int i = 0; i < _cardDeck.Count; i++)
            {
                int randomIndex = Random.Range(i, _cardDeck.Count);
                (_cardDeck[i], _cardDeck[randomIndex]) = (_cardDeck[randomIndex], _cardDeck[i]);
            }

            Random.state = originalState;
        }

        private static int CalculateBalancePerPlayer(int playerNumber)
        {
            return (playerNumber % 4) switch
            {
                1 => 0,
                2 or 3 => 6000,
                0 => 14000,
                _ => 0
            };
        }

        private static int CalculateCardsPerPlayer(int numberOfPlayers)
        {
            return numberOfPlayers switch
            {
                <= 3 => 12,
                4 => 10,
                5 => 9,
                6 => 8,
                7 => 7,
                _ => 6  //TODO check dit
            };
        }

        public List<CardData> TakeCards(int amount)
        {
            List<CardData> takenCards = new List<CardData>();
            for (var i = 0; i < amount; i++)
            {
                if (_cardDeck.Count == 0)
                {
                    Debug.Log("Timo: Geen kaarten meer in de deck!");
                    return null;
                }

                CardData takenCard = _cardDeck[0];
                _cardDeck.RemoveAt(0);
                takenCards.Add(takenCard);
            }

            return takenCards;
        }
    }
}*/