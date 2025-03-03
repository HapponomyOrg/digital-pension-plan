using System;
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
        [SerializeField] private Button editButton;
        [SerializeField] private TMP_InputField seedInputField;
        [SerializeField] private TMP_Dropdown gameModeDropDown;
        [SerializeField] private Button regenerateButton;
        [SerializeField] private TMP_InputField gameCodeInputField;
        [SerializeField] private Button createSession;
        [SerializeField] private GameObject seedInputError;
        [SerializeField] private GameObject gameCodeError;

        private void Start()
        {
            new Nats.NatsHost();

            Nats.NatsHost.C.onError += (sender, s) =>
            {
                natsError.SetActive(true);
                natsErrorTMP.text = s;
            };

            Nats.NatsHost.C.Connect();


            AddListeners();
        }

        private void OnEnable()
        {
            SessionData.Instance.Reset();

            hostInputField.text = SessionData.Instance.HostName;
            seedInputField.text = SessionData.Instance.Seed.ToString();
            
            gameCodeInputField.text =
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}";
        }

        private void AddListeners()
        {
            gameModeDropDown.onValueChanged.AddListener(OnValueChanged);
            
            editButton.onClick.AddListener(EditButtonOnClick);
            regenerateButton.onClick.AddListener(RegenerateButtonOnClick);
            createSession.onClick.AddListener(CreateSessionOnClick);

            hostInputField.onValueChanged.AddListener((val) => { SessionData.Instance.HostName = val; });

            seedInputField.onValueChanged.AddListener((val) =>
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
            });


            gameCodeInputField.onValueChanged.AddListener((val) =>
            {
                var str = val.Replace(" ", "");

                if (int.TryParse(str, out int result))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d*$"))
                    {
                        gameCodeError.SetActive(true);
                    }

                    SessionData.Instance.LobbyCode = result;
                    gameCodeError.SetActive(false);
                }
                else
                {
                    gameCodeError.SetActive(true);
                }
            });
        }

        private void OnValueChanged(int val)
        {
            switch (val)
            {
                case 0:
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.Sustainable;
                    break;
                case 1 :
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.DebtBased;
                    break;
                case 2:
                    SessionData.Instance.CurrentMoneySystem = MoneySystems.InterestAtIntervals;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CreateSessionOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new CreateSessionMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1,
                SessionData.Instance.LobbyCode));
            Nats.NatsHost.C.SubscribeToSubject(SessionData.Instance.LobbyCode.ToString());


            HostScene.SetActive(true);
            
            gameObject.SetActive(false);
        }


        private void RegenerateButtonOnClick()
        {
            SessionData.Instance.LobbyCode = Random.Range(100000000, 999999999);

            gameCodeInputField.text =
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}";
        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;

            seedInputField.interactable = !seedInputField.interactable;
        }
        
        public void ToggleMoneyInbalance(bool inbalance)
        {
            Debug.Log($"inbalance mode  = {inbalance}");
            SessionData.Instance.InbalanceMode = inbalance;
        }

        private void Update()
        {
            
            createSession.interactable = !(SessionData.Instance.HostName == "" || SessionData.Instance.LobbyCode == 0 ||
                                           gameCodeError.activeSelf || seedInputError.activeSelf);
        }
    }
}