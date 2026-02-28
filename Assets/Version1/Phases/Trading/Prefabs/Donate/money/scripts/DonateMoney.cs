using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.UIComponents.Scripts;
using Version1.Utilities;

namespace Version1.Donate.money.scripts
{
    public class DonateMoney : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Controls")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text donationAmountDisplay;

        [Header("Settings")]
        [SerializeField] private int priceStep = 1000;

        private int currentDonation;
        private int minDonation;
        private bool busy;

        private static readonly System.Globalization.CultureInfo de = new System.Globalization.CultureInfo("de-DE");

        private int MaxDonation => PlayerData.PlayerData.Instance.Balance;
        private bool CanDonate => MaxDonation >= minDonation;

        public void OnEnable()
        {
            minDonation = priceStep;

            if (CanDonate)
            {
                currentDonation = (int)Mathf.Floor(MaxDonation / 2000f) * 1000;
                currentDonation = Mathf.Clamp(currentDonation, minDonation, MaxDonation);
            }
            else
            {
                currentDonation = 0;
            }

            RefreshDisplay(animated: false);
            UpdateConfirmButton();

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);

            StartCoroutine(OverlayAnimator.Open(panelRect, canvasGroup));
        }

        public void IncreaseDonation()
        {
            if (busy) return;

            // Guard: nothing to donate at all
            if (!CanDonate) return;

            int prev = currentDonation;
            currentDonation = Mathf.Min(currentDonation + priceStep, MaxDonation);

            if (currentDonation != prev) RefreshDisplay(animated: true, from: prev);
            UpdateConfirmButton();
            StartCoroutine(OverlayAnimator.Punch(confirmButton.transform));
        }

        public void DecreaseDonation()
        {
            if (busy) return;

            // Guard: nothing to donate at all
            if (!CanDonate) return;

            int prev = currentDonation;
            currentDonation = Mathf.Max(currentDonation - priceStep, minDonation);

            if (currentDonation != prev) RefreshDisplay(animated: true, from: prev);
            UpdateConfirmButton();
            StartCoroutine(OverlayAnimator.Punch(confirmButton.transform));
        }

        private void UpdateConfirmButton()
        {
            confirmButton.interactable = !busy && currentDonation >= minDonation && currentDonation <= MaxDonation;
        }

        private void OnConfirmClicked()
        {
            if (busy || currentDonation < minDonation || currentDonation > MaxDonation) return;
            StartCoroutine(ConfirmSequence());
        }

        private void OnCancelClicked()
        {
            if (busy) return;
            StartCoroutine(OverlayAnimator.Close(panelRect, canvasGroup, () => gameObject.SetActive(false)));
        }

        private IEnumerator ConfirmSequence()
        {
            busy = true;
            confirmButton.interactable = false;
            cancelButton.interactable = false;

            yield return StartCoroutine(OverlayAnimator.Punch(confirmButton.transform, 1.3f, 0.15f));
            yield return StartCoroutine(OverlayAnimator.Shake(panelRect, 0.2f, 4f));

            PlayerData.PlayerData.Instance.Balance -= currentDonation;
            SendDonateMessage();

            yield return StartCoroutine(OverlayAnimator.Close(panelRect, canvasGroup, () => gameObject.SetActive(false)));

            confirmButton.interactable = true;
            cancelButton.interactable = true;
            busy = false;
        }

        private Coroutine countCoroutine;

        private void RefreshDisplay(bool animated, int from = 0)
        {
            if (countCoroutine != null) StopCoroutine(countCoroutine);

            if (animated)
            {
                countCoroutine = StartCoroutine(
                    OverlayAnimator.CountTo(donationAmountDisplay, from, currentDonation,
                        duration: 0.18f, format: "N0", culture: de));
            }
            else
            {
                donationAmountDisplay.text = currentDonation.ToString("N0", de);
            }
        }

        private void SendDonateMessage()
        {
            var sessionId = PlayerData.PlayerData.Instance.LobbyID;
            var msg = new DonateMoneyMessage(
                DateTime.Now.ToString("o"),
                sessionId,
                PlayerData.PlayerData.Instance.PlayerId,
                currentDonation);

            NetworkManager.Instance.Publish(sessionId.ToString(), msg);
        }
    }
}
