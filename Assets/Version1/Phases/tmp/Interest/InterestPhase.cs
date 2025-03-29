using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Phases.tmp;

namespace Version1.Phases.Interest
{
    public class InterestPhase : Phase
    {
        private const string interestScene = "InterestScene";

        public override event EventHandler<EventArgs> InitFinished;
        public override bool InitComplete { get; protected set; }


        public override void Init()
        {
            SceneManager.LoadScene(interestScene);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                InitFinished?.Invoke(this, EventArgs.Empty);
                InitFinished = null;
                InitComplete = true;
            };
        }

        public override void Start()
        {
            //throw new NotImplementedException();
        }

        public override void End()
        {
            //throw new NotImplementedException();
        }
    }
}
