using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Version1.Phases.BalanceModification
{
    public class BalanceModificationPhase : Phase
    {
        private const string balanceModificationScene = "BalanceModificationScene";

        public override event EventHandler<EventArgs> InitFinished;
        public override bool InitComplete { get; protected set; }


        public override void Init()
        {
            SceneManager.LoadScene(balanceModificationScene);
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
