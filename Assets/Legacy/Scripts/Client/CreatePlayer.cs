using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using NATS;
using TMPro;

namespace Client
{
    public class CreatePlayer : MonoBehaviour
    {
        public string playerName = "", ageString = "", codeString = "";
        private int code = 0;
        private int age = 0;
        private string gender = "MAN";

        [SerializeField] public Button button;

        [SerializeField] private GameObject errorObject;
        [SerializeField] private TMP_Text errorText;

        [SerializeField] private PlayerInfo playerInfo;

        private void Start()
        {
            NatsClient.Instance.OnRejected += (sender,msg) =>
            {
                if (PlayerManager.Instance.PlayerName == msg.TargetPlayer && PlayerManager.Instance.PlayerId == 456)
                {
                    errorObject.SetActive(true);
                    errorText.text = msg.Message;   
                }
            };
        }

        public void SetName(string _name)
        {
            if (_name == "")
                return;
            
            if (_name.Length > 20)
            {
                errorObject.SetActive(true);
                errorText.text =
                    $"{_name} : holds to many characters. \n Please enter a name under 20 characters.";
                return;
            }

            playerName = _name;
            button.interactable = playerName.Length > 0 && ageString.Length > 0 && codeString.Length > 0;
        }

        public void SetAge(string _age)
        {
            if (_age == "")
                return;
            
            ageString = _age;

            bool isValidAge = int.TryParse(_age, out var ageValidation);

            if (isValidAge)
            {
                age = ageValidation;
            }
            else
            {
                errorObject.SetActive(true);
                errorText.text = $"{_age} : doesn't seem like a possible age. \n Please enter a valid age like: 47.";
            }
            button.interactable = playerName.Length > 0 && ageString.Length > 0 && codeString.Length > 0;
        }

        public void SetGender(int _gender)
        {
            gender = _gender switch
            {
                0 => "MALE",
                1 => "FEMALE",
                2 => "NON-BINARY",
                3 => "OTHER",
                _ => "UNKNOWN"
            };
        }

        public void SetCode(string _code)
        {
            if (_code == "")
                return;
            codeString = Regex.Replace(_code, @"\s+", "");

            int codeValidation;

            try
            {
                codeValidation = int.Parse(codeString);
            }
            catch
            {
                errorObject.SetActive(true);
                errorText.text =
                    $"{_code} : doesn't seem like a valid code. \n Please enter a valid 9 figure code like : 123 456 789";
                return;
            }

            code = codeValidation;
            button.interactable = playerName.Length > 0 && ageString.Length > 0 && codeString.Length > 0;
        }

        public void OnJoin()
        {
            PlayerManager.Instance.PlayerName = playerName;
            PlayerManager.Instance.Age = age;
            PlayerManager.Instance.Gender = gender;
            PlayerManager.Instance.LobbyID = code;
            NatsClient.Instance.SubscribeToSubject(codeString);
            JoinRequestMessage msg =
                new JoinRequestMessage(DateTime.Now.ToString("o"), code, byte.MaxValue, playerName, age, gender);
            NatsClient.Instance.Publish(codeString, msg);
        }
    }


    [Serializable]
    public struct PlayerInfo
    {
        public string playerName, age, gender;

        public PlayerInfo(string _name, string _age, string _gender)
        {
            playerName = _name;
            age = _age;
            gender = _gender;
        }
    }
}