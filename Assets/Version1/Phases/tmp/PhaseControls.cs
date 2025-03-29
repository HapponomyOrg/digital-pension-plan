using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Version1.Utilities;
using Version1.Phases;
using Version1.Phases.tmp;
using Version1.Phases.Trading;

namespace Version1
{

    public class PhaseControls : MonoBehaviour
    {
        private Phase phase = new TradingPhase();

        private int phaseCount;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadTest()
        {
            phase.Init();
        }

        public void LoadNextPhase()
        {
            // Utilities.GameManager.Instance.InitPhase(phaseCount++);
        }
    }
}