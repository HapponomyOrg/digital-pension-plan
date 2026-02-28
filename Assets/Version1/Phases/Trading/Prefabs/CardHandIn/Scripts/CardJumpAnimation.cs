using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Phases.Trading.CardHandIn.Scripts
{
    public class CardJumpAnimation : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 14f;
        [SerializeField] private float jumpSpeed  = 2.2f;

        [Header("Flash Settings")]
        [SerializeField] private float flashSpeed  = 0.6f;
        [SerializeField] private Color flashColorA = Color.white;
        [SerializeField] private Color flashColorB = new Color(1f, 0.85f, 0f);

        private RectTransform rect;
        private Image         image;
        private Vector2       basePos;
        private float         phaseOffset;
        private bool          running;

        private void Awake()
        {
            rect        = GetComponent<RectTransform>();
            image       = GetComponent<Image>();
            phaseOffset = UnityEngine.Random.Range(0f, 1f);
        }

        private void OnEnable()  => StartDance();
        private void OnDisable() => StopDance();

        public void StartDance()
        {
            if (running) return;
            running = true;
            StartCoroutine(DanceRoutine());
        }

        public void StopDance()
        {
            running = false;
            StopAllCoroutines();
            if (rect  != null) rect.anchoredPosition = basePos;
            if (image != null) image.color = Color.white;
        }


        private IEnumerator DanceRoutine()
        {
            yield return new WaitForEndOfFrame();
            basePos = rect.anchoredPosition;

            while (running)
            {
                yield return new WaitForEndOfFrame();

                float t = Time.time;

                float jump = Mathf.Abs(Mathf.Sin((t * jumpSpeed + phaseOffset) * Mathf.PI)) * jumpHeight;
                rect.anchoredPosition = basePos + new Vector2(0f, jump);

                float ping = (Mathf.Sin((t * flashSpeed + phaseOffset) * Mathf.PI * 2f) + 1f) * 0.5f;
                if (image != null)
                    image.color = Color.Lerp(flashColorA, flashColorB, ping);
            }
        }
    }
}
