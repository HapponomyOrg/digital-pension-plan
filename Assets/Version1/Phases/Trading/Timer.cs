using System;
using TMPro;
using UnityEngine;

namespace Version1.Phases.Trading
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerDisplay;

        private float timeLeftInSeconds;
        private bool started;


        public void Init(int seconds)
        {
            timeLeftInSeconds = seconds;
            DisplayTimer();
        }

        public void StartTimer()
        {
            started = true;
        }

        private void Update()
        {
            if (!started)
                return;

            if (timeLeftInSeconds <= 0)
                return;

            timeLeftInSeconds -= Time.deltaTime;
            DisplayTimer();
        }

        public void StopTimer()
        {
            started = false;
        }

        private void DisplayTimer()
        {
            if (!timerDisplay)
                return;

            var time = (int)timeLeftInSeconds;

            var seconds = time % 60;
            var minutes = (time - seconds) / 60;

            timerDisplay.text = seconds < 10 ? $"{minutes}:0{seconds}" : $"{minutes}:{seconds}";
        }
    }
}

