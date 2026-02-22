using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Host;
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

        public CardHandInOverlay(bool busy)
        {
            this.busy = busy;
        }

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

            onConfirm       = msg;

            cardNameText.text = cardName;
            pointsText.text   = $"Worth <b>{pointValue}</b> {(pointValue == 1 ? "point" : "points")}";

            handInPanel.gameObject.SetActive(true);
            rewardPanel.gameObject.SetActive(false);
            gameObject.SetActive(true);

            StartCoroutine(AnimateOpen());
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
            StartCoroutine(AnimateClose());
        }

        private void OnDismissClicked()
        {
            if (busy) return;
            StartCoroutine(AnimateClose());
        }

        private IEnumerator AnimateOpen()
        {
            busy = true;

            handInPanel.localScale = Vector3.one * 0.6f;
            canvasGroup.alpha      = 0f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / openDuration;
                float e = EaseOutBack(Mathf.Clamp01(t));
                handInPanel.localScale = Vector3.LerpUnclamped(Vector3.one * 0.6f, Vector3.one, e);
                canvasGroup.alpha      = Mathf.Clamp01(t / 0.5f);
                yield return null;
            }

            handInPanel.localScale = Vector3.one;
            canvasGroup.alpha      = 1f;
            busy = false;
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
                float e = EaseInCubic(Mathf.Clamp01(t));

                cardIconRect.anchoredPosition =
                    iconStart + new Vector2(0f, Mathf.Lerp(0f, flyOutHeight, e));
                cardIconRect.localRotation =
                    Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, flyOutSpin, e));

                if (iconImage != null)
                    iconImage.color = new Color(1f, 1f, 1f,
                        Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.5f, 1f, t)));

                yield return null;
            }

            yield return StartCoroutine(ShakePanel(handInPanel, 0.18f, 8f));

            onConfirm?.Invoke();

            handInPanel.gameObject.SetActive(false);
            busy             = false;

            rewardTitleText.text = "Waiting for new cards…";
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

            yield return StartCoroutine(PopIn(rewardPanel, openDuration));

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
                float e = EaseOutBack(Mathf.Clamp01(t));

                rect.localScale = Vector3.one * e;
                group.alpha = Mathf.Clamp01(t / 0.3f);

                yield return null;
            }

            rect.localScale = Vector3.one;
            group.alpha     = 1f;
        }

        private IEnumerator PopIn(RectTransform target, float duration)
        {
            target.localScale = Vector3.one * 0.6f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                target.localScale = Vector3.LerpUnclamped(
                    Vector3.one * 0.6f, Vector3.one, EaseOutBack(Mathf.Clamp01(t)));
                yield return null;
            }
            target.localScale = Vector3.one;
        }

        private IEnumerator AnimateClose()
        {
            busy = true;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / closeDuration;
                float e = EaseInCubic(Mathf.Clamp01(t));
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, e);
                yield return null;
            }

            gameObject.SetActive(false);

            cardIconRect.anchoredPosition = Vector2.zero;
            cardIconRect.localRotation    = Quaternion.identity;
            var img = cardIconRect.GetComponent<Image>();
            if (img != null) img.color = Color.white;

            confirmButton.interactable = true;
            cancelButton .interactable = true;
            handInPanel.gameObject.SetActive(true);
            rewardPanel.gameObject.SetActive(false);

            busy = false;
        }

        private IEnumerator ShakePanel(RectTransform target, float duration, float magnitude)
        {
            Vector3 origin = target.localPosition;
            float   t      = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float decay = 1f - (t / duration);
                target.localPosition = origin + (Vector3)(UnityEngine.Random.insideUnitCircle * (magnitude * decay));
                yield return null;
            }
            target.localPosition = origin;
        }


        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }

        private static float EaseInCubic(float x) => x * x * x;
    }
}

