using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class TimerTestScript : MonoBehaviour
{
    [SerializeField] private Timer timer;
    private void Start()
    {
        new NatsClient();
        NatsClient.Instance.StartHeartbeat();

        NatsClient.Instance.OnStartRound += (sender, msg) =>
        {
            Debug.Log("StartTimer");
            timer.StartTimer(msg.Duration);
        };
        NatsClient.Instance.OnStopRound += (sender, msg) =>
        {
            Debug.Log("StopTimer");
            timer.StopTimer();
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("StartTimer");
            timer.StartTimer(15);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("StopTimer");
            timer.PauseTimer();
        }



        NatsClient.Instance.HandleMessages();
    }
    private void OnDestroy()
    {
        NatsClient.Instance.StopHeartbeat();
    }

    private void OnDisable()
    {
        NatsClient.Instance.StopHeartbeat();
    }
}
