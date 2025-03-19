using System;
using TMPro;
using Unity.VisualScripting;
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
      [SerializeField] private GameObject NatsError; // TODO add this error thing if it fails to connect
      [SerializeField] private GameObject NameError;


      private string playername;
      private int age;
      private int gender;
      private int gamecode;
   
      private void Start()
      {
         PlayerName.onValueChanged.AddListener(PlayerNameChanged);
         Age.onValueChanged.AddListener(AgeChanged);
         Gender.onValueChanged.AddListener(GenderChanged);
         GameCode.onValueChanged.AddListener(GameCodeChanged);
         CreateButton.onClick.AddListener(JoinSession);
         NetworkManager.Instance.OnRejected += InstanceOnOnRejected;
      }

      private void InstanceOnOnRejected(object sender, RejectedMessage e)
      {
         // TODO this is wrong select right text.
         NameError.SetActive(true);
         NameError.GetComponentAtIndex<TMP_Text>(2).text = e.Message;
         NameError.GetComponentAtIndex<TMP_Text>(3).text = e.ReferenceID;
      }

      private void JoinSession()
      {
         PlayerData.PlayerData.Instance.PlayerName = playername;
         PlayerData.PlayerData.Instance.Age = age;
         PlayerData.PlayerData.Instance.Gender = gender;
         PlayerData.PlayerData.Instance.LobbyID = gamecode;

         var uid = Guid.NewGuid().ToString();

         PlayerData.PlayerData.Instance.RequestID = uid;
         
         var msg = new JoinRequestMessage(DateTime.Now.ToString("o"), gamecode, 0, playername,age,gender,uid );
         NetworkManager.Instance.SubscribeToSubject(gamecode.ToString());
         NetworkManager.Instance.Publish(gamecode.ToString(),msg);
         
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
         if (playername != "" && age != -1 && gender != -1 && gamecode != -1)
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
