using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        private int Lobby;
        
        [SerializeField] private GameObject CreateScene;
        [SerializeField] private GameObject HostScene;
        
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
            regenerateButton.onClick.AddListener(RegenerateButtonOnClick);
            createSession.onClick.AddListener(CreateSessionOnClick);
        }

        private void CreateSessionOnClick()
        {
            // TODO NATS CODE
            HostScene.SetActive(true);
            CreateScene.SetActive(false);
        }


        private void RegenerateButtonOnClick()
        {
            Lobby = Random.Range(100000000, 999999999);
            
            gameCodeInputField.text =
                $"{Lobby.ToString().Substring(0, 3)} {Lobby.ToString().Substring(3, 3)} {Lobby.ToString().Substring(6, 3)}";

        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;
            
            seedInputField.interactable = !seedInputField.interactable;
        }
    }
}