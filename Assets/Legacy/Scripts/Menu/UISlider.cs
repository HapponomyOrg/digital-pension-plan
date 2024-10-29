using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text amountDisplay;

    public int SliderStep { get; private set; }
    public int Value { get; private set; }

    public void SetupSlider(int val, int min, int max, int stepSize = 1000)
    {
        SliderStep = stepSize;
        slider.maxValue = max;
        slider.minValue = min;
        slider.value = val;
        Value = val * SliderStep;
        slider.wholeNumbers = true;
    }

    public void SetDisplay()
    {
        var val = (int)slider.value * SliderStep;
        if (amountDisplay != null)
            amountDisplay.text = val.ToString();
        Value = val;
    }
}
