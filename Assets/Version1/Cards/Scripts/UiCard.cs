using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Version1.Cards.Scripts
{
    public class UiCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
        IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public CardData cardData;

        public bool isDragging = false;
        public bool isHovering = false;

        private Vector3 _target;

        [HideInInspector] public RectTransform rectTransform;

        private GameObject _cardLayer;
        private List<PlayerHand> _possibleBoxes;

        [HideInInspector] public PlayerHand currentBox;
        [HideInInspector] public PlayerHand oldBox;

        public UiCard Initialize(CardData data)
        {
            cardData = data;

            GetComponent<Image>().sprite = cardData.Art;

            rectTransform = GetComponent<RectTransform>();
            _cardLayer = GameObject.FindWithTag("cardLayer");
            _possibleBoxes = GameObject.FindGameObjectsWithTag("listBox")
                .Select(box => box.GetComponent<PlayerHand>())
                .ToList();

            return this;
        }

        private void OnEnable()
        {
            GetComponent<Image>().sprite = cardData.Art;

            rectTransform = GetComponent<RectTransform>();
            _cardLayer = GameObject.FindWithTag("cardLayer");
            _possibleBoxes = GameObject.FindGameObjectsWithTag("listBox")
                .Select(box => box.GetComponent<PlayerHand>())
                .ToList();
        }

        private void Update()
        {
            if (isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, _target, 4 * Time.deltaTime);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _target = Input.mousePosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            currentBox.RemoveCard(this);
            currentBox = null;
            transform.SetParent(_cardLayer.transform, true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PlayerHand nextParent =
                _possibleBoxes.FirstOrDefault(box => RectOverlaps(rectTransform, box.rectTransform));

            StartCoroutine(MoveToParent(nextParent));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isHovering && !isDragging)
            {
                isHovering = true;
                Vector3 currentPosition = rectTransform.localPosition;
                rectTransform.localPosition = new Vector3(currentPosition.x, currentPosition.y + 10, currentPosition.z);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isHovering)
            {
                isHovering = false;
                Vector3 currentPosition = rectTransform.localPosition;
                rectTransform.localPosition = new Vector3(currentPosition.x, currentPosition.y - 10, currentPosition.z);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // TODO When click it doesnt move
            _target = Input.mousePosition;
            isDragging = true;
        }

        private IEnumerator MoveToParent(PlayerHand parent)
        {
            parent = parent ?? oldBox;

            Vector3 targetPosition = parent.transform.position;
            for (float elapsedTime = 0f; elapsedTime < 0.5f && !isDragging; elapsedTime += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / 0.5f);
                yield return null;
            }

            if (!isDragging)
            {
                transform.position = targetPosition;
                transform.SetParent(parent.transform);
                oldBox = parent;
                parent.AddCard(this);
            }
        }

        private static bool RectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2)
        {
            Rect rect1 = GetWorldRect(rectTrans1);
            Rect rect2 = GetWorldRect(rectTrans2);
            return rect1.Overlaps(rect2);
        }

        private static Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Vector2 size = corners[2] - corners[0];
            return new Rect(corners[0].x, corners[0].y, size.x, size.y);
        }
    }
}
