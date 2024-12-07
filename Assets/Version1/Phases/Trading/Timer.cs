using System;
using UnityEngine;

namespace Version1.Phases.Trading
{
    public class Timer : MonoBehaviour
    {
        private float timeLeftInSeconds;
        private bool started;
        
        
        public void Init(int seconds)
        {
            timeLeftInSeconds = seconds;
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
        }
        
        public void StopTimer()
        {
            started = false;
        }
    }
}

