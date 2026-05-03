using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Phases.Trading.CardHandIn.Scripts;
using Version1.Phases.Trading.Prefabs.CardHandIn.Scripts;
using Version1.Phases.Trading.Scripts.UI;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading
{
    public class CardBar : MonoBehaviour
    {
        [SerializeField] private RectTransform    cardList;
        [SerializeField] private CardAmountDisplay cardDisplay;

        [Header("Hand-in")]
        [Tooltip("Drag your CardHandInOverlay prefab instance here")]
        [SerializeField] private CardHandInOverlay handInOverlay;

        private bool generateDisplays;


        public void Init()
        {
            GenerateCardDisplays();

            PlayerData.Instance.OnCardsChange += (sender, _) =>
            {
                generateDisplays = true;
            };
        }

        private void Update()
        {
            if (!generateDisplays) return;
            generateDisplays = false;
            GenerateCardDisplays();
        }


        private void GenerateCardDisplays()
        {
            if (cardList == null)
                throw new NullReferenceException("cardlist is not here");

            foreach (Transform child in cardList)
                Destroy(child.gameObject);

            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in PlayerData.Instance.Cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardDisplay, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);

                if (cardAmount.Value >= 4)
                {
                    var dance = obj.gameObject.AddComponent<CardJumpAnimation>();
                    dance.StartDance();

                    var button  = obj.gameObject.AddComponent<Button>();
                    var capturedId = cardAmount.Key;

                    button.onClick.AddListener(() => OpenHandInOverlay(capturedId));
                }
            }
        }


        private void OpenHandInOverlay(int cardId)
        {
            var cardData  = Utilities.GameManager.Instance.CardLibrary.CardData(cardId);
            var cardName  = cardData.name;
            var cardValue = cardData.Value;

            handInOverlay.gameObject.SetActive(true);
            handInOverlay.Show(cardId, cardName, cardValue, () => HandInCards(cardId));
        }

        private void HandInCards(int cardId)
        {
            for (var i = 0; i < 4; i++)
                PlayerData.Instance.RemoveCard(cardId);

            var cardData = Utilities.GameManager.Instance.CardLibrary.CardData(cardId);
            PlayerData.Instance.AddPoints(cardData.Value);

            var msg = new CardHandInMessage(
                DateTime.UtcNow.ToString("o"),
                PlayerData.Instance.LobbyID,
                PlayerData.Instance.PlayerId,
                cardData.ID,
                cardData.Value);

            NetworkManager.Instance.Publish(PlayerData.Instance.LobbyID.ToString(), msg);
        }
    }
}
