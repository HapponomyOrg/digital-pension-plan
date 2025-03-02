using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class Timer : MonoBehaviour
{
    [SerializeField] public int TotalRoundTime = 240;
    
    [SerializeField] private TMP_Text RoundTimerText;
    [SerializeField] private Image TimerImage;


    private float timer;
    public bool IsPaused = false;
    public bool IsStopped = false;

    public IEnumerator StartTimer(int roundTime)
    {
        TotalRoundTime = roundTime;
        timer = TotalRoundTime;
        IsPaused = false;
        if (!IsStopped) yield return StartCoroutine(TimerEnumerator());
    }

    public void StopTimer()
    {
        IsPaused = true;
        IsStopped = true;
        timer = TotalRoundTime;
    }

    public void PauseTimer()
    {
        IsPaused = true;
        IsStopped = false;
    }

    IEnumerator TimerEnumerator()
    {
        while (timer > 0f)
        {
            if (!IsPaused)
            {
                int minutes = Mathf.FloorToInt(timer / 60f);
                int seconds = Mathf.FloorToInt(timer % 60f);

                if (RoundTimerText)
                {
                    if (timer < 11 && Mathf.FloorToInt(timer % 2) == 0)
                    {
                        RoundTimerText.color = Color.red;
                    }
                    else
                    {
                        RoundTimerText.color = Color.white;   
                    }

                    RoundTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                if (TimerImage)
                {
                    TimerImage.fillAmount = Mathf.InverseLerp(0, TotalRoundTime, timer);
                }

                timer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}