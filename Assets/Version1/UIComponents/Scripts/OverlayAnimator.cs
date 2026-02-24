using System.Collections;
using UnityEngine;

namespace Version1.UIComponents.Scripts
{
    public static class OverlayAnimator
    {
        public const float DefaultOpenDuration = 0.25f;
        public const float DefaultCloseDuration = 0.18f;
        public const float DefaultShakeDuration = 0.20f;
        public const float DefaultShakeMagnitude = 8f;

        public static IEnumerator Open(
            RectTransform panel,
            CanvasGroup group,
            float duration = DefaultOpenDuration)
        {
            panel.localScale = Vector3.one * 0.55f;
            group.alpha = 0f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float e = EaseOutBack(Mathf.Clamp01(t));
                panel.localScale = Vector3.LerpUnclamped(Vector3.one * 0.55f, Vector3.one, e);
                group.alpha = Mathf.Clamp01(t / 0.4f);
                yield return null;
            }

            panel.localScale = Vector3.one;
            group.alpha = 1f;
        }

        public static IEnumerator Close(
            RectTransform panel,
            CanvasGroup group,
            System.Action onDone = null,
            float duration = DefaultCloseDuration)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float e = EaseInCubic(Mathf.Clamp01(t));
                panel.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 0.55f, e);
                group.alpha = Mathf.Lerp(1f, 0f, e);
                yield return null;
            }

            panel.localScale = Vector3.one * 0.55f;
            group.alpha = 0f;
            onDone?.Invoke();
        }

        public static IEnumerator Shake(
            RectTransform target,
            float duration = DefaultShakeDuration,
            float magnitude = DefaultShakeMagnitude)
        {
            Vector3 origin = target.localPosition;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float decay = 1f - (t / duration);
                target.localPosition = origin +
                                       (Vector3)(UnityEngine.Random.insideUnitCircle * magnitude * decay);
                yield return null;
            }

            target.localPosition = origin;
        }

        public static IEnumerator PopIn(RectTransform target, float duration = 0.2f)
        {
            target.localScale = Vector3.zero;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                target.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(t));
                yield return null;
            }

            target.localScale = Vector3.one;
        }

        public static IEnumerator CountTo(
            TMPro.TMP_Text label,
            int from,
            int to,
            float duration = 0.25f,
            string format = "N0",
            System.Globalization.CultureInfo culture = null)
        {
            culture ??= System.Globalization.CultureInfo.CurrentCulture;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                int value = Mathf.RoundToInt(Mathf.Lerp(from, to, EaseOutCubic(Mathf.Clamp01(t))));
                label.text = value.ToString(format, culture);
                yield return null;
            }

            label.text = to.ToString(format, culture);
        }

        public static IEnumerator Punch(Transform target, float scale = 1.2f, float duration = 0.12f)
        {
            Vector3 original = target.localScale;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float e = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
                target.localScale = original * (1f + (scale - 1f) * e);
                yield return null;
            }

            target.localScale = original;
        }

        public static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }

        public static float EaseInCubic(float x) => x * x * x;
        public static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    }
}
