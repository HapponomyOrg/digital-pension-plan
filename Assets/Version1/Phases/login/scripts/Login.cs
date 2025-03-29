using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Version1.Phases.login.scripts
{
    public class Login : MonoBehaviour
    {
        [SerializeField] private TMP_InputField PlayerName;
        [SerializeField] private TMP_InputField Age;
        [SerializeField] private TMP_Dropdown Gender;
        [SerializeField] private TMP_InputField GameCode;
        [SerializeField] private Button CreateButton;
        [SerializeField] private GameObject NatsError;
        [SerializeField] private GameObject NameError;
        [SerializeField] private TMP_Text NameErrorText;
        [SerializeField] private TMP_Text NameErrorRefCode;


        private string playername = "";
        private int age = -1;
        private int gender = -1;
        private int gamecode = -1;

        private void Awake()
        {
            PlayerName.onValueChanged.AddListener(PlayerNameChanged);
            Age.onValueChanged.AddListener(AgeChanged);
            Gender.onValueChanged.AddListener(GenderChanged);
            GameCode.onValueChanged.AddListener(GameCodeChanged);
            CreateButton.onClick.AddListener(JoinSession);
            NetworkManager.Instance.OnRejected += InstanceOnOnRejected;
            NetworkManager.Instance.OnError += InstanceOnError;
        }

        private void InstanceOnError(object sender, string e)
        {
            NatsError.SetActive(true);
            // NatsError.GetComponentAtIndex<TMP_Text>(2).text = e;
        }

        private void InstanceOnOnRejected(object sender, RejectedMessage e)
        {
            // TODO this is wrong select right text.
            if (e.TargetPlayer != PlayerData.PlayerData.Instance.PlayerName &&
                e.RequestID != PlayerData.PlayerData.Instance.RequestID) return;

            // TODO fix this whole component
            NameError.SetActive(true);
            NameErrorText.text = e.Message;
            NameErrorRefCode.text = e.ReferenceID;
        }

        private void JoinSession()
        {
            PlayerData.PlayerData.Instance.PlayerName = playername;
            PlayerData.PlayerData.Instance.Age = age;
            PlayerData.PlayerData.Instance.Gender = gender;
            PlayerData.PlayerData.Instance.LobbyID = gamecode;

            var uid = Guid.NewGuid().ToString();

            PlayerData.PlayerData.Instance.RequestID = uid;

            var msg = new JoinRequestMessage(DateTime.Now.ToString("o"), gamecode, 0, playername, age, gender, uid);
            Nats.NatsClient.C.SubscribeToSubject(gamecode.ToString());
            NetworkManager.Instance.Publish(gamecode.ToString(), msg);
        }

        private void GameCodeChanged(string arg)
        {
            string str = arg.Replace(" ", "");
            bool success = int.TryParse(str, out int number);

            if (success)
            {
                gamecode = number;
                CheckButton();
            }
            else
            {
                Console.WriteLine("Invalid integer.");
            }
        }

        private void GenderChanged(int arg)
        {
            gender = arg;
            CheckButton();
        }

        private void AgeChanged(string arg)
        {
            age = int.Parse(arg);
            CheckButton();
        }

        private void PlayerNameChanged(string arg)
        {
            playername = arg;
            CheckButton();
        }

        private void CheckButton()
        {
            if (playername != "" && age != -1 && gamecode != -1)
            {
                CreateButton.interactable = true;
            }
            else
            {
                CreateButton.interactable = false;
            }
        }
    }
}