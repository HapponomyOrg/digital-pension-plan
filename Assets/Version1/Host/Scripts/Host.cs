using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        [SerializeField] private Sprite checkMarkSprite;
        [SerializeField] private Sprite penSprite;

        [SerializeField] private TMP_InputField hostInputField;

        [SerializeField] private Button editButton;
        [SerializeField] private TMP_InputField seedInputField;

        [SerializeField] private TMP_Dropdown gameModeDropDown;

        [SerializeField] private Button regenerateButton;
        [SerializeField] private TMP_InputField gameCodeInputField;

        [SerializeField] private Button createSession;

        private void Start()
        {
            editButton.onClick.AddListener(EditButtonOnClick);
        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;
            
            seedInputField.interactable = !seedInputField.interactable;
        }
    }
}