using Assets.Version1.Phases;
using Version1.Nats.Messages.Host;
using Version1.Phases;

namespace Tests.Utilities
{
    public class MockPhaseManager : IPhaseManager
    {
        private int _roundNumber;

        public MockPhaseManager(int roundNumber = 1)
        {
            _roundNumber = roundNumber;
        }

        public void SetRoundNumber(int round) => _roundNumber = round;

        // IPhaseManager implementation
        public IPhase[] Phases { get; set; }
        public IPhaseController CurrentPhaseController { get; set; }

        public int GetRoundNumber() => _roundNumber;


        // Unused in tests — left as no-ops
        public int CurrentRound()
        {
            throw new System.NotImplementedException();
        }

        public void StartPhases(StartGameMessage msg) { }
        public void LoadPhase(int index, string name) { }
        public void LoadNextPhase()
        {
            throw new System.NotImplementedException();
        }

        public void EndPhases() { }
    }
}
