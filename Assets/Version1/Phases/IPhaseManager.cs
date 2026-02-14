using Assets.Version1.Phases;
using Version1.Nats.Messages.Host;

namespace Version1.Phases
{
    public interface IPhaseManager
    {
        IPhase[] Phases { get; set; }
        IPhaseController CurrentPhaseController { get; set; }

        void StartPhases(StartGameMessage msg);
        void LoadPhase(int index, string name);
        void EndPhases();
    }
}
