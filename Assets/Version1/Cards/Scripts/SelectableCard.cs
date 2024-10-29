using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    public bool isDraging = false;

    private Vector3 target;
    
    public void Update()
    {
        if (isDraging)
        {
            transform.position = Vector3.Lerp(transform.position, target, 2 * Time.deltaTime);   
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
       // print("OnDrag");
       target = Input.mousePosition;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       // print("BeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       // print("EndDrag");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      //  print("PointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      //  print("PointerExit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("PointerUp");
        isDraging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // print("PointerDown");
        isDraging = true;
    }
}
