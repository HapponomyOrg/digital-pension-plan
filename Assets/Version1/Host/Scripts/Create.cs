using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Host;
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
        [SerializeField] private Button editGameCodeButton;
        [SerializeField] private TMP_Dropdown gameModeDropDown;
        [SerializeField] private Button regenerateButton;
        [SerializeField] private TMP_InputField gameCodeInputField;
        [SerializeField] private Button createSession;
        [SerializeField] private GameObject gameCodeError;

        private int oldCode;

        private void Awake()
        {
            SetupGameModeDropdown();
        }

        private void OnEnable()
        {
            ResetUI();
            AddAllListeners();
        }

        private void OnDisable()
        {
            // Skip RemoveAllListeners for now - causes IL2CPP crash
            // Listeners will be cleaned up on next OnEnable
        }

        private void ResetUI()
        {
            try
            {
                // Handle game code editing state
                if (oldCode != SessionData.Instance.LobbyCode)
                {
                    if (gameCodeInputField != null) gameCodeInputField.interactable = true;
                    if (regenerateButton != null) regenerateButton.gameObject.SetActive(true);
                    if (editGameCodeButton != null) editGameCodeButton.gameObject.SetActive(false);
                }

                SessionData.Instance.Reset(false);

                // Set input field values without triggering listeners
                if (hostInputField != null) hostInputField.SetTextWithoutNotify(SessionData.Instance.HostName);

                var code = SessionData.Instance.LobbyCode.ToString();
                if (code.Length == 9)
                {
                    if (gameCodeInputField != null)
                        gameCodeInputField.SetTextWithoutNotify(
                            $"{code.Substring(0, 3)} {code.Substring(3, 3)} {code.Substring(6, 3)}");
                }

                // Handle locked game code state
                if (SessionData.Instance.LobbyCode == oldCode)
                {
                    if (gameCodeInputField != null) gameCodeInputField.interactable = false;
                    if (regenerateButton != null) regenerateButton.gameObject.SetActive(false);
                    if (editGameCodeButton != null) editGameCodeButton.gameObject.SetActive(true);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error resetting UI: {ex.Message}");
            }
        }

        private void SetupGameModeDropdown()
        {
            if (gameModeDropDown == null)
            {
                Debug.LogError("gameModeDropDown is not assigned in the inspector!");
                return;
            }

            gameModeDropDown.ClearOptions();
            var options = (from MoneySystems system in Enum.GetValues(typeof(MoneySystems))
                select FormatEnumForDisplay(system.ToString())
                into displayName
                select new TMP_Dropdown.OptionData(displayName)).ToList();

            if (options.Count > 0)
            {
                gameModeDropDown.options = options;
                gameModeDropDown.RefreshShownValue();
            }
            else
            {
                Debug.LogError("Failed to populate dropdown options");
            }
        }

        private void AddAllListeners()
        {
            // Button listeners
            if (editGameCodeButton != null)
                editGameCodeButton.onClick.AddListener(EditGameCodeOnClick);
            if (regenerateButton != null)
                regenerateButton.onClick.AddListener(RegenerateButtonOnClick);
            if (createSession != null)
                createSession.onClick.AddListener(CreateSessionOnClick);

            // Input field listeners
            if (hostInputField != null)
                hostInputField.onValueChanged.AddListener(OnHostNameChanged);
            if (gameCodeInputField != null)
                gameCodeInputField.onValueChanged.AddListener(OnGameCodeChanged);

            // Dropdown listener
            if (gameModeDropDown != null)
                gameModeDropDown.onValueChanged.AddListener(OnGameModeChanged);
        }

        private void OnHostNameChanged(string val)
        {
            SessionData.Instance.HostName = val;
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
                default:
                    Debug.LogWarning("This Money system is not implemented");
                    break;
            }
        }

        private void EditGameCodeOnClick()
        {
            try
            {
                if (editGameCodeButton != null && editGameCodeButton.image != null)
                {
                    editGameCodeButton.image.sprite =
                        editGameCodeButton.image.sprite == penSprite ? checkMarkSprite : penSprite;
                }

                if (gameCodeInputField != null)
                {
                    gameCodeInputField.interactable = !gameCodeInputField.interactable;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error in EditGameCodeOnClick: {ex.Message}");
            }
        }

        private void RegenerateButtonOnClick()
        {
            try
            {
                SessionData.Instance.LobbyCode = Random.Range(100000000, 999999999);

                if (gameCodeInputField != null)
                {
                    gameCodeInputField.SetTextWithoutNotify(
                        $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} " +
                        $"{SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} " +
                        $"{SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error in RegenerateButtonOnClick: {ex.Message}");
            }
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
            var codeStr = SessionData.Instance.LobbyCode.ToString();

            createSession.interactable = !(SessionData.Instance.HostName == "" ||
                                           SessionData.Instance.LobbyCode == 0 ||
                                           codeStr.Length < 9 ||
                                           gameCodeError.activeSelf);

            Nats.NatsHost.C.HandleMessages();
        }
    }
}
