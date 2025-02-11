using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NATS;
using System;
[Obsolete]
public class RestartGame : MonoBehaviour
{
    [SerializeField] private GameObject createSession;
    [SerializeField] private GameObject endDonatingButton;
    [SerializeField] private GameObject endGameButton;

    public void StopDonation()
    {
        endDonatingButton.SetActive(false);
        endGameButton.SetActive(true);
        EndGameMessage endGameMessage =
            new EndGameMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), endGameMessage);
    }

    public void ResetGame()
    {
        endDonatingButton.SetActive(true);
        endGameButton.SetActive(false);
        this.gameObject.SetActive(false);
        createSession.SetActive(true);
    }
}