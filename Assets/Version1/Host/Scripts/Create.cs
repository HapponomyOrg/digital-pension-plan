using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Host;
using Version1.Websocket;
using Random = UnityEngine.Random;

namespace Version1.Host.Scripts
{
    public class Create : MonoBehaviour
    {
        [SerializeField] private GameObject natsError;
        [SerializeField] private TMP_Text natsErrorTMP;

        [SerializeField] private GameObject HostScene;

        [SerializeField] private Sprite checkMarkSprite;
        [SerializeField] private Sprite penSprite;
        [SerializeField] private TMP_InputField hostInputField;
        [SerializeField] private Button editButton;
        [SerializeField] private Button editGameCodeButton;
        [SerializeField] private TMP_InputField seedInputField;
        [SerializeField] private TMP_Dropdown gameModeDropDown;
        [SerializeField] private Button regenerateButton;
        [SerializeField] private TMP_InputField gameCodeInputField;
        [SerializeField] private Button createSession;
        [SerializeField] private GameObject seedInputError;
        [SerializeField] private GameObject gameCodeError;

        private int oldCode;

        private async void Start()
        {
            await Nats.NatsHost.C.WebSocketClient.Connect();
            SetupGameModeDropdown();
        }

        private void OnEnable()
        {
            ResetUI();
            RemoveAllListeners();
            AddAllListeners();
        }

        private void OnDisable()
        {
            RemoveAllListeners();
        }

        private void ResetUI()
        {
            // Handle game code editing state
            if (oldCode != SessionData.Instance.LobbyCode)
            {
                gameCodeInputField.interactable = true;
                regenerateButton.gameObject.SetActive(true);
                editGameCodeButton.gameObject.SetActive(false);
            }

            SessionData.Instance.Reset(false);

            // Set input field values without triggering listeners
            hostInputField.SetTextWithoutNotify(SessionData.Instance.HostName);
            seedInputField.SetTextWithoutNotify(SessionData.Instance.Seed.ToString());
            gameCodeInputField.SetTextWithoutNotify(
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} " +
                $"{SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} " +
                $"{SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}");

            // Handle locked game code state
            if (SessionData.Instance.LobbyCode == oldCode)
            {
                gameCodeInputField.interactable = false;
                regenerateButton.gameObject.SetActive(false);
                editGameCodeButton.gameObject.SetActive(true);
            }
        }

        private void SetupGameModeDropdown()
        {
            gameModeDropDown.ClearOptions();
            var options = (from MoneySystems system in Enum.GetValues(typeof(MoneySystems))
                select FormatEnumForDisplay(system.ToString())
                into displayName
                select new TMP_Dropdown.OptionData(displayName)).ToList();

            gameModeDropDown.options = options;
            gameModeDropDown.RefreshShownValue();
        }

        private void RemoveAllListeners()
        {
            // Remove button listeners
            editButton.onClick.RemoveAllListeners();
            editGameCodeButton.onClick.RemoveAllListeners();
            regenerateButton.onClick.RemoveAllListeners();
            createSession.onClick.RemoveAllListeners();

            // Remove input field listeners
            hostInputField.onValueChanged.RemoveAllListeners();
            seedInputField.onValueChanged.RemoveAllListeners();
            gameCodeInputField.onValueChanged.RemoveAllListeners();

            // Remove dropdown listener
            gameModeDropDown.onValueChanged.RemoveAllListeners();
        }

        private void AddAllListeners()
        {
            // Button listeners
            editButton.onClick.AddListener(EditButtonOnClick);
            editGameCodeButton.onClick.AddListener(EditGameCodeOnClick);
            regenerateButton.onClick.AddListener(RegenerateButtonOnClick);
            createSession.onClick.AddListener(CreateSessionOnClick);

            // Input field listeners
            hostInputField.onValueChanged.AddListener(OnHostNameChanged);
            seedInputField.onValueChanged.AddListener(OnSeedChanged);
            gameCodeInputField.onValueChanged.AddListener(OnGameCodeChanged);

            // Dropdown listener
            gameModeDropDown.onValueChanged.AddListener(OnGameModeChanged);
        }

        private void OnHostNameChanged(string val)
        {
            SessionData.Instance.HostName = val;
        }

        private void OnSeedChanged(string val)
        {
            if (int.TryParse(val, out int result))
            {
                SessionData.Instance.Seed = result;
                seedInputError.SetActive(false);
            }
            else
            {
                seedInputError.SetActive(true);
            }
        }

        private void OnGameCodeChanged(string val)
        {
            var str = val.Replace(" ", "");

            if (int.TryParse(str, out int result))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d*$"))
                {
                    gameCodeError.SetActive(true);
                    return;
                }

                SessionData.Instance.LobbyCode = result;
                gameCodeError.SetActive(false);
            }
            else
            {
                gameCodeError.SetActive(true);
            }
        }

        private void OnGameModeChanged(int val)
        {
            switch (val)
            {
                case 0:
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.Sustainable;
                    break;
                case 1:
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.DebtBased;
                    break;
                case 2:
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.InterestAtIntervals;
                    break;
                default:
                    Debug.LogWarning("This Money system is not implemented");
                    break;
            }
        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;
            seedInputField.interactable = !seedInputField.interactable;
        }

        private void EditGameCodeOnClick()
        {
            editGameCodeButton.image.sprite =
                editGameCodeButton.image.sprite == penSprite ? checkMarkSprite : penSprite;
            gameCodeInputField.interactable = !gameCodeInputField.interactable;
        }

        private void RegenerateButtonOnClick()
        {
            SessionData.Instance.LobbyCode = Random.Range(100000000, 999999999);

            gameCodeInputField.SetTextWithoutNotify(
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} " +
                $"{SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} " +
                $"{SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}");
        }

        private void CreateSessionOnClick()
        {
            Debug.Log("Creating session...");

            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new CreateSessionMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1,
                SessionData.Instance.LobbyCode));

            Nats.NatsHost.C.Subscribe(SessionData.Instance.LobbyCode.ToString());

            oldCode = SessionData.Instance.LobbyCode;

            HostScene.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ToggleMoneyInbalance(bool inbalance)
        {
            Debug.Log($"inbalance mode = {inbalance}");
            SessionData.Instance.InbalanceMode = inbalance;
        }

        private string FormatEnumForDisplay(string enumValue)
        {
            return System.Text.RegularExpressions.Regex.Replace(enumValue, "(?<!^)([A-Z])", " $1");
        }

        private void Update()
        {
            createSession.interactable = !(SessionData.Instance.HostName == "" || 
                                           SessionData.Instance.LobbyCode == 0 ||
                                           gameCodeError.activeSelf || 
                                           seedInputError.activeSelf);

            Nats.NatsHost.C.HandleMessages();
        }
    }
}