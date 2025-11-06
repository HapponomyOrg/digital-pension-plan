using System;
using System.Collections;
using System.Collections.Generic;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class JoinSession : MonoBehaviour
{
    [SerializeField] Button Button;

    [SerializeField] TMP_InputField LobbyInput;

    [SerializeField] TMP_InputField NameInput;

    private void Start()
    {
        new NatsClient();
        NatsClient.Instance.OnConfirmJoin += (sender, msg) =>
        {
            print("ConfirmMessage");
            print(msg.ToString());
        };
    }

    /*public void onClick()
    {
        Button.interactable = false;
        NatsClient.C.SubscribeToSubject(LobbyInput.text);
        JoinRequestMessage msg =
            new JoinRequestMessage(DateTime.UtcNow.ToString(), int.Parse(LobbyInput.text), 0, NameInput.text);
        NatsClient.C.Publish(LobbyInput.text, msg);s
    }*/

    private void Update()
    {
        NatsClient.Instance.HandleMessages();
    }
}
