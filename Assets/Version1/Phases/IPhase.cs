using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Version1.Phases
{
    public interface IPhase
    {
        string Name { get; }
        string Scene { get; }
    }
}
