using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Version1.Phases.Login.Scripts
{
    public class Login : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TMP_InputField playerNameInput;

        [SerializeField] private TMP_InputField ageInput;
        [SerializeField] private TMP_Dropdown genderDropdown;
        [SerializeField] private TMP_InputField gameCodeInput;
        [SerializeField] private Button joinButton;

        [Header("Error Indicators")] [SerializeField]
        private GameObject natsError;

        [SerializeField] private GameObject nameError;

        private string playerName = string.Empty;
        private int age = -1;
        private int gender = -1;
        private int gameCode = -1;

        private static NetworkManager NetworkManager => NetworkManager.Instance;

        private void OnEnable()
        {
            playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
            ageInput.onValueChanged.AddListener(OnAgeChanged);
            genderDropdown.onValueChanged.AddListener(OnGenderChanged);
            gameCodeInput.onValueChanged.AddListener(OnGameCodeChanged);
            joinButton.onClick.AddListener(OnJoinButtonClicked);

            if (NetworkManager == null) return;
            NetworkManager.OnRejected += HandleRejected;
            NetworkManager.OnError += HandleError;
        }

        private void OnDisable()
        {
            playerNameInput.onValueChanged.RemoveListener(OnPlayerNameChanged);
            ageInput.onValueChanged.RemoveListener(OnAgeChanged);
            genderDropdown.onValueChanged.RemoveListener(OnGenderChanged);
            gameCodeInput.onValueChanged.RemoveListener(OnGameCodeChanged);
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);

            if (NetworkManager == null) return;

            NetworkManager.OnRejected -= HandleRejected;
            NetworkManager.OnError -= HandleError;
        }

        private void OnPlayerNameChanged(string value)
        {
            if (playerName != value)
                nameError.SetActive(false);

            playerName = value.Trim();
            UpdateJoinButtonState();
        }

        private void OnAgeChanged(string value)
        {
            age = int.TryParse(value, out var parsedAge) ? parsedAge : -1;
            UpdateJoinButtonState();
        }

        private void OnGenderChanged(int value)
        {
            gender = value;
            UpdateJoinButtonState();
        }

        private void OnGameCodeChanged(string value)
        {
            var sanitized = value.Replace(" ", string.Empty);
            gameCode = int.TryParse(sanitized, out var parsedCode) ? parsedCode : -1;
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            joinButton.interactable =
                !string.IsNullOrWhiteSpace(playerName) &&
                age > 0 &&
                gameCode > 0;
        }

        private void OnJoinButtonClicked()
        {
            if (NetworkManager == null)
            {
                Debug.LogError("NetworkManager not initialized!");
                return;
            }

            var playerData = PlayerData.PlayerData.Instance;
            playerData.PlayerName = playerName;
            playerData.Age = age;
            playerData.Gender = gender;
            playerData.LobbyID = gameCode;
            playerData.RequestID = Guid.NewGuid().ToString();

            var message = new JoinRequestMessage(
                DateTime.Now.ToString("o"),
                gameCode,
                0,
                playerName,
                age,
                gender,
                playerData.RequestID
            );

            NetworkManager.Subscribe(gameCode.ToString());
            NetworkManager.Publish(gameCode.ToString(), message);
        }

        private void HandleError(object sender, string error)
        {
            Debug.LogWarning($"NATS Error: {error}");
            natsError.SetActive(true);
        }

        private void HandleRejected(object sender, RejectedMessage message)
        {
            var player = PlayerData.PlayerData.Instance;
            if (message.TargetPlayer == player.PlayerName && message.RequestID == player.RequestID)
                nameError.SetActive(true);
        }
    }
}
