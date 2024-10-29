using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerTestScript : MonoBehaviour
{
   [SerializeField] private Timer timer;
    private void Start()
    {
        new NatsClient();
        NatsClient.C.StartHeartbeat();

        NatsClient.C.OnStartRound += (sender, msg) =>
        {
            Debug.Log("StartTimer");
            timer.StartTimer(msg.Duration);
        };
        NatsClient.C.OnStopRound += (sender, msg) =>
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
        
        
        
        NatsClient.C.HandleMessages();
    }
    private void OnDestroy()
    {
        NatsClient.C.StopHeartbeat();
    }

    private void OnDisable()
    {
        NatsClient.C.StopHeartbeat();
    }
}