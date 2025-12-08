using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Version1.Phases
{
    public interface IPhaseController
    {
        void StartPhase();
        void StopPhase();
    }
}
