using System;
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

        public void StartGame(int numOfPlayers)
        {
            _cardDeck = new List<CardData>();

            FillDeck();
            ShuffleDeck();

            var cardsPerPlayer = CalculateCardsPerPlayer(numOfPlayers);

            for (int i = 0; i < numOfPlayers + 1; i++)
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
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, i,
                        CalculateBalancePerPlayer(i), handCards, (int)SessionData.Instance.CurrentMoneySystem);
                }
                else
                {
                    Debug.Log($"6000");
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, i, 6000,
                        handCards,
                        (int)SessionData.Instance.CurrentMoneySystem);
                }

                Debug.Log($"sent cards, msg: {msg}");
                
                Nats.NatsHost.C.Publish($"{SessionData.Instance.LobbyCode}.{i}", msg);
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
                4 => 10,
                5 => 9,
                6 => 8,
                7 => 7,
                _ => 12
            };
        }

        private List<CardData> TakeCards(int amount)
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
}