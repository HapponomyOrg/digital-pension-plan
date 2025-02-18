using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Obsolete]
public class PopupTransition : MonoBehaviour
{
    [SerializeField]
    private RectTransform panelPosition;

    private float timeRunning = 0;

    private void OnEnable()
    {
        timeRunning = 0;
        panelPosition.position = new Vector3(Screen.width / 2, Screen.height * 2);
    }

    private void Update()
    {
        timeRunning += Time.deltaTime * .5f;
        if (timeRunning >= 1) return;
        float height = EaseInOutSine(Screen.height * 2, Screen.height / 2, timeRunning);
        panelPosition.position = new Vector3(Screen.width / 2, height);

    }

    public static float EaseInOutSine(float start, float end, float value)
    {

        end -= start;
        return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
    }

}
