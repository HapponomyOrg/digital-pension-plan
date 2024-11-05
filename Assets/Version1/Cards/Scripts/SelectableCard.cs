using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Version1.Cards.Scripts
{
    public class SelectableCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Card card;
        
        public bool isDragging = false;

        private Vector3 _target;
    
        [HideInInspector] public RectTransform rectTransform;
    
        private GameObject _cardLayer;
        private List<HorizontalListBox> _possibleBoxes;
    
        [HideInInspector] public HorizontalListBox currentBox;
        [HideInInspector] public HorizontalListBox oldBox;

        private void OnEnable()
        {
            GetComponent<Image>().sprite = card.Art;
            
            rectTransform = GetComponent<RectTransform>();
            _cardLayer = GameObject.FindWithTag("cardLayer");
            _possibleBoxes = GameObject.FindGameObjectsWithTag("listBox")
                .Select(box => box.GetComponent<HorizontalListBox>())
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
            HorizontalListBox nextParent = _possibleBoxes.FirstOrDefault(box => RectOverlaps(rectTransform, box.rectTransform));

            StartCoroutine(MoveToParent(nextParent));
        }

        public void OnPointerEnter(PointerEventData eventData) { }

        public void OnPointerExit(PointerEventData eventData) { }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        private IEnumerator MoveToParent(HorizontalListBox parent)
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
