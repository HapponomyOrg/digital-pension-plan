using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [Obsolete]
    public class UICard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        private Card card;

        public Card Card
        {
            get => card;
            set
            {
                card = value;
                display.sprite = card.Art;
            }
        }

        [SerializeField] private Image display;

        [field: SerializeField] public RectTransform Rect { get; private set; }
        [SerializeField] private float moveSpeed;

        public Transform DefaultParent { get; set; }

        private bool isDragging;

        private void Update()
        {
            if (isDragging)
            {
                MoveCard();
            }
        }

        private void MoveCard()
        {
            var mousePos = Input.mousePosition;
            var direction = mousePos - transform.position;

            //transform.position += direction.normalized * moveSpeed * Time.fixedDeltaTime;
            Vector2 velocity = direction.normalized *
                               Mathf.Min(moveSpeed, Vector2.Distance(transform.position, mousePos) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // print($"Dragging {gameObject.name}");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!PlayerManager.Instance.RoundIsActive)
                return;
            // print($"Start dragging {gameObject.name}");
            isDragging = true;
            transform.SetParent(DefaultParent);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // print($"Stop dragging {gameObject.name}");
            isDragging = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // print($"Enter bounding box of {gameObject.name}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // print($"Exit bounding box of {gameObject.name}");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // print($"Release {gameObject.name}");
            PlayerManager.Instance.ReleaseCard(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // print($"Click {gameObject.name}");
        }
    }
}
