using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalListBox : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup LayoutGroup;

    private void Update()
    {
        print(LayoutGroup.transform.childCount);
    }
}
