using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Version1.Phases
{
    public interface IPhaseManager
    {
        IPhase[] Phases { get; }
        IPhaseController CurrentPhaseController { get; set; }

        void StartPhases();
        void LoadPhase(int index, string name);
        void EndPhases();
    }
}
