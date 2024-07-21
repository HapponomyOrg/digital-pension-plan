using System;
using System.Collections.Generic;
using NATS;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deck
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] private DeckCards[] deckCards;

        // [SerializeField] private GameObject cardUIPrefab;
        // [SerializeField] private GameObject cardInHand;
        private List<Card> cards;
        private const int seed = 2;
        // private const int cardWidth = 32;

        public void StartGame(int numOfPlayers, int interestMode, bool inbalanceMode)
        {
            cards = new List<Card>();
            FillDeck();
            ShuffleDeck(seed);

            var cardsPerPlayer = CalculateCardsPerPlayer(numOfPlayers);


            // TODO this gives an error i think
            for (int i = 1; i < numOfPlayers + 1; i++)
            {
                List<Card> playerCards = TakeCards(cardsPerPlayer);

                int[] handCards = new int[playerCards.Count];

                for (int j = 0; j < playerCards.Count; j++)
                {
                    handCards[j] = playerCards[j].ID;
                }
                
                StartGameMessage msg;
                if (inbalanceMode)
                {
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1, i,
                        CalculateBalancePerPlayer(i), handCards, interestMode);
                }
                else
                {
                    msg = new StartGameMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1, i, 6000, handCards,
                        interestMode);
                }

                NatsHost.C.Publish($"{NatsHost.C.LobbyID}.{i}", msg);
            }
        }

        private int CalculateCardsPerPlayer(int numberOfPlayers)
        {
            switch (numberOfPlayers)
            {
                case 4:
                    return 10;
                case 5:
                    return 9;
                case 6:
                    return 8;
                case 7:
                    return 7;
                // TODO set to 6
                default:
                    return 12;
            }
        }

        private int CalculateBalancePerPlayer(int playerNumber)
        {
            switch (playerNumber % 4)
            {
                case 1:
                    return 0;
                case 2:
                case 3:
                    return 6000;
                case 0:
                    return 14000;
                default:
                    return 0;
            }
        }

        private void FillDeck()
        {
            foreach (var card in deckCards)
            {
                for (var i = 0; i < card.amount; i++)
                {
                    cards.Add(card.card);
                }
            }
        }

        private void ShuffleDeck(int seed)
        {
            Random.InitState(seed);

            for (int i = 0; i < cards.Count; i++)
            {
                int randomIndex = Random.Range(i, cards.Count);
                Card tempCard = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = tempCard;
            }
        }

        /*private void DisplayPlayerCardsOnUI(List<Card> playerCards)
        {
            playerCards.Sort((x, y) => x.ID.CompareTo(y.ID));

            float cardSpacing = 20f;

            float totalWidth = playerCards.Count * (cardWidth + cardSpacing);

            RectTransform cardInHandRectTransform = cardInHand.GetComponent<RectTransform>();
            cardInHandRectTransform.sizeDelta = new Vector2(totalWidth, cardInHandRectTransform.sizeDelta.y);

            for (int i = 0; i < playerCards.Count; i++)
            {
                var card = playerCards[i];
                GameObject cardUIObject = Instantiate(cardUIPrefab, cardInHand.transform);
                RectTransform cardRectTransform = cardUIObject.GetComponent<RectTransform>();

                float cardPositionX = i * (cardWidth + cardSpacing);

                cardRectTransform.anchoredPosition = new Vector2(cardPositionX, 0);

                RawImage cardUIImage = cardUIObject.GetComponent<RawImage>();
                //cardUIImage.texture = card.Art;
            }
        }*/

        public List<Card> TakeCards(int amount)
        {
            List<Card> takenCards = new List<Card>();
            for (var i = 0; i < amount; i++)
            {
                if (cards.Count == 0)
                {
                    Debug.Log("Geen kaarten meer in de deck!");
                    return null;
                }

                Card takenCard = cards[0];
                cards.RemoveAt(0);
                takenCards.Add(takenCard);
            }

            return takenCards;
        }

        public int RemainingCards()
        {
            return cards.Count;
        }
    }
}