using System;
using System.Collections.Generic;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardHandInOverlay : MonoBehaviour
    {
        [SerializeField] private Image cardDisplayPrefab;
        [SerializeField] private Transform cardDisplay;

        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public void Open(Card card)
        {
            gameObject.SetActive(true);

            // Generate the display
            GenerateDisplay(card.ID, card.Value);

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
                {
                    PlayerManager.Instance.RemoveCards(new int[] { card.ID, card.ID, card.ID, card.ID });
                    confirmButton.interactable = false;
                    cancelButton.interactable = false;


                    var sessionId = PlayerManager.Instance.LobbyID;
                    CardHandInMessage cardHandInMessage = new CardHandInMessage(DateTime.Now.ToString("o"), sessionId,
                        PlayerManager.Instance.PlayerId, card.ID, card.Value);
                    NatsClient.C.Publish(sessionId.ToString(), cardHandInMessage);

                    var uicards = new List<UICard>();
                    foreach (var uiCard in PlayerManager.Instance.UiCards)
                    {
                        if (uiCard.Card.ID == card.ID)
                        {
                            uicards.Add(uiCard);
                        }

                        if (uicards.Count >= 4) break;
                    }

                    foreach (var obj in uicards)
                    {
                        PlayerManager.Instance.UiCards.Remove(obj);
                        Destroy(obj.gameObject);
                    }

                    PlayerManager.Instance.AddPoints(card.Value);

                    confirmButton.interactable = true;
                    cancelButton.interactable = true;

                    gameObject.SetActive(false);
                }
            );
        }

        private void GenerateDisplay(int card, int point)
        {
            // Remove all the old cards
            foreach (Transform child in cardDisplay)
                Destroy(child.gameObject);

            // Generate the new cards
            for (int i = 0; i < 4; i++)
            {
                var d = Instantiate(cardDisplayPrefab, cardDisplay);
                d.sprite = PlayerManager.Instance.CardLibrary.cards[card].Art;
            }

            // Set the price display
            priceDisplay.text = $"Pension Points: {point}";
        }
    }
}