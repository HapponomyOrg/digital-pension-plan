using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Host;
using Version1.UIComponents.Scripts;
using Version1.Utilities;

namespace Version1.Phases.Trading.CardHandIn.Scripts
{
    public class CardHandInOverlay : MonoBehaviour
    {
        [Header("Hand-in panel")]
        [SerializeField] private RectTransform handInPanel;
        [SerializeField] private CanvasGroup   canvasGroup;
        [SerializeField] private TMP_Text      cardNameText;
        [SerializeField] private TMP_Text      pointsText;
        [SerializeField] private Button        confirmButton;
        [SerializeField] private Button        cancelButton;
        [SerializeField] private RectTransform cardIconRect;

        [Header("Reward panel")]
        [SerializeField] private RectTransform rewardPanel;
        [SerializeField] private TMP_Text      rewardTitleText;
        [SerializeField] private RectTransform cardSpawnRoot;
        [SerializeField] private Button        dismissButton;

        [Header("Card display prefab")]
        [SerializeField] private CardScript cardDisplayPrefab;

        [Header("Timings")]
        [SerializeField] private float openDuration      = 0.25f;
        [SerializeField] private float closeDuration     = 0.18f;
        [SerializeField] private float flyOutHeight      = 400f;
        [SerializeField] private float flyOutDuration    = 0.55f;
        [SerializeField] private float flyOutSpin        = 720f;
        [SerializeField] private float cardFlyInDuration = 0.35f;
        [SerializeField] private float cardStagger       = 0.12f;

        private Action onConfirm;
        private bool   busy;

        private void Awake()
        {
            gameObject.SetActive(false);
            rewardPanel.gameObject.SetActive(false);

            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton .onClick.AddListener(OnCancelClicked);
            dismissButton.onClick.AddListener(OnDismissClicked);
        }

        private void OnEnable()
        {
            if (NetworkManager.Instance != null)
                NetworkManager.Instance.OnConfirmHandIn += OnConfirmHandInReceived;
        }

        private void OnDisable()
        {
            if (NetworkManager.Instance != null)
                NetworkManager.Instance.OnConfirmHandIn -= OnConfirmHandInReceived;
        }

        public void Show(string cardName, int pointValue, Action msg)
        {
            if (busy) return;

            onConfirm        = msg;

            cardNameText.text = cardName;
            pointsText.text   = $"Worth <b>{pointValue}</b> {(pointValue == 1 ? "point" : "points")}";

            handInPanel.gameObject.SetActive(true);
            rewardPanel.gameObject.SetActive(false);
            gameObject.SetActive(true);

            StartCoroutine(OverlayAnimator.Open(handInPanel, canvasGroup));
        }

        private void OnConfirmHandInReceived(object sender, ConfirmHandInMessage msg)
        {
            if (msg.Receiver != PlayerData.PlayerData.Instance.PlayerId) return;
            StartCoroutine(ShowRewardSequence(msg.Cards));
        }

        private void OnConfirmClicked()
        {
            if (busy) return;
            StartCoroutine(HandInSequence());
        }

        private void OnCancelClicked()
        {
            if (busy) return;
            StartCoroutine(OverlayAnimator.Close(handInPanel, canvasGroup, ResetAndHide));
        }

        private void OnDismissClicked()
        {
            if (busy) return;
            StartCoroutine(OverlayAnimator.Close(rewardPanel, canvasGroup, ResetAndHide));
        }

        private IEnumerator HandInSequence()
        {
            busy = true;
            confirmButton.interactable = false;
            cancelButton .interactable = false;

            Vector2 iconStart = cardIconRect.anchoredPosition;
            var     iconImage = cardIconRect.GetComponent<Image>();

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / flyOutDuration;
                float e = OverlayAnimator.EaseInCubic(Mathf.Clamp01(t));

                cardIconRect.anchoredPosition =
                    iconStart + new Vector2(0f, Mathf.Lerp(0f, flyOutHeight, e));
                cardIconRect.localRotation =
                    Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, flyOutSpin, e));

                if (iconImage != null)
                    iconImage.color = new Color(1f, 1f, 1f,
                        Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.5f, 1f, t)));

                yield return null;
            }

            yield return StartCoroutine(OverlayAnimator.Shake(handInPanel));

            onConfirm?.Invoke();

            handInPanel.gameObject.SetActive(false);
            busy             = false;

            rewardTitleText.text   = "Waiting for new cards…";
            rewardPanel.gameObject.SetActive(true);
            rewardPanel.localScale = Vector3.one * 0.8f;
        }

        private IEnumerator ShowRewardSequence(int[] cardIds)
        {
            while (busy) yield return null;

            busy = true;

            handInPanel.gameObject.SetActive(false);
            rewardPanel.gameObject.SetActive(true);
            rewardTitleText.text = "You received:";

            foreach (Transform child in cardSpawnRoot)
                Destroy(child.gameObject);

            yield return StartCoroutine(OverlayAnimator.Open(rewardPanel, canvasGroup, openDuration));

            int index = 0;
            foreach (var id in cardIds)
            {
                var obj = Instantiate(cardDisplayPrefab, cardSpawnRoot);
                obj.SetDisplay(id, 1);

                var rect = obj.GetComponent<RectTransform>();
                rect.localScale = Vector3.zero;

                var cardGroup = obj.gameObject.AddComponent<CanvasGroup>();
                cardGroup.alpha = 0f;

                StartCoroutine(FlyCardIn(rect, cardGroup, index * cardStagger));
                index++;
            }

            float totalWait = cardIds.Length * cardStagger + cardFlyInDuration + 0.1f;
            yield return new WaitForSeconds(totalWait);

            busy             = false;
        }

        private IEnumerator FlyCardIn(RectTransform rect, CanvasGroup group, float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / cardFlyInDuration;
                float e = OverlayAnimator.EaseOutBack(Mathf.Clamp01(t));
                rect.localScale = Vector3.one * e;
                group.alpha     = Mathf.Clamp01(t / 0.3f);
                yield return null;
            }

            rect.localScale = Vector3.one;
            group.alpha     = 1f;
        }

        private void ResetAndHide()
        {
            cardIconRect.anchoredPosition = Vector2.zero;
            cardIconRect.localRotation    = Quaternion.identity;
            var img = cardIconRect.GetComponent<Image>();
            if (img != null) img.color = Color.white;

            confirmButton.interactable = true;
            cancelButton .interactable = true;
            handInPanel.gameObject.SetActive(true);
            rewardPanel.gameObject.SetActive(false);

            gameObject.SetActive(false);
            busy = false;
        }
    }
}
