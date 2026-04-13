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

        public void StartGame(Dictionary<int, PlayerListPrefab> players)
        {
            _cardDeck = new List<CardData>();

            FillDeck(players.Count);
            ShuffleDeck();

            var cardsPerPlayer = CalculateCardsPerPlayer(players.Count);

            // Pick bank player randomly
            PlayerListPrefab bankPlayer = null;
            if (players.Count > 0 && SessionData.Instance.CurrentMoneySystem == MoneySystems.DebtBased)
            {
                bankPlayer = players.ElementAt(Random.Range(0, players.Count)).Value;
            }

            int numPlayers = players.Count;
            int[] playerDebts;

            if (SessionData.Instance.CurrentMoneySystem == MoneySystems.DebtBased)
            {
                // Use debt sequence
                playerDebts = DistributeDebt(numPlayers);
            }
            else
            {
                // Everyone gets 6000, bank maybe 0
                playerDebts = new int[numPlayers];
                for (int i = 0; i < numPlayers; i++)
                {
                    playerDebts[i] = (i == 0) ? 0 : 6000; // bank at index 0
                }
            }

            int playerIndex = 0;
            foreach (var player in players)
            {
                List<CardData> playerCards = TakeCards(cardsPerPlayer);

                int[] handCards = new int[playerCards.Count];
                for (int j = 0; j < playerCards.Count; j++)
                {
                    handCards[j] = playerCards[j].ID;
                }

                int debt = playerDebts[playerIndex];

                if (bankPlayer != null && player.Value == bankPlayer)
                {
                    debt = 0;
                }

                StartGameMessage msg = new StartGameMessage(
                    DateTime.Now.ToString("o"),
                    SessionData.Instance.LobbyCode,
                    -1,
                    player.Key,
                    debt, // If player is bank player always give 0 balance / debt
                    handCards,
                    (int)SessionData.Instance.CurrentMoneySystem,
                    bankPlayer?.Name ?? ""
                );

                Debug.Log($"sent cards, msg: {msg}");

                Nats.NatsHost.C.Publish($"{SessionData.Instance.LobbyCode}", msg);

                playerIndex++;
            }
        }


        private int[] DistributeDebt(int numPlayers)
        {
            var debtArray = new int[numPlayers];

            // Bank player (index 0) has no debt
            debtArray[0] = 0;

            // Create debt bag
            int bagSize = (numPlayers - 1) * 3;
            var debtBag = new List<int>(bagSize);

            int[] debtSequence = new int[]
            {
                6000, 6000, 2000, 4000,
                2000, 0, 4000, 2000,
                0, 0, 0, 0
            };

            int sequenceLength = debtSequence.Length;

            // Fill debt bag by repeating sequence
            for (int i = 0; i < bagSize; i++)
            {
                int index = i % sequenceLength;
                debtBag.Add(debtSequence[index]);
            }

            // Distribute debt to players (excluding bank at index 0)
            for (int player = 1; player < numPlayers; player++)
            {
                int debt = 0;

                for (int draw = 0; draw < 3; draw++)
                {
                    int tokenIndex = Random.Range(0, debtBag.Count);
                    debt += debtBag[tokenIndex];
                    debtBag.RemoveAt(tokenIndex);
                }

                debtArray[player] = debt;
            }

            return debtArray;
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

        private static int CalculateCardsPerPlayer(int numberOfPlayers)
        {
            return numberOfPlayers switch
            {
                <= 3 => 12,
                4 => 10,
                5 => 9,
                6 => 8,
                7 => 7,
                _ => 6
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

        public void ReturnCards(int[] cardIds)
        {
            foreach (int id in cardIds)
            {
                CardData card = _cardLibrary.cards.FirstOrDefault(c => c.ID == id);
                if (card != null)
                {
                    int insertAt = Random.Range(0, _cardDeck.Count + 1);
                    _cardDeck.Insert(insertAt, card);
                }
                else
                {
                    Debug.LogWarning($"ReturnCards: unknown card ID {id}, skipping.");
                }
            }
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

            // Filter eerst op de juiste rarity
            var rarityCards = cardLibrary.cards
                .Where(c => c.Rarity == rarity)
                .ToArray();

            if (rarityCards.Length == 0) return 0;

            int curIndex = curIndices[rarity];

            for (int i = 0; i < numSets; i++)
            {
                if (curIndex >= rarityCards.Length) curIndex = 0;
                deck[rarityCards[curIndex]] += 4;
                points += CardPoints[rarity];
                curIndex++;
            }

            curIndices[rarity] = curIndex;
            return points;
        }
    }
}
