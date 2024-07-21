using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NavigationButton : MonoBehaviour
    {
        [field:SerializeField] public Button Button { get; private set; }
        [field:SerializeField] public TMP_Text ButtonText { get; private set; }
        
        [SerializeField] private NavigationButton[] otherButtons;

        [SerializeField] private bool startActive;

        [Header("Active")]
        [SerializeField] private ColorBlock selectedColor;
        
        [Header("Default")]
        [SerializeField] private ColorBlock defaultColor;

        private void Start()
        {
            if (startActive)
                SetActive();
        }

        public void SetActive()
        {
            foreach (var b in otherButtons)
            {
                b.Button.colors = defaultColor;
                b.ButtonText.color = Color.black;
            }

            Button.colors = selectedColor;
            ButtonText.color = Color.white;
        }
    }
}
