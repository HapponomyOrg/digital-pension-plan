using System.Collections.Generic;
using UnityEngine;
using System.Text;
using NATS;
using System;

public class testscript2 : MonoBehaviour
{
    
    private void Start()
    {
        new NatsClient();

        //NatsClient.C.OnSellRequest += (sender, msg) => { print(msg.ToString()); };
    }

    private void Update()
    {
        NatsClient.C.HandleMessages();

        if (Input.GetKeyDown(KeyCode.P))
        {

            // SellRequestMessage msg = new SellRequestMessage(1,25,DateTime.UtcNow.ToString(),"TEST",4,2500);

            // NatsClient.C.Publish(msg, true);
        }

    }
}
