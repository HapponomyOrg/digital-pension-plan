using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NATS;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    [SerializeField] private bool player1;

    // Start is called before the first frame update
    void Start()
    {
        new NatsClient();

        //TODO() change this to generate player id automatically
        PlayerManager.Instance.PlayerId = (byte)(player1 ? 0 : 1);

        /*NatsClient.C.on += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnDonate += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnBalanceUpdate += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnBuyRequest += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnDeptUpdate += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnInterestUpdate += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnPointUpdate += (sender, msg) => { print(msg.ToString()); };
        NatsClient.C.OnCardHandIn += (sender, msg) => { print(msg.ToString()); };*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Q))
            NatsClient.C.Publish("sessionID",
                new SellRequestMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), "SELL_TEST", 4, 2500));
        if (Input.GetKeyDown(KeyCode.W))
            NatsClient.C.Publish("sessionID",
                new DonatePointsMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), (byte)(player1 ? 1 : 0), 1400));
        if (Input.GetKeyDown(KeyCode.E))
            NatsClient.C.Publish("sessionID",
                new BalanceUpdateMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), "TRANS_TEST", 4500));
        if (Input.GetKeyDown(KeyCode.R))
            NatsClient.C.Publish("sessionID",
                new BuyRequestMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), "BUY_TEST", 4,
                    (byte)(player1 ? 1 : 0)));
        if (Input.GetKeyDown(KeyCode.T))
            NatsClient.C.Publish("sessionID",
                new DeptUpdateMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), 420));
        if (Input.GetKeyDown(KeyCode.Y))
            NatsClient.C.Publish("sessionID",
                new InterestUpdateMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), 600));
        if (Input.GetKeyDown(KeyCode.U))
            NatsClient.C.Publish("sessionID",
                new PointUpdateMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1), 40,
                    (byte)(player1 ? 1 : 0)));
        if (Input.GetKeyDown(KeyCode.I))
            NatsClient.C.Publish("sessionID",
                new CardHandInMessage(DateTime.UtcNow.ToString(), 1, (byte)(player1 ? 0 : 1),
                    new int[] { 1, 4, 8, 2 }));
                    */

        NatsClient.Instance.HandleMessages();
    }
}